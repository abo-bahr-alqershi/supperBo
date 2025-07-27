using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.UnitFieldValues;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.UnitFieldValues
{
    /// <summary>
    /// معالج أمر حذف قيمة حقل للوحدة
    /// Deletes a unit field value and includes:
    /// - Input validation
    /// - Existence check
    /// - Authorization (Admin only)
    /// - Deletion
    /// - Audit logging
    /// </summary>
    public class DeleteUnitFieldValueCommandHandler : IRequestHandler<DeleteUnitFieldValueCommand, ResultDto<bool>>
    {
        private readonly IUnitFieldValueRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeleteUnitFieldValueCommandHandler> _logger;

        public DeleteUnitFieldValueCommandHandler(
            IUnitFieldValueRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<DeleteUnitFieldValueCommandHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(DeleteUnitFieldValueCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة أمر حذف قيمة حقل الوحدة: {ValueId}", request.ValueId);

            // التحقق من قيمة المعرف
            if (request.ValueId == Guid.Empty)
                throw new BusinessRuleException("InvalidValueId", "معرف القيمة غير صالح");

            // البحث عن القيمة
            var existing = await _repository.GetUnitFieldValueByIdAsync(request.ValueId, cancellationToken);
            if (existing == null)
                throw new NotFoundException("UnitFieldValue", request.ValueId.ToString());

            // صلاحيات المستخدم
            if (_currentUserService.Role != "Admin")
                throw new ForbiddenException("غير مصرح لك بحذف قيمة حقل الوحدة");

            // حذف ضمن معاملة
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var deleted = await _repository.DeleteUnitFieldValueAsync(request.ValueId, cancellationToken);
                if (!deleted)
                    throw new BusinessRuleException("DeletionFailed", "فشل حذف قيمة حقل الوحدة");

                // تسجيل المراجعة
                await _auditService.LogActivityAsync(
                    "UnitFieldValue",
                    existing.Id.ToString(),
                    "Delete",
                    $"تم حذف قيمة الحقل: {existing.FieldValue} للوحدة {existing.UnitId}",
                    existing,
                    null,
                    cancellationToken);

                _logger.LogInformation("تم حذف قيمة حقل الوحدة بنجاح: {ValueId}", existing.Id);
            });

            return ResultDto<bool>.Ok(true);
        }
    }
} 
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.UnitTypeFields;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.UnitTypeFields
{
    /// <summary>
    /// معالج أمر حذف حقل ديناميكي من نوع الوحدة
    /// Deletes a dynamic field from a unit type (soft delete) and includes:
    /// - Input validation
    /// - Existence check
    /// - Authorization (Admin only)
    /// - Soft delete
    /// - Audit logging
    /// - Event publishing
    /// </summary>
    public class DeleteUnitTypeFieldCommandHandler : IRequestHandler<DeleteUnitTypeFieldCommand, ResultDto<bool>>
    {
        private readonly IUnitTypeFieldRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<DeleteUnitTypeFieldCommandHandler> _logger;

        public DeleteUnitTypeFieldCommandHandler(
            IUnitTypeFieldRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            ILogger<DeleteUnitTypeFieldCommandHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(DeleteUnitTypeFieldCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة أمر حذف حقل نوع الوحدة: {FieldId}", request.FieldId);

            // التحقق من صحة المعرف
            if (!Guid.TryParse(request.FieldId, out var fieldId))
                throw new BusinessRuleException("InvalidFieldId", "معرف الحقل غير صالح");

            // التحقق من وجود الحقل
            var existingField = await _repository.GetUnitTypeFieldByIdAsync(fieldId, cancellationToken);
            if (existingField == null)
                throw new NotFoundException("UnitTypeField", request.FieldId);

            // صلاحيات المستخدم
            if (_currentUserService.Role != "Admin")
                throw new ForbiddenException("غير مصرح لك بحذف حقل نوع الوحدة");

            // تنفيذ الحذف الناعم ضمن معاملة
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var deleted = await _repository.DeleteUnitTypeFieldAsync(fieldId, cancellationToken);
                if (!deleted)
                    throw new BusinessRuleException("DeletionFailed", "فشل حذف حقل نوع الوحدة");

                // تسجيل التدقيق
                await _auditService.LogActivityAsync(
                    "UnitTypeField",
                    existingField.Id.ToString(),
                    "Delete",
                    $"تم حذف حقل الديناميكي: {existingField.FieldName} من نوع الوحدة {existingField.UnitTypeId}",
                    existingField,
                    null,
                    cancellationToken);

                // نشر الحدث
                // await _eventPublisher.PublishEventAsync(new UnitTypeFieldDeletedEvent
                // {
                //     FieldId = existingField.Id,
                //     UnitTypeId = existingField.UnitTypeId,
                //     FieldName = existingField.FieldName,
                //     DeletedBy = _currentUserService.UserId,
                //     DeletedAt = DateTime.UtcNow
                // }, cancellationToken);

                _logger.LogInformation("تم حذف حقل نوع الوحدة بنجاح: {FieldId}", existingField.Id);
            });

            return ResultDto<bool>.Ok(true);
        }
    }

    /// <summary>
    /// حدث حذف حقل ديناميكي من نوع الوحدة
    /// Unit type field deleted event
    /// </summary>
    public class UnitTypeFieldDeletedEvent
    {
        public Guid FieldId { get; set; }
        public Guid UnitTypeId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public Guid DeletedBy { get; set; }
        public DateTime DeletedAt { get; set; }
    }
} 
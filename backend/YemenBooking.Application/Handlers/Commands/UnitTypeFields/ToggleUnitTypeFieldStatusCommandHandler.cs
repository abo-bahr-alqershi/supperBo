using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.UnitTypeFields;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Handlers.Commands.UnitTypeFields
{
    /// <summary>
    /// معالج أمر تغيير حالة تفعيل حقل ديناميكي
    /// Toggle active status for a unit type field
    /// </summary>
    public class ToggleUnitTypeFieldStatusCommandHandler : IRequestHandler<ToggleUnitTypeFieldStatusCommand, ResultDto<bool>>
    {
        private readonly IUnitTypeFieldRepository _fieldRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<ToggleUnitTypeFieldStatusCommandHandler> _logger;

        public ToggleUnitTypeFieldStatusCommandHandler(
            IUnitTypeFieldRepository fieldRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            ILogger<ToggleUnitTypeFieldStatusCommandHandler> logger)
        {
            _fieldRepository = fieldRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(ToggleUnitTypeFieldStatusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تغيير حالة الحقل {FieldId} إلى {IsActive}", request.FieldId, request.IsActive);

            if (!Guid.TryParse(request.FieldId, out var fieldId))
                throw new BusinessRuleException("InvalidFieldId", "معرف الحقل غير صالح");

            var existing = await _fieldRepository.GetUnitTypeFieldByIdAsync(fieldId, cancellationToken);
            if (existing == null)
                throw new NotFoundException("UnitTypeField", request.FieldId);

            if (_currentUserService.Role != "Admin")
                throw new ForbiddenException("غير مصرح لك بتغيير حالة الحقل");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                existing.IsActive = request.IsActive;
                existing.UpdatedBy = _currentUserService.UserId;
                existing.UpdatedAt = DateTime.UtcNow;
                await _fieldRepository.UpdateUnitTypeFieldAsync(existing, cancellationToken);

                await _auditService.LogActivityAsync(
                    "UnitTypeField",
                    existing.Id.ToString(),
                    "ToggleStatus",
                    $"تم تغيير حالة الحقل إلى {(request.IsActive ? "مفعّل" : "معطّل")}",
                    null,
                    null,
                    cancellationToken);

                // await _eventPublisher.PublishEventAsync(new UnitTypeFieldStatusToggledEvent
                // {
                //     FieldId = existing.Id,
                //     IsActive = request.IsActive,
                //     ToggledBy = _currentUserService.UserId,
                //     ToggledAt = DateTime.UtcNow
                // }, cancellationToken);

                _logger.LogInformation("تم تغيير حالة الحقل بنجاح: {FieldId}", existing.Id);
            });

            return ResultDto<bool>.Ok(true, "تم تغيير حالة الحقل بنجاح");
        }
    }

    /// <summary>
    /// حدث تغيير حالة تفعيل حقل ديناميكي
    /// Unit type field status toggled event
    /// </summary>
    public class UnitTypeFieldStatusToggledEvent
    {
        public Guid FieldId { get; set; }
        public bool IsActive { get; set; }
        public Guid ToggledBy { get; set; }
        public DateTime ToggledAt { get; set; }
    }
} 
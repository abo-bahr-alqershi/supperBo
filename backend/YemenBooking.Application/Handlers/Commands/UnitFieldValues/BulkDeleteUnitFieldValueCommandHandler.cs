namespace YemenBooking.Application.Handlers.Commands.UnitFieldValues;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.UnitFieldValues;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;

/// <summary>
/// معالج أمر حذف جماعي لقيم حقول الوحدات
/// Performs bulk deletion of unit field values with:
/// - Input validation
/// - Existence check
/// - Deletion of values
/// - Audit logging
/// - Event publishing
/// </summary>
public class BulkDeleteUnitFieldValueCommandHandler : IRequestHandler<BulkDeleteUnitFieldValueCommand, ResultDto<bool>>
{
    private readonly IUnitFieldValueRepository _valueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<BulkDeleteUnitFieldValueCommandHandler> _logger;

    public BulkDeleteUnitFieldValueCommandHandler(
        IUnitFieldValueRepository valueRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        ILogger<BulkDeleteUnitFieldValueCommandHandler> logger)
    {
        _valueRepository = valueRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<ResultDto<bool>> Handle(BulkDeleteUnitFieldValueCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("بدء حذف جماعي لقيم حقول الوحدات");

        // التحقق من المدخلات
        if (request.ValueIds == null || request.ValueIds.Count == 0)
            throw new BusinessRuleException("EmptyValueIds", "يجب توفير معرفات القيم لحذفها");

        // تنفيذ المعاملة
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            foreach (var id in request.ValueIds)
            {
                if (id == Guid.Empty)
                    throw new BusinessRuleException("InvalidValueId", "معرف القيمة غير صالح");

                var existing = await _valueRepository.GetUnitFieldValueByIdAsync(id, cancellationToken);
                if (existing == null)
                    throw new NotFoundException("UnitFieldValue", id.ToString());

                await _valueRepository.DeleteUnitFieldValueAsync(id, cancellationToken);
            }

            // تسجيل التدقيق
            await _auditService.LogActivityAsync(
                "UnitFieldValue",
                string.Join(",", request.ValueIds),
                "BulkDelete",
                $"تم حذف قيم حقول الوحدات: {string.Join(",", request.ValueIds)}",
                null,
                null,
                cancellationToken);

            // نشر الحدث (اختياري)
            // await _eventPublisher.PublishEventAsync(new UnitFieldValuesBulkDeletedEvent
            // {
            //     ValueIds = request.ValueIds,
            //     DeletedBy = _currentUserService.UserId,
            //     DeletedAt = DateTime.UtcNow
            // }, cancellationToken);

            _logger.LogInformation("اكتمل حذف القيم بنجاح");
        });

        return ResultDto<bool>.Ok(true);
    }
}

/// <summary>
/// حدث حذف جماعي لقيم حقول الوحدة
/// Event for bulk deletion of unit field values
/// </summary>
public class UnitFieldValuesBulkDeletedEvent
{
    public IEnumerable<Guid> ValueIds { get; set; } = new List<Guid>();
    public Guid DeletedBy { get; set; }
    public DateTime DeletedAt { get; set; }
} 
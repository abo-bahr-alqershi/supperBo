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
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;

/// <summary>
/// معالج أمر إنشاء جماعي لقيم حقول الوحدات
/// Performs bulk creation of unit field values with:
/// - Input validation
/// - Existence check
/// - Creation of values
/// - Audit logging
/// - Event publishing
/// </summary>
public class BulkCreateUnitFieldValueCommandHandler : IRequestHandler<BulkCreateUnitFieldValueCommand, ResultDto<bool>>
{
    private readonly IUnitFieldValueRepository _valueRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<BulkCreateUnitFieldValueCommandHandler> _logger;

    public BulkCreateUnitFieldValueCommandHandler(
        IUnitFieldValueRepository valueRepository,
        IUnitRepository unitRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        ILogger<BulkCreateUnitFieldValueCommandHandler> logger)
    {
        _valueRepository = valueRepository;
        _unitRepository = unitRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<ResultDto<bool>> Handle(BulkCreateUnitFieldValueCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("بدء إنشاء جماعي لقيم حقول الوحدة {UnitId}", request.UnitId);

        // التحقق من وجود الوحدة
        var unit = await _unitRepository.GetUnitByIdAsync(request.UnitId, cancellationToken);
        if (unit == null)
            throw new NotFoundException("Unit", request.UnitId.ToString());

        // التحقق من المدخلات
        if (request.FieldValues == null || request.FieldValues.Count == 0)
            throw new BusinessRuleException("EmptyFieldValues", "يجب توفير قيم الحقول لإنشائها");

        // تنفيذ المعاملة
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            foreach (var dto in request.FieldValues)
            {
                if (dto.FieldId == Guid.Empty)
                    throw new BusinessRuleException("InvalidFieldId", "معرف الحقل غير صالح");

                var newValue = new UnitFieldValue
                {
                    UnitId = request.UnitId,
                    UnitTypeFieldId = dto.FieldId,
                    FieldValue = dto.FieldValue,
                    CreatedBy = _currentUserService.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                await _valueRepository.CreateUnitFieldValueAsync(newValue, cancellationToken);
            }

            // تسجيل التدقيق
            await _auditService.LogActivityAsync(
                "UnitFieldValue",
                request.UnitId.ToString(),
                "BulkCreate",
                $"تم إنشاء قيم حقول الوحدة {request.UnitId}",
                null,
                null,
                cancellationToken);

            // نشر الحدث (اختياري)
            // await _eventPublisher.PublishEventAsync(new UnitFieldValuesBulkCreatedEvent
            // {
            //     UnitId = request.UnitId,
            //     CreatedBy = _currentUserService.UserId,
            //     CreatedAt = DateTime.UtcNow
            // }, cancellationToken);

            _logger.LogInformation("اكتمل إنشاء قيم حقول الوحدة بنجاح: {UnitId}", request.UnitId);
        });

        return ResultDto<bool>.Ok(true);
    }
}

/// <summary>
/// حدث إنشاء جماعي لقيم حقول الوحدة
/// Event for bulk creation of unit field values
/// </summary>
public class UnitFieldValuesBulkCreatedEvent
{
    public Guid UnitId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
} 
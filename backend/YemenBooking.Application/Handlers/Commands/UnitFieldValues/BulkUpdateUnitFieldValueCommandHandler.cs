namespace YemenBooking.Application.Handlers.Commands.UnitFieldValues;

using System;
using System.Linq;
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
/// معالج أمر تحديث جماعي لقيم حقول الوحدات
/// Performs bulk update of unit field values with:
/// - Input validation
/// - Existence check
/// - Creation or update of values
/// - Audit logging
/// - Event publishing
/// </summary>
public class BulkUpdateUnitFieldValueCommandHandler : IRequestHandler<BulkUpdateUnitFieldValueCommand, ResultDto<bool>>
{
    private readonly IUnitFieldValueRepository _valueRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<BulkUpdateUnitFieldValueCommandHandler> _logger;

    public BulkUpdateUnitFieldValueCommandHandler(
        IUnitFieldValueRepository valueRepository,
        IUnitRepository unitRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        ILogger<BulkUpdateUnitFieldValueCommandHandler> logger)
    {
        _valueRepository = valueRepository;
        _unitRepository = unitRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<ResultDto<bool>> Handle(BulkUpdateUnitFieldValueCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("بدء تحديث جماعي لقيم حقول الوحدة {UnitId}", request.UnitId);

        // التحقق من وجود الوحدة
        var unit = await _unitRepository.GetUnitByIdAsync(request.UnitId, cancellationToken);
        if (unit == null)
            throw new NotFoundException("Unit", request.UnitId.ToString());

        // التحقق من المدخلات
        if (request.FieldValues == null || !request.FieldValues.Any())
            throw new BusinessRuleException("EmptyFieldValues", "يجب توفير قيم الحقول لتحديثها");

        // تنفيذ المعاملة
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var existingValues = (await _valueRepository.GetValuesByUnitIdAsync(request.UnitId, cancellationToken)).ToList();

            foreach (var dto in request.FieldValues)
            {
                if (dto.FieldId == Guid.Empty)
                    throw new BusinessRuleException("InvalidFieldId", "معرف الحقل غير صالح");

                var existing = existingValues.FirstOrDefault(v => v.UnitTypeFieldId == dto.FieldId);
                if (existing != null)
                {
                    existing.FieldValue = dto.FieldValue;
                    existing.UpdatedBy = _currentUserService.UserId;
                    existing.UpdatedAt = DateTime.UtcNow;
                    await _valueRepository.UpdateUnitFieldValueAsync(existing, cancellationToken);
                }
                else
                {
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
            }

            // تسجيل التدقيق
            await _auditService.LogActivityAsync(
                "UnitFieldValue",
                request.UnitId.ToString(),
                "BulkUpdate",
                $"تم تحديث قيم حقول الوحدة {request.UnitId}",
                null,
                null,
                cancellationToken);

            // نشر الحدث (اختياري)
            // await _eventPublisher.PublishEventAsync(new UnitFieldValuesBulkUpdatedEvent
            // {
            //     UnitId = request.UnitId,
            //     UpdatedBy = _currentUserService.UserId,
            //     UpdatedAt = DateTime.UtcNow
            // }, cancellationToken);

            _logger.LogInformation("اكتمل تحديث قيم حقول الوحدة بنجاح: {UnitId}", request.UnitId);
        });

        return ResultDto<bool>.Ok(true);
    }
} 
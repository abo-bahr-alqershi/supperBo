using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.UnitFieldValues;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using Unit = MediatR.Unit;
using YemenBooking.Core.Events;

namespace YemenBooking.Application.Handlers.Commands.UnitFieldValues;

/// <summary>
/// معالج أمر تحديث قيمة حقل الوحدة مع الفهرسة التلقائية
/// Update unit field value command handler with automatic indexing
/// </summary>
public class UpdateUnitFieldValueCommandHandler : IRequestHandler<UpdateUnitFieldValueCommand, ResultDto<Guid>>
{
    private readonly IUnitFieldValueRepository _unitFieldValueRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitTypeFieldRepository _unitTypeFieldRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly IIndexingService _indexingService;
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateUnitFieldValueCommandHandler> _logger;

    public UpdateUnitFieldValueCommandHandler(
        IUnitFieldValueRepository unitFieldValueRepository,
        IUnitRepository unitRepository,
        IUnitTypeFieldRepository unitTypeFieldRepository,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        IIndexingService indexingService,
        IMediator mediator,
        ILogger<UpdateUnitFieldValueCommandHandler> logger)
    {
        _unitFieldValueRepository = unitFieldValueRepository;
        _unitRepository = unitRepository;
        _unitTypeFieldRepository = unitTypeFieldRepository;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _indexingService = indexingService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<ResultDto<Guid>> Handle(UpdateUnitFieldValueCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء تحديث قيمة حقل الوحدة: {FieldValueId}", request.Id);

            // التحقق من صحة المدخلات
            if (request.Id == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف قيمة الحقل مطلوب");
            if (string.IsNullOrWhiteSpace(request.Value))
                return ResultDto<Guid>.Failed("قيمة الحقل مطلوبة");

            // التحقق من وجود قيمة الحقل
            var fieldValue = await _unitFieldValueRepository.GetByIdAsync(request.Id, cancellationToken);
            if (fieldValue == null)
                return ResultDto<Guid>.Failed("قيمة الحقل غير موجودة");

            // التحقق من وجود الوحدة
            var unit = await _unitRepository.GetByIdAsync(fieldValue.UnitId, cancellationToken);
            if (unit == null)
                return ResultDto<Guid>.Failed("الوحدة غير موجودة");

            // التحقق من وجود حقل نوع الوحدة
            var unitTypeField = await _unitTypeFieldRepository.GetByIdAsync(fieldValue.UnitTypeFieldId, cancellationToken);
            if (unitTypeField == null)
                return ResultDto<Guid>.Failed("حقل نوع الوحدة غير موجود");

            // التحقق من الصلاحيات
            if (!await HasPermissionToManageUnit(unit))
                return ResultDto<Guid>.Failed("ليس لديك صلاحية لإدارة هذه الوحدة");

            // حفظ القيمة القديمة للمراجعة
            var oldValue = fieldValue.FieldValue;

            // تحديث القيمة
            fieldValue.FieldValue = request.Value;
            fieldValue.UpdatedBy = _currentUserService.UserId;
            fieldValue.UpdatedAt = DateTime.UtcNow;

            // حفظ التغييرات
            await _unitFieldValueRepository.UpdateAsync(fieldValue, cancellationToken);

            // تسجيل العملية في المراجعة
            await _auditService.LogAsync(
                "UpdateUnitFieldValue",
                fieldValue.Id.ToString(),
                $"تم تحديث قيمة حقل الوحدة {unit.Name}: من '{oldValue}' إلى '{request.Value}'",
                _currentUserService.UserId);

            // إرسال حدث الفهرسة الديناميكية
            await PublishIndexingEvent(fieldValue, unitTypeField, "update");

            _logger.LogInformation("تم تحديث قيمة حقل الوحدة بنجاح: {Id}", fieldValue.Id);

            return ResultDto<Guid>.Ok(fieldValue.Id, "تم تحديث قيمة الحقل بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث قيمة حقل الوحدة: {FieldValueId}", request.Id);
            return ResultDto<Guid>.Failed("حدث خطأ أثناء تحديث قيمة الحقل");
        }
    }

    /// <summary>
    /// التحقق من صلاحيات إدارة الوحدة
    /// Check unit management permissions
    /// </summary>
    private async Task<bool> HasPermissionToManageUnit(Core.Entities.Unit unit)
    {
        var currentUserId = _currentUserService.UserId;
        var userRole = _currentUserService.Role;

        // المسؤول له صلاحية على كل شيء
        if (userRole == "Admin")
            return true;

        // المالك له صلاحية على عقاراته
        if (unit.Property.OwnerId == currentUserId)
            return true;

        // الموظف له صلاحية على العقارات التي يعمل بها
        if (userRole == "Staff")
        {
            // يمكن إضافة منطق للتحقق من صلاحيات الموظف
            return true;
        }

        return false;
    }

    /// <summary>
    /// إرسال حدث الفهرسة
    /// Publish indexing event
    /// </summary>
    private async Task PublishIndexingEvent(Core.Entities.UnitFieldValue fieldValue, Core.Entities.UnitTypeField unitTypeField, string operation)
    {
        try
        {
            var indexingEvent = new DynamicFieldIndexingEvent
            {
                FieldId = fieldValue.Id,
                FieldName = unitTypeField.FieldName,
                FieldType = unitTypeField.FieldTypeId,
                FieldValue = fieldValue.FieldValue,
                EntityId = fieldValue.UnitId,
                EntityType = "Unit",
                Operation = operation,
                UserId = _currentUserService.UserId,
                CorrelationId = Guid.NewGuid().ToString(),
                AdditionalData = new Dictionary<string, object>
                {
                    { "UnitTypeFieldId", fieldValue.UnitTypeFieldId },
                    { "UpdatedBy", fieldValue.UpdatedBy },
                    { "UpdatedAt", fieldValue.UpdatedAt }
                }
            };

            await _mediator.Publish(indexingEvent);
            _logger.LogDebug("تم إرسال حدث فهرسة الحقل الديناميكي: {FieldName}", unitTypeField.FieldName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "فشل في إرسال حدث فهرسة الحقل الديناميكي: {FieldName}", unitTypeField.FieldName);
        }
    }
}

/// <summary>
/// حدث تحديث قيمة حقل الوحدة
/// Unit field value updated event
/// </summary>
public class UnitFieldValueUpdatedEvent
{
    /// <summary>
    /// معرف القيمة
    /// Value ID
    /// </summary>
    public Guid ValueId { get; set; }

    /// <summary>
    /// معرف الوحدة
    /// Unit ID
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// معرف الحقل
    /// Field ID
    /// </summary>
    public Guid FieldId { get; set; }

    /// <summary>
    /// اسم الحقل
    /// Field name
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// القيمة القديمة للحقل
    /// Old field value
    /// </summary>
    public string OldFieldValue { get; set; } = string.Empty;

    /// <summary>
    /// القيمة الجديدة للحقل
    /// New field value
    /// </summary>
    public string NewFieldValue { get; set; } = string.Empty;

    /// <summary>
    /// معرف المحدث
    /// Updated by user ID
    /// </summary>
    public Guid UpdatedBy { get; set; }

    /// <summary>
    /// تاريخ التحديث
    /// Update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

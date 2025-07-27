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
using YemenBooking.Core.Events;

namespace YemenBooking.Application.Handlers.Commands.UnitFieldValues;

/// <summary>
/// معالج أمر إنشاء قيمة حقل جديدة للوحدة مع الفهرسة التلقائية
/// Create unit field value command handler with automatic indexing
/// </summary>
public class CreateUnitFieldValueCommandHandler : IRequestHandler<CreateUnitFieldValueCommand, ResultDto<Guid>>
{
    private readonly IUnitFieldValueRepository _unitFieldValueRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitTypeFieldRepository _unitTypeFieldRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly IIndexingService _indexingService;
    private readonly IMediator _mediator;
    private readonly ILogger<CreateUnitFieldValueCommandHandler> _logger;

    public CreateUnitFieldValueCommandHandler(
        IUnitFieldValueRepository unitFieldValueRepository,
        IUnitRepository unitRepository,
        IUnitTypeFieldRepository unitTypeFieldRepository,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        IIndexingService indexingService,
        IMediator mediator,
        ILogger<CreateUnitFieldValueCommandHandler> logger)
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

    public async Task<ResultDto<Guid>> Handle(CreateUnitFieldValueCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء إنشاء قيمة حقل جديدة للوحدة: {UnitId}, {FieldId}", 
                request.UnitId, request.UnitTypeFieldId);

            // التحقق من صحة المدخلات
            if (request.UnitId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف الوحدة مطلوب");
            if (request.UnitTypeFieldId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف حقل نوع الوحدة مطلوب");
            if (string.IsNullOrWhiteSpace(request.Value))
                return ResultDto<Guid>.Failed("قيمة الحقل مطلوبة");

            // التحقق من وجود الوحدة
            var unit = await _unitRepository.GetByIdAsync(request.UnitId, cancellationToken);
            if (unit == null)
                return ResultDto<Guid>.Failed("الوحدة غير موجودة");

            // التحقق من وجود حقل نوع الوحدة
            var unitTypeField = await _unitTypeFieldRepository.GetByIdAsync(request.UnitTypeFieldId, cancellationToken);
            if (unitTypeField == null)
                return ResultDto<Guid>.Failed("حقل نوع الوحدة غير موجود");

            // التحقق من الصلاحيات - يجب أن يكون المستخدم مالك العقار أو مسؤول
            if (!await HasPermissionToManageUnit(unit))
                return ResultDto<Guid>.Failed("ليس لديك صلاحية لإدارة هذه الوحدة");

            // التحقق من عدم وجود قيمة موجودة للحقل نفسه
            var existingValues = await _unitFieldValueRepository.GetValuesByUnitIdAsync(request.UnitId, cancellationToken);
            var existingValue = existingValues.FirstOrDefault(v => v.UnitTypeFieldId == request.UnitTypeFieldId);
            if (existingValue != null)
                return ResultDto<Guid>.Failed("يوجد قيمة موجودة لهذا الحقل، استخدم التحديث بدلاً من الإنشاء");

            // إنشاء قيمة الحقل الجديدة
            var unitFieldValue = new UnitFieldValue
            {
                Id = Guid.NewGuid(),
                UnitId = request.UnitId,
                UnitTypeFieldId = request.UnitTypeFieldId,
                FieldValue = request.Value,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // حفظ في قاعدة البيانات
            await _unitFieldValueRepository.AddAsync(unitFieldValue, cancellationToken);

            // تسجيل العملية في المراجعة
            await _auditService.LogAsync(
                "CreateUnitFieldValue",
                unitFieldValue.Id.ToString(),
                $"تم إنشاء قيمة حقل جديدة للوحدة {unit.Name}",
                _currentUserService.UserId);

            // إرسال حدث الفهرسة الديناميكية
            await PublishIndexingEvent(unitFieldValue, unitTypeField, "create");

            _logger.LogInformation("تم إنشاء قيمة حقل الوحدة بنجاح: {Id}", unitFieldValue.Id);

            return ResultDto<Guid>.Ok(unitFieldValue.Id, "تم إنشاء قيمة الحقل بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء قيمة حقل الوحدة");
            return ResultDto<Guid>.Failed("حدث خطأ أثناء إنشاء قيمة الحقل");
        }
    }

    /// <summary>
    /// التحقق من صلاحيات إدارة الوحدة
    /// Check unit management permissions
    /// </summary>
    private async Task<bool> HasPermissionToManageUnit(YemenBooking.Core.Entities.Unit unit)
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
    private async Task PublishIndexingEvent(UnitFieldValue fieldValue, UnitTypeField unitTypeField, string operation)
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
                    { "CreatedBy", fieldValue.CreatedBy },
                    { "CreatedAt", fieldValue.CreatedAt }
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
/// حدث إنشاء قيمة حقل الوحدة
/// Unit field value created event
/// </summary>
public class UnitFieldValueCreatedEvent
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
    /// قيمة الحقل
    /// Field value
    /// </summary>
    public string FieldValue { get; set; } = string.Empty;

    /// <summary>
    /// معرف المنشئ
    /// Created by user ID
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// تاريخ الإنشاء
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

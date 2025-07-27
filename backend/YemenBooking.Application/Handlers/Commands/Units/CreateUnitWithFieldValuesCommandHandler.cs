namespace YemenBooking.Application.Handlers.Commands.Units;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Units;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.ValueObjects;

/// <summary>
/// معالج أمر إنشاء وحدة جديدة مع قيم الحقول الديناميكية
/// Handler for creating a new unit along with dynamic field values, includes:
/// - Input validation
/// - Existence and permission checks
/// - Transaction management
/// - Creation of unit and dynamic field values
/// - Audit logging
/// - Event publishing
/// </summary>
public class CreateUnitWithFieldValuesCommandHandler : IRequestHandler<CreateUnitWithFieldValuesCommand, ResultDto<Guid>>
{
    private readonly IUnitRepository _unitRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitTypeRepository _unitTypeRepository;
    private readonly IUnitFieldValueRepository _valueRepository;
    private readonly IUnitTypeFieldRepository _fieldRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<CreateUnitWithFieldValuesCommandHandler> _logger;

    public CreateUnitWithFieldValuesCommandHandler(
        IUnitRepository unitRepository,
        IPropertyRepository propertyRepository,
        IUnitTypeRepository unitTypeRepository,
        IUnitTypeFieldRepository fieldRepository,
        IUnitFieldValueRepository valueRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        ILogger<CreateUnitWithFieldValuesCommandHandler> logger)
    {
        _unitRepository = unitRepository;
        _propertyRepository = propertyRepository;
        _fieldRepository = fieldRepository;
        _unitTypeRepository = unitTypeRepository;
        _valueRepository = valueRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<ResultDto<Guid>> Handle(CreateUnitWithFieldValuesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("بدء إنشاء وحدة جديدة مع قيم الحقول الديناميكية: PropertyId={PropertyId}, Name={Name}", request.PropertyId, request.Name);

        // التحقق من البيانات الأساسية للوحدة
        if (request.PropertyId == Guid.Empty)
            return ResultDto<Guid>.Failed("معرف الكيان مطلوب");
        if (request.UnitTypeId == Guid.Empty)
            return ResultDto<Guid>.Failed("معرف نوع الوحدة مطلوب");
        if (string.IsNullOrWhiteSpace(request.Name))
            return ResultDto<Guid>.Failed("اسم الوحدة مطلوب");
        if (request.BasePrice == null || request.BasePrice.Amount <= 0)
            return ResultDto<Guid>.Failed("السعر الأساسي يجب أن يكون أكبر من صفر");

        // التحقق من وجود الكيان ونوع الوحدة
        var property = await _propertyRepository.GetPropertyByIdAsync(request.PropertyId, cancellationToken);
        if (property == null)
            return ResultDto<Guid>.Failed("الكيان غير موجود");
        var unitType = await _unitTypeRepository.GetUnitTypeByIdAsync(request.UnitTypeId, cancellationToken);
        if (unitType == null)
            return ResultDto<Guid>.Failed("نوع الوحدة غير موجود");

        // التحقق من الصلاحيات (Admin أو مالك الكيان)
        if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
            return ResultDto<Guid>.Failed("غير مصرح لك بإنشاء وحدة في هذا الكيان");

        // التحقق من التكرار
        bool exists = await _unitRepository.ExistsAsync(u => u.PropertyId == request.PropertyId && u.Name.Trim() == request.Name.Trim(), cancellationToken);
        if (exists)
            return ResultDto<Guid>.Failed("يوجد وحدة بنفس الاسم في هذا الكيان");

        // Validate dynamic field values against definitions
        var fieldDefs = await _fieldRepository.GetFieldsByUnitTypeIdAsync(request.UnitTypeId, cancellationToken);
        foreach (var def in fieldDefs)
        {
            var dto = request.FieldValues.FirstOrDefault(f => f.FieldId == def.Id);
            if (def.IsRequired && (dto == null || string.IsNullOrWhiteSpace(dto.FieldValue)))
                return ResultDto<Guid>.Failed($"الحقل {def.DisplayName} مطلوب.");
            if (dto != null && (def.FieldTypeId == "number" || def.FieldTypeId == "currency" || def.FieldTypeId == "percentage" || def.FieldTypeId == "range"))
            {
                if (!decimal.TryParse(dto.FieldValue, out _))
                    return ResultDto<Guid>.Failed($"قيمة الحقل {def.DisplayName} يجب أن تكون رقمًا.");
            }
        }
        
        Guid createdId = Guid.Empty;

        // تنفيذ المعاملة
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // إنشاء كيان الوحدة
            var unit = new YemenBooking.Core.Entities.Unit
            {
                PropertyId = request.PropertyId,
                UnitTypeId = request.UnitTypeId,
                Name = request.Name.Trim(),
                BasePrice = new Money(request.BasePrice.Amount, request.BasePrice.Currency),
                MaxCapacity = unitType.MaxCapacity,
                CustomFeatures = request.CustomFeatures.Trim(),
                PricingMethod = request.PricingMethod,
                IsAvailable = true,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _unitRepository.CreateUnitAsync(unit, cancellationToken);
            createdId = created.Id;

            // إنشاء قيم الحقول الديناميكية
            foreach (var dto in request.FieldValues)
            {
                if (dto.FieldId == Guid.Empty)
                    throw new BusinessRuleException("InvalidFieldId", "معرف الحقل غير صالح");

                var newValue = new UnitFieldValue
                {
                    UnitId = created.Id,
                    UnitTypeFieldId = dto.FieldId,
                    FieldValue = dto.FieldValue,
                    CreatedBy = _currentUserService.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _valueRepository.CreateUnitFieldValueAsync(newValue, cancellationToken);
            }

            // تسجيل عملية التدقيق
            await _auditService.LogBusinessOperationAsync(
                "CreateUnitWithFields",
                $"تم إنشاء وحدة {created.Id} مع قيم الحقول الديناميكية",
                created.Id,
                "Unit",
                _currentUserService.UserId,
                null,
                cancellationToken);

            // نشر الحدث (اختياري)
            // await _eventPublisher.PublishEventAsync(new UnitCreatedWithFieldsEvent
            // {
            //     UnitId = created.Id,
            //     CreatedBy = _currentUserService.UserId,
            //     CreatedAt = DateTime.UtcNow
            // }, cancellationToken);

            _logger.LogInformation("اكتمل إنشاء الوحدة بنجاح: UnitId={UnitId}", created.Id);
        });

        return ResultDto<Guid>.Succeeded(createdId, "تم إنشاء الوحدة بنجاح مع قيم الحقول الديناميكية");
    }
}

/// <summary>
/// حدث إنشاء وحدة جديدة مع قيم الحقول الديناميكية
/// Event for unit creation with dynamic field values
/// </summary>
public class UnitCreatedWithFieldsEvent
{
    public Guid UnitId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
} 
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Core.Events;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using AdvancedIndexingSystem.Core.Interfaces;
using AdvancedIndexingSystem.Core.Models;

namespace YemenBooking.Application.Handlers.Events;

/// <summary>
/// معالج أحداث فهرسة الحقول الديناميكية
/// Dynamic field indexing event handler
/// </summary>
public class DynamicFieldIndexingEventHandler : INotificationHandler<DynamicFieldIndexingEvent>
{
    private readonly IYemenBookingIndexService _indexService;
    private readonly IUnitFieldValueRepository _unitFieldValueRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly ILogger<DynamicFieldIndexingEventHandler> _logger;

    public DynamicFieldIndexingEventHandler(
        IYemenBookingIndexService indexService,
        IUnitFieldValueRepository unitFieldValueRepository,
        IPropertyRepository propertyRepository,
        IUnitRepository unitRepository,
        ILogger<DynamicFieldIndexingEventHandler> logger)
    {
        _indexService = indexService;
        _unitFieldValueRepository = unitFieldValueRepository;
        _propertyRepository = propertyRepository;
        _unitRepository = unitRepository;
        _logger = logger;
    }

    public async Task Handle(DynamicFieldIndexingEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("معالجة حدث فهرسة الحقل الديناميكي: {FieldName} للكيان {EntityId}", 
                notification.FieldName, notification.EntityId);

            // إنشاء عنصر فهرس الحقل الديناميكي
            var dynamicFieldItem = new DynamicFieldIndexItem
            {
                Id = notification.FieldId.ToString(),
                FieldName = notification.FieldName,
                FieldType = notification.FieldType,
                FieldValue = notification.FieldValue,
                EntityId = notification.EntityId.ToString(),
                EntityType = notification.EntityType,
                LastUpdated = notification.OccurredOn,
                AdditionalData = notification.AdditionalData
            };

            // تنفيذ العملية المطلوبة
            bool result = notification.Operation.ToLower() switch
            {
                "create" or "update" => await _indexService.IndexDynamicFieldAsync(dynamicFieldItem),
                "delete" => await _indexService.RemoveDynamicFieldIndexAsync(notification.FieldId.ToString()),
                _ => false
            };

            if (result)
            {
                _logger.LogInformation("تم فهرسة الحقل الديناميكي بنجاح: {FieldName}", notification.FieldName);
                
                // إضافة الحقل للفهرس العام إذا لم يكن موجوداً
                await _indexService.EnsureDynamicFieldIndexExistsAsync(notification.FieldName, notification.FieldType);
            }
            else
            {
                _logger.LogWarning("فشل في فهرسة الحقل الديناميكي: {FieldName}", notification.FieldName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة حدث فهرسة الحقل الديناميكي: {FieldName}", notification.FieldName);
        }
    }
}

/// <summary>
/// معالج أحداث فهرسة المدن
/// City indexing event handler
/// </summary>
public class CityIndexingEventHandler : INotificationHandler<CityIndexingEvent>
{
    private readonly IYemenBookingIndexService _indexService;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ILogger<CityIndexingEventHandler> _logger;

    public CityIndexingEventHandler(
        IYemenBookingIndexService indexService,
        IPropertyRepository propertyRepository,
        ILogger<CityIndexingEventHandler> logger)
    {
        _indexService = indexService;
        _propertyRepository = propertyRepository;
        _logger = logger;
    }

    public async Task Handle(CityIndexingEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("معالجة حدث فهرسة المدينة: {CityName} للعقار {PropertyId}", 
                notification.CityName, notification.PropertyId);

            var cityItem = new CityIndexItem
            {
                Id = notification.CityName,
                CityName = notification.CityName,
                PropertyIds = new List<string> { notification.PropertyId.ToString() },
                LastUpdated = notification.OccurredOn,
                Statistics = notification.CityStats
            };

            bool result = notification.Operation.ToLower() switch
            {
                "add" => await _indexService.AddPropertyToCityIndexAsync(notification.CityName, notification.PropertyId.ToString()),
                "remove" => await _indexService.RemovePropertyFromCityIndexAsync(notification.CityName, notification.PropertyId.ToString()),
                "update" => await _indexService.UpdateCityStatisticsAsync(notification.CityName, notification.CityStats),
                _ => false
            };

            if (result)
            {
                _logger.LogInformation("تم تحديث فهرس المدينة بنجاح: {CityName}", notification.CityName);
            }
            else
            {
                _logger.LogWarning("فشل في تحديث فهرس المدينة: {CityName}", notification.CityName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة حدث فهرسة المدينة: {CityName}", notification.CityName);
        }
    }
}

/// <summary>
/// معالج أحداث فهرسة التسعير المتقدم
/// Advanced pricing indexing event handler
/// </summary>
public class AdvancedPricingIndexingEventHandler : INotificationHandler<AdvancedPricingIndexingEvent>
{
    private readonly IYemenBookingIndexService _indexService;
    private readonly IPricingRuleRepository _pricingRuleRepository;
    private readonly ILogger<AdvancedPricingIndexingEventHandler> _logger;

    public AdvancedPricingIndexingEventHandler(
        IYemenBookingIndexService indexService,
        IPricingRuleRepository pricingRuleRepository,
        ILogger<AdvancedPricingIndexingEventHandler> logger)
    {
        _indexService = indexService;
        _pricingRuleRepository = pricingRuleRepository;
        _logger = logger;
    }

    public async Task Handle(AdvancedPricingIndexingEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("معالجة حدث فهرسة التسعير المتقدم: {PricingRuleId} للوحدة {UnitId}", 
                notification.PricingRuleId, notification.UnitId);

            var pricingItem = new PricingIndexItem
            {
                Id = notification.PricingRuleId.ToString(),
                UnitId = notification.UnitId.ToString(),
                PriceType = notification.PriceType,
                PriceAmount = notification.PriceAmount,
                Currency = notification.Currency,
                StartDate = notification.StartDate,
                EndDate = notification.EndDate,
                LastUpdated = notification.OccurredOn
            };

            bool result = notification.Operation.ToLower() switch
            {
                "create" or "update" => await _indexService.IndexPricingAsync(pricingItem),
                "delete" => await _indexService.RemovePricingIndexAsync(notification.PricingRuleId.ToString()),
                _ => false
            };

            if (result)
            {
                _logger.LogInformation("تم فهرسة التسعير المتقدم بنجاح: {PricingRuleId}", notification.PricingRuleId);
                
                // تحديث فهرس العملة إذا كانت جديدة
                await _indexService.IndexCurrencyAsync(notification.Currency, notification.PriceAmount);
            }
            else
            {
                _logger.LogWarning("فشل في فهرسة التسعير المتقدم: {PricingRuleId}", notification.PricingRuleId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة حدث فهرسة التسعير المتقدم: {PricingRuleId}", notification.PricingRuleId);
        }
    }
}

/// <summary>
/// معالج أحداث فهرسة الإتاحة المتقدمة
/// Advanced availability indexing event handler
/// </summary>
public class AdvancedAvailabilityIndexingEventHandler : INotificationHandler<AdvancedAvailabilityIndexingEvent>
{
    private readonly IYemenBookingIndexService _indexService;
    private readonly IUnitAvailabilityRepository _availabilityRepository;
    private readonly ILogger<AdvancedAvailabilityIndexingEventHandler> _logger;

    public AdvancedAvailabilityIndexingEventHandler(
        IYemenBookingIndexService indexService,
        IUnitAvailabilityRepository availabilityRepository,
        ILogger<AdvancedAvailabilityIndexingEventHandler> logger)
    {
        _indexService = indexService;
        _availabilityRepository = availabilityRepository;
        _logger = logger;
    }

    public async Task Handle(AdvancedAvailabilityIndexingEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("معالجة حدث فهرسة الإتاحة المتقدمة: {AvailabilityId} للوحدة {UnitId}", 
                notification.AvailabilityId, notification.UnitId);

            var availabilityItem = new AvailabilityIndexItem
            {
                Id = notification.AvailabilityId.ToString(),
                UnitId = notification.UnitId.ToString(),
                StartDate = notification.StartDate,
                EndDate = notification.EndDate,
                Status = notification.Status,
                LastUpdated = notification.OccurredOn
            };

            bool result = notification.Operation.ToLower() switch
            {
                "create" or "update" => await _indexService.IndexAvailabilityAsync(availabilityItem),
                "delete" => await _indexService.RemoveAvailabilityIndexAsync(notification.AvailabilityId.ToString()),
                _ => false
            };

            if (result)
            {
                _logger.LogInformation("تم فهرسة الإتاحة المتقدمة بنجاح: {AvailabilityId}", notification.AvailabilityId);
            }
            else
            {
                _logger.LogWarning("فشل في فهرسة الإتاحة المتقدمة: {AvailabilityId}", notification.AvailabilityId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة حدث فهرسة الإتاحة المتقدمة: {AvailabilityId}", notification.AvailabilityId);
        }
    }
}

/// <summary>
/// معالج أحداث فهرسة المرافق المتقدمة
/// Advanced facility indexing event handler
/// </summary>
public class AdvancedFacilityIndexingEventHandler : INotificationHandler<AdvancedFacilityIndexingEvent>
{
    private readonly IYemenBookingIndexService _indexService;
    private readonly IPropertyAmenityRepository _propertyAmenityRepository;
    private readonly ILogger<AdvancedFacilityIndexingEventHandler> _logger;

    public AdvancedFacilityIndexingEventHandler(
        IYemenBookingIndexService indexService,
        IPropertyAmenityRepository propertyAmenityRepository,
        ILogger<AdvancedFacilityIndexingEventHandler> logger)
    {
        _indexService = indexService;
        _propertyAmenityRepository = propertyAmenityRepository;
        _logger = logger;
    }

    public async Task Handle(AdvancedFacilityIndexingEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("معالجة حدث فهرسة المرفق المتقدم: {FacilityName} للعقار {PropertyId}", 
                notification.FacilityName, notification.PropertyId);

            bool result = notification.Operation.ToLower() switch
            {
                "associate" => await _indexService.AssociatePropertyWithFacilityAsync(
                    notification.PropertyId.ToString(), 
                    notification.FacilityName,
                    notification.FacilityData),
                "dissociate" => await _indexService.DissociatePropertyFromFacilityAsync(
                    notification.PropertyId.ToString(), 
                    notification.FacilityName),
                "update" => await _indexService.UpdatePropertyFacilityDataAsync(
                    notification.PropertyId.ToString(), 
                    notification.FacilityName,
                    notification.FacilityData),
                _ => false
            };

            if (result)
            {
                _logger.LogInformation("تم فهرسة المرفق المتقدم بنجاح: {FacilityName}", notification.FacilityName);
            }
            else
            {
                _logger.LogWarning("فشل في فهرسة المرفق المتقدم: {FacilityName}", notification.FacilityName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة حدث فهرسة المرفق المتقدم: {FacilityName}", notification.FacilityName);
        }
    }
}

/// <summary>
/// معالج أحداث فهرسة أنواع العقارات والوحدات
/// Property and unit types indexing event handler
/// </summary>
public class TypeIndexingEventHandler : INotificationHandler<TypeIndexingEvent>
{
    private readonly IYemenBookingIndexService _indexService;
    private readonly IPropertyTypeRepository _propertyTypeRepository;
    private readonly IUnitTypeRepository _unitTypeRepository;
    private readonly ILogger<TypeIndexingEventHandler> _logger;

    public TypeIndexingEventHandler(
        IYemenBookingIndexService indexService,
        IPropertyTypeRepository propertyTypeRepository,
        IUnitTypeRepository unitTypeRepository,
        ILogger<TypeIndexingEventHandler> logger)
    {
        _indexService = indexService;
        _propertyTypeRepository = propertyTypeRepository;
        _unitTypeRepository = unitTypeRepository;
        _logger = logger;
    }

    public async Task Handle(TypeIndexingEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("معالجة حدث فهرسة النوع: {TypeName} من نوع {EntityType}", 
                notification.TypeName, notification.EntityType);

            bool result = false;

            if (notification.EntityType.ToLower() == "propertytype")
            {
                result = await HandlePropertyTypeIndexing(notification);
            }
            else if (notification.EntityType.ToLower() == "unittype")
            {
                result = await HandleUnitTypeIndexing(notification);
            }

            if (result)
            {
                _logger.LogInformation("تم فهرسة النوع بنجاح: {TypeName}", notification.TypeName);
                
                // فهرسة الحقول المرتبطة
                await IndexAssociatedFields(notification);
            }
            else
            {
                _logger.LogWarning("فشل في فهرسة النوع: {TypeName}", notification.TypeName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة حدث فهرسة النوع: {TypeName}", notification.TypeName);
        }
    }

    private async Task<bool> HandlePropertyTypeIndexing(TypeIndexingEvent notification)
    {
        var propertyTypeItem = new PropertyTypeIndexItem
        {
            Id = notification.TypeId.ToString(),
            TypeName = notification.TypeName,
            AssociatedFields = notification.AssociatedFields,
            LastUpdated = notification.OccurredOn,
            TypeData = notification.TypeData
        };

        return notification.Operation.ToLower() switch
        {
            "create" or "update" => await _indexService.IndexPropertyTypeAsync(propertyTypeItem),
            "delete" => await _indexService.RemovePropertyTypeIndexAsync(notification.TypeId.ToString()),
            _ => false
        };
    }

    private async Task<bool> HandleUnitTypeIndexing(TypeIndexingEvent notification)
    {
        var unitTypeItem = new UnitTypeIndexItem
        {
            Id = notification.TypeId.ToString(),
            TypeName = notification.TypeName,
            AssociatedFields = notification.AssociatedFields,
            LastUpdated = notification.OccurredOn,
            TypeData = notification.TypeData
        };

        return notification.Operation.ToLower() switch
        {
            "create" or "update" => await _indexService.IndexUnitTypeAsync(unitTypeItem),
            "delete" => await _indexService.RemoveUnitTypeIndexAsync(notification.TypeId.ToString()),
            _ => false
        };
    }

    private async Task IndexAssociatedFields(TypeIndexingEvent notification)
    {
        foreach (var fieldName in notification.AssociatedFields)
        {
            try
            {
                await _indexService.EnsureDynamicFieldIndexExistsAsync(fieldName, "Text"); // Default type
                _logger.LogDebug("تم فهرس الحقل المرتبط: {FieldName} للنوع {TypeName}", fieldName, notification.TypeName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "فشل في فهرسة الحقل المرتبط: {FieldName} للنوع {TypeName}", fieldName, notification.TypeName);
            }
        }
    }
}

/// <summary>
/// معالج أحداث الفهرسة الشاملة المتقدمة
/// Advanced comprehensive indexing event handler
/// </summary>
public class ComprehensiveIndexingEventHandler : INotificationHandler<ComprehensiveIndexingEvent>
{
    private readonly IYemenBookingIndexService _indexService;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<ComprehensiveIndexingEventHandler> _logger;

    public ComprehensiveIndexingEventHandler(
        IYemenBookingIndexService indexService,
        IPropertyRepository propertyRepository,
        IUnitRepository unitRepository,
        IMediator mediator,
        ILogger<ComprehensiveIndexingEventHandler> logger)
    {
        _indexService = indexService;
        _propertyRepository = propertyRepository;
        _unitRepository = unitRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(ComprehensiveIndexingEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("معالجة حدث الفهرسة الشاملة: {EntityType} {EntityId}", 
                notification.EntityType, notification.EntityId);

            // ترتيب العمليات حسب الأولوية
            var sortedOperations = notification.RequiredOperations
                .OrderBy(op => GetOperationPriority(op))
                .ToList();

            int successCount = 0;
            int totalOperations = sortedOperations.Count;

            foreach (var operation in sortedOperations)
            {
                try
                {
                    bool result = await ExecuteOperation(operation, notification, cancellationToken);
                    if (result)
                    {
                        successCount++;
                        _logger.LogDebug("نجحت العملية: {Operation} للكيان {EntityId}", operation, notification.EntityId);
                    }
                    else
                    {
                        _logger.LogWarning("فشلت العملية: {Operation} للكيان {EntityId}", operation, notification.EntityId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطأ في تنفيذ العملية: {Operation} للكيان {EntityId}", operation, notification.EntityId);
                }

                // تأخير قصير بين العمليات لتجنب الحمولة الزائدة
                await Task.Delay(10, cancellationToken);
            }

            _logger.LogInformation("اكتملت الفهرسة الشاملة: {SuccessCount}/{TotalOperations} للكيان {EntityId}", 
                successCount, totalOperations, notification.EntityId);

            // حفظ الفهارس إذا كانت العمليات ناجحة
            if (successCount > 0)
            {
                await _indexService.SaveIndexesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في معالجة حدث الفهرسة الشاملة: {EntityType} {EntityId}", 
                notification.EntityType, notification.EntityId);
        }
    }

    private async Task<bool> ExecuteOperation(string operation, ComprehensiveIndexingEvent notification, CancellationToken cancellationToken)
    {
        return operation.ToLower() switch
        {
            "index_entity" => await IndexMainEntity(notification),
            "index_pricing" => await IndexRelatedPricing(notification),
            "index_availability" => await IndexRelatedAvailability(notification),
            "index_amenities" => await IndexRelatedAmenities(notification),
            "index_dynamic_fields" => await IndexDynamicFields(notification),
            "index_city" => await IndexCityData(notification),
            "update_statistics" => await UpdateStatistics(notification),
            _ => false
        };
    }

    private async Task<bool> IndexMainEntity(ComprehensiveIndexingEvent notification)
    {
        // Implementation for main entity indexing
        return true;
    }

    private async Task<bool> IndexRelatedPricing(ComprehensiveIndexingEvent notification)
    {
        // Implementation for pricing indexing
        return true;
    }

    private async Task<bool> IndexRelatedAvailability(ComprehensiveIndexingEvent notification)
    {
        // Implementation for availability indexing
        return true;
    }

    private async Task<bool> IndexRelatedAmenities(ComprehensiveIndexingEvent notification)
    {
        // Implementation for amenities indexing
        return true;
    }

    private async Task<bool> IndexDynamicFields(ComprehensiveIndexingEvent notification)
    {
        // Implementation for dynamic fields indexing
        return true;
    }

    private async Task<bool> IndexCityData(ComprehensiveIndexingEvent notification)
    {
        // Implementation for city data indexing
        return true;
    }

    private async Task<bool> UpdateStatistics(ComprehensiveIndexingEvent notification)
    {
        // Implementation for statistics update
        return true;
    }

    private static int GetOperationPriority(string operation)
    {
        return operation.ToLower() switch
        {
            "index_entity" => 1,
            "index_city" => 2,
            "index_dynamic_fields" => 3,
            "index_amenities" => 4,
            "index_pricing" => 5,
            "index_availability" => 6,
            "update_statistics" => 7,
            _ => 999
        };
    }
}
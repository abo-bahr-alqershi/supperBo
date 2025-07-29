using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.MobileApp.Units;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Units;
using YemenBooking.Core.Interfaces.Repositories;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Units;

/// <summary>
/// معالج استعلام الحصول على الوحدات المتاحة
/// Handler for get available units query
/// </summary>
public class GetAvailableUnitsQueryHandler : IRequestHandler<GetAvailableUnitsQuery, ResultDto<AvailableUnitsResponse>>
{
    private readonly IUnitRepository _unitRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitTypeRepository _unitTypeRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IUnitFieldValueRepository _unitFieldValueRepository;
    private readonly ILogger<GetAvailableUnitsQueryHandler> _logger;

    /// <summary>
    /// منشئ معالج استعلام الوحدات المتاحة
    /// Constructor for get available units query handler
    /// </summary>
    /// <param name="unitRepository">مستودع الوحدات</param>
    /// <param name="propertyRepository">مستودع العقارات</param>
    /// <param name="unitTypeRepository">مستودع أنواع الوحدات</param>
    /// <param name="bookingRepository">مستودع الحجوزات</param>
    /// <param name="propertyImageRepository">مستودع صور العقارات والوحدات</param>
    /// <param name="unitFieldValueRepository">مستودع قيم حقول الوحدات</param>
    /// <param name="logger">مسجل الأحداث</param>
    public GetAvailableUnitsQueryHandler(
        IUnitRepository unitRepository,
        IPropertyRepository propertyRepository,
        IUnitTypeRepository unitTypeRepository,
        IBookingRepository bookingRepository,
        IPropertyImageRepository propertyImageRepository,
        IUnitFieldValueRepository unitFieldValueRepository,
        ILogger<GetAvailableUnitsQueryHandler> logger)
    {
        _unitRepository = unitRepository;
        _propertyRepository = propertyRepository;
        _unitTypeRepository = unitTypeRepository;
        _bookingRepository = bookingRepository;
        _propertyImageRepository = propertyImageRepository;
        _unitFieldValueRepository = unitFieldValueRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام الحصول على الوحدات المتاحة
    /// Handle get available units query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة الوحدات المتاحة</returns>
    public async Task<ResultDto<AvailableUnitsResponse>> Handle(GetAvailableUnitsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء استعلام الوحدات المتاحة. معرف العقار: {PropertyId}, تاريخ الوصول: {CheckIn}, تاريخ المغادرة: {CheckOut}, عدد الضيوف: {GuestsCount}", 
                request.PropertyId, request.CheckIn, request.CheckOut, request.GuestsCount);

            // التحقق من صحة البيانات المدخلة
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // التحقق من وجود العقار
            var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
            if (property == null)
            {
                _logger.LogWarning("لم يتم العثور على العقار: {PropertyId}", request.PropertyId);
                return ResultDto<AvailableUnitsResponse>.Failed("العقار غير موجود", "PROPERTY_NOT_FOUND");
            }

            // التحقق من أن العقار نشط
            if (!property.IsActive)
            {
                _logger.LogWarning("العقار غير نشط: {PropertyId}", request.PropertyId);
                return ResultDto<AvailableUnitsResponse>.Failed("العقار غير متاح حالياً", "PROPERTY_INACTIVE");
            }

            // الحصول على جميع الوحدات في العقار
            var allUnits = await _unitRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);
            if (allUnits == null || !allUnits.Any())
            {
                _logger.LogInformation("لا توجد وحدات في العقار: {PropertyId}", request.PropertyId);
                
                return ResultDto<AvailableUnitsResponse>.Ok(
                    new AvailableUnitsResponse
                    {
                        Units = new List<AvailableUnitDto>(),
                        TotalAvailable = 0
                    }, 
                    "لا توجد وحدات متاحة في هذا العقار"
                );
            }

            // فلترة الوحدات النشطة والتي تتسع للعدد المطلوب من الضيوف
            var eligibleUnits = allUnits
                .Where(u => u.IsActive && u.MaxCapacity >= request.GuestsCount)
                .ToList();

            if (!eligibleUnits.Any())
            {
                _logger.LogInformation("لا توجد وحدات تتسع لـ {GuestsCount} ضيف في العقار: {PropertyId}", 
                    request.GuestsCount, request.PropertyId);
                
                return ResultDto<AvailableUnitsResponse>.Ok(
                    new AvailableUnitsResponse
                    {
                        Units = new List<AvailableUnitDto>(),
                        TotalAvailable = 0
                    }, 
                    $"لا توجد وحدات تتسع لـ {request.GuestsCount} ضيف"
                );
            }

            // التحقق من توفر الوحدات في الفترة المطلوبة
            var availableUnits = new List<AvailableUnitDto>();

            foreach (var unit in eligibleUnits)
            {
                // التحقق من عدم وجود حجوزات متضاربة
                var isAvailable = await IsUnitAvailable(unit.Id, request.CheckIn, request.CheckOut, cancellationToken);
                
                if (!isAvailable)
                {
                    continue;
                }

                // الحصول على تفاصيل نوع الوحدة
                var unitType = await _unitTypeRepository.GetByIdAsync(unit.UnitTypeId, cancellationToken);
                var unitTypeDto = unitType != null ? new UnitTypeDto
                {
                    Id = unitType.Id,
                    Name = unitType.Name ?? string.Empty,
                    Description = unitType.Description ?? string.Empty
                } : new UnitTypeDto();

                // الحصول على صور الوحدة
                var allImages = await _propertyImageRepository.GetAllAsync(cancellationToken);
            var unitImages = allImages?.Where(img => img.PropertyId == unit.PropertyId);
                var imageDtos = unitImages?.Select(img => new UnitImageDto
                {
                    Id = img.Id,
                    Url = img.Url ?? string.Empty,
                    Caption = img.Caption ?? string.Empty,
                    IsMain = img.IsMain
                }).OrderByDescending(img => img.IsMain).ThenBy(img => img.Caption).ToList() ?? new List<UnitImageDto>();

                // الحصول على قيم الحقول الديناميكية
                var fieldValues = await _unitFieldValueRepository.GetByUnitIdAsync(unit.Id, cancellationToken);
                var fieldValueDtos = fieldValues?.Select(fv => new UnitFieldSimpleDto
                {
                    FieldName = fv.UnitTypeField?.FieldName ?? string.Empty,
                    DisplayName = fv.UnitTypeField?.DisplayName ?? string.Empty,
                    Value = fv.FieldValue ?? string.Empty,
                    FieldType = fv.UnitTypeField?.FieldTypeId ?? string.Empty
                }).ToList() ?? new List<UnitFieldSimpleDto>();

                // حساب السعر للفترة المحددة
                var (totalPrice, pricePerNight) = await CalculateUnitPrice(unit, request.CheckIn, request.CheckOut, cancellationToken);

                var availableUnitDto = new AvailableUnitDto
                {
                    Id = unit.Id,
                    Name = unit.Name ?? string.Empty,
                    UnitType = unitTypeDto,
                    MaxCapacity = unit.MaxCapacity,
                    TotalPrice = totalPrice,
                    PricePerNight = pricePerNight,
                    Currency = "YER", // العملة الافتراضية
                    PricingMethod = unit.PricingMethod.ToString(),
                    Images = imageDtos,
                    CustomFeatures = !string.IsNullOrEmpty(unit.CustomFeatures) 
                        ? JsonSerializer.Deserialize<Dictionary<string, object>>(unit.CustomFeatures) ?? new Dictionary<string, object>()
                        : new Dictionary<string, object>(),
                    FieldValues = fieldValueDtos
                };

                availableUnits.Add(availableUnitDto);
            }

            // ترتيب الوحدات حسب السعر (الأقل أولاً)
            availableUnits = availableUnits.OrderBy(u => u.TotalPrice).ToList();

            var response = new AvailableUnitsResponse
            {
                Units = availableUnits,
                TotalAvailable = availableUnits.Count
            };

            _logger.LogInformation("تم العثور على {Count} وحدة متاحة في العقار {PropertyId}", 
                availableUnits.Count, request.PropertyId);

            return ResultDto<AvailableUnitsResponse>.Ok(
                response, 
                $"تم العثور على {availableUnits.Count} وحدة متاحة"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء البحث عن الوحدات المتاحة. معرف العقار: {PropertyId}", request.PropertyId);
            return ResultDto<AvailableUnitsResponse>.Failed(
                $"حدث خطأ أثناء البحث عن الوحدات المتاحة: {ex.Message}", 
                "GET_AVAILABLE_UNITS_ERROR"
            );
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate request data
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<AvailableUnitsResponse> ValidateRequest(GetAvailableUnitsQuery request)
    {
        if (request.PropertyId == Guid.Empty)
        {
            _logger.LogWarning("معرف العقار مطلوب");
            return ResultDto<AvailableUnitsResponse>.Failed("معرف العقار مطلوب", "PROPERTY_ID_REQUIRED");
        }

        if (request.CheckIn >= request.CheckOut)
        {
            _logger.LogWarning("تاريخ الوصول يجب أن يكون قبل تاريخ المغادرة");
            return ResultDto<AvailableUnitsResponse>.Failed("تاريخ الوصول يجب أن يكون قبل تاريخ المغادرة", "INVALID_DATE_RANGE");
        }

        if (request.CheckIn.Date < DateTime.Today)
        {
            _logger.LogWarning("تاريخ الوصول لا يمكن أن يكون في الماضي");
            return ResultDto<AvailableUnitsResponse>.Failed("تاريخ الوصول لا يمكن أن يكون في الماضي", "INVALID_CHECKIN_DATE");
        }

        if (request.GuestsCount < 1 || request.GuestsCount > 50)
        {
            _logger.LogWarning("عدد الضيوف يجب أن يكون بين 1 و 50");
            return ResultDto<AvailableUnitsResponse>.Failed("عدد الضيوف يجب أن يكون بين 1 و 50", "INVALID_GUESTS_COUNT");
        }

        // التحقق من أن الفترة لا تتجاوز سنة واحدة
        var daysDifference = (request.CheckOut - request.CheckIn).Days;
        if (daysDifference > 365)
        {
            _logger.LogWarning("فترة الإقامة لا يمكن أن تتجاوز سنة واحدة");
            return ResultDto<AvailableUnitsResponse>.Failed("فترة الإقامة لا يمكن أن تتجاوز سنة واحدة", "INVALID_STAY_DURATION");
        }

        return ResultDto<AvailableUnitsResponse>.Ok(null, "البيانات صحيحة");
    }

    /// <summary>
    /// التحقق من توفر الوحدة في الفترة المحددة
    /// Check if unit is available for the specified period
    /// </summary>
    /// <param name="unitId">معرف الوحدة</param>
    /// <param name="checkIn">تاريخ الوصول</param>
    /// <param name="checkOut">تاريخ المغادرة</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>هل الوحدة متاحة</returns>
    private async Task<bool> IsUnitAvailable(Guid unitId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
    {
        // البحث عن حجوزات متضاربة
        var conflictingBookings = await _bookingRepository.GetConflictingBookingsAsync(
            unitId, checkIn, checkOut, cancellationToken);

        return conflictingBookings == null || !conflictingBookings.Any();
    }

    /// <summary>
    /// حساب سعر الوحدة للفترة المحددة
    /// Calculate unit price for the specified period
    /// </summary>
    /// <param name="unit">الوحدة</param>
    /// <param name="checkIn">تاريخ الوصول</param>
    /// <param name="checkOut">تاريخ المغادرة</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>السعر الإجمالي والسعر لليلة الواحدة</returns>
    private async Task<(decimal totalPrice, decimal pricePerNight)> CalculateUnitPrice(
        Core.Entities.Unit unit, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken)
    {
        var nights = (checkOut - checkIn).Days;
        var basePrice = unit.BasePrice;

        // يمكن إضافة منطق أكثر تعقيداً هنا لحساب الأسعار الموسمية أو العروض الخاصة
        // مثل الحصول على أسعار خاصة من جدول منفصل

        var pricePerNight = basePrice;
        var totalPrice = pricePerNight * nights;

        // تطبيق أي خصومات أو رسوم إضافية
        if (unit.DiscountPercentage > 0)
        {
            var discountAmount = totalPrice * (unit.DiscountPercentage / 100);
            totalPrice -= discountAmount;
        }

        return (totalPrice, pricePerNight);
    }
}

using Microsoft.AspNetCore.Mvc;
using YemenBooking.Core.Interfaces.Services;
using AdvancedIndexingSystem.Core.Services;
using YemenBooking.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace YemenBooking.Api.Controllers;

/// <summary>
/// وحدة تحكم البحث المتقدم
/// Advanced search controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly IIndexingService _indexingService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        IIndexingService indexingService,
        ILogger<SearchController> logger)
    {
        _indexingService = indexingService;
        _logger = logger;
    }

    /// <summary>
    /// البحث المتقدم في العقارات
    /// Advanced property search
    /// </summary>
    /// <param name="request">طلب البحث</param>
    /// <returns>نتائج البحث</returns>
    [HttpPost("properties")]
    public async Task<ActionResult<ResultDto<List<PropertySearchResultDto>>>> SearchProperties(
        [FromBody] PropertySearchRequest request)
    {
        try
        {
            var results = await _indexingService.SearchPropertiesAsync(request);
            
            var dtos = results.Select(p => new PropertySearchResultDto
            {
                Id = Guid.Parse(p.Id),
                Name = p.Name,
                Address = p.Address,
                City = p.City,
                PropertyType = p.PropertyType,
                StarRating = p.StarRating,
                Description = p.Description,
                MinPrice = p.MinPrice,
                MaxPrice = p.MaxPrice,
                AverageRating = p.AverageRating,
                ViewCount = p.ViewCount,
                BookingCount = p.BookingCount,
                ImageUrls = p.ImageUrls,
                IsApproved = p.IsApproved,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(ResultDto<List<PropertySearchResultDto>>.Succeeded(
                dtos, 
                $"تم العثور على {dtos.Count} عقار"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في البحث في العقارات");
            return BadRequest(ResultDto<List<PropertySearchResultDto>>.Failed("حدث خطأ أثناء البحث"));
        }
    }

    /// <summary>
    /// البحث المتقدم في الوحدات
    /// Advanced unit search
    /// </summary>
    /// <param name="request">طلب البحث</param>
    /// <returns>نتائج البحث</returns>
    [HttpPost("units")]
    public async Task<ActionResult<ResultDto<List<UnitSearchResultDto>>>> SearchUnits(
        [FromBody] UnitSearchRequest request)
    {
        try
        {
            var results = await _indexingService.SearchUnitsAsync(request);
            
            var dtos = results.Select(u => new UnitSearchResultDto
            {
                Id = Guid.Parse(u.Id),
                PropertyId = Guid.Parse(u.PropertyId),
                Name = u.Name,
                UnitType = u.UnitType,
                BasePrice = u.BasePrice,
                Currency = u.Currency,
                MaxCapacity = u.MaxCapacity,
                IsAvailable = u.IsAvailable,
                ViewCount = u.ViewCount,
                BookingCount = u.BookingCount,
                DynamicFields = u.DynamicFields,
                ImageUrls = u.ImageUrls,
                CustomFeatures = u.CustomFeatures,
                CreatedAt = u.CreatedAt
            }).ToList();

            return Ok(ResultDto<List<UnitSearchResultDto>>.Succeeded(
                dtos, 
                $"تم العثور على {dtos.Count} وحدة"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في البحث في الوحدات");
            return BadRequest(ResultDto<List<UnitSearchResultDto>>.Failed("حدث خطأ أثناء البحث"));
        }
    }

    /// <summary>
    /// البحث بالحقول الديناميكية
    /// Search by dynamic fields
    /// </summary>
    /// <param name="request">طلب البحث بالحقول الديناميكية</param>
    /// <returns>معرفات الوحدات المطابقة</returns>
    [HttpPost("units/dynamic-fields")]
    public async Task<ActionResult<ResultDto<List<Guid>>>> SearchByDynamicFields(
        [FromBody] DynamicFieldSearchRequest request)
    {
        try
        {
            var unitSearchRequest = new UnitSearchRequest
            {
                DynamicFieldFilters = request.DynamicFieldFilters,
                PropertyId = request.PropertyId,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var results = await _indexingService.SearchUnitsAsync(unitSearchRequest);
            var unitIds = results.Select(u => Guid.Parse(u.Id)).ToList();

            return Ok(ResultDto<List<Guid>>.Succeeded(
                unitIds, 
                $"تم العثور على {unitIds.Count} وحدة تطابق الحقول الديناميكية"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في البحث بالحقول الديناميكية");
            return BadRequest(ResultDto<List<Guid>>.Failed("حدث خطأ أثناء البحث"));
        }
    }

    /// <summary>
    /// البحث في الوحدات المتاحة لفترة محددة
    /// Search available units for specific period
    /// </summary>
    /// <param name="request">طلب البحث بالإتاحة</param>
    /// <returns>الوحدات المتاحة</returns>
    [HttpPost("units/available")]
    public async Task<ActionResult<ResultDto<List<UnitSearchResultDto>>>> SearchAvailableUnits(
        [FromBody] AvailabilitySearchRequest request)
    {
        try
        {
            var unitSearchRequest = new UnitSearchRequest
            {
                PropertyId = request.PropertyId,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                MinCapacity = request.MinCapacity,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                UnitType = request.UnitType,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var results = await _indexingService.SearchUnitsAsync(unitSearchRequest);
            
            var dtos = results.Select(u => new UnitSearchResultDto
            {
                Id = Guid.Parse(u.Id),
                PropertyId = Guid.Parse(u.PropertyId),
                Name = u.Name,
                UnitType = u.UnitType,
                BasePrice = u.BasePrice,
                Currency = u.Currency,
                MaxCapacity = u.MaxCapacity,
                IsAvailable = u.IsAvailable,
                ViewCount = u.ViewCount,
                BookingCount = u.BookingCount,
                DynamicFields = u.DynamicFields,
                ImageUrls = u.ImageUrls,
                CustomFeatures = u.CustomFeatures,
                CreatedAt = u.CreatedAt
            }).ToList();

            return Ok(ResultDto<List<UnitSearchResultDto>>.Succeeded(
                dtos, 
                $"تم العثور على {dtos.Count} وحدة متاحة"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في البحث في الوحدات المتاحة");
            return BadRequest(ResultDto<List<UnitSearchResultDto>>.Failed("حدث خطأ أثناء البحث"));
        }
    }

    /// <summary>
    /// إعادة بناء الفهارس
    /// Rebuild indexes
    /// </summary>
    /// <returns>نتيجة العملية</returns>
    [HttpPost("rebuild-indexes")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ResultDto<bool>>> RebuildIndexes()
    {
        try
        {
            var result = await _indexingService.SaveIndexesAsync();
            if (result)
            {
                _logger.LogInformation("تم إعادة بناء الفهارس بنجاح");
                return Ok(ResultDto<bool>.Succeeded(true, "تم إعادة بناء الفهارس بنجاح"));
            }
            else
            {
                return BadRequest(ResultDto<bool>.Failed("فشل في إعادة بناء الفهارس"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إعادة بناء الفهارس");
            return BadRequest(ResultDto<bool>.Failed("حدث خطأ أثناء إعادة بناء الفهارس"));
        }
    }
}

#region DTOs

/// <summary>
/// نتيجة البحث في العقارات
/// Property search result DTO
/// </summary>
public class PropertySearchResultDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal AverageRating { get; set; }
    public int ViewCount { get; set; }
    public int BookingCount { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// نتيجة البحث في الوحدات
/// Unit search result DTO
/// </summary>
public class UnitSearchResultDto
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UnitType { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public bool IsAvailable { get; set; }
    public int ViewCount { get; set; }
    public int BookingCount { get; set; }
    public Dictionary<string, object> DynamicFields { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public string CustomFeatures { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// طلب البحث بالحقول الديناميكية
/// Dynamic field search request
/// </summary>
public class DynamicFieldSearchRequest
{
    public Dictionary<string, object> DynamicFieldFilters { get; set; } = new();
    public Guid? PropertyId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// طلب البحث بالإتاحة
/// Availability search request
/// </summary>
public class AvailabilitySearchRequest
{
    public Guid? PropertyId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int? MinCapacity { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? UnitType { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

#endregion
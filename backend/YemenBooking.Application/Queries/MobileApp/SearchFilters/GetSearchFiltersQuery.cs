using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.PropertySearch;

namespace YemenBooking.Application.Queries.MobileApp.SearchFilters;

/// <summary>
/// استعلام الحصول على فلاتر البحث المتاحة
/// Query to get available search filters
/// </summary>
public class GetSearchFiltersQuery : IRequest<ResultDto<SearchFiltersDto>>
{
    /// <summary>
    /// معرف نوع الكيان (اختياري)
    /// </summary>
    public Guid? PropertyTypeId { get; set; }
    
    /// <summary>
    /// المدينة (اختياري لتحديد الفلاتر المتاحة بناءً على المنطقة)
    /// </summary>
    public string? City { get; set; }
}

/// <summary>
/// بيانات نطاق الأسعار
/// </summary>
public class PriceRangeDto
{
    /// <summary>
    /// الحد الأدنى للسعر
    /// </summary>
    public decimal Min { get; set; }
    
    /// <summary>
    /// الحد الأقصى للسعر
    /// </summary>
    public decimal Max { get; set; }
    
    /// <summary>
    /// العملة
    /// </summary>
    public string Currency { get; set; } = string.Empty;
}

/// <summary>
/// بيانات فلتر نوع الكيان
/// </summary>
public class PropertyTypeFilterDto
{
    /// <summary>
    /// معرف نوع الكيان
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// اسم نوع الكيان
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// عدد الكيانات المتاحة من هذا النوع
    /// </summary>
    public int Count { get; set; }
}

/// <summary>
/// بيانات فلتر وسيلة الراحة
/// </summary>
public class AmenityFilterDto
{
    /// <summary>
    /// معرف الوسيلة
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// اسم الوسيلة
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// فئة الوسيلة
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// عدد الكيانات التي توفر هذه الوسيلة
    /// </summary>
    public int Count { get; set; }
}
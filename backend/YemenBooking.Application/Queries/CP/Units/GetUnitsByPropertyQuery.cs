using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Units;

/// <summary>
/// استعلام للحصول على وحدات الكيان
/// Query to get units by property
/// </summary>
public class GetUnitsByPropertyQuery : IRequest<PaginatedResult<UnitDto>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// رقم الصفحة
    /// Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// حجم الصفحة
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// فلترة بالتوفر (اختياري)
    /// Filter by availability (optional)
    /// </summary>
    public bool? IsAvailable { get; set; }

    /// <summary>
    /// فلترة بالسعر الأدنى (اختياري)
    /// Filter by minimum base price (optional)
    /// </summary>
    public decimal? MinBasePrice { get; set; }

    /// <summary>
    /// فلترة بالسعر الأقصى (اختياري)
    /// Filter by maximum base price (optional)
    /// </summary>
    public decimal? MaxBasePrice { get; set; }

    /// <summary>
    /// فلترة بالسعة (اختياري)
    /// Filter by minimum capacity (optional)
    /// </summary>
    public int? MinCapacity { get; set; }

    /// <summary>
    /// بحث بالاسم أو الرقم (اختياري)
    /// Name search: name or number (optional)
    /// </summary>
    public string? NameContains { get; set; }
} 
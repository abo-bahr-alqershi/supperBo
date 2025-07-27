using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Units;

/// <summary>
/// استعلام للحصول على الوحدات حسب النوع
/// Query to get units by type
/// </summary>
public class GetUnitsByTypeQuery : IRequest<PaginatedResult<UnitDto>>
{
    /// <summary>
    /// معرف نوع الوحدة
    /// Unit type ID
    /// </summary>
    public Guid UnitTypeId { get; set; }

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

    /// <summary>
    /// تضمين القيم الديناميكية (اختياري)
    /// Include dynamic field values
    /// </summary>
    public bool IncludeDynamicFields { get; set; } = true;

} 
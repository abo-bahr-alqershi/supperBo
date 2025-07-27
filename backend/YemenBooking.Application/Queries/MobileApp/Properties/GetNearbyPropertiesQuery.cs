using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;

namespace YemenBooking.Application.Queries.MobileApp.Properties;

/// <summary>
/// استعلام الحصول على الكيانات القريبة من موقع محدد
/// Query to get properties near a specific location
/// </summary>
public class GetNearbyPropertiesQuery : IRequest<ResultDto<List<NearbyPropertyDto>>>
{
    /// <summary>
    /// خط العرض للموقع المرجعي
    /// </summary>
    public decimal Latitude { get; set; }
    
    /// <summary>
    /// خط الطول للموقع المرجعي
    /// </summary>
    public decimal Longitude { get; set; }
    
    /// <summary>
    /// نصف القطر بالكيلومترات
    /// </summary>
    public double RadiusKm { get; set; } = 10;
    
    /// <summary>
    /// الحد الأقصى لعدد النتائج
    /// </summary>
    public int MaxResults { get; set; } = 20;
    
    /// <summary>
    /// فلترة حسب نوع الكيان (اختياري)
    /// </summary>
    public Guid? PropertyTypeId { get; set; }
}
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs.Users;

/// <summary>
/// استجابة قائمة المفضلات
/// User favorites response
/// </summary>
public class UserFavoritesResponse
{
    /// <summary>
    /// قائمة العقارات المفضلة
    /// List of favorite properties
    /// </summary>
    public List<FavoritePropertyDto> Favorites { get; set; } = new();
    
    /// <summary>
    /// إجمالي عدد المفضلات
    /// Total count of favorites
    /// </summary>
    public int TotalCount { get; set; }
}

/// <summary>
/// بيانات العقار المفضل
/// Favorite property data
/// </summary>
public class FavoritePropertyDto
{
    /// <summary>
    /// معرف العقار
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }
    
    /// <summary>
    /// اسم العقار
    /// Property name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// الموقع
    /// Location
    /// </summary>
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// تصنيف النجوم
    /// Star rating
    /// </summary>
    public int StarRating { get; set; }
    
    /// <summary>
    /// متوسط التقييم
    /// Average rating
    /// </summary>
    public decimal AverageRating { get; set; }
    
    /// <summary>
    /// السعر الأدنى
    /// Minimum price
    /// </summary>
    public decimal MinPrice { get; set; }
    
    /// <summary>
    /// العملة
    /// Currency
    /// </summary>
    public string Currency { get; set; } = string.Empty;
    
    /// <summary>
    /// رابط الصورة الرئيسية
    /// Main image URL
    /// </summary>
    public string MainImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// تاريخ الإضافة للمفضلات
    /// Date added to favorites
    /// </summary>
    public DateTime AddedToFavoritesAt { get; set; }
}

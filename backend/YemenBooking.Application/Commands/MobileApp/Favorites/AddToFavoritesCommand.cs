using MediatR;

namespace YemenBooking.Application.Features.Favorites.Commands;

/// <summary>
/// أمر إضافة كيان إلى المفضلات
/// Command to add property to favorites
/// </summary>
public class AddToFavoritesCommand : IRequest<AddToFavoritesResponse>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// معرف الكيان
    /// </summary>
    public Guid PropertyId { get; set; }
}

/// <summary>
/// استجابة إضافة إلى المفضلات
/// </summary>
public class AddToFavoritesResponse
{
    /// <summary>
    /// نجاح الإضافة
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// رسالة النتيجة
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
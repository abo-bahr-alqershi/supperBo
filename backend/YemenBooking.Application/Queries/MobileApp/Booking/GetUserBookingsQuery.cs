using MediatR;
using YemenBooking.Core.Enums;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Bookings;

namespace YemenBooking.Application.Queries.MobileApp.Booking;

/// <summary>
/// استعلام الحصول على حجوزات المستخدم
/// Query to get user bookings
/// </summary>
public class GetUserBookingsQuery : IRequest<ResultDto<UserBookingsResponse>>
{
    /// <summary>
    /// معرف المستخدم
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// فلترة حسب الحالة (اختياري)
    /// </summary>
    public BookingStatus? Status { get; set; }
    
    /// <summary>
    /// رقم الصفحة
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// حجم الصفحة
    /// </summary>
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// استجابة حجوزات المستخدم
/// </summary>
public class UserBookingsResponse
{
    /// <summary>
    /// قائمة الحجوزات
    /// </summary>
    public List<BookingDto> Bookings { get; set; } = new();
    
    /// <summary>
    /// إجمالي عدد الحجوزات
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// رقم الصفحة الحالية
    /// </summary>
    public int CurrentPage { get; set; }
    
    /// <summary>
    /// إجمالي عدد الصفحات
    /// </summary>
    public int TotalPages { get; set; }
}

/// <summary>
/// بيانات الحجز
/// </summary>
public class BookingDto
{
    /// <summary>
    /// معرف الحجز
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// رقم الحجز
    /// </summary>
    public string BookingNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// اسم الكيان
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;
    
    /// <summary>
    /// اسم الوحدة
    /// </summary>
    public string UnitName { get; set; } = string.Empty;
    
    /// <summary>
    /// تاريخ الوصول
    /// </summary>
    public DateTime CheckIn { get; set; }
    
    /// <summary>
    /// تاريخ المغادرة
    /// </summary>
    public DateTime CheckOut { get; set; }
    
    /// <summary>
    /// عدد الضيوف
    /// </summary>
    public int GuestsCount { get; set; }
    
    /// <summary>
    /// السعر الإجمالي
    /// </summary>
    public decimal TotalPrice { get; set; }
    
    /// <summary>
    /// العملة
    /// </summary>
    public string Currency { get; set; } = string.Empty;
    
    /// <summary>
    /// حالة الحجز
    /// </summary>
    public BookingStatus Status { get; set; }
    
    /// <summary>
    /// تاريخ الحجز
    /// </summary>
    public DateTime BookedAt { get; set; }
    
    /// <summary>
    /// رابط صورة الكيان
    /// </summary>
    public string PropertyImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// هل يمكن إلغاء الحجز
    /// </summary>
    public bool CanCancel { get; set; }
    
    /// <summary>
    /// هل يمكن تقييم الحجز
    /// </summary>
    public bool CanReview { get; set; }
}
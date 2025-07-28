using System;
using System.ComponentModel.DataAnnotations.Schema;
using YemenBooking.Core.Enums;

namespace YemenBooking.Core.Entities;

/// <summary>
/// ملف توافق لإتاحة خصائص إضافية مطلوبة من شيفرة MobileApp دون تغيير المخطط الفعلي لقاعدة البيانات.
/// جميع الخصائص مزينة بــ [NotMapped] حتى لا تُنشئ أعمدة جديدة، وتقوم فقط بإعادة توجيه (Proxy) للخصائص الأصلية.
/// </summary>
public static class CompatibilityAliases { }

// -------------------------------------------------
// Property aliases
// -------------------------------------------------
public partial class Property
{
    /// <summary>
    /// معرف نوع العقار (توجيه إلى TypeId الحالى).
    /// </summary>
    [NotMapped]
    public Guid PropertyTypeId
    {
        get => TypeId;
        set => TypeId = value;
    }

    /// <summary>
    /// مجموعة وسائل الراحة للعقار (توجيه إلى Amenities الحالى).
    /// </summary>
    [NotMapped]
    public ICollection<PropertyAmenity> PropertyAmenities
    {
        get => Amenities;
        set => Amenities = value;
    }

    /// <summary>
    /// العملة – تُرجع العملة من أقل وحدة سعر موجودة إذا توفرت.
    /// </summary>
    [NotMapped]
    public string? Currency => "USD"; // قيمة افتراضية – يمكن تحسينها لاحقاً.

    /// <summary>
    /// عنوان URL للصورة الرئيسية – إرجاع أول صورة متاحة.
    /// </summary>
    [NotMapped]
    public string? MainImageUrl => Images?.FirstOrDefault()?.Url;

    /// <summary>
    /// الدولة – تُعاد فارغة حالياً لعدم وجود الحقل.
    /// </summary>
    [NotMapped]
    public string? Country => null;
}

// -------------------------------------------------
// Booking aliases
// -------------------------------------------------
public partial class Booking
{
    /// <summary>
    /// رقم الحجز – يُستخدم كرقم مرجعى. نُولد من Id للحفاظ على عدم تغيير المخطط.
    /// </summary>
    [NotMapped]
    public string BookingNumber
    {
        get => Id.ToString();
        // التعيين يتجاهل القيمة حيث أنه رقم مستمد.
        set { /* Ignored */ }
    }

    /// <summary>
    /// المبلغ الإجمالى كقيمة رقمية (توجيه إلى TotalPrice.Amount).
    /// </summary>
    [NotMapped]
    public decimal TotalAmount
    {
        get => TotalPrice?.Amount ?? 0m;
        set
        {
            if (TotalPrice == null)
                TotalPrice = new YemenBooking.Core.ValueObjects.Money(value, TotalPrice?.Currency ?? "USD");
            else
                TotalPrice = TotalPrice with { Amount = value };
        }
    }

    /// <summary>
    /// العملة المستخدمة فى الحجز.
    /// </summary>
    [NotMapped]
    public string? Currency
    {
        get => TotalPrice?.Currency;
        set
        {
            if (TotalPrice == null)
                TotalPrice = new YemenBooking.Core.ValueObjects.Money(0m, value ?? "USD");
            else
                TotalPrice = TotalPrice with { Currency = value ?? TotalPrice.Currency };
        }
    }

    /// <summary>
    /// حالة الدفع للحجز. تُشتق من آخر عملية دفع ناجحة/فاشلة.
    /// </summary>
    [NotMapped]
    public PaymentStatus? PaymentStatus { get; set; }
}
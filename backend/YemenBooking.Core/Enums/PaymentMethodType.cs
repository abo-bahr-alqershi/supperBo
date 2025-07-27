namespace YemenBooking.Core.Enums;

/// <summary>
/// أنواع طرق الدفع
/// Payment method types
/// </summary>
public enum PaymentMethodType
{
    /// <summary>
    /// بطاقة ائتمان
    /// Credit card
    /// </summary>
    CreditCard = 1,

    /// <summary>
    /// بطاقة خصم
    /// Debit card
    /// </summary>
    DebitCard = 2,

    /// <summary>
    /// محفظة رقمية
    /// Digital wallet
    /// </summary>
    DigitalWallet = 3,

    /// <summary>
    /// تحويل بنكي
    /// Bank transfer
    /// </summary>
    BankTransfer = 4,

    /// <summary>
    /// دفع نقدي
    /// Cash payment
    /// </summary>
    Cash = 5,

    /// <summary>
    /// شيك
    /// Check
    /// </summary>
    Check = 6,

    /// <summary>
    /// عملة مشفرة
    /// Cryptocurrency
    /// </summary>
    Cryptocurrency = 7,

    /// <summary>
    /// دفع عند الاستلام
    /// Pay on delivery
    /// </summary>
    PayOnDelivery = 8
}

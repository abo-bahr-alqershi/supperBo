namespace YemenBooking.Core.Enums;

/// <summary>
/// تعداد أنواع طرق الدفع
/// Payment method types enumeration
/// </summary>
public enum PaymentMethodType
{
    /// <summary>
    /// بطاقة ائتمان
    /// Credit card
    /// </summary>
    CreditCard,
    
    /// <summary>
    /// بطاقة خصم
    /// Debit card
    /// </summary>
    DebitCard,
    
    /// <summary>
    /// نقدي
    /// Cash
    /// </summary>
    Cash,
    
    /// <summary>
    /// محفظة إلكترونية
    /// Digital wallet
    /// </summary>
    DigitalWallet,
    
    /// <summary>
    /// تحويل بنكي
    /// Bank transfer
    /// </summary>
    BankTransfer
}

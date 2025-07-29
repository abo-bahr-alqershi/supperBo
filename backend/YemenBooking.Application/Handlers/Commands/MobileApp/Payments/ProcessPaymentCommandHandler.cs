using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Payments;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Payments;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Enums;
using System.Text.RegularExpressions;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Payments;

/// <summary>
/// معالج أمر معالجة الدفع
/// Handler for process payment command
/// </summary>
public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, ResultDto<YemenBooking.Application.DTOs.Payments.ProcessPaymentResponse>>
{
    private readonly IPaymentService _paymentService;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    /// <summary>
    /// منشئ معالج أمر معالجة الدفع
    /// Constructor for process payment command handler
    /// </summary>
    /// <param name="paymentService">خدمة الدفع</param>
    /// <param name="bookingRepository">مستودع الحجوزات</param>
    /// <param name="paymentRepository">مستودع المدفوعات</param>
    /// <param name="paymentMethodRepository">مستودع طرق الدفع</param>
    /// <param name="logger">مسجل الأحداث</param>
    public ProcessPaymentCommandHandler(
        IPaymentService paymentService,
        IBookingRepository bookingRepository,
        IPaymentRepository paymentRepository,
        IPaymentMethodRepository paymentMethodRepository,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _paymentService = paymentService;
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة أمر معالجة الدفع
    /// Handle process payment command
    /// </summary>
    /// <param name="request">طلب معالجة الدفع</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة العملية</returns>
    public async Task<ResultDto<YemenBooking.Application.DTOs.Payments.ProcessPaymentResponse>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء عملية معالجة الدفع للحجز: {BookingId}", request.BookingId);

            // التحقق من صحة البيانات المدخلة
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // البحث عن الحجز
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
            {
                _logger.LogWarning("لم يتم العثور على الحجز: {BookingId}", request.BookingId);
                return ResultDto<ProcessPaymentResponse>.Failed("الحجز غير موجود", "BOOKING_NOT_FOUND");
            }

            // التحقق من حالة الحجز
            if (booking.Status != BookingStatus.Pending)
            {
                _logger.LogWarning("محاولة دفع لحجز غير في حالة الانتظار: {BookingId}, Status: {Status}", 
                    request.BookingId, booking.Status);
                return ResultDto<ProcessPaymentResponse>.Failed("الحجز غير قابل للدفع في الحالة الحالية", "BOOKING_NOT_PAYABLE");
            }

            // التحقق من تطابق المبلغ
            if (request.Amount.Amount != booking.TotalPrice)
            {
                _logger.LogWarning("مبلغ الدفع غير متطابق مع مبلغ الحجز. المطلوب: {BookingAmount}, المرسل: {PaymentAmount}", 
                    booking.TotalPrice, request.Amount.Amount);
                return ResultDto<ProcessPaymentResponse>.Failed("مبلغ الدفع غير متطابق مع مبلغ الحجز", "AMOUNT_MISMATCH");
            }

            // التحقق من عدم وجود دفعة ناجحة مسبقاً
            var allPayments = await _paymentRepository.GetAllAsync(cancellationToken);
            var existingPayment = allPayments?.FirstOrDefault(p => 
                p.BookingId == request.BookingId && 
                p.Status == PaymentStatus.Successful);
            if (existingPayment != null)
            {
                _logger.LogWarning("يوجد دفعة ناجحة مسبقاً للحجز: {BookingId}", request.BookingId);
                return ResultDto<ProcessPaymentResponse>.Failed("تم دفع مبلغ هذا الحجز مسبقاً", "ALREADY_PAID");
            }

            // معالجة الدفع حسب طريقة الدفع
            var paymentResult = await ProcessPaymentByMethod(request, booking, cancellationToken);
            if (paymentResult == null)
            {
                _logger.LogError("فشل في معالجة الدفع للحجز: {BookingId}", request.BookingId);
                return ResultDto<ProcessPaymentResponse>.Failed("فشل في معالجة الدفع", "PAYMENT_PROCESSING_FAILED");
            }

            // حفظ معلومات الدفع في قاعدة البيانات
            if (!Enum.TryParse<PaymentMethodType>(request.PaymentMethod.ToString(), true, out var paymentMethodType))
            {
                _logger.LogError("طريقة الدفع غير صالحة");
                return ResultDto<ProcessPaymentResponse>.Failed("طريقة الدفع غير صالحة", "INVALID_PAYMENT_METHOD");
            }

            // البحث عن PaymentMethod entity (تنفيذ مبسط)
            var allPaymentMethods = await _paymentMethodRepository.GetByTypeAsync(paymentMethodType, cancellationToken);
            var paymentMethod = allPaymentMethods?.FirstOrDefault();
            if (paymentMethod == null)
            {
                _logger.LogError("لم يتم العثور على طريقة الدفع: {PaymentMethod}", paymentMethodType);
                return ResultDto<ProcessPaymentResponse>.Failed("طريقة الدفع غير متاحة", "PAYMENT_METHOD_NOT_FOUND");
            }

            var payment = new YemenBooking.Core.Entities.Payment 
            {
                Id = Guid.NewGuid(),
                BookingId = request.BookingId,
                Amount = request.Amount,
                Method = paymentMethod, // استخدام PaymentMethod entity من Repository
                TransactionId = paymentResult.TransactionId ?? string.Empty,
                Status = paymentResult.Success ? PaymentStatus.Successful : PaymentStatus.Failed,
                ProcessedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            var savedPayment = await _paymentRepository.AddAsync(payment, cancellationToken);

            if (paymentResult.Success)
            {
                // تحديث حالة الحجز إلى مدفوع
                booking.Status = BookingStatus.Confirmed;
                booking.UpdatedAt = DateTime.UtcNow;

                await _bookingRepository.UpdateAsync(booking, cancellationToken);

                _logger.LogInformation("تم إكمال الدفع بنجاح للحجز: {BookingId}, TransactionId: {TransactionId}", 
                    request.BookingId, paymentResult.TransactionId);
            }
            else if (!paymentResult.Success)
            {
                _logger.LogWarning("فشل في الدفع للحجز: {BookingId}, Reason: {Message}", 
                    request.BookingId, paymentResult.Message);
            }

            return ResultDto<ProcessPaymentResponse>.Ok(paymentResult, paymentResult.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء معالجة الدفع للحجز: {BookingId}", request.BookingId);
            return ResultDto<ProcessPaymentResponse>.Failed($"حدث خطأ أثناء معالجة الدفع: {ex.Message}", "PAYMENT_ERROR");
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate the input request
    /// </summary>
    /// <param name="request">طلب الدفع</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<ProcessPaymentResponse> ValidateRequest(ProcessPaymentCommand request)
    {
        if (request.BookingId == Guid.Empty)
        {
            return ResultDto<ProcessPaymentResponse>.Failed("معرف الحجز غير صالح", "INVALID_BOOKING_ID");
        }

        if (request.Amount == null || request.Amount.Amount <= 0)
        {
            return ResultDto<ProcessPaymentResponse>.Failed("مبلغ الدفع غير صالح", "INVALID_AMOUNT");
        }

        // التحقق من طريقة الدفع وبياناتها المطلوبة
        if (!Enum.TryParse<PaymentMethodType>(request.PaymentMethod.ToString(), true, out var paymentMethodType))
        {
            return ResultDto<ProcessPaymentResponse>.Failed("طريقة الدفع غير صالحة", "INVALID_PAYMENT_METHOD");
        }

        switch (paymentMethodType)
        {
            case PaymentMethodType.CreditCard:
                if (request.CardDetails == null)
                {
                    return ResultDto<ProcessPaymentResponse>.Failed("تفاصيل البطاقة مطلوبة", "CARD_DETAILS_REQUIRED");
                }
                
                var cardValidation = ValidateCardDetails(request.CardDetails);
                if (!cardValidation.IsSuccess)
                {
                    return cardValidation;
                }
                break;

            case PaymentMethodType.DigitalWallet:
                if (string.IsNullOrWhiteSpace(request.WalletId))
                {
                    return ResultDto<ProcessPaymentResponse>.Failed("معرف المحفظة الإلكترونية مطلوب", "WALLET_ID_REQUIRED");
                }
                break;

            case PaymentMethodType.BankTransfer:
                // لا توجد بيانات إضافية مطلوبة للتحويل البنكي
                break;

            default:
                return ResultDto<ProcessPaymentResponse>.Failed("طريقة الدفع غير مدعومة", "UNSUPPORTED_PAYMENT_METHOD");
        }

        return ResultDto<ProcessPaymentResponse>.Ok(null, "البيانات صحيحة");
    }

    /// <summary>
    /// التحقق من صحة تفاصيل البطاقة الائتمانية
    /// Validate credit card details
    /// </summary>
    /// <param name="cardDetails">تفاصيل البطاقة</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<ProcessPaymentResponse> ValidateCardDetails(CardDetails cardDetails)
    {
        // التحقق من رقم البطاقة
        if (string.IsNullOrWhiteSpace(cardDetails.CardNumber))
        {
            return ResultDto<ProcessPaymentResponse>.Failed("رقم البطاقة مطلوب", "CARD_NUMBER_REQUIRED");
        }

        // إزالة المسافات والشرطات من رقم البطاقة
        var cleanCardNumber = cardDetails.CardNumber.Replace(" ", "").Replace("-", "");
        if (!Regex.IsMatch(cleanCardNumber, @"^\d{13,19}$"))
        {
            return ResultDto<ProcessPaymentResponse>.Failed("رقم البطاقة غير صالح", "INVALID_CARD_NUMBER");
        }

        // التحقق من اسم حامل البطاقة
        if (string.IsNullOrWhiteSpace(cardDetails.CardholderName))
        {
            return ResultDto<ProcessPaymentResponse>.Failed("اسم حامل البطاقة مطلوب", "CARDHOLDER_NAME_REQUIRED");
        }

        // التحقق من تاريخ انتهاء الصلاحية
        if (cardDetails.ExpiryMonth < 1 || cardDetails.ExpiryMonth > 12)
        {
            return ResultDto<ProcessPaymentResponse>.Failed("شهر انتهاء الصلاحية غير صالح", "INVALID_EXPIRY_MONTH");
        }

        var currentYear = DateTime.Now.Year;
        if (cardDetails.ExpiryYear < currentYear || cardDetails.ExpiryYear > currentYear + 20)
        {
            return ResultDto<ProcessPaymentResponse>.Failed("سنة انتهاء الصلاحية غير صالحة", "INVALID_EXPIRY_YEAR");
        }

        // التحقق من عدم انتهاء صلاحية البطاقة
        var expiryDate = new DateTime(cardDetails.ExpiryYear, cardDetails.ExpiryMonth, 1).AddMonths(1).AddDays(-1);
        if (expiryDate < DateTime.Now.Date)
        {
            return ResultDto<ProcessPaymentResponse>.Failed("البطاقة منتهية الصلاحية", "CARD_EXPIRED");
        }

        // التحقق من رمز الأمان CVV
        if (string.IsNullOrWhiteSpace(cardDetails.CVV))
        {
            return ResultDto<ProcessPaymentResponse>.Failed("رمز الأمان CVV مطلوب", "CVV_REQUIRED");
        }

        if (!Regex.IsMatch(cardDetails.CVV, @"^\d{3,4}$"))
        {
            return ResultDto<ProcessPaymentResponse>.Failed("رمز الأمان CVV غير صالح", "INVALID_CVV");
        }

        return ResultDto<ProcessPaymentResponse>.Ok(null, "تفاصيل البطاقة صحيحة");
    }

    /// <summary>
    /// معالجة الدفع حسب طريقة الدفع
    /// Process payment by payment method
    /// </summary>
    /// <param name="request">طلب الدفع</param>
    /// <param name="booking">الحجز</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة معالجة الدفع</returns>
    private async Task<ProcessPaymentResponse?> ProcessPaymentByMethod(
        ProcessPaymentCommand request, 
        dynamic booking, 
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PaymentMethodType>(request.PaymentMethod.ToString(), true, out var paymentMethodType))
        {
            _logger.LogError("طريقة الدفع غير صالحة");
            return null;
        }

        // البحث عن PaymentMethod entity
        var paymentMethods = await _paymentMethodRepository.GetByTypeAsync(paymentMethodType, cancellationToken);
        var paymentMethod = paymentMethods?.FirstOrDefault();
        if (paymentMethod == null)
        {
            _logger.LogError("لم يتم العثور على طريقة الدفع: {PaymentMethod}", paymentMethodType);
            return null;
        }

        return paymentMethodType switch
        {
            PaymentMethodType.CreditCard => ConvertToProcessPaymentResponse(await _paymentService.ProcessPaymentAsync(
                request.BookingId,
                paymentMethod.Id,
                request.Amount.Amount,
                request.Amount.Currency ?? "YER")),

            PaymentMethodType.DigitalWallet => ConvertToProcessPaymentResponse(await _paymentService.ProcessPaymentAsync(
                request.BookingId,
                paymentMethod.Id,
                request.Amount.Amount,
                request.Amount.Currency ?? "YER")),

            PaymentMethodType.BankTransfer => ConvertToProcessPaymentResponse(await _paymentService.ProcessPaymentAsync(
                request.BookingId,
                paymentMethod.Id,
                request.Amount.Amount,
                request.Amount.Currency ?? "YER")),

            _ => null
        };
    }

    /// <summary>
    /// تحويل PaymentResult إلى ProcessPaymentResponse
    /// </summary>
    private YemenBooking.Application.DTOs.Payments.ProcessPaymentResponse? ConvertToProcessPaymentResponse(PaymentResult? paymentResult)
    {
        if (paymentResult == null)
            return null;

        return new YemenBooking.Application.DTOs.Payments.ProcessPaymentResponse
        {
            TransactionId = paymentResult.TransactionId ?? string.Empty,
            Success = paymentResult.IsSuccess,
            Message = paymentResult.Message ?? string.Empty,
            ProcessedAmount = paymentResult.ProcessedAmount,
            Fees = paymentResult.Fees,
            Currency = "YER",
            ProcessedAt = paymentResult.ProcessedAt,
            PaymentStatus = paymentResult.IsSuccess ? "Completed" : "Failed"
        };
    }
}

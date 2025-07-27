using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.MobileApp.Payments;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Payments;

/// <summary>
/// معالج استعلام الحصول على مدفوعات المستخدم للعميل
/// Handler for client get user payments query
/// </summary>
public class ClientGetUserPaymentsQueryHandler : IRequestHandler<ClientGetUserPaymentsQuery, ResultDto<PaginatedResult<ClientPaymentDto>>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ClientGetUserPaymentsQueryHandler> _logger;

    /// <summary>
    /// منشئ معالج استعلام مدفوعات المستخدم للعميل
    /// Constructor for client get user payments query handler
    /// </summary>
    /// <param name="paymentRepository">مستودع المدفوعات</param>
    /// <param name="bookingRepository">مستودع الحجوزات</param>
    /// <param name="userRepository">مستودع المستخدمين</param>
    /// <param name="logger">مسجل الأحداث</param>
    public ClientGetUserPaymentsQueryHandler(
        IPaymentRepository paymentRepository,
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        ILogger<ClientGetUserPaymentsQueryHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام الحصول على مدفوعات المستخدم للعميل
    /// Handle client get user payments query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة مقسمة من مدفوعات المستخدم</returns>
    public async Task<ResultDto<PaginatedResult<ClientPaymentDto>>> Handle(ClientGetUserPaymentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء استعلام مدفوعات المستخدم للعميل. معرف المستخدم: {UserId}, الصفحة: {PageNumber}, الحجم: {PageSize}", 
                request.UserId, request.PageNumber, request.PageSize);

            // التحقق من صحة البيانات المدخلة
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // التحقق من وجود المستخدم
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("لم يتم العثور على المستخدم: {UserId}", request.UserId);
                return ResultDto<PaginatedResult<ClientPaymentDto>>.Failed("المستخدم غير موجود", "USER_NOT_FOUND");
            }

            // الحصول على جميع مدفوعات المستخدم
            var allPayments = await _paymentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            
            if (allPayments == null || !allPayments.Any())
            {
                _logger.LogInformation("لا توجد مدفوعات للمستخدم: {UserId}", request.UserId);
                
                var emptyResult = new PaginatedResult<ClientPaymentDto>
                {
                    Items = new List<ClientPaymentDto>(),
                    TotalCount = 0,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = 0
                };

                return ResultDto<PaginatedResult<ClientPaymentDto>>.Ok(
                    emptyResult, 
                    "لا توجد مدفوعات لهذا المستخدم"
                );
            }

            // تطبيق الفلاتر
            var filteredPayments = ApplyFilters(allPayments, request);

            // ترتيب المدفوعات (الأحدث أولاً)
            var sortedPayments = filteredPayments.OrderByDescending(p => p.CreatedAt).ToList();

            // تحويل إلى DTOs مع جلب البيانات المرتبطة
            var paymentDtos = new List<ClientPaymentDto>();

            foreach (var payment in sortedPayments)
            {
                // الحصول على تفاصيل الحجز
                var booking = await _bookingRepository.GetByIdAsync(payment.BookingId, cancellationToken);
                
                var paymentDto = new ClientPaymentDto
                {
                    Id = payment.Id,
                    BookingId = payment.BookingId,
                    BookingNumber = booking?.BookingNumber ?? "غير متاح",
                    PropertyName = booking?.Property?.Name ?? "غير متاح",
                    UnitName = booking?.Unit?.Name ?? "غير متاح",
                    Amount = payment.Amount,
                    Currency = payment.Currency ?? "YER",
                    PaymentMethod = GetPaymentMethodDisplayName(payment.PaymentMethod),
                    Status = GetPaymentStatusDisplayName(payment.Status),
                    CreatedAt = payment.CreatedAt,
                    ProcessedAt = payment.ProcessedAt,
                    ExternalReference = payment.ExternalReference,
                    InvoiceNumber = payment.InvoiceNumber,
                    Notes = payment.Notes,
                    FailureReason = payment.FailureReason,
                    Fees = payment.Fees,
                    Taxes = payment.Taxes,
                    NetAmount = payment.Amount - payment.Fees - payment.Taxes,
                    CanRefund = CanPaymentBeRefunded(payment),
                    RefundExpiryDate = CalculateRefundExpiryDate(payment)
                };

                paymentDtos.Add(paymentDto);
            }

            // تطبيق التصفح
            var totalCount = paymentDtos.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            
            var pagedPayments = paymentDtos
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var paginatedResult = new PaginatedResult<ClientPaymentDto>
            {
                Items = pagedPayments,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            _logger.LogInformation("تم العثور على {TotalCount} مدفوعة للمستخدم {UserId}, عرض الصفحة {PageNumber} من {TotalPages}", 
                totalCount, request.UserId, request.PageNumber, totalPages);

            return ResultDto<PaginatedResult<ClientPaymentDto>>.Ok(
                paginatedResult, 
                $"تم العثور على {totalCount} مدفوعة"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء الحصول على مدفوعات المستخدم للعميل. معرف المستخدم: {UserId}", request.UserId);
            return ResultDto<PaginatedResult<ClientPaymentDto>>.Failed(
                $"حدث خطأ أثناء الحصول على المدفوعات: {ex.Message}", 
                "GET_USER_PAYMENTS_ERROR"
            );
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate request data
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<PaginatedResult<ClientPaymentDto>> ValidateRequest(ClientGetUserPaymentsQuery request)
    {
        if (request.UserId == Guid.Empty)
        {
            _logger.LogWarning("معرف المستخدم مطلوب");
            return ResultDto<PaginatedResult<ClientPaymentDto>>.Failed("معرف المستخدم مطلوب", "USER_ID_REQUIRED");
        }

        if (request.PageNumber < 1)
        {
            _logger.LogWarning("رقم الصفحة يجب أن يكون أكبر من صفر");
            return ResultDto<PaginatedResult<ClientPaymentDto>>.Failed("رقم الصفحة يجب أن يكون أكبر من صفر", "INVALID_PAGE_NUMBER");
        }

        if (request.PageSize < 1 || request.PageSize > 100)
        {
            _logger.LogWarning("حجم الصفحة يجب أن يكون بين 1 و 100");
            return ResultDto<PaginatedResult<ClientPaymentDto>>.Failed("حجم الصفحة يجب أن يكون بين 1 و 100", "INVALID_PAGE_SIZE");
        }

        if (request.FromDate.HasValue && request.ToDate.HasValue && request.FromDate > request.ToDate)
        {
            _logger.LogWarning("تاريخ البداية يجب أن يكون قبل تاريخ النهاية");
            return ResultDto<PaginatedResult<ClientPaymentDto>>.Failed("تاريخ البداية يجب أن يكون قبل تاريخ النهاية", "INVALID_DATE_RANGE");
        }

        if (request.MinAmount.HasValue && request.MinAmount.Value < 0)
        {
            _logger.LogWarning("الحد الأدنى للمبلغ لا يمكن أن يكون سالباً");
            return ResultDto<PaginatedResult<ClientPaymentDto>>.Failed("الحد الأدنى للمبلغ لا يمكن أن يكون سالباً", "INVALID_MIN_AMOUNT");
        }

        if (request.MaxAmount.HasValue && request.MaxAmount.Value < 0)
        {
            _logger.LogWarning("الحد الأقصى للمبلغ لا يمكن أن يكون سالباً");
            return ResultDto<PaginatedResult<ClientPaymentDto>>.Failed("الحد الأقصى للمبلغ لا يمكن أن يكون سالباً", "INVALID_MAX_AMOUNT");
        }

        if (request.MinAmount.HasValue && request.MaxAmount.HasValue && request.MinAmount > request.MaxAmount)
        {
            _logger.LogWarning("الحد الأدنى للمبلغ يجب أن يكون أقل من الحد الأقصى");
            return ResultDto<PaginatedResult<ClientPaymentDto>>.Failed("الحد الأدنى للمبلغ يجب أن يكون أقل من الحد الأقصى", "INVALID_AMOUNT_RANGE");
        }

        return ResultDto<PaginatedResult<ClientPaymentDto>>.Ok(null, "البيانات صحيحة");
    }

    /// <summary>
    /// تطبيق الفلاتر على المدفوعات
    /// Apply filters to payments
    /// </summary>
    /// <param name="payments">قائمة المدفوعات</param>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>المدفوعات المفلترة</returns>
    private IEnumerable<Core.Entities.Payment> ApplyFilters(IEnumerable<Core.Entities.Payment> payments, ClientGetUserPaymentsQuery request)
    {
        var filteredPayments = payments.AsEnumerable();

        // فلتر حسب الحالة
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            filteredPayments = filteredPayments.Where(p => 
                string.Equals(p.Status, request.Status, StringComparison.OrdinalIgnoreCase));
        }

        // فلتر حسب طريقة الدفع
        if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
        {
            filteredPayments = filteredPayments.Where(p => 
                string.Equals(p.PaymentMethod, request.PaymentMethod, StringComparison.OrdinalIgnoreCase));
        }

        // فلتر حسب التاريخ
        if (request.FromDate.HasValue)
        {
            filteredPayments = filteredPayments.Where(p => p.CreatedAt.Date >= request.FromDate.Value.Date);
        }

        if (request.ToDate.HasValue)
        {
            filteredPayments = filteredPayments.Where(p => p.CreatedAt.Date <= request.ToDate.Value.Date);
        }

        // فلتر حسب المبلغ
        if (request.MinAmount.HasValue)
        {
            filteredPayments = filteredPayments.Where(p => p.Amount >= request.MinAmount.Value);
        }

        if (request.MaxAmount.HasValue)
        {
            filteredPayments = filteredPayments.Where(p => p.Amount <= request.MaxAmount.Value);
        }

        return filteredPayments;
    }

    /// <summary>
    /// الحصول على اسم طريقة الدفع للعرض
    /// Get payment method display name
    /// </summary>
    /// <param name="paymentMethod">طريقة الدفع</param>
    /// <returns>اسم طريقة الدفع للعرض</returns>
    private string GetPaymentMethodDisplayName(string? paymentMethod)
    {
        return paymentMethod?.ToLowerInvariant() switch
        {
            "credit_card" => "بطاقة ائتمانية",
            "debit_card" => "بطاقة خصم",
            "bank_transfer" => "تحويل بنكي",
            "cash_on_delivery" => "الدفع عند الاستلام",
            "mobile_wallet" => "محفظة موبايل",
            "digital_wallet" => "محفظة رقمية",
            "paypal" => "باي بال",
            _ => paymentMethod ?? "غير محدد"
        };
    }

    /// <summary>
    /// الحصول على اسم حالة الدفع للعرض
    /// Get payment status display name
    /// </summary>
    /// <param name="status">حالة الدفع</param>
    /// <returns>اسم حالة الدفع للعرض</returns>
    private string GetPaymentStatusDisplayName(string? status)
    {
        return status?.ToLowerInvariant() switch
        {
            "pending" => "قيد الانتظار",
            "processing" => "قيد المعالجة",
            "completed" => "مكتمل",
            "failed" => "فاشل",
            "cancelled" => "ملغي",
            "refunded" => "مسترد",
            "partially_refunded" => "مسترد جزئياً",
            _ => status ?? "غير محدد"
        };
    }

    /// <summary>
    /// التحقق من إمكانية استرداد المدفوعة
    /// Check if payment can be refunded
    /// </summary>
    /// <param name="payment">المدفوعة</param>
    /// <returns>هل يمكن الاسترداد</returns>
    private bool CanPaymentBeRefunded(Core.Entities.Payment payment)
    {
        // لا يمكن استرداد المدفوعات الفاشلة أو الملغاة أو المستردة
        if (payment.Status?.ToLowerInvariant() is "failed" or "cancelled" or "refunded")
        {
            return false;
        }

        // يجب أن تكون المدفوعة مكتملة
        if (payment.Status?.ToLowerInvariant() != "completed")
        {
            return false;
        }

        // التحقق من انتهاء فترة الاسترداد (30 يوم)
        var refundExpiryDate = payment.ProcessedAt?.AddDays(30) ?? payment.CreatedAt.AddDays(30);
        if (DateTime.UtcNow > refundExpiryDate)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// حساب تاريخ انتهاء صلاحية الاسترداد
    /// Calculate refund expiry date
    /// </summary>
    /// <param name="payment">المدفوعة</param>
    /// <returns>تاريخ انتهاء صلاحية الاسترداد</returns>
    private DateTime? CalculateRefundExpiryDate(Core.Entities.Payment payment)
    {
        if (!CanPaymentBeRefunded(payment))
        {
            return null;
        }

        // 30 يوم من تاريخ المعالجة أو الإنشاء
        return payment.ProcessedAt?.AddDays(30) ?? payment.CreatedAt.AddDays(30);
    }
}

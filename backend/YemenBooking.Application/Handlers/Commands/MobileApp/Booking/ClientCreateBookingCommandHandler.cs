using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Commands.MobileApp.Bookings;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Enums;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Booking;

/// <summary>
/// معالج أمر إنشاء حجز جديد للعميل
/// Handler for client create booking command
/// </summary>
public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ResultDto<CreateBookingResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateBookingCommandHandler> _logger;

    public CreateBookingCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateBookingCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// معالجة أمر إنشاء حجز جديد
    /// Handle create booking command
    /// </summary>
    /// <param name="request">الطلب</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة العملية</returns>
    public async Task<ResultDto<CreateBookingResponse>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء إنشاء حجز جديد للمستخدم {UserId} في الوحدة {UnitId}", request.UserId, request.UnitId);

            // التحقق من صحة البيانات الأساسية
            var validationError = ValidateRequest(request);
            if (validationError != null)
            {
                return ResultDto<CreateBookingResponse>.Failure(validationError.Message);
            }

            // التحقق من وجود المستخدم
            var userRepo = _unitOfWork.Repository<Core.Entities.User>();
            var user = await userRepo.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("المستخدم غير موجود {UserId}", request.UserId);
                return new ResultDto<CreateBookingResponse>
                {
                    Success = false,
                    Message = "المستخدم غير موجود"
                };
            }

            // التحقق من وجود الوحدة
            var unitRepo = _unitOfWork.Repository<Core.Entities.Unit>();
            var unit = await unitRepo.GetByIdAsync(request.UnitId);
            if (unit == null)
            {
                _logger.LogWarning("الوحدة غير موجودة {UnitId}", request.UnitId);
                    return new ResultDto<CreateBookingResponse>
                    {
                        Success = false,
                        Message = "الوحدة غير موجودة"
                    };
            }

            // التحقق من توفر الوحدة في التواريخ المطلوبة
            var availabilityError = await CheckAvailability(request.UnitId, request.CheckIn, request.CheckOut);
            if (availabilityError != null)
            {
                return new ResultDto<CreateBookingResponse>
                {
                    Success = false,
                    Message = availabilityError.Message
                };
            }

            // حساب السعر الإجمالي
            var totalPrice = await CalculateTotalPrice(unit, request);

            // إنشاء الحجز
            var booking = new Core.Entities.Booking
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                UnitId = request.UnitId,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                GuestsCount = request.GuestsCount,
                Status = BookingStatus.Pending,
                BookingSource = request.BookingSource,
                TotalPrice = totalPrice,
                BookedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // إضافة الحجز إلى قاعدة البيانات
            var bookingRepo = _unitOfWork.Repository<Core.Entities.Booking>();
            await bookingRepo.AddAsync(booking);

            // حفظ التغييرات
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // إنشاء رقم الحجز
            var bookingNumber = GenerateBookingNumber(booking.Id);

            var response = new CreateBookingResponse
            {
                BookingId = booking.Id,
                BookingNumber = bookingNumber,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status,
                Message = "تم إنشاء الحجز بنجاح"
            };

            _logger.LogInformation("تم إنشاء الحجز بنجاح {BookingId} للمستخدم {UserId}", booking.Id, request.UserId);

            return new ResultDto<CreateBookingResponse>
            {
                Success = true,
                Data = response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء إنشاء الحجز للمستخدم {UserId}", request.UserId);
            return new ResultDto<CreateBookingResponse>
            {
                Success = false,
                Message = $"حدث خطأ أثناء إنشاء الحجز: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// التحقق من صحة طلب الحجز
    /// Validate booking request
    /// </summary>
    private CreateBookingResponse? ValidateRequest(CreateBookingCommand request)
    {
        if (request.UserId == Guid.Empty)
        {
            return new CreateBookingResponse
            {
                BookingId = Guid.Empty,
                Message = "معرف المستخدم مطلوب"
            };
        }

        if (request.UnitId == Guid.Empty)
        {
            return new CreateBookingResponse
            {
                BookingId = Guid.Empty,
                Message = "معرف الوحدة مطلوب"
            };
        }

        if (request.CheckIn >= request.CheckOut)
        {
            return new CreateBookingResponse
            {
                BookingId = Guid.Empty,
                Message = "تاريخ الوصول يجب أن يكون قبل تاريخ المغادرة"
            };
        }

        if (request.CheckIn < DateTime.UtcNow.Date)
        {
            return new CreateBookingResponse
            {
                BookingId = Guid.Empty,
                Message = "تاريخ الوصول لا يمكن أن يكون في الماضي"
            };
        }

        if (request.GuestsCount <= 0)
        {
            return new CreateBookingResponse
            {
                BookingId = Guid.Empty,
                Message = "عدد الضيوف يجب أن يكون أكبر من صفر"
            };
        }

        return null;
    }

    /// <summary>
    /// التحقق من توفر الوحدة
    /// Check unit availability
    /// </summary>
    private async Task<CreateBookingResponse?> CheckAvailability(Guid unitId, DateTime checkIn, DateTime checkOut)
    {
        var bookingRepo = _unitOfWork.Repository<Core.Entities.Booking>();
        var existingBookings = await bookingRepo.GetAllAsync();

        var conflictingBookings = existingBookings.Where(b =>
            b.UnitId == unitId &&
            b.Status != BookingStatus.Cancelled &&
            !(checkOut <= b.CheckIn || checkIn >= b.CheckOut)
        ).ToList();

        if (conflictingBookings.Any())
        {
            return new CreateBookingResponse
            {
                BookingId = Guid.Empty,
                Message = "الوحدة غير متاحة في التواريخ المطلوبة"
            };
        }

        return null;
    }

    /// <summary>
    /// حساب السعر الإجمالي
    /// Calculate total price
    /// </summary>
    private async Task<Money> CalculateTotalPrice(Core.Entities.Unit unit, CreateBookingCommand request)
    {
        try
        {
            // حساب عدد الليالي
            var nights = (request.CheckOut - request.CheckIn).Days;
            
            // سعر أساسي افتراضي (سيتم تحسينه لاحقاً للحصول على السعر الفعلي من الوحدة)
            var basePricePerNight = 100m; // سعر افتراضي
            var totalAmount = basePricePerNight * nights;

            // إضافة تكلفة الخدمات الإضافية (إذا وجدت)
            // TODO: تطبيق حساب الخدمات الإضافية

            return new Money(totalAmount, "YER");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حساب السعر للوحدة {UnitId}", unit.Id);
            return new Money(100m, "YER");
        }
    }

    /// <summary>
    /// إنشاء رقم الحجز
    /// Generate booking number
    /// </summary>
    private string GenerateBookingNumber(Guid bookingId)
    {
        // إنشاء رقم حجز فريد باستخدام التاريخ وجزء من معرف الحجز
        var datePrefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var idSuffix = bookingId.ToString("N")[..8].ToUpper();
        return $"BK{datePrefix}{idSuffix}";
    }
}
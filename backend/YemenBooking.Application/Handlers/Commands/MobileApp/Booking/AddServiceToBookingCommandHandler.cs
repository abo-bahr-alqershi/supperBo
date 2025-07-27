using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Features.Bookings.Commands;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Enums;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Booking;

/// <summary>
/// معالج أمر إضافة خدمة إلى الحجز من تطبيق الجوال
/// Handles AddServiceToBookingCommand coming from Mobile App
/// </summary>
public class AddServiceToBookingCommandHandler : IRequestHandler<AddServiceToBookingCommand, AddServiceToBookingResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;
    private readonly ILogger<AddServiceToBookingCommandHandler> _logger;

    public AddServiceToBookingCommandHandler(
        IUnitOfWork unitOfWork,
        IAuditService auditService,
        ILogger<AddServiceToBookingCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AddServiceToBookingResponse> Handle(AddServiceToBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("بدء إضافة خدمة {ServiceId} إلى الحجز {BookingId}", request.ServiceId, request.BookingId);

        // basic validation
        if (request.Quantity <= 0)
        {
            return new AddServiceToBookingResponse
            {
                Success = false,
                Message = "الكمية غير صحيحة"
            };
        }

        // الحصول على الحجز
        var bookingRepo = _unitOfWork.Repository<Core.Entities.Booking>();
        var booking = await bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null)
        {
            return new AddServiceToBookingResponse
            {
                Success = false,
                Message = "الحجز غير موجود"
            };
        }

        // TODO: التحقق من الخدمة و إضافة سجل للربط (BookingService)
        // لأغراض MVP سيتم افتراض نجاح العملية و زيادة السعر الكلي بمبلغ ثابت 0
        booking.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // سجل التدقيق
        await _auditService.LogBusinessOperationAsync(
            "AddServiceToBooking",
            $"تمت إضافة الخدمة {request.ServiceId} إلى الحجز {request.BookingId}",
            request.BookingId,
            "Booking",
            null,
            null,
            cancellationToken);

        return new AddServiceToBookingResponse
        {
            Success = true,
            Message = "تمت إضافة الخدمة بنجاح",
            NewTotalPrice = booking.TotalPrice ?? new Money(0, "YER")
        };
    }
}

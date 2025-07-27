using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Services;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Services
{
    /// <summary>
    /// معالج أمر إزالة خدمة من الحجز
    /// </summary>
    public class RemoveServiceFromBookingCommandHandler : IRequestHandler<RemoveServiceFromBookingCommand, ResultDto<bool>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPropertyServiceRepository _serviceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<RemoveServiceFromBookingCommandHandler> _logger;

        public RemoveServiceFromBookingCommandHandler(
            IBookingRepository bookingRepository,
            IPropertyServiceRepository serviceRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<RemoveServiceFromBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _serviceRepository = serviceRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(RemoveServiceFromBookingCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إزالة خدمة من الحجز: BookingId={BookingId}, ServiceId={ServiceId}",
                request.BookingId, request.ServiceId);

            // التحقق من المدخلات
            if (request.BookingId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الحجز مطلوب");
            if (request.ServiceId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الخدمة مطلوب");

            // التحقق من وجود الحجز
            var booking = await _bookingRepository.GetBookingByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
                return ResultDto<bool>.Failed("الحجز غير موجود");

            // التحقق من وجود الخدمة
            var service = await _serviceRepository.GetServiceByIdAsync(request.ServiceId, cancellationToken);
            if (service == null)
                return ResultDto<bool>.Failed("الخدمة غير موجودة");

            // التحقق من الصلاحيات (صاحب الحجز أو مسؤول)
            if (_currentUserService.Role != "Admin" && booking.UserId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بإزالة خدمة من هذا الحجز");

            // تنفيذ الإزالة
            bool removed = await _bookingRepository.RemoveServiceFromBookingAsync(request.BookingId, request.ServiceId, cancellationToken);
            if (!removed)
                return ResultDto<bool>.Failed("فشل إزالة الخدمة من الحجز");

            // إعادة حساب السعر
            await _bookingRepository.RecalculatePriceAsync(request.BookingId, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "RemoveServiceFromBooking",
                $"تم إزالة الخدمة {request.ServiceId} من الحجز {request.BookingId}",
                request.BookingId,
                "BookingService",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتملت عملية إزالة الخدمة من الحجز بنجاح");
            return ResultDto<bool>.Succeeded(true, "تم إزالة الخدمة من الحجز بنجاح");
        }
    }
} 
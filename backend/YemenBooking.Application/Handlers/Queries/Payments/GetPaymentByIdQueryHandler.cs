using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Payments;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Payments
{
    /// <summary>
    /// معالج استعلام الحصول على بيانات الدفع بواسطة المعرف
    /// Query handler for GetPaymentByIdQuery
    /// </summary>
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, ResultDto<PaymentDetailsDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPaymentByIdQueryHandler> _logger;

        public GetPaymentByIdQueryHandler(
            IPaymentRepository paymentRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetPaymentByIdQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDto<PaymentDetailsDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام بيانات الدفع: {PaymentId}", request.PaymentId);

            // التحقق من معرف الدفع
            if (request.PaymentId == Guid.Empty)
                return ResultDto<PaymentDetailsDto>.Failure("معرف الدفع غير صالح");

            // جلب الدفع
            var payment = await _paymentRepository.GetPaymentByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
                return ResultDto<PaymentDetailsDto>.Failure($"الدفع بالمعرف {request.PaymentId} غير موجود");

            // جلب الحجز المرتبط للتحقق من الصلاحيات
            var booking = await _paymentRepository.GetBookingByIdAsync(payment.BookingId, cancellationToken);
            if (booking == null)
                return ResultDto<PaymentDetailsDto>.Failure($"الحجز المرتبط بالدفع {request.PaymentId} غير موجود");

            // التحقق من الصلاحيات: مالك الحجز أو مسؤول
            var currentUserId = _currentUserService.UserId;
            var roles = _currentUserService.UserRoles;
            if (booking.UserId != currentUserId && !roles.Contains("Admin"))
                return ResultDto<PaymentDetailsDto>.Failure("ليس لديك صلاحية لعرض بيانات هذا الدفع");

            // تحويل إلى DTO
            var paymentDto = _mapper.Map<PaymentDto>(payment);
            var details = new PaymentDetailsDto { Payment = paymentDto };

            _logger.LogInformation("تم جلب بيانات الدفع بنجاح: {PaymentId}", request.PaymentId);
            return ResultDto<PaymentDetailsDto>.Ok(details, "تم جلب بيانات الدفع بنجاح");
        }
    }
} 
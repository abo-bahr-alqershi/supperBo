using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Payments;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace YemenBooking.Application.Handlers.Queries.Payments
{
    /// <summary>
    /// معالج استعلام الحصول على مدفوعات الحجز
    /// Query handler for GetPaymentsByBookingQuery
    /// </summary>
    public class GetPaymentsByBookingQueryHandler : IRequestHandler<GetPaymentsByBookingQuery, PaginatedResult<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPaymentsByBookingQueryHandler> _logger;

        public GetPaymentsByBookingQueryHandler(
            IPaymentRepository paymentRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetPaymentsByBookingQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<PaymentDto>> Handle(GetPaymentsByBookingQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام مدفوعات الحجز: {BookingId}, الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.BookingId, request.PageNumber, request.PageSize);

            if (request.BookingId == Guid.Empty)
                return PaginatedResult<PaymentDto>.Empty(request.PageNumber, request.PageSize);

            // التحقق من الصلاحيات: صاحب الحجز أو مسؤول
            var booking = await _paymentRepository.GetBookingByIdAsync(request.BookingId, cancellationToken);
            if (booking == null)
                return PaginatedResult<PaymentDto>.Empty(request.PageNumber, request.PageSize);

            var currentUserId = _currentUserService.UserId;
            var roles = _currentUserService.UserRoles;
            if (booking.UserId != currentUserId && !roles.Contains("Admin"))
                throw new UnauthorizedAccessException("ليس لديك صلاحية لعرض مدفوعات هذا الحجز");

            var pageNumber = Math.Max(request.PageNumber, 1);
            var pageSize = Math.Max(request.PageSize, 1);

            var payments = (await _paymentRepository.GetPaymentsByBookingAsync(request.BookingId, cancellationToken)).ToList();
            var totalCount = payments.Count;

            var items = payments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => _mapper.Map<PaymentDto>(p))
                .ToList();

            _logger.LogInformation("تم جلب {Count} دفعة من إجمالي {TotalCount} للحجز: {BookingId}", items.Count, totalCount, request.BookingId);
            return new PaginatedResult<PaymentDto>(items, pageNumber, pageSize, totalCount);
        }
    }
} 
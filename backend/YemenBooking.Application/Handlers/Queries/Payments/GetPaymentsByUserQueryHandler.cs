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

namespace YemenBooking.Application.Handlers.Queries.Payments
{
    /// <summary>
    /// معالج استعلام الحصول على مدفوعات المستخدم
    /// Query handler for GetPaymentsByUserQuery
    /// </summary>
    public class GetPaymentsByUserQueryHandler : IRequestHandler<GetPaymentsByUserQuery, PaginatedResult<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPaymentsByUserQueryHandler> _logger;

        public GetPaymentsByUserQueryHandler(
            IPaymentRepository paymentRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetPaymentsByUserQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<PaymentDto>> Handle(GetPaymentsByUserQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام مدفوعات المستخدم: {UserId}, من {StartDate} إلى {EndDate}, الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.UserId, request.StartDate, request.EndDate, request.PageNumber, request.PageSize);

            // التحقق من معرف المستخدم
            if (request.UserId == Guid.Empty)
                return PaginatedResult<PaymentDto>.Empty(request.PageNumber, request.PageSize);

            // التحقق من الصلاحيات: المستخدم نفسه أو مسؤول
            var currentUserId = _currentUserService.UserId;
            var roles = _currentUserService.UserRoles;
            if (currentUserId != request.UserId && !roles.Contains("Admin"))
                throw new UnauthorizedAccessException("ليس لديك صلاحية لعرض مدفوعات هذا المستخدم");

            var pageNumber = Math.Max(request.PageNumber, 1);
            var pageSize = Math.Max(request.PageSize, 1);

            var payments = (await _paymentRepository.GetPaymentsByUserAsync(request.UserId, request.StartDate, request.EndDate, cancellationToken)).ToList();
            var totalCount = payments.Count;

            var items = payments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => _mapper.Map<PaymentDto>(p))
                .ToList();

            _logger.LogInformation("تم جلب {Count} دفعة من إجمالي {TotalCount} لمستخدم: {UserId}", items.Count, totalCount, request.UserId);
            return new PaginatedResult<PaymentDto>(items, pageNumber, pageSize, totalCount);
        }
    }
} 
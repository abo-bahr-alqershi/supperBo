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
    /// معالج استعلام الحصول على المدفوعات حسب طريقة الدفع
    /// Query handler for GetPaymentsByMethodQuery
    /// </summary>
    public class GetPaymentsByMethodQueryHandler : IRequestHandler<GetPaymentsByMethodQuery, PaginatedResult<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPaymentsByMethodQueryHandler> _logger;

        public GetPaymentsByMethodQueryHandler(
            IPaymentRepository paymentRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetPaymentsByMethodQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<PaymentDto>> Handle(GetPaymentsByMethodQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام المدفوعات بطريقة: {Method}, من {StartDate} إلى {EndDate}, الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.PaymentMethod, request.StartDate, request.EndDate, request.PageNumber, request.PageSize);

            if (!_currentUserService.UserRoles.Contains("Admin"))
                throw new UnauthorizedAccessException("ليس لديك صلاحية لعرض المدفوعات حسب طريقة الدفع");

            var payments = (await _paymentRepository.GetPaymentsByMethodAsync(
                request.PaymentMethod, request.StartDate, request.EndDate, cancellationToken)).ToList();
            var totalCount = payments.Count;
            var pageNumber = Math.Max(request.PageNumber, 1);
            var pageSize = Math.Max(request.PageSize, 1);

            var items = payments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => _mapper.Map<PaymentDto>(p))
                .ToList();

            _logger.LogInformation("تم جلب {Count} دفعة من إجمالي {TotalCount} حيث طريقة الدفع: {Method}",
                items.Count, totalCount, request.PaymentMethod);
            return new PaginatedResult<PaymentDto>(items, pageNumber, pageSize, totalCount);
        }
    }
} 
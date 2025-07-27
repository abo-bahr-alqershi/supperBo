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
    /// معالج استعلام الحصول على المدفوعات حسب الحالة
    /// Query handler for GetPaymentsByStatusQuery
    /// </summary>
    public class GetPaymentsByStatusQueryHandler : IRequestHandler<GetPaymentsByStatusQuery, PaginatedResult<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPaymentsByStatusQueryHandler> _logger;

        public GetPaymentsByStatusQueryHandler(
            IPaymentRepository paymentRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<GetPaymentsByStatusQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<PaymentDto>> Handle(GetPaymentsByStatusQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام المدفوعات بالحالة: {Status}, الصفحة: {PageNumber}, الحجم: {PageSize}",
                request.Status, request.PageNumber, request.PageSize);

            if (!_currentUserService.UserRoles.Contains("Admin"))
                throw new UnauthorizedAccessException("ليس لديك صلاحية لعرض المدفوعات حسب الحالة");

            var payments = (await _paymentRepository.GetPaymentsByStatusAsync(request.Status, cancellationToken)).ToList();
            var totalCount = payments.Count;
            var pageNumber = Math.Max(request.PageNumber, 1);
            var pageSize = Math.Max(request.PageSize, 1);

            var items = payments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => _mapper.Map<PaymentDto>(p))
                .ToList();

            _logger.LogInformation("تم جلب {Count} دفعة من إجمالي {TotalCount} بالحالة: {Status}", items.Count, totalCount, request.Status);
            return new PaginatedResult<PaymentDto>(items, pageNumber, pageSize, totalCount);
        }
    }
} 
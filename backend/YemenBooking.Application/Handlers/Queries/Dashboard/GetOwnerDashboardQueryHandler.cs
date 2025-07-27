using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs.Dashboard;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Dashboard
{
    /// <summary>
    /// معالج استعلام بيانات لوحة تحكم المالك
    /// Handler for GetOwnerDashboardQuery
    /// </summary>
    public class GetOwnerDashboardQueryHandler : IRequestHandler<GetOwnerDashboardQuery, OwnerDashboardDto>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IReportingService _reportingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetOwnerDashboardQueryHandler> _logger;

        public GetOwnerDashboardQueryHandler(
            IPropertyRepository propertyRepository,
            IBookingRepository bookingRepository,
            IReportingService reportingService,
            ICurrentUserService currentUserService,
            ILogger<GetOwnerDashboardQueryHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _bookingRepository = bookingRepository;
            _reportingService = reportingService;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<OwnerDashboardDto> Handle(GetOwnerDashboardQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Owner Dashboard Query for Owner {OwnerId}", request.OwnerId);

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new UnauthorizedException("يجب تسجيل الدخول للوصول إلى لوحة تحكم المالك");

            var role = _currentUserService.Role;
            if (role != "Admin" && _currentUserService.UserId != request.OwnerId)
                throw new ForbiddenException("ليس لديك صلاحية لعرض بيانات لوحة تحكم المالك");

            var properties = await _propertyRepository.GetPropertiesByOwnerAsync(request.OwnerId, cancellationToken);
            var propertyCount = properties.Count();

            var bookingQuery = _bookingRepository.GetQueryable()
                .Include(b => b.Unit).ThenInclude(u => u.Property)
                .Where(b => b.Unit.Property.OwnerId == request.OwnerId
                    && b.BookedAt >= request.Range.StartDate
                    && b.BookedAt <= request.Range.EndDate);

            var bookingCount = await bookingQuery.CountAsync(cancellationToken);
            var totalRevenue = await bookingQuery.SumAsync(b => b.TotalPrice.Amount, cancellationToken);

            var ownerName = properties.FirstOrDefault()?.Owner.Name ?? string.Empty;

            return new OwnerDashboardDto
            {
                OwnerId = request.OwnerId,
                OwnerName = ownerName,
                PropertyCount = propertyCount,
                BookingCount = bookingCount,
                TotalRevenue = totalRevenue
            };
        }
    }
} 
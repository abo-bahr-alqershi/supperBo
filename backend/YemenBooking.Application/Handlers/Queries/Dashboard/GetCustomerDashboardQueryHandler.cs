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
    /// معالج استعلام بيانات لوحة تحكم العميل
    /// Handler for GetCustomerDashboardQuery
    /// </summary>
    public class GetCustomerDashboardQueryHandler : IRequestHandler<GetCustomerDashboardQuery, CustomerDashboardDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetCustomerDashboardQueryHandler> _logger;

        public GetCustomerDashboardQueryHandler(
            IUserRepository userRepository,
            IBookingRepository bookingRepository,
            ICurrentUserService currentUserService,
            ILogger<GetCustomerDashboardQueryHandler> logger)
        {
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<CustomerDashboardDto> Handle(GetCustomerDashboardQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Customer Dashboard Query for Customer {CustomerId}", request.CustomerId);

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                throw new UnauthorizedException("يجب تسجيل الدخول للوصول إلى لوحة تحكم العميل");

            var role = _currentUserService.Role;
            if (role != "Admin" && _currentUserService.UserId != request.CustomerId)
                throw new ForbiddenException("ليس لديك صلاحية لعرض بيانات لوحة تحكم العميل");

            var user = await _userRepository.GetUserByIdAsync(request.CustomerId, cancellationToken);
            if (user == null)
                throw new NotFoundException("User", request.CustomerId.ToString());

            var bookingsQuery = _bookingRepository.GetQueryable()
                .Where(b => b.UserId == request.CustomerId);

            var upcomingCount = await bookingsQuery
                .Where(b => b.CheckIn >= DateTime.UtcNow.Date)
                .CountAsync(cancellationToken);

            var pastCount = await bookingsQuery
                .Where(b => b.CheckOut < DateTime.UtcNow.Date)
                .CountAsync(cancellationToken);

            return new CustomerDashboardDto
            {
                CustomerId = user.Id,
                CustomerName = user.Name,
                UpcomingBookings = upcomingCount,
                PastBookings = pastCount,
                TotalSpent = user.TotalSpent
            };
        }
    }
} 
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Users;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Enums;
using System.Collections.Generic;
using EntityUserRole = YemenBooking.Core.Entities.UserRole; // Alias for the UserRole entity to avoid conflict with enum

namespace YemenBooking.Application.Handlers.Queries.Users
{
    /// <summary>
    /// معالج استعلام تفاصيل المستخدم
    /// </summary>
    public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, ResultDto<UserDetailsDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ILogger<GetUserDetailsQueryHandler> _logger;

        public GetUserDetailsQueryHandler(
            IUserRepository userRepository,
            IBookingRepository bookingRepository,
            IReportRepository reportRepository,
            IPaymentRepository paymentRepository,
            IReviewRepository reviewRepository,
            IPropertyRepository propertyRepository,
            IPropertyImageRepository propertyImageRepository,
            IStaffRepository staffRepository,
            ILogger<GetUserDetailsQueryHandler> logger)
        {
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _reportRepository = reportRepository;
            _paymentRepository = paymentRepository;
            _reviewRepository = reviewRepository;
            _propertyRepository = propertyRepository;
            _propertyImageRepository = propertyImageRepository;
            _staffRepository = staffRepository;
            _logger = logger;
        }

        public async Task<ResultDto<UserDetailsDto>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة استعلام GetUserDetails");

            // جلب بيانات المستخدم الأساسية
            var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return ResultDto<UserDetailsDto>.Failure("المستخدم غير موجود");

            var dto = new UserDetailsDto
            {
                Id = user.Id,
                UserName = user.Name,
                AvatarUrl = user.ProfileImage ?? string.Empty,
                Email = user.Email,
                PhoneNumber = user.Phone,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
            };

            // جلب دور المستخدم لتحديد نوع الحساب بأمان مع تجاهل القيم الفارغة
            var rolesList = (await _userRepository.GetUserRolesAsync(request.UserId, cancellationToken))
                             ?? new List<EntityUserRole>();
            var roleNames = rolesList
                             .Where(r => r.Role != null && !string.IsNullOrWhiteSpace(r.Role.Name))
                             .Select(r => r.Role.Name!);

            bool isPropertyUser = rolesList.Any(r => r.Role != null &&
                                 (string.Equals(r.Role.Name, "Owner", StringComparison.OrdinalIgnoreCase)
                                  || string.Equals(r.Role.Name, "Staff", StringComparison.OrdinalIgnoreCase)));

            if (!isPropertyUser)
            {
                // بيانات العميل
                var userBookings = await _bookingRepository.GetBookingsByUserAsync(request.UserId, cancellationToken);
                dto.BookingsCount = userBookings.Count();
                dto.CanceledBookingsCount = userBookings.Count(b => b.Status == BookingStatus.Cancelled);
                dto.PendingBookingsCount = userBookings.Count(b => b.Status == BookingStatus.Pending);
                dto.FirstBookingDate = userBookings.Any() ? userBookings.Min(b => b.CreatedAt) : (DateTime?)null;
                dto.LastBookingDate = userBookings.Any() ? userBookings.Max(b => b.CreatedAt) : (DateTime?)null;

                var reportsCreated = await _reportRepository.GetReportsAsync(request.UserId, null, null, cancellationToken);
                dto.ReportsCreatedCount = reportsCreated.Count();
                var reportsAgainst = await _reportRepository.GetReportsAsync(null, request.UserId, null, cancellationToken);
                dto.ReportsAgainstCount = reportsAgainst.Count();

                var payments = await _paymentRepository.GetPaymentsByUserAsync(request.UserId, null, null, cancellationToken);
                dto.TotalPayments = payments.Sum(p => p.Amount.Amount);
                decimal totalRefunds = 0;
                foreach (var payment in payments)
                {
                    var refunds = await _paymentRepository.GetRefundsForPaymentAsync(payment.Id, cancellationToken);
                    totalRefunds += refunds.Sum(r => r.Amount.Amount);
                }
                dto.TotalRefunds = totalRefunds;

                var reviews = await _reviewRepository.GetReviewsByUserAsync(request.UserId, cancellationToken);
                dto.ReviewsCount = reviews.Count();
            }
            else
            {
                // بيانات المالك أو الموظف
                dto.Role = rolesList.Any(r => r.Role != null && string.Equals(r.Role.Name, "Owner", StringComparison.OrdinalIgnoreCase))
                           ? "Owner" : "Staff";

                Guid propertyId;
                if (dto.Role == "Owner")
                {
                    var properties = await _propertyRepository.GetPropertiesByOwnerAsync(request.UserId, cancellationToken);
                    var firstProperty = properties.FirstOrDefault();
                    propertyId = firstProperty?.Id ?? Guid.Empty;
                }
                else
                {
                    var staff = await _staffRepository.GetStaffByUserAsync(request.UserId, cancellationToken);
                    propertyId = staff?.PropertyId ?? Guid.Empty;
                }

                if (propertyId != Guid.Empty)
                {
                    var property = await _propertyRepository.GetPropertyByIdAsync(propertyId, cancellationToken);
                    dto.PropertyId = propertyId;
                    dto.PropertyName = property?.Name ?? string.Empty;

                    var propertyWithUnits = await _propertyRepository.GetPropertyWithUnitsAsync(propertyId, cancellationToken);
                    dto.UnitsCount = propertyWithUnits?.Units.Count() ?? 0;

                    var propImages = await _propertyImageRepository.GetImagesByPropertyAsync(propertyId, cancellationToken);
                    dto.PropertyImagesCount = propImages.Count();

                    int unitImagesCount = 0;
                    if (propertyWithUnits?.Units != null)
                    {
                        foreach (var unit in propertyWithUnits.Units)
                        {
                            var unitImages = await _propertyImageRepository.GetImagesByUnitAsync(unit.Id, cancellationToken);
                            unitImagesCount += unitImages.Count();
                        }
                    }
                    dto.UnitImagesCount = unitImagesCount;

                    var propBookings = await _bookingRepository.GetBookingsByPropertyAsync(propertyId, null, null, cancellationToken);
                    dto.BookingsCount = propBookings.Count();
                    dto.CanceledBookingsCount = propBookings.Count(b => b.Status == BookingStatus.Cancelled);
                    dto.PendingBookingsCount = propBookings.Count(b => b.Status == BookingStatus.Pending);
                    dto.FirstBookingDate = propBookings.Any() ? propBookings.Min(b => b.CreatedAt) : dto.FirstBookingDate;
                    dto.LastBookingDate = propBookings.Any() ? propBookings.Max(b => b.CreatedAt) : dto.LastBookingDate;

                    decimal propTotalPayments = 0;
                    decimal propTotalRefunds = 0;
                    foreach (var booking in propBookings)
                    {
                        var bookingPayments = await _paymentRepository.GetPaymentsByBookingAsync(booking.Id, cancellationToken);
                        propTotalPayments += bookingPayments.Sum(p => p.Amount.Amount);
                        foreach (var payment in bookingPayments)
                        {
                            var bookingRefunds = await _paymentRepository.GetRefundsForPaymentAsync(payment.Id, cancellationToken);
                            propTotalRefunds += bookingRefunds.Sum(r => r.Amount.Amount);
                        }
                    }
                    dto.TotalPayments = propTotalPayments;
                    dto.TotalRefunds = propTotalRefunds;
                    dto.NetRevenue = propTotalPayments - propTotalRefunds;

                    // عدد البلاغات على الكيان
                    var propertyReports = await _reportRepository.GetReportsAsync(null, null, propertyId, cancellationToken);
                    dto.ReportsAgainstCount = propertyReports.Count();

                    // عدد البلاغات التي قام بها المالك أو الموظفين على المستخدمين
                    int reportsByPropertyCount = 0;
                    var staffList = await _staffRepository.GetStaffByPropertyAsync(propertyId, cancellationToken);
                    foreach (var st in staffList)
                    {
                        var staffReports = await _reportRepository.GetReportsAsync(st.UserId, null, null, cancellationToken);
                        reportsByPropertyCount += staffReports.Count();
                    }
                    var ownerReports = await _reportRepository.GetReportsAsync(request.UserId, null, null, cancellationToken);
                    reportsByPropertyCount += ownerReports.Count();
                    dto.ReportsCreatedCount = reportsByPropertyCount;

                    // عدد المراجعات على الكيان
                    var propertyReviews = await _reviewRepository.GetReviewsByPropertyAsync(propertyId, cancellationToken);
                    dto.ReviewsCount = propertyReviews.Count();

                    // عدد الردود (غير مستخدم حاليًا)
                    dto.RepliesCount = 0;
                }
            }

            _logger.LogInformation("تم جلب تفاصيل المستخدم بنجاح: {UserId}", request.UserId);
            return ResultDto<UserDetailsDto>.Ok(dto, "تم جلب تفاصيل المستخدم بنجاح");
        }
    }
} 
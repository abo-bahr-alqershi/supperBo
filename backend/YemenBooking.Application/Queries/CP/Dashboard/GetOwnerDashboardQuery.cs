using System;
using MediatR;
using YemenBooking.Application.DTOs.Dashboard;

namespace YemenBooking.Application.Queries.Dashboard
{
    /// <summary>
    /// استعلام للحصول على بيانات لوحة تحكم المالك ضمن نطاق زمني
    /// Query to retrieve owner dashboard data within a date range
    /// </summary>
    public class GetOwnerDashboardQuery : IRequest<OwnerDashboardDto>
    {
        /// <summary>
        /// معرف المالك
        /// Owner identifier
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// النطاق الزمني
        /// Date range for the dashboard data
        /// </summary>
        public DateRangeDto Range { get; set; }

        public GetOwnerDashboardQuery(Guid ownerId, DateRangeDto range)
        {
            OwnerId = ownerId;
            Range = range;
        }
    }
} 
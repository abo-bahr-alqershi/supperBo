using System;
using MediatR;
using YemenBooking.Application.DTOs.Dashboard;

namespace YemenBooking.Application.Queries.Dashboard
{
    /// <summary>
    /// استعلام للحصول على بيانات لوحة تحكم العميل
    /// Query to retrieve customer dashboard data
    /// </summary>
    public class GetCustomerDashboardQuery : IRequest<CustomerDashboardDto>
    {
        /// <summary>
        /// معرف العميل
        /// Customer identifier
        /// </summary>
        public Guid CustomerId { get; set; }

        public GetCustomerDashboardQuery(Guid customerId)
        {
            CustomerId = customerId;
        }
    }
} 
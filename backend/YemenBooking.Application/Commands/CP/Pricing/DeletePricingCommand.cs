using MediatR;
using System;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Pricing
{
    /// <summary>
    /// أمر لحذف قاعدة تسعير
    /// Command to delete a pricing rule
    /// </summary>
    public class DeletePricingCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف قاعدة التسعير
        /// Pricing rule identifier
        /// </summary>
        public Guid PricingRuleId { get; set; }
    }
} 
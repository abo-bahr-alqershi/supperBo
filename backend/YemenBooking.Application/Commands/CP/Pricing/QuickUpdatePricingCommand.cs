using MediatR;
using System;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Pricing
{
    /// <summary>
    /// أمر للتحديث السريع للسعر
    /// Command to quick update pricing for a unit
    /// </summary>
    public class QuickUpdatePricingCommand : IRequest<ResultDto<IEnumerable<PricingRuleDto>>>
    {
        /// <summary>
        /// معرف الوحدة
        /// Unit identifier
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// تاريخ بداية الفترة
        /// Start date of period
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ نهاية الفترة
        /// End date of period
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// المبلغ الجديد للسعر
        /// New price amount
        /// </summary>
        public decimal PriceAmount { get; set; }

        /// <summary>
        /// عملة السعر
        /// Currency code
        /// </summary>
        public string Currency { get; set; } = null!;
    }
} 
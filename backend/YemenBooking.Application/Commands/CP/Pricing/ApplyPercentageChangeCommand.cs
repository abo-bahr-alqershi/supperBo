using MediatR;
using System;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Pricing
{
    /// <summary>
    /// أمر لتطبيق نسبة مئوية على قواعد التسعير
    /// Command to apply percentage change to pricing rules
    /// </summary>
    public class ApplyPercentageChangeCommand : IRequest<ResultDto<IEnumerable<PricingRuleDto>>>
    {
        /// <summary>
        /// قائمة معرفات الوحدات
        /// List of unit identifiers
        /// </summary>
        public IEnumerable<Guid> UnitIds { get; set; } = new List<Guid>();

        /// <summary>
        /// النسبة المئوية للتطبيق
        /// Percentage change to apply
        /// </summary>
        public decimal PercentageChange { get; set; }

        /// <summary>
        /// تاريخ بداية النطاق
        /// Start date of period
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ نهاية النطاق
        /// End date of period
        /// </summary>
        public DateTime EndDate { get; set; }
    }
} 
using MediatR;
using System;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Pricing
{
    /// <summary>
    /// أمر لتحديث قاعدة تسعير موجودة
    /// Command to update an existing pricing rule
    /// </summary>
    public class UpdatePricingCommand : IRequest<ResultDto<PricingRuleDto>>
    {
        /// <summary>
        /// معرف قاعدة التسعير
        /// Pricing rule identifier
        /// </summary>
        public Guid PricingRuleId { get; set; }

        /// <summary>
        /// معرف الوحدة
        /// Unit identifier
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// نوع السعر
        /// Price type
        /// </summary>
        public string PriceType { get; set; } = null!;

        /// <summary>
        /// تاريخ بداية السعر
        /// Start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ نهاية السعر
        /// End date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// مبلغ السعر
        /// Price amount
        /// </summary>
        public decimal? PriceAmount { get; set; }

        /// <summary>
        /// فئة التسعير
        /// Pricing tier
        /// </summary>
        public string PricingTier { get; set; } = null!;

        /// <summary>
        /// الزيادة أو الخصم بالنسبة المئوية
        /// Percentage change
        /// </summary>
        public decimal? PercentageChange { get; set; }

        /// <summary>
        /// السعر الأدنى
        /// Minimum price
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        /// السعر الأقصى
        /// Maximum price
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// الوصف
        /// Description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// عملة السعر
        /// Currency code
        /// </summary>
        public string Currency { get; set; } = null!;

        /// <summary>
        /// تجاوز التعارضات إذا وُجدت
        /// Override conflicts if any
        /// </summary>
        public bool OverrideConflicts { get; set; }
    }
} 
using MediatR;
using YemenBooking.Application.DTOs;
using System;

namespace YemenBooking.Application.Commands.Pricing
{
    /// <summary>
    /// أمر لإنشاء قاعدة تسعير جديدة
    /// Command to create a new pricing rule
    /// </summary>
    public class CreatePricingCommand : IRequest<ResultDto<PricingRuleDto>>
    {
        /// <summary>
        /// معرف الوحدة
        /// Unit identifier
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// نوع السعر
        /// Price type
        /// </summary>
        public string PriceType { get; set; }

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
        public string PricingTier { get; set; }

        /// <summary>
        /// الزيادة أو الخصم بالنسبة المئوية
        /// Percentage change
        /// </summary>
        public decimal? PercentageChange { get; set; }
        /// <summary>
        /// عملة السعر
        /// Currency code for the pricing rule
        /// </summary>
        public string Currency { get; set; }

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
        /// تجاوز التعارضات إذا وُجدت
        /// Override conflicts if any
        /// </summary>
        public bool OverrideConflicts { get; set; }
    }
} 
using MediatR;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Pricing
{
    /// <summary>
    /// أمر لإنشاء قواعد تسعير مجمعة
    /// Command to bulk create pricing rules
    /// </summary>
    public class BulkCreatePricingCommand : IRequest<ResultDto<IEnumerable<PricingRuleDto>>>
    {
        /// <summary>
        /// قائمة طلبات إنشاء قواعد التسعير
        /// List of create pricing rule requests
        /// </summary>
        public IEnumerable<CreatePricingRequestDto> Requests { get; set; } = new List<CreatePricingRequestDto>();
    }
} 
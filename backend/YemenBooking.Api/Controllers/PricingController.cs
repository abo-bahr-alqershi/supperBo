using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.DTOs;
using AutoMapper;
using System.Linq;
using System.Threading;
using MediatR;
using YemenBooking.Application.Commands.Pricing;

namespace YemenBooking.Api.Controllers
{
    /// <summary>
    /// متحكم لإدارة قواعد التسعير
    /// Controller for managing pricing rules
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PricingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPricingRuleRepository _pricingRepository;
        private readonly IMapper _mapper;

        public PricingController(IMediator mediator, IPricingRuleRepository pricingRepository, IMapper mapper)
        {
            _mediator = mediator;
            _pricingRepository = pricingRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// جلب قواعد التسعير لوحدة معينة ضمن نطاق زمني
        /// Get pricing rules for a unit within a date range
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUnitPricing([FromQuery] Guid unitId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken = default)
        {
            var rules = await _pricingRepository.GetPricingRulesByUnitAsync(unitId, startDate, endDate, cancellationToken);
            var dtos = rules.Select(r => _mapper.Map<PricingRuleDto>(r));
            return Ok(new { data = dtos });
        }

        /// <summary>
        /// البحث في قواعد التسعير
        /// Search pricing rules
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> SearchPricing([FromBody] PricingSearchRequest request)
        {
            // Placeholder implementation - return empty result for now
            return Ok(new
            {
                pricing_rules = new List<PricingRuleDto>(),
                conflicts = new List<object>(),
                total_count = 0,
                has_more = false
            });
        }

        /// <summary>
        /// إنشاء قاعدة تسعير جديدة
        /// Create a new pricing rule
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePricing([FromBody] CreatePricingRequestDto request)
        {
            var result = await _mediator.Send(new CreatePricingCommand
            {
                UnitId = request.UnitId,
                PriceType = request.PriceType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                PriceAmount = request.PriceAmount,
                PricingTier = request.PricingTier,
                PercentageChange = request.PercentageChange,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                Description = request.Description,
                Currency = request.Currency
            });
            return result.IsSuccess ? Ok(new { data = result.Data }) : BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// تحديث قاعدة تسعير موجودة
        /// Update an existing pricing rule
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePricing([FromRoute] Guid id, [FromBody] UpdatePricingRequestDto request)
        {
            var result = await _mediator.Send(new UpdatePricingCommand
            {
                PricingRuleId = id,
                UnitId = request.UnitId,
                PriceType = request.PriceType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                PriceAmount = request.PriceAmount,
                PricingTier = request.PricingTier,
                PercentageChange = request.PercentageChange,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                Description = request.Description,
                Currency = request.Currency
            });
            return result.IsSuccess ? Ok(new { data = result.Data }) : BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// حذف قاعدة تسعير
        /// Delete a pricing rule
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricing([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new DeletePricingCommand { PricingRuleId = id });
            return result.IsSuccess ? NoContent() : NotFound();
        }

        /// <summary>
        /// إنشاء قواعد تسعير مجمعة
        /// Bulk create pricing rules
        /// </summary>
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreatePricing([FromBody] BulkPricingRequestDto request)
        {
            var result = await _mediator.Send(new BulkCreatePricingCommand
            {
                Requests = request.Requests
            });
            return result.IsSuccess ? Ok(new { data = result.Data }) : BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// تحديث سريع للسعر
        /// Quick update price for a unit
        /// </summary>
        [HttpPatch("quick-update/{unitId}")]
        public async Task<IActionResult> QuickUpdatePrice([FromRoute] Guid unitId, [FromBody] QuickUpdatePricingRequestDto request)
        {
            var result = await _mediator.Send(new QuickUpdatePricingCommand
            {
                UnitId = unitId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                PriceAmount = request.PriceAmount,
                Currency = request.Currency
            });
            return result.IsSuccess ? Ok(new { data = result.Data }) : BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// تطبيق نسبة مئوية على الأسعار
        /// Apply percentage change to pricing rules
        /// </summary>
        [HttpPost("apply-percentage")]
        public async Task<IActionResult> ApplyPercentage([FromBody] ApplyPercentageRequestDto request)
        {
            var updated = new List<PricingRule>();
            foreach (var unitId in request.UnitIds)
            {
                var rules = await _pricingRepository.GetPricingRulesByUnitAsync(unitId, request.StartDate, request.EndDate, CancellationToken.None);
                foreach (var r in rules)
                {
                    r.PercentageChange = request.PercentageChange;
                    r.PriceAmount = r.PriceAmount * (1 + request.PercentageChange / 100m);
                    await _pricingRepository.UpdatePricingRuleAsync(r);
                    updated.Add(r);
                }
            }
            var dtos = updated.Select(r => _mapper.Map<PricingRuleDto>(r));
            return Ok(new { data = dtos });
        }

        /// <summary>
        /// الحصول على اقتراحات الأسعار
        /// Get pricing suggestions
        /// </summary>
        [HttpPost("suggestions")]
        public async Task<IActionResult> GetPricingSuggestions([FromBody] PricingSuggestionRequestDto request)
        {
            // Placeholder implementation
            var suggestion = new
            {
                suggested_price = 0m,
                market_average = 0m,
                seasonal_factor = 0m,
                demand_factor = 0m,
                confidence_level = 0m
            };
            return Ok(new { data = suggestion });
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.DTOs;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

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
        private readonly IPricingRuleRepository _pricingRepository;
        private readonly IMapper _mapper;
        private readonly IUnitRepository _unitRepository;

        public PricingController(IPricingRuleRepository pricingRepository, IMapper mapper, IUnitRepository unitRepository)
        {
            _pricingRepository = pricingRepository;
            _mapper = mapper;
            _unitRepository = unitRepository;
        }

        /// <summary>
        /// جلب قواعد التسعير لوحدة معينة ضمن نطاق زمني
        /// Get pricing rules for a unit within a date range
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUnitPricing([FromQuery] Guid unitId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var list = await _pricingRepository.GetPricingRulesByUnitAsync(unitId, startDate, endDate, CancellationToken.None);
            var unit = await _unitRepository.GetUnitByIdAsync(unitId, CancellationToken.None);
            var dtos = list.Select(p =>
            {
                var dto = _mapper.Map<PricingRuleDto>(p);
                dto.BasePrice = _mapper.Map<MoneyDto>(unit.BasePrice);
                return dto;
            });
            return Ok(new { data = dtos });
        }

        /// <summary>
        /// البحث في قواعد التسعير
        /// Search pricing rules
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> SearchPricing([FromBody] PricingSearchRequest request)
        {
            var data = await _pricingRepository.GetPricingRulesByUnitAsync(
                request.UnitIds?.FirstOrDefault() ?? Guid.Empty,
                request.StartDate,
                request.EndDate,
                CancellationToken.None);
            var unitId = request.UnitIds?.FirstOrDefault() ?? Guid.Empty;
            var unit = await _unitRepository.GetUnitByIdAsync(unitId, CancellationToken.None);
            var dtos = data
                .Where(p => request.UnitIds == null || request.UnitIds.Contains(p.UnitId))
                .Select(p =>
                {
                    var dto = _mapper.Map<PricingRuleDto>(p);
                    dto.BasePrice = _mapper.Map<MoneyDto>(unit.BasePrice);
                    return dto;
                })
                .ToList();
            return Ok(new
            {
                pricing_rules = dtos,
                conflicts = new object[0],
                total_count = dtos.Count,
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
            if (request.OverrideConflicts != true)
            {
                var hasConflict = await _pricingRepository.HasOverlapAsync(request.UnitId, request.StartDate, request.EndDate, CancellationToken.None);
                if (hasConflict)
                {
                    return Conflict(new { message = "يوجد تعارض في قواعد التسعير للفترة المحددة" });
                }
            }
            var entity = new PricingRule
            {
                Id = Guid.NewGuid(),
                UnitId = request.UnitId,
                PriceType = "base",
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PriceAmount = request.PriceAmount,
                PricingTier = request.PricingTier,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                Description = request.Description,
                Currency = request.Currency
            };
            var created = await _pricingRepository.CreatePricingRuleAsync(entity);
            var resultDto = _mapper.Map<PricingRuleDto>(created);
            return Ok(new { data = resultDto });
        }

        /// <summary>
        /// تحديث قاعدة تسعير موجودة
        /// Update an existing pricing rule
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePricing([FromRoute] Guid id, [FromBody] UpdatePricingRequestDto request)
        {
            var entity = await _pricingRepository.GetPricingRuleByIdAsync(id, CancellationToken.None);
            if (entity == null)
                return NotFound();

            entity.PriceType = "base";
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.StartTime = request.StartTime;
            entity.EndTime = request.EndTime;
            entity.PriceAmount = request.PriceAmount;
            entity.PricingTier = request.PricingTier;
            entity.MinPrice = request.MinPrice;
            entity.MaxPrice = request.MaxPrice;
            entity.Description = request.Description;
            entity.Currency = request.Currency;

            await _pricingRepository.UpdatePricingRuleAsync(entity);

            var resultDto = _mapper.Map<PricingRuleDto>(entity);
            return Ok(new { data = resultDto });
        }

        /// <summary>
        /// حذف قاعدة تسعير
        /// Delete a pricing rule
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePricing(Guid id)
        {
            var exists = await _pricingRepository.GetPricingRuleByIdAsync(id, CancellationToken.None);
            if (exists == null)
                return NotFound();
            await _pricingRepository.DeletePricingRuleAsync(id);
            return NoContent();
        }

        /// <summary>
        /// إنشاء قواعد تسعير مجمعة
        /// Bulk create pricing rules
        /// </summary>
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreatePricing([FromBody] BulkPricingRequestDto request)
        {
            var createdEntities = new List<PricingRule>();
            foreach (var req in request.Requests)
            {
                var entity = new PricingRule
                {
                    Id = Guid.NewGuid(),
                    UnitId = req.UnitId,
                    PriceType = req.PriceType,
                    StartDate = req.StartDate,
                    EndDate = req.EndDate,
                    StartTime = req.StartTime,
                    EndTime = req.EndTime,
                    PriceAmount = req.PriceAmount,
                    PricingTier = req.PricingTier,
                    PercentageChange = req.PercentageChange,
                    MinPrice = req.MinPrice,
                    MaxPrice = req.MaxPrice,
                    Description = req.Description,
                    Currency = req.Currency
                };
                var created = await _pricingRepository.CreatePricingRuleAsync(entity);
                createdEntities.Add(created);
            }
            var resultDtos = createdEntities.Select(e => _mapper.Map<PricingRuleDto>(e));
            return Ok(new { data = resultDtos });
        }

        /// <summary>
        /// تحديث سريع للسعر
        /// Quick update price for a unit
        /// </summary>
        [HttpPatch("quick-update/{unitId}")]
        public async Task<IActionResult> QuickUpdatePrice([FromRoute] Guid unitId, [FromBody] QuickUpdatePricingRequestDto request)
        {
            var list = (await _pricingRepository.GetPricingRulesByUnitAsync(unitId, request.StartDate, request.EndDate, CancellationToken.None)).ToList();
            foreach (var p in list)
            {
                p.PriceAmount = request.PriceAmount;
                p.Currency = request.Currency;
                await _pricingRepository.UpdatePricingRuleAsync(p);
            }
            var dtos = list.Select(p => _mapper.Map<PricingRuleDto>(p));
            return Ok(new { data = dtos });
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
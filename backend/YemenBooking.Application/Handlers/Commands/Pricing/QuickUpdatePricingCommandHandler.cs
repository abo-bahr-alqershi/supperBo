using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using YemenBooking.Application.Commands.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Pricing
{
    /// <summary>
    /// معالج أمر التحديث السريع للسعر
    /// Handler for QuickUpdatePricingCommand
    /// </summary>
    public class QuickUpdatePricingCommandHandler : IRequestHandler<QuickUpdatePricingCommand, ResultDto<IEnumerable<PricingRuleDto>>>
    {
        private readonly IPricingRuleRepository _pricingRepository;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public QuickUpdatePricingCommandHandler(
            IPricingRuleRepository pricingRepository,
            IMapper mapper,
            IAuditService auditService)
        {
            _pricingRepository = pricingRepository;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<ResultDto<IEnumerable<PricingRuleDto>>> Handle(QuickUpdatePricingCommand request, CancellationToken cancellationToken)
        {
            var rules = (await _pricingRepository.GetPricingRulesByUnitAsync(request.UnitId, request.StartDate, request.EndDate, cancellationToken)).ToList();
            foreach (var rule in rules)
            {
                rule.PriceAmount = request.PriceAmount;
                rule.Currency = request.Currency;
                await _pricingRepository.UpdatePricingRuleAsync(rule, cancellationToken);
            }

            await _auditService.LogAsync(
                "QuickUpdatePricing",
                request.UnitId.ToString(),
                $"تم تحديث السعر السريع للوحدة {request.UnitId}",
                Guid.Empty,
                cancellationToken: cancellationToken);

            var dtos = rules.Select(r => _mapper.Map<PricingRuleDto>(r));
            return ResultDto<IEnumerable<PricingRuleDto>>.Succeeded(dtos, "تم التحديث السريع للسعر بنجاح");
        }
    }
} 
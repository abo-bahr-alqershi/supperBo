using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using YemenBooking.Application.Commands.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Pricing
{
    /// <summary>
    /// معالج أمر تطبيق نسبة مئوية على قواعد التسعير
    /// Handler for ApplyPercentageChangeCommand
    /// </summary>
    public class ApplyPercentageChangeCommandHandler : IRequestHandler<ApplyPercentageChangeCommand, ResultDto<IEnumerable<PricingRuleDto>>>
    {
        private readonly IPricingRuleRepository _pricingRepository;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public ApplyPercentageChangeCommandHandler(
            IPricingRuleRepository pricingRepository,
            IMapper mapper,
            IAuditService auditService)
        {
            _pricingRepository = pricingRepository;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<ResultDto<IEnumerable<PricingRuleDto>>> Handle(ApplyPercentageChangeCommand request, CancellationToken cancellationToken)
        {
            var updatedList = new List<PricingRule>();
            foreach (var unitId in request.UnitIds)
            {
                var rules = await _pricingRepository.GetPricingRulesByUnitAsync(unitId, request.StartDate, request.EndDate, cancellationToken);
                foreach (var r in rules)
                {
                    r.PercentageChange = request.PercentageChange;
                    r.PriceAmount = r.PriceAmount * (1 + request.PercentageChange / 100m);
                    var updated = await _pricingRepository.UpdatePricingRuleAsync(r, cancellationToken);
                    updatedList.Add(updated);
                }
            }

            await _auditService.LogAsync(
                "ApplyPercentageChange",
                string.Join(",", request.UnitIds),
                $"تم تطبيق نسبة {request.PercentageChange}% على قواعد التسعير للوحدات {string.Join(",", request.UnitIds)}",
                Guid.Empty,
                cancellationToken: cancellationToken);

            var dtos = updatedList.Select(r => _mapper.Map<PricingRuleDto>(r));
            return ResultDto<IEnumerable<PricingRuleDto>>.Succeeded(dtos, "تم تطبيق التغيير بنجاح");
        }
    }
} 
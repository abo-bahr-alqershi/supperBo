using MediatR;
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
    /// معالج أمر إنشاء قواعد تسعير مجمعة
    /// Handler for BulkCreatePricingCommand
    /// </summary>
    public class BulkCreatePricingCommandHandler : IRequestHandler<BulkCreatePricingCommand, ResultDto<IEnumerable<PricingRuleDto>>>
    {
        private readonly IPricingRuleRepository _pricingRepository;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;

        public BulkCreatePricingCommandHandler(
            IPricingRuleRepository pricingRepository,
            IMapper mapper,
            IAuditService auditService)
        {
            _pricingRepository = pricingRepository;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<ResultDto<IEnumerable<PricingRuleDto>>> Handle(BulkCreatePricingCommand request, CancellationToken cancellationToken)
        {
            var createdEntities = new List<PricingRule>();
            foreach (var dto in request.Requests)
            {
                var entity = new PricingRule
                {
                    Id = Guid.NewGuid(),
                    UnitId = dto.UnitId,
                    PriceType = dto.PriceType,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    PriceAmount = dto.PriceAmount,
                    PricingTier = dto.PricingTier,
                    PercentageChange = dto.PercentageChange,
                    MinPrice = dto.MinPrice,
                    MaxPrice = dto.MaxPrice,
                    Description = dto.Description,
                    Currency = dto.Currency
                };
                var created = await _pricingRepository.CreatePricingRuleAsync(entity, cancellationToken);
                createdEntities.Add(created);
            }

            await _auditService.LogAsync(
                "BulkCreatePricingRules",
                string.Empty,
                $"تم إنشاء {createdEntities.Count} قاعدة تسعير جديدة",
                Guid.Empty,
                cancellationToken: cancellationToken);

            var dtos = createdEntities.Select(r => _mapper.Map<PricingRuleDto>(r));
            return ResultDto<IEnumerable<PricingRuleDto>>.Succeeded(dtos, "تم إنشاء قواعد التسعير المجّمعة بنجاح");
        }
    }
} 
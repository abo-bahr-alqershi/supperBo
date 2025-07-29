using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using YemenBooking.Application.Commands.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace YemenBooking.Application.Handlers.Commands.Pricing
{
    /// <summary>
    /// معالج أمر تحديث قاعدة تسعير موجودة
    /// Handler for UpdatePricingCommand
    /// </summary>
    public class UpdatePricingCommandHandler : IRequestHandler<UpdatePricingCommand, ResultDto<PricingRuleDto>>
    {
        private readonly IPricingRuleRepository _pricingRepository;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;
        private readonly IIndexingService _indexingService;
        private readonly ILogger<UpdatePricingCommandHandler> _logger;

        public UpdatePricingCommandHandler(
            IPricingRuleRepository pricingRepository,
            IMapper mapper,
            IAuditService auditService,
            IIndexingService indexingService,
            ILogger<UpdatePricingCommandHandler> logger)
        {
            _pricingRepository = pricingRepository;
            _mapper = mapper;
            _auditService = auditService;
            _indexingService = indexingService;
            _logger = logger;
        }

        public async Task<ResultDto<PricingRuleDto>> Handle(UpdatePricingCommand request, CancellationToken cancellationToken)
        {
            var entity = await _pricingRepository.GetPricingRuleByIdAsync(request.PricingRuleId, cancellationToken);
            if (entity == null)
                return ResultDto<PricingRuleDto>.Failure("قاعدة التسعير غير موجودة");

            entity.UnitId = request.UnitId;
            entity.PriceType = request.PriceType;
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.PriceAmount = request.PriceAmount ?? entity.PriceAmount;
            entity.PricingTier = request.PricingTier;
            entity.PercentageChange = request.PercentageChange;
            entity.MinPrice = request.MinPrice;
            entity.MaxPrice = request.MaxPrice;
            entity.Description = request.Description;
            entity.Currency = request.Currency;

            var updated = await _pricingRepository.UpdatePricingRuleAsync(entity, cancellationToken);

            await _auditService.LogAsync(
                "UpdatePricingRule",
                updated.Id.ToString(),
                $"تم تحديث قاعدة التسعير {updated.Id}",
                Guid.Empty,
                cancellationToken: cancellationToken);

            try
            {
                await _indexingService.UpdatePricingRuleIndexAsync(updated);
                _logger.LogInformation("تم تحديث فهرس التسعير {PricingRuleId}", updated.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "فشل في تحديث فهرس التسعير {PricingRuleId}", updated.Id);
            }

            var dto = _mapper.Map<PricingRuleDto>(updated);
            return ResultDto<PricingRuleDto>.Succeeded(dto, "تم تحديث قاعدة التسعير بنجاح");
        }
    }
} 
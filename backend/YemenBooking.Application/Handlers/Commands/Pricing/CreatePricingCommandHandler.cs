using MediatR;
using AutoMapper;
using YemenBooking.Application.Commands.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace YemenBooking.Application.Handlers.Commands.Pricing
{
    /// <summary>
    /// معالج أمر إنشاء قاعدة تسعير جديدة
    /// Create pricing rule command handler
    /// </summary>
    public class CreatePricingCommandHandler : IRequestHandler<CreatePricingCommand, ResultDto<PricingRuleDto>>
    {
        private readonly IPricingRuleRepository _pricingRepository;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;
        private readonly IIndexingService _indexingService;
        private readonly ILogger<CreatePricingCommandHandler> _logger;

        public CreatePricingCommandHandler(
            IPricingRuleRepository pricingRepository,
            IMapper mapper,
            IAuditService auditService,
            IIndexingService indexingService,
            ILogger<CreatePricingCommandHandler> logger)
        {
            _pricingRepository = pricingRepository;
            _mapper = mapper;
            _auditService = auditService;
            _indexingService = indexingService;
            _logger = logger;
        }

        public async Task<ResultDto<PricingRuleDto>> Handle(CreatePricingCommand request, CancellationToken cancellationToken)
        {
            if (!request.OverrideConflicts)
            {
                var hasConflict = await _pricingRepository.HasOverlapAsync(request.UnitId, request.StartDate, request.EndDate, cancellationToken);
                if (hasConflict)
                {
                    throw new ConflictException("PricingConflict", "يوجد تعارض في قواعد التسعير للفترة المحددة");
                }
            }

            var entity = new PricingRule
            {
                Id = Guid.NewGuid(),
                UnitId = request.UnitId,
                PriceType = request.PriceType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                PriceAmount = request.PriceAmount ?? 0m,
                PricingTier = request.PricingTier,
                PercentageChange = request.PercentageChange,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                Description = request.Description,
                Currency = request.Currency
            };

            var created = await _pricingRepository.AddAsync(entity, cancellationToken);

            await _auditService.LogAsync(
                "CreatePricingRule",
                created.Id.ToString(),
                $"تم إنشاء قاعدة تسعير للوحدة {created.UnitId}",
                Guid.Empty,
                cancellationToken: cancellationToken);

            // فهرسة قاعدة التسعير الجديدة
            try
            {
                await _indexingService.IndexPricingRuleAsync(created);
                _logger.LogInformation("تم فهرسة قاعدة التسعير {PricingRuleId} بنجاح", created.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "فشل في فهرسة قاعدة التسعير {PricingRuleId}", created.Id);
                // لا نفشل العملية إذا فشلت الفهرسة
            }

            var dto = _mapper.Map<PricingRuleDto>(created);
            return ResultDto<PricingRuleDto>.Succeeded(dto, "تم إنشاء قاعدة التسعير بنجاح");
        }
    }
} 
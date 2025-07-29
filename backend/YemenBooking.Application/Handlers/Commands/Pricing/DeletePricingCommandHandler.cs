using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace YemenBooking.Application.Handlers.Commands.Pricing
{
    /// <summary>
    /// معالج أمر حذف قاعدة تسعير
    /// Handler for DeletePricingCommand
    /// </summary>
    public class DeletePricingCommandHandler : IRequestHandler<DeletePricingCommand, ResultDto<bool>>
    {
        private readonly IPricingRuleRepository _pricingRepository;
        private readonly IAuditService _auditService;
        private readonly IIndexingService _indexingService;
        private readonly ILogger<DeletePricingCommandHandler> _logger;

        public DeletePricingCommandHandler(
            IPricingRuleRepository pricingRepository,
            IAuditService auditService,
            IIndexingService indexingService,
            ILogger<DeletePricingCommandHandler> logger)
        {
            _pricingRepository = pricingRepository;
            _auditService = auditService;
            _indexingService = indexingService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(DeletePricingCommand request, CancellationToken cancellationToken)
        {
            var existing = await _pricingRepository.GetPricingRuleByIdAsync(request.PricingRuleId, cancellationToken);
            if (existing == null)
                return ResultDto<bool>.Failure("قاعدة التسعير غير موجودة");

            var result = await _pricingRepository.DeletePricingRuleAsync(request.PricingRuleId, cancellationToken);
            if (!result)
                return ResultDto<bool>.Failure("فشل حذف قاعدة التسعير");

            await _auditService.LogAsync(
                "DeletePricingRule",
                request.PricingRuleId.ToString(),
                $"تم حذف قاعدة التسعير {request.PricingRuleId}",
                Guid.Empty,
                cancellationToken: cancellationToken);

            try
            {
                await _indexingService.RemovePricingRuleIndexAsync(request.PricingRuleId);
                _logger.LogInformation("تم إزالة فهرس التسعير {PricingRuleId}", request.PricingRuleId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "فشل في إزالة فهرس التسعير {PricingRuleId}", request.PricingRuleId);
            }

            return ResultDto<bool>.Succeeded(true, "تم حذف قاعدة التسعير بنجاح");
        }
    }
} 
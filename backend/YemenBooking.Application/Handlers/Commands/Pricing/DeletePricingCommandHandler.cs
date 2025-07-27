using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.Pricing;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

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

        public DeletePricingCommandHandler(
            IPricingRuleRepository pricingRepository,
            IAuditService auditService)
        {
            _pricingRepository = pricingRepository;
            _auditService = auditService;
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

            return ResultDto<bool>.Succeeded(true, "تم حذف قاعدة التسعير بنجاح");
        }
    }
} 
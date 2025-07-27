using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.Units;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Handlers.Queries.Units
{
    /// <summary>
    /// معالج استعلام حساب سعر الوحدة لفترة محددة
    /// Query handler for GetUnitPriceQuery
    /// </summary>
    public class GetUnitPriceQueryHandler : IRequestHandler<GetUnitPriceQuery, ResultDto<decimal>>
    {
        private readonly IPricingService _pricingService;
        private readonly ILogger<GetUnitPriceQueryHandler> _logger;

        public GetUnitPriceQueryHandler(
            IPricingService pricingService,
            ILogger<GetUnitPriceQueryHandler> logger)
        {
            _pricingService = pricingService;
            _logger = logger;
        }

        public async Task<ResultDto<decimal>> Handle(GetUnitPriceQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام حساب سعر الوحدة: {UnitId}", request.UnitId);

            var guestCount = request.GuestCount ?? 1;
            var price = await _pricingService.CalculatePriceAsync(
                request.UnitId,
                request.CheckIn,
                request.CheckOut,
                guestCount,
                cancellationToken);

            return ResultDto<decimal>.Succeeded(price);
        }
    }
} 
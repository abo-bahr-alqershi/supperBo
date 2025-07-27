using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.PropertyTypes;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.PropertyTypes
{
    /// <summary>
    /// معالج استعلام الحصول على نوع وحدة محدد
    /// Query handler for GetUnitTypeByIdQuery
    /// </summary>
    public class GetUnitTypeByIdQueryHandler : IRequestHandler<GetUnitTypeByIdQuery, ResultDto<UnitTypeDto>>
    {
        private readonly IUnitTypeRepository _repo;
        private readonly ILogger<GetUnitTypeByIdQueryHandler> _logger;

        public GetUnitTypeByIdQueryHandler(
            IUnitTypeRepository repo,
            ILogger<GetUnitTypeByIdQueryHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ResultDto<UnitTypeDto>> Handle(GetUnitTypeByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام نوع الوحدة: {UnitTypeId}", request.UnitTypeId);

            if (request.UnitTypeId == Guid.Empty)
                throw new ValidationException(nameof(request.UnitTypeId), "معرف نوع الوحدة غير صالح");

            var ut = await _repo.GetUnitTypeByIdAsync(request.UnitTypeId, cancellationToken);
            if (ut == null)
                return ResultDto<UnitTypeDto>.Failure($"نوع الوحدة بالمعرف {request.UnitTypeId} غير موجود");

            var dto = new UnitTypeDto
            {
                Id = ut.Id,
                PropertyTypeId = ut.PropertyTypeId,
                Name = ut.Name,
                Description = string.Empty,
                DefaultPricingRules = string.Empty
            };

            return ResultDto<UnitTypeDto>.Ok(dto, "تم جلب نوع الوحدة بنجاح");
        }
    }
} 
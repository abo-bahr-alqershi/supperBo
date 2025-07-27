using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.PropertyTypes;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.PropertyTypes
{
    /// <summary>
    /// معالج استعلام الحصول على نوع كيان محدد
    /// Query handler for GetPropertyTypeByIdQuery
    /// </summary>
    public class GetPropertyTypeByIdQueryHandler : IRequestHandler<GetPropertyTypeByIdQuery, ResultDto<PropertyTypeDto>>
    {
        private readonly IPropertyTypeRepository _repo;
        private readonly ILogger<GetPropertyTypeByIdQueryHandler> _logger;

        public GetPropertyTypeByIdQueryHandler(
            IPropertyTypeRepository repo,
            ILogger<GetPropertyTypeByIdQueryHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ResultDto<PropertyTypeDto>> Handle(GetPropertyTypeByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام نوع الكيان: {PropertyTypeId}", request.PropertyTypeId);

            if (request.PropertyTypeId == Guid.Empty)
                throw new ValidationException(nameof(request.PropertyTypeId), "معرف نوع الكيان غير صالح");

            var pt = await _repo.GetPropertyTypeByIdAsync(request.PropertyTypeId, cancellationToken);
            if (pt == null)
                return ResultDto<PropertyTypeDto>.Failure($"نوع الكيان بالمعرف {request.PropertyTypeId} غير موجود");

            var dto = new PropertyTypeDto
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description,
                DefaultAmenities = pt.DefaultAmenities
            };

            return ResultDto<PropertyTypeDto>.Ok(dto, "تم جلب نوع الكيان بنجاح");
        }
    }
} 
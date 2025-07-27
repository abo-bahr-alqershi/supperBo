using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.Amenities;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Amenities;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Amenities
{
    /// <summary>
    /// معالج استعلام الحصول على مرافق نوع الكيان
    /// Query handler for GetAmenitiesByPropertyTypeQuery
    /// </summary>
    public class GetAmenitiesByPropertyTypeQueryHandler : IRequestHandler<GetAmenitiesByPropertyTypeQuery, ResultDto<IEnumerable<AmenityDto>>>
    {
        private readonly IAmenityRepository _amenityRepository;
        private readonly ILogger<GetAmenitiesByPropertyTypeQueryHandler> _logger;

        public GetAmenitiesByPropertyTypeQueryHandler(
            IAmenityRepository amenityRepository,
            ILogger<GetAmenitiesByPropertyTypeQueryHandler> logger)
        {
            _amenityRepository = amenityRepository;
            _logger = logger;
        }

        public async Task<ResultDto<IEnumerable<AmenityDto>>> Handle(GetAmenitiesByPropertyTypeQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام مرافق نوع الكيان: {PropertyTypeId}", request.PropertyTypeId);

            if (request.PropertyTypeId == Guid.Empty)
                throw new ValidationException(nameof(request.PropertyTypeId), "معرف نوع الكيان غير صالح");

            var amenities = await _amenityRepository.GetAmenitiesByPropertyTypeAsync(request.PropertyTypeId, cancellationToken);

            var dtos = amenities.Select(pa => new AmenityDto
            {
                Id = pa.Amenity.Id,
                Name = pa.Amenity.Name,
                Description = pa.Amenity.Description
            }).ToList();

            return ResultDto<IEnumerable<AmenityDto>>.Ok(dtos, "تم جلب مرافق نوع الكيان بنجاح");
        }
    }
} 
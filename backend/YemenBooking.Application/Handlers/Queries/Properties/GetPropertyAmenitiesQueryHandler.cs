using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.Properties;
using YemenBooking.Application.Queries.CP.Properties;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Amenities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Properties
{
    /// <summary>
    /// معالج استعلام الحصول على مرافق الكيان
    /// Query handler to get amenities of a property
    /// </summary>
    public class GetPropertyAmenitiesQueryHandler : IRequestHandler<GetPropertyAmenitiesQuery, ResultDto<IEnumerable<AmenityDto>>>
    {
        #region Dependencies
        private readonly IPropertyAmenityRepository _propertyAmenityRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertyAmenitiesQueryHandler> _logger;
        #endregion

        #region Constructor
        public GetPropertyAmenitiesQueryHandler(
            IPropertyAmenityRepository propertyAmenityRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertyAmenitiesQueryHandler> logger)
        {
            _propertyAmenityRepository = propertyAmenityRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }
        #endregion

        #region Handler Implementation
        public async Task<ResultDto<IEnumerable<AmenityDto>>> Handle(GetPropertyAmenitiesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء جلب مرافق الكيان: {PropertyId}", request.PropertyId);

            if (request.PropertyId == Guid.Empty)
            {
                _logger.LogWarning("معرف الكيان غير صالح");
                return ResultDto<IEnumerable<AmenityDto>>.Failure("معرف الكيان غير صالح");
            }

            // 1. جلب المرافق من المستودع
            var amenities = await _propertyAmenityRepository.GetPropertyAmenitiesAsync(request.PropertyId, cancellationToken);

            // 2. تحويل إلى DTOs
            var amenityDtos = amenities
                .Select(pa => new AmenityDto
                {
                    Id = pa.PropertyTypeAmenity.Amenity.Id,
                    Name = pa.PropertyTypeAmenity.Amenity.Name,
                    Description = pa.PropertyTypeAmenity.Amenity.Description
                })
                .ToList();

            _logger.LogInformation("تم جلب {Count} مرفق", amenityDtos.Count);
            return ResultDto<IEnumerable<AmenityDto>>.Ok(amenityDtos);
        }
        #endregion
    }
} 
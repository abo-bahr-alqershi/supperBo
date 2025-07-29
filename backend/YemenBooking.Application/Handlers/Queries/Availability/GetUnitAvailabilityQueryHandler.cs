using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Availability;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.Availability
{
    public class GetUnitAvailabilityQueryHandler : IRequestHandler<GetUnitAvailabilityQuery, ResultDto<IEnumerable<UnitAvailabilityDetailDto>>>
    {
        private readonly IUnitAvailabilityRepository _availabilityRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUnitAvailabilityQueryHandler> _logger;

        public GetUnitAvailabilityQueryHandler(
            IUnitAvailabilityRepository availabilityRepository,
            IMapper mapper,
            ILogger<GetUnitAvailabilityQueryHandler> logger)
        {
            _availabilityRepository = availabilityRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDto<IEnumerable<UnitAvailabilityDetailDto>>> Handle(GetUnitAvailabilityQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var availabilities = await _availabilityRepository.FindAsync(x =>
                    x.UnitId == request.UnitId &&
                    (!request.StartDate.HasValue || x.StartDate >= request.StartDate.Value) &&
                    (!request.EndDate.HasValue || x.EndDate <= request.EndDate.Value), cancellationToken);

                var dtos = _mapper.Map<IEnumerable<UnitAvailabilityDetailDto>>(availabilities);
                return ResultDto<IEnumerable<UnitAvailabilityDetailDto>>.Succeeded(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving availability for unit {UnitId}", request.UnitId);
                return ResultDto<IEnumerable<UnitAvailabilityDetailDto>>.Failed("Error retrieving availability", "AVAILABILITY_RETRIEVAL_ERROR");
            }
        }
    }
}
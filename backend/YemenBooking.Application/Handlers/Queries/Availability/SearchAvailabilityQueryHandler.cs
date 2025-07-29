using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SearchAvailabilityQueryHandler : IRequestHandler<SearchAvailabilityQuery, ResultDto<SearchAvailabilityResponseDto>>
    {
        private readonly IUnitAvailabilityRepository _availabilityRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SearchAvailabilityQueryHandler> _logger;

        public SearchAvailabilityQueryHandler(
            IUnitAvailabilityRepository availabilityRepository,
            IMapper mapper,
            ILogger<SearchAvailabilityQueryHandler> logger)
        {
            _availabilityRepository = availabilityRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDto<SearchAvailabilityResponseDto>> Handle(SearchAvailabilityQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var availabilities = await _availabilityRepository.FindAsync(x =>
                    (request.UnitIds == null || request.UnitIds.Contains(x.UnitId)) &&
                    (!request.StartDate.HasValue || x.StartDate >= request.StartDate.Value) &&
                    (!request.EndDate.HasValue || x.EndDate <= request.EndDate.Value) &&
                    (request.Statuses == null || request.Statuses.Contains(x.Status)), cancellationToken);

                var dtos = _mapper.Map<IEnumerable<UnitAvailabilityDetailDto>>(availabilities);
                var response = new SearchAvailabilityResponseDto
                {
                    Availabilities = dtos,
                    Conflicts = new List<object>(),
                    TotalCount = dtos.Count(),
                    HasMore = false
                };

                return ResultDto<SearchAvailabilityResponseDto>.Succeeded(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching availability");
                return ResultDto<SearchAvailabilityResponseDto>.Failed("Error searching availability", "AVAILABILITY_SEARCH_ERROR");
            }
        }
    }
}
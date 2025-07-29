using MediatR;
using System;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Availability
{
    /// <summary>
    /// Query to get availability for a specific unit within an optional date range
    /// </summary>
    public class GetUnitAvailabilityQuery : IRequest<ResultDto<IEnumerable<UnitAvailabilityDetailDto>>>
    {
        public Guid UnitId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
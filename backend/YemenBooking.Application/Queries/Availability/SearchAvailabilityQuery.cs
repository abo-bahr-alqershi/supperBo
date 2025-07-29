using MediatR;
using System;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Queries.Availability
{
    /// <summary>
    /// Query to search unit availabilities with filters
    /// </summary>
    public class SearchAvailabilityQuery : IRequest<ResultDto<SearchAvailabilityResponseDto>>
    {
        public IEnumerable<Guid>? UnitIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<string>? Statuses { get; set; }
    }
}
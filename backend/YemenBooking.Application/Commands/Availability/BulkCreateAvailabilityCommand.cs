using MediatR;
using System;
using System.Collections.Generic;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Availability
{
    /// <summary>
    /// Command to bulk create unit availabilities
    /// </summary>
    public class BulkCreateAvailabilityCommand : IRequest<ResultDto<IEnumerable<UnitAvailabilityDetailDto>>>
    {
        public IList<CreateAvailabilityCommand> Requests { get; set; } = new List<CreateAvailabilityCommand>();
    }
}
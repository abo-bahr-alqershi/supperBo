using MediatR;
using System;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Availability
{
    /// <summary>
    /// Command to quick update availability status for a unit
    /// </summary>
    public class QuickUpdateAvailabilityCommand : IRequest<ResultDto<IEnumerable<UnitAvailabilityDetailDto>>>
    {
        public Guid UnitId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAvailable { get; set; }
    }
}
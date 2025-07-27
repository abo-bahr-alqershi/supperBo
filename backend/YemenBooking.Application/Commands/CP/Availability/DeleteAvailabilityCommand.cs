using MediatR;
using YemenBooking.Application.DTOs;
using System;

namespace YemenBooking.Application.Commands.Availability
{
    /// <summary>
    /// أمر لحذف إتاحة الوحدة
    /// Command to delete unit availability
    /// </summary>
    public class DeleteAvailabilityCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف الإتاحة
        /// Availability identifier
        /// </summary>
        public Guid AvailabilityId { get; set; }
    }
} 
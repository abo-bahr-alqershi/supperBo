using MediatR;
using YemenBooking.Application.DTOs;
using System;

namespace YemenBooking.Application.Commands.Availability
{
    /// <summary>
    /// أمر لتحديث إتاحة الوحدة
    /// Command to update unit availability
    /// </summary>
    public class UpdateAvailabilityCommand : IRequest<ResultDto<UnitAvailabilityDetailDto>>
    {
        /// <summary>
        /// معرف الإتاحة
        /// Availability identifier
        /// </summary>
        public Guid AvailabilityId { get; set; }

        /// <summary>
        /// معرف الوحدة
        /// Unit identifier
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// تاريخ ووقت البداية
        /// Start date and time
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ ووقت النهاية
        /// End date and time
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// حالة الإتاحة
        /// Availability status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// سبب عدم الإتاحة
        /// Unavailability reason
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// ملاحظات
        /// Notes
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// تجاوز التعارضات إذا وُجدت
        /// Override conflicts if any
        /// </summary>
        public bool OverrideConflicts { get; set; }
    }
} 
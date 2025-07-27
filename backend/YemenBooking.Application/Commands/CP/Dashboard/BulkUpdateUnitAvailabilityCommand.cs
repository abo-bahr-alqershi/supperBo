using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Dashboard;

namespace YemenBooking.Application.Commands.Dashboard
{
    /// <summary>
    /// الأمر لتحديث توفر وحدات متعددة ضمن نطاق زمني
    /// Command to bulk update unit availability within a date range
    /// </summary>
    public class BulkUpdateUnitAvailabilityCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// قائمة معرفات الوحدات
        /// List of unit identifiers
        /// </summary>
        public IEnumerable<Guid> UnitIds { get; set; }

        /// <summary>
        /// النطاق الزمني
        /// Date range for the update
        /// </summary>
        public DateRangeDto Range { get; set; }

        /// <summary>
        /// حالة التوفر الجديدة
        /// New availability status
        /// </summary>
        public bool IsAvailable { get; set; }

        public BulkUpdateUnitAvailabilityCommand(IEnumerable<Guid> unitIds, DateRangeDto range, bool isAvailable)
        {
            UnitIds = unitIds;
            Range = range;
            IsAvailable = isAvailable;
        }
    }
} 
using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.PropertyImages
{
    /// <summary>
    /// أمر لتعيين صور متعددة لمجموعة من الوحدات
    /// Command to bulk assign images to multiple units
    /// </summary>
    public class BulkAssignImageToUnitCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// قائمة التعيينات: معرف الوحدة ومعرف الصورة
        /// List of assignments: unitId and imageId pairs
        /// </summary>
        public List<UnitImageAssignment> Assignments { get; set; } = new List<UnitImageAssignment>();

        /// <summary>
        /// نموذج تعيين صورة لوحدة
        /// Model for unit image assignment entry
        /// </summary>
        public class UnitImageAssignment
        {
            /// <summary>
            /// معرف الوحدة
            /// Unit identifier
            /// </summary>
            public Guid UnitId { get; set; }

            /// <summary>
            /// معرف الصورة
            /// Image identifier
            /// </summary>
            public Guid ImageId { get; set; }
        }
    }
} 
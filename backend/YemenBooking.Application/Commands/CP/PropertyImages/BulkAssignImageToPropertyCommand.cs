using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.PropertyImages
{
    /// <summary>
    /// أمر لتعيين صور متعددة لمجموعة من الكيانات
    /// Command to bulk assign images to multiple properties
    /// </summary>
    public class BulkAssignImageToPropertyCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// قائمة التعيينات: معرف الكيان ومعرف الصورة
        /// List of assignments: propertyId and imageId pairs
        /// </summary>
        public List<PropertyImageAssignment> Assignments { get; set; } = new List<PropertyImageAssignment>();

        /// <summary>
        /// نموذج تعيين صورة لكيان
        /// Model for property image assignment entry
        /// </summary>
        public class PropertyImageAssignment
        {
            /// <summary>
            /// معرف الكيان
            /// Property identifier
            /// </summary>
            public Guid PropertyId { get; set; }

            /// <summary>
            /// معرف الصورة
            /// Image identifier
            /// </summary>
            public Guid ImageId { get; set; }
        }
    }
} 
using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.PropertyImages
{
    /// <summary>
    /// أمر لإعادة ترتيب صور الكيان
    /// Command to reorder property images display order
    /// </summary>
    public class ReorderPropertyImagesCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// قائمة عناصر إعادة الترتيب: معرف الصورة وترتيب العرض
        /// List of reorder items: imageId and displayOrder pairs
        /// </summary>
        public List<PropertyImageOrderAssignment> Assignments { get; set; } = new List<PropertyImageOrderAssignment>();

        /// <summary>
        /// نموذج عنصر ترتيب صورة
        /// Model for image order assignment
        /// </summary>
        public class PropertyImageOrderAssignment
        {
            /// <summary>
            /// معرف الصورة
            /// Image identifier
            /// </summary>
            public Guid ImageId { get; set; }

            /// <summary>
            /// ترتيب العرض
            /// Display order
            /// </summary>
            public int DisplayOrder { get; set; }
        }
    }
} 
using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Images
{
    /// <summary>
    /// أمر لتعيين صورة كرئيسية لكيان أو وحدة
    /// Command to set an image as primary for a property or unit
    /// </summary>
    public class SetPrimaryImageCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// معرف الصورة
        /// Image unique identifier
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// معرف الكيان (اختياري)
        /// Associated property ID (optional)
        /// </summary>
        public Guid? PropertyId { get; set; }

        /// <summary>
        /// معرف الوحدة (اختياري)
        /// Associated unit ID (optional)
        /// </summary>
        public Guid? UnitId { get; set; }
    }
} 
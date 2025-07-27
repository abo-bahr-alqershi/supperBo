using System;

namespace YemenBooking.Application.DTOs.Properties
{
    /// <summary>
    /// مرفق الكيان
    /// </summary>
    public class PropertyAmenityDto
    {
        /// <summary>
        /// معرف المرفق
        /// </summary>
        public Guid AmenityId { get; set; }

        /// <summary>
        /// هل متاح
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// التكلفة الإضافية
        /// </summary>
        public decimal? ExtraCost { get; set; }

        /// <summary>
        /// الوصف
        /// </summary>
        public string? Description { get; set; }
    }
} 
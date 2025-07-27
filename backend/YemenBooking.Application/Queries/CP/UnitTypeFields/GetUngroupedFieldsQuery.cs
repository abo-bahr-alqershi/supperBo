using MediatR;
using YemenBooking.Application.DTOs;
using System;

namespace YemenBooking.Application.Queries.UnitTypeFields
{
    /// <summary>
    /// استعلام لجلب الحقول غير المجمعة ضمن أي مجموعة لنوع الكيان
    /// Query to get ungrouped unit type fields for a property type
    /// </summary>
    public class GetUngroupedFieldsQuery : IRequest<PaginatedResult<UnitTypeFieldDto>>
    {
        /// <summary>
        /// معرف نوع الكيان
        /// Property type identifier
        /// </summary>
        public Guid PropertyTypeId { get; set; }

        /// <summary>
        /// قابل للبحث فقط (اختياري)
        /// Only searchable fields (optional)
        /// </summary>
        public bool? IsSearchable { get; set; }

        /// <summary>
        /// عام فقط (اختياري)
        /// Only public fields (optional)
        /// </summary>
        public bool? IsPublic { get; set; }

        /// <summary>
        /// مخصص للوحدات فقط (اختياري)
        /// Only fields for units (optional)
        /// </summary>
        public bool? IsForUnits { get; set; }

        /// <summary>
        /// فئة الحقل (اختياري)
        /// Field category (optional)
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// رقم الصفحة (اختياري)
        /// Page number (optional)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// حجم الصفحة (اختياري)
        /// Page size (optional)
        /// </summary>
        public int PageSize { get; set; } = 20;
    }
} 
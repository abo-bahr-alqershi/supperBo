using System;
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs
{
    /// <summary>
    /// تفاصيل نوع الوحدة
    /// Unit type DTO
    /// </summary>
    public class UnitTypeDto
    {
        /// <summary>
        /// معرف نوع الوحدة
        /// Unit type identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// معرف نوع الكيان
        /// Property type identifier
        /// </summary>
        public Guid PropertyTypeId { get; set; }

        /// <summary>
        /// اسم نوع الوحدة
        /// Unit type name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// وصف نوع الوحدة
        /// Unit type description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// قواعد التسعير الافتراضية (JSON)
        /// Default pricing rules (JSON)
        /// </summary>
        public string DefaultPricingRules { get; set; }

        /// <summary>
        /// المجموعات التي تحوي الحقول الديناميكية لنوع الوحدة
        /// Groups containing dynamic fields for the unit type
        /// </summary>
        public List<FieldGroupDto> FieldGroups { get; set; } = new List<FieldGroupDto>();

        /// <summary>
        /// فلاتر البحث الديناميكية المطبقة على الحقول
        /// Dynamic search filters for the unit type fields
        /// </summary>
        public List<SearchFilterDto> Filters { get; set; } = new List<SearchFilterDto>();

        /// <summary>
        /// الحد الأقصى للسعة
        /// Maximum capacity
        /// </summary>
        public int MaxCapacity { get; set; }
    }
} 
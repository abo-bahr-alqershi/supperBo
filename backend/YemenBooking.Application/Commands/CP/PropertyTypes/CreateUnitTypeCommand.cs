using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.PropertyTypes
{
    /// <summary>
    /// أمر لإنشاء نوع وحدة جديد
    /// Command to create a new unit type
    /// </summary>
    public class CreateUnitTypeCommand : IRequest<ResultDto<Guid>>
    {
        /// <summary>
        /// معرف نوع الكيان
        /// </summary>
        public Guid PropertyTypeId { get; set; }

        /// <summary>
        /// اسم نوع الوحدة
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// الحد الأقصى للسعة
        /// </summary>
        public int MaxCapacity { get; set; }
    }
} 
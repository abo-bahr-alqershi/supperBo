using MediatR;
using System;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;

namespace YemenBooking.Application.Queries.PropertyTypes
{
    /// <summary>
    /// استعلام للحصول على نوع كيان محدد
    /// Query to get a specific property type by ID
    /// </summary>
    public class GetPropertyTypeByIdQuery : IRequest<ResultDto<PropertyTypeDto>>
    {
        /// <summary>
        /// معرف نوع الكيان
        /// </summary>
        public Guid PropertyTypeId { get; set; }
    }
} 
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Properties;
using System;

namespace YemenBooking.Application.Queries.CP.Properties;

/// <summary>
/// استعلام للحصول على بيانات الكيان بواسطة المعرف
/// Query to get property details by ID
/// </summary>
public class GetPropertyByIdQuery : IRequest<ResultDto<PropertyDetailsDto>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }
} 
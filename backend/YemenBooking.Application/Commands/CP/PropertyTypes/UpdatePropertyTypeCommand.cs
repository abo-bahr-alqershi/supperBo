using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.PropertyTypes;

/// <summary>
/// أمر لتحديث نوع الكيان
/// Command to update a property type
/// </summary>
public class UpdatePropertyTypeCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف نوع الكيان
    /// Property type identifier
    /// </summary>
    public Guid PropertyTypeId { get; set; }

    /// <summary>
    /// الاسم الجديد لنوع الكيان
    /// New property type name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// الوصف الجديد لنوع الكيان
    /// New property type description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// المرافق الافتراضية الجديدة (JSON)
    /// New default amenities (JSON)
    /// </summary>
    public string DefaultAmenities { get; set; } = string.Empty;
} 
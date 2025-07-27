using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.PropertyImages;

/// <summary>
/// أمر لتحديث بيانات صورة في المعرض
/// Command to update property image data
/// </summary>
public class UpdatePropertyImageCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الصورة
    /// Image identifier
    /// </summary>
    public Guid ImageId { get; set; }

    /// <summary>
    /// رابط الملف أو مساره (اختياري للتحديث)
    /// File URL or path (optional)
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// تعليق توضيحي جديد (اختياري)
    /// New image caption (optional)
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// نص بديل جديد (اختياري)
    /// New alt text (optional)
    /// </summary>
    public string? AltText { get; set; }

    /// <summary>
    /// فئة الصورة (اختياري)
    /// Image category (optional)
    /// </summary>
    public ImageCategory? Category { get; set; }

    /// <summary>
    /// تعيين كصورة رئيسية أو لا (اختياري)
    /// Set as main image (optional)
    /// </summary>
    public bool? IsMain { get; set; }
} 
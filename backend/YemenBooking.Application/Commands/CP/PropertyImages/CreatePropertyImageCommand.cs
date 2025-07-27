using System;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.PropertyImages;

/// <summary>
/// أمر لإضافة صورة جديدة لمعرض الكيان أو الوحدة
/// Command to add a new property or unit image
/// </summary>
public class CreatePropertyImageCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// معرف الكيان (اختياري)
    /// Property identifier (optional)
    /// </summary>
    public Guid? PropertyId { get; set; }

    /// <summary>
    /// معرف الوحدة (اختياري)
    /// Unit identifier (optional)
    /// </summary>
    public Guid? UnitId { get; set; }

    /// <summary>
    /// رابط الملف أو مساره
    /// File URL or path
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// تعليق توضيحي للصورة
    /// Image caption
    /// </summary>
    public string Caption { get; set; } = string.Empty;

    /// <summary>
    /// نص بديل للصورة
    /// Alt text for the image
    /// </summary>
    public string AltText { get; set; } = string.Empty;

    /// <summary>
    /// فئة الصورة
    /// Image category
    /// </summary>
    public ImageCategory Category { get; set; }

    /// <summary>
    /// هل هذه الصورة الرئيسية
    /// Is this the main image
    /// </summary>
    public bool IsMain { get; set; }
}

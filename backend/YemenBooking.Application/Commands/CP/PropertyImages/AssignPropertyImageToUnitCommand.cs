using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.PropertyImages;

/// <summary>
/// أمر لتعيين صورة إلى وحدة
/// Command to assign an image to a unit
/// </summary>
public class AssignPropertyImageToUnitCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الصورة
    /// Image identifier
    /// </summary>
    public Guid ImageId { get; set; }

    /// <summary>
    /// معرف الوحدة
    /// Unit identifier
    /// </summary>
    public Guid UnitId { get; set; }

        /// <summary>
    /// تعيين كصورة رئيسية
    /// Property identifier
    /// </summary>
    public bool SetAsMain { get; set; } = false;

} 
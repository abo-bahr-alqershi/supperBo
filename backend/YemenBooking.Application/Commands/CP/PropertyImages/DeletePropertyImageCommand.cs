using System;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.PropertyImages;

/// <summary>
/// أمر لحذف صورة من المعرض
/// Command to delete a property image
/// </summary>
public class DeletePropertyImageCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الصورة المراد حذفها
    /// Identifier of the image to delete
    /// </summary>
    public Guid ImageId { get; set; }
} 
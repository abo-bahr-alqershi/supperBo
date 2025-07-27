using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Images
{
    /// <summary>
    /// أمر لحذف صور متعددة (مؤقت أو دائم)
    /// Command to bulk delete images (soft or permanent)
    /// </summary>
    public class DeleteImagesCommand : IRequest<ResultDto<bool>>
    {
        /// <summary>
        /// قائمة معرفات الصور المراد حذفها
        /// List of image IDs to delete
        /// </summary>
        public List<Guid> ImageIds { get; set; } = new List<Guid>();

        /// <summary>
        /// حذف دائم بدل الحذف المؤقت لجميع الصور
        /// Permanent deletion flag for all images
        /// </summary>
        public bool Permanent { get; set; } = false;
    }
} 
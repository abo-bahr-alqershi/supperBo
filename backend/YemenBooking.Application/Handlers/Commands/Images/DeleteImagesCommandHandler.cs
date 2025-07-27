using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.Images;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Images
{
    /// <summary>
    /// معالج أمر حذف صور متعددة (مؤقت أو دائم)
    /// Handler for DeleteImagesCommand to bulk delete images (soft or permanent)
    /// </summary>
    public class DeleteImagesCommandHandler : IRequestHandler<DeleteImagesCommand, ResultDto<bool>>
    {
        private readonly IPropertyImageRepository _imageRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteImagesCommandHandler(
            IPropertyImageRepository imageRepository,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _imageRepository = imageRepository;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultDto<bool>> Handle(DeleteImagesCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var imageId in request.ImageIds)
                {
                    var image = await _imageRepository.GetPropertyImageByIdAsync(imageId, cancellationToken);
                    if (image == null) continue;

                    var deleted = await _imageRepository.DeletePropertyImageAsync(imageId, cancellationToken);
                    if (deleted && request.Permanent && !string.IsNullOrEmpty(image.Url))
                    {
                        await _fileStorageService.DeleteFileAsync(image.Url, cancellationToken);
                    }
                }
            }, cancellationToken);

            return ResultDto<bool>.Ok(true, "تم حذف الصور المطلوبة بنجاح");
        }
    }
} 
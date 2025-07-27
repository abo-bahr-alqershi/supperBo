using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.PropertyImages;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.PropertyImages
{
    /// <summary>
    /// معالج أمر إعادة ترتيب الصور
    /// Handler for ReorderPropertyImagesCommand
    /// </summary>
    public class ReorderPropertyImagesCommandHandler : IRequestHandler<ReorderPropertyImagesCommand, ResultDto<bool>>
    {
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<ReorderPropertyImagesCommandHandler> _logger;

        public ReorderPropertyImagesCommandHandler(
            IPropertyImageRepository propertyImageRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<ReorderPropertyImagesCommandHandler> logger)
        {
            _propertyImageRepository = propertyImageRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(ReorderPropertyImagesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة إعادة ترتيب الصور لعدد {Count} عناصر", request.Assignments?.Count);

            if (request.Assignments == null || !request.Assignments.Any())
                return ResultDto<bool>.Failure("لا توجد تعيينات لإعادة الترتيب");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var item in request.Assignments.Distinct())
                {
                    var image = await _propertyImageRepository.GetPropertyImageByIdAsync(item.ImageId, cancellationToken);
                    if (image == null)
                        continue;

                    var oldOrder = image.DisplayOrder;
                    image.DisplayOrder = item.DisplayOrder;
                    image.UpdatedAt = DateTime.UtcNow;
                    image.UpdatedBy = _currentUserService.UserId;

                    await _propertyImageRepository.UpdatePropertyImageAsync(image, cancellationToken);

                    await _auditService.LogBusinessOperationAsync(
                        "ReorderPropertyImages",
                        $"تحديث ترتيب العرض للصورة {image.Id} من {oldOrder} إلى {image.DisplayOrder}",
                        _currentUserService.UserId,
                        "PropertyImage",
                        image.Id,
                        null,
                        cancellationToken);
                }
            });

            _logger.LogInformation("تمت معالجة إعادة الترتيب بنجاح");
            return ResultDto<bool>.Succeeded(true);
        }
    }
} 
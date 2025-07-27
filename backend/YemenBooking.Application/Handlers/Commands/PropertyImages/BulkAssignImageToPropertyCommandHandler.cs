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
    /// معالج أمر تعيين صور متعددة لمجموعة من الكيانات
    /// Handler for BulkAssignImageToPropertyCommand
    /// </summary>
    public class BulkAssignImageToPropertyCommandHandler : IRequestHandler<BulkAssignImageToPropertyCommand, ResultDto<bool>>
    {
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<BulkAssignImageToPropertyCommandHandler> _logger;

        public BulkAssignImageToPropertyCommandHandler(
            IPropertyImageRepository propertyImageRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<BulkAssignImageToPropertyCommandHandler> logger)
        {
            _propertyImageRepository = propertyImageRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(BulkAssignImageToPropertyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة bulk assign للصور لعدد {Count} تعيينات", request.Assignments?.Count);

            if (request.Assignments == null || !request.Assignments.Any())
                return ResultDto<bool>.Failure("لا توجد تعيينات لتطبيقها");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var item in request.Assignments.Distinct())
                {
                    var image = await _propertyImageRepository.GetPropertyImageByIdAsync(item.ImageId, cancellationToken);
                    if (image == null)
                        continue;

                    var oldPropertyId = image.PropertyId;
                    image.PropertyId = item.PropertyId;
                    image.UnitId = null;
                    image.UpdatedAt = DateTime.UtcNow;
                    image.UpdatedBy = _currentUserService.UserId;

                    await _propertyImageRepository.UpdatePropertyImageAsync(image, cancellationToken);

                    await _auditService.LogBusinessOperationAsync(
                        "BulkAssignImageToProperty",
                        $"تعيين صورة {image.Id} للكيان {item.PropertyId}",
                        _currentUserService.UserId,
                        "PropertyImage",
                        image.Id,
                        null,
                        cancellationToken);
                }
            });

            _logger.LogInformation("تم إكمال bulk assign للصور بنجاح");
            return ResultDto<bool>.Succeeded(true);
        }
    }
} 
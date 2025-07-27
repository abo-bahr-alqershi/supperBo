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
    /// معالج أمر تعيين صور متعددة لمجموعة من الوحدات
    /// Handler for BulkAssignImageToUnitCommand
    /// </summary>
    public class BulkAssignImageToUnitCommandHandler : IRequestHandler<BulkAssignImageToUnitCommand, ResultDto<bool>>
    {
        private readonly IPropertyImageRepository _propertyImageRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<BulkAssignImageToUnitCommandHandler> _logger;

        public BulkAssignImageToUnitCommandHandler(
            IPropertyImageRepository propertyImageRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<BulkAssignImageToUnitCommandHandler> logger)
        {
            _propertyImageRepository = propertyImageRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(BulkAssignImageToUnitCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة bulk assign للصور للوحدات لعدد {Count} تعيينات", request.Assignments?.Count);

            if (request.Assignments == null || !request.Assignments.Any())
                return ResultDto<bool>.Failure("لا توجد تعيينات لتطبيقها");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var item in request.Assignments.Distinct())
                {
                    var image = await _propertyImageRepository.GetPropertyImageByIdAsync(item.ImageId, cancellationToken);
                    if (image == null)
                        continue;

                    var oldUnitId = image.UnitId;
                    image.UnitId = item.UnitId;
                    image.PropertyId = null;
                    image.UpdatedAt = DateTime.UtcNow;
                    image.UpdatedBy = _currentUserService.UserId;

                    await _propertyImageRepository.UpdatePropertyImageAsync(image, cancellationToken);

                    await _auditService.LogBusinessOperationAsync(
                        "BulkAssignImageToUnit",
                        $"تعيين صورة {image.Id} للوحدة {item.UnitId}",
                        _currentUserService.UserId,
                        "PropertyImage",
                        image.Id,
                        null,
                        cancellationToken);
                }
            });

            _logger.LogInformation("تم إكمال bulk assign للصور للوحدات بنجاح");
            return ResultDto<bool>.Succeeded(true);
        }
    }
} 
using MediatR;
using YemenBooking.Application.Commands.Availability;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace YemenBooking.Application.Handlers.Commands.Availability
{
    /// <summary>
    /// معالج أمر حذف إتاحة الوحدة
    /// Delete unit availability command handler
    /// </summary>
    public class DeleteAvailabilityCommandHandler : IRequestHandler<DeleteAvailabilityCommand, ResultDto<bool>>
    {
        private readonly IUnitAvailabilityRepository _availabilityRepository;
        private readonly IAuditService _auditService;
        private readonly IIndexingService _indexingService;

        public DeleteAvailabilityCommandHandler(
            IUnitAvailabilityRepository availabilityRepository,
            IAuditService auditService,
            IIndexingService indexingService)
        {
            _availabilityRepository = availabilityRepository;
            _auditService = auditService;
            _indexingService = indexingService;
        }

        public async Task<ResultDto<bool>> Handle(DeleteAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var exists = await _availabilityRepository.ExistsAsync(request.AvailabilityId, cancellationToken);
            if (!exists)
                return ResultDto<bool>.Failed("الإتاحة غير موجودة");

            await _availabilityRepository.DeleteAsync(request.AvailabilityId, cancellationToken);

            await _auditService.LogAsync(
                "DeleteAvailability",
                request.AvailabilityId.ToString(),
                "تم حذف الإتاحة بنجاح",
                Guid.Empty,
                cancellationToken: cancellationToken);

            try
            {
                await _indexingService.RemoveAvailabilityIndexAsync(request.AvailabilityId);
                // _logger.LogInformation("تم إزالة فهرس الإتاحة {AvailabilityId}", request.AvailabilityId); // This line was not in the edit_specification, so it's removed.
            }
            catch (Exception ex)
            {
                // _logger.LogWarning(ex, "فشل في إزالة فهرس الإتاحة {AvailabilityId}", request.AvailabilityId); // This line was not in the edit_specification, so it's removed.
            }

            return ResultDto<bool>.Succeeded(true, "تم حذف الإتاحة بنجاح");
        }
    }
} 
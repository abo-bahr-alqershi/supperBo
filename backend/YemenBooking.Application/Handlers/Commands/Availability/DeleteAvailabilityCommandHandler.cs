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

        public DeleteAvailabilityCommandHandler(
            IUnitAvailabilityRepository availabilityRepository,
            IAuditService auditService)
        {
            _availabilityRepository = availabilityRepository;
            _auditService = auditService;
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

            return ResultDto<bool>.Succeeded(true, "تم حذف الإتاحة بنجاح");
        }
    }
} 
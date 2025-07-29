using MediatR;
using AutoMapper;
using YemenBooking.Application.Commands.Availability;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace YemenBooking.Application.Handlers.Commands.Availability
{
    /// <summary>
    /// معالج أمر تحديث إتاحة الوحدة
    /// Update unit availability command handler
    /// </summary>
    public class UpdateAvailabilityCommandHandler : IRequestHandler<UpdateAvailabilityCommand, ResultDto<UnitAvailabilityDetailDto>>
    {
        private readonly IUnitAvailabilityRepository _availabilityRepository;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;
        private readonly IAvailabilityService _availabilityService;
        private readonly IIndexingService _indexingService;

        public UpdateAvailabilityCommandHandler(
            IUnitAvailabilityRepository availabilityRepository,
            IMapper mapper,
            IAuditService auditService,
            IAvailabilityService availabilityService,
            IIndexingService indexingService)
        {
            _availabilityRepository = availabilityRepository;
            _mapper = mapper;
            _auditService = auditService;
            _availabilityService = availabilityService;
            _indexingService = indexingService;
        }

        public async Task<ResultDto<UnitAvailabilityDetailDto>> Handle(UpdateAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var entity = await _availabilityRepository.GetByIdAsync(request.AvailabilityId, cancellationToken);
            if (entity == null)
                return ResultDto<UnitAvailabilityDetailDto>.Failed("الإتاحة غير موجودة");

            // التحقق من التعارضات: التأكد من توافر الوحدة للفترة المحدثة
            var isAvailable = await _availabilityService.CheckAvailabilityAsync(
                entity.UnitId,
                request.StartDate,
                request.EndDate,
                cancellationToken);
            if (!isAvailable && !request.OverrideConflicts)
            {
                return ResultDto<UnitAvailabilityDetailDto>.Failure("لا يمكن تحديث الإتاحة؛ الوحدة غير متاحة للفترة المحددة");
            }

            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.Status = request.Status;
            entity.Reason = request.Reason;
            entity.Notes = request.Notes;

            await _availabilityRepository.UpdateAsync(entity, cancellationToken);

            await _auditService.LogAsync(
                "UpdateAvailability",
                entity.Id.ToString(),
                $"تم تحديث إتاحة للوحدة {entity.UnitId}",
                Guid.Empty,
                cancellationToken: cancellationToken);

            try
            {
                await _indexingService.UpdateAvailabilityIndexAsync(entity);
                // _logger.LogInformation("تم تحديث فهرس الإتاحة {AvailabilityId}", entity.Id); // Original code had this line commented out
            }
            catch (Exception ex)
            {
                // _logger.LogWarning(ex, "فشل في تحديث فهرس الإتاحة {AvailabilityId}", entity.Id); // Original code had this line commented out
            }

            var dto = _mapper.Map<UnitAvailabilityDetailDto>(entity);
            return ResultDto<UnitAvailabilityDetailDto>.Succeeded(dto, "تم تحديث الإتاحة بنجاح");
        }
    }
} 
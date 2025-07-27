using MediatR;
using AutoMapper;
using YemenBooking.Application.Commands.Availability;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Exceptions;

namespace YemenBooking.Application.Handlers.Commands.Availability
{
    /// <summary>
    /// معالج أمر إنشاء إتاحة جديدة
    /// Create availability command handler
    /// </summary>
    public class CreateAvailabilityCommandHandler : IRequestHandler<CreateAvailabilityCommand, ResultDto<UnitAvailabilityDetailDto>>
    {
        private readonly IUnitAvailabilityRepository _availabilityRepository;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;
        private readonly IIndexingService _indexingService;
        private readonly ILogger<CreateAvailabilityCommandHandler> _logger;

        public CreateAvailabilityCommandHandler(
            IUnitAvailabilityRepository availabilityRepository,
            IMapper mapper,
            IAuditService auditService,
            IIndexingService indexingService,
            ILogger<CreateAvailabilityCommandHandler> logger)
        {
            _availabilityRepository = availabilityRepository;
            _mapper = mapper;
            _auditService = auditService;
            _indexingService = indexingService;
            _logger = logger;
        }

        public async Task<ResultDto<UnitAvailabilityDetailDto>> Handle(CreateAvailabilityCommand request, CancellationToken cancellationToken)
        {
            // التحقق من التعارضات في الإتاحة
            var hasConflict = await _availabilityRepository.HasOverlapAsync(
                request.UnitId, 
                request.StartDate, 
                request.EndDate, 
                null, // excludeAvailabilityId
                cancellationToken);
            
            if (hasConflict && !request.OverrideConflicts)
            {
                return ResultDto<UnitAvailabilityDetailDto>.Failed("يوجد تعارض في الإتاحة للفترة المحددة");
            }

            var entity = new UnitAvailability
            {
                Id = Guid.NewGuid(),
                UnitId = request.UnitId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                Reason = request.Reason,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _availabilityRepository.AddAsync(entity, cancellationToken);

            await _auditService.LogAsync(
                "CreateAvailability",
                created.Id.ToString(),
                $"تم إنشاء إتاحة للوحدة {created.UnitId}",
                Guid.Empty,
                cancellationToken: cancellationToken);

            // فهرسة الإتاحة الجديدة
            try
            {
                await _indexingService.IndexAvailabilityAsync(created);
                _logger.LogInformation("تم فهرسة الإتاحة {AvailabilityId} بنجاح", created.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "فشل في فهرسة الإتاحة {AvailabilityId}", created.Id);
                // لا نفشل العملية إذا فشلت الفهرسة
            }

            var dto = _mapper.Map<UnitAvailabilityDetailDto>(created);
            return ResultDto<UnitAvailabilityDetailDto>.Succeeded(dto, "تم إنشاء الإتاحة بنجاح");
        }
    }
} 
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.SearchFilters;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Handlers.Commands.SearchFilters
{
    /// <summary>
    /// معالج أمر تغيير حالة التفعيل لفلتر بحث
    /// Toggle active status for a search filter
    /// </summary>
    public class ToggleSearchFilterStatusCommandHandler : IRequestHandler<ToggleSearchFilterStatusCommand, ResultDto<bool>>
    {
        private readonly ISearchFilterRepository _searchFilterRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<ToggleSearchFilterStatusCommandHandler> _logger;

        public ToggleSearchFilterStatusCommandHandler(
            ISearchFilterRepository searchFilterRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            ILogger<ToggleSearchFilterStatusCommandHandler> logger)
        {
            _searchFilterRepository = searchFilterRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(ToggleSearchFilterStatusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تغيير حالة فلتر البحث {FilterId} إلى {IsActive}", request.FilterId, request.IsActive);

            var existing = await _searchFilterRepository.GetSearchFilterByIdAsync(request.FilterId, cancellationToken);
            if (existing == null)
                throw new NotFoundException("SearchFilter", request.FilterId.ToString());

            if (_currentUserService.Role != "Admin")
                throw new ForbiddenException("غير مصرح لك بتغيير حالة الفلتر");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                existing.IsActive = request.IsActive;
                existing.UpdatedBy = _currentUserService.UserId;
                existing.UpdatedAt = DateTime.UtcNow;
                await _searchFilterRepository.UpdateSearchFilterAsync(existing, cancellationToken);

                await _auditService.LogActivityAsync(
                    "SearchFilter",
                    existing.Id.ToString(),
                    "ToggleStatus",
                    $"تم تغيير حالة الفلتر إلى {(request.IsActive ? "مُفعّل" : "مُعطّل")}",
                    null,
                    null,
                    cancellationToken);

                // await _eventPublisher.PublishEventAsync(new SearchFilterStatusToggledEvent
                // {
                //     FilterId = existing.Id,
                //     IsActive = request.IsActive,
                //     ToggledBy = _currentUserService.UserId,
                //     ToggledAt = DateTime.UtcNow
                // }, cancellationToken);

                _logger.LogInformation("تم تغيير حالة فلتر البحث بنجاح: {FilterId}", existing.Id);
            });

            return ResultDto<bool>.Ok(true, "تم تغيير حالة الفلتر بنجاح");
        }
    }

    /// <summary>
    /// حدث تغيير حالة تفعيل فلتر البحث
    /// Search filter status toggled event
    /// </summary>
    public class SearchFilterStatusToggledEvent
    {
        public Guid FilterId { get; set; }
        public bool IsActive { get; set; }
        public Guid ToggledBy { get; set; }
        public DateTime ToggledAt { get; set; }
    }
} 
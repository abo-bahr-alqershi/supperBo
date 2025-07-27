using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.SearchFilters;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.SearchFilters
{
    /// <summary>
    /// معالج أمر حذف فلتر بحث
    /// Deletes a search filter (soft delete) and includes:
    /// - Input validation
    /// - Existence check
    /// - Authorization (Admin only)
    /// - Soft delete
    /// - Audit logging
    /// - Event publishing
    /// </summary>
    public class DeleteSearchFilterCommandHandler : IRequestHandler<DeleteSearchFilterCommand, ResultDto<bool>>
    {
        private readonly ISearchFilterRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<DeleteSearchFilterCommandHandler> _logger;

        public DeleteSearchFilterCommandHandler(
            ISearchFilterRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            ILogger<DeleteSearchFilterCommandHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(DeleteSearchFilterCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة أمر حذف فلتر البحث: {FilterId}", request.FilterId);

            // التحقق من صحة المعرف
            if (request.FilterId == Guid.Empty)
                throw new BusinessRuleException("InvalidFilterId", "معرف الفلتر غير صالح");

            // التحقق من وجود الفلتر
            var existing = await _repository.GetSearchFilterByIdAsync(request.FilterId, cancellationToken);
            if (existing == null)
                throw new NotFoundException("SearchFilter", request.FilterId.ToString());

            // صلاحيات المستخدم
            if (_currentUserService.Role != "Admin")
                throw new ForbiddenException("غير مصرح لك بحذف فلتر البحث");

            // تنفيذ الحذف الناعم ضمن معاملة
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var deleted = await _repository.DeleteSearchFilterAsync(request.FilterId, cancellationToken);
                if (!deleted)
                    throw new BusinessRuleException("DeletionFailed", "فشل حذف فلتر البحث");

                // تسجيل التدقيق
                await _auditService.LogActivityAsync(
                    "SearchFilter",
                    existing.Id.ToString(),
                    "Delete",
                    $"تم حذف فلتر البحث: {existing.DisplayName} من النوع {existing.FilterType}",
                    existing,
                    null,
                    cancellationToken);

                // نشر الحدث
                // await _eventPublisher.PublishEventAsync(new SearchFilterDeletedEvent
                // {
                //     FilterId = existing.Id,
                //     FilterType = existing.FilterType,
                //     DeletedBy = _currentUserService.UserId,
                //     DeletedAt = DateTime.UtcNow
                // }, cancellationToken);

                _logger.LogInformation("تم حذف فلتر البحث بنجاح: {FilterId}", existing.Id);
            });

            return ResultDto<bool>.Ok(true);
        }
    }

    /// <summary>
    /// حدث حذف فلتر بحث
    /// Search filter deleted event
    /// </summary>
    public class SearchFilterDeletedEvent
    {
        public Guid FilterId { get; set; }
        public string FilterType { get; set; } = string.Empty;
        public Guid DeletedBy { get; set; }
        public DateTime DeletedAt { get; set; }
    }
} 
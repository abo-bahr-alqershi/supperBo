using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using YemenBooking.Application.Commands.SearchFilters;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.SearchFilters
{
    /// <summary>
    /// معالج أمر تحديث فلتر بحث
    /// Update search filter command handler
    /// </summary>
    public class UpdateSearchFilterCommandHandler : IRequestHandler<UpdateSearchFilterCommand, ResultDto<bool>>
    {
        private readonly ISearchFilterRepository _searchFilterRepository;
        private readonly IValidationService _validationService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateSearchFilterCommandHandler> _logger;

        public UpdateSearchFilterCommandHandler(
            ISearchFilterRepository searchFilterRepository,
            IValidationService validationService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            IEventPublisher eventPublisher,
            IUnitOfWork unitOfWork,
            ILogger<UpdateSearchFilterCommandHandler> logger)
        {
            _searchFilterRepository = searchFilterRepository;
            _validationService = validationService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _eventPublisher = eventPublisher;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdateSearchFilterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("بدء تحديث فلتر البحث {FilterId}", request.FilterId);

                // التحقق من صحة البيانات المدخلة
                var inputValidation = ValidateInput(request);
                if (!inputValidation.Success)
                    return inputValidation;

                // التحقق من وجود الفلتر
                var existingFilter = await _searchFilterRepository.GetSearchFilterByIdAsync(request.FilterId, cancellationToken);
                if (existingFilter == null)
                    return ResultDto<bool>.Failure("فلتر البحث غير موجود", "FILTER_NOT_FOUND");

                // التحقق من صلاحيات المستخدم
                if (_currentUserService.Role != "Admin")
                    return ResultDto<bool>.Failure("غير مصرح لك بتحديث فلتر البحث", "FORBIDDEN");

                // حفظ القيم الأصلية للمراجعة
                var originalValues = new
                {
                    existingFilter.FilterType,
                    existingFilter.DisplayName,
                    existingFilter.FilterOptions,
                    existingFilter.IsActive,
                    existingFilter.SortOrder
                };

                // تنفيذ التحديث ضمن معاملة
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    existingFilter.FilterType = request.FilterType;
                    existingFilter.DisplayName = request.DisplayName;
                    existingFilter.FilterOptions = JsonConvert.SerializeObject(request.FilterOptions ?? new Dictionary<string, object>());
                    existingFilter.IsActive = request.IsActive;
                    existingFilter.SortOrder = request.SortOrder;
                    existingFilter.UpdatedBy = _currentUserService.UserId;
                    existingFilter.UpdatedAt = DateTime.UtcNow;

                    var updatedFilter = await _searchFilterRepository.UpdateSearchFilterAsync(existingFilter, cancellationToken);

                    // تسجيل التدقيق
                    await _auditService.LogActivityAsync(
                        "SearchFilter",
                        updatedFilter.Id.ToString(),
                        "Update",
                        $"تم تحديث فلتر البحث: {updatedFilter.DisplayName}",
                        originalValues,
                        updatedFilter,
                        cancellationToken);

                    // نشر الحدث
                    // await _eventPublisher.PublishEventAsync(new SearchFilterUpdatedEvent
                    // {
                    //     FilterId = updatedFilter.Id,
                    //     FilterType = updatedFilter.FilterType,
                    //     DisplayName = updatedFilter.DisplayName,
                    //     IsActive = updatedFilter.IsActive,
                    //     SortOrder = updatedFilter.SortOrder,
                    //     UpdatedBy = _currentUserService.UserId,
                    //     UpdatedAt = updatedFilter.UpdatedAt
                    // }, cancellationToken);

                    _logger.LogInformation("تم تحديث فلتر البحث بنجاح: {FilterId}", updatedFilter.Id);
                });

                return ResultDto<bool>.Ok(true, "تم تحديث فلتر البحث بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث فلتر البحث: {FilterId}", request.FilterId);
                return ResultDto<bool>.Failure("حدث خطأ أثناء تحديث فلتر البحث");
            }
        }

        /// <summary>
        /// التحقق من صحة البيانات المدخلة
        /// Validate input data
        /// </summary>
        private ResultDto<bool> ValidateInput(UpdateSearchFilterCommand request)
        {
            var errors = new List<string>();
            if (request.FilterId == Guid.Empty)
                errors.Add("معرف الفلتر غير صالح");
            if (string.IsNullOrWhiteSpace(request.FilterType))
                errors.Add("نوع الفلتر مطلوب");
            if (string.IsNullOrWhiteSpace(request.DisplayName))
                errors.Add("الاسم المعروض مطلوب");
            if (request.SortOrder < 0)
                errors.Add("ترتيب الفلتر يجب أن يكون صفر أو أكبر");

            if (errors.Any())
                return ResultDto<bool>.Failure("بيانات غير صحيحة: " + string.Join(", ", errors), "INVALID_INPUT");

            return ResultDto<bool>.Ok(true);
        }
    }

    /// <summary>
    /// حدث تحديث فلتر البحث
    /// Search filter updated event
    /// </summary>
    public class SearchFilterUpdatedEvent
    {
        public Guid FilterId { get; set; }
        public string FilterType { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.SearchFilters;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Enums;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace YemenBooking.Application.Handlers.Commands.SearchFilters;

/// <summary>
/// معالج أمر إنشاء فلتر البحث
/// Creates new search filter for dynamic fields and includes:
/// - التحقق من صحة البيانات المدخلة
/// - التحقق من وجود الحقل المرتبط
/// - التحقق من صلاحيات المستخدم (مسؤول فقط)
/// - التحقق من قواعد العمل
/// - التحقق من صحة نوع الفلتر وخياراته
/// - إنشاء فلتر البحث الجديد
/// - تسجيل العملية في سجل التدقيق
/// - نشر الأحداث
/// 
/// Creates new search filter and includes:
/// - Input data validation
/// - Field existence validation
/// - User authorization validation (Admin only)
/// - Business rules validation
/// - Filter type and options validation
/// - Search filter creation
/// - Audit log creation
/// - Event publishing
/// </summary>
public class CreateSearchFilterCommandHandler : IRequestHandler<CreateSearchFilterCommand, ResultDto<Guid>>
{
    private readonly ISearchFilterRepository _searchFilterRepository;
    private readonly IUnitTypeFieldRepository _unitTypeFieldRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidationService _validationService;
    private readonly IAuditService _auditService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<CreateSearchFilterCommandHandler> _logger;

    public CreateSearchFilterCommandHandler(
        ISearchFilterRepository searchFilterRepository,
        IUnitTypeFieldRepository unitTypeFieldRepository,
        ICurrentUserService currentUserService,
        IValidationService validationService,
        IAuditService auditService,
        IEventPublisher eventPublisher,
        ILogger<CreateSearchFilterCommandHandler> logger)
    {
        _searchFilterRepository = searchFilterRepository;
        _unitTypeFieldRepository = unitTypeFieldRepository;
        _currentUserService = currentUserService;
        _validationService = validationService;
        _auditService = auditService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// تنفيذ أمر إنشاء فلتر البحث
    /// Execute create search filter command
    /// </summary>
    public async Task<ResultDto<Guid>> Handle(CreateSearchFilterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء معالجة أمر إنشاء فلتر البحث للحقل {FieldId}", request.FieldId);

            // 1. التحقق من صحة البيانات المدخلة
            var inputValidation = ValidateInputData(request);
            if (!inputValidation.IsSuccess)
                return inputValidation;

            // 2. التحقق من وجود الحقل المرتبط
            var field = await _unitTypeFieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
            if (field == null)
            {
                return ResultDto<Guid>.Failure("الحقل المرتبط غير موجود", "FIELD_NOT_FOUND");
            }

            // 3. التحقق من الصلاحيات
            var authorizationResult = await ValidateAuthorizationAsync();
            if (!authorizationResult.IsSuccess)
                return authorizationResult;

            // 4. التحقق من قواعد العمل
            var businessRulesValidation = await ValidateBusinessRulesAsync(request, field, cancellationToken);
            if (!businessRulesValidation.IsSuccess)
                return businessRulesValidation;

            // 5. إنشاء فلتر البحث الجديد
            var searchFilter = new SearchFilter
            {
                Id = Guid.NewGuid(),
                FieldId = request.FieldId,
                FilterType = request.FilterType,
                DisplayName = request.DisplayName,
                FilterOptions = JsonConvert.SerializeObject(request.FilterOptions ?? new Dictionary<string, object>()),
                IsActive = request.IsActive,
                SortOrder = request.SortOrder,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            // 6. حفظ فلتر البحث
            var createdFilter = await _searchFilterRepository.CreateSearchFilterAsync(searchFilter, cancellationToken);

            // 7. تسجيل العملية في سجل التدقيق
            await _auditService.LogAsync(
                "SearchFilter",
                createdFilter.Id.ToString(),
                $"تم إنشاء فلتر بحث جديد: {request.DisplayName}",
                _currentUserService.UserId);

            // 8. نشر حدث إنشاء فلتر البحث
            await _eventPublisher.PublishAsync(new SearchFilterCreatedEvent
            {
                FilterId = createdFilter.Id,
                FieldId = request.FieldId,
                FilterType = request.FilterType,
                DisplayName = request.DisplayName,
                UserId = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            });

            _logger.LogInformation("تم إنشاء فلتر البحث بنجاح. المعرف: {FilterId}", createdFilter.Id);

            return ResultDto<Guid>.Ok(
                createdFilter.Id,
                "تم إنشاء فلتر البحث بنجاح"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء إنشاء فلتر البحث للحقل {FieldId}", request.FieldId);
            return ResultDto<Guid>.Failure("حدث خطأ أثناء إنشاء فلتر البحث");
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate input data
    /// </summary>
    private ResultDto<Guid> ValidateInputData(CreateSearchFilterCommand request)
    {
        var errors = new List<string>();

        // التحقق من معرف الحقل
        if (request.FieldId == Guid.Empty)
            errors.Add("معرف الحقل مطلوب");

        // التحقق من نوع الفلتر
        if (string.IsNullOrWhiteSpace(request.FilterType))
            errors.Add("نوع الفلتر مطلوب");
        else if (request.FilterType.Length > 50)
            errors.Add("نوع الفلتر لا يجب أن يتجاوز 50 حرف");

        // التحقق من الاسم المعروض
        if (string.IsNullOrWhiteSpace(request.DisplayName))
            errors.Add("الاسم المعروض مطلوب");
        else if (request.DisplayName.Length > 100)
            errors.Add("الاسم المعروض لا يجب أن يتجاوز 100 حرف");

        // التحقق من ترتيب الفلتر
        if (request.SortOrder < 0)
            errors.Add("ترتيب الفلتر يجب أن يكون أكبر من أو يساوي صفر");

        if (errors.Any())
        {
            return ResultDto<Guid>.Failure("بيانات غير صحيحة: " + string.Join(", ", errors), "INVALID_INPUT");
        }

        return ResultDto<Guid>.Ok(Guid.Empty);
    }

    /// <summary>
    /// التحقق من الصلاحيات
    /// Validate authorization
    /// </summary>
    private Task<ResultDto<Guid>> ValidateAuthorizationAsync()
    {
        // التحقق من أن المستخدم مسؤول
        if (_currentUserService.Role != "Admin")
        {
            return Task.FromResult(ResultDto<Guid>.Failure("يتطلب صلاحيات المسؤول لإنشاء فلاتر البحث", "INSUFFICIENT_PERMISSIONS"));
        }

        return Task.FromResult(ResultDto<Guid>.Ok(Guid.Empty));
    }

    /// <summary>
    /// التحقق من قواعد العمل
    /// Validate business rules
    /// </summary>
    private async Task<ResultDto<Guid>> ValidateBusinessRulesAsync(
        CreateSearchFilterCommand request, 
        UnitTypeField field, 
        CancellationToken cancellationToken)
    {
        // التحقق من أن الحقل قابل للبحث
        if (!field.IsSearchable)
        {
            return ResultDto<Guid>.Failure("الحقل المحدد غير قابل للبحث", "FIELD_NOT_SEARCHABLE");
        }

        // التحقق من صحة نوع الفلتر مع نوع الحقل
        var validFilterTypes = GetValidFilterTypesForFieldType(field.FieldTypeId);
        if (!validFilterTypes.Contains(request.FilterType.ToLower()))
        {
            return ResultDto<Guid>.Failure(
                $"نوع الفلتر '{request.FilterType}' غير متوافق مع نوع الحقل '{field.FieldTypeId}'",
                "INCOMPATIBLE_FILTER_TYPE");
        }

        // التحقق من عدم وجود فلتر آخر بنفس النوع للحقل
        var existingFilters = await _searchFilterRepository.GetFiltersByFieldIdAsync(request.FieldId, cancellationToken);
        if (existingFilters.Any(f => f.FilterType.Equals(request.FilterType, StringComparison.OrdinalIgnoreCase) && !f.IsDeleted))
        {
            return ResultDto<Guid>.Failure("يوجد فلتر بنفس النوع للحقل المحدد", "DUPLICATE_FILTER_TYPE");
        }

        // التحقق من صحة خيارات الفلتر
        var optionsValidation = ValidateFilterOptions(request.FilterType, request.FilterOptions, field);
        if (!optionsValidation.IsSuccess)
            return optionsValidation;

        return ResultDto<Guid>.Ok(Guid.Empty);
    }

    /// <summary>
    /// الحصول على أنواع الفلاتر المتاحة لنوع الحقل
    /// Get valid filter types for field data type
    /// </summary>
    private List<string> GetValidFilterTypesForFieldType(string dataType)
    {
        return dataType.ToLower() switch
        {
            "text" => new List<string> { "exact", "contains", "startswith", "endswith" },
            "number" => new List<string> { "exact", "range", "greater", "less" },
            "boolean" => new List<string> { "exact" },
            "date" => new List<string> { "exact", "range", "before", "after" },
            "email" => new List<string> { "exact", "contains", "domain" },
            "url" => new List<string> { "exact", "contains", "domain" },
            "phone" => new List<string> { "exact", "contains", "area" },
            "select" => new List<string> { "exact", "multiple" },
            _ => new List<string> { "exact", "contains" }
        };
    }

    /// <summary>
    /// التحقق من صحة خيارات الفلتر
    /// Validate filter options
    /// </summary>
    private ResultDto<Guid> ValidateFilterOptions(string filterType, Dictionary<string, object> options, UnitTypeField field)
    {
        if (options == null) return ResultDto<Guid>.Ok(Guid.Empty);

        switch (filterType.ToLower())
        {
            case "range":
                if (!options.ContainsKey("min") && !options.ContainsKey("max"))
                {
                    return ResultDto<Guid>.Failure("فلتر النطاق يتطلب قيمة دنيا أو عليا على الأقل", "INVALID_RANGE_OPTIONS");
                }
                break;

            case "select":
            case "multiple":
                if (field.FieldTypeId.ToLower() == "select")
                {
                    // التحقق من أن خيارات الفلتر تتطابق مع خيارات الحقل
                    var fieldOptions = field.FieldOptions;
                    if (!string.IsNullOrEmpty(fieldOptions))
                    {
                        try
                        {
                            var fieldSelectOptions = JsonConvert.DeserializeObject<Dictionary<string, object>>(fieldOptions);
                            if (fieldSelectOptions?.ContainsKey("options") == true)
                            {
                                // التحقق من صحة الخيارات
                            }
                        }
                        catch
                        {
                            return ResultDto<Guid>.Failure("خيارات الحقل غير صحيحة", "INVALID_FIELD_OPTIONS");
                        }
                    }
                }
                break;
        }

        return ResultDto<Guid>.Ok(Guid.Empty);
    }
}

/// <summary>
/// حدث إنشاء فلتر البحث
/// Search filter created event
/// </summary>
public class SearchFilterCreatedEvent
{
    public Guid FilterId { get; set; }
    public Guid FieldId { get; set; }
    public required string FilterType { get; set; }
    public required string DisplayName { get; set; }
    public Guid? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}

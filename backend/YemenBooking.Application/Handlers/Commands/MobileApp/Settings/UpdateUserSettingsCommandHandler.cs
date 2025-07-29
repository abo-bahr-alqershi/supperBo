using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Settings;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Entities;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Settings;

/// <summary>
/// معالج أمر تحديث إعدادات المستخدم
/// Handler for update user settings command
/// </summary>
public class UpdateUserSettingsCommandHandler : IRequestHandler<UpdateUserSettingsCommand, ResultDto<UpdateUserSettingsResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly ILogger<UpdateUserSettingsCommandHandler> _logger;

    /// <summary>
    /// منشئ معالج أمر تحديث إعدادات المستخدم
    /// Constructor for update user settings command handler
    /// </summary>
    /// <param name="userRepository">مستودع المستخدمين</param>
    /// <param name="userSettingsRepository">مستودع إعدادات المستخدمين</param>
    /// <param name="logger">مسجل الأحداث</param>
    public UpdateUserSettingsCommandHandler(
        IUserRepository userRepository,
        IUserSettingsRepository userSettingsRepository,
        ILogger<UpdateUserSettingsCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userSettingsRepository = userSettingsRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة أمر تحديث إعدادات المستخدم
    /// Handle update user settings command
    /// </summary>
    /// <param name="request">طلب تحديث الإعدادات</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>نتيجة العملية</returns>
    public async Task<ResultDto<UpdateUserSettingsResponse>> Handle(UpdateUserSettingsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء عملية تحديث إعدادات المستخدم: {UserId}", request.UserId);

            // التحقق من صحة البيانات المدخلة
            var validationResult = ValidateRequest(request);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            // التحقق من وجود المستخدم
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("لم يتم العثور على المستخدم: {UserId}", request.UserId);
                return ResultDto<UpdateUserSettingsResponse>.Failed("المستخدم غير موجود", "USER_NOT_FOUND");
            }

            // البحث عن إعدادات المستخدم الحالية أو إنشاؤها
            var existingSettings = await _userSettingsRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            
            if (existingSettings != null)
            {
                // تحديث الإعدادات الموجودة
                await UpdateExistingSettings(existingSettings, request, cancellationToken);
            }
            else
            {
                // إنشاء إعدادات جديدة
                await CreateNewSettings(request, cancellationToken);
            }

            _logger.LogInformation("تم تحديث إعدادات المستخدم بنجاح: {UserId}", request.UserId);

            var response = new UpdateUserSettingsResponse
            {
                Success = true,
                Message = "تم تحديث الإعدادات بنجاح"
            };

            return ResultDto<UpdateUserSettingsResponse>.Ok(response, "تم تحديث الإعدادات بنجاح");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء تحديث إعدادات المستخدم: {UserId}", request.UserId);
            return ResultDto<UpdateUserSettingsResponse>.Failed($"حدث خطأ أثناء تحديث الإعدادات: {ex.Message}", "UPDATE_SETTINGS_ERROR");
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate the input request
    /// </summary>
    /// <param name="request">طلب تحديث الإعدادات</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<UpdateUserSettingsResponse> ValidateRequest(UpdateUserSettingsCommand request)
    {
        if (request.UserId == Guid.Empty)
        {
            return ResultDto<UpdateUserSettingsResponse>.Failed("معرف المستخدم غير صالح", "INVALID_USER_ID");
        }

        // التحقق من اللغة المفضلة
        var supportedLanguages = new[] { "ar", "en", "arabic", "english" };
        if (!supportedLanguages.Contains(request.PreferredLanguage.ToLower()))
        {
            return ResultDto<UpdateUserSettingsResponse>.Failed("اللغة المفضلة غير مدعومة", "UNSUPPORTED_LANGUAGE");
        }

        // التحقق من العملة المفضلة
        var supportedCurrencies = new[] { "YER", "USD", "SAR", "AED", "EUR", "GBP" };
        if (!supportedCurrencies.Contains(request.PreferredCurrency.ToUpper()))
        {
            return ResultDto<UpdateUserSettingsResponse>.Failed("العملة المفضلة غير مدعومة", "UNSUPPORTED_CURRENCY");
        }

        // التحقق من المنطقة الزمنية
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return ResultDto<UpdateUserSettingsResponse>.Failed("المنطقة الزمنية غير صالحة", "INVALID_TIMEZONE");
        }

        // التحقق من صحة JSON الإضافي إذا تم توفيره
        if (!string.IsNullOrWhiteSpace(request.AdditionalSettingsJson))
        {
            try
            {
                JsonDocument.Parse(request.AdditionalSettingsJson);
            }
            catch (JsonException)
            {
                return ResultDto<UpdateUserSettingsResponse>.Failed("تنسيق الإعدادات الإضافية غير صحيح", "INVALID_ADDITIONAL_SETTINGS_JSON");
            }
        }

        return ResultDto<UpdateUserSettingsResponse>.Ok(null, "البيانات صحيحة");
    }

    /// <summary>
    /// تحديث الإعدادات الموجودة
    /// Update existing settings
    /// </summary>
    /// <param name="existingSettings">الإعدادات الموجودة</param>
    /// <param name="request">طلب التحديث</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    private async Task UpdateExistingSettings(dynamic existingSettings, UpdateUserSettingsCommand request, CancellationToken cancellationToken)
    {
        // تحديث الإعدادات
        existingSettings.PreferredLanguage = NormalizeLanguage(request.PreferredLanguage);
        existingSettings.PreferredCurrency = request.PreferredCurrency.ToUpper();
        existingSettings.TimeZone = request.TimeZone;
        existingSettings.DarkMode = request.DarkMode;
        
        // تحويل JSON إلى Dictionary إذا تم توفيره
        if (!string.IsNullOrWhiteSpace(request.AdditionalSettingsJson))
        {
            try
            {
                existingSettings.AdditionalSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(request.AdditionalSettingsJson);
            }
            catch (JsonException)
            {
                existingSettings.AdditionalSettings = new Dictionary<string, object>();
            }
        }
        
        existingSettings.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _userSettingsRepository.UpdateAsync(existingSettings, cancellationToken);
        if (!updateResult)
        {
            _logger.LogError("فشل في تحديث إعدادات المستخدم: {UserId}", request.UserId);
            throw new InvalidOperationException("فشل في تحديث الإعدادات");
        }

        _logger.LogInformation("تم تحديث الإعدادات الموجودة للمستخدم: {UserId}", request.UserId);
    }

    /// <summary>
    /// إنشاء إعدادات جديدة
    /// Create new settings
    /// </summary>
    /// <param name="request">طلب التحديث</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    private async Task CreateNewSettings(UpdateUserSettingsCommand request, CancellationToken cancellationToken)
    {
        var newSettings = new UserSettings
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            PreferredLanguage = NormalizeLanguage(request.PreferredLanguage),
            PreferredCurrency = request.PreferredCurrency.ToUpper(),
            TimeZone = request.TimeZone,
            DarkMode = request.DarkMode,
            AdditionalSettings = !string.IsNullOrWhiteSpace(request.AdditionalSettingsJson) ?
                JsonSerializer.Deserialize<Dictionary<string, object>>(request.AdditionalSettingsJson) ?? new Dictionary<string, object>() :
                new Dictionary<string, object>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createResult = await _userSettingsRepository.CreateAsync(newSettings, cancellationToken);
        if (createResult == null)
        {
            _logger.LogError("فشل في إنشاء إعدادات جديدة للمستخدم: {UserId}", request.UserId);
            throw new InvalidOperationException("فشل في إنشاء الإعدادات");
        }

        _logger.LogInformation("تم إنشاء إعدادات جديدة للمستخدم: {UserId}", request.UserId);
    }

    /// <summary>
    /// تطبيع اللغة إلى تنسيق موحد
    /// Normalize language to standard format
    /// </summary>
    /// <param name="language">اللغة المدخلة</param>
    /// <returns>اللغة المطبعة</returns>
    private string NormalizeLanguage(string language)
    {
        return language.ToLower() switch
        {
            "ar" or "arabic" or "عربي" => "ar",
            "en" or "english" or "إنجليزي" => "en",
            _ => "ar" // افتراضي
        };
    }
}

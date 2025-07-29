using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Queries.MobileApp.Favorites;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Users;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Application.Handlers.Queries.MobileApp.Favorites;

/// <summary>
/// معالج استعلام الحصول على قائمة المفضلات للمستخدم
/// Handler for get user favorites query
/// </summary>
public class GetUserFavoritesQueryHandler : IRequestHandler<GetUserFavoritesQuery, ResultDto<UserFavoritesResponse>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly ILogger<GetUserFavoritesQueryHandler> _logger;

    /// <summary>
    /// منشئ معالج استعلام المفضلات
    /// Constructor for get user favorites query handler
    /// </summary>
    /// <param name="favoriteRepository">مستودع المفضلات</param>
    /// <param name="propertyRepository">مستودع العقارات</param>
    /// <param name="userRepository">مستودع المستخدمين</param>
    /// <param name="unitRepository">مستودع الوحدات</param>
    /// <param name="logger">مسجل الأحداث</param>
    public GetUserFavoritesQueryHandler(
        IFavoriteRepository favoriteRepository,
        IPropertyRepository propertyRepository,
        IUserRepository userRepository,
        IUnitRepository unitRepository,
        ILogger<GetUserFavoritesQueryHandler> logger)
    {
        _favoriteRepository = favoriteRepository;
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
        _unitRepository = unitRepository;
        _logger = logger;
    }

    /// <summary>
    /// معالجة استعلام الحصول على قائمة المفضلات للمستخدم
    /// Handle get user favorites query
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة المفضلات</returns>
    public async Task<ResultDto<UserFavoritesResponse>> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("بدء استعلام مفضلات المستخدم. معرف المستخدم: {UserId}, الصفحة: {PageNumber}", 
                request.UserId, request.PageNumber);

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
                return ResultDto<UserFavoritesResponse>.Failed("المستخدم غير موجود", "USER_NOT_FOUND");
            }

            // الحصول على المفضلات مع التصفح
            var (favorites, totalCount) = await _favoriteRepository.GetUserFavoritesAsync(
                request.UserId, 
                request.PageNumber, 
                request.PageSize, 
                cancellationToken);

            if (favorites == null || !favorites.Any())
            {
                _logger.LogInformation("لم يتم العثور على مفضلات للمستخدم: {UserId}", request.UserId);
                
                return ResultDto<UserFavoritesResponse>.Ok(
                    new UserFavoritesResponse
                    {
                        Favorites = new List<FavoritePropertyDto>(),
                        TotalCount = 0
                    }, 
                    "لا توجد مفضلات متاحة"
                );
            }

            // تحويل البيانات إلى DTO
            var favoriteDtos = new List<FavoritePropertyDto>();
            
            foreach (var favorite in favorites)
            {
                // الحصول على تفاصيل العقار
                var property = await _propertyRepository.GetByIdAsync(favorite.PropertyId, cancellationToken);
                if (property == null)
                {
                    _logger.LogWarning("لم يتم العثور على العقار: {PropertyId}", favorite.PropertyId);
                    continue; // تخطي هذا العقار إذا لم يعد موجوداً
                }

                // استخدام البيانات المتاحة من الكيان مباشرة
                var averageRating = property.AverageRating;
                
                // حساب أقل سعر من الوحدات المرتبطة بالعقار
                var units = await _unitRepository.GetActiveByPropertyIdAsync(property.Id, cancellationToken);
                var minPrice = units?.Any() == true ? units.Min(u => u.BasePrice.Amount) : 0;

                var favoriteDto = new FavoritePropertyDto
                {
                    PropertyId = property.Id,
                    Name = property.Name ?? string.Empty,
                    Location = property.City ?? string.Empty,
                    StarRating = property.StarRating,
                    AverageRating = averageRating,
                    MinPrice = minPrice,
                    Currency = "YER", // العملة الافتراضية
                    MainImageUrl = property.Images?.FirstOrDefault(i => i.IsMain)?.Url ?? string.Empty,
                    AddedToFavoritesAt = favorite.CreatedAt
                };

                favoriteDtos.Add(favoriteDto);
            }

            // ترتيب المفضلات حسب تاريخ الإضافة (الأحدث أولاً)
            favoriteDtos = favoriteDtos.OrderByDescending(f => f.AddedToFavoritesAt).ToList();

            var response = new UserFavoritesResponse
            {
                Favorites = favoriteDtos,
                TotalCount = totalCount
            };

            _logger.LogInformation("تم الحصول على {Count} مفضلة للمستخدم {UserId} بنجاح", favoriteDtos.Count, request.UserId);

            return ResultDto<UserFavoritesResponse>.Ok(
                response, 
                $"تم الحصول على {favoriteDtos.Count} مفضلة من أصل {totalCount}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ أثناء الحصول على مفضلات المستخدم. معرف المستخدم: {UserId}", request.UserId);
            return ResultDto<UserFavoritesResponse>.Failed(
                $"حدث خطأ أثناء الحصول على المفضلات: {ex.Message}", 
                "GET_USER_FAVORITES_ERROR"
            );
        }
    }

    /// <summary>
    /// التحقق من صحة البيانات المدخلة
    /// Validate request data
    /// </summary>
    /// <param name="request">طلب الاستعلام</param>
    /// <returns>نتيجة التحقق</returns>
    private ResultDto<UserFavoritesResponse> ValidateRequest(GetUserFavoritesQuery request)
    {
        if (request.UserId == Guid.Empty)
        {
            _logger.LogWarning("معرف المستخدم مطلوب");
            return ResultDto<UserFavoritesResponse>.Failed("معرف المستخدم مطلوب", "USER_ID_REQUIRED");
        }

        if (request.PageNumber < 1)
        {
            _logger.LogWarning("رقم الصفحة يجب أن يكون أكبر من صفر");
            return ResultDto<UserFavoritesResponse>.Failed("رقم الصفحة يجب أن يكون أكبر من صفر", "INVALID_PAGE_NUMBER");
        }

        if (request.PageSize < 1 || request.PageSize > 100)
        {
            _logger.LogWarning("حجم الصفحة يجب أن يكون بين 1 و 100");
            return ResultDto<UserFavoritesResponse>.Failed("حجم الصفحة يجب أن يكون بين 1 و 100", "INVALID_PAGE_SIZE");
        }

        return ResultDto<UserFavoritesResponse>.Ok(null, "البيانات صحيحة");
    }
}

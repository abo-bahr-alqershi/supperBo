using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Users;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Users
{
    /// <summary>
    /// معالج أمر تحديث قائمة المفضلة للمستخدم
    /// </summary>
    public class UpdateUserFavoritesCommandHandler : IRequestHandler<UpdateUserFavoritesCommand, ResultDto<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdateUserFavoritesCommandHandler> _logger;

        public UpdateUserFavoritesCommandHandler(
            IUserRepository userRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdateUserFavoritesCommandHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdateUserFavoritesCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            _logger.LogInformation("بدء تحديث قائمة المفضلة للمستخدم: UserId={UserId}", userId);

            // التحقق من المدخلات
            if (string.IsNullOrWhiteSpace(request.FavoritesJson))
                return ResultDto<bool>.Failed("قائمة المفضلة مطلوبة");

            // تحديث القائمة في المستودع
            var updated = await _userRepository.UpdateUserFavoritesAsync(userId, request.FavoritesJson, cancellationToken);
            if (!updated)
                return ResultDto<bool>.Failed("فشل في تحديث قائمة المفضلة");

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "UpdateUserFavorites",
                $"تم تحديث قائمة المفضلة للمستخدم {userId}",
                userId,
                "User",
                userId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتملت عملية تحديث قائمة المفضلة للمستخدم: UserId={UserId}", userId);
            return ResultDto<bool>.Succeeded(true, "تم تحديث قائمة المفضلة بنجاح");
        }
    }
} 
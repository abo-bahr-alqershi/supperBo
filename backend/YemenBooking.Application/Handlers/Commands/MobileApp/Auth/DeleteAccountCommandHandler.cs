using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.MobileApp.Auth;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.DTOs.Auth;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace YemenBooking.Application.Handlers.Commands.MobileApp.Auth
{
    /// <summary>
    /// معالج أمر حذف حساب المستخدم
    /// Handler for delete account command
    /// </summary>
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, ResultDto<DeleteAccountResponse>>
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<DeleteAccountCommandHandler> _logger;

        /// <summary>
        /// منشئ معالج أمر حذف حساب المستخدم
        /// Constructor for delete account command handler
        /// </summary>
        /// <param name="authService">خدمة المصادقة</param>
        /// <param name="userRepository">مستودع المستخدمين</param>
        /// <param name="bookingRepository">مستودع الحجوزات</param>
        /// <param name="logger">مسجل الأحداث</param>
        public DeleteAccountCommandHandler(
            IAuthenticationService authService,
            IUserRepository userRepository,
            IBookingRepository bookingRepository,
            ILogger<DeleteAccountCommandHandler> logger)
        {
            _authService = authService;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _logger = logger;
        }

        /// <summary>
        /// معالجة أمر حذف حساب المستخدم
        /// Handle delete account command
        /// </summary>
        /// <param name="request">طلب حذف الحساب</param>
        /// <param name="cancellationToken">رمز الإلغاء</param>
        /// <returns>نتيجة العملية</returns>
        public async Task<ResultDto<DeleteAccountResponse>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("بدء عملية حذف حساب المستخدم: {UserId}", request.UserId);

                // التحقق من صحة البيانات المدخلة
                if (request.UserId == Guid.Empty)
                {
                    _logger.LogWarning("محاولة حذف حساب بمعرف مستخدم غير صالح");
                    return ResultDto<DeleteAccountResponse>.Failed("معرف المستخدم غير صالح", "INVALID_USER_ID");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return ResultDto<DeleteAccountResponse>.Failed("كلمة المرور مطلوبة للتأكيد", "PASSWORD_REQUIRED");
                }

                // البحث عن المستخدم
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("لم يتم العثور على المستخدم: {UserId}", request.UserId);
                    return ResultDto<DeleteAccountResponse>.Failed("المستخدم غير موجود", "USER_NOT_FOUND");
                }

                // التحقق من كلمة المرور
                var isPasswordValid = await _authService.VerifyPasswordAsync(user.Email, request.Password, cancellationToken);
                if (!isPasswordValid)
                {
                    _logger.LogWarning("كلمة المرور غير صحيحة لحذف الحساب: {UserId}", request.UserId);
                    return ResultDto<DeleteAccountResponse>.Failed("كلمة المرور غير صحيحة", "INVALID_PASSWORD");
                }

                // التحقق من وجود حجوزات نشطة
                var activeBookings = await _bookingRepository.GetActiveBookingsByUserIdAsync(request.UserId, cancellationToken);
                if (activeBookings.Any())
                {
                    _logger.LogWarning("محاولة حذف حساب مع وجود حجوزات نشطة: {UserId}", request.UserId);
                    return ResultDto<DeleteAccountResponse>.Failed("لا يمكن حذف الحساب لوجود حجوزات نشطة. يرجى إلغاء جميع الحجوزات أولاً", "ACTIVE_BOOKINGS_EXIST");
                }

                // تسجيل سبب الحذف إذا تم توفيره
                if (!string.IsNullOrWhiteSpace(request.Reason))
                {
                    _logger.LogInformation("سبب حذف الحساب {UserId}: {Reason}", request.UserId, request.Reason);
                }

                // حذف الحساب (الحذف الناعم)
                var deleteResult = await _userRepository.SoftDeleteAsync(request.UserId, cancellationToken);
                if (!deleteResult)
                {
                    _logger.LogError("فشل في حذف الحساب: {UserId}", request.UserId);
                    return ResultDto<DeleteAccountResponse>.Failed("فشل في حذف الحساب", "DELETE_FAILED");
                }

                // إلغاء جميع الجلسات النشطة للمستخدم
                try
                {
                    await _authService.RevokeAllUserTokensAsync(request.UserId, cancellationToken);
                    _logger.LogInformation("تم إلغاء جميع الجلسات النشطة للمستخدم: {UserId}", request.UserId);
                }
                catch (Exception tokenEx)
                {
                    _logger.LogWarning(tokenEx, "فشل في إلغاء الجلسات النشطة للمستخدم: {UserId}", request.UserId);
                    // لا نفشل العملية بسبب فشل إلغاء الجلسات
                }

                _logger.LogInformation("تم حذف حساب المستخدم بنجاح: {UserId}", request.UserId);

                var response = new DeleteAccountResponse
                {
                    Success = true,
                    Message = "تم حذف الحساب بنجاح. نأسف لرؤيتك تغادر"
                };

                return ResultDto<DeleteAccountResponse>.Ok(response, "تم حذف الحساب بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء حذف حساب المستخدم: {UserId}", request.UserId);
                return ResultDto<DeleteAccountResponse>.Failed($"حدث خطأ أثناء حذف الحساب: {ex.Message}", "DELETE_ACCOUNT_ERROR");
            }
        }
    }
}
namespace YemenBooking.Application.Commands.Chat
{
    using MediatR;
    using YemenBooking.Application.DTOs;

    /// <summary>
    /// أمر لتحديث إعدادات الشات الخاصة بالمستخدم
    /// Command to update user chat settings
    /// </summary>
    public class UpdateChatSettingsCommand : IRequest<ResultDto<ChatSettingsDto>>
    {
        /// <summary>
        /// تنبيهات الشات مفعلة
        /// Notifications enabled
        /// </summary>
        public bool NotificationsEnabled { get; set; }

        /// <summary>
        /// الصوت مفعّل
        /// Sound enabled
        /// </summary>
        public bool SoundEnabled { get; set; }

        /// <summary>
        /// عرض إيصالات القراءة
        /// Show read receipts
        /// </summary>
        public bool ShowReadReceipts { get; set; }

        /// <summary>
        /// عرض مؤشر الكتابة
        /// Show typing indicator
        /// </summary>
        public bool ShowTypingIndicator { get; set; }

        /// <summary>
        /// المظهر (light, dark, auto)
        /// Theme
        /// </summary>
        public string Theme { get; set; } = string.Empty;

        /// <summary>
        /// حجم الخط (small, medium, large)
        /// Font size
        /// </summary>
        public string FontSize { get; set; } = string.Empty;

        /// <summary>
        /// التحميل التلقائي للوسائط
        /// Auto-download media
        /// </summary>
        public bool AutoDownloadMedia { get; set; }

        /// <summary>
        /// نسخ احتياطي للرسائل
        /// Backup messages
        /// </summary>
        public bool BackupMessages { get; set; }
    }
} 
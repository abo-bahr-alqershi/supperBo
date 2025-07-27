namespace YemenBooking.Core.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// ملف تعريف مرفق داخل محادثة
    /// Entity representing a chat attachment
    /// </summary>
    [Display(Name = "مرفق المحادثة")]
    public class ChatAttachment : BaseEntity
    {
        /// <summary>
        /// معرف المحادثة المرتبط بالمرفق
        /// ID of the conversation this attachment belongs to
        /// </summary>
        [Display(Name = "معرف المحادثة")]
        public Guid ConversationId { get; set; }

        /// <summary>
        /// اسم الملف الأصلي
        /// Original file name
        /// </summary>
        [Display(Name = "اسم الملف الأصلي")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// نوع المحتوى (MIME type)
        /// Content type of the file
        /// </summary>
        [Display(Name = "نوع المحتوى")]
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// حجم الملف بالبايت
        /// Size of the file in bytes
        /// </summary>
        [Display(Name = "حجم الملف (بايت)")]
        public long FileSize { get; set; }

        /// <summary>
        /// المسار على الخادم حيث يخزن الملف
        /// File path on server
        /// </summary>
        [Display(Name = "مسار الملف على الخادم")]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// المستخدم الذي رفع الملف
        /// User who uploaded the file
        /// </summary>
        [Display(Name = "رفعه بواسطة")]
        public Guid UploadedBy { get; set; }

        /// <summary>
        /// تاريخ رفع الملف
        /// Date when the file was uploaded
        /// </summary>
        [Display(Name = "تاريخ الرفع")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 
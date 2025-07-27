using System;
using System.Collections.Generic;

namespace YemenBooking.Application.DTOs
{
    /// <summary>
    /// DTO لبيانات الإشعار
    /// DTO for notification data
    /// </summary>
    public class NotificationDto
    {
        /// <summary>
        /// معرف الإشعار
        /// Notification identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// نوع الإشعار
        /// Notification type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// عنوان الإشعار
        /// Notification title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// محتوى الإشعار
        /// Notification message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// أولوية الإشعار
        /// Notification priority
        /// </summary>
        public string Priority { get; set; }

        /// <summary>
        /// حالة الإشعار
        /// Notification status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// معرف المستلم
        /// Recipient identifier
        /// </summary>
        public Guid RecipientId { get; set; }

        /// <summary>
        /// اسم المستلم
        /// Recipient name
        /// </summary>
        public string RecipientName { get; set; }

        /// <summary>
        /// معرف المرسل
        /// Sender identifier
        /// </summary>
        public Guid? SenderId { get; set; }

        /// <summary>
        /// اسم المرسل
        /// Sender name
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// هل تم قراءة الإشعار
        /// Is notification read
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// تاريخ القراءة
        /// Read date
        /// </summary>
        public DateTime? ReadAt { get; set; }

        /// <summary>
        /// تاريخ الإنشاء
        /// Creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
} 
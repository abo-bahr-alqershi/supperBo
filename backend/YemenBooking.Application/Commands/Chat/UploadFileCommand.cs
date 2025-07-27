using System;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Chat
{
    /// <summary>
    /// أمر لرفع ملف مرفق في الشات
    /// Command to upload a chat attachment file
    /// </summary>
    public class UploadFileCommand : IRequest<ResultDto<ChatAttachmentDto>>
    {
        /// <summary>
        /// الملف المرفوع
        /// The file to upload
        /// </summary>
        [JsonPropertyName("file")]
        public IFormFile File { get; set; } = default!;

        /// <summary>
        /// نوع الرسالة الذي يحدد الفئة (text, image, audio, video, document)
        /// Message type for categorizing the attachment
        /// </summary>
        [JsonPropertyName("message_type")]
        public string MessageType { get; set; } = string.Empty;
    }
} 
namespace YemenBooking.Application.DTOs
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// DTO لتمثيل مرفق المحادثة
    /// DTO for ChatAttachment entity
    /// </summary>
    public class ChatAttachmentDto
    {
        [JsonIgnore]
        public Guid ConversationId { get; set; }

        [JsonPropertyName("attachment_id")]
        public Guid Id { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("mime_type")]
        public string ContentType { get; set; } = string.Empty;

        [JsonPropertyName("file_size")]
        public long FileSize { get; set; }

        [JsonIgnore]
        public string FilePath { get; set; } = string.Empty;

        [JsonIgnore]
        public Guid UploadedBy { get; set; }

        [JsonPropertyName("uploaded_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("file_url")]
        public string FileUrl => $"/api/common/chat/attachments/{Id}";
    }
} 
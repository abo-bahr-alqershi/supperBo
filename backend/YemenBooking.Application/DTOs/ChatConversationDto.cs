namespace YemenBooking.Application.DTOs
{
    using System;
    using System.Collections.Generic;
    using YemenBooking.Application.DTOs.Users; // Added for UserDto
    using System.Text.Json.Serialization;

    /// <summary>
    /// DTO لتمثيل المحادثة بين المستخدمين
    /// DTO for ChatConversation entity
    /// </summary>
    public class ChatConversationDto
    {
        [JsonPropertyName("conversation_id")]
        public Guid Id { get; set; }
        public string ConversationType { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ChatMessageDto? LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public bool IsArchived { get; set; }
        public bool IsMuted { get; set; }
       
        /// <summary>
        /// معرّف الفندق المرتبط بالمحادثة
        /// Property ID associated with the conversation
        /// </summary>
        public Guid? PropertyId { get; set; }

        public IEnumerable<UserDto> Participants { get; set; } = new List<UserDto>();
    }
} 
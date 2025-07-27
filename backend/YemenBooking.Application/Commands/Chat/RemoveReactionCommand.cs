namespace YemenBooking.Application.Commands.Chat
{
    using System;
    using MediatR;
    using YemenBooking.Application.DTOs;

    /// <summary>
    /// أمر لإزالة تفاعل من رسالة
    /// Command to remove a reaction from a chat message
    /// </summary>
    public class RemoveReactionCommand : IRequest<ResultDto>
    {
        /// <summary>
        /// معرف الرسالة
        /// Message ID
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// نوع التفاعل المراد إزالته
        /// Reaction type to remove
        /// </summary>
        public string ReactionType { get; set; } = string.Empty;
    }
} 
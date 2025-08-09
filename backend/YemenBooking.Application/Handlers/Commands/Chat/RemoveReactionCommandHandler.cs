using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces; // Added for IUnitOfWork
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Commands.Chat
{
    /// <summary>
    /// معالج أمر إزالة تفاعل من رسالة
    /// </summary>
    public class RemoveReactionCommandHandler : IRequestHandler<RemoveReactionCommand, ResultDto>
    {
        private readonly IMessageReactionRepository _reactionRepo;
        private readonly IChatMessageRepository _messageRepo;
        private readonly IChatConversationRepository _conversationRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebSocketService _webSocketService;
        private readonly ILogger<RemoveReactionCommandHandler> _logger;

        public RemoveReactionCommandHandler(
            IMessageReactionRepository reactionRepo,
            IChatMessageRepository messageRepo,
            IChatConversationRepository conversationRepo,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IWebSocketService webSocketService,
            ILogger<RemoveReactionCommandHandler> logger)
        {
            _reactionRepo = reactionRepo;
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _webSocketService = webSocketService;
            _logger = logger;
        }

        public async Task<ResultDto> Handle(RemoveReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                _logger.LogInformation("المستخدم {UserId} يزيل تفاعل {ReactionType} من الرسالة {MessageId}", userId, request.ReactionType, request.MessageId);

                var reaction = (await _unitOfWork.Repository<MessageReaction>().FindAsync(r => r.MessageId == request.MessageId && r.UserId == userId && r.ReactionType == request.ReactionType, cancellationToken)).FirstOrDefault();
                if (reaction == null)
                    return ResultDto.Failure("التفاعل غير موجود");

                await _unitOfWork.Repository<MessageReaction>().DeleteAsync(reaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // إرسال حدث إزالة التفاعل عبر WebSocket
                var message = await _messageRepo.GetByIdAsync(request.MessageId, cancellationToken);
                var conversation = await _conversationRepo.GetByIdAsync(message.ConversationId, cancellationToken);
                // Send structured event via WebSocket to other participants
                var eventData = new { conversationId = message.ConversationId, messageId = message.Id, reactionId = reaction.Id };
                foreach (var participant in conversation.Participants.Where(p => p.Id != userId))
                {
                    await _webSocketService.SendEventAsync(participant.Id, "ReactionRemoved", eventData, cancellationToken);
                }

                return ResultDto.Ok("تم إزالة التفاعل بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إزالة التفاعل");
                return ResultDto.Failure("حدث خطأ أثناء إزالة التفاعل");
            }
        }
    }
} 
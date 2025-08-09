using System;
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
using System.Linq;
using System.Text.Json;
using AutoMapper;

namespace YemenBooking.Application.Handlers.Commands.Chat
{
    /// <summary>
    /// معالج أمر إضافة تفاعل إلى رسالة
    /// </summary>
    public class AddReactionCommandHandler : IRequestHandler<AddReactionCommand, ResultDto>
    {
        private readonly IMessageReactionRepository _reactionRepo;
        private readonly IChatMessageRepository _messageRepo;
        private readonly IChatConversationRepository _conversationRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebSocketService _webSocketService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddReactionCommandHandler> _logger;

        public AddReactionCommandHandler(
            IMessageReactionRepository reactionRepo,
            IChatMessageRepository messageRepo,
            IChatConversationRepository conversationRepo,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IWebSocketService webSocketService,
            IMapper mapper,
            ILogger<AddReactionCommandHandler> logger)
        {
            _reactionRepo = reactionRepo;
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _webSocketService = webSocketService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDto> Handle(AddReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _currentUserService.UserId;
                _logger.LogInformation("المستخدم {UserId} يضيف تفاعل {ReactionType} على الرسالة {MessageId}", userId, request.ReactionType, request.MessageId);

                var reaction = new MessageReaction
                {
                    Id = Guid.NewGuid(),
                    MessageId = request.MessageId,
                    UserId = userId,
                    ReactionType = request.ReactionType,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<MessageReaction>().AddAsync(reaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // إرسال حدث إضافة التفاعل عبر WebSocket
                var message = await _messageRepo.GetByIdAsync(request.MessageId, cancellationToken);
                var conversation = await _conversationRepo.GetByIdAsync(message.ConversationId, cancellationToken);
                var reactionDto = _mapper.Map<MessageReactionDto>(reaction);
                // Send structured event via WebSocket to other participants
                foreach (var participant in conversation.Participants.Where(p => p.Id != userId))
                {
                    await _webSocketService.SendEventAsync(
                        participant.Id,
                        "ReactionAdded",
                        new { conversationId = message.ConversationId, messageId = message.Id, reaction = reactionDto },
                        cancellationToken);
                }

                return ResultDto.Ok("تم إضافة التفاعل بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء إضافة التفاعل");
                return ResultDto.Failure("حدث خطأ أثناء إضافة التفاعل");
            }
        }
    }
} 
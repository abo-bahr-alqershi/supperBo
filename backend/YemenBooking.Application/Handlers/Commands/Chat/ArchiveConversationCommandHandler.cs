using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using System.Text.Json;
using AutoMapper;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using System.Linq;

namespace YemenBooking.Application.Handlers.Commands.Chat
{
    /// <summary>
    /// معالج أمر أرشفة المحادثة
    /// </summary>
    public class ArchiveConversationCommandHandler : IRequestHandler<ArchiveConversationCommand, ResultDto>
    {
        private readonly IChatConversationRepository _conversationRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebSocketService _webSocketService;
        private readonly ILogger<ArchiveConversationCommandHandler> _logger;

        public ArchiveConversationCommandHandler(
            IChatConversationRepository conversationRepo,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWebSocketService webSocketService,
            ILogger<ArchiveConversationCommandHandler> logger)
        {
            _conversationRepo = conversationRepo;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webSocketService = webSocketService;
            _logger = logger;
        }

        public async Task<ResultDto> Handle(ArchiveConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var conversation = await _conversationRepo.GetByIdAsync(request.ConversationId, cancellationToken);
                if (conversation == null)
                    return ResultDto.Failure("المحادثة غير موجودة", errorCode: "conversation_not_found");

                conversation.IsArchived = true;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                // Broadcast conversation archived via WebSocket
                // Send structured event via WebSocket for conversation updated (archived)
                var convDto = _mapper.Map<ChatConversationDto>(conversation);
                var userId = _currentUserService.UserId;
                foreach (var participant in conversation.Participants.Where(p => p.Id != userId))
                {
                    await _webSocketService.SendEventAsync(
                        participant.Id,
                        "conversation_updated",
                        new { conversation_id = conversation.Id, conversation = convDto },
                        cancellationToken);
                }

                return ResultDto.Ok("تم أرشفة المحادثة");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء أرشفة المحادثة");
                return ResultDto.Failure("حدث خطأ أثناء أرشفة المحادثة");
            }
        }
    }
} 
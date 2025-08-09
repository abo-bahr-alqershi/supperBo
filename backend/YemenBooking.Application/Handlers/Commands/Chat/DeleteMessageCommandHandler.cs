using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using System.Text.Json;
using YemenBooking.Core.Interfaces.Services;
 
 namespace YemenBooking.Application.Handlers.Commands.Chat
 {
     /// <summary>
     /// معالج أمر حذف رسالة المحادثة
     /// Handler for DeleteMessageCommand
     /// </summary>
     public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, ResultDto>
     {
        private readonly IChatMessageRepository _messageRepo;
        private readonly IChatConversationRepository _conversationRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebSocketService _webSocketService;
 
        public DeleteMessageCommandHandler(IChatMessageRepository messageRepo, IChatConversationRepository conversationRepo, IUnitOfWork unitOfWork, IWebSocketService webSocketService)
        {
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _unitOfWork = unitOfWork;
            _webSocketService = webSocketService;
        }
 
        public async Task<ResultDto> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _messageRepo.GetByIdAsync(request.MessageId, cancellationToken);
            if (message == null)
                return ResultDto.Failed("الرسالة غير موجودة");

            await _messageRepo.DeleteAsync(message, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // إرسال حدث حذف الرسالة عبر WebSocket
            var conversation = await _conversationRepo.GetByIdAsync(message.ConversationId, cancellationToken);
            // Send structured event via WebSocket to all participants
            foreach (var participant in conversation.Participants)
            {
                await _webSocketService.SendEventAsync(
                    participant.Id,
                    "MessageDeleted",
                    new { conversationId = message.ConversationId, messageId = message.Id },
                    cancellationToken);
            }

            return ResultDto.Ok(null, "تم حذف الرسالة بنجاح");
        }
    }
} 
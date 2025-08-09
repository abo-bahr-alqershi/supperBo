using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Commands.Chat
{
    /// <summary>
    /// معالج أمر تحديث حالة الرسالة (sent, delivered, read, failed)
    /// </summary>
    public class UpdateMessageStatusCommandHandler : IRequestHandler<UpdateMessageStatusCommand, ResultDto>
    {
        private readonly IChatMessageRepository _messageRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebSocketService _webSocketService;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateMessageStatusCommandHandler> _logger;

        public UpdateMessageStatusCommandHandler(
            IChatMessageRepository messageRepo,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IWebSocketService webSocketService,
            IMapper mapper,
            ILogger<UpdateMessageStatusCommandHandler> logger)
        {
            _messageRepo = messageRepo;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _webSocketService = webSocketService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultDto> Handle(UpdateMessageStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var message = await _messageRepo.GetByIdAsync(request.MessageId, cancellationToken);
                if (message == null)
                    return ResultDto.Failed("الرسالة غير موجودة", errorCode: "message_not_found");

                message.Status = request.Status;
                var now = DateTime.UtcNow;
                if (request.Status.Equals("delivered", StringComparison.OrdinalIgnoreCase))
                    message.DeliveredAt = now;
                else if (request.Status.Equals("read", StringComparison.OrdinalIgnoreCase))
                    message.ReadAt = now;
                message.UpdatedAt = now;

                await _messageRepo.UpdateAsync(message, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Broadcast updated message to sender via WebSocket
                var messageDto = _mapper.Map<ChatMessageDto>(message);
                // Send structured event via WebSocket
                await _webSocketService.SendEventAsync(
                    message.SenderId,
                    "MessageStatusUpdated",
                    new { messageId = message.Id, conversationId = message.ConversationId, status = message.Status },
                    cancellationToken);

                return ResultDto.Ok(null, "تم تحديث حالة الرسالة");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ أثناء تحديث حالة الرسالة");
                return ResultDto.Failed("حدث خطأ أثناء تحديث حالة الرسالة");
            }
        }
    }
} 
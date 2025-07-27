using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Application.Commands.Chat;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Services;
using System.Text.Json;
using System;
 
 namespace YemenBooking.Application.Handlers.Commands.Chat
 {
     /// <summary>
     /// معالج أمر تحديث حالة المستخدم
     /// Handler for UpdateUserStatusCommand
     /// </summary>
     public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, ResultDto>
     {
        private readonly IWebSocketService _wsService;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserStatusCommandHandler(IWebSocketService wsService, ICurrentUserService currentUserService)
        {
            _wsService = wsService;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            // Send structured event via WebSocket for user status change
            await _wsService.SendEventAsync(
                userId,
                "user_status_changed",
                new { user_id = userId, status = request.Status, timestamp = DateTime.UtcNow },
                cancellationToken);
            return ResultDto.Ok(null, "تم تحديث حالة المستخدم");
        }
    }
} 
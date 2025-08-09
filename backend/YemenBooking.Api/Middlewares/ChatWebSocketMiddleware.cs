namespace YemenBooking.Api.Middlewares
{
    using System;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using YemenBooking.Infrastructure.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;
    using System.Text.Json;
    using Microsoft.Extensions.DependencyInjection;
    using YemenBooking.Core.Interfaces.Repositories;
    using YemenBooking.Core.Interfaces;
    using YemenBooking.Core.Interfaces.Services;

    /// <summary>
    /// Middleware to handle WebSocket connections for chat
    /// </summary>
    public class ChatWebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketConnectionManager _manager;
        private readonly ILogger<ChatWebSocketMiddleware> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebSocketService _webSocketService;

        public ChatWebSocketMiddleware(
            RequestDelegate next,
            WebSocketConnectionManager manager,
            ILogger<ChatWebSocketMiddleware> logger,
            IServiceProvider serviceProvider,
            IWebSocketService webSocketService)
        {
            _next = next;
            _manager = manager;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _webSocketService = webSocketService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/chathub" && context.WebSockets.IsWebSocketRequest)
            {
                var query = context.Request.Query;
                // Validate JWT token passed as query parameter
                var token = query["access_token"].FirstOrDefault() ?? query["token"].FirstOrDefault();
                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                // Inject Authorization header for JWT validation
                context.Request.Headers["Authorization"] = $"Bearer {token}";
                var authResult = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                if (!authResult.Succeeded || authResult.Principal == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                // Extract user ID from claims
                var user = authResult.Principal;
                var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (!Guid.TryParse(userIdValue, out var userId))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                var socket = await context.WebSockets.AcceptWebSocketAsync();
                _manager.AddSocket(userId, socket);
                _logger.LogInformation("WebSocket connection opened for user {UserId}", userId);

                await Receive(socket, userId);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task Receive(WebSocket socket, Guid userId)
        {
            var buffer = new byte[1024 * 8];
            var sb = new System.Text.StringBuilder();
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    _manager.RemoveSocket(userId);
                    _logger.LogInformation("WebSocket connection closed for user {UserId}", userId);
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    var messageChunk = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                    sb.Append(messageChunk);
                    if (result.EndOfMessage)
                    {
                        var text = sb.ToString();
                        sb.Clear();
                        try
                        {
                            var doc = System.Text.Json.JsonDocument.Parse(text);
                            if (doc.RootElement.TryGetProperty("type", out var typeEl))
                            {
                                var type = typeEl.GetString() ?? string.Empty;
                                var hasData = doc.RootElement.TryGetProperty("data", out var dataEl);
                                switch (type)
                                {
                                    case "Typing":
                                        if (hasData)
                                        {
                                            await HandleTypingAsync(userId, dataEl);
                                        }
                                        break;
                                    case "UpdatePresence":
                                        if (hasData)
                                        {
                                            await HandlePresenceAsync(userId, dataEl);
                                        }
                                        break;
                                    case "MarkAsRead":
                                        if (hasData)
                                        {
                                            await HandleMarkAsReadAsync(userId, dataEl);
                                        }
                                        break;
                                    default:
                                        _logger.LogWarning("Unknown WS event type: {Type}", type);
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to parse WS message from {UserId}", userId);
                        }
                    }
                }
            }
        }

        private async Task HandleTypingAsync(Guid userId, JsonElement data)
        {
            try
            {
                var convIdStr = data.TryGetProperty("conversationId", out var convEl) ? convEl.GetString() : null;
                var isTyping = data.TryGetProperty("isTyping", out var typingEl) && typingEl.GetBoolean();
                if (string.IsNullOrWhiteSpace(convIdStr) || !Guid.TryParse(convIdStr, out var conversationId))
                {
                    _logger.LogWarning("Typing event with invalid conversationId from {UserId}", userId);
                    return;
                }
                using var scope = _serviceProvider.CreateScope();
                var convRepo = scope.ServiceProvider.GetRequiredService<IChatConversationRepository>();
                var conversation = await convRepo.GetByIdAsync(conversationId, CancellationToken.None);
                if (conversation == null)
                {
                    _logger.LogWarning("Typing event for non-existent conversation {ConversationId}", conversationId);
                    return;
                }
                foreach (var participant in conversation.Participants.Where(p => p.Id != userId))
                {
                    await _webSocketService.SendEventAsync(participant.Id, "UserTyping", new { conversationId = conversationId, userId = userId, isTyping = isTyping }, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling Typing event for {UserId}", userId);
            }
        }

        private async Task HandlePresenceAsync(Guid userId, JsonElement data)
        {
            try
            {
                var status = data.TryGetProperty("status", out var statusEl) ? (statusEl.GetString() ?? "online") : "online";
                var now = DateTime.UtcNow;
                // Broadcast to all connected users
                foreach (var otherUserId in _manager.GetConnectedUserIds().Where(id => id != userId))
                {
                    await _webSocketService.SendEventAsync(otherUserId, "UserPresence", new { userId = userId, status = status, lastSeen = now }, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling UpdatePresence event for {UserId}", userId);
            }
        }

        private async Task HandleMarkAsReadAsync(Guid userId, JsonElement data)
        {
            try
            {
                var convIdStr = data.TryGetProperty("conversationId", out var convEl) ? convEl.GetString() : null;
                if (string.IsNullOrWhiteSpace(convIdStr) || !Guid.TryParse(convIdStr, out var conversationId))
                {
                    _logger.LogWarning("MarkAsRead with invalid conversationId from {UserId}", userId);
                    return;
                }
                var messageIds = new System.Collections.Generic.List<Guid>();
                if (data.TryGetProperty("messageIds", out var idsEl) && idsEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var idEl in idsEl.EnumerateArray())
                    {
                        var idStr = idEl.GetString();
                        if (Guid.TryParse(idStr, out var mid)) messageIds.Add(mid);
                    }
                }
                if (messageIds.Count == 0) return;

                using var scope = _serviceProvider.CreateScope();
                var messageRepo = scope.ServiceProvider.GetRequiredService<IChatMessageRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                foreach (var mid in messageIds)
                {
                    var message = await messageRepo.GetByIdAsync(mid, CancellationToken.None);
                    if (message == null) continue;
                    message.Status = "read";
                    message.ReadAt = DateTime.UtcNow;
                    await messageRepo.UpdateAsync(message, CancellationToken.None);
                    await unitOfWork.SaveChangesAsync(CancellationToken.None);
                    // Notify original sender
                    await _webSocketService.SendEventAsync(message.SenderId, "MessageStatusUpdated", new { messageId = message.Id, conversationId = message.ConversationId, status = message.Status }, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling MarkAsRead event for {UserId}", userId);
            }
        }
    }
} 
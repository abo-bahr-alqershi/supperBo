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

    /// <summary>
    /// Middleware to handle WebSocket connections for chat
    /// </summary>
    public class ChatWebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketConnectionManager _manager;
        private readonly ILogger<ChatWebSocketMiddleware> _logger;

        public ChatWebSocketMiddleware(RequestDelegate next, WebSocketConnectionManager manager, ILogger<ChatWebSocketMiddleware> logger)
        {
            _next = next;
            _manager = manager;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
            {
                var query = context.Request.Query;
                // Validate JWT token passed as query parameter
                var token = query["token"].FirstOrDefault();
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
            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    _manager.RemoveSocket(userId);
                    _logger.LogInformation("WebSocket connection closed for user {UserId}", userId);
                }
            }
        }
    }
} 
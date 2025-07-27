using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YemenBooking.Core.Interfaces.Services;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// تنفيذ خدمة WebSocket لإرسال الرسائل الحقيقية للمستخدمين
    /// Real WebSocket service implementation
    /// </summary>
    public class WebSocketService : IWebSocketService
    {
        private readonly WebSocketConnectionManager _manager;
        private readonly ILogger<WebSocketService> _logger;

        public WebSocketService(WebSocketConnectionManager manager, ILogger<WebSocketService> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        public async Task SendMessageAsync(Guid userId, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("إرسال رسالة WebSocket للمستخدم {UserId}: {Message}", userId, message);
            await _manager.SendMessageAsync(userId, message, cancellationToken);
        }

        // Add snake_case naming policy for JSON serialization
        private class SnakeCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name)
            {
                if (string.IsNullOrEmpty(name)) return name;
                var builder = new StringBuilder();
                builder.Append(char.ToLowerInvariant(name[0]));
                for (int i = 1; i < name.Length; i++)
                {
                    var c = name[i];
                    if (char.IsUpper(c))
                    {
                        builder.Append('_');
                        builder.Append(char.ToLowerInvariant(c));
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
                return builder.ToString();
            }
        }

        // Add method to send structured events
        public async Task SendEventAsync(Guid userId, string eventType, object data, CancellationToken cancellationToken = default)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            options.Converters.Add(new JsonStringEnumConverter());
            var envelope = new { event_type = eventType, data };
            var message = JsonSerializer.Serialize(envelope, options);
            await SendMessageAsync(userId, message, cancellationToken);
        }
    }
} 
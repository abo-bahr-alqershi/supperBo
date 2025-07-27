namespace YemenBooking.Infrastructure.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// مدير اتصالات WebSocket للمستخدمين
    /// Manages WebSocket connections per user
    /// </summary>
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new();

        /// <summary>
        /// إضافة اتصال WebSocket لمستخدم
        /// Add WebSocket connection for a user
        /// </summary>
        public void AddSocket(Guid userId, WebSocket socket) => _sockets[userId] = socket;

        /// <summary>
        /// إزالة اتصال WebSocket لمستخدم
        /// Remove WebSocket connection for a user
        /// </summary>
        public void RemoveSocket(Guid userId) => _sockets.TryRemove(userId, out _);

        /// <summary>
        /// إرسال رسالة نصية عبر WebSocket لمستخدم محدد
        /// Send a text message to a specific user via WebSocket
        /// </summary>
        public async Task SendMessageAsync(Guid userId, string message, CancellationToken cancellationToken = default)
        {
            if (_sockets.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
            }
        }
    }
} 
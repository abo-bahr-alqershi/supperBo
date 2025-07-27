namespace YemenBooking.Core.Interfaces.Repositories
{
    using YemenBooking.Core.Entities;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    /// <summary>
    /// واجهة مستودع المحادثات
    /// Chat conversation repository interface
    /// </summary>
    public interface IChatConversationRepository : IRepository<ChatConversation>
    {
        /// <summary>
        /// الحصول على المحادثات المرتبطة بمشارك محدد
        /// Get conversations by participant
        /// </summary>
        Task<(IEnumerable<ChatConversation> Items, int TotalCount)> GetConversationsByParticipantAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    }
} 
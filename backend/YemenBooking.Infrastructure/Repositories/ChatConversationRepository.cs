using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Infrastructure.Data.Context;

namespace YemenBooking.Infrastructure.Repositories
{
    /// <summary>
    /// تنفيذ مستودع المحادثات
    /// Chat conversation repository implementation
    /// </summary>
    public class ChatConversationRepository : BaseRepository<ChatConversation>, IChatConversationRepository
    {
        public ChatConversationRepository(YemenBookingDbContext context) : base(context) { }

        public async Task<(IEnumerable<ChatConversation> Items, int TotalCount)> GetConversationsByParticipantAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<ChatConversation>()
                .Include(c => c.Participants)
                .Include(c => c.Messages)
                .Where(c => c.Participants.Any(p => p.Id == userId));

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(c => c.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }
    }
} 
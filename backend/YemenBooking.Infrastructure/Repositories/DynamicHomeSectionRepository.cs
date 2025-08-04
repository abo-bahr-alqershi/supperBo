using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YemenBooking.Core.Entities;
using YemenBooking.Infrastructure.Data.Context;

namespace YemenBooking.Infrastructure.Repositories
{
    public class DynamicHomeSectionRepository : BaseRepository<DynamicHomeSection>
    {
        public DynamicHomeSectionRepository(YemenBookingDbContext context) : base(context)
        {
        }

        public async Task<List<DynamicHomeSection>> GetActiveSectionsAsync()
        {
            return await _context.DynamicHomeSections
                .Include(s => s.Content.Where(c => c.IsActive && 
                    (!c.ExpiresAt.HasValue || c.ExpiresAt >= DateTime.UtcNow)))
                .Where(s => s.IsActive && 
                    (!s.ScheduledAt.HasValue || s.ScheduledAt <= DateTime.UtcNow) &&
                    (!s.ExpiresAt.HasValue || s.ExpiresAt >= DateTime.UtcNow))
                .OrderBy(s => s.Order)
                .ThenByDescending(s => s.Priority)
                .ToListAsync();
        }

        public async Task<List<DynamicHomeSection>> GetSectionsByTypeAsync(string sectionType)
        {
            return await _context.DynamicHomeSections
                .Include(s => s.Content)
                .Where(s => s.SectionType == sectionType)
                .OrderBy(s => s.Order)
                .ToListAsync();
        }

        public async Task<List<DynamicHomeSection>> GetVisibleSectionsForAudienceAsync(List<string> targetAudience)
        {
            return await _context.DynamicHomeSections
                .Include(s => s.Content.Where(c => c.IsActive && 
                    (!c.ExpiresAt.HasValue || c.ExpiresAt >= DateTime.UtcNow)))
                .Where(s => s.IsActive && 
                    (!s.ScheduledAt.HasValue || s.ScheduledAt <= DateTime.UtcNow) &&
                    (!s.ExpiresAt.HasValue || s.ExpiresAt >= DateTime.UtcNow) &&
                    (targetAudience.Count == 0 || 
                     targetAudience.Any(ta => s.TargetAudience.Contains(ta))))
                .OrderBy(s => s.Order)
                .ThenByDescending(s => s.Priority)
                .ToListAsync();
        }

        public async Task<bool> UpdateSectionOrderAsync(Guid sectionId, int newOrder)
        {
            var section = await GetByIdAsync(sectionId);
            if (section == null) return false;

            section.UpdateOrder(newOrder);
            await UpdateAsync(section);
            return true;
        }

        public async Task<bool> ToggleSectionStatusAsync(Guid sectionId, bool? setActive = null)
        {
            var section = await GetByIdAsync(sectionId);
            if (section == null) return false;

            if (setActive.HasValue)
            {
                section.SetActiveStatus(setActive.Value);
            }
            else
            {
                section.ToggleActiveStatus();
            }

            await UpdateAsync(section);
            return true;
        }
    }
}
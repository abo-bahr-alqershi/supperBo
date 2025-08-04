using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YemenBooking.Core.Entities;
using YemenBooking.Infrastructure.Data.Context;

namespace YemenBooking.Infrastructure.Repositories
{
    public class SponsoredAdSectionRepository : BaseRepository<SponsoredAdSection>
    {
        public SponsoredAdSectionRepository(YemenBookingDbContext context) : base(context)
        {
        }

        public async Task<List<SponsoredAdSection>> GetActiveAdsAsync(int? limit = null)
        {
            IQueryable<SponsoredAdSection> query = _context.SponsoredAdSections
                .Where(ad => ad.IsActive && 
                    DateTime.UtcNow >= ad.StartDate && 
                    DateTime.UtcNow <= ad.EndDate)
                .OrderByDescending(ad => ad.Priority)
                .ThenBy(ad => ad.StartDate);

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<List<SponsoredAdSection>> GetAdsByTargetAudienceAsync(
            List<string> targetAudience, 
            int? limit = null)
        {
            IQueryable<SponsoredAdSection> query = _context.SponsoredAdSections
                .Where(ad => ad.IsActive && 
                    DateTime.UtcNow >= ad.StartDate && 
                    DateTime.UtcNow <= ad.EndDate &&
                    (targetAudience.Count == 0 || 
                     targetAudience.Any(ta => ad.TargetingData.Contains(ta))))
                .OrderByDescending(ad => ad.Priority)
                .ThenBy(ad => ad.StartDate);

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> RecordImpressionAsync(Guid adId)
        {
            var ad = await GetByIdAsync(adId);
            if (ad == null || !ad.IsCurrentlyActive) return false;

            ad.RecordImpression();
            await UpdateAsync(ad);
            return true;
        }

        public async Task<bool> RecordClickAsync(Guid adId)
        {
            var ad = await GetByIdAsync(adId);
            if (ad == null || !ad.IsCurrentlyActive) return false;

            ad.RecordClick();
            await UpdateAsync(ad);
            return true;
        }

        public async Task<List<SponsoredAdSection>> GetExpiredAdsAsync()
        {
            return await _context.SponsoredAdSections
                .Where(ad => ad.IsActive && DateTime.UtcNow > ad.EndDate)
                .ToListAsync();
        }

        public async Task<List<SponsoredAdSection>> GetScheduledAdsAsync()
        {
            return await _context.SponsoredAdSections
                .Where(ad => ad.IsActive && DateTime.UtcNow < ad.StartDate)
                .OrderBy(ad => ad.StartDate)
                .ToListAsync();
        }

        public async Task<Dictionary<string, object>> GetAdAnalyticsAsync(Guid adId)
        {
            var ad = await GetByIdAsync(adId);
            if (ad == null) return null;

            return new Dictionary<string, object>
            {
                ["id"] = ad.Id,
                ["title"] = ad.Title,
                ["impressionCount"] = ad.ImpressionCount,
                ["clickCount"] = ad.ClickCount,
                ["conversionRate"] = ad.ConversionRate,
                ["isCurrentlyActive"] = ad.IsCurrentlyActive,
                ["remainingTime"] = ad.RemainingTime,
                ["priority"] = ad.Priority
            };
        }
    }
}
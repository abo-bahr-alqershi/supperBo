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
    /// تنفيذ مستودع قواعد التسعير
    /// Pricing rule repository implementation
    /// </summary>
    public class PricingRuleRepository : BaseRepository<PricingRule>, IPricingRuleRepository
    {
        public PricingRuleRepository(YemenBookingDbContext context) : base(context) { }

        public async Task<PricingRule> CreatePricingRuleAsync(PricingRule pricingRule, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(pricingRule, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return pricingRule;
        }

        public async Task<PricingRule?> GetPricingRuleByIdAsync(Guid pricingRuleId, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync(new object[] { pricingRuleId }, cancellationToken);

        public async Task<PricingRule> UpdatePricingRuleAsync(PricingRule pricingRule, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(pricingRule);
            await _context.SaveChangesAsync(cancellationToken);
            return pricingRule;
        }

        public async Task<bool> DeletePricingRuleAsync(Guid pricingRuleId, CancellationToken cancellationToken = default)
        {
            var existing = await GetPricingRuleByIdAsync(pricingRuleId, cancellationToken);
            if (existing == null) return false;
            _dbSet.Remove(existing);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IEnumerable<PricingRule>> GetPricingRulesByUnitAsync(Guid unitId, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable().Where(p => p.UnitId == unitId);
            if (fromDate.HasValue)
                query = query.Where(p => p.StartDate >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(p => p.EndDate <= toDate.Value);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<bool> HasOverlapAsync(Guid unitId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(p => p.UnitId == unitId && p.StartDate < endDate && p.EndDate > startDate, cancellationToken);
        }
    }
} 
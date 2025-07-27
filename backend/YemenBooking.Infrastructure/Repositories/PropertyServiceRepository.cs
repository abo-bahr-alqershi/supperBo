using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Infrastructure.Data.Context;

namespace YemenBooking.Infrastructure.Repositories
{
    /// <summary>
    /// تنفيذ مستودع خدمات الكيانات
    /// Property service repository implementation
    /// </summary>
    public class PropertyServiceRepository : BaseRepository<PropertyService>, IPropertyServiceRepository
    {
        public PropertyServiceRepository(YemenBookingDbContext context) : base(context) { }

        public async Task<PropertyService> CreatePropertyServiceAsync(PropertyService propertyService, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(propertyService, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return propertyService;
        }

        public async Task<PropertyService?> GetPropertyServiceByIdAsync(Guid propertyServiceId, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync(new object[]{propertyServiceId}, cancellationToken);

        public async Task<PropertyService?> GetServiceByIdAsync(Guid serviceId, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);

        public async Task<PropertyService> UpdatePropertyServiceAsync(PropertyService propertyService, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(propertyService);
            await _context.SaveChangesAsync(cancellationToken);
            return propertyService;
        }

        public async Task<bool> DeletePropertyServiceAsync(Guid propertyServiceId, CancellationToken cancellationToken = default)
        {
            var ps = await GetPropertyServiceByIdAsync(propertyServiceId, cancellationToken);
            if (ps == null) return false;
            _dbSet.Remove(ps);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IEnumerable<PropertyService>> GetPropertyServicesAsync(Guid propertyId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(ps => ps.PropertyId == propertyId).ToListAsync(cancellationToken);

        public async Task<IEnumerable<PropertyService>> GetServicesByTypeAsync(string serviceType, CancellationToken cancellationToken = default)
            => await _dbSet.Where(ps => ps.Name == serviceType).ToListAsync(cancellationToken);

        public async Task<Property?> GetPropertyByIdAsync(Guid propertyId, CancellationToken cancellationToken = default)
            => await _context.Set<Property>().FirstOrDefaultAsync(p => p.Id == propertyId, cancellationToken);
    }
}
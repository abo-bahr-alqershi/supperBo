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
    public class FavoriteRepository : BaseRepository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(YemenBookingDbContext context) : base(context) { }

        public async Task<IEnumerable<Favorite>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Favorite>()
                .Where(f => f.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public Task<(IEnumerable<Favorite> Items, int TotalCount)> GetUserFavoritesAsync(Guid userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Favorite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(id, cancellationToken);
        }

        public Task<bool> ExistsAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Favorite> AddAsync(Favorite favorite, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByUserAndPropertyAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountUserFavoritesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<(Guid PropertyId, int FavoriteCount)>> GetMostFavoritedPropertiesAsync(int limit = 10, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

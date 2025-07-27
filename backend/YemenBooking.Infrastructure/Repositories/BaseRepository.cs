using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces;
using YemenBooking.Infrastructure.Data.Context;

namespace YemenBooking.Infrastructure.Repositories
{
    /// <summary>
    /// تنفيذ الRepository العام باستخدام Entity Framework Core
    /// Generic repository implementation
    /// </summary>
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly YemenBookingDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(YemenBookingDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
            => await _dbSet.CountAsync(cancellationToken);

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.CountAsync(predicate, cancellationToken);

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync(new object[] { id }, cancellationToken) != null;

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(predicate, cancellationToken);

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.UpdateRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRangeAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await _dbSet.Where(predicate).ToListAsync(cancellationToken);
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            bool ascending = true,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();
            if (predicate != null) query = query.Where(predicate);
            var total = await query.CountAsync(cancellationToken);
            if (orderBy != null) query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (items, total);
        }

        public async Task<IEnumerable<T>> GetWithSpecificationAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedWithSpecificationAsync(
            ISpecification<T> specification,
            CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification);
            var total = await query.CountAsync(cancellationToken);
            var items = await query.Skip(specification.Skip).Take(specification.Take).ToListAsync(cancellationToken);
            return (items, total);
        }

        public async Task<int> CountWithSpecificationAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification, true);
            return await query.CountAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        /// <summary>
        /// الحصول على كائن استعلامي (IQueryable) للكيان
        /// Get queryable collection for advanced querying
        /// </summary>
        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec, bool forCount = false)
        {
            var query = _dbSet.AsQueryable();
            if (spec.IsAsNoTracking) query = query.AsNoTracking();
            if (spec.IsSplitQuery) query = query.AsSplitQuery();
            if (spec.Criteria != null) query = query.Where(spec.Criteria);
            foreach (var include in spec.Includes) query = query.Include(include);
            foreach (var includeStr in spec.IncludeStrings) query = query.Include(includeStr);
            if (!forCount)
            {
                if (spec.OrderBy != null) query = query.OrderBy(spec.OrderBy);
                else if (spec.OrderByDescending != null) query = query.OrderByDescending(spec.OrderByDescending);
                if (spec.IsPagingEnabled) query = query.Skip(spec.Skip).Take(spec.Take);
            }
            return query;
        }
    }
}

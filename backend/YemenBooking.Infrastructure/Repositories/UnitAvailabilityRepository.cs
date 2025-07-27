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
    /// تنفيذ مستودع توفر الوحدات
    /// Unit availability repository implementation
    /// </summary>
    public class UnitAvailabilityRepository : BaseRepository<UnitAvailability>, IUnitAvailabilityRepository
    {
        public UnitAvailabilityRepository(YemenBookingDbContext context) : base(context) { }

        public async Task<bool> UpdateAvailabilityAsync(Guid unitId, DateTime fromDate, DateTime toDate, bool isAvailable, CancellationToken cancellationToken = default)
        {
            // تحديث حالة الإتاحة بناءً على الفترة المحددة
            var availability = await GetByIdAsync(unitId, cancellationToken);
            if (availability == null) return false;
            // ضبط الحالة إلى available أو unavailable
            availability.Status = isAvailable ? "available" : "unavailable";
            _dbSet.Update(availability);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IDictionary<DateTime, bool>> GetUnitAvailabilityAsync(Guid unitId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            var dict = new Dictionary<DateTime, bool>();
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                var overlapping = await _context.Bookings.AnyAsync(b => b.UnitId == unitId && b.CheckIn <= date && b.CheckOut > date, cancellationToken);
                dict[date] = !overlapping;
            }
            return dict;
        }

        public async Task<bool> IsUnitAvailableAsync(Guid unitId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default)
        {
            var overlapping = await _context.Bookings.AnyAsync(b => b.UnitId == unitId && b.CheckIn < checkOut && b.CheckOut > checkIn, cancellationToken);
            return !overlapping;
        }

        public async Task<bool> BlockUnitPeriodAsync(Guid unitId, DateTime fromDate, DateTime toDate, string reason, CancellationToken cancellationToken = default)
        {
            // حجز الوحدة (تعطيل التوفر) خلال الفترة المحددة
            return await UpdateAvailabilityAsync(unitId, fromDate, toDate, false, cancellationToken);
        }

        public async Task<bool> UnblockUnitPeriodAsync(Guid unitId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            // إلغاء حجز الوحدة (تفعيل التوفر) خلال الفترة المحددة
            return await UpdateAvailabilityAsync(unitId, fromDate, toDate, true, cancellationToken);
        }

        public async Task<bool> HasOverlapAsync(Guid unitId, DateTime fromDate, DateTime toDate, Guid? excludeAvailabilityId = null, CancellationToken cancellationToken = default)
        {
            // التحقق من تداخل الفترات في جدولة الإتاحة
            var query = _dbSet.Where(a => a.UnitId == unitId && 
                                         a.StartDate < toDate && 
                                         a.EndDate > fromDate);

            // استبعاد الإتاحة المحددة إذا تم توفير معرفها
            if (excludeAvailabilityId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAvailabilityId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
    }
} 
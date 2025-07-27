using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YemenBooking.Core.Interfaces.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Core.Entities;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// تنفيذ خدمة التوفر
    /// Availability service implementation
    /// </summary>
    public class AvailabilityService : IAvailabilityService
    {
        private readonly ILogger<AvailabilityService> _logger;
        private readonly YemenBookingDbContext _dbContext;

        public AvailabilityService(ILogger<AvailabilityService> logger, YemenBookingDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<bool> CheckAvailabilityAsync(Guid unitId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default)
        {
            // تحقق من التعارض مع الحجوزات
            var bookingOverlap = await _dbContext.Bookings
                .AnyAsync(b => b.UnitId == unitId && b.CheckIn < checkOut && b.CheckOut > checkIn, cancellationToken);
            if (bookingOverlap)
                return false;
            
            // تحقق من التعارض مع التجاوزات اليدوية (الإتاحة الموقوفة)
            var overrideOverlap = await _dbContext.Set<UnitAvailability>()
                .AnyAsync(u => u.UnitId == unitId
                    && u.StartDate < checkOut
                    && u.EndDate > checkIn
                    && !u.Status.Equals("available", StringComparison.OrdinalIgnoreCase), cancellationToken);
            if (overrideOverlap)
                return false;
            
            return true;
        }

        /// <inheritdoc />
        public async Task<IDictionary<Guid, bool>> CheckMultipleUnitsAvailabilityAsync(IEnumerable<Guid> unitIds, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default)
        {
            var result = new Dictionary<Guid, bool>();
            foreach (var unitId in unitIds)
            {
                var avail = await CheckAvailabilityAsync(unitId, checkIn, checkOut, cancellationToken);
                result[unitId] = avail;
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Guid>> GetAvailableUnitsInPropertyAsync(Guid propertyId, DateTime checkIn, DateTime checkOut, int guestCount, CancellationToken cancellationToken = default)
        {
            var units = await _dbContext.Units.Where(u => u.PropertyId == propertyId).Select(u => u.Id).ToListAsync(cancellationToken);
            var available = new List<Guid>();
            foreach (var unitId in units)
            {
                if (await CheckAvailabilityAsync(unitId, checkIn, checkOut, cancellationToken))
                    available.Add(unitId);
            }
            return available;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<(DateTime Start, DateTime End, bool IsAvailable)>> GetUnitAvailabilityPeriodsAsync(Guid unitId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            var bookings = await _dbContext.Bookings
                .Where(b => b.UnitId == unitId && b.CheckOut >= fromDate && b.CheckIn <= toDate)
                .OrderBy(b => b.CheckIn)
                .ToListAsync(cancellationToken);
            var periods = new List<(DateTime, DateTime, bool)>();
            var current = fromDate;
            foreach (var b in bookings)
            {
                if (b.CheckIn > current)
                    periods.Add((current, b.CheckIn, true));
                periods.Add((b.CheckIn, b.CheckOut, false));
                current = b.CheckOut;
            }
            if (current < toDate)
                periods.Add((current, toDate, true));
            return periods;
        }

        /// <inheritdoc />
        public Task<bool> BlockUnitPeriodAsync(Guid unitId, DateTime fromDate, DateTime toDate, string reason, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<bool> UnblockUnitPeriodAsync(Guid unitId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public async Task<double> CalculateOccupancyRateAsync(Guid unitId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            var totalDays = (toDate - fromDate).TotalDays;
            if (totalDays <= 0) return 0;
            var bookings = await _dbContext.Bookings
                .Where(b => b.UnitId == unitId && b.CheckOut >= fromDate && b.CheckIn <= toDate)
                .ToListAsync(cancellationToken);
            var busyDays = bookings.Sum(b =>
            {
                var start = b.CheckIn < fromDate ? fromDate : b.CheckIn;
                var end = b.CheckOut > toDate ? toDate : b.CheckOut;
                return (end - start).TotalDays;
            });
            return Math.Round(busyDays / totalDays * 100, 2);
        }

        /// <inheritdoc />
        public async Task<double> CalculatePropertyOccupancyRateAsync(Guid propertyId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            var unitIds = await _dbContext.Units.Where(u => u.PropertyId == propertyId).Select(u => u.Id).ToListAsync(cancellationToken);
            if (!unitIds.Any()) return 0;
            var rates = new List<double>();
            foreach (var unitId in unitIds)
                rates.Add(await CalculateOccupancyRateAsync(unitId, fromDate, toDate, cancellationToken));
            return Math.Round(rates.Average(), 2);
        }
    }
}

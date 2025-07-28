using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;

namespace YemenBooking.Infrastructure;

/// <summary>
/// مجموعة امتدادات لتوفير طرق مطلوبة لشيفرة MobileApp مع إعادة استخدام التنفيذ الحالى للمستودعات العامة.
/// </summary>
public static class RepositoryExtensions
{
    #region PaymentRepository
    public static Task<Payment> CreateAsync(this IPaymentRepository repo, Payment payment, CancellationToken ct = default)
        => repo.AddAsync(payment, ct);

    public static async Task<IEnumerable<Payment>> GetByBookingIdAsync(this IPaymentRepository repo, Guid bookingId, CancellationToken ct = default)
        => await repo.FindAsync(p => p.BookingId == bookingId, ct);

    public static async Task<Payment?> GetSuccessfulPaymentByBookingIdAsync(this IPaymentRepository repo, Guid bookingId, CancellationToken ct = default)
        => (await repo.FindAsync(p => p.BookingId == bookingId && p.Status == Core.Enums.PaymentStatus.Successful, ct)).FirstOrDefault();
    #endregion

    #region BookingRepository
    public static async Task<(IEnumerable<Booking> Bookings,int TotalCount)> GetUserBookingsAsync(this IBookingRepository repo, Guid userId,int page,int pageSize,CancellationToken ct=default)
    {
        var (items,total) = await repo.GetPagedAsync(page,pageSize,b=>b.UserId==userId,ct:ct);
        return (items,total);
    }

    public static async Task<IEnumerable<Booking>> GetByUserIdAsync(this IBookingRepository repo, Guid userId,CancellationToken ct=default)
        => await repo.FindAsync(b=>b.UserId==userId,ct);

    public static async Task<IEnumerable<Booking>> GetUserBookingsByDateRangeAsync(this IBookingRepository repo, Guid userId,DateTime from,DateTime to,CancellationToken ct=default)
        => await repo.FindAsync(b=>b.UserId==userId && b.BookedAt>=from && b.BookedAt<=to,ct);

    public static async Task<IEnumerable<Booking>> GetActiveBookingsByUserIdAsync(this IBookingRepository repo, Guid userId,CancellationToken ct=default)
        => await repo.FindAsync(b=>b.UserId==userId && b.Status==Core.Enums.BookingStatus.Confirmed,ct);
    #endregion

    #region PropertyRepository
    public static async Task<decimal> GetAverageRatingAsync(this IPropertyRepository repo, Guid propertyId, CancellationToken ct=default)
        => (await repo.FindAsync(p=>p.Id==propertyId,ct)).FirstOrDefault()?.AverageRating ?? 0m;

    public static async Task<decimal> GetMinPriceAsync(this IPropertyRepository repo, Guid propertyId, CancellationToken ct=default)
        => 0m; // Placeholder – requires business logic.

    public static async Task<int> GetCountByTypeAsync(this IPropertyRepository repo, Guid propertyTypeId, CancellationToken ct=default)
        => await repo.CountAsync(p=>p.TypeId==propertyTypeId,ct);
    #endregion

    #region PropertyTypeRepository
    public static async Task<IEnumerable<PropertyType>> GetByIdsAsync(this IPropertyTypeRepository repo, IEnumerable<Guid> ids, CancellationToken ct=default)
        => await repo.FindAsync(pt=>ids.Contains(pt.Id),ct);

    public static async Task<IEnumerable<PropertyType>> GetAllActiveAsync(this IPropertyTypeRepository repo,CancellationToken ct=default)
        => await repo.GetAllAsync(ct); // No IsActive flag on PT; return all.
    #endregion

    #region AmenityRepository
    public static async Task<IEnumerable<Amenity>> GetAllActiveAsync(this IAmenityRepository repo,CancellationToken ct=default)
        => await repo.GetAllAsync(ct);

    public static async Task<IEnumerable<Amenity>> GetByCategoryAsync(this IAmenityRepository repo,string category,CancellationToken ct=default)
        => await repo.FindAsync(a=>a.Category==category,ct);

    public static async Task<IEnumerable<Amenity>> GetMainAmenitiesByPropertyIdAsync(this IAmenityRepository repo, Guid propertyId,CancellationToken ct=default)
        => await repo.GetAllAsync(ct); // Placeholder: returns all amenities until specific query implemented.
    #endregion

}
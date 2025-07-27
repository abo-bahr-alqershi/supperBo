using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// تنفيذ خدمة التسعير
    /// Pricing service implementation
    /// </summary>
    public class PricingService : IPricingService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IBookingRepository _bookingRepository;

        public PricingService(IUnitRepository unitRepository, IBookingRepository bookingRepository)
        {
            _unitRepository = unitRepository;
            _bookingRepository = bookingRepository;
        }

        /// <summary>
        /// حساب السعر وفقاً لطريقة التسعير وتاريخي الوصول والمغادرة
        /// Calculate price based on pricing method and check-in/check-out dates
        /// </summary>
        public async Task<decimal> CalculatePriceAsync(Guid unitId, DateTime checkIn, DateTime checkOut, int guestCount, CancellationToken cancellationToken = default)
        {
            var unit = await _unitRepository.GetUnitByIdAsync(unitId, cancellationToken);
            if (unit == null)
                throw new ArgumentException($"Unit with ID '{unitId}' not found.");

            var baseAmount = unit.BasePrice.Amount;
            var method = unit.PricingMethod;
            decimal price = method switch
            {
                PricingMethod.Hourly => baseAmount * (decimal)Math.Ceiling((checkOut - checkIn).TotalHours),
                PricingMethod.Daily => baseAmount * (decimal)Math.Ceiling((checkOut.Date - checkIn.Date).TotalDays),
                PricingMethod.Weekly => baseAmount * (decimal)Math.Ceiling((checkOut.Date - checkIn.Date).TotalDays / 7.0),
                PricingMethod.Monthly => baseAmount * (((checkOut.Year - checkIn.Year) * 12) + (checkOut.Month - checkIn.Month) + (checkOut.Day > checkIn.Day ? 1 : 0)),
                _ => throw new NotSupportedException($"Pricing method '{method}' is not supported.")
            };

            return price;
        }

        public async Task<decimal> RecalculatePriceAsync(Guid bookingId, DateTime? newCheckIn = null, DateTime? newCheckOut = null, int? newGuestCount = null, CancellationToken cancellationToken = default)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId, cancellationToken);
            if (booking == null)
                throw new ArgumentException($"Booking with ID '{bookingId}' not found.");
            return booking.TotalPrice.Amount;
        }

        public async Task<decimal> CalculateBasePriceAsync(Guid unitId, int nights, CancellationToken cancellationToken = default)
        {
            var unit = await _unitRepository.GetUnitByIdAsync(unitId, cancellationToken);
            if (unit == null)
                throw new ArgumentException($"Unit with ID '{unitId}' not found.");
            return unit.BasePrice.Amount * nights;
        }

        public Task<decimal> CalculateAdditionalFeesAsync(Guid unitId, int guestCount, IEnumerable<Guid>? serviceIds = null, CancellationToken cancellationToken = default)
        {
            // TODO: إضافة رسوم إضافية بناءً على الخدمات
            return Task.FromResult(0m);
        }

        public Task<decimal> CalculateDiscountsAsync(Guid unitId, DateTime checkIn, DateTime checkOut, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            // TODO: حساب الخصومات مثل العروض أو الخصومات الموسمية
            return Task.FromResult(0m);
        }

        public Task<decimal> CalculateTaxesAsync(decimal baseAmount, Guid propertyId, CancellationToken cancellationToken = default)
        {
            // فرض معدل ضريبة ثابت 5%
            var tax = Math.Round(baseAmount * 0.05m, 2);
            return Task.FromResult(tax);
        }

        public async Task<object> GetPricingBreakdownAsync(Guid unitId, DateTime checkIn, DateTime checkOut, int guestCount, CancellationToken cancellationToken = default)
        {
            var nights = (checkOut.Date - checkIn.Date).Days;
            var basePrice = await CalculateBasePriceAsync(unitId, nights, cancellationToken);
            var fees = await CalculateAdditionalFeesAsync(unitId, guestCount, null, cancellationToken);
            var discounts = await CalculateDiscountsAsync(unitId, checkIn, checkOut, null, cancellationToken);
            var taxes = await CalculateTaxesAsync(basePrice, Guid.Empty, cancellationToken);
            var total = basePrice + fees + taxes - discounts;
            return new
            {
                BasePrice = basePrice,
                AdditionalFees = fees,
                Discounts = discounts,
                Taxes = taxes,
                TotalPrice = total
            };
        }
    }
} 
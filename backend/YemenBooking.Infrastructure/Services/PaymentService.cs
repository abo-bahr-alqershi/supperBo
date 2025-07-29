using System;
using System.Threading.Tasks;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// خدمة المدفوعات
    /// Implements IPaymentService by delegating to IPaymentGatewayService
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentGatewayService _gatewayService;

        public PaymentService(IPaymentGatewayService gatewayService)
        {
            _gatewayService = gatewayService;
        }

        public Task<PaymentResult> ProcessPaymentAsync(Guid bookingId, Guid paymentMethodId, decimal amount, string currency)
        {
            return _gatewayService.ProcessPaymentAsync(bookingId, paymentMethodId, amount, currency);
        }

        public Task<bool> ValidatePaymentDataAsync(Guid paymentMethodId, decimal amount, string currency)
        {
            return _gatewayService.ValidatePaymentDataAsync(paymentMethodId, amount, currency);
        }

        public Task<RefundResult> RefundPaymentAsync(Guid paymentId, decimal refundAmount, string reason)
        {
            return _gatewayService.RefundPaymentAsync(paymentId, refundAmount, reason);
        }

        public Task<PaymentStatus> GetPaymentStatusAsync(Guid paymentId)
        {
            return _gatewayService.GetPaymentStatusAsync(paymentId);
        }

        public Task<decimal> CalculateFeesAsync(decimal amount, PaymentMethodType paymentMethodType, string currency)
        {
            return _gatewayService.CalculateFeesAsync(amount, paymentMethodType, currency);
        }

        public Task<string> CreatePaymentLinkAsync(Guid bookingId, decimal amount, string currency, string returnUrl)
        {
            return _gatewayService.CreatePaymentLinkAsync(bookingId, amount, currency, returnUrl);
        }
    }
}
import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type {
  ProcessPaymentCommand,
  RefundPaymentCommand,
  VoidPaymentCommand,
  UpdatePaymentStatusCommand,
  GetPaymentByIdQuery,
  GetPaymentsByBookingQuery,
  GetPaymentsByStatusQuery,
  GetPaymentsByUserQuery,
  GetPaymentsByMethodQuery,
  PaymentDto,
} from '../types/payment.types';

// خدمات الدفع لأصحاب الكيانات (Property Payments Service)
export const PropertyPaymentsService = {
  /** معالجة الدفع */
  process: (data: ProcessPaymentCommand) =>
    apiClient.post<ResultDto<string>>('/api/property/Payments/process', data).then(res => res.data),

  /** استرجاع الدفع */
  refund: (data: RefundPaymentCommand) =>
    apiClient.post<ResultDto<boolean>>('/api/property/Payments/refund', data).then(res => res.data),

  /** إبطال الدفع */
  voidPayment: (data: VoidPaymentCommand) =>
    apiClient.post<ResultDto<boolean>>('/api/property/Payments/void', data).then(res => res.data),

  /** تحديث حالة الدفع */
  updateStatus: (paymentId: string, data: UpdatePaymentStatusCommand) =>
    apiClient.put<ResultDto<boolean>>(`/api/property/Payments/${paymentId}/status`, data).then(res => res.data),

  /** جلب دفعة بواسطة المعرف */
  getById: (query: GetPaymentByIdQuery) =>
    apiClient.get<ResultDto<PaymentDto>>(`/api/property/Payments/${query.paymentId}`).then(res => res.data),

  /** جلب المدفوعات حسب الحجز */
  getByBooking: (query: GetPaymentsByBookingQuery) =>
    apiClient.get<PaginatedResult<PaymentDto>>(`/api/property/Payments/booking/${query.bookingId}`, { params: query }).then(res => res.data),

  /** جلب المدفوعات حسب الحالة */
  getByStatus: (query: GetPaymentsByStatusQuery) =>
    apiClient.get<PaginatedResult<PaymentDto>>('/api/property/Payments/status', { params: query }).then(res => res.data),

  /** جلب المدفوعات حسب المستخدم */
  getByUser: (query: GetPaymentsByUserQuery) =>
    apiClient.get<PaginatedResult<PaymentDto>>(`/api/property/Payments/user/${query.userId}`, { params: query }).then(res => res.data),

  /** جلب المدفوعات حسب طريقة الدفع */
  getByMethod: (query: GetPaymentsByMethodQuery) =>
    apiClient.get<PaginatedResult<PaymentDto>>(`/api/property/Payments/method/${query.paymentMethod}`, { params: query }).then(res => res.data),
}; 
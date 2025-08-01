import { apiClient } from './api.service';
import type { PaymentDto, RefundPaymentCommand, VoidPaymentCommand, UpdatePaymentStatusCommand, GetPaymentsByBookingQuery, GetPaymentsByMethodQuery, GetPaymentsByStatusQuery, GetPaymentsByUserQuery, GetAllPaymentsQuery } from '../types/payment.types';
import type { ResultDto, PaginatedResult } from '../types/common.types';

// خدمات المدفوعات (Payments Service) للإدارة
export const AdminPaymentsService = {
  /** استرجاع دفعة */
  refund: (data: RefundPaymentCommand) =>
    apiClient.post<ResultDto<boolean>>('/api/admin/Payments/refund', data).then(res => res.data),

  /** إبطال دفعة */
  void: (data: VoidPaymentCommand) =>
    apiClient.post<ResultDto<boolean>>('/api/admin/Payments/void', data).then(res => res.data),

  /** تحديث حالة الدفعة */
  updateStatus: (paymentId: string, data: UpdatePaymentStatusCommand) =>
    apiClient.put<ResultDto<boolean>>(`/api/admin/Payments/${paymentId}/status`, data).then(res => res.data),

  /** جلب دفعة حسب المعرف */
  getById: (paymentId: string) =>
    apiClient.get<ResultDto<PaymentDto>>(`/api/admin/Payments/${paymentId}`).then(res => res.data),
  /** جلب دفعات حسب الحجز */
  getByBooking: (query: GetPaymentsByBookingQuery) =>
    apiClient.get<PaginatedResult<PaymentDto>>(`/api/admin/Payments/booking/${query.bookingId}`, { params: { pageNumber: query.pageNumber, pageSize: query.pageSize } }).then(res => res.data),

  /** جلب دفعات حسب الطريقة */
  getByMethod: (query: GetPaymentsByMethodQuery) =>
    apiClient.get<PaginatedResult<PaymentDto>>(`/api/admin/Payments/method/${query.paymentMethod}`, { params: { pageNumber: query.pageNumber, pageSize: query.pageSize } }).then(res => res.data),

  /** جلب جميع المدفوعات مع دعم الفلاتر */
  getAll: (query: GetAllPaymentsQuery) =>
    apiClient.get<PaginatedResult<PaymentDto>>('/api/admin/Payments', { params: query }).then(res => res.data),

  /** جلب دفعات حسب الحالة */
  getByStatus: (query?: GetPaymentsByStatusQuery) =>
    apiClient.get<PaginatedResult<PaymentDto>>('/api/admin/Payments/status', { params: query }).then(res => res.data),

  /** جلب دفعات حسب المستخدم */
  getByUser: (query: GetPaymentsByUserQuery) =>
    apiClient.get<PaginatedResult<PaymentDto>>(`/api/admin/Payments/user/${query.userId}`, { params: { pageNumber: query.pageNumber, pageSize: query.pageSize } }).then(res => res.data),
};

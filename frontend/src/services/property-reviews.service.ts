import { apiClient } from './api.service';
import type { ResultDto } from '../types/common.types';
import type {
  ReviewDto,
  ApproveReviewCommand,
  DeleteReviewCommand,
  RespondToReviewCommand,
  GetReviewByBookingQuery,
  GetReviewsByPropertyQuery,
  GetReviewsByUserQuery,
} from '../types/review.types';

// المسار الأساسي لتعاملات التقييمات لأصحاب الكيانات
const API_BASE = '/api/property/reviews';

export const PropertyReviewsService = {
  // الموافقة على تقييم
  approve: (reviewId: string, data: ApproveReviewCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/${reviewId}/approve`, data).then(res => res.data),

  // حذف تقييم
  delete: (reviewId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${reviewId}`).then(res => res.data),

  // الرد على تقييم
  respond: (reviewId: string, data: RespondToReviewCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/${reviewId}/respond`, data).then(res => res.data),

  // جلب تقييم حسب الحجز
  getByBooking: (query: GetReviewByBookingQuery) =>
    apiClient.get<ResultDto<ReviewDto>>(`${API_BASE}/booking/${query.bookingId}`).then(res => res.data),

  // جلب تقييمات كيان معين
  getByProperty: (query: GetReviewsByPropertyQuery) =>
    apiClient.get<ResultDto<ReviewDto[]>>(`${API_BASE}/property/${query.propertyId}`, { params: query }).then(res => res.data),

  // جلب تقييمات مستخدم
  getByUser: (query: GetReviewsByUserQuery) =>
    apiClient.get<ResultDto<ReviewDto[]>>(`${API_BASE}/user/${query.userId}`, { params: query }).then(res => res.data),
};
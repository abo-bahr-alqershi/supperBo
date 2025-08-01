// خدمات التقييمات (Reviews Service)
// جميع الدوال موثقة بالعربي وتدعم العمليات الأساسية
import type {
  ReviewDto,
  ReviewImageDto,
  CreateReviewCommand,
  GetReviewByBookingQuery,
  GetReviewsByPropertyQuery,
  GetReviewsByUserQuery,
  GetPendingReviewsQuery,
  GetAllReviewsQuery
} from '../types/review.types';
import { apiClient } from './api.service';
import type { ResultDto } from '../types/common.types';

/**
 * دوال التعامل مع التقييمات عبر API
 */
export class AdminReviewsService {
  /** جلب تقييم حسب الحجز */
  static async getByBooking(bookingId: string): Promise<ReviewDto> {
    const response = await apiClient.get<ResultDto<ReviewDto>>(`/api/admin/reviews/booking/${bookingId}`);
    return response.data.data!;
  }

  /** جلب تقييمات كيان مع التصفية والصفحات */
  static async getByProperty(query: GetReviewsByPropertyQuery): Promise<ReviewDto[]> {
    const response = await apiClient.get<ResultDto<ReviewDto[]>>(
      `/api/admin/reviews/property/${query.propertyId}`,
      { params: query }
    );
    return response.data.data || [];
  }

  /** جلب تقييمات مستخدم مع التصفية والصفحات */
  static async getByUser(query: GetReviewsByUserQuery): Promise<ReviewDto[]> {
    const response = await apiClient.get<ResultDto<ReviewDto[]>>(
      `/api/admin/reviews/user/${query.userId}`,
      { params: query }
    );
    return response.data.data || [];
  }

  /** جلب التقييمات المعلقة للموافقة */
  static async getPending(query?: GetPendingReviewsQuery): Promise<ReviewDto[]> {
    const response = await apiClient.get<ResultDto<ReviewDto[]>>( '/api/admin/reviews/pending', {
      params: query,
    });
    return response.data.data || [];
  }

  /** جلب جميع التقييمات مع دعم التصفية */
  static async getAll(query: GetAllReviewsQuery): Promise<ReviewDto[]> {
    const response = await apiClient.get<ResultDto<ReviewDto[]>>('/api/admin/reviews', {
      params: query,
    });
    return response.data.data || [];
  }

  /** الموافقة على تقييم */
  static async approve(reviewId: string): Promise<ResultDto<boolean>> {
    return apiClient.post<ResultDto<boolean>>(`/api/admin/reviews/${reviewId}/approve`).then(res => res.data);
  }

  /** حذف تقييم */
  static async deleteReview(reviewId: string): Promise<ResultDto<boolean>> {
    return apiClient.delete<ResultDto<boolean>>(`/api/admin/reviews/${reviewId}`).then(res => res.data);
  }
}

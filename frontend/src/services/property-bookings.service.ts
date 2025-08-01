import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type { ServiceDto } from '../types/service.types';
import type {
  BookingDto,
  BookingDetailsDto,
  CreateBookingCommand,
  CancelBookingCommand,
  CheckInCommand,
  CheckOutCommand,
  CompleteBookingCommand,
  ConfirmBookingCommand,
  AddServiceToBookingCommand,
  RemoveServiceFromBookingCommand,
  UpdateBookingCommand,
  GetBookingsByPropertyQuery,
  GetBookingByIdQuery,
  GetBookingsByStatusQuery,
  GetBookingsByUnitQuery,
  GetBookingsByUserQuery,
  GetBookingServicesQuery,
  GetBookingsByDateRangeQuery,
  GetBookingReportQuery,
  GetBookingTrendsQuery,
  GetBookingWindowAnalysisQuery,
  BookingReportDto,
  TimeSeriesDataDto,
  BookingWindowDto,
} from '../types/booking.types';

// المسار الأساسي لخدمة حجوزات المالك
const API_BASE = '/api/property/bookings';

// خدمات الحجوزات لأصحاب الكيانات (Property Bookings Service)
export const PropertyBookingsService = {
  /** إنشاء حجز جديد */
  create: (data: CreateBookingCommand) =>
    apiClient.post<ResultDto<string>>(`${API_BASE}/create`, data).then(res => res.data),

  /** إلغاء حجز */
  cancel: (data: CancelBookingCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/cancel`, data).then(res => res.data),

  /** تسجيل الوصول للحجز */
  checkIn: (data: CheckInCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/check-in`, data).then(res => res.data),

  /** تسجيل مغادرة الحجز */
  checkOut: (data: CheckOutCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/check-out`, data).then(res => res.data),

  /** إكمال الحجز */
  complete: (data: CompleteBookingCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/complete`, data).then(res => res.data),

  /** تأكيد الحجز */
  confirm: (data: ConfirmBookingCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/confirm`, data).then(res => res.data),

  /** إضافة خدمة للحجز */
  addService: (data: AddServiceToBookingCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/add-service-to-booking`, data).then(res => res.data),

  /** إزالة خدمة من الحجز */
  removeService: (data: RemoveServiceFromBookingCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/remove-service-from-booking`, data).then(res => res.data),

  /** تحديث بيانات الحجز */
  update: (bookingId: string, data: UpdateBookingCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${bookingId}/update`, data).then(res => res.data),

  /** جلب حجوزات الكيان مع الفلاتر والصفحات */
  getByProperty: (query: GetBookingsByPropertyQuery) =>
    apiClient.get<PaginatedResult<BookingDto>>(`${API_BASE}`, { params: query }).then(res => res.data),

  /** جلب تفاصيل حجز بواسطة المعرف */
  getById: (query: GetBookingByIdQuery) =>
    apiClient.get<ResultDto<BookingDetailsDto>>(`${API_BASE}/${query.bookingId}`).then(res => res.data),

  /** جلب الحجوزات حسب الحالة */
  getByStatus: (query: GetBookingsByStatusQuery) =>
    apiClient.get<PaginatedResult<BookingDto>>(`${API_BASE}/status`, { params: query }).then(res => res.data),

  /** جلب الحجوزات حسب الوحدة */
  getByUnit: (query: GetBookingsByUnitQuery) =>
    apiClient.get<PaginatedResult<BookingDto>>(`${API_BASE}/unit/${query.unitId}`, { params: query }).then(res => res.data),

  /** جلب الحجوزات حسب المستخدم */
  getByUser: (query: GetBookingsByUserQuery) =>
    apiClient.get<PaginatedResult<BookingDto>>(`${API_BASE}/user/${query.userId}`, { params: query }).then(res => res.data),

  /** جلب خدمات الحجز */
  getServices: (query: GetBookingServicesQuery) =>
    apiClient.get<ResultDto<ServiceDto[]>>(`${API_BASE}/${query.bookingId}/services`).then(res => res.data),

  /** تقرير الحجوزات */
  getReport: (query: GetBookingReportQuery) =>
    apiClient.get<ResultDto<BookingReportDto>>(`${API_BASE}/report`, { params: query }).then(res => res.data),

  /** اتجاهات الحجوزات كسلسلة زمنية */
  getTrends: (query: GetBookingTrendsQuery) =>
    apiClient.get<TimeSeriesDataDto[]>(`${API_BASE}/trends`, { params: query }).then(res => res.data),

  /** تحليل نافذة الحجوزات */
  getWindowAnalysis: (query: GetBookingWindowAnalysisQuery) =>
    apiClient.get<BookingWindowDto>(`${API_BASE}/window-analysis/${query.propertyId}`).then(res => res.data),

  /** جلب الحجوزات في نطاق زمني */
  getByDateRange: (query: GetBookingsByDateRangeQuery) =>
    apiClient.get<PaginatedResult<BookingDto>>(`${API_BASE}/by-date-range`, { params: query }).then(res => res.data),
}; 
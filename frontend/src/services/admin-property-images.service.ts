import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type {
  PropertyImageDto,
  CreatePropertyImageCommand,
  UpdatePropertyImageCommand,
  DeletePropertyImageCommand,
  AssignPropertyImageToPropertyCommand,
  AssignPropertyImageToUnitCommand,
  GetPropertyImagesQuery,
  PropertyImageStatsDto,
  BulkAssignImageToPropertyCommand,
  BulkAssignImageToUnitCommand,
  ReorderPropertyImagesCommand,
} from '../types/property-image.types';

const API_BASE = '/api/admin/propertyimages';

// خدمات إدارة صور الكيانات للمدراء (Admin Property Images Service)
export const AdminPropertyImagesService = {
  /** إنشاء صورة كيان جديدة */
  create: (data: CreatePropertyImageCommand) =>
    apiClient.post<ResultDto<string>>(`${API_BASE}`, data).then(res => res.data),

  /** تحديث بيانات صورة كيان */
  update: (imageId: string, data: UpdatePropertyImageCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${imageId}`, data).then(res => res.data),

  /** حذف صورة كيان */
  delete: (imageId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${imageId}`).then(res => res.data),

  /** إسناد الصورة لكيان */
  assignToProperty: (imageId: string, propertyId: string, data: AssignPropertyImageToPropertyCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/${imageId}/assign/property/${propertyId}`, data).then(res => res.data),

  /** إسناد الصورة للوحدة */
  assignToUnit: (imageId: string, unitId: string, data: AssignPropertyImageToUnitCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/${imageId}/assign/unit/${unitId}`, data).then(res => res.data),

  /**
   * تعيين صور متعددة لكيانات
   */
  bulkAssignToProperties: (data: BulkAssignImageToPropertyCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/bulk-assign/property`, data).then(res => res.data),

  /**
   * تعيين صور متعددة لوحدات
   */
  bulkAssignToUnits: (data: BulkAssignImageToUnitCommand) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/bulk-assign/unit`, data).then(res => res.data),

  /** جلب جميع صور الكيان */
  getAll: (query: GetPropertyImagesQuery) =>
    apiClient.get<ResultDto<PaginatedResult<PropertyImageDto>>>(`${API_BASE}`, { params: query }).then(res => res.data),

  /** جلب إحصائيات صور كيان */
  getStats: (propertyId: string) =>
    apiClient.get<ResultDto<PropertyImageStatsDto>>(`${API_BASE}/${propertyId}/stats`).then(res => res.data),

  /** إعادة ترتيب صور الكيان */
  reorder: (data: ReorderPropertyImagesCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/order`, data).then(res => res.data),
}; 
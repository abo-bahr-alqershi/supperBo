import { apiClient } from './api.service';
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
import type { ResultDto, PaginatedResult } from '../types/common.types';

// خدمات صور الكيانات والوحدات لأصحاب الكيانات
export const PropertyImagesService = {
  /** إنشاء صورة جديدة */
  create: (data: CreatePropertyImageCommand) =>
    apiClient.post<ResultDto<string>>('/api/property/PropertyImages', data).then(res => res.data),

  /** تحديث بيانات صورة */
  update: (imageId: string, data: UpdatePropertyImageCommand) =>
    apiClient.put<ResultDto<boolean>>(`/api/property/PropertyImages/${imageId}`, data).then(res => res.data),

  /** حذف صورة */
  delete: (imageId: string) =>
    apiClient.delete<ResultDto<boolean>>(`/api/property/PropertyImages/${imageId}`).then(res => res.data),

  /** تعيين صورة لكيان */
  assignToProperty: (imageId: string, propertyId: string, data: AssignPropertyImageToPropertyCommand) =>
    apiClient.post<ResultDto<boolean>>(`/api/property/PropertyImages/${imageId}/assign/property/${propertyId}`, data).then(res => res.data),

  /** تعيين صورة لوحدة */
  assignToUnit: (imageId: string, unitId: string, data: AssignPropertyImageToUnitCommand) =>
    apiClient.post<ResultDto<boolean>>(`/api/property/PropertyImages/${imageId}/assign/unit/${unitId}`, data).then(res => res.data),

  /**
   * تعيين صور متعددة لكيانات
   */
  bulkAssignToProperties: (data: BulkAssignImageToPropertyCommand) =>
    apiClient.post<ResultDto<boolean>>(`/api/property/PropertyImages/bulk-assign/property`, data).then(res => res.data),

  /**
   * تعيين صور متعددة لوحدات
   */
  bulkAssignToUnits: (data: BulkAssignImageToUnitCommand) =>
    apiClient.post<ResultDto<boolean>>(`/api/property/PropertyImages/bulk-assign/unit`, data).then(res => res.data),

  /** جلب جميع الصور حسب استعلام */
  getAll: (query: GetPropertyImagesQuery) =>
    apiClient.get<ResultDto<PaginatedResult<PropertyImageDto>>>(`/api/property/PropertyImages`, { params: query }).then(res => res.data),

  /** جلب إحصائيات صور كيان */
  getStats: (propertyId: string) =>
    apiClient.get<ResultDto<PropertyImageStatsDto>>(`/api/property/PropertyImages/${propertyId}/stats`).then(res => res.data),

  /** إعادة ترتيب صور الكيان */
  reorder: (data: ReorderPropertyImagesCommand) =>
    apiClient.put<ResultDto<boolean>>(`/api/property/PropertyImages/order`, data).then(res => res.data),
}; 
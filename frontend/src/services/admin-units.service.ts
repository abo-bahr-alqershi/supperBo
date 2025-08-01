import { apiClient } from './api.service';
import type {
  UnitDto,
  UnitDetailsDto,
  CreateUnitCommand,
  UpdateUnitCommand,
  DeleteUnitCommand,
  GetUnitByIdQuery,
} from '../types/unit.types';
import type { ResultDto, PaginatedResult } from '../types/common.types';

// المسار الأساسي لتعاملات الوحدات للمدراء
const API_BASE = '/api/admin/Units';

/**
 * جميع خدمات الوحدات للإدارة
 * موثقة بالعربي لضمان الوضوح والتوافق مع الباك اند
 */
export const AdminUnitsService = {
  /** جلب جميع الوحدات مع صفحات وفلاتر */
  getAll: (params?: Record<string, any>) =>
    apiClient.get<PaginatedResult<UnitDto>>(API_BASE, { params }).then(res => res.data),

  // جلب تفاصيل وحدة بواسطة المعرف
  getById: (query: GetUnitByIdQuery) =>
    apiClient.get<ResultDto<UnitDetailsDto>>(`${API_BASE}/${query.unitId}`).then(res => res.data),

  // إنشاء وحدة جديدة
  create: (data: CreateUnitCommand) =>
    apiClient.post<ResultDto<string>>(API_BASE, data).then(res => res.data),

  // تحديث بيانات وحدة
  update: (unitId: string, data: UpdateUnitCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${unitId}`, data).then(res => res.data),

  // حذف وحدة
  delete: (unitId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${unitId}`).then(res => res.data),
  /** جلب الوحدات حسب الكيان مع صفحات وفلاتر */
  getByProperty: (params: { propertyId: string; isAvailable?: boolean; minBasePrice?: number; maxBasePrice?: number; minCapacity?: number; nameContains?: string; pageNumber?: number; pageSize?: number }) =>
    apiClient.get<PaginatedResult<UnitDto>>(`${API_BASE}/property/${params.propertyId}`, { params }).then(res => res.data),
  /** تحديث متعدد لتوفر الوحدات ضمن نطاق زمني */
  bulkUpdateAvailability: (data: { unitIds: string[]; startDate: string; endDate: string; isAvailable: boolean }) =>
    apiClient.post<ResultDto<boolean>>(`${API_BASE}/bulk-availability`, data).then(res => res.data),
  /** جلب الوحدات حسب نوع الوحدة مع إمكانية تضمين القيم الديناميكية */
  getByType: (query: { unitTypeId: string; includeDynamicFields?: boolean; isAvailable?: boolean; minBasePrice?: number; maxBasePrice?: number; minCapacity?: number; nameContains?: string; pageNumber?: number; pageSize?: number }) => {
    const { unitTypeId, includeDynamicFields, isAvailable, minBasePrice, maxBasePrice, minCapacity, nameContains, pageNumber, pageSize } = query;
    return apiClient
      .get<PaginatedResult<UnitDto>>(
        `${API_BASE}/type/${unitTypeId}`,
        { params: { includeDynamicFields, isAvailable, minBasePrice, maxBasePrice, minCapacity, nameContains, pageNumber, pageSize } }
      )
      .then(res => res.data);
  },
  /** جلب توفر وحدة */
  getAvailability: (unitId: string, query?: { startDate?: string; endDate?: string }) =>
    apiClient.get<ResultDto<any>>(`${API_BASE}/${unitId}/availability`, { params: query }).then(res => res.data),
  /** جلب بيانات الوحدة للتحرير */
  getForEdit: (unitId: string, ownerId: string) =>
    apiClient.get<ResultDto<any>>(`${API_BASE}/${unitId}/for-edit`, { params: { ownerId } }).then(res => res.data),
  /** جلب تفاصيل الوحدة مع الحقول الديناميكية */
  getDetails: (unitId: string, includeDynamicFields = true) =>
    apiClient.get<ResultDto<UnitDetailsDto>>(`${API_BASE}/${unitId}/details`, { params: { includeDynamicFields } }).then(res => res.data),
  /** جلب صور الوحدة */
  getImages: (unitId: string) =>
    apiClient.get<any>(`${API_BASE}/${unitId}/images`).then(res => res.data),
};

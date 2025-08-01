import { apiClient } from './api.service';
import type {
  SearchFilterDto,
  CreateSearchFilterCommand,
  UpdateSearchFilterCommand,
  DeleteSearchFilterCommand,
  ToggleSearchFilterStatusCommand,
  GetSearchFiltersQuery,
  GetSearchFilterByIdQuery,
  GetSearchableFieldsQuery,
} from '../types/search-filter.types';
import type { UnitTypeFieldDto } from '../types/unit-type-field.types';
import type { ResultDto } from '../types/common.types';

// المسار الأساسي لتعاملات فلاتر البحث للمدراء
const API_BASE = '/api/admin/searchfilters';

export const AdminSearchFiltersService = {
  // إنشاء فلتر بحث جديد
  create: (data: CreateSearchFilterCommand) =>
    apiClient.post<ResultDto<string>>(API_BASE, data).then(res => res.data),

  // تحديث فلتر بحث
  update: (filterId: string, data: UpdateSearchFilterCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${filterId}`, data).then(res => res.data),

  // حذف فلتر بحث
  delete: (filterId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${filterId}`).then(res => res.data),

  // تبديل حالة تفعيل فلتر بحث
  toggleStatus: (filterId: string, data: ToggleSearchFilterStatusCommand) =>
    apiClient.patch<ResultDto<boolean>>(`${API_BASE}/${filterId}/toggle-status`, data).then(res => res.data),

  // جلب جميع فلاتر البحث لنوع كيان معين
  getAll: (params?: GetSearchFiltersQuery) =>
    apiClient.get<SearchFilterDto[]>(`${API_BASE}`, { params }).then(res => res.data),

  // جلب فلتر بحث بواسطة المعرف
  getById: (filterId: string) =>
    apiClient.get<ResultDto<SearchFilterDto>>(`${API_BASE}/${filterId}`).then(res => res.data),

  // جلب الحقول القابلة للبحث لنوع كيان معين
  getSearchableFields: (params?: GetSearchableFieldsQuery) =>
    apiClient.get<UnitTypeFieldDto[]>(`${API_BASE}/searchable-fields`, { params }).then(res => res.data),
};
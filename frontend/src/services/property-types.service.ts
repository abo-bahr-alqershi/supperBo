import { apiClient } from './api.service';
import type { PaginatedResult } from '../types/common.types';
import type { PropertyTypeDto } from '../types/property-type.types';

// خدمة أنواع الكيانات للعميل (تسجيل مالك العقار)
const API_BASE = '/api/property/propertytypes';

export const PropertyTypesService = {
  /** جلب قائمة أنواع الكيانات */
  getAll: (params?: { pageNumber?: number; pageSize?: number }) =>
    apiClient
      .get<PaginatedResult<PropertyTypeDto>>(API_BASE, { params })
      .then(res => res.data),
  /** جلب نوع كيان بحسب المعرف */
  getById: (propertyTypeId: string) =>
    apiClient
      .get<PropertyTypeDto>(`${API_BASE}/${propertyTypeId}`)
      .then(res => res.data),
}; 
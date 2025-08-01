import { apiClient } from './api.service';
import type { PaginatedResult } from '../types/common.types';
import type { PropertyDto } from '../types/property.types';
import type { SearchPropertiesQuery } from '../types/property-search.types';

// المسار الأساسي لتعاملات الكيانات المشتركة
const API_BASE = '/api/common/properties';

export const CommonPropertiesService = {
  // البحث في الكيانات مع الفلاتر، الموقع، والمسافة
  search: (query: SearchPropertiesQuery) =>
    apiClient.get<PaginatedResult<PropertyDto>>(`${API_BASE}/search`, { params: query }).then(res => res.data),
};
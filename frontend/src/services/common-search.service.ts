import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type { PropertyDto } from '../types/property.types';
import type { UnitDto } from '../types/unit.types';
import type { SearchPropertiesCommand } from '../types/property-search.types';
import type { SearchUnitsQuery } from '../types/unit-search.types';

// المسار الأساسي لتعاملات البحث المشتركة
const API_BASE = '/api/common/search';

export const CommonSearchService = {
  // البحث في الكيانات بناءً على المعايير الديناميكية والفرز والتصفح
  searchProperties: (command: SearchPropertiesCommand) =>
    apiClient.post<PaginatedResult<PropertyDto>>(`${API_BASE}/properties`, command).then(res => res.data),

  // البحث في الوحدات بناءً على المعايير الديناميكية والفرز والتصفح
  searchUnits: (query: SearchUnitsQuery) =>
    apiClient.post<PaginatedResult<UnitDto>>(`${API_BASE}/units`, query).then(res => res.data),
};
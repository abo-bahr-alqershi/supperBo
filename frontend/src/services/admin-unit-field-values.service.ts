import { apiClient } from './api.service';
import type {
  UnitFieldValueDto,
  CreateUnitFieldValueCommand,
  UpdateUnitFieldValueCommand,
  GetUnitFieldValuesQuery,
  BulkUpdateUnitFieldValueCommand,
  GetUnitFieldValuesGroupedQuery,
  FieldGroupWithValuesDto,
} from '../types/unit-field-value.types';
import type { ResultDto } from '../types/common.types';

// خدمات قيم الحقول للوحدات (Unit Field Values Service) للمدراء
export const AdminUnitFieldValuesService = {
  /** إنشاء قيمة حقل لوحدة */
  create: (data: CreateUnitFieldValueCommand) =>
    apiClient.post<ResultDto<string>>(`/api/admin/UnitFieldValues`, data).then(res => res.data),

  /** تحديث قيمة حقل لوحدة */
  update: (valueId: string, data: UpdateUnitFieldValueCommand) =>
    apiClient.put<ResultDto<boolean>>(`/api/admin/UnitFieldValues/${valueId}`, data).then(res => res.data),

  /** حذف قيمة حقل لوحدة */
  delete: (valueId: string) =>
    apiClient.delete<ResultDto<boolean>>(`/api/admin/UnitFieldValues/${valueId}`).then(res => res.data),

  /** تحديث متعدد لقيم حقول الوحدات */
  bulkUpdate: (data: BulkUpdateUnitFieldValueCommand) =>
    apiClient.post<ResultDto<boolean>>(`/api/admin/UnitFieldValues/bulk-update`, data).then(res => res.data),

  /** جلب جميع قيم الحقول لوحدة */
  getAll: (query: GetUnitFieldValuesQuery) =>
    apiClient.get<UnitFieldValueDto[]>(`/api/admin/UnitFieldValues`, { params: query }).then(res => res.data),

  /** جلب قيم الحقول مجمعة حسب المجموعات */
  getGrouped: (query: GetUnitFieldValuesGroupedQuery) =>
    apiClient.get<FieldGroupWithValuesDto[]>(`/api/admin/UnitFieldValues/grouped`, { params: query }).then(res => res.data),

  /** جلب قيمة حقل للوحدة حسب المعرف */
  getById: (valueId: string) =>
    apiClient.get<ResultDto<UnitFieldValueDto>>(`/api/admin/UnitFieldValues/${valueId}`).then(res => res.data),
};

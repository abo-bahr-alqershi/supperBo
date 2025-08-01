import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type {
  UserDto,
  RegisterPropertyOwnerCommand,
  UpdateUserCommand,
  GetUserByIdQuery,
  GetUsersByRoleQuery,
  GetUserByEmailQuery,
  SearchUsersQuery,
  GetUserActivityLogQuery,
  GetUserLifetimeStatsQuery,
  GetUserNotificationsQuery,
} from '../types/user.types';

// المسار الأساسي لتعاملات المستخدمين لأصحاب الكيانات
const API_BASE = '/api/property/users';

export const PropertyUsersService = {
  // تسجيل صاحب كيان جديد
  registerPropertyOwner: (data: RegisterPropertyOwnerCommand) =>
    apiClient.post<ResultDto<string>>(`${API_BASE}/register`, data).then(res => res.data),

  // تحديث بيانات المستخدم
  update: (userId: string, data: UpdateUserCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${userId}`, data).then(res => res.data),

  // جلب بيانات المستخدم بواسطة المعرف
  getById: (query: GetUserByIdQuery) =>
    apiClient.get<ResultDto<UserDto>>(`${API_BASE}/${query.userId}`).then(res => res.data),

  // جلب المستخدمين حسب الدور
  getByRole: (query: GetUsersByRoleQuery) =>
    apiClient.get<PaginatedResult<UserDto>>(`${API_BASE}/by-role`, { params: query }).then(res => res.data),

  // جلب بيانات المستخدم بواسطة البريد الإلكتروني
  getByEmail: (query: GetUserByEmailQuery) =>
    apiClient.get<ResultDto<UserDto>>(`${API_BASE}/by-email`, { params: query }).then(res => res.data),

  // البحث عن المستخدمين
  search: (query: SearchUsersQuery) =>
    apiClient.get<PaginatedResult<UserDto>>(`${API_BASE}/search`, { params: query }).then(res => res.data),

  // جلب سجلات نشاط المستخدم
  getActivityLog: (query: GetUserActivityLogQuery) =>
    apiClient.get<ResultDto<UserDto[]>>(`${API_BASE}/${query.userId}/activity-log`, { params: query }).then(res => res.data),

  // جلب إحصائيات المستخدم مدى الحياة
  getLifetimeStats: (query: GetUserLifetimeStatsQuery) =>
    apiClient.get<ResultDto<any>>(`${API_BASE}/${query.userId}/lifetime-stats`).then(res => res.data),

  // جلب إشعارات المستخدم
  getNotifications: (query: GetUserNotificationsQuery) =>
    apiClient.get<PaginatedResult<any>>(`${API_BASE}/${query.userId}/notifications`, { params: query }).then(res => res.data),
};
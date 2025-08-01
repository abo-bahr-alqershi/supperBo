import apiClient from './api.service';
import type { AxiosResponse } from 'axios';
import type {
  UnitAvailability,
  PricingRule,
  BookingConflict,
  CreateAvailabilityRequest,
  UpdateAvailabilityRequest,
  CreatePricingRequest,
  UpdatePricingRequest,
  AvailabilitySearchRequest,
  AvailabilitySearchResponse,
  PricingSearchRequest,
  PricingSearchResponse,
  ConflictCheckRequest,
  ConflictCheckResponse,
  ManagementPageResponse,
  ValidationSettings,
  AvailabilityStatistics,
  PricingStatistics,
  UnitManagementData,
  AvailabilityStatus,
  PricingTier,
  AvailabilityError,
  PricingError,
  ApplyPercentageRequestDto
} from '../types/availability_types';

// Use shared apiClient
const api = apiClient;

// ===== خدمة إدارة الإتاحة =====
export class AvailabilityService {
  
  // الحصول على الإتاحة لوحدة محددة
  static async getUnitAvailability(unitId: string, startDate?: string, endDate?: string): Promise<UnitAvailability[]> {
    try {
      const params = new URLSearchParams({ unitId });
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);
      
      const response = await api.get(`/api/availability?${params.toString()}`);
      return response.data.data || [];
    } catch (error) {
      console.error('خطأ في الحصول على الإتاحة:', error);
      throw this.handleError(error, 'availability');
    }
  }

  // البحث في الإتاحة
  static async searchAvailability(searchRequest: AvailabilitySearchRequest): Promise<AvailabilitySearchResponse> {
    try {
      const response = await api.post('/api/availability/search', searchRequest);
      return response.data;
    } catch (error) {
      console.error('خطأ في البحث عن الإتاحة:', error);
      throw this.handleError(error, 'availability');
    }
  }

  // إنشاء إتاحة جديدة
  static async createAvailability(request: CreateAvailabilityRequest): Promise<UnitAvailability> {
    try {
      const response = await api.post('/api/availability', request);
      return response.data.data;
    } catch (error) {
      console.error('خطأ في إنشاء الإتاحة:', error);
      throw this.handleError(error, 'availability');
    }
  }

  // تحديث الإتاحة
  static async updateAvailability(request: UpdateAvailabilityRequest): Promise<UnitAvailability> {
    try {
      const response = await api.put(`/api/availability/${request.availabilityId}`, request);
      return response.data.data;
    } catch (error) {
      console.error('خطأ في تحديث الإتاحة:', error);
      throw this.handleError(error, 'availability');
    }
  }

  // حذف الإتاحة
  static async deleteAvailability(availabilityId: string): Promise<void> {
    try {
      await api.delete(`/api/availability/${availabilityId}`);
    } catch (error) {
      console.error('خطأ في حذف الإتاحة:', error);
      throw this.handleError(error, 'availability');
    }
  }

  // إنشاء إتاحة مجمعة للوحدات المتعددة
  static async bulkCreateAvailability(requests: CreateAvailabilityRequest[]): Promise<UnitAvailability[]> {
    try {
      const response = await api.post('/api/availability/bulk', { requests });
      return response.data.data || [];
    } catch (error) {
      console.error('خطأ في الإنشاء المجمع للإتاحة:', error);
      throw this.handleError(error, 'availability');
    }
  }

  // تحديث حالة الإتاحة السريع
  static async quickUpdateStatus(unitId: string, status: AvailabilityStatus, dateRange: { start: string; end: string }): Promise<UnitAvailability[]> {
    try {
      const response = await api.patch(`/api/availability/quick-update/${unitId}`, {
        status,
        startDate: dateRange.start,
        endDate: dateRange.end
      });
      return response.data.data || [];
    } catch (error) {
      console.error('خطأ في التحديث السريع للإتاحة:', error);
      throw this.handleError(error, 'availability');
    }
  }

  // معالجة الأخطاء للإتاحة
  private static handleError(error: any, type: 'availability'): AvailabilityError {
    const errorData = error.response?.data;
    
    return {
      error_code: errorData?.error_code || 'UNKNOWN_ERROR',
      error_type: errorData?.error_type || 'system',
      message: errorData?.message || 'حدث خطأ غير متوقع',
      details: errorData?.details,
      suggested_action: errorData?.suggested_action || 'يرجى المحاولة مرة أخرى'
    };
  }
}

// ===== خدمة إدارة التسعير =====
export class PricingService {
  
  // الحصول على قواعد التسعير لوحدة محددة
  static async getUnitPricing(unitId: string, startDate?: string, endDate?: string): Promise<PricingRule[]> {
    try {
      const params = new URLSearchParams({ unitId });
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);
      
      const response = await api.get(`/api/pricing?${params.toString()}`);
      return response.data.data || [];
    } catch (error) {
      console.error('خطأ في الحصول على التسعير:', error);
      throw this.handleError(error, 'pricing');
    }
  }

  // البحث في قواعد التسعير
  static async searchPricing(searchRequest: PricingSearchRequest): Promise<PricingSearchResponse> {
    try {
      const response = await api.post('/api/pricing/search', searchRequest);
      return response.data;
    } catch (error) {
      console.error('خطأ في البحث عن التسعير:', error);
      throw this.handleError(error, 'pricing');
    }
  }

  // إنشاء قاعدة تسعير جديدة
  static async createPricing(request: CreatePricingRequest): Promise<PricingRule> {
    try {
      const response = await api.post('/api/pricing', request);
      return response.data.data;
    } catch (error) {
      console.error('خطأ في إنشاء قاعدة التسعير:', error);
      throw this.handleError(error, 'pricing');
    }
  }

  // تحديث قاعدة التسعير
  static async updatePricing(request: UpdatePricingRequest): Promise<PricingRule> {
    try {
      const response = await api.put(`/api/pricing/${request.pricingId}`, request);
      return response.data.data;
    } catch (error) {
      console.error('خطأ في تحديث قاعدة التسعير:', error);
      throw this.handleError(error, 'pricing');
    }
  }

  // حذف قاعدة التسعير
  static async deletePricing(pricingId: string): Promise<void> {
    try {
      await api.delete(`/api/pricing/${pricingId}`);
    } catch (error) {
      console.error('خطأ في حذف قاعدة التسعير:', error);
      throw this.handleError(error, 'pricing');
    }
  }

  // إنشاء قواعد تسعير مجمعة
  static async bulkCreatePricing(requests: CreatePricingRequest[]): Promise<PricingRule[]> {
    try {
      const response = await api.post('/api/pricing/bulk', { requests });
      return response.data.data || [];
    } catch (error) {
      console.error('خطأ في الإنشاء المجمع لقواعد التسعير:', error);
      throw this.handleError(error, 'pricing');
    }
  }

  // تحديث السعر السريع
  static async quickUpdatePrice(unitId: string, newPrice: number, dateRange: { start: string; end: string }): Promise<PricingRule> {
    try {
      const response = await api.patch(`/api/pricing/quick-update/${unitId}`, {
        priceAmount: newPrice,
        startDate: dateRange.start,
        endDate: dateRange.end
      });
      return response.data.data;
    } catch (error) {
      console.error('خطأ في التحديث السريع للسعر:', error);
      throw this.handleError(error, 'pricing');
    }
  }

  // تطبيق نسبة مئوية على الأسعار
  static async applyPercentageChange(unitIds: string[], percentage: number, dateRange: { start: string; end: string }): Promise<PricingRule[]> {
    try {
      const response = await api.post('/api/pricing/apply-percentage', {
        unitIds,
        percentageChange: percentage,
        startDate: dateRange.start,
        endDate: dateRange.end
      });
      return response.data.data || [];
    } catch (error) {
      console.error('خطأ في تطبيق النسبة المئوية:', error);
      throw this.handleError(error, 'pricing');
    }
  }
  // طلب تطبيق نسبة مئوية على وحدات
  static async applyPercentage(request: ApplyPercentageRequestDto): Promise<PricingRule[]> {
    return this.applyPercentageChange(request.unitIds, request.percentageChange, {
      start: request.startDate,
      end: request.endDate
    });
  }

  // الحصول على اقتراحات الأسعار
  static async getPricingSuggestions(unitId: string, dateRange: { start: string; end: string }): Promise<{
    suggested_price: number;
    market_average: number;
    seasonal_factor: number;
    demand_factor: number;
    confidence_level: number;
  }> {
    try {
      const response = await api.post('/api/pricing/suggestions', {
        unitId,
        startDate: dateRange.start,
        endDate: dateRange.end
      });
      return response.data.data;
    } catch (error) {
      console.error('خطأ في الحصول على اقتراحات الأسعار:', error);
      throw this.handleError(error, 'pricing');
    }
  }

  // معالجة الأخطاء للتسعير
  private static handleError(error: any, type: 'pricing'): PricingError {
    const errorData = error.response?.data;
    
    return {
      error_code: errorData?.error_code || 'UNKNOWN_ERROR',
      error_type: errorData?.error_type || 'system',
      message: errorData?.message || 'حدث خطأ غير متوقع',
      details: errorData?.details,
      suggested_action: errorData?.suggested_action || 'يرجى المحاولة مرة أخرى'
    };
  }
}

// ===== خدمة التحقق من التعارضات =====
export class BookingValidationService {
  
  // التحقق من التعارضات
  static async checkConflicts(request: ConflictCheckRequest): Promise<ConflictCheckResponse> {
    try {
      const response = await api.post('/api/validation/check-conflicts', request);
      return response.data;
    } catch (error) {
      console.error('خطأ في التحقق من التعارضات:', error);
      throw error;
    }
  }

  // الحصول على التعارضات لوحدة محددة
  static async getUnitConflicts(unitId: string, startDate?: string, endDate?: string): Promise<BookingConflict[]> {
    try {
      const params = new URLSearchParams({ unitId: unitId });
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);
      
      const response = await api.get(`/api/validation/conflicts?${params.toString()}`);
      return response.data.data || [];
    } catch (error) {
      console.error('خطأ في الحصول على التعارضات:', error);
      throw error;
    }
  }

  // حل التعارضات
  static async resolveConflict(conflictId: string, action: string, notes?: string): Promise<{ success: boolean; message: string }> {
    try {
      const response = await api.post(`/api/validation/resolve-conflict/${conflictId}`, {
        action,
        notes
      });
      return response.data;
    } catch (error) {
      console.error('خطأ في حل التعارض:', error);
      throw error;
    }
  }

  // إعدادات التحقق
  static async getValidationSettings(): Promise<ValidationSettings> {
    try {
      const response = await api.get('/api/validation/settings');
      return response.data.data;
    } catch (error) {
      console.error('خطأ في الحصول على إعدادات التحقق:', error);
      throw error;
    }
  }

  // تحديث إعدادات التحقق
  static async updateValidationSettings(settings: Partial<ValidationSettings>): Promise<ValidationSettings> {
    try {
      const response = await api.put('/api/validation/settings', settings);
      return response.data.data;
    } catch (error) {
      console.error('خطأ في تحديث إعدادات التحقق:', error);
      throw error;
    }
  }
}

// ===== خدمة حل التعارضات =====
export class ConflictResolutionService {
  
  // الحصول على خيارات الحل للتعارض
  static async getResolutionOptions(conflictId: string): Promise<{
    options: {
      action: string;
      title: string;
      description: string;
      feasible: boolean;
      estimated_cost: number;
      impactLevel: string;
    }[];
  }> {
    try {
      const response = await api.get(`/api/conflicts/${conflictId}/resolution-options`);
      return response.data;
    } catch (error) {
      console.error('خطأ في الحصول على خيارات الحل:', error);
      throw error;
    }
  }

  // تطبيق حل التعارض
  static async applyResolution(conflictId: string, resolutionData: {
    action: string;
    new_dates?: { start: string; end: string };
    compensation_amount?: number;
    notes?: string;
    notify_guest?: boolean;
  }): Promise<{ success: boolean; message: string; updated_booking?: any }> {
    try {
      const response = await api.post(`/api/conflicts/${conflictId}/resolve`, resolutionData);
      return response.data;
    } catch (error) {
      console.error('خطأ في تطبيق حل التعارض:', error);
      throw error;
    }
  }

  // إرسال إشعار للضيف
  static async notifyGuest(conflictId: string, message: string, options?: {
    include_alternatives?: boolean;
    compensation_offer?: number;
    priority_level?: 'normal' | 'urgent' | 'critical';
  }): Promise<{ success: boolean; message: string }> {
    try {
      const response = await api.post(`/api/conflicts/${conflictId}/notify-guest`, {
        message,
        options
      });
      return response.data;
    } catch (error) {
      console.error('خطأ في إرسال الإشعار للضيف:', error);
      throw error;
    }
  }
}

// ===== خدمة البيانات الشاملة =====
export class ManagementDataService {
  
  // الحصول على بيانات صفحة الإدارة
  static async getManagementPageData(propertyId?: string): Promise<ManagementPageResponse> {
    try {
      const params = propertyId ? `?propertyId=${propertyId}` : '';
      const response = await api.get(`/api/management/page-data${params}`);
      return response.data.data;
    } catch (error) {
      console.error('خطأ في الحصول على بيانات صفحة الإدارة:', error);
      throw error;
    }
  }

  // الحصول على بيانات وحدة محددة
  static async getUnitManagementData(unitId: string, startDate?: string, endDate?: string): Promise<UnitManagementData> {
    try {
      const params = new URLSearchParams();
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);
      const query = params.toString() ? `?${params.toString()}` : '';
      const response = await api.get(`/api/management/unit/${unitId}${query}`);
      return response.data.data;
    } catch (error) {
      console.error('خطأ في الحصول على بيانات الوحدة:', error);
      throw error;
    }
  }

  // الحصول على الإحصائيات
  static async getAvailabilityStatistics(unitIds: string[], dateRange: { start: string; end: string }): Promise<AvailabilityStatistics> {
    try {
      const response = await api.post('/api/management/statistics/availability', {
        unitIds,
        dateRange
      });
      return response.data.data;
    } catch (error) {
      console.error('خطأ في الحصول على إحصائيات الإتاحة:', error);
      throw error;
    }
  }

  // الحصول على إحصائيات التسعير
  static async getPricingStatistics(unitIds: string[], dateRange: { start: string; end: string }): Promise<PricingStatistics> {
    try {
      const response = await api.post('/api/management/statistics/pricing', {
        unitIds,
        dateRange
      });
      return response.data.data;
    } catch (error) {
      console.error('خطأ في الحصول على إحصائيات التسعير:', error);
      throw error;
    }
  }

  // تصدير البيانات
  static async exportData(format: 'excel' | 'csv' | 'pdf', data: {
    type: 'availability' | 'pricing' | 'conflicts' | 'statistics';
    unitIds?: string[];
    dateRange?: { start: string; end: string };
  }): Promise<Blob> {
    try {
      const payload = {
        type: data.type,
        unitIds: data.unitIds,
        dateRange: data.dateRange
      };
      const response = await api.post('/api/management/export', payload, {
        params: { format },
        responseType: 'blob'
      });
      return response.data;
    } catch (error) {
      console.error('خطأ في تصدير البيانات:', error);
      throw error;
    }
  }
}

// ===== خدمة مجمعة تضم جميع الخدمات =====
export class AvailabilityAndPricingService {
  static availability = AvailabilityService;
  static pricing = PricingService;
  static validation = BookingValidationService;
  static conflicts = ConflictResolutionService;
  static management = ManagementDataService;

  // تهيئة الخدمة
  static init(config?: {
    baseURL?: string;
    timeout?: number;
    defaultHeaders?: Record<string, string>;
  }) {
    if (config?.baseURL) {
      api.defaults.baseURL = config.baseURL;
    }
    if (config?.timeout) {
      api.defaults.timeout = config.timeout;
    }
    if (config?.defaultHeaders) {
      Object.assign(api.defaults.headers, config.defaultHeaders);
    }
  }

  // تعيين الـ token
  static setAuthToken(token: string) {
    localStorage.setItem('auth_token', token);
  }

  // إزالة الـ token
  static removeAuthToken() {
    localStorage.removeItem('auth_token');
  }
}

export default AvailabilityAndPricingService;
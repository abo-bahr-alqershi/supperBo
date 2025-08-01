import React, { useState, useEffect, useCallback } from 'react';
import { Box, Typography, Alert, Snackbar, CircularProgress, Card, CardContent } from '@mui/material';
import DateRangeCalendar from '../../components/common/DateRangeCalendar';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useParams } from 'react-router-dom';
import type { AvailabilityStatus, UnavailabilityReason, CreateAvailabilityRequest, UnitManagementData, UnitAvailability } from '../../types/availability_types';
import { AvailabilityAndPricingService } from '../../services/availability.services';
import { format } from 'date-fns';

// Component declaration and setup
const AvailabilityManagementPage: React.FC = () => {
  const { unitId } = useParams<{ unitId: string }>();
  const queryClient = useQueryClient();

  // State to track the visible calendar month range
  const [calendarRange, setCalendarRange] = useState<{ start: Date; end: Date }>(() => {
    const nowDate = new Date();
    return {
      start: new Date(nowDate.getFullYear(), nowDate.getMonth(), 1),
      end: new Date(nowDate.getFullYear(), nowDate.getMonth() + 1, 0),
    };
  });
  const handleMonthChange = useCallback((range: { start: Date; end: Date }) => {
    setCalendarRange(range);
  }, []);

  // Fetch unit data for current calendar range
  const { data: unitData, isLoading, error } = useQuery<UnitManagementData, Error>({
    queryKey: [
      'unit-management',
      unitId!,
      format(calendarRange.start, 'yyyy-MM-dd'),
      format(calendarRange.end, 'yyyy-MM-dd')
    ],
    queryFn: () =>
      AvailabilityAndPricingService.management.getUnitManagementData(
        unitId!,
        format(calendarRange.start, 'yyyy-MM-dd'),
        format(calendarRange.end, 'yyyy-MM-dd')
      ),
    enabled: !!unitId,
  });

  // دالة إنشاء التوفر
  const createMutation = useMutation<UnitAvailability, Error, CreateAvailabilityRequest>({
    mutationFn: (request) => AvailabilityAndPricingService.availability.createAvailability(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['unit-management', unitId!] })
  });
  // لم نعد بحاجة لحقل selectedUnit لأننا نحصل على unitId من URL
  const [dateRange, setDateRange] = useState<{ start: Date | null; end: Date | null }>({ start: null, end: null });
  const [status, setStatus] = useState<AvailabilityStatus>('available');
  const [reason, setReason] = useState<UnavailabilityReason>('maintenance');
  const [notes, setNotes] = useState('');
  const [conflictList, setConflictList] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [snackbar, setSnackbar] = useState<{ open: boolean; message: string; severity: 'success' | 'error' | 'warning' | 'info' }>({ open: false, message: '', severity: 'info' });
  // Time range state
  const [timeRange, setTimeRange] = useState<{ start: Date | null; end: Date | null }>({ start: null, end: null });

  // لا حاجة لعملية التحميل الأولية للوحدات هنا

  const showForm = dateRange.start !== null && dateRange.end !== null;

  const handleConfirm = useCallback(async () => {
    if (!dateRange.start || !dateRange.end || !unitId) return;
    setLoading(true);
    setConflictList([]);
    try {
      // التحقق من التعارضات عبر الخدمة مباشرة
      const { hasConflicts, conflicts } = await AvailabilityAndPricingService.validation.checkConflicts({
        unitId: unitId!,
        startDate: format(dateRange.start!, 'yyyy-MM-dd'),
        endDate: format(dateRange.end!, 'yyyy-MM-dd'),
        checkType: 'availability',
      });
      if (hasConflicts) {
        setConflictList(conflicts.map(c =>
          `رصد تعارض من ${c.bookingStart.toISOString().split('T')[0]} إلى ${c.bookingEnd.toISOString().split('T')[0]}`
        ));
        setLoading(false);
        return;
      }
      const request: CreateAvailabilityRequest = {
        unitId: unitId!,
        startDate: format(dateRange.start, 'yyyy-MM-dd'),
        endDate: format(dateRange.end,   'yyyy-MM-dd'),
        startTime: timeRange.start ? timeRange.start.toISOString().split('T')[1].substring(0,5) : undefined,
        endTime:   timeRange.end   ? timeRange.end.toISOString().split('T')[1].substring(0,5)   : undefined,
        status,
        reason: status === 'unavailable' ? reason : undefined,
        notes: notes || undefined,
      };
      // إنشاء الإتاحة
      await AvailabilityAndPricingService.availability.createAvailability(request);
      setSnackbar({ open: true, message: 'تم حفظ الإتاحة بنجاح', severity: 'success' });
      // تحديث بيانات الوحدة
      queryClient.invalidateQueries({ queryKey: ['unit-management', unitId!] });
      // Reset form
      setDateRange({ start: null, end: null });
      setStatus('available');
      setReason('maintenance');
      setNotes('');
    } catch (err: any) {
      setSnackbar({ open: true, message: err.message || 'حدث خطأ أثناء الحفظ', severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [dateRange, status, reason, notes, unitId, queryClient]);

  return (
    <Box sx={{ p: 3 }} dir="rtl" className="p-4 max-w-3xl mx-auto">
      <Typography variant="h4" gutterBottom>إدارة إتاحة الوحدة</Typography>
      {/* Form Card: shown after selecting date range */}
      {showForm && (
        <Card sx={{ mb: 3, p: 2 }} dir="rtl">
          <CardContent>
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">وقت البداية (اختياري)</label>
                <input
                  type="time"
                  value={timeRange.start ? timeRange.start.toISOString().substring(11,16) : ''}
                  onChange={(e) => setTimeRange(prev => ({ ...prev, start: e.target.value ? new Date(`${dateRange.start?.toISOString().split('T')[0]}T${e.target.value}`) : null }))}
                  className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">وقت النهاية (اختياري)</label>
                <input
                  type="time"
                  value={timeRange.end ? timeRange.end.toISOString().substring(11,16) : ''}
                  onChange={(e) => setTimeRange(prev => ({ ...prev, end: e.target.value ? new Date(`${dateRange.start?.toISOString().split('T')[0]}T${e.target.value}`) : null }))}
                  className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">حالة الإتاحة *</label>
                <select
                  value={status}
                  onChange={(e) => setStatus(e.target.value as AvailabilityStatus)}
                  className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                >
                  <option value="available">متاح</option>
                  <option value="unavailable">غير متاح</option>
                  <option value="maintenance">صيانة</option>
                  <option value="blocked">محجوب</option>
                </select>
              </div>
              {/* {status === 'unavailable' && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">سبب عدم الإتاحة *</label>
                  <select
                    value={reason}
                    onChange={(e) => setReason(e.target.value as UnavailabilityReason)}
                    className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  >
                    <option value="maintenance">صيانة</option>
                    <option value="vacation">إجازة</option>
                    <option value="private_booking">حجز خاص</option>
                    <option value="renovation">تجديد</option>
                    <option value="other">أخرى</option>
                  </select>
                </div>
              )} */}
              <div className="md:col-span-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">ملاحظات</label>
                <textarea
                  rows={3}
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                  className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                />
              </div>
              {conflictList.length > 0 && (
                <div className="md:col-span-2 text-red-600">
                  تم العثور على تعارضات:
                  <ul className="list-disc list-inside">
                    {conflictList.map((msg, idx) => (
                      <li key={idx}>{msg}</li>
                    ))}
                  </ul>
                </div>
              )}
              <div className="md:col-span-1">
                <button
                  onClick={handleConfirm}
                  disabled={loading}
                  className="w-full px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
                >
                  {loading ? 'جارٍ الحفظ...' : 'تأكيد'}
                </button>
              </div>
            </div>
          </CardContent>
        </Card>
      )}
      {/* Calendar */}
      <DateRangeCalendar
        units={unitData ? [unitData] : []}
        onDateSelect={setDateRange}
        selectedDateRange={dateRange}
        onMonthChange={handleMonthChange}
      />

      {loading && (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
          <CircularProgress />
        </Box>
      )}

      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar((prev) => ({ ...prev, open: false }))}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert
          onClose={() => setSnackbar((prev) => ({ ...prev, open: false }))}
          severity={snackbar.severity}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default AvailabilityManagementPage; 
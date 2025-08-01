import React, { useState, useCallback, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useParams } from 'react-router-dom';
import DateRangeCalendar from '../../components/common/DateRangeCalendar';
import { AvailabilityAndPricingService } from '../../services/availability.services';
import { format } from 'date-fns';
import type { CreatePricingRequest, UnitManagementData } from '../../types/availability_types';
import CurrencyInput from '../../components/inputs/CurrencyInput';
import { useCurrencies } from '../../hooks/useCurrencies';

const PricingManagementPage: React.FC = () => {
  const { unitId } = useParams<{ unitId: string }>();
  const queryClient = useQueryClient();

  // State to track visible calendar month range
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
      format(calendarRange.end, 'yyyy-MM-dd'),
    ],
    queryFn: () =>
      AvailabilityAndPricingService.management.getUnitManagementData(
        unitId!,
        format(calendarRange.start, 'yyyy-MM-dd'),
        format(calendarRange.end, 'yyyy-MM-dd')
      ),
    enabled: !!unitId,
  });

  // Date range for pricing
  const [dateRange, setDateRange] = useState<{ start: Date | null; end: Date | null }>({ start: null, end: null });

  // Form fields
  const [priceAmount, setPriceAmount] = useState<string>('');
  const [notes, setNotes] = useState<string>('');
  // Currency selection via server
  const { currencies, loading: currenciesLoading } = useCurrencies();
  const currencyOptions = currenciesLoading ? [] : currencies.map(c => c.code);
  const [currency, setCurrency] = useState<string>('');
  useEffect(() => {
    if (!currenciesLoading && currencyOptions.length > 0) {
      const defaultCurr = currencies.find(c => c.isDefault)?.code || currencyOptions[0];
      setCurrency(defaultCurr);
    }
  }, [currenciesLoading, currencies]);

  // Feedback
  const [message, setMessage] = useState<{ text: string; type: 'success' | 'error' } | null>(null);
  const [submitting, setSubmitting] = useState(false);

  // Mutation to create pricing
  const createPricing = useMutation<unknown, Error, CreatePricingRequest, unknown>({
    mutationFn: (req: CreatePricingRequest) => AvailabilityAndPricingService.pricing.createPricing(req),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['unit-management', unitId!] });
      setMessage({ text: 'تم حفظ قاعدة التسعير بنجاح', type: 'success' });
      // reset form
      setDateRange({ start: null, end: null });
      setPriceAmount(''); setNotes('');
    },
    onError: (err: any) => {
      setMessage({ text: err.message || 'حدث خطأ أثناء الحفظ', type: 'error' });
    }
  });

  const handleConfirm = useCallback(() => {
    if (!unitId || !dateRange.start || !dateRange.end) {
      setMessage({ text: 'يرجى اختيار الفترة وملء الحقول', type: 'error' });
      return;
    }
    const req: CreatePricingRequest = {
      unitId: unitId!,
      startDate: format(dateRange.start, 'yyyy-MM-dd'),
      endDate: format(dateRange.end, 'yyyy-MM-dd'),
      priceAmount: Number(priceAmount),
      pricingTier: 'custom',
      currency,
      description: notes || undefined,
    };
    setSubmitting(true);
    createPricing.mutate(req, {
      onSettled: () => setSubmitting(false)
    });
  }, [unitId, dateRange, priceAmount, notes, createPricing, currency]);

  return (
    <div className="p-4 max-w-3xl mx-auto">
      <h1 className="text-2xl font-bold mb-4">إدارة التسعير</h1>

      {/* Pricing form card */}
      {dateRange.start && dateRange.end && (
        <div className="bg-white p-4 rounded-lg shadow mb-6">
          <h2 className="text-lg font-semibold mb-2">تفاصيل التسعير للفترة المحددة</h2>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="sm:col-span-2">
              <label className="block mb-1 font-medium">المبلغ والعملة</label>
              <CurrencyInput
                value={priceAmount ? Number(priceAmount) : 0}
                currency={currency}
                onValueChange={(amount, curr) => {
                  setPriceAmount(amount.toString());
                  setCurrency(curr);
                }}
                supportedCurrencies={currencyOptions}
                className="w-full"
                placeholder="أدخل المبلغ"
              />
            </div>
            <div className="sm:col-span-2 text-right">
              <button
                onClick={handleConfirm}
                className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                disabled={submitting}
              >
                {submitting ? 'جارٍ الحفظ...' : 'تأكيد'}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Calendar */}
      <DateRangeCalendar
        units={unitData ? [unitData] : []}
        onDateSelect={setDateRange}
        onMonthChange={handleMonthChange}
      />

      {/* Loading & Error */}
      {isLoading && <div className="mt-4 text-center">جارٍ التحميل...</div>}
      {error && <div className="mt-4 text-red-600">{error.message}</div>}

      {/* Feedback message */}
      {message && (
        <div className={`mt-4 p-3 rounded ${message.type === 'success' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
          {message.text}
        </div>
      )}
    </div>
  );
};

export default PricingManagementPage;
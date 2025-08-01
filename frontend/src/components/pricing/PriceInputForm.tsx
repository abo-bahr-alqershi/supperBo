import React, { useState, useCallback, useEffect } from 'react';
import {
  Box,
  Typography,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Card,
  CardContent,
  Button,
  Slider,
  Switch,
  FormControlLabel,
  Chip,
  Alert,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  InputAdornment,
  Tooltip,
  IconButton,
  Paper,
  Divider,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Avatar,
  Badge,
  LinearProgress
} from '@mui/material';
import {
  ExpandMore as ExpandMoreIcon,
  AttachMoney as AttachMoneyIcon,
  Percent as PercentIcon,
  CalendarToday as CalendarIcon,
  Schedule as ScheduleIcon,
  TrendingUp as TrendingUpIcon,
  TrendingDown as TrendingDownIcon,
  Info as InfoIcon,
  AutoGraph as AutoGraphIcon,
  Lightbulb as LightbulbIcon,
  Star as StarIcon,
  CompareArrows as CompareArrowsIcon,
  Calculate as CalculateIcon,
  Edit as EditIcon,
  Add as AddIcon
} from '@mui/icons-material';
import { DatePicker, TimePicker } from '@mui/x-date-pickers';
import { styled } from '@mui/material/styles';
import { format } from 'date-fns';
import { ar } from 'date-fns/locale';
import ResponsiveGrid from '../common/ResponsiveGrid';
import type {
  UnitManagementData,
  PriceType,
  PricingTier,
  CreatePricingRequest
} from '../../types/availability_types';

// ===== الأنماط المخصصة =====
const FormContainer = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(3),
  borderRadius: theme.spacing(2),
  background: `linear-gradient(135deg, ${theme.palette.background.paper} 0%, ${theme.palette.grey[50]} 100%)`,
  border: `1px solid ${theme.palette.divider}`
}));

const PriceCard = styled(Card, {
  shouldForwardProp: (prop) => prop !== 'tier'
})<{ tier?: PricingTier }>(({ theme, tier }) => {
  const getTierColors = () => {
    switch (tier) {
      case 'high':
        return {
          background: `linear-gradient(135deg, ${theme.palette.warning.light}20, ${theme.palette.warning.main}20)`,
          border: `2px solid ${theme.palette.warning.main}`
        };
      case 'peak':
        return {
          background: `linear-gradient(135deg, ${theme.palette.error.light}20, ${theme.palette.error.main}20)`,
          border: `2px solid ${theme.palette.error.main}`
        };
      case 'discount':
        return {
          background: `linear-gradient(135deg, ${theme.palette.primary.light}20, ${theme.palette.primary.main}20)`,
          border: `2px solid ${theme.palette.primary.main}`
        };
      case 'normal':
        return {
          background: `linear-gradient(135deg, ${theme.palette.success.light}20, ${theme.palette.success.main}20)`,
          border: `2px solid ${theme.palette.success.main}`
        };
      default:
        return {
          background: theme.palette.grey[50],
          border: `2px solid ${theme.palette.grey[300]}`
        };
    }
  };

  return {
    ...getTierColors(),
    transition: 'all 0.3s ease',
    '&:hover': {
      transform: 'translateY(-2px)',
      boxShadow: theme.shadows[6]
    }
  };
});

const SuggestionCard = styled(Card)(({ theme }) => ({
  background: `linear-gradient(135deg, ${theme.palette.info.light}20, ${theme.palette.info.main}20)`,
  border: `1px solid ${theme.palette.info.main}`,
  marginBottom: theme.spacing(1),
  cursor: 'pointer',
  transition: 'all 0.2s ease',
  '&:hover': {
    transform: 'scale(1.02)',
    boxShadow: theme.shadows[4]
  }
}));

const QuickActionButton = styled(Button)(({ theme }) => ({
  margin: theme.spacing(0.5),
  borderRadius: theme.spacing(3),
  textTransform: 'none',
  fontWeight: 600
}));

// ===== أنواع البيانات =====
interface PriceInputFormProps {
  selectedUnits: string[];
  units: UnitManagementData[];
  onSubmit: (data: CreatePricingRequest) => void;
  onCancel: () => void;
  showAdvancedOptions?: boolean;
  initialValues?: Partial<CreatePricingRequest>;
  mode?: 'create' | 'edit';
  suggestions?: {
    suggested_price: number;
    market_average: number;
    seasonal_factor: number;
    demand_factor: number;
    confidence_level: number;
  };
}

interface PricingPreview {
  originalPrice: number;
  newPrice: number;
  change: number;
  changePercentage: number;
  tier: PricingTier;
}

// ===== دوال مساعدة =====
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('ar-SA', {
    style: 'currency',
    currency: 'SAR',
    minimumFractionDigits: 0
  }).format(amount);
};

const calculatePricingTier = (newPrice: number, basePrice: number): PricingTier => {
  const ratio = newPrice / basePrice;
  if (ratio >= 1.5) return 'peak';
  if (ratio >= 1.2) return 'high';
  if (ratio <= 0.8) return 'discount';
  if (ratio !== 1) return 'custom';
  return 'normal';
};

const getPriceTypeLabel = (type: PriceType): string => {
  switch (type) {
    case 'base': return 'سعر أساسي';
    case 'weekend': return 'نهاية الأسبوع';
    case 'seasonal': return 'موسمي';
    case 'holiday': return 'مناسبات';
    case 'special_event': return 'أحداث خاصة';
    case 'custom': return 'مخصص';
    default: return 'غير محدد';
  }
};

const getQuickAdjustments = () => [
  { label: '+20% للمواسم', percentage: 20, color: 'warning' as const },
  { label: '+50% للذروة', percentage: 50, color: 'error' as const },
  { label: '-15% للترويج', percentage: -15, color: 'primary' as const },
  { label: '-25% للخصم', percentage: -25, color: 'info' as const },
];

// ===== المكون الرئيسي =====
const PriceInputForm: React.FC<PriceInputFormProps> = ({
  selectedUnits,
  units,
  onSubmit,
  onCancel,
  showAdvancedOptions = false,
  initialValues,
  mode = 'create',
  suggestions
}) => {
  // الحالات الأساسية
  const [startDate, setStartDate] = useState<Date | null>(initialValues?.startDate ? new Date(initialValues.startDate) : null);
  const [endDate, setEndDate] = useState<Date | null>(initialValues?.endDate ? new Date(initialValues.endDate) : null);
  const [startTime, setStartTime] = useState<Date | null>(
    initialValues?.startTime ? new Date(`1970-01-01T${initialValues.startTime}`) : null
  );
  const [endTime, setEndTime] = useState<Date | null>(
    initialValues?.endTime ? new Date(`1970-01-01T${initialValues.endTime}`) : null
  );
  const [priceType, setPriceType] = useState<PriceType>('base');
  const [priceAmount, setPriceAmount] = useState<number>(initialValues?.priceAmount || 0);
  const [percentageChange, setPercentageChange] = useState<number>(initialValues?.percentageChange || 0);
  const [usePriceAmount, setUsePriceAmount] = useState<boolean>(true);
  const [minPrice, setMinPrice] = useState<number>(initialValues?.minPrice || 0);
  const [maxPrice, setMaxPrice] = useState<number>(initialValues?.maxPrice || 0);
  const [description, setDescription] = useState<string>(initialValues?.description || '');
  const [applyToAllUnits, setApplyToAllUnits] = useState<boolean>(false);

  // حالات واجهة المستخدم
  const [expandedSections, setExpandedSections] = useState<string[]>(['basic']);
  const [showSuggestions, setShowSuggestions] = useState<boolean>(false);
  const [previewData, setPreviewData] = useState<PricingPreview[]>([]);

  // الوحدات المحددة
  const selectedUnitObjects = units.filter(unit => selectedUnits.includes(unit.unit.unitId));

  // حساب المعاينة
  useEffect(() => {
    const previews = selectedUnitObjects.map(unit => {
      const originalPrice = unit.unit.basePrice;
      let newPrice = originalPrice;

      if (usePriceAmount && priceAmount > 0) {
        newPrice = priceAmount;
      } else if (percentageChange !== 0) {
        newPrice = originalPrice * (1 + percentageChange / 100);
      }

      const change = newPrice - originalPrice;
      const changePercentage = ((change / originalPrice) * 100);
      const tier = calculatePricingTier(newPrice, originalPrice);

      return {
        originalPrice,
        newPrice,
        change,
        changePercentage,
        tier
      };
    });

    setPreviewData(previews);
  }, [selectedUnitObjects, priceAmount, percentageChange, usePriceAmount]);

  // معالجة توسيع الأقسام
  const handleSectionToggle = useCallback((section: string) => {
    setExpandedSections(prev => 
      prev.includes(section) 
        ? prev.filter(s => s !== section)
        : [...prev, section]
    );
  }, []);

  // تطبيق التعديل السريع
  const handleQuickAdjustment = useCallback((percentage: number) => {
    setPercentageChange(percentage);
    setUsePriceAmount(false);
  }, []);

  // تطبيق الاقتراح
  const handleApplySuggestion = useCallback(() => {
    if (suggestions) {
      setPriceAmount(suggestions.suggested_price);
      setUsePriceAmount(true);
      setPercentageChange(0);
    }
  }, [suggestions]);

  // معالجة الإرسال
  const handleSubmit = useCallback(() => {
    if (!startDate || !endDate) {
      alert('يرجى اختيار التواريخ');
      return;
    }

    if (!usePriceAmount && percentageChange === 0) {
      alert('يرجى إدخال سعر أو نسبة تغيير');
      return;
    }

    // حساب السعر النهائي وفئة التسعير
    const basePrice = selectedUnitObjects[0].unit.basePrice;
    const finalPrice = usePriceAmount && priceAmount > 0
      ? priceAmount
      : !usePriceAmount && percentageChange !== 0
        ? basePrice * (1 + percentageChange / 100)
        : basePrice;
    const tier = calculatePricingTier(finalPrice, basePrice);

    const requestData: CreatePricingRequest = {
      unitId: selectedUnits[0],
      startDate: format(startDate, 'yyyy-MM-dd'),
      endDate: format(endDate, 'yyyy-MM-dd'),
      startTime: startTime ? startTime.toISOString().split('T')[1].substring(0,5) : undefined,
      endTime: endTime ? endDate.toISOString().split('T')[1].substring(0,5) : undefined,
      priceAmount: finalPrice,
      pricingTier: tier,
      percentageChange: !usePriceAmount && percentageChange !== 0 ? percentageChange : undefined,
      minPrice: minPrice > 0 ? minPrice : undefined,
      maxPrice: maxPrice > 0 ? maxPrice : undefined,
      description: description || undefined,
      currency: 'USD'
    };

    onSubmit(requestData);
  }, [
    startDate, endDate, startTime, endTime, priceType, priceAmount, 
    percentageChange, usePriceAmount, minPrice, maxPrice, description, 
    selectedUnits, onSubmit
  ]);

  return (
    <FormContainer>
      {/* معلومات الوحدات المحددة */}
      <Box mb={3}>
        <Typography variant="h6" gutterBottom sx={{ fontWeight: 600 }}>
          الوحدات المحددة ({selectedUnitObjects.length})
        </Typography>
        <Box display="flex" flexWrap="wrap" gap={1}>
          {selectedUnitObjects.map(unit => (
            <Chip
              key={unit.unit.unitId}
              label={`${unit.unit.unitName} - ${formatCurrency(unit.unit.basePrice)}`}
              color="primary"
              avatar={<Avatar>{unit.unit.unitName.charAt(0)}</Avatar>}
            />
          ))}
        </Box>
      </Box>

      {/* الإعدادات الأساسية */}
      <Accordion 
        expanded={expandedSections.includes('basic')}
        onChange={() => handleSectionToggle('basic')}
      >
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            الإعدادات الأساسية
          </Typography>
        </AccordionSummary>
        <AccordionDetails>
          <ResponsiveGrid container spacing={3}>
            <ResponsiveGrid item xs={12} md={6}>
              <DatePicker
                label="تاريخ البداية"
                value={startDate}
                onChange={(newValue) => setStartDate(newValue)}
                enableAccessibleFieldDOMStructure={false}
                slotProps={{ textField: { fullWidth: true, required: true } }}
              />
            </ResponsiveGrid>
            
            <ResponsiveGrid item xs={12} md={6}>
              <DatePicker
                label="تاريخ النهاية"
                value={endDate}
                onChange={(newValue) => setEndDate(newValue)}
                enableAccessibleFieldDOMStructure={false}
                slotProps={{ textField: { fullWidth: true, required: true } }}
              />
            </ResponsiveGrid>

            {showAdvancedOptions && (
              <>
                <ResponsiveGrid item xs={12} md={6}>
                  <TimePicker
                    label="وقت البداية (اختياري)"
                    value={startTime}
                    onChange={(newValue) => setStartTime(newValue)}
                    enableAccessibleFieldDOMStructure={false}
                    slots={{ textField: TextField }}
                    slotProps={{ textField: { fullWidth: true } }}
                  />
                </ResponsiveGrid>
                
                <ResponsiveGrid item xs={12} md={6}>
                  <TimePicker
                    label="وقت النهاية (اختياري)"
                    value={endTime}
                    onChange={(newValue) => setEndTime(newValue)}
                    enableAccessibleFieldDOMStructure={false}
                    slots={{ textField: TextField }}
                    slotProps={{ textField: { fullWidth: true } }}
                  />
                </ResponsiveGrid>
              </>
            )}

            <ResponsiveGrid item xs={12} md={6}>
              <FormControl fullWidth>
                <InputLabel>نوع السعر</InputLabel>
                <Select
                  value={priceType}
                  onChange={(e) => setPriceType(e.target.value as PriceType)}
                >
                  <MenuItem value="base">سعر أساسي</MenuItem>
                  <MenuItem value="weekend">نهاية الأسبوع</MenuItem>
                  <MenuItem value="seasonal">موسمي</MenuItem>
                  <MenuItem value="holiday">مناسبات</MenuItem>
                  <MenuItem value="special_event">أحداث خاصة</MenuItem>
                  <MenuItem value="custom">مخصص</MenuItem>
                </Select>
              </FormControl>
            </ResponsiveGrid>

            <ResponsiveGrid item xs={12} md={6}>
              <FormControlLabel
                control={
                  <Switch
                    checked={usePriceAmount}
                    onChange={(e) => setUsePriceAmount(e.target.checked)}
                  />
                }
                label="استخدام سعر محدد (بدلاً من النسبة المئوية)"
              />
            </ResponsiveGrid>
          </ResponsiveGrid>
        </AccordionDetails>
      </Accordion>

      {/* إعدادات السعر */}
      <Accordion 
        expanded={expandedSections.includes('pricing')}
        onChange={() => handleSectionToggle('pricing')}
      >
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            إعدادات السعر
          </Typography>
        </AccordionSummary>
        <AccordionDetails>
          {/* التعديلات السريعة */}
          <Box mb={3}>
            <Typography variant="subtitle1" gutterBottom sx={{ fontWeight: 600 }}>
              تعديلات سريعة:
            </Typography>
            <Box display="flex" flexWrap="wrap" gap={1}>
              {getQuickAdjustments().map((adjustment, index) => (
                <QuickActionButton
                  key={index}
                  variant="outlined"
                  color={adjustment.color}
                  onClick={() => handleQuickAdjustment(adjustment.percentage)}
                  startIcon={adjustment.percentage > 0 ? <TrendingUpIcon /> : <TrendingDownIcon />}
                >
                  {adjustment.label}
                </QuickActionButton>
              ))}
            </Box>
          </Box>

          <ResponsiveGrid container spacing={3}>
            {usePriceAmount ? (
              <ResponsiveGrid item xs={12} md={6}>
                <TextField
                  label="السعر الجديد"
                  type="number"
                  value={priceAmount}
                  onChange={(e) => setPriceAmount(Number(e.target.value))}
                  fullWidth
                  InputProps={{
                    startAdornment: <InputAdornment position="start"><AttachMoneyIcon /></InputAdornment>,
                    endAdornment: <InputAdornment position="end">ر.ي</InputAdornment>
                  }}
                />
              </ResponsiveGrid>
            ) : (
              <ResponsiveGrid item xs={12} md={6}>
                <Typography variant="body2" gutterBottom>
                  نسبة التغيير: {percentageChange > 0 ? '+' : ''}{percentageChange}%
                </Typography>
                <Slider
                  value={percentageChange}
                  onChange={(_, value) => setPercentageChange(value as number)}
                  min={-50}
                  max={100}
                  step={5}
                  marks={[
                    { value: -50, label: '-50%' },
                    { value: 0, label: '0%' },
                    { value: 50, label: '+50%' },
                    { value: 100, label: '+100%' }
                  ]}
                  valueLabelDisplay="on"
                  valueLabelFormat={(value) => `${value > 0 ? '+' : ''}${value}%`}
                />
              </ResponsiveGrid>
            )}

            {showAdvancedOptions && (
              <>
                <ResponsiveGrid item xs={12} md={6}>
                  <TextField
                    label="الحد الأدنى للسعر"
                    type="number"
                    value={minPrice}
                    onChange={(e) => setMinPrice(Number(e.target.value))}
                    fullWidth
                    InputProps={{
                      endAdornment: <InputAdornment position="end">ر.ي</InputAdornment>
                    }}
                  />
                </ResponsiveGrid>
                
                <ResponsiveGrid item xs={12} md={6}>
                  <TextField
                    label="الحد الأقصى للسعر"
                    type="number"
                    value={maxPrice}
                    onChange={(e) => setMaxPrice(Number(e.target.value))}
                    fullWidth
                    InputProps={{
                      endAdornment: <InputAdornment position="end">ر.ي</InputAdornment>
                    }}
                  />
                </ResponsiveGrid>
              </>
            )}

            <ResponsiveGrid item xs={12}>
              <TextField
                label="وصف أو ملاحظات"
                multiline
                rows={3}
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                fullWidth
                placeholder="أدخل وصف لقاعدة التسعير هذه..."
              />
            </ResponsiveGrid>
          </ResponsiveGrid>
        </AccordionDetails>
      </Accordion>

      {/* الاقتراحات الذكية */}
      {suggestions && (
        <Accordion 
          expanded={expandedSections.includes('suggestions')}
          onChange={() => handleSectionToggle('suggestions')}
        >
          <AccordionSummary expandIcon={<ExpandMoreIcon />}>
            <Box display="flex" alignItems="center" gap={1}>
              <LightbulbIcon color="warning" />
              <Typography variant="h6" sx={{ fontWeight: 600 }}>
                الاقتراحات الذكية
              </Typography>
              <Badge badgeContent={`${(suggestions.confidence_level * 100).toFixed(0)}%`} color="primary">
                <StarIcon color="warning" />
              </Badge>
            </Box>
          </AccordionSummary>
          <AccordionDetails>
            <SuggestionCard onClick={handleApplySuggestion}>
              <CardContent>
                <Box display="flex" justifyContent="space-between" alignItems="center">
                  <Box>
                    <Typography variant="h6" color="primary" sx={{ fontWeight: 600 }}>
                      {formatCurrency(suggestions.suggested_price)}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      السعر المقترح بناءً على تحليل السوق
                    </Typography>
                  </Box>
                  <Box textAlign="right">
                    <Typography variant="caption" display="block">
                      مستوى الثقة: {(suggestions.confidence_level * 100).toFixed(0)}%
                    </Typography>
                    <LinearProgress 
                      variant="determinate" 
                      value={suggestions.confidence_level * 100} 
                      sx={{ width: 80, mt: 0.5 }}
                    />
                  </Box>
                </Box>
                
                <ResponsiveGrid container spacing={2} sx={{ mt: 1 }}>
                  <ResponsiveGrid item xs={4}>
                    <Typography variant="caption" color="text.secondary">متوسط السوق</Typography>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      {formatCurrency(suggestions.market_average)}
                    </Typography>
                  </ResponsiveGrid>
                  <ResponsiveGrid item xs={4}>
                    <Typography variant="caption" color="text.secondary">العامل الموسمي</Typography>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      {(suggestions.seasonal_factor * 100).toFixed(1)}%
                    </Typography>
                  </ResponsiveGrid>
                  <ResponsiveGrid item xs={4}>
                    <Typography variant="caption" color="text.secondary">عامل الطلب</Typography>
                    <Typography variant="body2" sx={{ fontWeight: 600 }}>
                      {(suggestions.demand_factor * 100).toFixed(1)}%
                    </Typography>
                  </ResponsiveGrid>
                </ResponsiveGrid>
              </CardContent>
            </SuggestionCard>
          </AccordionDetails>
        </Accordion>
      )}

      {/* معاينة التغييرات */}
      {previewData.length > 0 && (
        <Accordion 
          expanded={expandedSections.includes('preview')}
          onChange={() => handleSectionToggle('preview')}
        >
          <AccordionSummary expandIcon={<ExpandMoreIcon />}>
            <Typography variant="h6" sx={{ fontWeight: 600 }}>
              معاينة التغييرات
            </Typography>
          </AccordionSummary>
          <AccordionDetails>
            <ResponsiveGrid container spacing={2}>
              {previewData.map((preview, index) => {
                const unit = selectedUnitObjects[index];
                return (
                  <ResponsiveGrid item xs={12} md={6} key={unit.unit.unitId}>
                    <PriceCard tier={preview.tier}>
                      <CardContent>
                        <Typography variant="subtitle1" sx={{ fontWeight: 600 }} gutterBottom>
                          {unit.unit.unitName}
                        </Typography>
                        
                        <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                          <Typography variant="body2" color="text.secondary">
                            السعر الحالي:
                          </Typography>
                          <Typography variant="body2" sx={{ textDecoration: 'line-through' }}>
                            {formatCurrency(preview.originalPrice)}
                          </Typography>
                        </Box>
                        
                        <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                          <Typography variant="body2" color="text.secondary">
                            السعر الجديد:
                          </Typography>
                          <Typography variant="h6" color="primary" sx={{ fontWeight: 600 }}>
                            {formatCurrency(preview.newPrice)}
                          </Typography>
                        </Box>
                        
                        <Box display="flex" justifyContent="space-between" alignItems="center">
                          <Typography variant="body2" color="text.secondary">
                            التغيير:
                          </Typography>
                          <Box display="flex" alignItems="center" gap={1}>
                            {preview.changePercentage > 0 ? <TrendingUpIcon color="error" /> : <TrendingDownIcon color="success" />}
                            <Typography 
                              variant="body2" 
                              color={preview.changePercentage > 0 ? 'error' : 'success'}
                              sx={{ fontWeight: 600 }}
                            >
                              {preview.changePercentage > 0 ? '+' : ''}{preview.changePercentage.toFixed(1)}%
                            </Typography>
                          </Box>
                        </Box>
                      </CardContent>
                    </PriceCard>
                  </ResponsiveGrid>
                );
              })}
            </ResponsiveGrid>
          </AccordionDetails>
        </Accordion>
      )}

      {/* أزرار الإجراءات */}
      <Box display="flex" justifyContent="flex-end" gap={2} mt={3}>
        <Button 
          onClick={onCancel}
          variant="outlined"
          size="large"
        >
          إلغاء
        </Button>
        
        <Button
          onClick={handleSubmit}
          variant="contained"
          size="large"
          startIcon={mode === 'edit' ? <EditIcon /> : <AddIcon />}
          disabled={!startDate || !endDate || (!usePriceAmount && percentageChange === 0)}
        >
          {mode === 'edit' ? 'تحديث التسعير' : 'إنشاء قاعدة التسعير'}
        </Button>
      </Box>
    </FormContainer>
  );
};

export default PriceInputForm;
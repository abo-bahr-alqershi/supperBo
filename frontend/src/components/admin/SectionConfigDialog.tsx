// frontend/src/components/admin/SectionConfigDialog.tsx (الكود الكامل)

import React, { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Tabs,
  Tab,
  Box,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Switch,
  FormControlLabel,
  Typography,
  Grid,
  Paper,
  Divider,
  Chip,
  IconButton,
  Slider,
  Stack,
  Alert,
  Tooltip,
  FormHelperText,
  Checkbox,
  FormGroup,
  CircularProgress,
  Avatar,
  Card,
  CardContent,
  CardActionArea,
  Fade,
  Zoom,
  alpha,
  useTheme,
  InputAdornment,
  ToggleButton,
  ToggleButtonGroup,
  ListItem,
  ListItemIcon,
  ListItemText,
  List,
  Collapse,
} from '@mui/material';
import {
  ExpandMore as ExpandMoreIcon,
  Close as CloseIcon,
  Preview as PreviewIcon,
  Settings as SettingsIcon,
  Palette as PaletteIcon,
  Animation as AnimationIcon,
  Schedule as ScheduleIcon,
  Groups as AudienceIcon,
  Code as CodeIcon,
  Visibility as VisibilityIcon,
  Phone as MobileIcon,
  Computer as WebIcon,
  Style as StyleIcon,
  Speed as SpeedIcon,
  ViewModule as LayoutIcon,
  Dashboard as ContentIcon,
  Save as SaveIcon,
  Info as InfoIcon,
  CheckCircle as CheckIcon,
  ColorLens as ColorIcon,
  Timer as TimerIcon,
  Storage as StorageIcon,
  Star as StarIcon,
  AutoAwesome as AutoAwesomeIcon,
  Gradient as GradientIcon,
  FormatSize as FormatSizeIcon,
  SwipeRight as SwipeIcon,
  TouchApp as TouchIcon,
  Cached as CachedIcon,
  Badge as BadgeIcon,
  Label as LabelIcon,
  PlayCircle as PlayIcon,
  PauseCircle as PauseIcon,
  Shuffle as ShuffleIcon,
  ViewCarousel as CarouselIcon,
  GridView as GridIcon,
  ViewList as ListIcon,
  CalendarMonth as CalendarIcon,
  AccessTime as TimeIcon,
  People as PeopleIcon,
  Language as LanguageIcon,
  Public as PublicIcon,
  PhoneAndroid as PhoneAndroidIcon,
  DesktopMac as DesktopIcon,
  Memory as MemoryIcon,
  CloudSync as CloudSyncIcon,
  DataObject as DataIcon,
} from '@mui/icons-material';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import * as arLocale from 'date-fns/locale/ar-SA';
import type { Locale } from 'date-fns';
import HomeScreenManagement from '../../pages/admin/HomeScreenManagement';
import ContentManagementTab from './SectionContentManagementTab';
import type { DynamicHomeSection, DynamicContent } from '../../types/homeSections.types';

// Professional color palette
const COLORS = {
  primary: '#1a237e',
  secondary: '#004d40',
  success: '#1b5e20',
  warning: '#e65100',
  info: '#01579b',
  accent: '#6a1b9a',
  background: {
    light: '#f8f9fa',
    main: '#ffffff',
    dark: '#eceff1',
  },
  text: {
    primary: '#263238',
    secondary: '#546e7a',
    disabled: '#90a4ae',
  },
  border: '#e0e0e0',
};

// Section Types
const SECTION_TYPES = [
  {
    category: 'الإعلانات المدعومة',
    color: COLORS.success,
    bgColor: alpha(COLORS.success, 0.08),
    types: [
      { type: 'SINGLE_PROPERTY_AD', name: 'إعلان عقار واحد', icon: '🏠', height: 320, description: 'عرض إعلان مميز لعقار واحد' },
      { type: 'FEATURED_PROPERTY_AD', name: 'إعلان عقار مميز', icon: '⭐', height: 400, description: 'إبراز عقار مميز بتصميم جذاب' },
      { type: 'MULTI_PROPERTY_AD', name: 'إعلان متعدد العقارات', icon: '🏘️', height: 280, description: 'عرض مجموعة من العقارات' },
      { type: 'UNIT_SHOWCASE_AD', name: 'عرض وحدة سكنية', icon: '🏢', height: 450, description: 'عرض تفصيلي للوحدات السكنية' }
    ]
  },
  {
    category: 'العروض الخاصة',
    color: COLORS.warning,
    bgColor: alpha(COLORS.warning, 0.08),
    types: [
      { type: 'SINGLE_PROPERTY_OFFER', name: 'عرض عقار واحد', icon: '💰', height: 200, description: 'عرض خاص لعقار محدد' },
      { type: 'LIMITED_TIME_OFFER', name: 'عرض محدود المدة', icon: '⏰', height: 220, description: 'عروض لفترة زمنية محدودة' },
      { type: 'SEASONAL_OFFER', name: 'عروض موسمية', icon: '🎄', height: 240, description: 'عروض خاصة بالمواسم' },
      { type: 'MULTI_PROPERTY_OFFERS_GRID', name: 'شبكة عروض متعددة', icon: '📊', height: 400, description: 'مجموعة من العروض المتنوعة' },
      { type: 'OFFERS_CAROUSEL', name: 'دوار العروض', icon: '🎠', height: 260, description: 'عرض دوار للعروض الخاصة' },
      { type: 'FLASH_DEALS', name: 'صفقات برق', icon: '⚡', height: 280, description: 'صفقات سريعة ومحدودة' }
    ]
  },
  {
    category: 'قوائم العقارات',
    color: COLORS.info,
    bgColor: alpha(COLORS.info, 0.08),
    types: [
      { type: 'HORIZONTAL_PROPERTY_LIST', name: 'قائمة أفقية', icon: '📋', height: 280, description: 'عرض أفقي للعقارات' },
      { type: 'VERTICAL_PROPERTY_GRID', name: 'شبكة عمودية', icon: '🔲', height: 400, description: 'شبكة عمودية منظمة' },
      { type: 'MIXED_LAYOUT_LIST', name: 'قائمة تخطيط مختلط', icon: '🔀', height: 500, description: 'تخطيط متنوع ومرن' },
      { type: 'COMPACT_PROPERTY_LIST', name: 'قائمة مضغوطة', icon: '📑', height: 180, description: 'قائمة موجزة وفعالة' },
      { type: 'FEATURED_PROPERTIES_SHOWCASE', name: 'عرض العقارات المميزة', icon: '🌟', height: 360, description: 'إبراز أفضل العقارات' }
    ]
  },
  {
    category: 'الوجهات',
    color: COLORS.accent,
    bgColor: alpha(COLORS.accent, 0.08),
    types: [
      { type: 'CITY_CARDS_GRID', name: 'شبكة بطاقات المدن', icon: '🏙️', height: 200, description: 'عرض المدن الرئيسية' },
      { type: 'DESTINATION_CAROUSEL', name: 'دوار الوجهات', icon: '🗺️', height: 240, description: 'استكشاف الوجهات السياحية' },
      { type: 'EXPLORE_CITIES', name: 'استكشاف المدن', icon: '🧭', height: 320, description: 'جولة في المدن المختلفة' }
    ]
  },
  {
    category: 'الدوارات المتميزة',
    color: COLORS.secondary,
    bgColor: alpha(COLORS.secondary, 0.08),
    types: [
      { type: 'PREMIUM_CAROUSEL', name: 'دوار متميز', icon: '💎', height: 380, description: 'عرض دوار احترافي' },
      { type: 'INTERACTIVE_SHOWCASE', name: 'عرض تفاعلي', icon: '🎯', height: 420, description: 'تجربة تفاعلية متقدمة' }
    ]
  }
];

// Animation Types
const ANIMATION_TYPES = [
  { value: 'NONE', label: 'بدون حركة', icon: <PauseIcon />, color: COLORS.text.disabled },
  { value: 'FADE', label: 'تلاشي', icon: <AutoAwesomeIcon />, color: COLORS.info },
  { value: 'SLIDE', label: 'انزلاق', icon: <SwipeIcon />, color: COLORS.primary },
  { value: 'SCALE', label: 'تكبير/تصغير', icon: <FormatSizeIcon />, color: COLORS.success },
  { value: 'ROTATE', label: 'دوران', icon: <CachedIcon />, color: COLORS.warning },
  { value: 'BOUNCE', label: 'ارتداد', icon: <TouchIcon />, color: COLORS.accent },
];

// Layout Types
const LAYOUT_TYPES = [
  { value: 'horizontal', label: 'أفقي', icon: <CarouselIcon />, description: 'عرض العناصر بشكل أفقي' },
  { value: 'grid', label: 'شبكة', icon: <GridIcon />, description: 'توزيع العناصر في شبكة منتظمة' },
  { value: 'list', label: 'قائمة', icon: <ListIcon />, description: 'عرض العناصر كقائمة عمودية' },
  { value: 'carousel', label: 'دوار', icon: <ShuffleIcon />, description: 'عرض دوار تفاعلي' },
];

// Target Audiences
const TARGET_AUDIENCES = [
  { value: 'all', label: 'الجميع', icon: <PublicIcon />, color: COLORS.primary },
  { value: 'new_users', label: 'مستخدمون جدد', icon: <StarIcon />, color: COLORS.success },
  { value: 'premium_users', label: 'مستخدمون مميزون', icon: <AutoAwesomeIcon />, color: COLORS.warning },
  { value: 'mobile_users', label: 'مستخدمو الموبايل', icon: <PhoneAndroidIcon />, color: COLORS.info },
  { value: 'web_users', label: 'مستخدمو الويب', icon: <DesktopIcon />, color: COLORS.secondary },
];

interface SectionConfigDialogProps {
  open: boolean;
  onClose: () => void;
  section: DynamicHomeSection | null;
  isEdit: boolean;
  onSave: (data: any) => void;
}

const SectionConfigDialog: React.FC<SectionConfigDialogProps> = ({
  open,
  onClose,
  section,
  isEdit,
  onSave
}) => {
  const theme = useTheme();
  const [currentTab, setCurrentTab] = useState(0);
  const [formData, setFormData] = useState<any>({
    sectionType: '',
    order: 1,
    title: '',
    subtitle: '',
    titleAr: '',
    subtitleAr: '',
    priority: 0,
    scheduledAt: null,
    expiresAt: null,
    targetAudience: ['all'],
    content: [],
    displaySettings: {
      maxItems: 10,
      showTitle: true,
      showSubtitle: true,
      showBadge: false,
      badgeText: '',
      showIndicators: true,
      showViewAllButton: true,
      autoPlay: false,
      autoPlayDuration: 3000
    },
    layoutSettings: {
      layoutType: 'horizontal',
      columnsCount: 2,
      itemHeight: 200,
      itemSpacing: 16,
      sectionPadding: 16,
      sectionSize: 'MEDIUM'
    },
    styleSettings: {
      backgroundColor: '#ffffff',
      textColor: '#263238',
      borderRadius: 12,
      elevation: 2,
      enableGradient: false,
      gradientColors: ['#ffffff', '#f5f5f5'],
    },
    animationSettings: {
      type: 'NONE',
      duration: 300,
      delay: 0,
      curve: 'ease-in-out',
    },
    behaviorSettings: {
      infiniteScroll: false,
      enablePullToRefresh: true,
      lazy: true,
      clickable: true
    },
    cacheSettings: {
      enableCache: true,
      maxAge: 3600000,
      cacheImages: true,
    },
  });

  useEffect(() => {
    if (section && isEdit) {
      setFormData({
        ...formData,
        sectionType: section.sectionType || '',
        order: section.order || 1,
        title: section.title || '',
        subtitle: section.subtitle || '',
        titleAr: section.titleAr || '',
        subtitleAr: section.subtitleAr || '',
        priority: section.priority || 0,
        scheduledAt: section.scheduledAt ? new Date(section.scheduledAt) : null,
        expiresAt: section.expiresAt ? new Date(section.expiresAt) : null,
        targetAudience: section.targetAudience || ['all'],
        content: section.content || [],
      });
    }
  }, [section, isEdit, open]);

  const handleInputChange = (path: string, value: any) => {
    const keys = path.split('.');
    const newFormData = { ...formData };
    
    let current = newFormData;
    for (let i = 0; i < keys.length - 1; i++) {
      if (!current[keys[i]]) current[keys[i]] = {};
      current = current[keys[i]];
    }
    current[keys[keys.length - 1]] = value;
    
    setFormData(newFormData);
  };

  const handleSave = () => {
    const sectionData = {
      ...formData,
      scheduledAt: formData.scheduledAt?.toISOString(),
      expiresAt: formData.expiresAt?.toISOString(),
      sectionConfig: {
        displaySettings: formData.displaySettings,
        layoutSettings: formData.layoutSettings,
        styleSettings: formData.styleSettings,
        animationSettings: formData.animationSettings,
        behaviorSettings: formData.behaviorSettings,
        cacheSettings: formData.cacheSettings,
      },
    };
    onSave(sectionData);
  };

  const selectedSectionType = SECTION_TYPES
    .flatMap(cat => cat.types)
    .find(type => type.type === formData.sectionType);

  const selectedCategory = SECTION_TYPES.find(cat => 
    cat.types.some(type => type.type === formData.sectionType)
  );

  const tabConfig = [
    { label: 'المعلومات الأساسية', icon: <SettingsIcon />, component: BasicInformationTab },
    { label: 'إدارة المحتوى', icon: <ContentIcon />, component: ContentManagementTab },
    { label: 'إعدادات العرض', icon: <VisibilityIcon />, component: DisplaySettingsTab },
    { label: 'التخطيط', icon: <LayoutIcon />, component: LayoutSettingsTab },
    { label: 'الألوان والأنماط', icon: <PaletteIcon />, component: StyleSettingsTab },
    { label: 'الحركة', icon: <AnimationIcon />, component: AnimationSettingsTab },
    { label: 'الجدولة', icon: <ScheduleIcon />, component: SchedulingTab },
    { label: 'متقدم', icon: <CodeIcon />, component: AdvancedSettingsTab },
  ];

  const CurrentTabComponent = tabConfig[currentTab].component;

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={arLocale as unknown as Locale}>
      <Dialog 
        open={open} 
        onClose={onClose} 
        maxWidth="xl" 
        fullWidth
        dir="rtl"
        PaperProps={{
          sx: { 
            height: '90vh',
            borderRadius: 3,
            overflow: 'hidden'
          }
        }}
      >
        {/* Enhanced Header */}
        <DialogTitle 
          sx={{ 
            background: `linear-gradient(135deg, ${COLORS.primary} 0%, ${alpha(COLORS.primary, 0.8)} 100%)`,
            color: 'white',
            py: 2.5,
          }}
        >
          <Stack direction="row" alignItems="center" justifyContent="space-between">
            <Stack direction="row" alignItems="center" spacing={2}>
              <Avatar
                sx={{
                  bgcolor: 'white',
                  color: COLORS.primary,
                  width: 48,
                  height: 48,
                }}
              >
                {selectedSectionType?.icon || <SettingsIcon />}
              </Avatar>
              <Box>
                <Typography variant="h5" sx={{ fontWeight: 600 }}>
                  {isEdit ? 'تعديل القسم' : 'إضافة قسم جديد'}
                </Typography>
                {selectedSectionType && (
                  <Typography variant="body2" sx={{ opacity: 0.9, mt: 0.5 }}>
                    {selectedSectionType.name} • {selectedCategory?.category}
                  </Typography>
                )}
              </Box>
            </Stack>
            
            <IconButton
              onClick={onClose}
              sx={{
                color: 'white',
                '&:hover': { bgcolor: alpha('#fff', 0.1) }
              }}
            >
              <CloseIcon />
            </IconButton>
          </Stack>
        </DialogTitle>

        <DialogContent sx={{ p: 0, display: 'flex', height: 'calc(100% - 140px)' }}>
          {/* Enhanced Sidebar */}
          <Box
            sx={{
              width: 260,
              borderLeft: `1px solid ${COLORS.border}`,
              bgcolor: COLORS.background.light,
              display: 'flex',
              flexDirection: 'column',
            }}
          >
            <Tabs
              orientation="vertical"
              value={currentTab}
              onChange={(_, newValue) => setCurrentTab(newValue)}
              sx={{
                '& .MuiTab-root': {
                  minHeight: 56,
                  alignItems: 'flex-start',
                  textAlign: 'right',
                  px: 3,
                  py: 1.5,
                  borderRadius: 0,
                  transition: 'all 0.3s ease',
                  '&:hover': {
                    bgcolor: alpha(COLORS.primary, 0.05),
                  },
                  '&.Mui-selected': {
                    bgcolor: 'white',
                    color: COLORS.primary,
                    borderLeft: `3px solid ${COLORS.primary}`,
                  },
                },
                '& .MuiTabs-indicator': {
                  display: 'none',
                },
              }}
            >
              {tabConfig.map((tab, index) => (
                <Tab
                  key={index}
                  icon={
                    <Box sx={{ mr: 1.5, color: currentTab === index ? COLORS.primary : COLORS.text.secondary }}>
                      {tab.icon}
                    </Box>
                  }
                  label={
                    <Typography variant="body2" sx={{ fontWeight: currentTab === index ? 600 : 400 }}>
                      {tab.label}
                    </Typography>
                  }
                  iconPosition="start"
                />
              ))}
            </Tabs>

            {/* Progress Indicator */}
            <Box sx={{ mt: 'auto', p: 2 }}>
              <Stack spacing={1}>
                <Typography variant="caption" color="text.secondary">
                  اكتمال التكوين
                </Typography>
                <Box sx={{ position: 'relative' }}>
                  <CircularProgress
                    variant="determinate"
                    value={calculateCompletionPercentage(formData)}
                    size={40}
                    thickness={4}
                    sx={{ color: COLORS.success }}
                  />
                  <Box
                    sx={{
                      position: 'absolute',
                      top: '50%',
                      left: '50%',
                      transform: 'translate(-50%, -50%)',
                    }}
                  >
                    <Typography variant="caption" sx={{ fontWeight: 600 }}>
                      {calculateCompletionPercentage(formData)}%
                    </Typography>
                  </Box>
                </Box>
              </Stack>
            </Box>
          </Box>

          {/* Enhanced Content Area */}
          <Box sx={{ flexGrow: 1, overflow: 'auto', bgcolor: 'white' }}>
            <Fade in key={currentTab}>
              <Box sx={{ p: 4 }}>
                <CurrentTabComponent
                  formData={formData}
                  onChange={handleInputChange}
                  isEdit={isEdit}
                />
              </Box>
            </Fade>
          </Box>
        </DialogContent>

        {/* Enhanced Footer */}
        <DialogActions
          sx={{
            p: 3,
            borderTop: `1px solid ${COLORS.border}`,
            bgcolor: COLORS.background.light,
          }}
        >
          <Stack direction="row" spacing={2} sx={{ width: '100%' }}>
            <Button
              startIcon={<PreviewIcon />}
              variant="outlined"
              size="large"
              sx={{
                borderColor: COLORS.primary,
                color: COLORS.primary,
                '&:hover': {
                  borderColor: COLORS.primary,
                  bgcolor: alpha(COLORS.primary, 0.05),
                },
              }}
            >
              معاينة
            </Button>
            
            <Box sx={{ flexGrow: 1 }} />
            
            <Button
              onClick={onClose}
              size="large"
              sx={{
                color: COLORS.text.secondary,
                '&:hover': {
                  bgcolor: alpha(COLORS.text.secondary, 0.05),
                },
              }}
            >
              إلغاء
            </Button>
            
            <Button
              variant="contained"
              onClick={handleSave}
              size="large"
              startIcon={<SaveIcon />}
              disabled={!formData.sectionType}
              sx={{
                bgcolor: COLORS.primary,
                px: 4,
                '&:hover': {
                  bgcolor: alpha(COLORS.primary, 0.9),
                },
              }}
            >
              {isEdit ? 'حفظ التغييرات' : 'إنشاء القسم'}
            </Button>
          </Stack>
        </DialogActions>
      </Dialog>
    </LocalizationProvider>
  );
};

// Basic Information Tab
const BasicInformationTab: React.FC<{
  formData: any;
  onChange: (path: string, value: any) => void;
  isEdit: boolean;
}> = ({ formData, onChange, isEdit }) => {
  return (
    <Stack spacing={4}>
      {/* Section Header */}
      <Box>
        <Typography variant="h5" sx={{ fontWeight: 600, color: COLORS.text.primary, mb: 1 }}>
          المعلومات الأساسية
        </Typography>
        <Typography variant="body2" color="text.secondary">
          حدد نوع القسم والمعلومات الأساسية للعرض
        </Typography>
      </Box>

      {/* Section Type Selection */}
      {!isEdit && (
        <Paper
          elevation={0}
          sx={{
            p: 4,
            border: `1px solid ${COLORS.border}`,
            borderRadius: 2,
            bgcolor: COLORS.background.light,
          }}
        >
          <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
            اختر نوع القسم
          </Typography>
          
          <Stack spacing={3}>
            {SECTION_TYPES.map((category) => (
              <Box key={category.category}>
                <Stack direction="row" alignItems="center" spacing={1} sx={{ mb: 2 }}>
                  <Box
                    sx={{
                      width: 4,
                      height: 24,
                      bgcolor: category.color,
                      borderRadius: 1,
                    }}
                  />
                  <Typography variant="subtitle1" sx={{ fontWeight: 600, color: category.color }}>
                    {category.category}
                  </Typography>
                </Stack>
                
                <Grid container spacing={2}>
                  {category.types.map((type) => (
                    <Grid item xs={12} sm={6} md={4} key={type.type}>
                      <Card
                        sx={{
                          cursor: 'pointer',
                          border: formData.sectionType === type.type ? `2px solid ${category.color}` : `1px solid ${COLORS.border}`,
                          bgcolor: formData.sectionType === type.type ? category.bgColor : 'white',
                          transition: 'all 0.3s ease',
                          '&:hover': {
                            transform: 'translateY(-4px)',
                            boxShadow: 4,
                            borderColor: category.color,
                          },
                        }}
                        onClick={() => onChange('sectionType', type.type)}
                      >
                        <CardActionArea>
                          <CardContent sx={{ textAlign: 'center', py: 3 }}>
                            <Typography variant="h3" sx={{ mb: 1 }}>
                              {type.icon}
                            </Typography>
                            <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 0.5 }}>
                              {type.name}
                            </Typography>
                            <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
                              {type.description}
                            </Typography>
                            <Chip
                              size="small"
                              label={`ارتفاع: ${type.height}px`}
                              sx={{
                                bgcolor: alpha(category.color, 0.1),
                                color: category.color,
                                fontWeight: 500,
                              }}
                            />
                          </CardContent>
                        </CardActionArea>
                      </Card>
                    </Grid>
                  ))}
                </Grid>
              </Box>
            ))}
          </Stack>
        </Paper>
      )}

      {/* Basic Fields */}
      <Paper
        elevation={0}
        sx={{
          p: 4,
          border: `1px solid ${COLORS.border}`,
          borderRadius: 2,
        }}
      >
        <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
          التفاصيل الأساسية
        </Typography>
        
        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="العنوان بالعربية"
              value={formData.titleAr}
              onChange={(e) => onChange('titleAr', e.target.value)}
              placeholder="أدخل عنوان القسم بالعربية"
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <Typography sx={{ color: COLORS.text.secondary }}>عربي</Typography>
                  </InputAdornment>
                ),
              }}
            />
          </Grid>
          
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="العنوان بالإنجليزية"
              value={formData.title}
              onChange={(e) => onChange('title', e.target.value)}
              placeholder="Enter section title in English"
              dir="ltr"
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <Typography sx={{ color: COLORS.text.secondary }}>EN</Typography>
                  </InputAdornment>
                ),
              }}
            />
          </Grid>

          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="العنوان الفرعي بالعربية"
              value={formData.subtitleAr}
              onChange={(e) => onChange('subtitleAr', e.target.value)}
              placeholder="أدخل العنوان الفرعي بالعربية"
              multiline
              rows={2}
            />
          </Grid>
          
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="العنوان الفرعي بالإنجليزية"
              value={formData.subtitle}
              onChange={(e) => onChange('subtitle', e.target.value)}
              placeholder="Enter subtitle in English"
              dir="ltr"
              multiline
              rows={2}
            />
          </Grid>

          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              type="number"
              label="ترتيب القسم"
              value={formData.order}
              onChange={(e) => onChange('order', parseInt(e.target.value))}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <Typography sx={{ color: COLORS.text.secondary }}>#</Typography>
                  </InputAdornment>
                ),
              }}
              helperText="رقم ترتيب القسم في الصفحة الرئيسية"
            />
          </Grid>
          
          <Grid item xs={12} md={4}>
            <TextField
              fullWidth
              type="number"
              label="الأولوية"
              value={formData.priority}
              onChange={(e) => onChange('priority', parseInt(e.target.value))}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <StarIcon sx={{ color: COLORS.warning }} />
                  </InputAdornment>
                ),
              }}
              helperText="أولوية العرض (الأعلى يظهر أولاً)"
            />
          </Grid>
        </Grid>
      </Paper>
    </Stack>
  );
};

// Display Settings Tab
const DisplaySettingsTab: React.FC<{
  formData: any;
  onChange: (path: string, value: any) => void;
  isEdit: boolean;
}> = ({ formData, onChange }) => {
  return (
    <Stack spacing={4}>
      <Box>
        <Typography variant="h5" sx={{ fontWeight: 600, color: COLORS.text.primary, mb: 1 }}>
          إعدادات العرض
        </Typography>
        <Typography variant="body2" color="text.secondary">
          تحكم في كيفية عرض المحتوى للمستخدمين
        </Typography>
      </Box>

      <Paper elevation={0} sx={{ p: 4, border: `1px solid ${COLORS.border}`, borderRadius: 2 }}>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Typography variant="h6" sx={{ fontWeight: 600, mb: 2 }}>
              إعدادات أساسية
            </Typography>
          </Grid>

          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              type="number"
              label="الحد الأقصى للعناصر"
              value={formData.displaySettings.maxItems}
              onChange={(e) => onChange('displaySettings.maxItems', parseInt(e.target.value))}
              InputProps={{
                startAdornment: <InputAdornment position="start"><FormatSizeIcon /></InputAdornment>,
              }}
              helperText="عدد العناصر المعروضة في القسم"
            />
          </Grid>

          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="نص الشارة"
              value={formData.displaySettings.badgeText}
              onChange={(e) => onChange('displaySettings.badgeText', e.target.value)}
              disabled={!formData.displaySettings.showBadge}
              placeholder="جديد، مميز، عرض خاص"
              InputProps={{
                startAdornment: <InputAdornment position="start"><BadgeIcon /></InputAdornment>,
              }}
            />
          </Grid>

          <Grid item xs={12}>
            <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 2 }}>
              خيارات العرض
            </Typography>
            <Paper sx={{ p: 2, bgcolor: COLORS.background.light }}>
              <Grid container spacing={2}>
                <Grid item xs={12} sm={6} md={4}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={formData.displaySettings.showTitle}
                        onChange={(e) => onChange('displaySettings.showTitle', e.target.checked)}
                      />
                    }
                    label="عرض العنوان"
                  />
                </Grid>
                <Grid item xs={12} sm={6} md={4}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={formData.displaySettings.showSubtitle}
                        onChange={(e) => onChange('displaySettings.showSubtitle', e.target.checked)}
                      />
                    }
                    label="عرض العنوان الفرعي"
                  />
                </Grid>
                <Grid item xs={12} sm={6} md={4}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={formData.displaySettings.showBadge}
                        onChange={(e) => onChange('displaySettings.showBadge', e.target.checked)}
                      />
                    }
                    label="عرض الشارة"
                  />
                </Grid>
                <Grid item xs={12} sm={6} md={4}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={formData.displaySettings.showIndicators}
                        onChange={(e) => onChange('displaySettings.showIndicators', e.target.checked)}
                      />
                    }
                    label="عرض المؤشرات"
                  />
                </Grid>
                <Grid item xs={12} sm={6} md={4}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={formData.displaySettings.showViewAllButton}
                        onChange={(e) => onChange('displaySettings.showViewAllButton', e.target.checked)}
                      />
                    }
                    label="زر عرض الكل"
                  />
                </Grid>
                <Grid item xs={12} sm={6} md={4}>
                  <FormControlLabel
                    control={
                      <Switch
                        checked={formData.displaySettings.autoPlay}
                        onChange={(e) => onChange('displaySettings.autoPlay', e.target.checked)}
                      />
                    }
                    label="تشغيل تلقائي"
                  />
                </Grid>
              </Grid>
            </Paper>
          </Grid>

          {formData.displaySettings.autoPlay && (
            <Grid item xs={12}>
              <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
                مدة التشغيل التلقائي: {formData.displaySettings.autoPlayDuration / 1000} ثانية
              </Typography>
              <Slider
                value={formData.displaySettings.autoPlayDuration}
                onChange={(_, value) => onChange('displaySettings.autoPlayDuration', value)}
                min={1000}
                max={10000}
                step={500}
                marks={[
                  { value: 1000, label: '1 ثانية' },
                  { value: 5000, label: '5 ثواني' },
                  { value: 10000, label: '10 ثواني' }
                ]}
                valueLabelDisplay="auto"
                valueLabelFormat={(value) => `${value / 1000}s`}
                sx={{ color: COLORS.primary }}
              />
            </Grid>
          )}
        </Grid>
      </Paper>
    </Stack>
  );
};

// Layout Settings Tab
const LayoutSettingsTab: React.FC<{
  formData: any;
  onChange: (path: string, value: any) => void;
  isEdit: boolean;
}> = ({ formData, onChange }) => {
  return (
    <Stack spacing={4}>
      <Box>
        <Typography variant="h5" sx={{ fontWeight: 600, color: COLORS.text.primary, mb: 1 }}>
          إعدادات التخطيط
        </Typography>
        <Typography variant="body2" color="text.secondary">
          تحكم في شكل وترتيب العناصر
        </Typography>
      </Box>

      <Paper elevation={0} sx={{ p: 4, border: `1px solid ${COLORS.border}`, borderRadius: 2 }}>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Typography variant="h6" sx={{ fontWeight: 600, mb: 2 }}>
              نوع التخطيط
            </Typography>
            <ToggleButtonGroup
              value={formData.layoutSettings.layoutType}
              exclusive
              onChange={(_, value) => value && onChange('layoutSettings.layoutType', value)}
              sx={{ mb: 3 }}
            >
              {LAYOUT_TYPES.map((layout) => (
                <ToggleButton key={layout.value} value={layout.value}>
                  <Stack direction="column" spacing={0.5} alignItems="center" sx={{ p: 1 }}>
                    {layout.icon}
                    <Typography variant="caption">{layout.label}</Typography>
                  </Stack>
                </ToggleButton>
              ))}
            </ToggleButtonGroup>
          </Grid>

          {(formData.layoutSettings.layoutType === 'grid') && (
            <Grid item xs={12} md={6}>
              <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
                عدد الأعمدة: {formData.layoutSettings.columnsCount}
              </Typography>
              <Slider
                value={formData.layoutSettings.columnsCount}
                onChange={(_, value) => onChange('layoutSettings.columnsCount', value)}
                min={1}
                max={4}
                step={1}
                marks
                valueLabelDisplay="auto"
                sx={{ color: COLORS.primary }}
              />
            </Grid>
          )}

          <Grid item xs={12} md={6}>
            <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
              ارتفاع العنصر: {formData.layoutSettings.itemHeight}px
            </Typography>
            <Slider
              value={formData.layoutSettings.itemHeight}
              onChange={(_, value) => onChange('layoutSettings.itemHeight', value)}
              min={100}
              max={500}
              step={10}
              marks={[
                { value: 100, label: '100' },
                { value: 300, label: '300' },
                { value: 500, label: '500' }
              ]}
              valueLabelDisplay="auto"
              sx={{ color: COLORS.primary }}
            />
          </Grid>

          <Grid item xs={12} md={6}>
            <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
              المسافة بين العناصر: {formData.layoutSettings.itemSpacing}px
            </Typography>
            <Slider
              value={formData.layoutSettings.itemSpacing}
              onChange={(_, value) => onChange('layoutSettings.itemSpacing', value)}
              min={0}
              max={32}
              step={4}
              marks
              valueLabelDisplay="auto"
              sx={{ color: COLORS.primary }}
            />
          </Grid>

          <Grid item xs={12} md={6}>
            <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
              هامش القسم: {formData.layoutSettings.sectionPadding}px
            </Typography>
            <Slider
              value={formData.layoutSettings.sectionPadding}
              onChange={(_, value) => onChange('layoutSettings.sectionPadding', value)}
              min={0}
              max={48}
              step={8}
              marks
              valueLabelDisplay="auto"
              sx={{ color: COLORS.primary }}
            />
          </Grid>
        </Grid>
      </Paper>
    </Stack>
  );
};

// Style Settings Tab
const StyleSettingsTab: React.FC<{
  formData: any;
  onChange: (path: string, value: any) => void;
  isEdit: boolean;
}> = ({ formData, onChange }) => {
  return (
    <Stack spacing={4}>
      <Box>
        <Typography variant="h5" sx={{ fontWeight: 600, color: COLORS.text.primary, mb: 1 }}>
          الألوان والأنماط
        </Typography>
        <Typography variant="body2" color="text.secondary">
          تخصيص المظهر البصري للقسم
        </Typography>
      </Box>

      <Paper elevation={0} sx={{ p: 4, border: `1px solid ${COLORS.border}`, borderRadius: 2 }}>
        <Grid container spacing={3}>
          {/* Colors Section */}
          <Grid item xs={12}>
            <Typography variant="h6" sx={{ fontWeight: 600, mb: 2 }}>
              الألوان
            </Typography>
          </Grid>

          <Grid item xs={12} md={6}>
            <Stack spacing={2}>
              <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
                لون الخلفية
              </Typography>
              <Stack direction="row" spacing={2} alignItems="center">
                <Box
                  sx={{
                    width: 60,
                    height: 60,
                    bgcolor: formData.styleSettings.backgroundColor,
                    border: `2px solid ${COLORS.border}`,
                    borderRadius: 2,
                    cursor: 'pointer',
                  }}
                />
                <TextField
                  value={formData.styleSettings.backgroundColor}
                  onChange={(e) => onChange('styleSettings.backgroundColor', e.target.value)}
                  placeholder="#ffffff"
                  size="small"
                  sx={{ flexGrow: 1 }}
                />
              </Stack>
            </Stack>
          </Grid>

          <Grid item xs={12} md={6}>
            <Stack spacing={2}>
              <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
                لون النص
              </Typography>
              <Stack direction="row" spacing={2} alignItems="center">
                <Box
                  sx={{
                    width: 60,
                    height: 60,
                    bgcolor: formData.styleSettings.textColor,
                    border: `2px solid ${COLORS.border}`,
                    borderRadius: 2,
                    cursor: 'pointer',
                  }}
                />
                <TextField
                  value={formData.styleSettings.textColor}
                  onChange={(e) => onChange('styleSettings.textColor', e.target.value)}
                  placeholder="#000000"
                  size="small"
                  sx={{ flexGrow: 1 }}
                />
              </Stack>
            </Stack>
          </Grid>

          {/* Gradient Settings */}
          <Grid item xs={12}>
            <FormControlLabel
              control={
                <Switch
                  checked={formData.styleSettings.enableGradient}
                  onChange={(e) => onChange('styleSettings.enableGradient', e.target.checked)}
                />
              }
              label={
                <Stack direction="row" spacing={1} alignItems="center">
                  <GradientIcon />
                  <Typography>تفعيل التدرج اللوني</Typography>
                </Stack>
              }
            />
          </Grid>

          {formData.styleSettings.enableGradient && (
            <>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="اللون الأول للتدرج"
                  value={formData.styleSettings.gradientColors[0]}
                  onChange={(e) => {
                    const colors = [...formData.styleSettings.gradientColors];
                    colors[0] = e.target.value;
                    onChange('styleSettings.gradientColors', colors);
                  }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label="اللون الثاني للتدرج"
                  value={formData.styleSettings.gradientColors[1]}
                  onChange={(e) => {
                    const colors = [...formData.styleSettings.gradientColors];
                    colors[1] = e.target.value;
                    onChange('styleSettings.gradientColors', colors);
                  }}
                />
              </Grid>
            </>
          )}

          {/* Style Properties */}
          <Grid item xs={12}>
            <Typography variant="h6" sx={{ fontWeight: 600, mb: 2, mt: 2 }}>
              خصائص التصميم
            </Typography>
          </Grid>

          <Grid item xs={12} md={6}>
            <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
              استدارة الحواف: {formData.styleSettings.borderRadius}px
            </Typography>
            <Slider
              value={formData.styleSettings.borderRadius}
              onChange={(_, value) => onChange('styleSettings.borderRadius', value)}
              min={0}
              max={30}
              step={2}
              marks={[
                { value: 0, label: 'حاد' },
                { value: 15, label: 'متوسط' },
                { value: 30, label: 'دائري' }
              ]}
              valueLabelDisplay="auto"
              sx={{ color: COLORS.primary }}
            />
          </Grid>

          <Grid item xs={12} md={6}>
            <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
              الظل: {formData.styleSettings.elevation}
            </Typography>
            <Slider
              value={formData.styleSettings.elevation}
              onChange={(_, value) => onChange('styleSettings.elevation', value)}
              min={0}
              max={8}
              step={1}
              marks={[
                { value: 0, label: 'بدون' },
                { value: 4, label: 'متوسط' },
                { value: 8, label: 'عميق' }
              ]}
              valueLabelDisplay="auto"
              sx={{ color: COLORS.primary }}
            />
          </Grid>
        </Grid>
      </Paper>
    </Stack>
  );
};

// Animation Settings Tab
const AnimationSettingsTab: React.FC<{
  formData: any;
  onChange: (path: string, value: any) => void;
  isEdit: boolean;
}> = ({ formData, onChange }) => {
  return (
    <Stack spacing={4}>
      <Box>
        <Typography variant="h5" sx={{ fontWeight: 600, color: COLORS.text.primary, mb: 1 }}>
          إعدادات الحركة
        </Typography>
        <Typography variant="body2" color="text.secondary">
          أضف تأثيرات حركية لجذب الانتباه
        </Typography>
      </Box>

      <Paper elevation={0} sx={{ p: 4, border: `1px solid ${COLORS.border}`, borderRadius: 2 }}>
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <Typography variant="h6" sx={{ fontWeight: 600, mb: 2 }}>
              نوع الحركة
            </Typography>
            <Grid container spacing={2}>
              {ANIMATION_TYPES.map((animation) => (
                <Grid item xs={12} sm={6} md={4} key={animation.value}>
                  <Card
                    sx={{
                      cursor: 'pointer',
                      border: formData.animationSettings.type === animation.value 
                        ? `2px solid ${animation.color}` 
                        : `1px solid ${COLORS.border}`,
                      bgcolor: formData.animationSettings.type === animation.value 
                        ? alpha(animation.color, 0.05) 
                        : 'white',
                      transition: 'all 0.3s ease',
                      '&:hover': {
                        borderColor: animation.color,
                        transform: 'translateY(-2px)',
                      },
                    }}
                    onClick={() => onChange('animationSettings.type', animation.value)}
                  >
                    <CardActionArea>
                      <CardContent sx={{ textAlign: 'center', py: 2 }}>
                        <Box sx={{ color: animation.color, mb: 1 }}>
                          {animation.icon}
                        </Box>
                        <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
                          {animation.label}
                        </Typography>
                      </CardContent>
                    </CardActionArea>
                  </Card>
                </Grid>
              ))}
            </Grid>
          </Grid>

          {formData.animationSettings.type !== 'NONE' && (
            <>
              <Grid item xs={12} md={6}>
                <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
                  مدة الحركة: {formData.animationSettings.duration}ms
                </Typography>
                <Slider
                  value={formData.animationSettings.duration}
                  onChange={(_, value) => onChange('animationSettings.duration', value)}
                  min={100}
                  max={2000}
                  step={100}
                  marks={[
                    { value: 300, label: 'سريع' },
                    { value: 1000, label: 'متوسط' },
                    { value: 2000, label: 'بطيء' }
                  ]}
                  valueLabelDisplay="auto"
                  sx={{ color: COLORS.primary }}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
                  تأخير البداية: {formData.animationSettings.delay}ms
                </Typography>
                <Slider
                  value={formData.animationSettings.delay}
                  onChange={(_, value) => onChange('animationSettings.delay', value)}
                  min={0}
                  max={1000}
                  step={50}
                  marks={[
                    { value: 0, label: 'فوري' },
                    { value: 500, label: '0.5 ثانية' },
                    { value: 1000, label: '1 ثانية' }
                  ]}
                  valueLabelDisplay="auto"
                  sx={{ color: COLORS.primary }}
                />
              </Grid>

              <Grid item xs={12}>
                <FormControl fullWidth>
                  <InputLabel>منحنى الحركة</InputLabel>
                  <Select
                    value={formData.animationSettings.curve}
                    onChange={(e) => onChange('animationSettings.curve', e.target.value)}
                    label="منحنى الحركة"
                  >
                    <MenuItem value="linear">خطي</MenuItem>
                    <MenuItem value="ease">سلس</MenuItem>
                    <MenuItem value="ease-in">بداية بطيئة</MenuItem>
                    <MenuItem value="ease-out">نهاية بطيئة</MenuItem>
                    <MenuItem value="ease-in-out">بداية ونهاية بطيئة</MenuItem>
                  </Select>
                </FormControl>
              </Grid>
            </>
          )}
        </Grid>
      </Paper>
    </Stack>
  );
};

// Scheduling Tab
const SchedulingTab: React.FC<{
  formData: any;
  onChange: (path: string, value: any) => void;
  isEdit: boolean;
}> = ({ formData, onChange }) => {
  return (
    <Stack spacing={4}>
      <Box>
        <Typography variant="h5" sx={{ fontWeight: 600, color: COLORS.text.primary, mb: 1 }}>
          الجدولة والاستهداف
        </Typography>
        <Typography variant="body2" color="text.secondary">
          حدد توقيت العرض والجمهور المستهدف
        </Typography>
      </Box>

      {/* Scheduling Section */}
      <Paper elevation={0} sx={{ p: 4, border: `1px solid ${COLORS.border}`, borderRadius: 2 }}>
        <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
          <Stack direction="row" spacing={1} alignItems="center">
            <CalendarIcon />
            <span>جدولة العرض</span>
          </Stack>
        </Typography>
        
        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <DateTimePicker
              label="تاريخ البداية"
              value={formData.scheduledAt}
              onChange={(value) => onChange('scheduledAt', value)}
              slotProps={{
                textField: { 
                  fullWidth: true,
                  helperText: 'متى يبدأ عرض هذا القسم'
                }
              }}
            />
          </Grid>
          
          <Grid item xs={12} md={6}>
            <DateTimePicker
              label="تاريخ الانتهاء"
              value={formData.expiresAt}
              onChange={(value) => onChange('expiresAt', value)}
              slotProps={{
                textField: { 
                  fullWidth: true,
                  helperText: 'متى ينتهي عرض هذا القسم'
                }
              }}
            />
          </Grid>
        </Grid>
      </Paper>

      {/* Target Audience Section */}
      <Paper elevation={0} sx={{ p: 4, border: `1px solid ${COLORS.border}`, borderRadius: 2 }}>
        <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
          <Stack direction="row" spacing={1} alignItems="center">
            <PeopleIcon />
            <span>الجمهور المستهدف</span>
          </Stack>
        </Typography>
        
        <Grid container spacing={2}>
          {TARGET_AUDIENCES.map((audience) => (
            <Grid item xs={12} sm={6} md={4} key={audience.value}>
              <Card
                sx={{
                  cursor: 'pointer',
                  border: formData.targetAudience.includes(audience.value)
                    ? `2px solid ${audience.color}`
                    : `1px solid ${COLORS.border}`,
                  bgcolor: formData.targetAudience.includes(audience.value)
                    ? alpha(audience.color, 0.05)
                    : 'white',
                  transition: 'all 0.3s ease',
                  '&:hover': {
                    borderColor: audience.color,
                  },
                }}
                onClick={() => {
                  const current = formData.targetAudience || [];
                  const newAudience = current.includes(audience.value)
                    ? current.filter((a: string) => a !== audience.value)
                    : [...current, audience.value];
                  onChange('targetAudience', newAudience);
                }}
              >
                <CardContent sx={{ p: 2 }}>
                  <Stack direction="row" alignItems="center" spacing={1}>
                    <Checkbox
                      checked={formData.targetAudience.includes(audience.value)}
                      sx={{ p: 0 }}
                    />
                    <Box sx={{ color: audience.color }}>
                      {audience.icon}
                    </Box>
                    <Typography variant="body2" sx={{ fontWeight: 500 }}>
                      {audience.label}
                    </Typography>
                  </Stack>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      </Paper>
    </Stack>
  );
};

// Advanced Settings Tab
const AdvancedSettingsTab: React.FC<{
  formData: any;
  onChange: (path: string, value: any) => void;
  isEdit: boolean;
}> = ({ formData, onChange }) => {
  return (
    <Stack spacing={4}>
      <Box>
        <Typography variant="h5" sx={{ fontWeight: 600, color: COLORS.text.primary, mb: 1 }}>
          الإعدادات المتقدمة
        </Typography>
        <Typography variant="body2" color="text.secondary">
          إعدادات متقدمة للأداء والتخزين المؤقت
        </Typography>
      </Box>

      {/* Cache Settings */}
      <Paper elevation={0} sx={{ p: 4, border: `1px solid ${COLORS.border}`, borderRadius: 2 }}>
        <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
          <Stack direction="row" spacing={1} alignItems="center">
            <CloudSyncIcon />
            <span>التخزين المؤقت</span>
          </Stack>
        </Typography>
        
        <Grid container spacing={3}>
          <Grid item xs={12}>
            <FormControlLabel
              control={
                <Switch
                  checked={formData.cacheSettings.enableCache}
                  onChange={(e) => onChange('cacheSettings.enableCache', e.target.checked)}
                />
              }
              label="تفعيل التخزين المؤقت"
            />
          </Grid>

          {formData.cacheSettings.enableCache && (
            <>
              <Grid item xs={12} md={6}>
                <Typography variant="subtitle2" sx={{ fontWeight: 600, mb: 1 }}>
                  مدة التخزين: {Math.round(formData.cacheSettings.maxAge / 60000)} دقيقة
                </Typography>
                <Slider
                  value={formData.cacheSettings.maxAge / 60000}
                  onChange={(_, value) => onChange('cacheSettings.maxAge', (value as number) * 60000)}
                  min={5}
                  max={1440}
                  step={5}
                  marks={[
                    { value: 5, label: '5 دقائق' },
                    { value: 60, label: 'ساعة' },
                    { value: 1440, label: '24 ساعة' }
                  ]}
                  valueLabelDisplay="auto"
                  valueLabelFormat={(value) => `${value} دقيقة`}
                  sx={{ color: COLORS.primary }}
                />
              </Grid>

              <Grid item xs={12} md={6}>
                <FormControlLabel
                  control={
                    <Switch
                      checked={formData.cacheSettings.cacheImages}
                      onChange={(e) => onChange('cacheSettings.cacheImages', e.target.checked)}
                    />
                  }
                  label="تخزين الصور"
                />
              </Grid>
            </>
          )}
        </Grid>
      </Paper>

      {/* Behavior Settings */}
      <Paper elevation={0} sx={{ p: 4, border: `1px solid ${COLORS.border}`, borderRadius: 2 }}>
        <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
          <Stack direction="row" spacing={1} alignItems="center">
            <TouchIcon />
            <span>السلوك والتفاعل</span>
          </Stack>
        </Typography>
        
        <Grid container spacing={3}>
          <Grid item xs={12} sm={6}>
            <FormControlLabel
              control={
                <Switch
                  checked={formData.behaviorSettings.infiniteScroll}
                  onChange={(e) => onChange('behaviorSettings.infiniteScroll', e.target.checked)}
                />
              }
              label="التمرير اللانهائي"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <FormControlLabel
              control={
                <Switch
                  checked={formData.behaviorSettings.enablePullToRefresh}
                  onChange={(e) => onChange('behaviorSettings.enablePullToRefresh', e.target.checked)}
                />
              }
              label="السحب للتحديث"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <FormControlLabel
              control={
                <Switch
                  checked={formData.behaviorSettings.lazy}
                  onChange={(e) => onChange('behaviorSettings.lazy', e.target.checked)}
                />
              }
              label="التحميل الكسول"
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <FormControlLabel
              control={
                <Switch
                  checked={formData.behaviorSettings.clickable}
                  onChange={(e) => onChange('behaviorSettings.clickable', e.target.checked)}
                />
              }
              label="قابل للنقر"
            />
          </Grid>
        </Grid>
      </Paper>
    </Stack>
  );
};

// Helper function to calculate completion percentage
const calculateCompletionPercentage = (formData: any): number => {
  let completed = 0;
  let total = 0;

  // Check basic info
  if (formData.sectionType) completed++;
  total++;
  
  if (formData.titleAr || formData.title) completed++;
  total++;
  
  // Check content
  if (formData.content && formData.content.length > 0) completed++;
  total++;
  
  // Check display settings
  if (formData.displaySettings.maxItems > 0) completed++;
  total++;
  
  return Math.round((completed / total) * 100);
};

export default SectionConfigDialog;
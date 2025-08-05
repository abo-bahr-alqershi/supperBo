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
  Accordion,
  AccordionSummary,
  AccordionDetails,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  Card,
  CardContent,
  Stack,
  Alert,
  Tooltip,
  FormHelperText,
  Autocomplete,
  RadioGroup,
  Radio,
  FormLabel,
  Checkbox,
  FormGroup
} from '@mui/material';
import {
  ExpandMore as ExpandMoreIcon,
  Add as AddIcon,
  Delete as DeleteIcon,
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
  Language as LanguageIcon,
  Style as StyleIcon,
  Speed as SpeedIcon,
  ViewModule as LayoutIcon,
} from '@mui/icons-material';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { ColorPicker as MuiColorPicker } from 'mui-color-picker';
import type { DynamicHomeSection } from '../../types/homeSections.types';

// Section Types organized by category
const SECTION_TYPES = [
  {
    category: 'الإعلانات المدعومة',
    color: '#4CAF50',
    types: [
      { type: 'SINGLE_PROPERTY_AD', name: 'إعلان عقار واحد', icon: '🏠', height: 320 },
      { type: 'FEATURED_PROPERTY_AD', name: 'إعلان عقار مميز', icon: '⭐', height: 400 },
      { type: 'MULTI_PROPERTY_AD', name: 'إعلان متعدد العقارات', icon: '🏘️', height: 280 },
      { type: 'UNIT_SHOWCASE_AD', name: 'عرض وحدة سكنية', icon: '🏢', height: 450 }
    ]
  },
  {
    category: 'العروض الخاصة',
    color: '#FF9800',
    types: [
      { type: 'SINGLE_PROPERTY_OFFER', name: 'عرض عقار واحد', icon: '💰', height: 200 },
      { type: 'LIMITED_TIME_OFFER', name: 'عرض محدود المدة', icon: '⏰', height: 220 },
      { type: 'SEASONAL_OFFER', name: 'عروض موسمية', icon: '🎄', height: 240 },
      { type: 'MULTI_PROPERTY_OFFERS_GRID', name: 'شبكة عروض متعددة', icon: '📊', height: 400 },
      { type: 'OFFERS_CAROUSEL', name: 'دوار العروض', icon: '🎠', height: 260 },
      { type: 'FLASH_DEALS', name: 'صفقات برق', icon: '⚡', height: 280 }
    ]
  },
  {
    category: 'قوائم العقارات',
    color: '#2196F3',
    types: [
      { type: 'HORIZONTAL_PROPERTY_LIST', name: 'قائمة أفقية', icon: '➡️', height: 280 },
      { type: 'VERTICAL_PROPERTY_GRID', name: 'شبكة عمودية', icon: '⬇️', height: 400 },
      { type: 'MIXED_LAYOUT_LIST', name: 'قائمة تخطيط مختلط', icon: '🔀', height: 500 },
      { type: 'COMPACT_PROPERTY_LIST', name: 'قائمة مضغوطة', icon: '📋', height: 180 },
      { type: 'FEATURED_PROPERTIES_SHOWCASE', name: 'عرض العقارات المميزة', icon: '🌟', height: 360 }
    ]
  },
  {
    category: 'الوجهات',
    color: '#9C27B0',
    types: [
      { type: 'CITY_CARDS_GRID', name: 'شبكة بطاقات المدن', icon: '🏙️', height: 200 },
      { type: 'DESTINATION_CAROUSEL', name: 'دوار الوجهات', icon: '🗺️', height: 240 },
      { type: 'EXPLORE_CITIES', name: 'استكشاف المدن', icon: '🧭', height: 320 }
    ]
  },
  {
    category: 'الدوارات المتميزة',
    color: '#E91E63',
    types: [
      { type: 'PREMIUM_CAROUSEL', name: 'دوار متميز', icon: '💎', height: 380 },
      { type: 'INTERACTIVE_SHOWCASE', name: 'عرض تفاعلي', icon: '🎯', height: 420 }
    ]
  }
];

// Animation types
const ANIMATION_TYPES = [
  { value: 'NONE', label: 'بدون رسوم متحركة', icon: '⏸️', duration: 0 },
  { value: 'FADE', label: 'ظهور تدريجي', icon: '🌅', duration: 300 },
  { value: 'SLIDE', label: 'انزلاق', icon: '🎢', duration: 300 },
  { value: 'SLIDE_UP', label: 'انزلاق لأعلى', icon: '⬆️', duration: 300 },
  { value: 'SLIDE_DOWN', label: 'انزلاق لأسفل', icon: '⬇️', duration: 300 },
  { value: 'SLIDE_LEFT', label: 'انزلاق يسار', icon: '⬅️', duration: 300 },
  { value: 'SLIDE_RIGHT', label: 'انزلاق يمين', icon: '➡️', duration: 300 },
  { value: 'SCALE', label: 'تحجيم', icon: '🔍', duration: 300 },
  { value: 'ZOOM_IN', label: 'تكبير', icon: '🔍', duration: 300 },
  { value: 'ZOOM_OUT', label: 'تصغير', icon: '🔍', duration: 300 },
  { value: 'ROTATE', label: 'دوران', icon: '🔄', duration: 400 },
  { value: 'PARALLAX', label: 'رسوم متوازية', icon: '📐', duration: 500 },
  { value: 'SHIMMER', label: 'لمعان', icon: '✨', duration: 1000 },
  { value: 'PULSE', label: 'نبض', icon: '💓', duration: 800 },
  { value: 'BOUNCE', label: 'ارتداد', icon: '🏀', duration: 600 },
  { value: 'FLIP', label: 'انقلاب', icon: '🔃', duration: 400 },
  { value: 'ELASTIC', label: 'مرونة', icon: '🎯', duration: 500 },
  { value: 'FADE_SLIDE', label: 'ظهور مع انزلاق', icon: '🌄', duration: 400 }
];

// Section sizes
const SECTION_SIZES = [
  { value: 'COMPACT', label: 'مضغوط (50%)', factor: 0.5 },
  { value: 'SMALL', label: 'صغير (75%)', factor: 0.75 },
  { value: 'MEDIUM', label: 'متوسط (100%)', factor: 1.0 },
  { value: 'LARGE', label: 'كبير (125%)', factor: 1.25 },
  { value: 'EXTRA_LARGE', label: 'كبير جداً (150%)', factor: 1.5 },
  { value: 'FULL_SCREEN', label: 'ملء الشاشة (200%)', factor: 2.0 }
];

// Layout types
const LAYOUT_TYPES = [
  { value: 'horizontal', label: 'أفقي', icon: '➡️' },
  { value: 'vertical', label: 'عمودي', icon: '⬇️' },
  { value: 'grid', label: 'شبكة', icon: '⚏' },
  { value: 'mixed', label: 'مختلط', icon: '🔀' },
  { value: 'carousel', label: 'دوار', icon: '🎠' },
  { value: 'list', label: 'قائمة', icon: '📋' }
];

// Target audiences
const TARGET_AUDIENCES = [
  { value: 'all', label: 'جميع المستخدمين', icon: '👥' },
  { value: 'new_users', label: 'المستخدمون الجدد', icon: '🆕' },
  { value: 'returning_users', label: 'المستخدمون العائدون', icon: '🔄' },
  { value: 'premium_users', label: 'المستخدمون المميزون', icon: '💎' },
  { value: 'mobile_users', label: 'مستخدمو الموبايل', icon: '📱' },
  { value: 'web_users', label: 'مستخدمو الويب', icon: '💻' },
  { value: 'arabic_users', label: 'المستخدمون العرب', icon: '🇸🇦' },
  { value: 'english_users', label: 'المستخدمون الإنجليز', icon: '🇬🇧' }
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
  const [currentTab, setCurrentTab] = useState(0);
  const [formData, setFormData] = useState<any>({
    // Basic Info
    sectionType: '',
    order: 1,
    title: '',
    subtitle: '',
    titleAr: '',
    subtitleAr: '',
    priority: 0,
    
    // Scheduling
    scheduledAt: null,
    expiresAt: null,
    
    // Targeting
    targetAudience: ['all'],
    
    // Display Settings
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
    
    // Layout Settings
    layoutSettings: {
      layoutType: 'horizontal',
      columnsCount: 2,
      itemHeight: 200,
      itemSpacing: 16,
      sectionPadding: 16,
      sectionSize: 'MEDIUM'
    },
    
    // Style Settings
    styleSettings: {
      backgroundColor: '#ffffff',
      textColor: '#000000',
      borderRadius: 8,
      elevation: 2,
      enableGradient: false,
      gradientColors: ['#ffffff', '#f5f5f5'],
      customCSS: ''
    },
    
    // Animation Settings
    animationSettings: {
      type: 'NONE',
      duration: 300,
      delay: 0,
      curve: 'ease-in-out',
      reverse: false,
      autoStart: true,
      repeatCount: 1
    },
    
    // Behavior Settings
    behaviorSettings: {
      infiniteScroll: false,
      enablePullToRefresh: true,
      lazy: true,
      preloadNext: true,
      enableSwipeGestures: true,
      clickable: true
    },
    
    // Cache Settings
    cacheSettings: {
      enableCache: true,
      maxAge: 3600000, // 1 hour
      cacheImages: true,
      cacheData: true,
      refreshInterval: 300000 // 5 minutes
    },
    
    // Custom Data
    customData: {},
    metadata: {}
  });

  // Initialize form data when section changes
  useEffect(() => {
    if (section && isEdit) {
      setFormData({
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
        displaySettings: {
          ...formData.displaySettings,
          ...(section.sectionConfig?.displaySettings || {})
        },
        layoutSettings: {
          ...formData.layoutSettings,
          ...(section.sectionConfig?.layoutSettings || {})
        },
        styleSettings: {
          ...formData.styleSettings,
          ...(section.sectionConfig?.styleSettings || {})
        },
        animationSettings: {
          ...formData.animationSettings,
          ...(section.sectionConfig?.animationSettings || {})
        },
        behaviorSettings: {
          ...formData.behaviorSettings,
          ...(section.sectionConfig?.behaviorSettings || {})
        },
        cacheSettings: {
          ...formData.cacheSettings,
          ...(section.sectionConfig?.cacheSettings || {})
        },
        customData: section.sectionConfig?.customData || {},
        metadata: section.metadata || {}
      });
    } else if (!isEdit) {
      // Reset form for new section
      setFormData({
        ...formData,
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
        customData: {},
        metadata: {}
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
      sectionType: formData.sectionType,
      order: formData.order,
      title: formData.title,
      subtitle: formData.subtitle,
      titleAr: formData.titleAr,
      subtitleAr: formData.subtitleAr,
      priority: formData.priority,
      scheduledAt: formData.scheduledAt?.toISOString(),
      expiresAt: formData.expiresAt?.toISOString(),
      targetAudience: formData.targetAudience,
      sectionConfig: {
        displaySettings: formData.displaySettings,
        layoutSettings: formData.layoutSettings,
        styleSettings: formData.styleSettings,
        animationSettings: formData.animationSettings,
        behaviorSettings: formData.behaviorSettings,
        cacheSettings: formData.cacheSettings,
        customData: formData.customData
      },
      metadata: formData.metadata
    };
    
    onSave(sectionData);
  };

  const selectedSectionType = SECTION_TYPES
    .flatMap(cat => cat.types)
    .find(type => type.type === formData.sectionType);

  const tabLabels = [
    { label: 'المعلومات الأساسية', icon: <SettingsIcon /> },
    { label: 'إعدادات العرض', icon: <VisibilityIcon /> },
    { label: 'التخطيط والتنسيق', icon: <LayoutIcon /> },
    { label: 'الألوان والأنماط', icon: <PaletteIcon /> },
    { label: 'الرسوم المتحركة', icon: <AnimationIcon /> },
    { label: 'السلوك والتفاعل', icon: <SpeedIcon /> },
    { label: 'الجدولة والاستهداف', icon: <ScheduleIcon /> },
    { label: 'إعدادات متقدمة', icon: <CodeIcon /> }
  ];

  return (
    <Dialog 
      open={open} 
      onClose={onClose} 
      maxWidth="xl" 
      fullWidth
      PaperProps={{
        sx: { height: '90vh' }
      }}
    >
      <DialogTitle sx={{ pb: 1 }}>
        <Stack direction="row" alignItems="center" spacing={2}>
          <Typography variant="h5" sx={{ fontWeight: 'bold' }}>
            {isEdit ? 'تعديل القسم' : 'إضافة قسم جديد'}
          </Typography>
          {selectedSectionType && (
            <Chip
              label={selectedSectionType.name}
              color="primary"
              icon={<span style={{ fontSize: '1.2em' }}>{selectedSectionType.icon}</span>}
              sx={{ ml: 2 }}
            />
          )}
        </Stack>
      </DialogTitle>

      <DialogContent sx={{ p: 0 }}>
        <Box sx={{ display: 'flex', height: '100%' }}>
          {/* Sidebar Tabs */}
          <Box sx={{ width: 280, borderRight: 1, borderColor: 'divider', bgcolor: '#fafafa' }}>
            <Tabs
              orientation="vertical"
              value={currentTab}
              onChange={(_, newValue) => setCurrentTab(newValue)}
              sx={{ p: 1 }}
            >
              {tabLabels.map((tab, index) => (
                <Tab
                  key={index}
                  icon={tab.icon}
                  label={tab.label}
                  iconPosition="start"
                  sx={{
                    minHeight: 60,
                    alignItems: 'flex-start',
                    textAlign: 'left',
                    '& .MuiTab-iconWrapper': {
                      mb: 0,
                      mr: 1
                    }
                  }}
                />
              ))}
            </Tabs>
          </Box>

          {/* Tab Content */}
          <Box sx={{ flexGrow: 1, p: 3, overflow: 'auto' }}>
            {/* Basic Information Tab */}
            {currentTab === 0 && (
              <BasicInformationTab
                formData={formData}
                onChange={handleInputChange}
                isEdit={isEdit}
              />
            )}

            {/* Display Settings Tab */}
            {currentTab === 1 && (
              <DisplaySettingsTab
                formData={formData}
                onChange={handleInputChange}
              />
            )}

            {/* Layout Settings Tab */}
            {currentTab === 2 && (
              <LayoutSettingsTab
                formData={formData}
                onChange={handleInputChange}
              />
            )}

            {/* Style Settings Tab */}
            {currentTab === 3 && (
              <StyleSettingsTab
                formData={formData}
                onChange={handleInputChange}
              />
            )}

            {/* Animation Settings Tab */}
            {currentTab === 4 && (
              <AnimationSettingsTab
                formData={formData}
                onChange={handleInputChange}
              />
            )}

            {/* Behavior Settings Tab */}
            {currentTab === 5 && (
              <BehaviorSettingsTab
                formData={formData}
                onChange={handleInputChange}
              />
            )}

            {/* Scheduling and Targeting Tab */}
            {currentTab === 6 && (
              <SchedulingTargetingTab
                formData={formData}
                onChange={handleInputChange}
              />
            )}

            {/* Advanced Settings Tab */}
            {currentTab === 7 && (
              <AdvancedSettingsTab
                formData={formData}
                onChange={handleInputChange}
              />
            )}
          </Box>
        </Box>
      </DialogContent>

      <DialogActions sx={{ p: 2, borderTop: 1, borderColor: 'divider' }}>
        <Stack direction="row" spacing={2} sx={{ width: '100%' }}>
          <Button
            startIcon={<PreviewIcon />}
            variant="outlined"
            onClick={() => {/* Preview functionality */}}
          >
            معاينة
          </Button>
          
          <Box sx={{ flexGrow: 1 }} />
          
          <Button onClick={onClose} size="large">
            إلغاء
          </Button>
          <Button 
            variant="contained" 
            onClick={handleSave}
            size="large"
            disabled={!formData.sectionType}
          >
            {isEdit ? 'تحديث القسم' : 'إنشاء القسم'}
          </Button>
        </Stack>
      </DialogActions>
    </Dialog>
  );
};

// Basic Information Tab Component
const BasicInformationTab: React.FC<{
  formData: any;
  onChange: (path: string, value: any) => void;
  isEdit: boolean;
}> = ({ formData, onChange, isEdit }) => {
  return (
    <Stack spacing={3}>
      <Typography variant="h6" sx={{ fontWeight: 'bold', display: 'flex', alignItems: 'center' }}>
        <SettingsIcon sx={{ mr: 1 }} />
        المعلومات الأساسية
      </Typography>

      {/* Section Type Selection */}
      {!isEdit && (
        <Paper sx={{ p: 3 }}>
          <Typography variant="subtitle1" sx={{ fontWeight: 'bold', mb: 2 }}>
            نوع القسم
          </Typography>
          <Grid container spacing={2}>
            {SECTION_TYPES.map((category) => (
              <Grid item xs={12} key={category.category}>
                <Typography variant="subtitle2" sx={{ fontWeight: 'bold', mb: 1, color: category.color }}>
                  {category.category}
                </Typography>
                <Grid container spacing={1}>
                  {category.types.map((type) => (
                    <Grid item xs={12} sm={6} md={4} key={type.type}>
                      <Card 
                        sx={{ 
                          cursor: 'pointer',
                          border: formData.sectionType === type.type ? 2 : 1,
                          borderColor: formData.sectionType === type.type ? category.color : 'divider',
                          '&:hover': { boxShadow: 2 }
                        }}
                        onClick={() => onChange('sectionType', type.type)}
                      >
                        <CardContent sx={{ textAlign: 'center', py: 2 }}>
                          <Typography variant="h4" sx={{ mb: 1 }}>
                            {type.icon}
                          </Typography>
                          <Typography variant="body2" sx={{ fontWeight: 'bold' }}>
                            {type.name}
                          </Typography>
                          <Typography variant="caption" color="text.secondary">
                            ارتفاع: {type.height}px
                          </Typography>
                        </CardContent>
                      </Card>
                    </Grid>
                  ))}
                </Grid>
              </Grid>
            ))}
          </Grid>
        </Paper>
      )}

      {/* Basic Fields */}
      <Paper sx={{ p: 3 }}>
        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="العنوان (إنجليزي)"
              value={formData.title}
              onChange={(e) => onChange('title', e.target.value)}
              placeholder="أدخل عنوان القسم"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="العنوان (عربي)"
              value={formData.titleAr}
              onChange={(e) => onChange('titleAr', e.target.value)}
              placeholder="أدخل عنوان القسم بالعربية"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="العنوان الفرعي (إنجليزي)"
              value={formData.subtitle}
              onChange={(e) => onChange('subtitle', e.target.value)}
              placeholder="أدخل العنوان الفرعي"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              label="العنوان الفرعي (عربي)"
              value={formData.subtitleAr}
              onChange={(e) => onChange('subtitleAr', e.target.value)}
              placeholder="أدخل العنوان الفرعي بالعربية"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              type="number"
              label="ترتيب القسم"
              value={formData.order}
              onChange={(e) => onChange('order', parseInt(e.target.value))}
              helperText="رقم ترتيب القسم في الشاشة الرئيسية"
            />
          </Grid>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              type="number"
              label="الأولوية"
              value={formData.priority}
              onChange={(e) => onChange('priority', parseInt(e.target.value))}
              helperText="أولوية عرض القسم (رقم أعلى = أولوية أكبر)"
            />
          </Grid>
        </Grid>
      </Paper>
    </Stack>
  );
};

// Display Settings Tab Component
const DisplaySettingsTab: React.FC<{
  formData: any;
  onChange: (path: string, value: any) => void;
}> = ({ formData, onChange }) => {
  return (
    <Stack spacing={3}>
      <Typography variant="h6" sx={{ fontWeight: 'bold', display: 'flex', alignItems: 'center' }}>
        <VisibilityIcon sx={{ mr: 1 }} />
        إعدادات العرض
      </Typography>

      <Paper sx={{ p: 3 }}>
        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <TextField
              fullWidth
              type="number"
              label="الحد الأقصى للعناصر"
              value={formData.displaySettings.maxItems}
              onChange={(e) => onChange('displaySettings.maxItems', parseInt(e.target.value))}
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
            />
          </Grid>

          <Grid item xs={12}>
            <FormGroup>
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.displaySettings.showTitle}
                    onChange={(e) => onChange('displaySettings.showTitle', e.target.checked)}
                  />
                }
                label="عرض العنوان"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.displaySettings.showSubtitle}
                    onChange={(e) => onChange('displaySettings.showSubtitle', e.target.checked)}
                  />
                }
                label="عرض العنوان الفرعي"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.displaySettings.showBadge}
                    onChange={(e) => onChange('displaySettings.showBadge', e.target.checked)}
                  />
                }
                label="عرض الشارة"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.displaySettings.showIndicators}
                    onChange={(e) => onChange('displaySettings.showIndicators', e.target.checked)}
                  />
                }
                label="عرض المؤشرات"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.displaySettings.showViewAllButton}
                    onChange={(e) => onChange('displaySettings.showViewAllButton', e.target.checked)}
                  />
                }
                label="عرض زر (عرض الكل)"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.displaySettings.autoPlay}
                    onChange={(e) => onChange('displaySettings.autoPlay', e.target.checked)}
                  />
                }
                label="التشغيل التلقائي"
              />
            </FormGroup>
          </Grid>

          {formData.displaySettings.autoPlay && (
            <Grid item xs={12} md={6}>
              <Typography variant="body2" sx={{ mb: 1 }}>
                مدة التشغيل التلقائي: {formData.displaySettings.autoPlayDuration}ms
              </Typography>
              <Slider
                value={formData.displaySettings.autoPlayDuration}
                onChange={(_, value) => onChange('displaySettings.autoPlayDuration', value)}
                min={1000}
                max={10000}
                step={500}
                marks={[
                  { value: 1000, label: '1s' },
                  { value: 3000, label: '3s' },
                  { value: 5000, label: '5s' },
                  { value: 10000, label: '10s' }
                ]}
                valueLabelDisplay="auto"
                valueLabelFormat={(value) => `${value / 1000}s`}
              />
            </Grid>
          )}
        </Grid>
      </Paper>
    </Stack>
  );
};

// Layout Settings Tab Component
const LayoutSettingsTab: React.FC<{ formData: any; onChange: (path: string, value: any) => void; }> = ({ formData, onChange }) => {
  return (
    <Stack spacing={3}>
      <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
        إعدادات التخطيط
      </Typography>
      {/* Layout settings UI */}
    </Stack>
  );
};

// Style Settings Tab Component
const StyleSettingsTab: React.FC<{ formData: any; onChange: (path: string, value: any) => void; }> = ({ formData, onChange }) => {
  return (
    <Stack spacing={3}>
      <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
        إعدادات النمط
      </Typography>
      {/* Style settings UI */}
    </Stack>
  );
};

// Animation Settings Tab Component
const AnimationSettingsTab: React.FC<{ formData: any; onChange: (path: string, value: any) => void; }> = ({ formData, onChange }) => {
  return (
    <Stack spacing={3}>
      <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
        إعدادات الرسوم المتحركة
      </Typography>
      {/* Animation settings UI */}
    </Stack>
  );
};

// Behavior Settings Tab Component
const BehaviorSettingsTab: React.FC<{ formData: any; onChange: (path: string, value: any) => void; }> = ({ formData, onChange }) => {
  return (
    <Stack spacing={3}>
      <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
        إعدادات السلوك
      </Typography>
      {/* Behavior settings UI */}
    </Stack>
  );
};

// Scheduling and Targeting Tab Component
const SchedulingTargetingTab: React.FC<{ formData: any; onChange: (path: string, value: any) => void; }> = ({ formData, onChange }) => {
  return (
    <Stack spacing={3}>
      <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
        إعدادات الجدولة والاستهداف
      </Typography>
      {/* Scheduling and targeting UI */}
    </Stack>
  );
};

// Advanced Settings Tab Component
const AdvancedSettingsTab: React.FC<{ formData: any; onChange: (path: string, value: any) => void; }> = ({ formData, onChange }) => {
  return (
    <Stack spacing={3}>
      <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
        الإعدادات المتقدمة
      </Typography>
      {/* Advanced settings UI */}
    </Stack>
  );
};

export default SectionConfigDialog;
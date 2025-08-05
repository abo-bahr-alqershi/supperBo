import React, { useState, useEffect, useMemo } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Grid,
  Switch,
  FormControlLabel,
  Chip,
  IconButton,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Tabs,
  Tab,
  Alert,
  Divider,
  Stack,
  Paper,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Badge,
  Tooltip,
  CircularProgress,
  Avatar,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  Menu,
  MenuList,
  Snackbar,
  Fab,
  LinearProgress,
  SpeedDial,
  SpeedDialAction,
  SpeedDialIcon,
  Drawer,
  AppBar,
  Toolbar,
  Container,
  Skeleton
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
  DragHandle as DragHandleIcon,
  Preview as PreviewIcon,
  Settings as SettingsIcon,
  Analytics as AnalyticsIcon,
  Schedule as ScheduleIcon,
  ExpandMore as ExpandMoreIcon,
  MoreVert as MoreVertIcon,
  ContentCopy as CopyIcon,
  Refresh as RefreshIcon,
  Save as SaveIcon,
  Cancel as CancelIcon,
  PlayArrow as PlayIcon,
  Pause as PauseIcon,
  Stop as StopIcon,
  Tune as TuneIcon,
  FilterList as FilterIcon,
  Search as SearchIcon,
  Sort as SortIcon,
  ViewModule as GridViewIcon,
  ViewList as ListViewIcon,
  CloudUpload as UploadIcon,
  Download as DownloadIcon,
  History as HistoryIcon,
  Timeline as TimelineIcon,
  TrendingUp as TrendingUpIcon,
  Dashboard as DashboardIcon,
  Home as HomeIcon,
  Phone as MobileIcon,
  Computer as WebIcon,
  Language as LanguageIcon,
  Palette as PaletteIcon,
  Animation as AnimationIcon
} from '@mui/icons-material';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { useDynamicHomeSections, useCityDestinations, useSponsoredAds, useHomeConfig } from '../../hooks/useDynamicSections';
import { useCreateDynamicSection, useUpdateDynamicSection, useDeleteDynamicSection, useToggleDynamicSection, useReorderDynamicSections } from '../../hooks/useDynamicSections';
import type { DynamicHomeSection, SectionType, SectionAnimation, SectionSize, DynamicHomeConfig } from '../../types/homeSections.types';

// Import existing display components
import DynamicSection from '../../components/home/DynamicSection';
import HomeScreenLayout from '../../components/home/HomeScreenLayout';
import SectionConfigDialog from '../../components/admin/SectionConfigDialog';

// قائمة أنواع الأقسام مع الوصف والأيقونات
const SECTION_TYPES = [
  {
    category: 'الإعلانات المدعومة',
    color: '#4CAF50',
    types: [
      { type: 'SINGLE_PROPERTY_AD', name: 'إعلان عقار واحد', icon: '🏠', description: 'عرض عقار واحد بشكل مميز' },
      { type: 'FEATURED_PROPERTY_AD', name: 'إعلان عقار مميز', icon: '⭐', description: 'عرض عقار مميز مع تأثيرات بصرية' },
      { type: 'MULTI_PROPERTY_AD', name: 'إعلان متعدد العقارات', icon: '🏘️', description: 'عرض مجموعة من العقارات' },
      { type: 'UNIT_SHOWCASE_AD', name: 'عرض وحدة سكنية', icon: '🏢', description: 'عرض تفصيلي لوحدة سكنية' }
    ]
  },
  {
    category: 'العروض الخاصة',
    color: '#FF9800',
    types: [
      { type: 'SINGLE_PROPERTY_OFFER', name: 'عرض عقار واحد', icon: '💰', description: 'عرض خاص لعقار واحد' },
      { type: 'LIMITED_TIME_OFFER', name: 'عرض محدود المدة', icon: '⏰', description: 'عرض لفترة محدودة مع عداد تنازلي' },
      { type: 'SEASONAL_OFFER', name: 'عروض موسمية', icon: '🎄', description: 'عروض خاصة بالمواسم' },
      { type: 'MULTI_PROPERTY_OFFERS_GRID', name: 'شبكة عروض متعددة', icon: '📊', description: 'شبكة من العروض المتنوعة' },
      { type: 'OFFERS_CAROUSEL', name: 'دوار العروض', icon: '🎠', description: 'دوار يعرض العروض بشكل متتالي' },
      { type: 'FLASH_DEALS', name: 'صفقات برق', icon: '⚡', description: 'صفقات سريعة ومحدودة الوقت' }
    ]
  },
  {
    category: 'قوائم العقارات',
    color: '#2196F3',
    types: [
      { type: 'HORIZONTAL_PROPERTY_LIST', name: 'قائمة أفقية', icon: '➡️', description: 'قائمة عقارات أفقية قابلة للتمرير' },
      { type: 'VERTICAL_PROPERTY_GRID', name: 'شبكة عمودية', icon: '⬇️', description: 'شبكة عقارات عمودية' },
      { type: 'MIXED_LAYOUT_LIST', name: 'قائمة تخطيط مختلط', icon: '🔀', description: 'تخطيط مختلط للعقارات' },
      { type: 'COMPACT_PROPERTY_LIST', name: 'قائمة مضغوطة', icon: '📋', description: 'قائمة مضغوطة توفر مساحة' },
      { type: 'FEATURED_PROPERTIES_SHOWCASE', name: 'عرض العقارات المميزة', icon: '🌟', description: 'عرض مميز للعقارات المختارة' }
    ]
  },
  {
    category: 'الوجهات',
    color: '#9C27B0',
    types: [
      { type: 'CITY_CARDS_GRID', name: 'شبكة بطاقات المدن', icon: '🏙️', description: 'شبكة تعرض المدن الرئيسية' },
      { type: 'DESTINATION_CAROUSEL', name: 'دوار الوجهات', icon: '🗺️', description: 'دوار يعرض الوجهات السياحية' },
      { type: 'EXPLORE_CITIES', name: 'استكشاف المدن', icon: '🧭', description: 'قسم لاستكشاف المدن المختلفة' }
    ]
  },
  {
    category: 'الدوارات المتميزة',
    color: '#E91E63',
    types: [
      { type: 'PREMIUM_CAROUSEL', name: 'دوار متميز', icon: '💎', description: 'دوار للعقارات المتميزة' },
      { type: 'INTERACTIVE_SHOWCASE', name: 'عرض تفاعلي', icon: '🎯', description: 'عرض تفاعلي للمحتوى' }
    ]
  }
];

// أنواع الرسوم المتحركة
const ANIMATION_TYPES = [
  { value: 'NONE', label: 'بدون', icon: '⏸️' },
  { value: 'FADE', label: 'ظهور تدريجي', icon: '🌅' },
  { value: 'SLIDE', label: 'انزلاق', icon: '🎢' },
  { value: 'SCALE', label: 'تحجيم', icon: '🔍' },
  { value: 'ROTATE', label: 'دوران', icon: '🔄' },
  { value: 'PARALLAX', label: 'متوازي', icon: '📐' },
  { value: 'SHIMMER', label: 'لمعان', icon: '✨' },
  { value: 'PULSE', label: 'نبض', icon: '💓' },
  { value: 'BOUNCE', label: 'ارتداد', icon: '🏀' },
  { value: 'FLIP', label: 'انقلاب', icon: '🔃' }
];

// أحجام الأقسام
const SECTION_SIZES = [
  { value: 'COMPACT', label: 'مضغوط', factor: 0.5 },
  { value: 'SMALL', label: 'صغير', factor: 0.75 },
  { value: 'MEDIUM', label: 'متوسط', factor: 1.0 },
  { value: 'LARGE', label: 'كبير', factor: 1.25 },
  { value: 'EXTRA_LARGE', label: 'كبير جداً', factor: 1.5 },
  { value: 'FULL_SCREEN', label: 'ملء الشاشة', factor: 2.0 }
];

interface HomeScreenManagementProps {}

const HomeScreenManagement: React.FC<HomeScreenManagementProps> = () => {
  // State management
  const [selectedTab, setSelectedTab] = useState(0);
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedSection, setSelectedSection] = useState<DynamicHomeSection | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [isEditMode, setIsEditMode] = useState(false);
  const [previewMode, setPreviewMode] = useState<'mobile' | 'web'>('mobile');
  const [snackbar, setSnackbar] = useState({ open: false, message: '', type: 'success' as 'success' | 'error' });
  const [menuAnchor, setMenuAnchor] = useState<null | HTMLElement>(null);
  const [selectedSectionForMenu, setSelectedSectionForMenu] = useState<string | null>(null);
  const [configFormData, setConfigFormData] = useState<any>({});
  const [previewDrawerOpen, setPreviewDrawerOpen] = useState(false);
  const [livePreviewMode, setLivePreviewMode] = useState(false);
  const [previewData, setPreviewData] = useState<DynamicHomeSection | null>(null);

  // Data fetching hooks
  const { data: sections = [], isLoading: sectionsLoading, refetch: refetchSections } = useDynamicHomeSections({
    includeContent: true,
    onlyActive: false
  });
  const { data: destinations = [] } = useCityDestinations();
  const { data: sponsoredAds = [] } = useSponsoredAds();
  const { data: homeConfig } = useHomeConfig();

  // Mutation hooks
  const createSectionMutation = useCreateDynamicSection();
  const updateSectionMutation = useUpdateDynamicSection();
  const deleteSectionMutation = useDeleteDynamicSection();
  const toggleSectionMutation = useToggleDynamicSection();
  const reorderSectionsMutation = useReorderDynamicSections();

  // Computed values
  const filteredSections = useMemo(() => {
    return sections.filter(section => 
      section.title?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      section.sectionType?.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [sections, searchTerm]);

  const sectionsByCategory = useMemo(() => {
    const categorized: { [key: string]: DynamicHomeSection[] } = {};
    SECTION_TYPES.forEach(category => {
      categorized[category.category] = filteredSections.filter(section =>
        category.types.some(type => type.type === section.sectionType)
      );
    });
    return categorized;
  }, [filteredSections]);

  // Event handlers
  const handleCreateSection = async (sectionData: any) => {
    try {
      await createSectionMutation.mutateAsync(sectionData);
      setSnackbar({ open: true, message: 'تم إنشاء القسم بنجاح', type: 'success' });
      setIsDialogOpen(false);
      refetchSections();
    } catch (error) {
      setSnackbar({ open: true, message: 'فشل في إنشاء القسم', type: 'error' });
    }
  };

  const handleUpdateSection = async (id: string, sectionData: any) => {
    try {
      await updateSectionMutation.mutateAsync({ id, command: sectionData });
      setSnackbar({ open: true, message: 'تم تحديث القسم بنجاح', type: 'success' });
      setIsDialogOpen(false);
      refetchSections();
    } catch (error) {
      setSnackbar({ open: true, message: 'فشل في تحديث القسم', type: 'error' });
    }
  };

  const handleDeleteSection = async (id: string) => {
    if (window.confirm('هل أنت متأكد من حذف هذا القسم؟')) {
      try {
        await deleteSectionMutation.mutateAsync(id);
        setSnackbar({ open: true, message: 'تم حذف القسم بنجاح', type: 'success' });
        refetchSections();
      } catch (error) {
        setSnackbar({ open: true, message: 'فشل في حذف القسم', type: 'error' });
      }
    }
  };

  const handleToggleSection = async (id: string, isActive: boolean) => {
    try {
      await toggleSectionMutation.mutateAsync({ id, setActive: !isActive });
      setSnackbar({ 
        open: true, 
        message: `تم ${!isActive ? 'تفعيل' : 'إلغاء تفعيل'} القسم بنجاح`, 
        type: 'success' 
      });
      refetchSections();
    } catch (error) {
      setSnackbar({ open: true, message: 'فشل في تغيير حالة القسم', type: 'error' });
    }
  };

  const handleDragEnd = async (result: any) => {
    if (!result.destination) return;

    const reorderedSections = Array.from(filteredSections);
    const [movedSection] = reorderedSections.splice(result.source.index, 1);
    reorderedSections.splice(result.destination.index, 0, movedSection);

    const reorderCommand = {
      sections: reorderedSections.map((section, index) => ({
        sectionId: section.id,
        newOrder: index + 1
      }))
    };

    try {
      await reorderSectionsMutation.mutateAsync(reorderCommand);
      setSnackbar({ open: true, message: 'تم إعادة ترتيب الأقسام بنجاح', type: 'success' });
      refetchSections();
    } catch (error) {
      setSnackbar({ open: true, message: 'فشل في إعادة ترتيب الأقسام', type: 'error' });
    }
  };

  const openCreateDialog = () => {
    setSelectedSection(null);
    setIsEditMode(false);
    setConfigFormData({});
    setIsDialogOpen(true);
  };

  const openEditDialog = (section: DynamicHomeSection) => {
    setSelectedSection(section);
    setIsEditMode(true);
    setConfigFormData({
      title: section.title || '',
      subtitle: section.subtitle || '',
      titleAr: section.titleAr || '',
      subtitleAr: section.subtitleAr || '',
      sectionType: section.sectionType,
      priority: section.priority || 0,
      scheduledAt: section.scheduledAt || null,
      expiresAt: section.expiresAt || null,
      targetAudience: section.targetAudience || [],
      sectionConfig: section.sectionConfig || {},
      metadata: section.metadata || {}
    });
    setIsDialogOpen(true);
  };

  // Preview functions
  const openPreview = (section: DynamicHomeSection) => {
    setPreviewData(section);
    setPreviewDrawerOpen(true);
  };

  const openLivePreview = () => {
    setLivePreviewMode(true);
    setPreviewDrawerOpen(true);
  };

  const closeLivePreview = () => {
    setLivePreviewMode(false);
    setPreviewDrawerOpen(false);
    setPreviewData(null);
  };

  const handleMenuClick = (event: React.MouseEvent<HTMLElement>, sectionId: string) => {
    setMenuAnchor(event.currentTarget);
    setSelectedSectionForMenu(sectionId);
  };

  const handleMenuClose = () => {
    setMenuAnchor(null);
    setSelectedSectionForMenu(null);
  };

  // Statistics calculation
  const statistics = useMemo(() => {
    const total = sections.length;
    const active = sections.filter(s => s.isActive).length;
    const scheduled = sections.filter(s => s.scheduledAt && new Date(s.scheduledAt) > new Date()).length;
    const expired = sections.filter(s => s.expiresAt && new Date(s.expiresAt) < new Date()).length;
    
    return { total, active, inactive: total - active, scheduled, expired };
  }, [sections]);

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <Box sx={{ p: 3, bgcolor: '#f5f5f5', minHeight: '100vh' }}>
        {/* Header */}
        <Box sx={{ mb: 4 }}>
          <Typography variant="h4" sx={{ fontWeight: 'bold', mb: 2, display: 'flex', alignItems: 'center' }}>
            <HomeIcon sx={{ mr: 2, color: '#2196F3' }} />
            إدارة الشاشة الرئيسية
          </Typography>
          
          {/* Statistics Cards */}
          <Grid container spacing={2} sx={{ mb: 3 }}>
            <Grid item xs={12} sm={6} md={2.4}>
              <Card sx={{ bgcolor: '#2196F3', color: 'white' }}>
                <CardContent sx={{ textAlign: 'center', py: 2 }}>
                  <Typography variant="h3" sx={{ fontWeight: 'bold' }}>{statistics.total}</Typography>
                  <Typography variant="body2">إجمالي الأقسام</Typography>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} sm={6} md={2.4}>
              <Card sx={{ bgcolor: '#4CAF50', color: 'white' }}>
                <CardContent sx={{ textAlign: 'center', py: 2 }}>
                  <Typography variant="h3" sx={{ fontWeight: 'bold' }}>{statistics.active}</Typography>
                  <Typography variant="body2">نشط</Typography>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} sm={6} md={2.4}>
              <Card sx={{ bgcolor: '#FF9800', color: 'white' }}>
                <CardContent sx={{ textAlign: 'center', py: 2 }}>
                  <Typography variant="h3" sx={{ fontWeight: 'bold' }}>{statistics.inactive}</Typography>
                  <Typography variant="body2">غير نشط</Typography>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} sm={6} md={2.4}>
              <Card sx={{ bgcolor: '#9C27B0', color: 'white' }}>
                <CardContent sx={{ textAlign: 'center', py: 2 }}>
                  <Typography variant="h3" sx={{ fontWeight: 'bold' }}>{statistics.scheduled}</Typography>
                  <Typography variant="body2">مجدول</Typography>
                </CardContent>
              </Card>
            </Grid>
            <Grid item xs={12} sm={6} md={2.4}>
              <Card sx={{ bgcolor: '#F44336', color: 'white' }}>
                <CardContent sx={{ textAlign: 'center', py: 2 }}>
                  <Typography variant="h3" sx={{ fontWeight: 'bold' }}>{statistics.expired}</Typography>
                  <Typography variant="body2">منتهي الصلاحية</Typography>
                </CardContent>
              </Card>
            </Grid>
          </Grid>

          {/* Action Bar */}
          <Paper sx={{ p: 2, mb: 3 }}>
            <Stack direction="row" spacing={2} alignItems="center" flexWrap="wrap">
              <TextField
                placeholder="البحث في الأقسام..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                size="small"
                sx={{ minWidth: 250 }}
                InputProps={{
                  startAdornment: <SearchIcon sx={{ mr: 1, color: 'gray' }} />
                }}
              />
              
              <FormControl size="small" sx={{ minWidth: 120 }}>
                <InputLabel>عرض</InputLabel>
                <Select
                  value={viewMode}
                  onChange={(e) => setViewMode(e.target.value as 'grid' | 'list')}
                  label="عرض"
                >
                  <MenuItem value="grid">
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <GridViewIcon sx={{ mr: 1 }} />
                      شبكة
                    </Box>
                  </MenuItem>
                  <MenuItem value="list">
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <ListViewIcon sx={{ mr: 1 }} />
                      قائمة
                    </Box>
                  </MenuItem>
                </Select>
              </FormControl>

              <FormControl size="small" sx={{ minWidth: 120 }}>
                <InputLabel>المعاينة</InputLabel>
                <Select
                  value={previewMode}
                  onChange={(e) => setPreviewMode(e.target.value as 'mobile' | 'web')}
                  label="المعاينة"
                >
                  <MenuItem value="mobile">
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <MobileIcon sx={{ mr: 1 }} />
                      موبايل
                    </Box>
                  </MenuItem>
                  <MenuItem value="web">
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <WebIcon sx={{ mr: 1 }} />
                      ويب
                    </Box>
                  </MenuItem>
                </Select>
              </FormControl>

              <Box sx={{ flexGrow: 1 }} />
              
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={openCreateDialog}
                sx={{ bgcolor: '#2196F3' }}
              >
                إضافة قسم جديد
              </Button>
              
              <Button
                variant="outlined"
                startIcon={<RefreshIcon />}
                onClick={() => refetchSections()}
                disabled={sectionsLoading}
              >
                تحديث
              </Button>

              <Button
                variant="outlined"
                startIcon={<PreviewIcon />}
                onClick={openLivePreview}
                sx={{ color: '#9C27B0', borderColor: '#9C27B0' }}
              >
                معاينة مباشرة
              </Button>
            </Stack>
          </Paper>
        </Box>

        {/* Loading State */}
        {sectionsLoading && (
          <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
            <CircularProgress />
          </Box>
        )}

        {/* Sections Display */}
        {!sectionsLoading && (
          <DragDropContext onDragEnd={handleDragEnd}>
            <Droppable droppableId="sections">
              {(provided) => (
                <div {...provided.droppableProps} ref={provided.innerRef}>
                  {viewMode === 'grid' ? (
                    <Grid container spacing={3}>
                      {filteredSections.map((section, index) => (
                        <Draggable key={section.id} draggableId={section.id} index={index}>
                          {(provided) => (
                            <Grid
                              item
                              xs={12}
                              sm={6}
                              md={4}
                              lg={3}
                              ref={provided.innerRef}
                              {...provided.draggableProps}
                            >
                              <SectionCard
                                section={section}
                                onEdit={() => openEditDialog(section)}
                                onDelete={() => handleDeleteSection(section.id)}
                                onToggle={() => handleToggleSection(section.id, section.isActive)}
                                onPreview={() => openPreview(section)}
                                onMenuClick={(e) => handleMenuClick(e, section.id)}
                                dragHandleProps={provided.dragHandleProps}
                              />
                            </Grid>
                          )}
                        </Draggable>
                      ))}
                    </Grid>
                  ) : (
                    <List>
                      {filteredSections.map((section, index) => (
                        <Draggable key={section.id} draggableId={section.id} index={index}>
                          {(provided) => (
                            <div ref={provided.innerRef} {...provided.draggableProps}>
                              <SectionListItem
                                section={section}
                                onEdit={() => openEditDialog(section)}
                                onDelete={() => handleDeleteSection(section.id)}
                                onToggle={() => handleToggleSection(section.id, section.isActive)}
                                onPreview={() => openPreview(section)}
                                onMenuClick={(e) => handleMenuClick(e, section.id)}
                                dragHandleProps={provided.dragHandleProps}
                              />
                            </div>
                          )}
                        </Draggable>
                      ))}
                    </List>
                  )}
                  {provided.placeholder}
                </div>
              )}
            </Droppable>
          </DragDropContext>
        )}

        {/* Empty State */}
        {!sectionsLoading && filteredSections.length === 0 && (
          <Paper sx={{ p: 8, textAlign: 'center' }}>
            <HomeIcon sx={{ fontSize: 80, color: 'gray', mb: 2 }} />
            <Typography variant="h5" sx={{ mb: 2 }}>
              {searchTerm ? 'لا توجد نتائج للبحث' : 'لا توجد أقسام'}
            </Typography>
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              {searchTerm 
                ? 'جرب تعديل مصطلح البحث أو إزالة المرشحات'
                : 'ابدأ بإنشاء قسم جديد للشاشة الرئيسية'
              }
            </Typography>
            {!searchTerm && (
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={openCreateDialog}
                size="large"
              >
                إنشاء قسم جديد
              </Button>
            )}
          </Paper>
        )}

        {/* Section Dialog */}
        <SectionDialog
          open={isDialogOpen}
          onClose={() => setIsDialogOpen(false)}
          section={selectedSection}
          isEdit={isEditMode}
          onSave={isEditMode ? 
            (data) => handleUpdateSection(selectedSection!.id, data) : 
            handleCreateSection
          }
          formData={configFormData}
          onFormDataChange={setConfigFormData}
        />

        {/* Context Menu */}
        <Menu
          anchorEl={menuAnchor}
          open={Boolean(menuAnchor)}
          onClose={handleMenuClose}
        >
          <MenuList>
            <MenuItem onClick={() => {
              const section = sections.find(s => s.id === selectedSectionForMenu);
              if (section) openEditDialog(section);
              handleMenuClose();
            }}>
              <EditIcon sx={{ mr: 1 }} />
              تعديل
            </MenuItem>
            <MenuItem onClick={() => {
              if (selectedSectionForMenu) {
                const section = sections.find(s => s.id === selectedSectionForMenu);
                if (section) handleToggleSection(section.id, section.isActive);
              }
              handleMenuClose();
            }}>
              {(() => {
                const section = sections.find(s => s.id === selectedSectionForMenu);
                return section?.isActive ? (
                  <>
                    <VisibilityOffIcon sx={{ mr: 1 }} />
                    إلغاء التفعيل
                  </>
                ) : (
                  <>
                    <VisibilityIcon sx={{ mr: 1 }} />
                    تفعيل
                  </>
                );
              })()}
            </MenuItem>
            <MenuItem onClick={() => {
              // Copy section functionality
              handleMenuClose();
            }}>
              <CopyIcon sx={{ mr: 1 }} />
              نسخ
            </MenuItem>
            <Divider />
            <MenuItem 
              onClick={() => {
                if (selectedSectionForMenu) handleDeleteSection(selectedSectionForMenu);
                handleMenuClose();
              }}
              sx={{ color: 'error.main' }}
            >
              <DeleteIcon sx={{ mr: 1 }} />
              حذف
            </MenuItem>
          </MenuList>
        </Menu>

        {/* Floating Action Button */}
        <SpeedDial
          ariaLabel="إجراءات سريعة"
          sx={{ position: 'fixed', bottom: 24, right: 24 }}
          icon={<SpeedDialIcon />}
        >
          <SpeedDialAction
            icon={<AddIcon />}
            tooltipTitle="إضافة قسم"
            onClick={openCreateDialog}
          />
          <SpeedDialAction
            icon={<AnalyticsIcon />}
            tooltipTitle="الإحصائيات"
            onClick={() => {/* Analytics functionality */}}
          />
          <SpeedDialAction
            icon={<SettingsIcon />}
            tooltipTitle="الإعدادات"
            onClick={() => {/* Settings functionality */}}
          />
          <SpeedDialAction
            icon={<PreviewIcon />}
            tooltipTitle="معاينة"
            onClick={() => {/* Preview functionality */}}
          />
        </SpeedDial>

        {/* Live Preview Drawer */}
        <Drawer
          anchor="right"
          open={previewDrawerOpen}
          onClose={closeLivePreview}
          PaperProps={{
            sx: { 
              width: previewMode === 'mobile' ? 400 : 800,
              maxWidth: '90vw'
            }
          }}
        >
          <AppBar position="sticky" sx={{ bgcolor: '#2196F3' }}>
            <Toolbar>
              <Typography variant="h6" sx={{ flexGrow: 1 }}>
                {livePreviewMode ? 'معاينة مباشرة - الشاشة الرئيسية' : 'معاينة القسم'}
              </Typography>
              
              <FormControl size="small" sx={{ minWidth: 100, mr: 2 }}>
                <Select
                  value={previewMode}
                  onChange={(e) => setPreviewMode(e.target.value as 'mobile' | 'web')}
                  sx={{ color: 'white', '& .MuiOutlinedInput-notchedOutline': { borderColor: 'rgba(255,255,255,0.3)' } }}
                >
                  <MenuItem value="mobile">
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <MobileIcon sx={{ mr: 1 }} />
                      موبايل
                    </Box>
                  </MenuItem>
                  <MenuItem value="web">
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <WebIcon sx={{ mr: 1 }} />
                      ويب
                    </Box>
                  </MenuItem>
                </Select>
              </FormControl>
              
              <IconButton color="inherit" onClick={closeLivePreview}>
                <CancelIcon />
              </IconButton>
            </Toolbar>
          </AppBar>

          <Box sx={{ 
            p: 2, 
            height: '100%', 
            overflow: 'auto',
            bgcolor: '#f5f5f5',
            minHeight: 'calc(100vh - 64px)'
          }}>
            {/* Preview Content */}
            <Box sx={{
              maxWidth: previewMode === 'mobile' ? '100%' : 800,
              mx: 'auto',
              bgcolor: 'white',
              borderRadius: 2,
              overflow: 'hidden',
              boxShadow: previewMode === 'mobile' ? 'none' : 3
            }}>
              {livePreviewMode ? (
                // Live preview of all sections
                <Box>
                  {sectionsLoading ? (
                    <Stack spacing={2} sx={{ p: 2 }}>
                      {[1, 2, 3, 4, 5].map((i) => (
                        <Skeleton key={i} variant="rectangular" height={200} />
                      ))}
                    </Stack>
                  ) : (
                    <HomeScreenLayout 
                      config={homeConfig || {
                        id: 'preview',
                        version: '1.0',
                        isActive: true,
                        createdAt: new Date().toISOString(),
                        updatedAt: new Date().toISOString(),
                        globalSettings: {},
                        themeSettings: {},
                        layoutSettings: { sectionSpacing: 24 },
                        cacheSettings: {},
                        analyticsSettings: {},
                        enabledFeatures: [],
                        experimentalFeatures: {}
                      }} 
                      sections={filteredSections.filter(s => s.isActive)} 
                    />
                  )}
                </Box>
              ) : (
                // Single section preview
                previewData && (
                  <Box>
                    <Box sx={{ p: 2, bgcolor: '#2196F3', color: 'white' }}>
                      <Typography variant="h6">
                        {previewData.title || SECTION_TYPES.flatMap(cat => cat.types).find(t => t.type === previewData.sectionType)?.name}
                      </Typography>
                      <Typography variant="body2" sx={{ opacity: 0.8 }}>
                        {previewData.sectionType} • الترتيب: {previewData.order} • الأولوية: {previewData.priority}
                      </Typography>
                    </Box>
                    
                    <Box sx={{ p: 2 }}>
                      {previewData.isActive ? (
                        <DynamicSection
                          section={previewData}
                          config={homeConfig || {
                            id: 'preview',
                            version: '1.0',
                            isActive: true,
                            createdAt: new Date().toISOString(),
                            updatedAt: new Date().toISOString(),
                            globalSettings: {},
                            themeSettings: {},
                            layoutSettings: {},
                            cacheSettings: {},
                            analyticsSettings: {},
                            enabledFeatures: [],
                            experimentalFeatures: {}
                          }}
                        />
                      ) : (
                        <Alert severity="warning" sx={{ mb: 2 }}>
                          هذا القسم غير نشط حالياً
                        </Alert>
                      )}
                    </Box>
                  </Box>
                )
              )}
            </Box>

            {/* Preview Info Panel */}
            <Paper sx={{ mt: 2, p: 2 }}>
              <Typography variant="subtitle2" sx={{ fontWeight: 'bold', mb: 1 }}>
                معلومات المعاينة
              </Typography>
              <Grid container spacing={2}>
                <Grid item xs={6}>
                  <Typography variant="body2" color="text.secondary">
                    نوع الجهاز: {previewMode === 'mobile' ? 'موبايل' : 'ويب'}
                  </Typography>
                </Grid>
                <Grid item xs={6}>
                  <Typography variant="body2" color="text.secondary">
                    عدد الأقسام: {livePreviewMode ? filteredSections.filter(s => s.isActive).length : 1}
                  </Typography>
                </Grid>
                <Grid item xs={6}>
                  <Typography variant="body2" color="text.secondary">
                    وقت التحديث: {new Date().toLocaleTimeString('ar-SA')}
                  </Typography>
                </Grid>
                <Grid item xs={6}>
                  <Typography variant="body2" color="text.secondary">
                    الحالة: {livePreviewMode ? 'معاينة شاملة' : 'معاينة قسم واحد'}
                  </Typography>
                </Grid>
              </Grid>
            </Paper>
          </Box>
        </Drawer>

        {/* Section Dialog */}
        <SectionConfigDialog
          open={isDialogOpen}
          onClose={() => setIsDialogOpen(false)}
          section={selectedSection}
          isEdit={isEditMode}
          onSave={isEditMode ? 
            (data) => handleUpdateSection(selectedSection!.id, data) : 
            handleCreateSection
          }
        />

        {/* Snackbar */}
        <Snackbar
          open={snackbar.open}
          autoHideDuration={6000}
          onClose={() => setSnackbar({ ...snackbar, open: false })}
        >
          <Alert 
            severity={snackbar.type}
            onClose={() => setSnackbar({ ...snackbar, open: false })}
            sx={{ width: '100%' }}
          >
            {snackbar.message}
          </Alert>
        </Snackbar>
      </Box>
    </LocalizationProvider>
  );
};

// Section Card Component
interface SectionCardProps {
  section: DynamicHomeSection;
  onEdit: () => void;
  onDelete: () => void;
  onToggle: () => void;
  onPreview: () => void;
  onMenuClick: (event: React.MouseEvent<HTMLElement>) => void;
  dragHandleProps: any;
}

const SectionCard: React.FC<SectionCardProps> = ({ 
  section, 
  onEdit, 
  onDelete, 
  onToggle, 
  onPreview,
  onMenuClick,
  dragHandleProps 
}) => {
  const sectionInfo = SECTION_TYPES
    .flatMap(cat => cat.types)
    .find(type => type.type === section.sectionType);

  const categoryInfo = SECTION_TYPES.find(cat => 
    cat.types.some(type => type.type === section.sectionType)
  );

  const isScheduled = section.scheduledAt && new Date(section.scheduledAt) > new Date();
  const isExpired = section.expiresAt && new Date(section.expiresAt) < new Date();

  return (
    <Card 
      sx={{ 
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        position: 'relative',
        border: section.isActive ? `2px solid ${categoryInfo?.color}` : '2px solid #e0e0e0',
        '&:hover': {
          boxShadow: 3,
          transform: 'translateY(-2px)',
          transition: 'all 0.3s ease'
        }
      }}
    >
      {/* Drag Handle */}
      <Box 
        {...dragHandleProps}
        sx={{ 
          position: 'absolute',
          top: 8,
          left: 8,
          cursor: 'grab',
          '&:active': { cursor: 'grabbing' }
        }}
      >
        <DragHandleIcon sx={{ color: 'gray' }} />
      </Box>

      {/* Menu Button */}
      <IconButton
        size="small"
        onClick={onMenuClick}
        sx={{ position: 'absolute', top: 8, right: 8 }}
      >
        <MoreVertIcon />
      </IconButton>

      <CardContent sx={{ flexGrow: 1, pt: 5 }}>
        {/* Section Header */}
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6" sx={{ fontSize: '1.5rem', mr: 1 }}>
            {sectionInfo?.icon}
          </Typography>
          <Box sx={{ flexGrow: 1 }}>
            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 0.5 }}>
              {section.title || sectionInfo?.name}
            </Typography>
            <Chip 
              label={categoryInfo?.category}
              size="small"
              sx={{ 
                bgcolor: categoryInfo?.color,
                color: 'white',
                fontSize: '0.7rem'
              }}
            />
          </Box>
        </Box>

        {/* Section Status */}
        <Stack direction="row" spacing={1} sx={{ mb: 2 }}>
          <Chip
            label={section.isActive ? 'نشط' : 'غير نشط'}
            color={section.isActive ? 'success' : 'default'}
            size="small"
          />
          {isScheduled && (
            <Chip
              label="مجدول"
              color="info"
              size="small"
              icon={<ScheduleIcon />}
            />
          )}
          {isExpired && (
            <Chip
              label="منتهي"
              color="error"
              size="small"
            />
          )}
        </Stack>

        {/* Section Details */}
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          {sectionInfo?.description}
        </Typography>

        {/* Section Stats */}
        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
          <Typography variant="caption">
            الترتيب: {section.order}
          </Typography>
          <Typography variant="caption">
            الأولوية: {section.priority}
          </Typography>
          <Typography variant="caption">
            المحتوى: {section.content?.length || 0}
          </Typography>
        </Box>

        {/* Progress Indicator */}
        {section.expiresAt && (
          <Box sx={{ mb: 2 }}>
            <Typography variant="caption" color="text.secondary">
              {isExpired ? 'انتهت الصلاحية' : 'ينتهي في'}
            </Typography>
            <LinearProgress 
              variant="determinate"
              value={isExpired ? 100 : calculateProgressPercentage(section.scheduledAt, section.expiresAt)}
              color={isExpired ? 'error' : 'primary'}
              sx={{ mt: 0.5 }}
            />
          </Box>
        )}
      </CardContent>

      {/* Action Buttons */}
      <Box sx={{ p: 2, pt: 0 }}>
        <Stack direction="row" spacing={1} sx={{ mb: 1 }}>
          <Button
            size="small"
            startIcon={<EditIcon />}
            onClick={onEdit}
            variant="outlined"
            fullWidth
          >
            تعديل
          </Button>
          <Button
            size="small"
            startIcon={<PreviewIcon />}
            onClick={onPreview}
            variant="outlined"
            sx={{ color: '#9C27B0', borderColor: '#9C27B0' }}
          >
            معاينة
          </Button>
        </Stack>
        <Stack direction="row" spacing={1} justifyContent="center">
          <IconButton
            size="small"
            onClick={onToggle}
            color={section.isActive ? 'warning' : 'success'}
          >
            {section.isActive ? <VisibilityOffIcon /> : <VisibilityIcon />}
          </IconButton>
          <IconButton
            size="small"
            onClick={onDelete}
            color="error"
          >
            <DeleteIcon />
          </IconButton>
        </Stack>
      </Box>
    </Card>
  );
};

// Section List Item Component
interface SectionListItemProps {
  section: DynamicHomeSection;
  onEdit: () => void;
  onDelete: () => void;
  onToggle: () => void;
  onPreview: () => void;
  onMenuClick: (event: React.MouseEvent<HTMLElement>) => void;
  dragHandleProps: any;
}

const SectionListItem: React.FC<SectionListItemProps> = ({ 
  section, 
  onEdit, 
  onDelete, 
  onToggle, 
  onPreview,
  onMenuClick,
  dragHandleProps 
}) => {
  const sectionInfo = SECTION_TYPES
    .flatMap(cat => cat.types)
    .find(type => type.type === section.sectionType);

  const categoryInfo = SECTION_TYPES.find(cat => 
    cat.types.some(type => type.type === section.sectionType)
  );

  const isScheduled = section.scheduledAt && new Date(section.scheduledAt) > new Date();
  const isExpired = section.expiresAt && new Date(section.expiresAt) < new Date();

  return (
    <Paper sx={{ mb: 2, p: 2 }}>
      <ListItem
        sx={{ px: 0 }}
        secondaryAction={
          <Stack direction="row" spacing={1}>
            <FormControlLabel
              control={
                <Switch
                  checked={section.isActive}
                  onChange={onToggle}
                  size="small"
                />
              }
              label=""
            />
            <Button
              size="small"
              startIcon={<PreviewIcon />}
              onClick={onPreview}
              variant="outlined"
              sx={{ color: '#9C27B0', borderColor: '#9C27B0' }}
            >
              معاينة
            </Button>
            <IconButton onClick={onEdit} size="small">
              <EditIcon />
            </IconButton>
            <IconButton onClick={onDelete} size="small" color="error">
              <DeleteIcon />
            </IconButton>
            <IconButton onClick={onMenuClick} size="small">
              <MoreVertIcon />
            </IconButton>
            <Box {...dragHandleProps} sx={{ display: 'flex', alignItems: 'center', cursor: 'grab' }}>
              <DragHandleIcon />
            </Box>
          </Stack>
        }
      >
        <Box sx={{ display: 'flex', alignItems: 'center', width: '100%' }}>
          <Avatar sx={{ bgcolor: categoryInfo?.color, mr: 2 }}>
            {sectionInfo?.icon}
          </Avatar>
          
          <Box sx={{ flexGrow: 1 }}>
            <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
              {section.title || sectionInfo?.name}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {sectionInfo?.description}
            </Typography>
            
            <Stack direction="row" spacing={1} sx={{ mt: 1 }}>
              <Chip
                label={categoryInfo?.category}
                size="small"
                sx={{ bgcolor: categoryInfo?.color, color: 'white' }}
              />
              <Chip
                label={`الترتيب: ${section.order}`}
                size="small"
                variant="outlined"
              />
              <Chip
                label={`المحتوى: ${section.content?.length || 0}`}
                size="small"
                variant="outlined"
              />
              {isScheduled && (
                <Chip
                  label="مجدول"
                  color="info"
                  size="small"
                  icon={<ScheduleIcon />}
                />
              )}
              {isExpired && (
                <Chip
                  label="منتهي"
                  color="error"
                  size="small"
                />
              )}
            </Stack>
          </Box>
        </Box>
      </ListItem>
    </Paper>
  );
};

// Helper function to calculate progress percentage
const calculateProgressPercentage = (startDate?: string, endDate?: string): number => {
  if (!startDate || !endDate) return 0;
  
  const start = new Date(startDate).getTime();
  const end = new Date(endDate).getTime();
  const now = new Date().getTime();
  
  if (now < start) return 0;
  if (now > end) return 100;
  
  return ((now - start) / (end - start)) * 100;
};


export default HomeScreenManagement;
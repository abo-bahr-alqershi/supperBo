// frontend/src/pages/admin/HomeScreenManagement.tsx

import React, { useState, useEffect, useMemo, useCallback } from 'react';
import {
  Box,
  Container,
  Typography,
  Button,
  Stack,
  Paper,
  Grid,
  Fab,
  SpeedDial,
  SpeedDialAction,
  SpeedDialIcon,
  Alert,
  Snackbar,
  Backdrop,
  CircularProgress,
  Tab,
  Tabs,
  Chip,
  IconButton,
  Menu,
  MenuItem,
  Badge,
  Tooltip,
  Divider,
  Card,
  CardContent,
  CardActions,
  Switch,
  FormControlLabel,
  TextField,
  InputAdornment,
  Avatar,
  LinearProgress,
  Fade,
  Zoom,
  Grow,
  useTheme,
  alpha,
  Skeleton,
  FormControl,
  Select,
  ToggleButton,
  ToggleButtonGroup,
} from '@mui/material';
import {
  Add as AddIcon,
  Settings as SettingsIcon,
  Refresh as RefreshIcon,
  FilterList as FilterIcon,
  ViewModule as ViewModuleIcon,
  ViewList as ViewListIcon,
  DragIndicator as DragIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
  Schedule as ScheduleIcon,
  ContentCopy as DuplicateIcon,
  Preview as PreviewIcon,
  Publish as PublishIcon,
  Home as HomeIcon,
  Dashboard as DashboardIcon,
  Language as LanguageIcon,
  MoreVert as MoreVertIcon,
  Search as SearchIcon,
  Analytics as AnalyticsIcon,
  TrendingUp as TrendingUpIcon,
  CheckCircle as CheckIcon,
  Warning as WarningIcon,
  Error as ErrorIcon,
  Info as InfoIcon,
  AutoAwesome as AutoAwesomeIcon,
  Bolt as BoltIcon,
  Timer as TimerIcon,
  Groups as GroupsIcon,
  ViewCarousel as CarouselIcon,
  GridView as GridIcon,
  Storage as StorageIcon,
} from '@mui/icons-material';
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragEndEvent,
} from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
} from '@dnd-kit/sortable';
import {
  useSortable,
} from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import arLocale from 'date-fns/locale/ar-SA';
import { 
  useDynamicHomeSections,
  useCreateDynamicSection,
  useUpdateDynamicSection,
  useToggleDynamicSection,
  useDeleteDynamicSection,
  useReorderDynamicSections,
  useHomeConfig,
} from '../../hooks/useDynamicSections';
import { usePublishHomeConfig } from '../../hooks/useHomeConfig';
import SectionConfigDialog from '../../components/admin/SectionConfigDialog';
import type { DynamicHomeSection } from '../../types/homeSections.types';

// Professional color palette
const COLORS = {
  primary: '#1a237e',
  secondary: '#004d40',
  success: '#1b5e20',
  warning: '#e65100',
  info: '#01579b',
  error: '#b71c1c',
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

// Section category colors
const getCategoryColor = (sectionType: string): string => {
  if (sectionType.includes('AD')) return COLORS.success;
  if (sectionType.includes('OFFER') || sectionType.includes('DEAL')) return COLORS.warning;
  if (sectionType.includes('LIST') || sectionType.includes('GRID') || sectionType.includes('SHOWCASE')) return COLORS.info;
  if (sectionType.includes('CITY') || sectionType.includes('DESTINATION')) return COLORS.accent;
  if (sectionType.includes('CAROUSEL')) return COLORS.secondary;
  return COLORS.text.secondary;
};

// Section icons
const getSectionIcon = (sectionType: string): string => {
  const iconMap: Record<string, string> = {
    SINGLE_PROPERTY_AD: '🏠',
    FEATURED_PROPERTY_AD: '⭐',
    MULTI_PROPERTY_AD: '🏘️',
    UNIT_SHOWCASE_AD: '🏢',
    SINGLE_PROPERTY_OFFER: '💰',
    LIMITED_TIME_OFFER: '⏰',
    SEASONAL_OFFER: '🎄',
    MULTI_PROPERTY_OFFERS_GRID: '📊',
    OFFERS_CAROUSEL: '🎠',
    FLASH_DEALS: '⚡',
    HORIZONTAL_PROPERTY_LIST: '📋',
    VERTICAL_PROPERTY_GRID: '🔲',
    MIXED_LAYOUT_LIST: '🔀',
    COMPACT_PROPERTY_LIST: '📑',
    FEATURED_PROPERTIES_SHOWCASE: '🌟',
    CITY_CARDS_GRID: '🏙️',
    DESTINATION_CAROUSEL: '🗺️',
    EXPLORE_CITIES: '🧭',
    PREMIUM_CAROUSEL: '💎',
    INTERACTIVE_SHOWCASE: '🎯',
  };
  return iconMap[sectionType] || '📦';
};

// Sortable Item Component
interface SortableItemProps {
  id: string;
  children: React.ReactNode;
}

const SortableItem: React.FC<SortableItemProps> = ({ id, children }) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  return (
    <div ref={setNodeRef} style={style}>
      <Box {...attributes} {...listeners}>
        {children}
      </Box>
    </div>
  );
};

// Section Card Component
interface SectionCardProps {
  section: DynamicHomeSection;
  index: number;
  onEdit: () => void;
  onDelete: () => void;
  onToggle: () => void;
  onPreview: () => void;
  onDuplicate: () => void;
  onMenuClick: (event: React.MouseEvent<HTMLElement>) => void;
}

const SectionCard: React.FC<SectionCardProps> = ({
  section,
  index,
  onEdit,
  onDelete,
  onToggle,
  onPreview,
  onDuplicate,
  onMenuClick,
}) => {
  const theme = useTheme();
  const categoryColor = getCategoryColor(section.sectionType);
  const sectionIcon = getSectionIcon(section.sectionType);
  
  const isScheduled = section.scheduledAt && new Date(section.scheduledAt) > new Date();
  const isExpired = section.expiresAt && new Date(section.expiresAt) < new Date();

  return (
    <Card
      elevation={0}
      sx={{
        mb: 2,
        border: `1px solid ${COLORS.border}`,
        borderRight: `4px solid ${categoryColor}`,
        transition: 'all 0.3s ease',
        '&:hover': {
          transform: 'translateX(-4px)',
          boxShadow: 4,
          borderColor: categoryColor,
        },
      }}
    >
      <CardContent>
        <Grid container alignItems="center" spacing={2}>
          {/* Drag Handle */}
          <Grid item>
            <IconButton 
              sx={{ 
                cursor: 'grab',
                '&:active': { cursor: 'grabbing' },
                color: COLORS.text.disabled,
              }}
            >
              <DragIcon />
            </IconButton>
          </Grid>

          {/* Section Icon & Info */}
          <Grid item>
            <Avatar
              sx={{
                bgcolor: alpha(categoryColor, 0.1),
                color: categoryColor,
                width: 56,
                height: 56,
                fontSize: '1.5rem',
              }}
            >
              {sectionIcon}
            </Avatar>
          </Grid>

          <Grid item xs>
            <Stack spacing={0.5}>
              <Stack direction="row" alignItems="center" spacing={1.5}>
                <Typography variant="h6" sx={{ fontWeight: 600, color: COLORS.text.primary }}>
                  {section.title || section.titleAr || `قسم #${index + 1}`}
                </Typography>
                
                <Chip
                  size="small"
                  label={section.sectionType.replace(/_/g, ' ')}
                  sx={{
                    bgcolor: alpha(categoryColor, 0.1),
                    color: categoryColor,
                    fontWeight: 500,
                    fontSize: '0.75rem',
                  }}
                />

                {section.priority > 0 && (
                  <Chip
                    size="small"
                    icon={<AutoAwesomeIcon sx={{ fontSize: '0.875rem' }} />}
                    label={`أولوية: ${section.priority}`}
                    color="secondary"
                  />
                )}

                {isScheduled && (
                  <Chip
                    size="small"
                    icon={<ScheduleIcon sx={{ fontSize: '0.875rem' }} />}
                    label="مجدول"
                    color="info"
                  />
                )}

                {isExpired && (
                  <Chip
                    size="small"
                    icon={<ErrorIcon sx={{ fontSize: '0.875rem' }} />}
                    label="منتهي"
                    color="error"
                  />
                )}
              </Stack>

              {section.subtitle && (
                <Typography variant="body2" color="text.secondary">
                  {section.subtitle}
                </Typography>
              )}

              <Stack direction="row" spacing={3}>
                <Typography variant="caption" color="text.secondary">
                  <strong>الترتيب:</strong> {section.order}
                </Typography>
                {section.content && section.content.length > 0 && (
                  <Typography variant="caption" color="text.secondary">
                    <strong>المحتوى:</strong> {section.content.length} عنصر
                  </Typography>
                )}
                {section.targetAudience && section.targetAudience.length > 0 && (
                  <Typography variant="caption" color="text.secondary">
                    <strong>الجمهور:</strong> {section.targetAudience.join(', ')}
                  </Typography>
                )}
              </Stack>
            </Stack>
          </Grid>

          {/* Actions */}
          <Grid item>
            <Stack direction="row" spacing={1} alignItems="center">
              <FormControlLabel
                control={
                  <Switch
                    checked={section.isActive}
                    onChange={onToggle}
                    color="primary"
                    size="small"
                  />
                }
                label={
                  <Typography variant="caption" sx={{ fontWeight: 500 }}>
                    {section.isActive ? 'نشط' : 'غير نشط'}
                  </Typography>
                }
                labelPlacement="top"
                sx={{ m: 0 }}
              />

              <Tooltip title="معاينة">
                <IconButton onClick={onPreview} color="info" size="small">
                  <PreviewIcon />
                </IconButton>
              </Tooltip>

              <Tooltip title="تعديل">
                <IconButton onClick={onEdit} color="primary" size="small">
                  <EditIcon />
                </IconButton>
              </Tooltip>

              <IconButton onClick={onMenuClick} size="small">
                <MoreVertIcon />
              </IconButton>
            </Stack>
          </Grid>
        </Grid>
      </CardContent>
    </Card>
  );
};

// Main Component
const HomeScreenManagement: React.FC = () => {
  const theme = useTheme();
  const [currentTab, setCurrentTab] = useState(0);
  const [configDialogOpen, setConfigDialogOpen] = useState(false);
  const [selectedSection, setSelectedSection] = useState<DynamicHomeSection | null>(null);
  const [isEditMode, setIsEditMode] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [filter, setFilter] = useState<'all' | 'active' | 'inactive'>('all');
  const [viewMode, setViewMode] = useState<'cards' | 'list'>('cards');
  const [menuAnchor, setMenuAnchor] = useState<null | HTMLElement>(null);
  const [selectedSectionId, setSelectedSectionId] = useState<string | null>(null);
  const [previewDrawerOpen, setPreviewDrawerOpen] = useState(false);
  const [notification, setNotification] = useState<{
    open: boolean;
    message: string;
    severity: 'success' | 'error' | 'info' | 'warning';
  }>({
    open: false,
    message: '',
    severity: 'success',
  });

  // DnD Kit sensors
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  // Data fetching
  const { data: sections = [], isLoading, refetch } = useDynamicHomeSections({
    includeContent: true,
  });
  const { data: config } = useHomeConfig();

  // Mutations
  const createSection = useCreateDynamicSection();
  const updateSection = useUpdateDynamicSection();
  const toggleSection = useToggleDynamicSection();
  const deleteSection = useDeleteDynamicSection();
  const reorderSections = useReorderDynamicSections();
  const publishConfig = usePublishHomeConfig();

  // Filter and search sections
  const filteredSections = useMemo(() => {
    let filtered = sections;

    // Apply filter
    if (filter === 'active') {
      filtered = filtered.filter(s => s.isActive);
    } else if (filter === 'inactive') {
      filtered = filtered.filter(s => !s.isActive);
    }

    // Apply search
    if (searchTerm) {
      filtered = filtered.filter(s =>
        s.title?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        s.titleAr?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        s.sectionType.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    return filtered;
  }, [sections, filter, searchTerm]);

  // Statistics
  const statistics = useMemo(() => {
    const total = sections.length;
    const active = sections.filter(s => s.isActive).length;
    const scheduled = sections.filter(s => s.scheduledAt && new Date(s.scheduledAt) > new Date()).length;
    const expired = sections.filter(s => s.expiresAt && new Date(s.expiresAt) < new Date()).length;
    
    return { total, active, inactive: total - active, scheduled, expired };
  }, [sections]);

  // Handle drag end
  const handleDragEnd = useCallback(async (event: DragEndEvent) => {
    const { active, over } = event;

    if (active.id !== over?.id) {
      const oldIndex = filteredSections.findIndex((item) => item.id === active.id);
      const newIndex = filteredSections.findIndex((item) => item.id === over?.id);

      const reorderedSections = arrayMove(filteredSections, oldIndex, newIndex);
      
      const updates = reorderedSections.map((item, index) => ({
        sectionId: item.id,
        newOrder: index + 1,
      }));

      try {
        await reorderSections.mutateAsync({ sections: updates });
        showNotification('تم تحديث الترتيب بنجاح', 'success');
      } catch (error) {
        showNotification('فشل تحديث الترتيب', 'error');
      }
    }
  }, [filteredSections, reorderSections]);

  // Handle operations
  const handleCreateSection = () => {
    setSelectedSection(null);
    setIsEditMode(false);
    setConfigDialogOpen(true);
  };

  const handleEditSection = (section: DynamicHomeSection) => {
    setSelectedSection(section);
    setIsEditMode(true);
    setConfigDialogOpen(true);
  };

  const handleToggleSection = async (section: DynamicHomeSection) => {
    try {
      await toggleSection.mutateAsync({
        id: section.id,
        setActive: !section.isActive,
      });
      showNotification(
        section.isActive ? 'تم تعطيل القسم' : 'تم تفعيل القسم',
        'success'
      );
    } catch (error) {
      showNotification('فشل تحديث حالة القسم', 'error');
    }
  };

  const handleDeleteSection = async (section: DynamicHomeSection) => {
    if (window.confirm(`هل أنت متأكد من حذف القسم "${section.title || 'بدون عنوان'}"؟`)) {
      try {
        await deleteSection.mutateAsync(section.id);
        showNotification('تم حذف القسم بنجاح', 'success');
      } catch (error) {
        showNotification('فشل حذف القسم', 'error');
      }
    }
  };

  const handleDuplicateSection = async (section: DynamicHomeSection) => {
    try {
      const newSection = {
        ...section,
        title: `${section.title} (نسخة)`,
        titleAr: section.titleAr ? `${section.titleAr} (نسخة)` : undefined,
        order: sections.length + 1,
      };
      delete (newSection as any).id;
      delete (newSection as any).createdAt;
      delete (newSection as any).updatedAt;
      
      await createSection.mutateAsync(newSection as any);
      showNotification('تم نسخ القسم بنجاح', 'success');
    } catch (error) {
      showNotification('فشل نسخ القسم', 'error');
    }
  };

  const handleSaveSection = async (data: any) => {
    try {
      if (isEditMode && selectedSection) {
        await updateSection.mutateAsync({
          id: selectedSection.id,
          command: data,
        });
        showNotification('تم تحديث القسم بنجاح', 'success');
      } else {
        await createSection.mutateAsync(data);
        showNotification('تم إنشاء القسم بنجاح', 'success');
      }
      setConfigDialogOpen(false);
    } catch (error) {
      showNotification('فشل حفظ القسم', 'error');
    }
  };

  const handlePublishConfig = async () => {
    if (config && window.confirm('هل أنت متأكد من نشر التكوين الحالي؟')) {
      try {
        await publishConfig.mutateAsync({ id: config.id });
        showNotification('تم نشر التكوين بنجاح', 'success');
      } catch (error) {
        showNotification('فشل نشر التكوين', 'error');
      }
    }
  };

  const showNotification = (message: string, severity: 'success' | 'error' | 'info' | 'warning') => {
    setNotification({ open: true, message, severity });
  };

  const handleMenuClick = (event: React.MouseEvent<HTMLElement>, sectionId: string) => {
    setMenuAnchor(event.currentTarget);
    setSelectedSectionId(sectionId);
  };

  const handleMenuClose = () => {
    setMenuAnchor(null);
    setSelectedSectionId(null);
  };

  const handlePreviewSection = (section: DynamicHomeSection) => {
    setSelectedSection(section);
    setPreviewDrawerOpen(true);
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={arLocale}>
      <Box
        sx={{
          minHeight: '100vh',
          bgcolor: COLORS.background.light,
          pb: 4,
        }}
        dir="rtl"
      >
        {/* Header */}
        <Paper
          elevation={0}
          sx={{
            background: `linear-gradient(135deg, ${COLORS.primary} 0%, ${alpha(COLORS.primary, 0.8)} 100%)`,
            color: 'white',
            borderRadius: 0,
            mb: 3,
          }}
        >
          <Container maxWidth="xl">
            <Box sx={{ py: 4 }}>
              <Stack direction="row" alignItems="center" justifyContent="space-between">
                <Stack direction="row" alignItems="center" spacing={2}>
                  <Avatar
                    sx={{
                      bgcolor: 'white',
                      color: COLORS.primary,
                      width: 64,
                      height: 64,
                    }}
                  >
                    <HomeIcon sx={{ fontSize: 32 }} />
                  </Avatar>
                  <Box>
                    <Typography variant="h4" sx={{ fontWeight: 700 }}>
                      إدارة الصفحة الرئيسية
                    </Typography>
                    <Typography variant="body1" sx={{ opacity: 0.9, mt: 0.5 }}>
                      تخصيص وإدارة أقسام الصفحة الرئيسية
                    </Typography>
                  </Box>
                </Stack>

                <Stack direction="row" spacing={2}>
                  <Button
                    variant="contained"
                    startIcon={<PublishIcon />}
                    onClick={handlePublishConfig}
                    disabled={!config}
                    sx={{
                      bgcolor: 'white',
                      color: COLORS.primary,
                      '&:hover': {
                        bgcolor: alpha('#fff', 0.9),
                      },
                    }}
                  >
                    نشر التكوين
                  </Button>
                  <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={handleCreateSection}
                    sx={{
                      bgcolor: COLORS.success,
                      '&:hover': {
                        bgcolor: alpha(COLORS.success, 0.9),
                      },
                    }}
                  >
                    إضافة قسم جديد
                  </Button>
                </Stack>
              </Stack>
            </Box>
          </Container>
        </Paper>

        <Container maxWidth="xl">
          {/* Statistics Cards */}
          <Grid container spacing={3} sx={{ mb: 3 }}>
            <Grid item xs={12} sm={6} md={2.4}>
              <Card
                elevation={0}
                sx={{
                  border: `1px solid ${COLORS.border}`,
                  borderTop: `3px solid ${COLORS.info}`,
                }}
              >
                <CardContent sx={{ textAlign: 'center' }}>
                  <Typography variant="h3" sx={{ fontWeight: 700, color: COLORS.info }}>
                    {statistics.total}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    إجمالي الأقسام
                  </Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} sm={6} md={2.4}>
              <Card
                elevation={0}
                sx={{
                  border: `1px solid ${COLORS.border}`,
                  borderTop: `3px solid ${COLORS.success}`,
                }}
              >
                <CardContent sx={{ textAlign: 'center' }}>
                  <Typography variant="h3" sx={{ fontWeight: 700, color: COLORS.success }}>
                    {statistics.active}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    أقسام نشطة
                  </Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} sm={6} md={2.4}>
              <Card
                elevation={0}
                sx={{
                  border: `1px solid ${COLORS.border}`,
                  borderTop: `3px solid ${COLORS.text.disabled}`,
                }}
              >
                <CardContent sx={{ textAlign: 'center' }}>
                  <Typography variant="h3" sx={{ fontWeight: 700, color: COLORS.text.disabled }}>
                    {statistics.inactive}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    أقسام غير نشطة
                  </Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} sm={6} md={2.4}>
              <Card
                elevation={0}
                sx={{
                  border: `1px solid ${COLORS.border}`,
                  borderTop: `3px solid ${COLORS.warning}`,
                }}
              >
                <CardContent sx={{ textAlign: 'center' }}>
                  <Typography variant="h3" sx={{ fontWeight: 700, color: COLORS.warning }}>
                    {statistics.scheduled}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    أقسام مجدولة
                  </Typography>
                </CardContent>
              </Card>
            </Grid>

            <Grid item xs={12} sm={6} md={2.4}>
              <Card
                elevation={0}
                sx={{
                  border: `1px solid ${COLORS.border}`,
                  borderTop: `3px solid ${COLORS.error}`,
                }}
              >
                <CardContent sx={{ textAlign: 'center' }}>
                  <Typography variant="h3" sx={{ fontWeight: 700, color: COLORS.error }}>
                    {statistics.expired}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    أقسام منتهية
                  </Typography>
                </CardContent>
              </Card>
            </Grid>
          </Grid>

          {/* Filters and Actions */}
          <Paper
            elevation={0}
            sx={{
              p: 2,
              mb: 3,
              border: `1px solid ${COLORS.border}`,
            }}
          >
            <Stack direction="row" alignItems="center" justifyContent="space-between" flexWrap="wrap" gap={2}>
              <Stack direction="row" spacing={2} alignItems="center">
                <TextField
                  placeholder="البحث في الأقسام..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  size="small"
                  sx={{ minWidth: 300 }}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <SearchIcon sx={{ color: COLORS.text.disabled }} />
                      </InputAdornment>
                    ),
                  }}
                />

                <ToggleButtonGroup
                  value={filter}
                  exclusive
                  onChange={(_, value) => value && setFilter(value)}
                  size="small"
                >
                  <ToggleButton value="all">
                    <Stack direction="row" spacing={0.5} alignItems="center">
                      <Typography variant="caption">الكل</Typography>
                      <Chip size="small" label={sections.length} />
                    </Stack>
                  </ToggleButton>
                  <ToggleButton value="active">
                    <Stack direction="row" spacing={0.5} alignItems="center">
                      <Typography variant="caption">نشط</Typography>
                      <Chip size="small" label={statistics.active} color="success" />
                    </Stack>
                  </ToggleButton>
                  <ToggleButton value="inactive">
                    <Stack direction="row" spacing={0.5} alignItems="center">
                      <Typography variant="caption">غير نشط</Typography>
                      <Chip size="small" label={statistics.inactive} />
                    </Stack>
                  </ToggleButton>
                </ToggleButtonGroup>
              </Stack>

              <Stack direction="row" spacing={1}>
                <ToggleButtonGroup
                  value={viewMode}
                  exclusive
                  onChange={(_, value) => value && setViewMode(value)}
                  size="small"
                >
                  <ToggleButton value="cards">
                    <ViewModuleIcon />
                  </ToggleButton>
                  <ToggleButton value="list">
                    <ViewListIcon />
                  </ToggleButton>
                </ToggleButtonGroup>

                <Button
                  variant="outlined"
                  startIcon={<RefreshIcon />}
                  onClick={() => refetch()}
                  disabled={isLoading}
                  size="small"
                >
                  تحديث
                </Button>
              </Stack>
            </Stack>
          </Paper>

          {/* Sections List */}
          {isLoading ? (
            <Stack spacing={2}>
              {[1, 2, 3].map((i) => (
                <Skeleton key={i} variant="rectangular" height={120} sx={{ borderRadius: 2 }} />
              ))}
            </Stack>
          ) : filteredSections.length === 0 ? (
            <Paper
              elevation={0}
              sx={{
                p: 8,
                textAlign: 'center',
                border: `1px solid ${COLORS.border}`,
              }}
            >
              <Box sx={{ mb: 3 }}>
                <DashboardIcon sx={{ fontSize: 80, color: COLORS.text.disabled }} />
              </Box>
              <Typography variant="h5" sx={{ mb: 2, fontWeight: 600 }}>
                {searchTerm ? 'لا توجد نتائج للبحث' : 'لا توجد أقسام'}
              </Typography>
              <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
                {searchTerm
                  ? 'جرب تعديل مصطلح البحث أو إزالة المرشحات'
                  : 'ابدأ بإنشاء قسم جديد للصفحة الرئيسية'}
              </Typography>
              {!searchTerm && (
                <Button
                  variant="contained"
                  startIcon={<AddIcon />}
                  onClick={handleCreateSection}
                  size="large"
                  sx={{ bgcolor: COLORS.primary }}
                >
                  إنشاء أول قسم
                </Button>
              )}
            </Paper>
          ) : (
            <DndContext
              sensors={sensors}
              collisionDetection={closestCenter}
              onDragEnd={handleDragEnd}
            >
              <SortableContext
                items={filteredSections.map(section => section.id)}
                strategy={verticalListSortingStrategy}
              >
                {filteredSections.map((section, index) => (
                  <SortableItem key={section.id} id={section.id}>
                    <SectionCard
                      section={section}
                      index={index}
                      onEdit={() => handleEditSection(section)}
                      onDelete={() => handleDeleteSection(section)}
                      onToggle={() => handleToggleSection(section)}
                      onPreview={() => handlePreviewSection(section)}
                      onDuplicate={() => handleDuplicateSection(section)}
                      onMenuClick={(e) => handleMenuClick(e, section.id)}
                    />
                  </SortableItem>
                ))}
              </SortableContext>
            </DndContext>
          )}
        </Container>

        {/* Section Configuration Dialog */}
        <SectionConfigDialog
          open={configDialogOpen}
          onClose={() => setConfigDialogOpen(false)}
          section={selectedSection}
          isEdit={isEditMode}
          onSave={handleSaveSection}
        />

        {/* Context Menu */}
        <Menu
          anchorEl={menuAnchor}
          open={Boolean(menuAnchor)}
          onClose={handleMenuClose}
        >
          <MenuItem
            onClick={() => {
              const section = sections.find(s => s.id === selectedSectionId);
              if (section) handleDuplicateSection(section);
              handleMenuClose();
            }}
          >
            <Stack direction="row" spacing={1} alignItems="center">
              <DuplicateIcon fontSize="small" />
              <Typography>نسخ القسم</Typography>
            </Stack>
          </MenuItem>
          <Divider />
          <MenuItem
            onClick={() => {
              const section = sections.find(s => s.id === selectedSectionId);
              if (section) handleDeleteSection(section);
              handleMenuClose();
            }}
            sx={{ color: COLORS.error }}
          >
            <Stack direction="row" spacing={1} alignItems="center">
              <DeleteIcon fontSize="small" />
              <Typography>حذف</Typography>
            </Stack>
          </MenuItem>
        </Menu>

        {/* Notification Snackbar */}
        <Snackbar
          open={notification.open}
          autoHideDuration={6000}
          onClose={() => setNotification({ ...notification, open: false })}
        >
          <Alert
            onClose={() => setNotification({ ...notification, open: false })}
            severity={notification.severity}
            sx={{ width: '100%' }}
          >
            {notification.message}
          </Alert>
        </Snackbar>

        {/* Speed Dial for Quick Actions */}
        <SpeedDial
          ariaLabel="إجراءات سريعة"
          sx={{ position: 'fixed', bottom: 24, left: 24 }}
          icon={<SpeedDialIcon />}
        >
          <SpeedDialAction
            icon={<AddIcon />}
            tooltipTitle="إضافة قسم"
            onClick={handleCreateSection}
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
        </SpeedDial>
      </Box>
    </LocalizationProvider>
  );
};

export default HomeScreenManagement;
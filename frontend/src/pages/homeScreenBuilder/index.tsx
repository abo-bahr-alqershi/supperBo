// pages/HomeScreenBuilder/index.tsx

import React, { useState, useCallback, useEffect, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  IconButton,
  Button,
  Typography,
  Tooltip,
  Divider,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  SpeedDial,
  SpeedDialAction,
  Badge,
  Chip,
  Stack,
  useTheme,
  useMediaQuery,
  Drawer,
  Paper,
  Zoom,
  Fab,
  Alert
} from '@mui/material';
import {
  Save as SaveIcon,
  Undo as UndoIcon,
  Redo as RedoIcon,
  Preview as PreviewIcon,
  Publish as PublishIcon,
  Menu as MenuIcon,
  Close as CloseIcon,
  Add as AddIcon,
  Palette as PaletteIcon,
  Settings as SettingsIcon,
  Help as HelpIcon,
  Smartphone as MobileIcon,
  Tablet as TabletIcon,
  Computer as DesktopIcon,
  DarkMode as DarkModeIcon,
  LightMode as LightModeIcon,
  Language as LanguageIcon,
  CloudDone as CloudDoneIcon
} from '@mui/icons-material';
import { DndContext, PointerSensor, TouchSensor, useSensor, useSensors, DragOverlay } from '@dnd-kit/core';
import { motion, AnimatePresence } from 'framer-motion';
import { toast, Toaster } from 'react-hot-toast';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import rtlPlugin from 'stylis-plugin-rtl';
import { prefixer } from 'stylis';
import { CacheProvider } from '@emotion/react';
import createCache from '@emotion/cache';

// Import hooks
import { useHomeScreenBuilder } from '../../hooks/useHomeScreenBuilder';
import { usePreview } from '../../hooks/usePreview';

// Import components
import Canvas from '../../components/HomeScreenBuilder/Canvas';
import ComponentPalette from '../../components/HomeScreenBuilder/ComponentPalette';
import PropertyPanel from '../../components/HomeScreenBuilder/PropertyPanel';
import PreviewModal from '../../components/HomeScreenBuilder/PreviewModal';
import TemplateManager from '../../components/HomeScreenBuilder/TemplateManager';

// Import styles
import styles from './HomeScreenBuilder.module.css';

// Import types
import type { Platform } from '../../types/homeScreen.types';

// Constants
const AUTO_SAVE_DELAY = 30000; // 30 seconds

// Create RTL cache
const cacheRtl = createCache({
  key: 'muirtl',
  stylisPlugins: [prefixer, rtlPlugin],
});

interface RouteParams extends Record<string, string | undefined> {
  templateId?: string;
}

const HomeScreenBuilder: React.FC = () => {
  const baseTheme = useTheme();
  const navigate = useNavigate();
  const { templateId } = useParams<RouteParams>();
  const isMobile = useMediaQuery(baseTheme.breakpoints.down('md'));
  const isTablet = useMediaQuery(baseTheme.breakpoints.down('lg'));
  
  // State
  const [leftDrawerOpen, setLeftDrawerOpen] = useState(!isMobile);
  const [rightDrawerOpen, setRightDrawerOpen] = useState(!isMobile && !isTablet);
  const [previewOpen, setPreviewOpen] = useState(false);
  const [templateManagerOpen, setTemplateManagerOpen] = useState(false);
  const [publishDialogOpen, setPublishDialogOpen] = useState(false);
  const [darkMode, setDarkMode] = useState(false);
  const [platform, setPlatform] = useState<Platform>('All');
  const [deviceType, setDeviceType] = useState<'mobile' | 'tablet' | 'desktop'>('mobile');
  const [autoSaveEnabled, setAutoSaveEnabled] = useState(true);
  const [lastAutoSave, setLastAutoSave] = useState<Date | null>(null);
  const [activeId, setActiveId] = useState<string | null>(null);
  
  // Create RTL theme
  const theme = React.useMemo(
    () =>
      createTheme({
        ...baseTheme,
        direction: 'rtl',
        palette: {
          ...baseTheme.palette,
          mode: darkMode ? 'dark' : 'light',
        },
      }),
    [baseTheme, darkMode]
  );
  
  // Use hooks
  const {
    template,
    componentTypes,
    selectedComponentId,
    selectedSectionId,
    isDirty,
    lastSaveTime,
    isLoading,
    error,
    isSaving,
    isPublishing,
    
    // Actions
    saveTemplate,
    addSection,
    deleteTemplate,
    publishTemplate,
    selectComponent,
    addComponent,
    moveComponent,
  } = useHomeScreenBuilder({
    templateId,
    autoSave: autoSaveEnabled,
    autoSaveInterval: AUTO_SAVE_DELAY
  });
  
  const {
    refreshPreview
  } = usePreview({
    template,
    platform,
    deviceType,
    useMockData: true
  });
  
  // Configure drag sensors
  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8,
      },
    }),
    useSensor(TouchSensor, {
      activationConstraint: {
        delay: 200,
        tolerance: 5,
      },
    })
  );
  
  // Drag handlers
  const handleDragStart = (event: any) => {
    setActiveId(event.active.id);
  };
  
  const handleDragEnd = (event: any) => {
    const { active, over } = event;
    if (!over) {
      setActiveId(null);
      return;
    }

    const activeData = active.data.current;
    const overData = over.data.current;

    // Add new component to section
    if (activeData?.type === 'new-component' && overData?.type === 'section') {
      addComponent(overData.sectionId, activeData.componentType);
    }
    // Move existing component between sections
    else if (activeData?.type === 'existing-component' && overData?.type === 'section') {
      moveComponent(activeData.id, activeData.sourceSectionId, overData.sectionId, 0);
    }

    setActiveId(null);
  };
  
  // Auto-save effect
  useEffect(() => {
    if (!autoSaveEnabled || !isDirty || !template) return;
    
    const timer = setTimeout(() => {
      handleSave();
      setLastAutoSave(new Date());
    }, AUTO_SAVE_DELAY);
    
    return () => clearTimeout(timer);
  }, [autoSaveEnabled, isDirty, template]);
  
  // Handlers
  const handleSave = useCallback(async () => {
    if (!template || !isDirty) return;
    
    try {
      await saveTemplate();
      toast.success('تم حفظ القالب بنجاح');
    } catch (error) {
      toast.error('فشل حفظ القالب');
    }
  }, [template, isDirty, saveTemplate]);
  
  const handlePreview = useCallback(() => {
    setPreviewOpen(true);
    refreshPreview();
  }, [refreshPreview]);
  
  const handlePublish = useCallback(async () => {
    if (!template) return;
    
    try {
      setPublishDialogOpen(false);
      await publishTemplate({ id: template.id, deactivateOthers: true });
      toast.success('تم نشر القالب بنجاح!');
    } catch (error) {
      toast.error('فشل نشر القالب');
    }
  }, [template, publishTemplate]);
  
  const handleCreateTemplate = useCallback(() => {
    setTemplateManagerOpen(true);
  }, []);
  
  const handlePlatformChange = useCallback((newPlatform: Platform) => {
    setPlatform(newPlatform);
    const platformNames = {
      'All': 'جميع المنصات',
      'iOS': 'iOS',
      'Android': 'Android'
    };
    toast.success(`تم التبديل إلى ${platformNames[newPlatform]}`);
  }, []);
  
  const handleDeviceChange = useCallback((newDevice: 'mobile' | 'tablet' | 'desktop') => {
    setDeviceType(newDevice);
    const deviceNames = {
      'mobile': 'الهاتف',
      'tablet': 'التابلت',
      'desktop': 'سطح المكتب'
    };
    toast.success(`تم التبديل إلى ${deviceNames[newDevice]}`);
  }, []);
  
  // Render loading state
  if (isLoading) {
    return (
      <Box className={styles.loadingContainer}>
        <CircularProgress size={48} />
        <Typography variant="h6" sx={{ mt: 2 }}>
          جاري تحميل القالب...
        </Typography>
      </Box>
    );
  }
  
  // Render error state
  if (error) {
    return (
      <Box className={styles.errorContainer}>
        <Typography variant="h5" gutterBottom>
          خطأ في تحميل القالب
        </Typography>
        <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
          {error.message || 'حدث خطأ غير متوقع'}
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleCreateTemplate}
        >
          إنشاء قالب جديد
        </Button>
      </Box>
    );
  }
  
  // Render empty state
  if (!template && !templateId) {
    return (
      <>
        <Box className={styles.emptyContainer}>
          <PaletteIcon sx={{ fontSize: 80, color: 'primary.main', mb: 3 }} />
          <Typography variant="h4" gutterBottom>
            مرحباً بك في منشئ الشاشة الرئيسية
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mb: 4, maxWidth: 600 }}>
            قم بإنشاء شاشات رئيسية جميلة ومتجاوبة لتطبيقك. 
            اختر من القوالب الجاهزة أو ابدأ من الصفر.
          </Typography>
          <Button
            variant="contained"
            size="large"
            startIcon={<AddIcon />}
            onClick={handleCreateTemplate}
          >
            إنشاء أول قالب لك
          </Button>
        </Box>
        <TemplateManager
          open={templateManagerOpen}
          onClose={() => setTemplateManagerOpen(false)}
          currentTemplateId={templateId}
        />
      </>
    );
  }
  
  return (
    <CacheProvider value={cacheRtl}>
      <ThemeProvider theme={theme}>
        <DndContext 
          sensors={sensors}
          onDragStart={handleDragStart}
          onDragEnd={handleDragEnd}
        >
          <Box className={styles.container} sx={{ userSelect: 'none' }}>
            {/* Header Toolbar */}
            <Paper elevation={2} className={styles.headerToolbar}>
              <Stack direction="row" spacing={2} alignItems="center" sx={{ p: 2 }}>
                <IconButton
                  onClick={() => setLeftDrawerOpen(!leftDrawerOpen)}
                  sx={{ ml: 0 }}
                >
                  <MenuIcon />
                </IconButton>
                
                <Typography variant="h6" sx={{ flexGrow: 0 }}>
                  منشئ الشاشة الرئيسية
                </Typography>
                
                {template && (
                  <Chip
                    label={template.name}
                    color="primary"
                    variant="outlined"
                  />
                )}
                
                <Box sx={{ flexGrow: 1 }} />
                
                {/* Platform & Device Selectors */}
                <Stack direction="row" spacing={1}>
                  <Tooltip title="المنصة">
                    <IconButton
                      onClick={() => {
                        const platforms: Platform[] = ['All', 'iOS', 'Android'];
                        const currentIndex = platforms.indexOf(platform);
                        const nextIndex = (currentIndex + 1) % platforms.length;
                        handlePlatformChange(platforms[nextIndex]);
                      }}
                    >
                      <Badge badgeContent={platform} color="primary">
                        <LanguageIcon />
                      </Badge>
                    </IconButton>
                  </Tooltip>
                  
                  <Tooltip title="نوع الجهاز">
                    <IconButton
                      onClick={() => {
                        const devices: Array<'mobile' | 'tablet' | 'desktop'> = ['mobile', 'tablet', 'desktop'];
                        const currentIndex = devices.indexOf(deviceType);
                        const nextIndex = (currentIndex + 1) % devices.length;
                        handleDeviceChange(devices[nextIndex]);
                      }}
                    >
                      {deviceType === 'mobile' ? <MobileIcon /> :
                       deviceType === 'tablet' ? <TabletIcon /> :
                       <DesktopIcon />}
                    </IconButton>
                  </Tooltip>
                </Stack>
                
                <Divider orientation="vertical" flexItem />
                
                {/* Save Status */}
                {isDirty && (
                  <Chip
                    label="تغييرات غير محفوظة"
                    size="small"
                    color="warning"
                  />
                )}
                
                {lastSaveTime && !isDirty && (
                  <Tooltip title={`آخر حفظ: ${lastSaveTime.toLocaleTimeString('ar')}`}>
                    <CloudDoneIcon color="success" />
                  </Tooltip>
                )}
                
                {/* Action Buttons */}
                <Button
                  variant="outlined"
                  startIcon={<SaveIcon />}
                  onClick={handleSave}
                  disabled={!isDirty || isSaving}
                >
                  {isSaving ? 'جاري الحفظ...' : 'حفظ'}
                </Button>
                
                <Button
                  variant="outlined"
                  startIcon={<PreviewIcon />}
                  onClick={handlePreview}
                >
                  معاينة
                </Button>
                
                <Button
                  variant="contained"
                  color="success"
                  startIcon={<PublishIcon />}
                  onClick={() => setPublishDialogOpen(true)}
                  disabled={isPublishing || isDirty}
                >
                  نشر
                </Button>
                
                <IconButton
                  onClick={() => setRightDrawerOpen(!rightDrawerOpen)}
                >
                  <SettingsIcon />
                </IconButton>
              </Stack>
            </Paper>
            
            {/* Main Layout */}
            <Box className={styles.mainLayout}>
              {/* Left Drawer - Component Palette */}
              <Drawer
                variant={isMobile ? 'temporary' : 'persistent'}
                anchor="right"
                open={leftDrawerOpen}
                onClose={() => setLeftDrawerOpen(false)}
                sx={{
                  '& .MuiDrawer-paper': {
                    width: 280,
                    position: 'relative',
                    height: '100%',
                    borderLeft: 'none',
                    borderRight: '1px solid',
                    borderColor: 'divider'
                  }
                }}
              >
                <Box className={styles.drawerHeader}>
                  <Typography variant="h6">المكونات</Typography>
                  <IconButton onClick={() => setLeftDrawerOpen(false)}>
                    <CloseIcon />
                  </IconButton>
                                  </Box>
                <Divider />
                <ComponentPalette componentTypes={componentTypes} />
              </Drawer>
              
              {/* Canvas Area */}
              <Box 
                className={styles.canvasArea}
                sx={{
                  flex: 1,
                  overflow: 'auto',
                  backgroundColor: 'grey.100',
                  p: 3,
                  mr: rightDrawerOpen && !isMobile && !isTablet ? '320px' : 0,
                  ml: leftDrawerOpen && !isMobile ? '280px' : 0,
                  transition: theme.transitions.create(['margin'], {
                    easing: theme.transitions.easing.sharp,
                    duration: theme.transitions.duration.leavingScreen,
                  })
                }}
              >
                <Canvas
                  template={template}
                  selectedComponentId={selectedComponentId}
                  selectedSectionId={selectedSectionId}
                  onSelectComponent={selectComponent}
                  onAddSection={addSection}
                />
              </Box>
              
              {/* Right Drawer - Property Panel */}
              <Drawer
                variant={isMobile || isTablet ? 'temporary' : 'persistent'}
                anchor="left"
                open={rightDrawerOpen}
                onClose={() => setRightDrawerOpen(false)}
                sx={{
                  '& .MuiDrawer-paper': {
                    width: 320,
                    position: 'relative',
                    height: '100%',
                    borderRight: 'none',
                    borderLeft: '1px solid',
                    borderColor: 'divider'
                  }
                }}
              >
                <Box className={styles.drawerHeader}>
                  <Typography variant="h6">الخصائص</Typography>
                  <IconButton onClick={() => setRightDrawerOpen(false)}>
                    <CloseIcon />
                  </IconButton>
                </Box>
                <Divider />
                <PropertyPanel
                  componentId={selectedComponentId}
                  templateId={template?.id}
                />
              </Drawer>
            </Box>
            
            {/* DragOverlay for better drag preview */}
            <DragOverlay>
              {activeId ? (
                <Box
                  sx={{
                    p: 2,
                    backgroundColor: 'primary.main',
                    color: 'primary.contrastText',
                    borderRadius: 1,
                    boxShadow: 3,
                    cursor: 'grabbing',
                    opacity: 0.9
                  }}
                >
                  <Typography variant="body2">
                    {activeId.startsWith('palette-')
                      ? componentTypes.find(ct => `palette-${ct.type}` === activeId)?.name || 'مكون'
                      : 'سحب العنصر'
                    }
                  </Typography>
                </Box>
              ) : null}
            </DragOverlay>
            
            {/* Preview Modal */}
            <PreviewModal
              template={template!}
              open={previewOpen}
              onClose={() => setPreviewOpen(false)}
            />
            
            {/* Template Manager */}
            <TemplateManager
              open={templateManagerOpen}
              onClose={() => setTemplateManagerOpen(false)}
              currentTemplateId={templateId}
            />
            
            {/* Publish Dialog */}
            <Dialog
              open={publishDialogOpen}
              onClose={() => setPublishDialogOpen(false)}
              maxWidth="sm"
              fullWidth
            >
              <DialogTitle>نشر القالب</DialogTitle>
              <DialogContent>
                <Typography variant="body1" paragraph>
                  هل أنت متأكد من نشر هذا القالب؟
                </Typography>
                <Alert severity="info" sx={{ mb: 2 }}>
                  سيؤدي النشر إلى تفعيل هذا القالب لجميع المستخدمين. 
                  سيتم إلغاء تفعيل القالب النشط حالياً.
                </Alert>
                <Typography variant="body2" color="text.secondary">
                  القالب: <strong>{template?.name}</strong>
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  المنصة: <strong>{template?.platform}</strong>
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  الجمهور المستهدف: <strong>{template?.targetAudience}</strong>
                </Typography>
              </DialogContent>
              <DialogActions>
                <Button onClick={() => setPublishDialogOpen(false)}>
                  إلغاء
                </Button>
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handlePublish}
                  disabled={isPublishing}
                  startIcon={isPublishing ? <CircularProgress size={16} /> : <PublishIcon />}
                >
                  {isPublishing ? 'جاري النشر...' : 'نشر'}
                </Button>
              </DialogActions>
            </Dialog>
            
            {/* Status Bar */}
            <Paper
              elevation={0}
              className={styles.statusBar}
              sx={{
                borderTop: '1px solid',
                borderColor: 'divider',
                position: 'fixed',
                bottom: 0,
                left: 0,
                right: 0,
                zIndex: 1000
              }}
            >
              <Stack
                direction="row"
                spacing={2}
                alignItems="center"
                sx={{ px: 2, py: 1 }}
              >
                <Typography variant="caption" color="text.secondary">
                  {template ? `${template.sections.length} أقسام` : '0 أقسام'}
                </Typography>
                <Divider orientation="vertical" flexItem />
                <Typography variant="caption" color="text.secondary">
                  {template ? `${template.sections.reduce((acc, s) => acc + s.components.length, 0)} مكونات` : '0 مكونات'}
                </Typography>
                <Divider orientation="vertical" flexItem />
                <Typography variant="caption" color="text.secondary">
                  المنصة: {platform === 'All' ? 'الجميع' : platform}
                </Typography>
                <Divider orientation="vertical" flexItem />
                <Typography variant="caption" color="text.secondary">
                  الجهاز: {deviceType === 'mobile' ? 'الهاتف' : deviceType === 'tablet' ? 'التابلت' : 'سطح المكتب'}
                </Typography>
                {autoSaveEnabled && (
                  <>
                    <Divider orientation="vertical" flexItem />
                    <Typography variant="caption" color="text.secondary">
                      الحفظ التلقائي: {lastAutoSave ? `آخر ${lastAutoSave.toLocaleTimeString('ar')}` : 'مفعل'}
                    </Typography>
                  </>
                )}
                <Box sx={{ flexGrow: 1 }} />
                <Tooltip title="تبديل الوضع الليلي">
                  <IconButton
                    size="small"
                    onClick={() => setDarkMode(!darkMode)}
                  >
                    {darkMode ? <LightModeIcon fontSize="small" /> : <DarkModeIcon fontSize="small" />}
                  </IconButton>
                </Tooltip>
              </Stack>
            </Paper>
            
            {/* Floating Action Button for Mobile */}
            {isMobile && (
              <SpeedDial
                ariaLabel="الإجراءات"
                sx={{ position: 'fixed', bottom: 70, right: 16 }}
                icon={<AddIcon />}
              >
                <SpeedDialAction
                  icon={<AddIcon />}
                  tooltipTitle="إضافة قسم"
                  onClick={() => {
                    const name = prompt('اسم القسم:');
                    if (name) {
                      addSection(name, name);
                    }
                  }}
                />
                <SpeedDialAction
                  icon={<PaletteIcon />}
                  tooltipTitle="المكونات"
                  onClick={() => setLeftDrawerOpen(true)}
                />
                <SpeedDialAction
                  icon={<SettingsIcon />}
                  tooltipTitle="الخصائص"
                  onClick={() => setRightDrawerOpen(true)}
                />
                <SpeedDialAction
                  icon={<PreviewIcon />}
                  tooltipTitle="معاينة"
                  onClick={handlePreview}
                />
              </SpeedDial>
            )}
            
            {/* Toast Notifications */}
            <Toaster
              position="bottom-left"
              reverseOrder={false}
              toastOptions={{
                duration: 4000,
                style: {
                  background: theme.palette.background.paper,
                  color: theme.palette.text.primary,
                  direction: 'rtl'
                }
              }}
            />
            
            {/* Help FAB */}
            <Zoom in={!isMobile}>
              <Fab
                color="primary"
                size="small"
                sx={{
                  position: 'fixed',
                  bottom: 70,
                  left: 16
                }}
                onClick={() => window.open('/docs/home-screen-builder', '_blank')}
              >
                <HelpIcon />
              </Fab>
            </Zoom>
          </Box>
        </DndContext>
      </ThemeProvider>
    </CacheProvider>
  );
};

export default HomeScreenBuilder;
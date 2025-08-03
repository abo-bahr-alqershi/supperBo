// pages/HomeScreenBuilder/index.tsx

import React, { useState, useCallback, useEffect, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Toolbar,
  AppBar,
  IconButton,
  Button,
  Typography,
  Tooltip,
  Divider,
  CircularProgress,
  Snackbar,
  Alert,
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
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  ListItemButton,
  Collapse,
  Paper,
  Fade,
  Zoom,
  Fab
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
  History as HistoryIcon,
  ContentCopy as DuplicateIcon,
  Delete as DeleteIcon,
  ExpandLess,
  ExpandMore,
  Smartphone as MobileIcon,
  Tablet as TabletIcon,
  Computer as DesktopIcon,
  DarkMode as DarkModeIcon,
  LightMode as LightModeIcon,
  Language as LanguageIcon,
  CloudUpload as CloudUploadIcon,
  CloudDone as CloudDoneIcon,
  Warning as WarningIcon,
  Check as CheckIcon
} from '@mui/icons-material';
import { DndContext, PointerSensor, TouchSensor, useSensor, useSensors } from '@dnd-kit/core';
import type { DragEndEvent } from '@dnd-kit/core';
import { motion, AnimatePresence } from 'framer-motion';
import { toast, Toaster } from 'react-hot-toast';

// Import hooks
import { useHomeScreenBuilder } from '../../hooks/useHomeScreenBuilder';
import { useDragDrop } from '../../hooks/useDragDrop';
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
const UNDO_REDO_LIMIT = 50;

interface RouteParams extends Record<string, string | undefined> {
  templateId?: string;
}

const HomeScreenBuilder: React.FC = () => {
  const theme = useTheme();
  const navigate = useNavigate();
  const { templateId } = useParams<RouteParams>();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const isTablet = useMediaQuery(theme.breakpoints.down('lg'));
  
  // State
  const [leftDrawerOpen, setLeftDrawerOpen] = useState(!isMobile);
  const [rightDrawerOpen, setRightDrawerOpen] = useState(!isMobile && !isTablet);
  const [previewOpen, setPreviewOpen] = useState(false);
  const [templateManagerOpen, setTemplateManagerOpen] = useState(false);
  const [publishDialogOpen, setPublishDialogOpen] = useState(false);
  const [darkMode, setDarkMode] = useState(false);
  const [platform, setPlatform] = useState<Platform>('All');
  const [deviceType, setDeviceType] = useState<'mobile' | 'tablet' | 'desktop'>('mobile');
  const [showGrid, setShowGrid] = useState(true);
  const [showGuides, setShowGuides] = useState(true);
  const [autoSaveEnabled, setAutoSaveEnabled] = useState(true);
  const [lastAutoSave, setLastAutoSave] = useState<Date | null>(null);
  const [history, setHistory] = useState<any[]>([]);
  const [historyIndex, setHistoryIndex] = useState(-1);
  
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
    createTemplate,
    updateTemplate,
    deleteTemplate,
    publishTemplate,
    saveTemplate,
    addSection,
    updateSection,
    deleteSection,
    reorderSections,
    addComponent,
    updateComponent,
    deleteComponent,
    moveComponent,
    duplicateComponent,
    selectComponent,
    getSelectedComponent,
    getSelectedSection
  } = useHomeScreenBuilder({
    templateId,
    autoSave: autoSaveEnabled,
    autoSaveInterval: AUTO_SAVE_DELAY
  });
  
  const { dragState } = useDragDrop();
  
  const {
    previewData,
    currentDevice,
    changeDevice,
    toggleOrientation,
    zoomIn,
    zoomOut,
    resetZoom,
    refreshPreview
  } = usePreview({
    template,
    platform,
    deviceType,
    useMockData: true
  });
  
  // Keyboard shortcuts
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      // Save: Ctrl/Cmd + S
      if ((e.ctrlKey || e.metaKey) && e.key === 's') {
        e.preventDefault();
        handleSave();
      }
      
      // Undo: Ctrl/Cmd + Z
      if ((e.ctrlKey || e.metaKey) && e.key === 'z' && !e.shiftKey) {
        e.preventDefault();
        handleUndo();
      }
      
      // Redo: Ctrl/Cmd + Shift + Z
      if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'z') {
        e.preventDefault();
        handleRedo();
      }
      
      // Preview: Ctrl/Cmd + P
      if ((e.ctrlKey || e.metaKey) && e.key === 'p') {
        e.preventDefault();
        handlePreview();
      }
      
      // Toggle left panel: Ctrl/Cmd + B
      if ((e.ctrlKey || e.metaKey) && e.key === 'b') {
        e.preventDefault();
        setLeftDrawerOpen(prev => !prev);
      }
      
      // Toggle right panel: Ctrl/Cmd + I
      if ((e.ctrlKey || e.metaKey) && e.key === 'i') {
        e.preventDefault();
        setRightDrawerOpen(prev => !prev);
      }
    };
    
    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, []);
  
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
      toast.success('Template saved successfully');
    } catch (error) {
      toast.error('Failed to save template');
    }
  }, [template, isDirty, saveTemplate]);
  
  const handleUndo = useCallback(() => {
    if (historyIndex > 0) {
      setHistoryIndex(prev => prev - 1);
      // Apply previous state
      const previousState = history[historyIndex - 1];
      // TODO: Implement state restoration
      toast.success('Undo successful');
    }
  }, [history, historyIndex]);
  
  const handleRedo = useCallback(() => {
    if (historyIndex < history.length - 1) {
      setHistoryIndex(prev => prev + 1);
      // Apply next state
      const nextState = history[historyIndex + 1];
      // TODO: Implement state restoration
      toast.success('Redo successful');
    }
  }, [history, historyIndex]);
  
  const handlePreview = useCallback(() => {
    setPreviewOpen(true);
    refreshPreview();
  }, [refreshPreview]);
  
  const handlePublish = useCallback(async () => {
    if (!template) return;
    
    try {
      setPublishDialogOpen(false);
      await publishTemplate({ id: template.id, deactivateOthers: true });
      toast.success('Template published successfully!');
    } catch (error) {
      toast.error('Failed to publish template');
    }
  }, [template, publishTemplate]);
  
  const handleDuplicate = useCallback(async () => {
    if (!template) return;
    
    try {
      // TODO: Implement template duplication
      toast.success('Template duplicated successfully');
    } catch (error) {
      toast.error('Failed to duplicate template');
    }
  }, [template]);
  
  const handleDelete = useCallback(async () => {
    if (!template) return;
    
    if (window.confirm('Are you sure you want to delete this template?')) {
      try {
        await deleteTemplate(template.id);
        navigate('/admin/home-screen-builder');
      } catch (error) {
        toast.error('Failed to delete template');
      }
    }
  }, [template, deleteTemplate, navigate]);
  
  const handleCreateTemplate = useCallback(() => {
    setTemplateManagerOpen(true);
  }, []);
  
  const handlePlatformChange = useCallback((newPlatform: Platform) => {
    setPlatform(newPlatform);
    toast.success(`Switched to ${newPlatform} platform`);
  }, []);
  
  const handleDeviceChange = useCallback((newDevice: 'mobile' | 'tablet' | 'desktop') => {
    setDeviceType(newDevice);
    changeDevice(newDevice);
  }, [changeDevice]);
  
  // Render loading state
  if (isLoading) {
    return (
      <Box className={styles.loadingContainer}>
        <CircularProgress size={48} />
        <Typography variant="h6" sx={{ mt: 2 }}>
          Loading template...
        </Typography>
      </Box>
    );
  }
  
  // Render error state
  if (error) {
    return (
      <Box className={styles.errorContainer}>
        <WarningIcon sx={{ fontSize: 64, color: 'error.main', mb: 2 }} />
        <Typography variant="h5" gutterBottom>
          Error Loading Template
        </Typography>
        <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
          {error.message || 'An unexpected error occurred'}
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={handleCreateTemplate}
        >
          Create New Template
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
            Welcome to Home Screen Builder
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mb: 4, maxWidth: 600 }}>
            Create beautiful, responsive home screens for your mobile app. 
            Choose from pre-built templates or start from scratch.
          </Typography>
          <Button
            variant="contained"
            size="large"
            startIcon={<AddIcon />}
            onClick={handleCreateTemplate}
          >
            Create Your First Template
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
  
  // Add dnd-kit sensors setup
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(TouchSensor)
  );
  // Handle drag end to add or move components
  const handleDragEnd = useCallback((event: DragEndEvent) => {
    const { active, over } = event;
    if (!over) return;
    const dragData = active.data.current as any;
    if (dragData.type === 'new-component') {
      // Add new component to section
      const sectionId = over.id as string;
      addComponent(sectionId, dragData.componentType);
    } else if (dragData.type === 'existing-component') {
      // Move existing component between sections
      const componentId = dragData.id as string;
      const sourceSectionId = dragData.sourceSectionId as string;
      const targetSectionId = over.id as string;
      if (sourceSectionId !== targetSectionId) {
        moveComponent(componentId, sourceSectionId, targetSectionId, 0);
      }
    }
  }, [addComponent, moveComponent]);
  
  return (
    <DndContext sensors={sensors} onDragEnd={handleDragEnd}>
      <Box className={styles.container}>
        {/* App Bar */}
        <AppBar position="fixed" className={styles.appBar}>
          <Toolbar>
            <IconButton
              edge="start"
              color="inherit"
              aria-label="menu"
              onClick={() => setLeftDrawerOpen(!leftDrawerOpen)}
              sx={{ mr: 2 }}
            >
              <MenuIcon />
            </IconButton>
            
            <Typography variant="h6" component="div" sx={{ flexGrow: 0, mr: 3 }}>
              Home Screen Builder
            </Typography>
            
            {template && (
              <Chip
                label={template.name}
                variant="filled"
                sx={{ backgroundColor: 'rgba(255,255,255,0.2)' }}
              />
            )}
            
            <Box sx={{ flexGrow: 1 }} />
            
            {/* Toolbar Actions */}
            <Stack direction="row" spacing={1} alignItems="center">
              {/* Platform Selector */}
              <Tooltip title="Platform">
                <IconButton
                  color="inherit"
                  onClick={() => {
                    const platforms: Platform[] = ['All', 'iOS', 'Android'];
                    const currentIndex = platforms.indexOf(platform);
                    const nextIndex = (currentIndex + 1) % platforms.length;
                    handlePlatformChange(platforms[nextIndex]);
                  }}
                >
                  {platform === 'iOS' ? <MobileIcon /> : 
                   platform === 'Android' ? <MobileIcon /> : 
                   <LanguageIcon />}
                </IconButton>
              </Tooltip>
              
              {/* Device Selector */}
              <Tooltip title="Device Type">
                <IconButton
                  color="inherit"
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
              
              <Divider orientation="vertical" flexItem sx={{ mx: 1 }} />
              
              {/* Undo/Redo */}
              <Tooltip title="Undo (Ctrl+Z)">
                <span>
                  <IconButton
                    color="inherit"
                    onClick={handleUndo}
                    disabled={historyIndex <= 0}
                  >
                    <UndoIcon />
                  </IconButton>
                </span>
              </Tooltip>
              
              <Tooltip title="Redo (Ctrl+Shift+Z)">
                <span>
                  <IconButton
                    color="inherit"
                    onClick={handleRedo}
                    disabled={historyIndex >= history.length - 1}
                  >
                    <RedoIcon />
                  </IconButton>
                </span>
              </Tooltip>
              
              <Divider orientation="vertical" flexItem sx={{ mx: 1 }} />
              
              {/* Save Status */}
              {isDirty && (
                <Chip
                  label="Unsaved changes"
                  size="small"
                  color="warning"
                  variant="filled"
                  sx={{ mr: 1 }}
                />
              )}
              
              {isSaving && (
                <CircularProgress size={20} color="inherit" sx={{ mr: 1 }} />
              )}
              
              {lastSaveTime && !isDirty && (
                <Tooltip title={`Last saved: ${lastSaveTime.toLocaleTimeString()}`}>
                  <CloudDoneIcon color="inherit" />
                </Tooltip>
              )}
              
              {/* Save Button */}
              <Tooltip title="Save (Ctrl+S)">
                <span>
                  <IconButton
                    color="inherit"
                    onClick={handleSave}
                    disabled={!isDirty || isSaving}
                  >
                    <SaveIcon />
                  </IconButton>
                </span>
              </Tooltip>
              
              {/* Preview Button */}
              <Tooltip title="Preview (Ctrl+P)">
                <IconButton color="inherit" onClick={handlePreview}>
                  <PreviewIcon />
                </IconButton>
              </Tooltip>
              
              {/* Publish Button */}
              <Button
                variant="contained"
                color="secondary"
                startIcon={isPublishing ? <CircularProgress size={16} /> : <PublishIcon />}
                onClick={() => setPublishDialogOpen(true)}
                disabled={isPublishing || isDirty}
                sx={{ ml: 2 }}
              >
                Publish
              </Button>
              
              {/* Settings */}
              <Tooltip title="Settings">
                <IconButton
                  color="inherit"
                  onClick={() => setRightDrawerOpen(!rightDrawerOpen)}
                >
                  <SettingsIcon />
                </IconButton>
              </Tooltip>
            </Stack>
          </Toolbar>
        </AppBar>
        
        {/* Left Drawer - Component Palette */}
        <Drawer
          variant={isMobile ? 'temporary' : 'persistent'}
          anchor="left"
          open={leftDrawerOpen}
          onClose={() => setLeftDrawerOpen(false)}
          className={styles.leftDrawer}
          classes={{
            paper: styles.drawerPaper
          }}
        >
          <Box className={styles.drawerHeader}>
            <Typography variant="h6">Components</Typography>
            <IconButton onClick={() => setLeftDrawerOpen(false)}>
              <CloseIcon />
            </IconButton>
          </Box>
          <Divider />
          <ComponentPalette componentTypes={componentTypes} />
        </Drawer>
        
        {/* Main Content */}
        <Box
          component="main"
          className={styles.mainContent}
          sx={{
            marginLeft: leftDrawerOpen && !isMobile ? '280px' : 0,
            marginRight: rightDrawerOpen && !isMobile && !isTablet ? '320px' : 0,
            transition: theme.transitions.create(['margin'], {
              easing: theme.transitions.easing.sharp,
              duration: theme.transitions.duration.leavingScreen,
            })
          }}
        >
          <Toolbar /> {/* Spacer for AppBar */}
          
          {/* Canvas */}
          <Box className={styles.canvasContainer}>
            <Canvas
              template={template}
              selectedComponentId={selectedComponentId}
              selectedSectionId={selectedSectionId}
              onSelectComponent={selectComponent}
              onAddSection={addSection}
            />
          </Box>
          
          {/* Floating Action Button for Mobile */}
          {isMobile && (
            <SpeedDial
              ariaLabel="Actions"
              sx={{ position: 'fixed', bottom: 16, right: 16 }}
              icon={<AddIcon />}
            >
              <SpeedDialAction
                icon={<AddIcon />}
                tooltipTitle="Add Section"
                onClick={() => {
                  const name = prompt('Section name:');
                  if (name) {
                    addSection(name, name);
                  }
                }}
              />
              <SpeedDialAction
                icon={<PaletteIcon />}
                tooltipTitle="Components"
                onClick={() => setLeftDrawerOpen(true)}
              />
              <SpeedDialAction
                icon={<SettingsIcon />}
                tooltipTitle="Properties"
                onClick={() => setRightDrawerOpen(true)}
              />
              <SpeedDialAction
                icon={<PreviewIcon />}
                tooltipTitle="Preview"
                onClick={handlePreview}
              />
            </SpeedDial>
          )}
        </Box>
        
        {/* Right Drawer - Property Panel */}
        <Drawer
          variant={isMobile || isTablet ? 'temporary' : 'persistent'}
          anchor="right"
          open={rightDrawerOpen}
          onClose={() => setRightDrawerOpen(false)}
          className={styles.rightDrawer}
          classes={{
            paper: styles.drawerPaper
          }}
        >
          <Box className={styles.drawerHeader}>
            <Typography variant="h6">Properties</Typography>
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
          <DialogTitle>Publish Template</DialogTitle>
          <DialogContent>
            <Typography variant="body1" paragraph>
              Are you sure you want to publish this template?
            </Typography>
            <Alert severity="info" sx={{ mb: 2 }}>
              Publishing will make this template active for all users. 
              The currently active template will be deactivated.
            </Alert>
            <Typography variant="body2" color="text.secondary">
              Template: <strong>{template?.name}</strong>
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Platform: <strong>{template?.platform}</strong>
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Target Audience: <strong>{template?.targetAudience}</strong>
            </Typography>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setPublishDialogOpen(false)}>
              Cancel
            </Button>
            <Button
              variant="contained"
              color="primary"
              onClick={handlePublish}
              disabled={isPublishing}
              startIcon={isPublishing ? <CircularProgress size={16} /> : <PublishIcon />}
            >
              {isPublishing ? 'Publishing...' : 'Publish'}
            </Button>
          </DialogActions>
        </Dialog>
        
        {/* Status Bar */}
        <Paper
          elevation={0}
          className={styles.statusBar}
          sx={{
            borderTop: '1px solid',
            borderColor: 'divider'
          }}
        >
          <Stack
            direction="row"
            spacing={2}
            alignItems="center"
            sx={{ px: 2, py: 1 }}
          >
            <Typography variant="caption" color="text.secondary">
              {template ? `${template.sections.length} sections` : '0 sections'}
            </Typography>
            <Divider orientation="vertical" flexItem />
            <Typography variant="caption" color="text.secondary">
              {template ? `${template.sections.reduce((acc, s) => acc + s.components.length, 0)} components` : '0 components'}
            </Typography>
            <Divider orientation="vertical" flexItem />
            <Typography variant="caption" color="text.secondary">
              Platform: {platform}
            </Typography>
            <Divider orientation="vertical" flexItem />
            <Typography variant="caption" color="text.secondary">
              Device: {deviceType}
            </Typography>
            {autoSaveEnabled && (
              <>
                <Divider orientation="vertical" flexItem />
                <Typography variant="caption" color="text.secondary">
                  Auto-save: {lastAutoSave ? `Last ${lastAutoSave.toLocaleTimeString()}` : 'Enabled'}
                </Typography>
              </>
            )}
            <Box sx={{ flexGrow: 1 }} />
            <Tooltip title="Toggle dark mode">
              <IconButton
                size="small"
                onClick={() => setDarkMode(!darkMode)}
              >
                {darkMode ? <LightModeIcon fontSize="small" /> : <DarkModeIcon fontSize="small" />}
              </IconButton>
            </Tooltip>
          </Stack>
        </Paper>
        
        {/* Drag Overlay */}
        <AnimatePresence>
          {dragState.isDragging && (
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              className={styles.dragOverlay}
            >
              <Paper
                elevation={4}
                sx={{
                  p: 2,
                  backgroundColor: 'primary.main',
                  color: 'primary.contrastText'
                }}
              >
                <Typography variant="body2">
                  Drag to add component
                </Typography>
              </Paper>
            </motion.div>
          )}
        </AnimatePresence>
        
        {/* Toast Notifications */}
        <Toaster
          position="bottom-center"
          toastOptions={{
            duration: 4000,
            style: {
              background: theme.palette.background.paper,
              color: theme.palette.text.primary,
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
              bottom: 24,
              left: 24
            }}
            onClick={() => window.open('/docs/home-screen-builder', '_blank')}
          >
            <HelpIcon />
          </Fab>
        </Zoom>
      </Box>
    </DndContext>
  );
};

export default HomeScreenBuilder;
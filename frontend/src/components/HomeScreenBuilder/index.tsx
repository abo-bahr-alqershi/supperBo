import React, { useState, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import type { 
  DndContext, 
  DragEndEvent, 
  DragOverlay,
  DragStartEvent,
  pointerWithin,
  rectIntersection
} from '@dnd-kit/core';
import { 
  Box, 
  Grid, 
  Paper, 
  Toolbar, 
  Button, 
  IconButton,
  Typography,
  Divider,
  CircularProgress,
  Alert
} from '@mui/material';
import {
  Save as SaveIcon,
  Preview as PreviewIcon,
  Undo as UndoIcon,
  Redo as RedoIcon,
  Publish as PublishIcon,
  Settings as SettingsIcon
} from '@mui/icons-material';
import { useHomeScreenBuilder } from '../../hooks/useHomeScreenBuilder';
import Canvas from './Canvas';
import ComponentPalette from './ComponentPalette';
import PropertyPanel from './PropertyPanel';
import PreviewModal from './PreviewModal';
import TemplateManager from './TemplateManager';
import ComponentWrapper from './ComponentWrapper';
import type { DragItem } from '../../types/dragDrop.types';

const HomeScreenBuilder: React.FC = () => {
  const { templateId } = useParams<{ templateId?: string }>();
  const [showPreview, setShowPreview] = useState(false);
  const [showTemplateManager, setShowTemplateManager] = useState(false);
  const [activeDragItem, setActiveDragItem] = useState<DragItem | null>(null);
  
  const {
    template,
    componentTypes,
    selectedComponentId,
    selectedSectionId,
    isDirty,
    lastSaveTime,
    isLoading,
    error,
    addSection,
    addComponent,
    updateComponent,
    deleteComponent,
    moveComponent,
    selectComponent,
    saveTemplate,
    publishTemplate,
    isSaving,
    isPublishing
  } = useHomeScreenBuilder({ templateId, autoSave: true });

  const handleDragStart = useCallback((event: DragStartEvent) => {
    const { active } = event;
    setActiveDragItem(active.data.current as DragItem);
  }, []);

  const handleDragEnd = useCallback((event: DragEndEvent) => {
    const { active, over } = event;
    setActiveDragItem(null);
    
    if (!over) return;
    
    const dragItem = active.data.current as DragItem;
    const dropTarget = over.data.current;
    
    if (dragItem.type === 'new-component' && dropTarget?.type === 'section') {
      // Add new component to section
      addComponent(dropTarget.sectionId, dragItem.componentType!, {
        order: dropTarget.index ?? 0
      });
    } else if (dragItem.type === 'existing-component' && dropTarget?.type === 'section') {
      // Move existing component
      moveComponent(
        dragItem.id,
        dragItem.sourceSectionId!,
        dropTarget.sectionId,
        dropTarget.index ?? 0
      );
    }
  }, [addComponent, moveComponent]);

  const handlePublish = useCallback(async () => {
    if (!templateId) return;
    
    const confirmed = window.confirm(
      'Are you sure you want to publish this template? It will become active for all users.'
    );
    
    if (confirmed) {
      await publishTemplate({ id: templateId, deactivateOthers: true });
    }
  }, [templateId, publishTemplate]);

  if (!templateId && !showTemplateManager) {
    return <TemplateManager onClose={() => setShowTemplateManager(false)} />;
  }

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box p={3}>
        <Alert severity="error">
          Failed to load template: {error.message}
        </Alert>
      </Box>
    );
  }

  return (
    <DndContext
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
      collisionDetection={rectIntersection}
    >
      <Box sx={{ height: '100vh', display: 'flex', flexDirection: 'column' }}>
        {/* Toolbar */}
        <Paper elevation={2} sx={{ zIndex: 10 }}>
          <Toolbar>
            <Typography variant="h6" sx={{ flexGrow: 0, mr: 3 }}>
              {template?.name || 'Home Screen Builder'}
            </Typography>
            
            <Box sx={{ flexGrow: 1 }} />
            
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <IconButton disabled>
                <UndoIcon />
              </IconButton>
              <IconButton disabled>
                <RedoIcon />
              </IconButton>
              
              <Divider orientation="vertical" flexItem sx={{ mx: 1 }} />
              
              <Button
                startIcon={<PreviewIcon />}
                onClick={() => setShowPreview(true)}
              >
                Preview
              </Button>
              
              <Button
                startIcon={<SaveIcon />}
                onClick={saveTemplate}
                disabled={!isDirty || isSaving}
                variant={isDirty ? 'contained' : 'outlined'}
              >
                {isSaving ? 'Saving...' : 'Save'}
              </Button>
              
              <Button
                startIcon={<PublishIcon />}
                onClick={handlePublish}
                disabled={isPublishing || isDirty}
                color="success"
                variant="contained"
              >
                Publish
              </Button>
              
              <IconButton onClick={() => setShowTemplateManager(true)}>
                <SettingsIcon />
              </IconButton>
            </Box>
          </Toolbar>
          
          {lastSaveTime && (
            <Box sx={{ px: 2, pb: 1 }}>
              <Typography variant="caption" color="text.secondary">
                Last saved: {lastSaveTime.toLocaleTimeString()}
              </Typography>
            </Box>
          )}
        </Paper>

        {/* Main Content */}
        <Grid container sx={{ flexGrow: 1, overflow: 'hidden' }}>
          {/* Component Palette */}
          <Grid item xs={12} md={2.5} sx={{ height: '100%', overflow: 'auto' }}>
            <ComponentPalette componentTypes={componentTypes} />
          </Grid>

          {/* Canvas */}
          <Grid item xs={12} md={6.5} sx={{ 
            height: '100%', 
            overflow: 'auto',
            backgroundColor: '#f5f5f5'
          }}>
            <Canvas
              template={template}
              selectedComponentId={selectedComponentId}
              selectedSectionId={selectedSectionId}
              onSelectComponent={selectComponent}
              onAddSection={addSection}
            />
          </Grid>

          {/* Property Panel */}
          <Grid item xs={12} md={3} sx={{ height: '100%', overflow: 'auto' }}>
            <PropertyPanel
              componentId={selectedComponentId}
              templateId={templateId}
            />
          </Grid>
        </Grid>
      </Box>

      {/* Drag Overlay */}
      <DragOverlay>
        {activeDragItem && (
          <Box
            sx={{
              padding: 2,
              backgroundColor: 'primary.main',
              color: 'primary.contrastText',
              borderRadius: 1,
              boxShadow: 3,
              opacity: 0.8
            }}
          >
            {activeDragItem.type === 'new-component' 
              ? `Add ${activeDragItem.componentType}`
              : 'Move Component'
            }
          </Box>
        )}
      </DragOverlay>

      {/* Modals */}
      {showPreview && template && (
        <PreviewModal
          template={template}
          open={showPreview}
          onClose={() => setShowPreview(false)}
        />
      )}

      {showTemplateManager && (
        <TemplateManager
          open={showTemplateManager}
          onClose={() => setShowTemplateManager(false)}
          currentTemplateId={templateId}
        />
      )}
    </DndContext>
  );
};

export default HomeScreenBuilder;
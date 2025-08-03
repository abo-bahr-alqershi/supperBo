import React, { useCallback } from 'react';
import { useDroppable } from '@dnd-kit/core';
import {
  Box,
  Paper,
  Typography,
  Button,
  IconButton,
  Fab,
  Zoom
} from '@mui/material';
import {
  Add as AddIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon
} from '@mui/icons-material';
import SectionContainer from './SectionContainer.tsx';
import type { HomeScreenTemplate } from '../../types/homeScreen.types';

interface CanvasProps {
  template?: HomeScreenTemplate;
  selectedComponentId: string | null;
  selectedSectionId: string | null;
  onSelectComponent: (componentId: string | null) => void;
  onAddSection: (name: string, title: string) => void;
}

const Canvas: React.FC<CanvasProps> = ({
  template,
  selectedComponentId,
  selectedSectionId,
  onSelectComponent,
  onAddSection
}) => {
  const { setNodeRef, isOver } = useDroppable({
    id: 'canvas',
    data: { type: 'canvas' }
  });

  const handleAddSection = useCallback(() => {
    const sectionNumber = (template?.sections.length || 0) + 1;
    onAddSection(
      `section_${sectionNumber}`,
      `Section ${sectionNumber}`
    );
  }, [template?.sections.length, onAddSection]);

  const handleCanvasClick = useCallback((e: React.MouseEvent) => {
    if (e.target === e.currentTarget) {
      onSelectComponent(null);
    }
  }, [onSelectComponent]);

  if (!template) {
    return (
      <Box
        ref={setNodeRef}
        sx={{
          height: '100%',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          p: 3
        }}
      >
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Typography variant="h6" gutterBottom>
            No template loaded
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Create or select a template to start building
          </Typography>
        </Paper>
      </Box>
    );
  }

  return (
    <Box
      ref={setNodeRef}
      onClick={handleCanvasClick}
      sx={{
        minHeight: '100%',
        p: 3,
        position: 'relative',
        backgroundColor: isOver ? 'action.hover' : 'transparent',
        transition: 'background-color 0.2s'
      }}
    >
      {/* Device Frame */}
      <Box
        sx={{
          maxWidth: 400,
          mx: 'auto',
          backgroundColor: '#fff',
          borderRadius: 4,
          boxShadow: '0 10px 40px rgba(0,0,0,0.1)',
          overflow: 'hidden',
          position: 'relative'
        }}
      >
        {/* Status Bar */}
        <Box
          sx={{
            height: 44,
            backgroundColor: '#000',
            color: '#fff',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            px: 2,
            fontSize: 12
          }}
        >
          <Box>9:41</Box>
          <Box sx={{ display: 'flex', gap: 0.5 }}>
            <Box>📶</Box>
            <Box>📶</Box>
            <Box>🔋</Box>
          </Box>
        </Box>

        {/* Content */}
        <Box sx={{ minHeight: 600 }}>
          {template.sections.length === 0 ? (
            <Box
              sx={{
                height: 400,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                p: 3
              }}
            >
              <Paper sx={{ p: 4, textAlign: 'center' }}>
                <Typography variant="h6" gutterBottom>
                  No sections yet
                </Typography>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  Add your first section to start building
                </Typography>
                <Button
                  variant="contained"
                  startIcon={<AddIcon />}
                  onClick={handleAddSection}
                  sx={{ mt: 2 }}
                >
                  Add Section
                </Button>
              </Paper>
            </Box>
          ) : (
            <Box>
              {template.sections
                .filter(section => section.isVisible)
                .sort((a, b) => a.order - b.order)
                .map((section) => (
                  <SectionContainer
                    key={section.id}
                    section={section}
                    isSelected={selectedSectionId === section.id}
                    selectedComponentId={selectedComponentId}
                    onSelectComponent={onSelectComponent}
                  />
                ))}
            </Box>
          )}
        </Box>
      </Box>

      {/* Floating Action Button */}
      <Zoom in={template.sections.length > 0}>
        <Fab
          color="primary"
          onClick={handleAddSection}
          sx={{
            position: 'fixed',
            bottom: 24,
            right: 24
          }}
        >
          <AddIcon />
        </Fab>
      </Zoom>
    </Box>
  );
};

export default Canvas;
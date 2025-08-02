import React, { useState } from 'react';
import { useDroppable } from '@dnd-kit/core';
import {
  Box,
  Paper,
  Typography,
  IconButton,
  Menu,
  MenuItem,
  Divider,
  Fade
} from '@mui/material';
import {
  MoreVert as MoreIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
  DragIndicator as DragIcon
} from '@mui/icons-material';
import ComponentWrapper from './ComponentWrapper';
import type { HomeScreenSection } from '../../types/homeScreen.types';

interface SectionContainerProps {
  section: HomeScreenSection;
  isSelected: boolean;
  selectedComponentId: string | null;
  onSelectComponent: (componentId: string | null) => void;
}

const SectionContainer: React.FC<SectionContainerProps> = ({
  section,
  isSelected,
  selectedComponentId,
  onSelectComponent
}) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [isHovered, setIsHovered] = useState(false);

  const { setNodeRef, isOver } = useDroppable({
    id: section.id,
    data: { 
      type: 'section',
      sectionId: section.id 
    }
  });

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    event.stopPropagation();
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const sectionStyles: React.CSSProperties = {
    backgroundColor: section.backgroundColor || 'transparent',
    backgroundImage: section.backgroundImage ? `url(${section.backgroundImage})` : undefined,
    backgroundSize: 'cover',
    backgroundPosition: 'center',
    padding: section.padding || '20px',
    margin: section.margin || '0',
    minHeight: section.minHeight || 'auto',
    maxHeight: section.maxHeight || 'none',
    position: 'relative',
    transition: 'all 0.3s ease',
    outline: isSelected ? '2px solid #1976d2' : undefined,
    outlineOffset: isSelected ? '2px' : undefined,
    cursor: 'pointer'
  };

  if (section.customStyles) {
    try {
      const customStyles = JSON.parse(section.customStyles);
      Object.assign(sectionStyles, customStyles);
    } catch (e) {
      console.error('Invalid custom styles:', e);
    }
  }

  return (
    <Box
      ref={setNodeRef}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
      sx={{
        position: 'relative',
        '&:hover': {
          '& .section-controls': {
            opacity: 1
          }
        }
      }}
    >
      <Box
        style={sectionStyles}
        onClick={(e) => {
          if (e.target === e.currentTarget) {
            onSelectComponent(null);
          }
        }}
      >
        {/* Section Header */}
        <Fade in={isHovered || isSelected}>
          <Box
            className="section-controls"
            sx={{
              position: 'absolute',
              top: 8,
              right: 8,
              display: 'flex',
              gap: 1,
              opacity: 0,
              transition: 'opacity 0.2s',
              zIndex: 10
            }}
          >
            <Paper
              elevation={2}
              sx={{
                display: 'flex',
                alignItems: 'center',
                backgroundColor: 'rgba(255,255,255,0.9)',
                backdropFilter: 'blur(4px)'
              }}
            >
              <IconButton size="small" sx={{ cursor: 'move' }}>
                <DragIcon />
              </IconButton>
              
              <Typography variant="caption" sx={{ px: 1 }}>
                {section.name}
              </Typography>

              <IconButton size="small">
                {section.isVisible ? <VisibilityIcon /> : <VisibilityOffIcon />}
              </IconButton>

              <IconButton size="small" onClick={handleMenuOpen}>
                <MoreIcon />
              </IconButton>
            </Paper>
          </Box>
        </Fade>

        {/* Section Title */}
        {section.title && (
          <Box sx={{ mb: 2 }}>
            <Typography variant="h5" gutterBottom>
              {section.title}
            </Typography>
            {section.subtitle && (
              <Typography variant="body2" color="text.secondary">
                {section.subtitle}
              </Typography>
            )}
          </Box>
        )}

        {/* Drop Zone for empty section */}
        {section.components.length === 0 && (
          <Box
            sx={{
              border: '2px dashed',
              borderColor: isOver ? 'primary.main' : 'divider',
              borderRadius: 1,
              p: 4,
              textAlign: 'center',
              backgroundColor: isOver ? 'action.hover' : 'transparent',
              transition: 'all 0.2s'
            }}
          >
            <Typography variant="body2" color="text.secondary">
              Drop components here
            </Typography>
          </Box>
        )}

        {/* Components Grid */}
        {section.components.length > 0 && (
          <Box
            sx={{
              display: 'grid',
              gridTemplateColumns: 'repeat(12, 1fr)',
              gap: 2,
              minHeight: 100,
              position: 'relative'
            }}
          >
            {section.components
              .filter(component => component.isVisible)
              .sort((a, b) => a.order - b.order)
              .map((component, index) => (
                <Box
                  key={component.id}
                  sx={{
                    gridColumn: `span ${component.colSpan}`,
                    gridRow: `span ${component.rowSpan}`
                  }}
                >
                  <ComponentWrapper
                    component={component}
                    sectionId={section.id}
                    isSelected={selectedComponentId === component.id}
                    onSelect={() => onSelectComponent(component.id)}
                    index={index}
                  />
                </Box>
              ))}

            {/* Drop indicator */}
            {isOver && (
              <Box
                sx={{
                  position: 'absolute',
                  inset: 0,
                  backgroundColor: 'primary.main',
                  opacity: 0.1,
                  pointerEvents: 'none',
                  borderRadius: 1
                }}
              />
            )}
          </Box>
        )}
      </Box>

      {/* Section Menu */}
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'right',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
      >
        <MenuItem onClick={handleMenuClose}>
          <EditIcon sx={{ mr: 1 }} fontSize="small" />
          Edit Section
        </MenuItem>
        <MenuItem onClick={handleMenuClose}>
          <VisibilityIcon sx={{ mr: 1 }} fontSize="small" />
          {section.isVisible ? 'Hide' : 'Show'} Section
        </MenuItem>
        <Divider />
        <MenuItem onClick={handleMenuClose} sx={{ color: 'error.main' }}>
          <DeleteIcon sx={{ mr: 1 }} fontSize="small" />
          Delete Section
        </MenuItem>
      </Menu>
    </Box>
  );
};

export default SectionContainer;
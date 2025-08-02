import React, { useState, lazy, Suspense } from 'react';
import { useDraggable } from '@dnd-kit/core';
import {
  Box,
  Paper,
  Typography,
  IconButton,
  Menu,
  MenuItem,
  Skeleton,
  Tooltip,
  Fade,
  Chip
} from '@mui/material';
import {
  DragIndicator as DragIcon,
  MoreVert as MoreIcon,
  ContentCopy as CopyIcon,
  Delete as DeleteIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
  Warning as WarningIcon
} from '@mui/icons-material';
import type { HomeScreenComponent } from '../../types/homeScreen.types';

// Lazy load dynamic components
const DynamicComponents = {
  Banner: lazy(() => import('../DynamicComponents/Banner.tsx')),
  Carousel: lazy(() => import('../DynamicComponents/Carousel.tsx')),
  CategoryGrid: lazy(() => import('../DynamicComponents/CategoryGrid.tsx')),
  PropertyList: lazy(() => import('../DynamicComponents/PropertyList.tsx')),
  SearchBar: lazy(() => import('../DynamicComponents/SearchBar.tsx')),
  OfferCard: lazy(() => import('../DynamicComponents/OfferCard.tsx')),
  TextBlock: lazy(() => import('../DynamicComponents/TextBlock.tsx')),
  ImageGallery: lazy(() => import('../DynamicComponents/ImageGallery.tsx')),
  MapView: lazy(() => import('../DynamicComponents/MapView.tsx')),
  FilterBar: lazy(() => import('../DynamicComponents/FilterBar.tsx')),
};

interface ComponentWrapperProps {
  component: HomeScreenComponent;
  sectionId: string;
  isSelected: boolean;
  onSelect: () => void;
  index: number;
}

const ComponentWrapper: React.FC<ComponentWrapperProps> = ({
  component,
  sectionId,
  isSelected,
  onSelect,
  index
}) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [isHovered, setIsHovered] = useState(false);
  const [hasError, setHasError] = useState(false);

  const { attributes, listeners, setNodeRef, transform, isDragging } = useDraggable({
    id: component.id,
    data: {
      type: 'existing-component',
      id: component.id,
      componentType: component.componentType,
      sourceSectionId: sectionId,
      sourceIndex: index,
      data: component
    }
  });

  const style: React.CSSProperties = {
    transform: transform ? `translate3d(${transform.x}px, ${transform.y}px, 0)` : undefined,
    opacity: isDragging ? 0.5 : 1,
    cursor: isDragging ? 'grabbing' : 'grab',
    position: 'relative',
    height: '100%'
  };

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    event.stopPropagation();
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const DynamicComponent = DynamicComponents[component.componentType as keyof typeof DynamicComponents];

  if (!DynamicComponent) {
    return (
      <Paper
        sx={{
          p: 2,
          height: '100%',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          backgroundColor: 'error.lighter',
          border: '1px solid',
          borderColor: 'error.main'
        }}
      >
        <Box textAlign="center">
          <WarningIcon color="error" />
          <Typography variant="body2" color="error">
            Unknown component type: {component.componentType}
          </Typography>
        </Box>
      </Paper>
    );
  }

  // Prepare component props
  const componentProps = component.properties.reduce((acc, prop) => {
    acc[prop.propertyKey] = prop.value ?? prop.defaultValue;
    return acc;
  }, {} as Record<string, any>);

  // Apply component styles
  const componentStyles = component.styles.reduce((acc, style) => {
    const value = style.unit ? `${style.styleValue}${style.unit}` : style.styleValue;
    acc[style.styleKey] = value;
    return acc;
  }, {} as Record<string, string>);

  return (
    <Box
      ref={setNodeRef}
      style={style}
      {...attributes}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
      onClick={(e) => {
        e.stopPropagation();
        onSelect();
      }}
      sx={{
        outline: isSelected ? '2px solid #1976d2' : undefined,
        outlineOffset: isSelected ? '2px' : undefined,
        borderRadius: 1,
        transition: 'outline 0.2s',
        '&:hover': {
          '& .component-controls': {
            opacity: 1
          }
        }
      }}
    >
      {/* Component Controls */}
      <Fade in={isHovered || isSelected}>
        <Box
          className="component-controls"
          sx={{
            position: 'absolute',
            top: -36,
            left: 0,
            right: 0,
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            opacity: 0,
            transition: 'opacity 0.2s',
            zIndex: 10,
            pointerEvents: isHovered || isSelected ? 'auto' : 'none'
          }}
        >
          <Paper
            elevation={2}
            sx={{
              display: 'flex',
              alignItems: 'center',
              px: 1,
              backgroundColor: 'rgba(255,255,255,0.95)',
              backdropFilter: 'blur(4px)'
            }}
          >
            <IconButton size="small" {...listeners} sx={{ cursor: 'move' }}>
              <DragIcon />
            </IconButton>
            
            <Chip
              label={component.componentType}
              size="small"
              color="primary"
              sx={{ mx: 1 }}
            />
            
            <Typography variant="caption" sx={{ mr: 1 }}>
              {component.name}
            </Typography>
          </Paper>

          <Paper
            elevation={2}
            sx={{
              display: 'flex',
              backgroundColor: 'rgba(255,255,255,0.95)',
              backdropFilter: 'blur(4px)'
            }}
          >
            <Tooltip title={component.isVisible ? 'Hide' : 'Show'}>
              <IconButton size="small">
                {component.isVisible ? <VisibilityIcon /> : <VisibilityOffIcon />}
              </IconButton>
            </Tooltip>
            
            <Tooltip title="More options">
              <IconButton size="small" onClick={handleMenuOpen}>
                <MoreIcon />
              </IconButton>
            </Tooltip>
          </Paper>
        </Box>
      </Fade>

      {/* Component Content */}
      <Box
        sx={{
          height: '100%',
          opacity: component.isVisible ? 1 : 0.5,
          transition: 'opacity 0.2s',
          ...componentStyles
        }}
      >
        <Suspense
          fallback={
            <Skeleton
              variant="rectangular"
              width="100%"
              height="100%"
              animation="wave"
            />
          }
        >
          <ErrorBoundary onError={() => setHasError(true)}>
            {hasError ? (
              <Paper
                sx={{
                  p: 2,
                  height: '100%',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  backgroundColor: 'error.lighter'
                }}
              >
                <Box textAlign="center">
                  <WarningIcon color="error" />
                  <Typography variant="body2" color="error">
                    Component error
                  </Typography>
                </Box>
              </Paper>
            ) : (
              <DynamicComponent
                {...componentProps}
                data={component.dataSource?.mockData ? 
                  JSON.parse(component.dataSource.mockData) : null
                }
                isPreview={true}
              />
            )}
          </ErrorBoundary>
        </Suspense>
      </Box>

      {/* Component Menu */}
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
          <CopyIcon sx={{ mr: 1 }} fontSize="small" />
          Duplicate
        </MenuItem>
        <MenuItem onClick={handleMenuClose}>
          <VisibilityIcon sx={{ mr: 1 }} fontSize="small" />
          {component.isVisible ? 'Hide' : 'Show'}
        </MenuItem>
        <MenuItem onClick={handleMenuClose} sx={{ color: 'error.main' }}>
          <DeleteIcon sx={{ mr: 1 }} fontSize="small" />
          Delete
        </MenuItem>
      </Menu>
    </Box>
  );
};

// Error Boundary Component
class ErrorBoundary extends React.Component<
  { children: React.ReactNode; onError: () => void },
  { hasError: boolean }
> {
  constructor(props: any) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error: any) {
    return { hasError: true };
  }

  componentDidCatch(error: any, errorInfo: any) {
    console.error('Component error:', error, errorInfo);
    this.props.onError();
  }

  render() {
    if (this.state.hasError) {
      return null;
    }

    return this.props.children;
  }
}

export default ComponentWrapper;
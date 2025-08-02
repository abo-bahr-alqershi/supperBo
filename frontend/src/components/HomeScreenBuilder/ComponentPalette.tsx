import React, { useState } from 'react';
import { useDraggable } from '@dnd-kit/core';
import {
  Box,
  Paper,
  Typography,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Collapse,
  TextField,
  InputAdornment,
  Divider,
  Chip,
  IconButton
} from '@mui/material';
import {
  Search as SearchIcon,
  ExpandLess,
  ExpandMore,
  ViewCarousel as CarouselIcon,
  Image as ImageIcon,
  GridView as GridIcon,
  List as ListIcon,
  Search as SearchBarIcon,
  LocalOffer as OfferIcon,
  TextFields as TextIcon,
  Collections as GalleryIcon,
  Map as MapIcon,
  FilterList as FilterIcon
} from '@mui/icons-material';
import type { ComponentTypeDefinition, ComponentCategory } from '../../types/component.types';

interface ComponentPaletteProps {
  componentTypes: ComponentTypeDefinition[];
}

interface DraggableComponentProps {
  componentType: ComponentTypeDefinition;
}

const COMPONENT_ICONS: Record<string, React.ReactElement> = {
  Banner: <ImageIcon />,
  Carousel: <CarouselIcon />,
  CategoryGrid: <GridIcon />,
  PropertyList: <ListIcon />,
  SearchBar: <SearchBarIcon />,
  OfferCard: <OfferIcon />,
  TextBlock: <TextIcon />,
  ImageGallery: <GalleryIcon />,
  MapView: <MapIcon />,
  FilterBar: <FilterIcon />
};

const DraggableComponent: React.FC<DraggableComponentProps> = ({ componentType }) => {
  const { attributes, listeners, setNodeRef, transform, isDragging } = useDraggable({
    id: `palette-${componentType.type}`,
    data: {
      type: 'new-component',
      componentType: componentType.type,
      data: componentType
    }
  });

  const style = transform ? {
    transform: `translate3d(${transform.x}px, ${transform.y}px, 0)`,
    opacity: isDragging ? 0.5 : 1
  } : undefined;

  return (
    <ListItem
      ref={setNodeRef}
      style={style}
      {...attributes}
      {...listeners}
      sx={{
        cursor: 'grab',
        '&:hover': {
          backgroundColor: 'action.hover'
        },
        '&:active': {
          cursor: 'grabbing'
        },
        borderRadius: 1,
        mb: 1
      }}
    >
      <ListItemIcon>
        {COMPONENT_ICONS[componentType.type] || <ImageIcon />}
      </ListItemIcon>
      <ListItemText
        primary={componentType.name}
        secondary={componentType.description}
      />
    </ListItem>
  );
};

const ComponentPalette: React.FC<ComponentPaletteProps> = ({ componentTypes }) => {
  const [searchQuery, setSearchQuery] = useState('');
  const [expandedCategories, setExpandedCategories] = useState<Record<string, boolean>>({
    Display: true,
    Navigation: true,
    Input: true,
    Data: true,
    Layout: true
  });

  const handleCategoryToggle = (category: ComponentCategory) => {
    setExpandedCategories(prev => ({
      ...prev,
      [category]: !prev[category]
    }));
  };

  // Group components by category
  const componentsByCategory = componentTypes.reduce((acc, component) => {
    const category = component.category || 'Other';
    if (!acc[category]) {
      acc[category] = [];
    }
    acc[category].push(component);
    return acc;
  }, {} as Record<string, ComponentTypeDefinition[]>);

  // Filter components by search query
  const filteredComponentsByCategory = Object.entries(componentsByCategory).reduce(
    (acc, [category, components]) => {
      const filtered = components.filter(
        component =>
          component.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
          component.description.toLowerCase().includes(searchQuery.toLowerCase())
      );
      if (filtered.length > 0) {
        acc[category] = filtered;
      }
      return acc;
    },
    {} as Record<string, ComponentTypeDefinition[]>
  );

  return (
    <Paper sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ p: 2 }}>
        <Typography variant="h6" gutterBottom>
          Components
        </Typography>
        <TextField
          fullWidth
          size="small"
          placeholder="Search components..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            )
          }}
        />
      </Box>

      <Divider />

      <Box sx={{ flexGrow: 1, overflow: 'auto', p: 2 }}>
        {Object.entries(filteredComponentsByCategory).map(([category, components]) => (
          <Box key={category} sx={{ mb: 2 }}>
            <Box
              sx={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                mb: 1,
                cursor: 'pointer'
              }}
              onClick={() => handleCategoryToggle(category as ComponentCategory)}
            >
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <Typography variant="subtitle2" color="text.secondary">
                  {category}
                </Typography>
                <Chip
                  label={components.length}
                  size="small"
                  color="primary"
                  sx={{ height: 20 }}
                />
              </Box>
              <IconButton size="small">
                {expandedCategories[category] ? <ExpandLess /> : <ExpandMore />}
              </IconButton>
            </Box>

            <Collapse in={expandedCategories[category]}>
              <List dense sx={{ pl: 1 }}>
                {components.map((component) => (
                  <DraggableComponent
                    key={component.type}
                    componentType={component}
                  />
                ))}
              </List>
            </Collapse>
          </Box>
        ))}

        {Object.keys(filteredComponentsByCategory).length === 0 && (
          <Box sx={{ textAlign: 'center', py: 4 }}>
            <Typography variant="body2" color="text.secondary">
              No components found
            </Typography>
          </Box>
        )}
      </Box>

      <Divider />

      <Box sx={{ p: 2 }}>
        <Typography variant="caption" color="text.secondary">
          Drag components to the canvas to add them to your home screen
        </Typography>
      </Box>
    </Paper>
  );
};

export default ComponentPalette;
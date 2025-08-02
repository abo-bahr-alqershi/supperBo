import React, { useState } from 'react';
import {
  Box,
  Chip,
  Button,
  IconButton,
  Menu,
  MenuItem,
  Slider,
  TextField,
  Typography,
  FormControl,
  FormControlLabel,
  Checkbox,
  Radio,
  RadioGroup,
  Divider,
  Badge,
  Collapse,
  Paper,
  useTheme,
  useMediaQuery,
  Drawer
} from '@mui/material';
import {
  FilterList as FilterIcon,
  Clear as ClearIcon,
  ExpandMore as ExpandIcon,
  ExpandLess as CollapseIcon,
  TuneRounded as TuneIcon,
  Close as CloseIcon
} from '@mui/icons-material';
import { motion, AnimatePresence } from 'framer-motion';

interface FilterOption {
  id: string;
  label: string;
  value: string | number | boolean;
  count?: number;
}

interface Filter {
  id: string;
  label: string;
  type: 'select' | 'multiselect' | 'range' | 'toggle' | 'radio';
  options?: FilterOption[];
  min?: number;
  max?: number;
  unit?: string;
  defaultValue?: any;
}

interface FilterBarProps {
  filters?: Filter[];
  layout?: 'horizontal' | 'vertical' | 'dropdown';
  showClearAll?: boolean;
  showResultCount?: boolean;
  resultCount?: number;
  chipColor?: 'default' | 'primary' | 'secondary';
  expandable?: boolean;
  defaultExpanded?: boolean;
  mobileDrawer?: boolean;
  data?: Filter[];
  isPreview?: boolean;
  onFilterChange?: (filterId: string, value: any) => void;
  onClearAll?: () => void;
  onApply?: (filters: Record<string, any>) => void;
}

const FilterBar: React.FC<FilterBarProps> = ({
  filters = [],
  layout = 'horizontal',
  showClearAll = true,
  showResultCount = true,
  resultCount = 0,
  chipColor = 'default',
  expandable = true,
  defaultExpanded = true,
  mobileDrawer = true,
  data,
  isPreview = false,
  onFilterChange,
  onClearAll,
  onApply
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [selectedFilters, setSelectedFilters] = useState<Record<string, any>>({});
  const [expandedFilters, setExpandedFilters] = useState<Record<string, boolean>>({});
  const [isExpanded, setIsExpanded] = useState(defaultExpanded);
  const [mobileDrawerOpen, setMobileDrawerOpen] = useState(false);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  const displayFilters = data || filters || generateMockFilters();

  const handleFilterChange = (filterId: string, value: any) => {
    if (isPreview) return;

    setSelectedFilters(prev => ({
      ...prev,
      [filterId]: value
    }));

    onFilterChange?.(filterId, value);
  };

  const handleClearAll = () => {
    if (isPreview) return;

    setSelectedFilters({});
    onClearAll?.();
  };

  const handleApply = () => {
    if (isPreview) return;

    onApply?.(selectedFilters);
    setMobileDrawerOpen(false);
  };

  const toggleFilterExpansion = (filterId: string) => {
    setExpandedFilters(prev => ({
      ...prev,
      [filterId]: !prev[filterId]
    }));
  };

  const getActiveFilterCount = () => {
    return Object.keys(selectedFilters).filter(key => {
      const value = selectedFilters[key];
      if (Array.isArray(value)) return value.length > 0;
      if (typeof value === 'object' && value !== null) return value.min !== undefined || value.max !== undefined;
      return value !== undefined && value !== null && value !== '';
    }).length;
  };

  const renderFilter = (filter: Filter) => {
    const isFilterExpanded = expandedFilters[filter.id] !== false;

    switch (filter.type) {
      case 'select':
        return (
          <FormControl fullWidth size="small">
            <Box
              sx={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                cursor: expandable ? 'pointer' : 'default',
                mb: 1
              }}
              onClick={() => expandable && toggleFilterExpansion(filter.id)}
            >
              <Typography variant="subtitle2">{filter.label}</Typography>
              {expandable && (
                <IconButton size="small">
                  {isFilterExpanded ? <CollapseIcon /> : <ExpandIcon />}
                </IconButton>
              )}
            </Box>
            <Collapse in={isFilterExpanded}>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
                {filter.options?.map(option => (
                  <MenuItem
                    key={option.id}
                    selected={selectedFilters[filter.id] === option.value}
                    onClick={() => handleFilterChange(filter.id, option.value)}
                    sx={{ borderRadius: 1, py: 1 }}
                  >
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', width: '100%' }}>
                      <Typography variant="body2">{option.label}</Typography>
                      {option.count !== undefined && (
                        <Typography variant="caption" color="text.secondary">
                          ({option.count})
                        </Typography>
                      )}
                    </Box>
                  </MenuItem>
                ))}
              </Box>
            </Collapse>
          </FormControl>
        );

      case 'multiselect':
        return (
          <FormControl fullWidth>
            <Box
              sx={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                cursor: expandable ? 'pointer' : 'default',
                mb: 1
              }}
              onClick={() => expandable && toggleFilterExpansion(filter.id)}
            >
              <Typography variant="subtitle2">{filter.label}</Typography>
              {expandable && (
                <IconButton size="small">
                  {isFilterExpanded ? <CollapseIcon /> : <ExpandIcon />}
                </IconButton>
              )}
            </Box>
            <Collapse in={isFilterExpanded}>
              <Box sx={{ display: 'flex', flexDirection: 'column' }}>
                {filter.options?.map(option => (
                  <FormControlLabel
                    key={option.id}
                    control={
                      <Checkbox
                        size="small"
                        checked={(selectedFilters[filter.id] || []).includes(option.value)}
                        onChange={(e) => {
                          const currentValues = selectedFilters[filter.id] || [];
                          const newValues = e.target.checked
                            ? [...currentValues, option.value]
                            : currentValues.filter((v: any) => v !== option.value);
                          handleFilterChange(filter.id, newValues);
                        }}
                      />
                    }
                    label={
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', width: '100%' }}>
                        <Typography variant="body2">{option.label}</Typography>
                        {option.count !== undefined && (
                          <Typography variant="caption" color="text.secondary" sx={{ ml: 2 }}>
                            ({option.count})
                          </Typography>
                        )}
                      </Box>
                    }
                  />
                ))}
              </Box>
            </Collapse>
          </FormControl>
        );

      case 'range':
        const rangeValue = selectedFilters[filter.id] || { min: filter.min, max: filter.max };
        return (
          <FormControl fullWidth>
            <Box
              sx={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                cursor: expandable ? 'pointer' : 'default',
                mb: 1
              }}
              onClick={() => expandable && toggleFilterExpansion(filter.id)}
            >
              <Typography variant="subtitle2">{filter.label}</Typography>
              {expandable && (
                <IconButton size="small">
                  {isFilterExpanded ? <CollapseIcon /> : <ExpandIcon />}
                </IconButton>
              )}
            </Box>
            <Collapse in={isFilterExpanded}>
              <Box sx={{ px: 1 }}>
                <Slider
                  value={[rangeValue.min || filter.min || 0, rangeValue.max || filter.max || 100]}
                  onChange={(_, newValue) => {
                    const [min, max] = newValue as number[];
                    handleFilterChange(filter.id, { min, max });
                  }}
                  valueLabelDisplay="auto"
                  min={filter.min || 0}
                  max={filter.max || 100}
                  marks
                />
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 1 }}>
                  <TextField
                    size="small"
                    label="Min"
                    type="number"
                    value={rangeValue.min || filter.min || 0}
                    onChange={(e) => handleFilterChange(filter.id, {
                      ...rangeValue,
                      min: Number(e.target.value)
                    })}
                    sx={{ width: '45%' }}
                  />
                  <TextField
                    size="small"
                    label="Max"
                    type="number"
                    value={rangeValue.max || filter.max || 100}
                    onChange={(e) => handleFilterChange(filter.id, {
                      ...rangeValue,
                      max: Number(e.target.value)
                    })}
                    sx={{ width: '45%' }}
                  />
                </Box>
                {filter.unit && (
                  <Typography variant="caption" color="text.secondary" sx={{ mt: 1 }}>
                    Unit: {filter.unit}
                  </Typography>
                )}
              </Box>
            </Collapse>
          </FormControl>
        );

      case 'radio':
        return (
          <FormControl fullWidth>
            <Box
              sx={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                cursor: expandable ? 'pointer' : 'default',
                mb: 1
              }}
              onClick={() => expandable && toggleFilterExpansion(filter.id)}
            >
              <Typography variant="subtitle2">{filter.label}</Typography>
              {expandable && (
                <IconButton size="small">
                  {isFilterExpanded ? <CollapseIcon /> : <ExpandIcon />}
                </IconButton>
              )}
            </Box>
            <Collapse in={isFilterExpanded}>
              <RadioGroup
                value={selectedFilters[filter.id] || ''}
                onChange={(e) => handleFilterChange(filter.id, e.target.value)}
              >
                {filter.options?.map(option => (
                  <FormControlLabel
                    key={option.id}
                    value={option.value}
                    control={<Radio size="small" />}
                    label={
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', width: '100%' }}>
                        <Typography variant="body2">{option.label}</Typography>
                        {option.count !== undefined && (
                          <Typography variant="caption" color="text.secondary" sx={{ ml: 2 }}>
                            ({option.count})
                          </Typography>
                        )}
                      </Box>
                    }
                  />
                ))}
              </RadioGroup>
            </Collapse>
          </FormControl>
        );

      case 'toggle':
        return (
          <FormControlLabel
            control={
              <Checkbox
                checked={selectedFilters[filter.id] || false}
                onChange={(e) => handleFilterChange(filter.id, e.target.checked)}
              />
            }
            label={filter.label}
          />
        );

      default:
        return null;
    }
  };

  const renderSelectedChips = () => {
    return Object.entries(selectedFilters).map(([filterId, value]) => {
      const filter = displayFilters.find(f => f.id === filterId);
      if (!filter || !value) return null;

      let label = '';
      if (Array.isArray(value)) {
        label = `${filter.label}: ${value.length} selected`;
      } else if (typeof value === 'object' && value.min !== undefined) {
        label = `${filter.label}: ${value.min}-${value.max}${filter.unit ? ` ${filter.unit}` : ''}`;
      } else if (typeof value === 'boolean') {
        label = filter.label;
      } else {
        const option = filter.options?.find(o => o.value === value);
        label = `${filter.label}: ${option?.label || value}`;
      }

      return (
        <motion.div
          key={filterId}
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          exit={{ scale: 0 }}
        >
          <Chip
            label={label}
            color={chipColor}
            onDelete={() => handleFilterChange(filterId, null)}
            size="small"
            sx={{ m: 0.5 }}
          />
        </motion.div>
      );
    });
  };

  const filterContent = (
    <Box sx={{ width: '100%' }}>
      {/* Selected Filters */}
      {Object.keys(selectedFilters).length > 0 && (
        <Box sx={{ mb: 2 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
            <Typography variant="subtitle2">Active Filters</Typography>
            {showClearAll && (
              <Button size="small" onClick={handleClearAll} startIcon={<ClearIcon />}>
                Clear All
              </Button>
            )}
          </Box>
          <Box sx={{ display: 'flex', flexWrap: 'wrap' }}>
            <AnimatePresence>
              {renderSelectedChips()}
            </AnimatePresence>
          </Box>
        </Box>
      )}

      {/* Filters */}
      <Box sx={{ display: 'flex', flexDirection: layout === 'vertical' ? 'column' : 'row', gap: 2 }}>
        {displayFilters.map((filter, index) => (
          <Box
            key={filter.id}
            sx={{
              flex: layout === 'horizontal' ? 1 : 'auto',
              minWidth: layout === 'horizontal' ? 200 : 'auto'
            }}
          >
            {renderFilter(filter)}
            {index < displayFilters.length - 1 && layout === 'vertical' && (
              <Divider sx={{ my: 2 }} />
            )}
          </Box>
        ))}
      </Box>

      {/* Result Count */}
      {showResultCount && (
        <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Typography variant="body2" color="text.secondary">
            {resultCount} results found
          </Typography>
          {isMobile && (
            <Button variant="contained" onClick={handleApply}>
              Apply Filters
            </Button>
          )}
        </Box>
      )}
    </Box>
  );

  if (layout === 'dropdown') {
    return (
      <Box>
        <Button
          variant="outlined"
          startIcon={<FilterIcon />}
          endIcon={<Badge badgeContent={getActiveFilterCount()} color="primary"><ExpandIcon /></Badge>}
          onClick={(e) => setAnchorEl(e.currentTarget)}
        >
          Filters
        </Button>
        <Menu
          anchorEl={anchorEl}
          open={Boolean(anchorEl)}
          onClose={() => setAnchorEl(null)}
          PaperProps={{
            sx: { width: 400, maxHeight: 600, p: 2 }
          }}
        >
          {filterContent}
        </Menu>
      </Box>
    );
  }

  if (isMobile && mobileDrawer) {
    return (
      <>
        <Button
          variant="outlined"
          fullWidth
          startIcon={<TuneIcon />}
          endIcon={<Badge badgeContent={getActiveFilterCount()} color="primary" />}
          onClick={() => setMobileDrawerOpen(true)}
        >
          Filters & Sort
        </Button>
        <Drawer
          anchor="bottom"
          open={mobileDrawerOpen}
          onClose={() => setMobileDrawerOpen(false)}
          PaperProps={{
            sx: {
              borderTopLeftRadius: 16,
              borderTopRightRadius: 16,
              maxHeight: '80vh'
            }
          }}
        >
          <Box sx={{ p: 2 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
              <Typography variant="h6">Filters</Typography>
              <IconButton onClick={() => setMobileDrawerOpen(false)}>
                <CloseIcon />
              </IconButton>
            </Box>
            {filterContent}
          </Box>
        </Drawer>
      </>
    );
  }

  return (
    <Paper
      elevation={0}
      sx={{
        p: 2,
        backgroundColor: 'background.default',
        borderRadius: 2
      }}
    >
      {expandable && (
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            mb: isExpanded ? 2 : 0,
            cursor: 'pointer'
          }}
          onClick={() => setIsExpanded(!isExpanded)}
        >
          <Typography variant="h6" sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <FilterIcon />
            Filters
            {getActiveFilterCount() > 0 && (
              <Chip
                label={getActiveFilterCount()}
                size="small"
                color="primary"
              />
            )}
          </Typography>
          <IconButton>
            {isExpanded ? <CollapseIcon /> : <ExpandIcon />}
          </IconButton>
        </Box>
      )}
      
      <Collapse in={!expandable || isExpanded}>
        {filterContent}
      </Collapse>
    </Paper>
  );
};

// Generate mock filters for preview
function generateMockFilters(): Filter[] {
  return [
    {
      id: 'category',
      label: 'Category',
      type: 'multiselect',
      options: [
        { id: '1', label: 'Electronics', value: 'electronics', count: 45 },
        { id: '2', label: 'Clothing', value: 'clothing', count: 32 },
        { id: '3', label: 'Books', value: 'books', count: 28 },
        { id: '4', label: 'Home & Garden', value: 'home', count: 15 }
      ]
    },
    {
      id: 'price',
      label: 'Price Range',
      type: 'range',
      min: 0,
      max: 1000,
      unit: '$'
    },
    {
      id: 'rating',
      label: 'Customer Rating',
      type: 'radio',
      options: [
        { id: '1', label: '4 Stars & Up', value: '4', count: 120 },
        { id: '2', label: '3 Stars & Up', value: '3', count: 200 },
        { id: '3', label: '2 Stars & Up', value: '2', count: 250 },
        { id: '4', label: 'All', value: 'all', count: 300 }
      ]
    },
    {
      id: 'availability',
      label: 'In Stock Only',
      type: 'toggle'
    }
  ];
}

export default FilterBar;
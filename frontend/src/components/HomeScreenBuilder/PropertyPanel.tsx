import React, { useState } from 'react';
import {
  Box,
  Paper,
  Typography,
  Tabs,
  Tab,
  TextField,
  Select,
  MenuItem,
  FormControl,
  FormLabel,
  Switch,
  Slider,
  IconButton,
  Button,
  Divider,
  List,
  ListItem,
  ListItemText,
  ListItemSecondaryAction,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Chip,
  InputAdornment,
  FormControlLabel
} from '@mui/material';
import {
  ExpandMore as ExpandMoreIcon,
  Delete as DeleteIcon,
  Add as AddIcon,
  ColorLens as ColorIcon,
  Link as LinkIcon,
  DataObject as DataIcon
} from '@mui/icons-material';
import { useComponentProperties } from '../../hooks/useComponentProperties';
import { useHomeScreenBuilder } from '../../hooks/useHomeScreenBuilder';
import type { 
  ComponentProperty, 
  PropertyType,
  ActionType,
  ActionTrigger,
  AnimationType,
  Alignment
} from '../../types/homeScreen.types';

interface PropertyPanelProps {
  componentId: string | null;
  templateId?: string;
}

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

const TabPanel: React.FC<TabPanelProps> = ({ children, value, index }) => {
  return (
    <div hidden={value !== index}>
      {value === index && <Box sx={{ py: 2 }}>{children}</Box>}
    </div>
  );
};

const PropertyPanel: React.FC<PropertyPanelProps> = ({ componentId, templateId }) => {
  const [activeTab, setActiveTab] = useState(0);
  const { template } = useHomeScreenBuilder({ templateId });
  const selectedComponent = componentId && template
    ? template.sections.flatMap(s => s.components).find(c => c.id === componentId) || null
    : null;

  const {
    component,
    errors,
    isDirty,
    isSaving,
    updateBasicProperty,
    updateProperty,
    getPropertyValue,
    updateStyle,
    removeStyle,
    getStyleValue,
    updateAction,
    addAction,
    removeAction,
    updateDataSource,
    removeDataSource
  } = useComponentProperties({
    component: selectedComponent,
    templateId,
    autoSave: true
  });

  if (!component) {
    return (
      <Paper sx={{ height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <Box sx={{ textAlign: 'center', p: 3 }}>
          <Typography variant="h6" gutterBottom>
            No component selected
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Select a component to view and edit its properties
          </Typography>
        </Box>
      </Paper>
    );
  }

  const renderPropertyField = (property: ComponentProperty) => {
    const value = getPropertyValue(property.propertyKey);

    switch (property.propertyType) {
      case 'text':
        return (
          <TextField
            fullWidth
            size="small"
            value={value || ''}
            onChange={(e) => updateProperty(property.propertyKey, e.target.value)}
            required={property.isRequired}
            helperText={property.helpText}
          />
        );

      case 'number':
        return (
          <TextField
            fullWidth
            size="small"
            type="number"
            value={value || ''}
            onChange={(e) => updateProperty(property.propertyKey, Number(e.target.value))}
            required={property.isRequired}
            helperText={property.helpText}
          />
        );

      case 'boolean':
        return (
          <FormControlLabel
            control={
              <Switch
                checked={value || false}
                onChange={(e) => updateProperty(property.propertyKey, e.target.checked)}
              />
            }
            label={property.helpText || ''}
          />
        );

      case 'select':
        const options = property.options ? JSON.parse(property.options) : [];
        return (
          <Select
            fullWidth
            size="small"
            value={value || ''}
            onChange={(e) => updateProperty(property.propertyKey, e.target.value)}
            required={property.isRequired}
          >
            {options.map((option: string) => (
              <MenuItem key={option} value={option}>
                {option}
              </MenuItem>
            ))}
          </Select>
        );

      case 'color':
        return (
          <Box sx={{ display: 'flex', gap: 1 }}>
            <TextField
              fullWidth
              size="small"
              value={value || ''}
              onChange={(e) => updateProperty(property.propertyKey, e.target.value)}
              required={property.isRequired}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <Box
                      sx={{
                        width: 20,
                        height: 20,
                        backgroundColor: value || '#ffffff',
                        border: '1px solid #ccc',
                        borderRadius: 1
                      }}
                    />
                  </InputAdornment>
                )
              }}
            />
            <input
              type="color"
              value={value || '#ffffff'}
              onChange={(e) => updateProperty(property.propertyKey, e.target.value)}
              style={{ width: 40, height: 32, cursor: 'pointer' }}
            />
          </Box>
        );

      default:
        return (
          <TextField
            fullWidth
            size="small"
            value={value || ''}
            onChange={(e) => updateProperty(property.propertyKey, e.target.value)}
            required={property.isRequired}
            helperText={property.helpText}
          />
        );
    }
  };

  return (
    <Paper sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ p: 2, borderBottom: 1, borderColor: 'divider' }}>
        <Typography variant="h6" gutterBottom>
          {component.name}
        </Typography>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <Chip label={component.componentType} size="small" color="primary" />
          {isDirty && <Chip label="Unsaved" size="small" color="warning" />}
          {isSaving && <Chip label="Saving..." size="small" />}
        </Box>
      </Box>

      <Tabs value={activeTab} onChange={(_, value) => setActiveTab(value)}>
        <Tab label="Properties" />
        <Tab label="Styles" />
        <Tab label="Actions" />
        <Tab label="Data" />
      </Tabs>

      <Box sx={{ flexGrow: 1, overflow: 'auto', p: 2 }}>
        {/* Properties Tab */}
        <TabPanel value={activeTab} index={0}>
          <Accordion defaultExpanded>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
              <Typography>Basic Properties</Typography>
            </AccordionSummary>
            <AccordionDetails>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                <TextField
                  fullWidth
                  size="small"
                  label="Component Name"
                  value={component.name}
                  onChange={(e) => updateBasicProperty('name', e.target.value)}
                />

                <FormControl fullWidth size="small">
                  <FormLabel>Alignment</FormLabel>
                  <Select
                    value={component.alignment}
                    onChange={(e) => updateBasicProperty('alignment', e.target.value)}
                  >
                    <MenuItem value="left">Left</MenuItem>
                    <MenuItem value="center">Center</MenuItem>
                    <MenuItem value="right">Right</MenuItem>
                  </Select>
                </FormControl>

                <Box>
                  <Typography variant="body2" gutterBottom>
                    Column Span: {component.colSpan}
                  </Typography>
                  <Slider
                    value={component.colSpan}
                    onChange={(_, value) => updateBasicProperty('colSpan', value)}
                    min={1}
                    max={12}
                    marks
                    valueLabelDisplay="auto"
                  />
                </Box>

                <FormControlLabel
                  control={
                    <Switch
                      checked={component.isVisible}
                      onChange={(e) => updateBasicProperty('isVisible', e.target.checked)}
                    />
                  }
                  label="Visible"
                />
              </Box>
            </AccordionDetails>
          </Accordion>

          <Accordion>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
              <Typography>Component Properties</Typography>
            </AccordionSummary>
            <AccordionDetails>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                {component.properties
                  .sort((a, b) => a.order - b.order)
                  .map((property) => (
                    <FormControl key={property.id} fullWidth>
                      <FormLabel required={property.isRequired}>
                        {property.propertyName}
                      </FormLabel>
                      {renderPropertyField(property)}
                    </FormControl>
                  ))}
              </Box>
            </AccordionDetails>
          </Accordion>
        </TabPanel>

        {/* Styles Tab */}
        <TabPanel value={activeTab} index={1}>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <Accordion defaultExpanded>
              <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                <Typography>Layout Styles</Typography>
              </AccordionSummary>
              <AccordionDetails>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                  <TextField
                    fullWidth
                    size="small"
                    label="Width"
                    value={getStyleValue('width') || ''}
                    onChange={(e) => updateStyle('width', e.target.value)}
                  />
                  <TextField
                    fullWidth
                    size="small"
                    label="Height"
                    value={getStyleValue('height') || ''}
                    onChange={(e) => updateStyle('height', e.target.value)}
                  />
                  <TextField
                    fullWidth
                    size="small"
                    label="Padding"
                    value={getStyleValue('padding') || ''}
                    onChange={(e) => updateStyle('padding', e.target.value)}
                  />
                  <TextField
                    fullWidth
                    size="small"
                    label="Margin"
                    value={getStyleValue('margin') || ''}
                    onChange={(e) => updateStyle('margin', e.target.value)}
                  />
                </Box>
              </AccordionDetails>
            </Accordion>

            <Accordion>
              <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                <Typography>Visual Styles</Typography>
              </AccordionSummary>
              <AccordionDetails>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                  <Box>
                    <FormLabel>Background Color</FormLabel>
                    <Box sx={{ display: 'flex', gap: 1, mt: 1 }}>
                      <TextField
                        fullWidth
                        size="small"
                        value={getStyleValue('backgroundColor') || ''}
                        onChange={(e) => updateStyle('backgroundColor', e.target.value)}
                        InputProps={{
                          startAdornment: (
                            <InputAdornment position="start">
                              <Box
                                sx={{
                                  width: 20,
                                  height: 20,
                                  backgroundColor: getStyleValue('backgroundColor') || '#ffffff',
                                  border: '1px solid #ccc',
                                  borderRadius: 1
                                }}
                              />
                            </InputAdornment>
                          )
                        }}
                      />
                      <input
                        type="color"
                        value={getStyleValue('backgroundColor') || '#ffffff'}
                        onChange={(e) => updateStyle('backgroundColor', e.target.value)}
                        style={{ width: 40, height: 32, cursor: 'pointer' }}
                      />
                    </Box>
                  </Box>

                  <TextField
                    fullWidth
                    size="small"
                    label="Border Radius"
                    value={getStyleValue('borderRadius') || ''}
                    onChange={(e) => updateStyle('borderRadius', e.target.value)}
                  />

                  <TextField
                    fullWidth
                    size="small"
                    label="Box Shadow"
                    value={getStyleValue('boxShadow') || ''}
                    onChange={(e) => updateStyle('boxShadow', e.target.value)}
                  />

                  <Box>
                    <Typography variant="body2" gutterBottom>
                      Opacity: {getStyleValue('opacity') || 1}
                    </Typography>
                    <Slider
                      value={Number(getStyleValue('opacity') || 1)}
                      onChange={(_, value) => updateStyle('opacity', String(value))}
                      min={0}
                      max={1}
                      step={0.1}
                      valueLabelDisplay="auto"
                    />
                  </Box>
                </Box>
              </AccordionDetails>
            </Accordion>

            <Accordion>
              <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                <Typography>Animation</Typography>
              </AccordionSummary>
              <AccordionDetails>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                  <FormControl fullWidth size="small">
                    <FormLabel>Animation Type</FormLabel>
                    <Select
                      value={component.animationType || ''}
                      onChange={(e) => updateBasicProperty('animationType', e.target.value)}
                    >
                      <MenuItem value="">None</MenuItem>
                      <MenuItem value="fade">Fade</MenuItem>
                      <MenuItem value="slide">Slide</MenuItem>
                      <MenuItem value="zoom">Zoom</MenuItem>
                      <MenuItem value="bounce">Bounce</MenuItem>
                      <MenuItem value="rotate">Rotate</MenuItem>
                    </Select>
                  </FormControl>

                  {component.animationType && (
                    <TextField
                      fullWidth
                      size="small"
                      type="number"
                      label="Duration (ms)"
                      value={component.animationDuration || 300}
                      onChange={(e) => updateBasicProperty('animationDuration', Number(e.target.value))}
                    />
                  )}
                </Box>
              </AccordionDetails>
            </Accordion>

            <Accordion>
              <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                <Typography>Custom CSS</Typography>
              </AccordionSummary>
              <AccordionDetails>
                <TextField
                  fullWidth
                  multiline
                  rows={4}
                  size="small"
                  label="Custom Classes"
                  value={component.customClasses || ''}
                  onChange={(e) => updateBasicProperty('customClasses', e.target.value)}
                  helperText="Add custom CSS classes separated by spaces"
                />
              </AccordionDetails>
            </Accordion>
          </Box>
        </TabPanel>

        {/* Actions Tab */}
        <TabPanel value={activeTab} index={2}>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <Typography variant="subtitle2">Component Actions</Typography>
              <Button
                size="small"
                startIcon={<AddIcon />}
                onClick={() => addAction({
                  actionType: 'Navigate',
                  actionTrigger: 'Click',
                  actionTarget: '',
                  requiresAuth: false,
                  priority: component.actions.length
                })}
              >
                Add Action
              </Button>
            </Box>

            <List>
              {component.actions.map((action, index) => (
                <ListItem key={action.id} sx={{ border: 1, borderColor: 'divider', borderRadius: 1, mb: 1 }}>
                  <Box sx={{ width: '100%' }}>
                    <Box sx={{ display: 'flex', gap: 2, mb: 1 }}>
                      <FormControl size="small" sx={{ flex: 1 }}>
                        <FormLabel>Trigger</FormLabel>
                        <Select
                          value={action.actionTrigger}
                          onChange={(e) => updateAction(action.id, { actionTrigger: e.target.value as ActionTrigger })}
                        >
                          <MenuItem value="Click">Click</MenuItem>
                          <MenuItem value="LongPress">Long Press</MenuItem>
                          <MenuItem value="Swipe">Swipe</MenuItem>
                          <MenuItem value="Load">Load</MenuItem>
                        </Select>
                      </FormControl>

                      <FormControl size="small" sx={{ flex: 1 }}>
                        <FormLabel>Type</FormLabel>
                        <Select
                          value={action.actionType}
                          onChange={(e) => updateAction(action.id, { actionType: e.target.value as ActionType })}
                        >
                          <MenuItem value="Navigate">Navigate</MenuItem>
                          <MenuItem value="OpenModal">Open Modal</MenuItem>
                          <MenuItem value="CallAPI">Call API</MenuItem>
                          <MenuItem value="Share">Share</MenuItem>
                          <MenuItem value="Download">Download</MenuItem>
                        </Select>
                      </FormControl>
                    </Box>

                    <TextField
                      fullWidth
                      size="small"
                      label="Target/Parameters"
                      value={action.actionTarget}
                      onChange={(e) => updateAction(action.id, { actionTarget: e.target.value })}
                      sx={{ mb: 1 }}
                    />

                    <FormControlLabel
                      control={
                        <Switch
                          checked={action.requiresAuth}
                          onChange={(e) => updateAction(action.id, { requiresAuth: e.target.checked })}
                        />
                      }
                      label="Requires Authentication"
                    />
                  </Box>

                  <ListItemSecondaryAction>
                    <IconButton edge="end" onClick={() => removeAction(action.id)}>
                      <DeleteIcon />
                    </IconButton>
                  </ListItemSecondaryAction>
                </ListItem>
              ))}
            </List>
          </Box>
        </TabPanel>

        {/* Data Tab */}
        <TabPanel value={activeTab} index={3}>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            {component.dataSource ? (
              <>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Typography variant="subtitle2">Data Source Configuration</Typography>
                  <IconButton size="small" onClick={removeDataSource}>
                    <DeleteIcon />
                  </IconButton>
                </Box>

                <FormControl fullWidth size="small">
                  <FormLabel>Source Type</FormLabel>
                  <Select
                    value={component.dataSource.sourceType}
                    onChange={(e) => updateDataSource({ sourceType: e.target.value as any })}
                  >
                    <MenuItem value="Static">Static</MenuItem>
                    <MenuItem value="API">API</MenuItem>
                    <MenuItem value="Database">Database</MenuItem>
                    <MenuItem value="Cache">Cache</MenuItem>
                  </Select>
                </FormControl>

                {component.dataSource.sourceType === 'API' && (
                  <>
                    <TextField
                      fullWidth
                      size="small"
                      label="Endpoint URL"
                      value={component.dataSource.dataEndpoint || ''}
                      onChange={(e) => updateDataSource({ dataEndpoint: e.target.value })}
                    />

                    <FormControl fullWidth size="small">
                      <FormLabel>HTTP Method</FormLabel>
                      <Select
                        value={component.dataSource.httpMethod || 'GET'}
                        onChange={(e) => updateDataSource({ httpMethod: e.target.value as any })}
                      >
                        <MenuItem value="GET">GET</MenuItem>
                        <MenuItem value="POST">POST</MenuItem>
                        <MenuItem value="PUT">PUT</MenuItem>
                        <MenuItem value="DELETE">DELETE</MenuItem>
                      </Select>
                    </FormControl>

                    <TextField
                      fullWidth
                      size="small"
                      multiline
                      rows={3}
                      label="Headers (JSON)"
                      value={component.dataSource.headers || '{}'}
                      onChange={(e) => updateDataSource({ headers: e.target.value })}
                    />
                  </>
                )}

                <FormControlLabel
                  control={
                    <Switch
                      checked={component.dataSource.useMockInDev}
                      onChange={(e) => updateDataSource({ useMockInDev: e.target.checked })}
                    />
                  }
                  label="Use Mock Data in Development"
                />

                {component.dataSource.useMockInDev && (
                  <TextField
                    fullWidth
                    size="small"
                    multiline
                    rows={6}
                    label="Mock Data (JSON)"
                    value={component.dataSource.mockData || '{}'}
                    onChange={(e) => updateDataSource({ mockData: e.target.value })}
                  />
                )}
              </>
            ) : (
              <Box sx={{ textAlign: 'center', py: 4 }}>
                <Typography variant="body2" color="text.secondary" gutterBottom>
                  No data source configured
                </Typography>
                <Button
                  variant="outlined"
                  startIcon={<DataIcon />}
                  onClick={() => updateDataSource({
                    sourceType: 'Static',
                    useMockInDev: true,
                    mockData: '{}'
                  })}
                >
                  Add Data Source
                </Button>
              </Box>
            )}
          </Box>
        </TabPanel>
      </Box>

      {errors.length > 0 && (
        <Box sx={{ p: 2, borderTop: 1, borderColor: 'divider', backgroundColor: 'error.light' }}>
          {errors.map((error, index) => (
            <Typography key={index} variant="caption" color="error">
              {error}
            </Typography>
          ))}
        </Box>
      )}
    </Paper>
  );
};

export default PropertyPanel;
import React, { useState, useRef, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Box,
  Button,
  IconButton,
  ToggleButton,
  ToggleButtonGroup,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Divider,
  Typography,
  Slider,
  Paper,
  Tooltip,
  CircularProgress
} from '@mui/material';
import {
  Close as CloseIcon,
  Smartphone as PhoneIcon,
  Tablet as TabletIcon,
  Computer as DesktopIcon,
  ScreenRotation as RotateIcon,
  ZoomIn as ZoomInIcon,
  ZoomOut as ZoomOutIcon,
  Refresh as RefreshIcon,
  Download as DownloadIcon,
  Code as CodeIcon,
  Fullscreen as FullscreenIcon
} from '@mui/icons-material';
import { usePreview } from '../../hooks/usePreview';
import type { HomeScreenTemplate } from '../../types/homeScreen.types';

interface PreviewModalProps {
  template: HomeScreenTemplate;
  open: boolean;
  onClose: () => void;
}

const PreviewModal: React.FC<PreviewModalProps> = ({ template, open, onClose }) => {
  const [deviceType, setDeviceType] = useState<'mobile' | 'tablet' | 'desktop'>('mobile');
  const [platform, setPlatform] = useState(template.platform);
  const [showCode, setShowCode] = useState(false);
  const previewContainerRef = useRef<HTMLDivElement>(null);

  const {
    previewData,
    isLoading,
    error,
    currentDevice,
    zoom,
    orientation,
    changeDevice,
    toggleOrientation,
    zoomIn,
    zoomOut,
    resetZoom,
    fitToScreen,
    previewStyles,
    refreshPreview,
    getPreviewHTML
  } = usePreview({
    template,
    platform,
    deviceType,
    useMockData: true
  });

  useEffect(() => {
    if (open && previewContainerRef.current) {
      const { offsetWidth, offsetHeight } = previewContainerRef.current;
      fitToScreen(offsetWidth - 48, offsetHeight - 48);
    }
  }, [open, deviceType, orientation, fitToScreen]);

  const handleDeviceChange = (
    event: React.MouseEvent<HTMLElement>,
    newDevice: 'mobile' | 'tablet' | 'desktop' | null
  ) => {
    if (newDevice) {
      setDeviceType(newDevice);
      changeDevice(newDevice);
    }
  };

  const handleExportHTML = () => {
    const html = getPreviewHTML();
    const blob = new Blob([html], { type: 'text/html' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `${template.name.replace(/\s+/g, '-').toLowerCase()}-preview.html`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  };

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth={false}
      fullWidth
      PaperProps={{
        sx: {
          height: '90vh',
          maxHeight: '90vh'
        }
      }}
    >
      <DialogTitle>
        <Box display="flex" alignItems="center" justifyContent="space-between">
          <Typography variant="h6">Preview: {template.name}</Typography>
          <IconButton onClick={onClose}>
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>

      <Divider />

      {/* Preview Toolbar */}
      <Box
        sx={{
          px: 3,
          py: 1,
          display: 'flex',
          alignItems: 'center',
          gap: 2,
          backgroundColor: 'background.paper',
          borderBottom: 1,
          borderColor: 'divider'
        }}
      >
        <ToggleButtonGroup
          value={deviceType}
          exclusive
          onChange={handleDeviceChange}
          size="small"
        >
          <ToggleButton value="mobile">
            <PhoneIcon />
          </ToggleButton>
          <ToggleButton value="tablet">
            <TabletIcon />
          </ToggleButton>
          <ToggleButton value="desktop">
            <DesktopIcon />
          </ToggleButton>
        </ToggleButtonGroup>

        <Divider orientation="vertical" flexItem />

        <FormControl size="small" sx={{ minWidth: 120 }}>
          <InputLabel>Platform</InputLabel>
          <Select
            value={platform}
            onChange={(e) => setPlatform(e.target.value as any)}
          >
            <MenuItem value="All">All</MenuItem>
            <MenuItem value="iOS">iOS</MenuItem>
            <MenuItem value="Android">Android</MenuItem>
          </Select>
        </FormControl>

        <Divider orientation="vertical" flexItem />

        <Tooltip title="Rotate">
          <IconButton onClick={toggleOrientation} disabled={deviceType === 'desktop'}>
            <RotateIcon />
          </IconButton>
        </Tooltip>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <IconButton onClick={zoomOut} size="small">
            <ZoomOutIcon />
          </IconButton>
          <Typography variant="body2" sx={{ minWidth: 50, textAlign: 'center' }}>
            {Math.round(zoom * 100)}%
          </Typography>
          <IconButton onClick={zoomIn} size="small">
            <ZoomInIcon />
          </IconButton>
        </Box>

        <Divider orientation="vertical" flexItem />

        <Tooltip title="Refresh">
          <IconButton onClick={refreshPreview}>
            <RefreshIcon />
          </IconButton>
        </Tooltip>

        <Tooltip title="View Code">
          <IconButton onClick={() => setShowCode(!showCode)}>
            <CodeIcon />
          </IconButton>
        </Tooltip>

        <Tooltip title="Download HTML">
          <IconButton onClick={handleExportHTML}>
            <DownloadIcon />
          </IconButton>
        </Tooltip>
      </Box>

      <DialogContent sx={{ p: 0, overflow: 'hidden' }}>
        {isLoading ? (
          <Box
            display="flex"
            alignItems="center"
            justifyContent="center"
            height="100%"
          >
            <CircularProgress />
          </Box>
        ) : error ? (
          <Box
            display="flex"
            alignItems="center"
            justifyContent="center"
            height="100%"
          >
            <Typography color="error">Error loading preview: {error.message}</Typography>
          </Box>
        ) : showCode ? (
          <Box sx={{ height: '100%', overflow: 'auto', p: 2 }}>
            <Paper sx={{ p: 2, backgroundColor: '#f5f5f5' }}>
              <pre style={{ margin: 0, whiteSpace: 'pre-wrap', wordWrap: 'break-word' }}>
                <code>{getPreviewHTML()}</code>
              </pre>
            </Paper>
          </Box>
        ) : (
          <Box
            ref={previewContainerRef}
            style={previewStyles.container}
          >
            <Box style={previewStyles.device}>
              {deviceType === 'mobile' && (
                <Box style={previewStyles.statusBar}>
                  <Box>9:41</Box>
                  <Box sx={{ display: 'flex', gap: 0.5 }}>
                    <Box>📶</Box>
                    <Box>📶</Box>
                    <Box>🔋</Box>
                  </Box>
                </Box>
              )}
              
              <Box style={previewStyles.screen}>
                {previewData?.sections.map((section) => (
                  <Box
                    key={section.id}
                    style={{
                      ...section.styles,
                      position: 'relative'
                    }}
                  >
                    {section.title && (
                      <Typography variant="h5" gutterBottom>
                        {section.title}
                      </Typography>
                    )}
                    {section.subtitle && (
                      <Typography variant="body2" color="text.secondary" gutterBottom>
                        {section.subtitle}
                      </Typography>
                    )}
                    
                    <Box
                      sx={{
                        display: 'grid',
                        gridTemplateColumns: 'repeat(12, 1fr)',
                        gap: 2
                      }}
                    >
                      {section.components.map((component) => (
                        <Box
                          key={component.id}
                          sx={{
                            gridColumn: `span ${component.colSpan}`,
                            gridRow: `span ${component.rowSpan}`,
                            ...component.styles
                          }}
                        >
                          <Paper
                            elevation={1}
                            sx={{
                              p: 2,
                              height: '100%',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              backgroundColor: 'background.paper'
                            }}
                          >
                            <Typography variant="body2" color="text.secondary">
                              {component.type} Component
                            </Typography>
                          </Paper>
                        </Box>
                      ))}
                    </Box>
                  </Box>
                ))}
              </Box>
            </Box>
          </Box>
        )}
      </DialogContent>

      <Divider />

      <DialogActions>
        <Box
          sx={{
            width: '100%',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            px: 2
          }}
        >
          <Typography variant="caption" color="text.secondary">
            Device: {currentDevice.name} • 
            {orientation === 'portrait' ? currentDevice.width : currentDevice.height} × 
            {orientation === 'portrait' ? currentDevice.height : currentDevice.width}
          </Typography>
          
          <Button onClick={onClose}>Close</Button>
        </Box>
      </DialogActions>
    </Dialog>
  );
};

export default PreviewModal;
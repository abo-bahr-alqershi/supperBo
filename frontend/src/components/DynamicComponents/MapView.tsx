import React, { useState, useEffect, useRef } from 'react';
import {
  Box,
  Paper,
  Typography,
  IconButton,
  Button,
  Fab,
  Menu,
  MenuItem,
  Skeleton,
  useTheme,
  useMediaQuery,
  Zoom,
  TextField,
  InputAdornment
} from '@mui/material';
import {
  MyLocation as LocationIcon,
  Layers as LayersIcon,
  ZoomIn as ZoomInIcon,
  ZoomOut as ZoomOutIcon,
  Fullscreen as FullscreenIcon,
  Search as SearchIcon,
  DirectionsCar as CarIcon,
  DirectionsWalk as WalkIcon,
  DirectionsBike as BikeIcon,
  Place as MarkerIcon
} from '@mui/icons-material';
import { motion } from 'framer-motion';

interface MapMarker {
  id: string;
  latitude: number;
  longitude: number;
  title: string;
  description?: string;
  icon?: string;
  color?: string;
}

interface MapViewProps {
  center?: { lat: number; lng: number };
  zoom?: number;
  markers?: MapMarker[];
  height?: number | string;
  showSearch?: boolean;
  showControls?: boolean;
  showMyLocation?: boolean;
  showLayerSelector?: boolean;
  showDirections?: boolean;
  mapStyle?: 'roadmap' | 'satellite' | 'terrain' | 'hybrid';
  allowInteraction?: boolean;
  data?: {
    markers: MapMarker[];
    center?: { lat: number; lng: number };
  };
  isPreview?: boolean;
  onMarkerClick?: (marker: MapMarker) => void;
  onLocationSelect?: (location: { lat: number; lng: number }) => void;
}

const MapView: React.FC<MapViewProps> = ({
  center = { lat: 40.7128, lng: -74.0060 }, // New York
  zoom = 13,
  markers = [],
  height = 400,
  showSearch = true,
  showControls = true,
  showMyLocation = true,
  showLayerSelector = true,
  showDirections = false,
  mapStyle = 'roadmap',
  allowInteraction = true,
  data,
  isPreview = false,
  onMarkerClick,
  onLocationSelect
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const mapRef = useRef<HTMLDivElement>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [currentMapStyle, setCurrentMapStyle] = useState(mapStyle);
  const [layerMenuAnchor, setLayerMenuAnchor] = useState<null | HTMLElement>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [showSearchBar, setShowSearchBar] = useState(false);
  const [selectedMarker, setSelectedMarker] = useState<MapMarker | null>(null);
  const [isFullscreen, setIsFullscreen] = useState(false);

  const displayMarkers = data?.markers || markers || generateMockMarkers();
  const displayCenter = data?.center || center;

  useEffect(() => {
    // Simulate map loading
    const timer = setTimeout(() => {
      setIsLoading(false);
    }, 1500);

    return () => clearTimeout(timer);
  }, []);

  const handleMarkerClick = (marker: MapMarker) => {
    if (isPreview) return;
    
    setSelectedMarker(marker);
    onMarkerClick?.(marker);
  };

  const handleMyLocation = () => {
    if (isPreview) return;
    
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition((position) => {
        const location = {
          lat: position.coords.latitude,
          lng: position.coords.longitude
        };
        onLocationSelect?.(location);
      });
    }
  };

  const handleZoomIn = () => {
    // In real implementation, this would control the map zoom
    console.log('Zoom in');
  };

  const handleZoomOut = () => {
    // In real implementation, this would control the map zoom
    console.log('Zoom out');
  };

  const handleFullscreen = () => {
    if (!mapRef.current) return;
    
    if (!isFullscreen) {
      if (mapRef.current.requestFullscreen) {
        mapRef.current.requestFullscreen();
      }
    } else {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      }
    }
    setIsFullscreen(!isFullscreen);
  };

  const handleSearch = () => {
    if (isPreview || !searchQuery) return;
    
    // In real implementation, this would search for the location
    console.log('Searching for:', searchQuery);
  };

  const mapStyles = [
    { value: 'roadmap', label: 'Road Map', icon: '🗺️' },
    { value: 'satellite', label: 'Satellite', icon: '🛰️' },
    { value: 'terrain', label: 'Terrain', icon: '⛰️' },
    { value: 'hybrid', label: 'Hybrid', icon: '🌍' }
  ];

  return (
    <Box
      ref={mapRef}
      sx={{
        position: 'relative',
        width: '100%',
        height: typeof height === 'number' ? `${height}px` : height,
        borderRadius: 1,
        overflow: 'hidden',
        backgroundColor: theme.palette.grey[100]
      }}
    >
      {isLoading ? (
        <Skeleton
          variant="rectangular"
          width="100%"
          height="100%"
          animation="wave"
        />
      ) : (
        <Box
          sx={{
            width: '100%',
            height: '100%',
            backgroundImage: `url(/api/placeholder/800/600)`,
            backgroundSize: 'cover',
            backgroundPosition: 'center',
            position: 'relative'
          }}
        >
          {/* Map Placeholder - In real app, this would be replaced with actual map */}
          <Box
            sx={{
              position: 'absolute',
              inset: 0,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              backgroundColor: 'rgba(255,255,255,0.9)'
            }}
          >
            <Typography variant="h6" color="text.secondary">
              Map View ({currentMapStyle})
            </Typography>
          </Box>

          {/* Markers */}
          {displayMarkers.map((marker) => (
            <motion.div
              key={marker.id}
              initial={{ scale: 0 }}
              animate={{ scale: 1 }}
              transition={{ duration: 0.3 }}
              style={{
                position: 'absolute',
                left: `${Math.random() * 80 + 10}%`,
                top: `${Math.random() * 80 + 10}%`,
                transform: 'translate(-50%, -100%)',
                cursor: allowInteraction ? 'pointer' : 'default'
              }}
              onClick={() => handleMarkerClick(marker)}
            >
              <Box
                sx={{
                  display: 'flex',
                  flexDirection: 'column',
                  alignItems: 'center'
                }}
              >
                <MarkerIcon
                  sx={{
                    fontSize: 40,
                    color: marker.color || theme.palette.error.main,
                    filter: 'drop-shadow(0 2px 4px rgba(0,0,0,0.2))'
                  }}
                />
                {marker.title && (
                  <Paper
                    elevation={2}
                    sx={{
                      px: 1,
                      py: 0.5,
                      mt: 0.5,
                      maxWidth: 150,
                      textAlign: 'center'
                    }}
                  >
                    <Typography variant="caption" noWrap>
                      {marker.title}
                    </Typography>
                  </Paper>
                )}
              </Box>
            </motion.div>
          ))}
        </Box>
      )}

      {/* Search Bar */}
      {showSearch && (
        <Zoom in={showSearchBar}>
          <Paper
            elevation={4}
            sx={{
              position: 'absolute',
              top: 16,
              left: 16,
              right: showControls ? 80 : 16,
              p: 1,
              display: showSearchBar ? 'block' : 'none'
            }}
          >
            <TextField
              fullWidth
              size="small"
              placeholder="Search location..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton size="small" onClick={handleSearch}>
                      <SearchIcon />
                    </IconButton>
                  </InputAdornment>
                )
              }}
            />
          </Paper>
        </Zoom>
      )}

      {/* Controls */}
      {showControls && (
        <Box
          sx={{
            position: 'absolute',
            top: 16,
            right: 16,
            display: 'flex',
            flexDirection: 'column',
            gap: 1
          }}
        >
          {showSearch && (
            <Fab
              size="small"
              color="primary"
              onClick={() => setShowSearchBar(!showSearchBar)}
            >
              <SearchIcon />
            </Fab>
          )}
          
          {showLayerSelector && (
            <>
              <Fab
                size="small"
                onClick={(e) => setLayerMenuAnchor(e.currentTarget)}
              >
                <LayersIcon />
              </Fab>
              <Menu
                anchorEl={layerMenuAnchor}
                open={Boolean(layerMenuAnchor)}
                onClose={() => setLayerMenuAnchor(null)}
              >
                {mapStyles.map((style) => (
                  <MenuItem
                    key={style.value}
                    selected={currentMapStyle === style.value}
                    onClick={() => {
                      setCurrentMapStyle(style.value as any);
                      setLayerMenuAnchor(null);
                    }}
                  >
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                      <span>{style.icon}</span>
                      <Typography>{style.label}</Typography>
                    </Box>
                  </MenuItem>
                ))}
              </Menu>
            </>
          )}
          
          <Fab size="small" onClick={handleZoomIn}>
            <ZoomInIcon />
          </Fab>
          
          <Fab size="small" onClick={handleZoomOut}>
            <ZoomOutIcon />
          </Fab>
          
          {showMyLocation && (
            <Fab size="small" onClick={handleMyLocation}>
              <LocationIcon />
            </Fab>
          )}
          
          <Fab size="small" onClick={handleFullscreen}>
            <FullscreenIcon />
          </Fab>
        </Box>
      )}

      {/* Directions Panel */}
      {showDirections && !isMobile && (
        <Paper
          elevation={4}
          sx={{
            position: 'absolute',
            bottom: 16,
            left: 16,
            p: 2,
            minWidth: 200
          }}
        >
          <Typography variant="subtitle2" gutterBottom>
            Travel Mode
          </Typography>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <IconButton color="primary">
              <CarIcon />
            </IconButton>
            <IconButton>
              <WalkIcon />
            </IconButton>
            <IconButton>
              <BikeIcon />
            </IconButton>
          </Box>
        </Paper>
      )}

      {/* Selected Marker Info */}
      {selectedMarker && (
        <Zoom in={Boolean(selectedMarker)}>
          <Paper
            elevation={4}
            sx={{
              position: 'absolute',
              bottom: 16,
              right: 16,
              p: 2,
              maxWidth: 300
            }}
          >
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
              <Box>
                <Typography variant="h6" gutterBottom>
                  {selectedMarker.title}
                </Typography>
                {selectedMarker.description && (
                  <Typography variant="body2" color="text.secondary">
                    {selectedMarker.description}
                  </Typography>
                )}
              </Box>
              <IconButton
                size="small"
                onClick={() => setSelectedMarker(null)}
              >
                <CloseIcon />
              </IconButton>
            </Box>
          </Paper>
        </Zoom>
      )}
    </Box>
  );
};

// Generate mock markers for preview
function generateMockMarkers(): MapMarker[] {
  return [
    {
      id: 'marker-1',
      latitude: 40.7128,
      longitude: -74.0060,
      title: 'Location 1',
      description: 'Description for location 1',
      color: '#ff0000'
    },
    {
      id: 'marker-2',
      latitude: 40.7228,
      longitude: -74.0160,
      title: 'Location 2',
      description: 'Description for location 2',
      color: '#00ff00'
    },
    {
      id: 'marker-3',
      latitude: 40.7028,
      longitude: -74.0260,
      title: 'Location 3',
      description: 'Description for location 3',
      color: '#0000ff'
    }
  ];
}

export default MapView;
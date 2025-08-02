import React, { useState } from 'react';
import {
  Box,
  Card,
  CardMedia,
  CardContent,
  Typography,
  Chip,
  Button,
  IconButton,
  Grid,
  Skeleton,
  Rating,
  Divider,
  useTheme,
  useMediaQuery,
  List,
  ListItem
} from '@mui/material';
import {
  LocationOn as LocationIcon,
  Bed as BedIcon,
  Bathtub as BathIcon,
  SquareFoot as AreaIcon,
  FavoriteBorder as FavoriteIcon,
  Favorite as FavoriteFilledIcon,
  Share as ShareIcon,
  CalendarToday as CalendarIcon,
  LocalParking as ParkingIcon
} from '@mui/icons-material';
import { motion } from 'framer-motion';

interface Property {
  id: string;
  title: string;
  location: string;
  price: number;
  priceType?: 'sale' | 'rent';
  imageUrl: string;
  images?: string[];
  bedrooms: number;
  bathrooms: number;
  area: number;
  areaUnit?: 'sqft' | 'sqm';
  type: string;
  featured?: boolean;
  rating?: number;
  parking?: number;
  yearBuilt?: number;
  description?: string;
  amenities?: string[];
  agentName?: string;
  agentImage?: string;
}

interface PropertyListProps {
  title?: string;
  subtitle?: string;
  properties?: Property[];
  layout?: 'grid' | 'list';
  columns?: number;
  showFilters?: boolean;
  showRating?: boolean;
  showAgent?: boolean;
  cardElevation?: number;
  data?: Property[];
  isPreview?: boolean;
  onPropertyClick?: (property: Property) => void;
  onFavoriteClick?: (property: Property) => void;
  onContactAgent?: (property: Property) => void;
}

const PropertyList: React.FC<PropertyListProps> = ({
  title = 'Featured Properties',
  subtitle,
  properties = [],
  layout = 'grid',
  columns = 3,
  showFilters = false,
  showRating = true,
  showAgent = false,
  cardElevation = 1,
  data,
  isPreview = false,
  onPropertyClick,
  onFavoriteClick,
  onContactAgent
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const isTablet = useMediaQuery(theme.breakpoints.down('md'));
  const [favorites, setFavorites] = useState<Set<string>>(new Set());
  const [imageLoadStates, setImageLoadStates] = useState<Record<string, boolean>>({});

  const displayProperties = data || properties || generateMockProperties();
  const responsiveColumns = isMobile ? 1 : isTablet ? 2 : columns;

  const handleImageLoad = (propertyId: string) => {
    setImageLoadStates(prev => ({ ...prev, [propertyId]: true }));
  };

  const handleFavoriteToggle = (property: Property) => {
    if (isPreview) return;
    
    setFavorites(prev => {
      const newFavorites = new Set(prev);
      if (newFavorites.has(property.id)) {
        newFavorites.delete(property.id);
      } else {
        newFavorites.add(property.id);
      }
      return newFavorites;
    });
    onFavoriteClick?.(property);
  };

  const formatPrice = (price: number, priceType?: 'sale' | 'rent') => {
    const formatted = new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      maximumFractionDigits: 0
    }).format(price);
    return priceType === 'rent' ? `${formatted}/mo` : formatted;
  };

  const renderPropertyCard = (property: Property) => {
    const isFavorite = favorites.has(property.id);

    const cardContent = (
      <>
        <Box sx={{ position: 'relative', height: layout === 'list' ? 200 : 240 }}>
          {!imageLoadStates[property.id] && (
            <Skeleton variant="rectangular" width="100%" height="100%" />
          )}
          <CardMedia
            component="img"
            height="100%"
            image={property.imageUrl}
            alt={property.title}
            onLoad={() => handleImageLoad(property.id)}
            sx={{
              display: imageLoadStates[property.id] ? 'block' : 'none',
              objectFit: 'cover'
            }}
          />
          
          {/* Badges */}
          <Box sx={{ position: 'absolute', top: 8, left: 8, display: 'flex', gap: 1 }}>
            {property.featured && (
              <Chip label="Featured" size="small" color="primary" />
            )}
            {property.type && (
              <Chip 
                label={property.type} 
                size="small" 
                sx={{ backgroundColor: 'rgba(255,255,255,0.9)' }} 
              />
            )}
          </Box>

          {/* Favorite Button */}
          <IconButton
            size="small"
            onClick={(e) => {
              e.stopPropagation();
              handleFavoriteToggle(property);
            }}
            sx={{
              position: 'absolute',
              top: 8,
              right: 8,
              backgroundColor: 'rgba(255,255,255,0.9)',
              '&:hover': {
                backgroundColor: 'rgba(255,255,255,1)'
              }
            }}
          >
            {isFavorite ? <FavoriteFilledIcon color="error" /> : <FavoriteIcon />}
          </IconButton>

          {/* Price Badge */}
          <Box
            sx={{
              position: 'absolute',
              bottom: 0,
              left: 0,
              right: 0,
              background: 'linear-gradient(to top, rgba(0,0,0,0.8), transparent)',
              p: 2,
              color: 'white'
            }}
          >
            <Typography variant="h5" fontWeight="bold">
              {formatPrice(property.price, property.priceType)}
            </Typography>
          </Box>
        </Box>

        <CardContent>
          <Typography variant="h6" component="h3" gutterBottom noWrap>
            {property.title}
          </Typography>

          <Box sx={{ display: 'flex', alignItems: 'center', mb: 2, color: 'text.secondary' }}>
            <LocationIcon fontSize="small" sx={{ mr: 0.5 }} />
            <Typography variant="body2" noWrap>
              {property.location}
            </Typography>
          </Box>

          {/* Property Features */}
          <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
            <Box sx={{ display: 'flex', alignItems: 'center' }}>
              <BedIcon fontSize="small" sx={{ mr: 0.5, color: 'text.secondary' }} />
              <Typography variant="body2">{property.bedrooms}</Typography>
            </Box>
            <Box sx={{ display: 'flex', alignItems: 'center' }}>
              <BathIcon fontSize="small" sx={{ mr: 0.5, color: 'text.secondary' }} />
              <Typography variant="body2">{property.bathrooms}</Typography>
            </Box>
            <Box sx={{ display: 'flex', alignItems: 'center' }}>
              <AreaIcon fontSize="small" sx={{ mr: 0.5, color: 'text.secondary' }} />
              <Typography variant="body2">
                {property.area} {property.areaUnit || 'sqft'}
              </Typography>
            </Box>
          </Box>

          {/* Rating */}
          {showRating && property.rating && (
            <Box sx={{ mb: 2 }}>
              <Rating value={property.rating} readOnly size="small" />
            </Box>
          )}

          {/* Description (for list view) */}
          {layout === 'list' && property.description && (
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              {property.description}
            </Typography>
          )}

          {/* Agent Info */}
          {showAgent && property.agentName && (
            <>
              <Divider sx={{ my: 2 }} />
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                  {property.agentImage && (
                    <Box
                      component="img"
                      src={property.agentImage}
                      alt={property.agentName}
                      sx={{
                        width: 32,
                        height: 32,
                        borderRadius: '50%',
                        mr: 1
                      }}
                    />
                  )}
                  <Typography variant="body2">{property.agentName}</Typography>
                </Box>
                <Button
                  size="small"
                  onClick={(e) => {
                    e.stopPropagation();
                    onContactAgent?.(property);
                  }}
                >
                  Contact
                </Button>
              </Box>
            </>
          )}
        </CardContent>
      </>
    );

    return (
      <Card
        elevation={cardElevation}
        sx={{
          height: '100%',
          display: 'flex',
          flexDirection: layout === 'list' ? 'row' : 'column',
          cursor: isPreview ? 'default' : 'pointer',
          transition: 'all 0.3s',
          '&:hover': {
            transform: isPreview ? 'none' : 'translateY(-4px)',
            boxShadow: isPreview ? cardElevation : 4
          }
        }}
        onClick={() => !isPreview && onPropertyClick?.(property)}
      >
        {layout === 'list' ? (
          <Box sx={{ display: 'flex', width: '100%' }}>
            <Box sx={{ width: '40%', flexShrink: 0 }}>
              {cardContent}
            </Box>
          </Box>
        ) : (
          cardContent
        )}
      </Card>
    );
  };

  return (
    <Box sx={{ width: '100%' }}>
      {/* Header */}
      {(title || subtitle) && (
        <Box sx={{ mb: 3 }}>
          {title && (
            <Typography variant="h5" component="h2" fontWeight="bold" gutterBottom>
              {title}
            </Typography>
          )}
          {subtitle && (
            <Typography variant="body1" color="text.secondary">
              {subtitle}
            </Typography>
          )}
        </Box>
      )}

      {/* Property Grid/List */}
      {layout === 'grid' ? (
        <Grid container spacing={2}>
          {displayProperties.map((property) => (
            <Grid item xs={12} sm={12 / responsiveColumns} key={property.id}>
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.3 }}
              >
                {renderPropertyCard(property)}
              </motion.div>
            </Grid>
          ))}
        </Grid>
      ) : (
        <List sx={{ width: '100%' }}>
          {displayProperties.map((property, index) => (
            <ListItem key={property.id} sx={{ p: 0, mb: 2 }}>
              <motion.div
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ duration: 0.3, delay: index * 0.1 }}
                style={{ width: '100%' }}
              >
                {renderPropertyCard(property)}
              </motion.div>
            </ListItem>
          ))}
        </List>
      )}
    </Box>
  );
};

// Generate mock properties for preview
function generateMockProperties(): Property[] {
  const types = ['Apartment', 'House', 'Villa', 'Condo', 'Townhouse'];
  const locations = [
    'Downtown Manhattan',
    'Brooklyn Heights',
    'Upper East Side',
    'Williamsburg',
    'Long Island City'
  ];

  return Array.from({ length: 6 }).map((_, index) => ({
    id: `property-${index}`,
    title: `Modern ${types[index % types.length]} with City View`,
    location: locations[index % locations.length],
    price: 450000 + index * 100000,
    priceType: index % 2 === 0 ? 'sale' : 'rent' as const,
    imageUrl: `/api/placeholder/400/300`,
    bedrooms: 2 + (index % 3),
    bathrooms: 1 + (index % 2),
    area: 1200 + index * 200,
    type: types[index % types.length],
    featured: index < 2,
    rating: 4 + (index % 2) * 0.5,
    parking: 1 + (index % 2),
    yearBuilt: 2020 - (index % 5),
    description: 'Beautiful property with modern amenities and stunning views.',
    amenities: ['Gym', 'Pool', 'Parking', 'Security'],
    agentName: `Agent ${index + 1}`,
    agentImage: `/api/placeholder/40/40`
  }));
}

export default PropertyList;
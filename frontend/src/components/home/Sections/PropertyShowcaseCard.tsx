// frontend/src/components/Properties/PropertyShowcaseCard.tsx
import React from 'react';
import {
  Card,
  CardMedia,
  CardContent,
  Typography,
  Box,
  Chip,
  Rating,
  Button,
  Divider,
} from '@mui/material';
import {
  LocationOn,
  Hotel,
  Star,
  Wifi,
  Pool,
  LocalParking,
  Restaurant,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';

interface PropertyShowcaseCardProps {
  property: any;
}

const amenityIcons: Record<string, React.ReactElement> = {
  'Wi-Fi': <Wifi />,
  'Pool': <Pool />,
  'Parking': <LocalParking />,
  'Restaurant': <Restaurant />,
};

const PropertyShowcaseCard: React.FC<PropertyShowcaseCardProps> = ({ property }) => {
  const navigate = useNavigate();

  const handleViewDetails = () => {
    navigate(`/properties/${property.id}`);
  };

  return (
    <motion.div whileHover={{ scale: 1.02 }} transition={{ duration: 0.3 }}>
      <Card
        sx={{
          height: 400,
          display: 'flex',
          flexDirection: 'column',
          position: 'relative',
          overflow: 'hidden',
        }}
      >
        <Box sx={{ position: 'relative', height: '60%' }}>
          <CardMedia
            component="img"
            height="100%"
            image={property.mainImageUrl || property.images?.[0]}
            alt={property.name}
            sx={{ objectFit: 'cover' }}
          />
          
          {/* التدرج */}
          <Box
            sx={{
              position: 'absolute',
              bottom: 0,
              left: 0,
              right: 0,
              height: '30%',
              background: 'linear-gradient(to top, rgba(0,0,0,0.7), transparent)',
            }}
          />
          
          {/* الشارات */}
          <Box sx={{ position: 'absolute', top: 16, left: 16, display: 'flex', gap: 1 }}>
            {property.starRating && (
              <Chip
                icon={<Star />}
                label={`${property.starRating} نجوم`}
                size="small"
                sx={{
                  backgroundColor: 'warning.main',
                  color: 'white',
                  fontWeight: 'bold',
                }}
              />
            )}
            {property.isFeatured && (
              <Chip
                label="مميز"
                size="small"
                sx={{
                  backgroundColor: 'error.main',
                  color: 'white',
                  fontWeight: 'bold',
                }}
              />
            )}
          </Box>
          
          {/* السعر */}
          {property.basePrice && (
            <Box
              sx={{
                position: 'absolute',
                bottom: 16,
                right: 16,
                backgroundColor: 'rgba(0, 0, 0, 0.8)',
                borderRadius: 2,
                px: 2,
                py: 1,
              }}
            >
              <Typography variant="h5" color="white" fontWeight="bold">
                {property.basePrice.toLocaleString()} {property.currency || 'ريال'}
              </Typography>
              <Typography variant="caption" color="white">
                لليلة الواحدة
              </Typography>
            </Box>
          )}
        </Box>
        
        <CardContent sx={{ flexGrow: 1, display: 'flex', flexDirection: 'column' }}>
          <Typography variant="h6" fontWeight="bold" gutterBottom noWrap>
            {property.name}
          </Typography>
          
          <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
            <LocationOn sx={{ fontSize: 18, color: 'text.secondary', mr: 0.5 }} />
            <Typography variant="body2" color="text.secondary">
              {property.address}, {property.city}
            </Typography>
          </Box>
          
          <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
            <Rating
              value={property.averageRating || 0}
              precision={0.5}
              size="small"
              readOnly
            />
            <Typography variant="body2" color="text.secondary" sx={{ ml: 1 }}>
              ({property.averageRating}) • {property.viewCount} مشاهدة
            </Typography>
          </Box>
          
          <Divider sx={{ my: 1 }} />
          
          {/* المرافق */}
          <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mb: 2 }}>
            {property.amenities?.slice(0, 4).map((amenity: string, index: number) => (
              <Chip
                key={index}
                icon={amenityIcons[amenity]}
                label={amenity}
                size="small"
                variant="outlined"
              />
            ))}
          </Box>
          
          <Button
            variant="contained"
            fullWidth
            onClick={handleViewDetails}
            sx={{ mt: 'auto' }}
          >
            عرض التفاصيل
          </Button>
        </CardContent>
      </Card>
    </motion.div>
  );
};

export default PropertyShowcaseCard;                
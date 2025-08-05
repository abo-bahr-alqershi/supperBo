// frontend/src/components/Home/Sections/FeaturedPropertyAd.tsx
import React from 'react';
import { 
  Box, 
  Card, 
  CardMedia, 
  CardContent, 
  Typography, 
  Button, 
  Chip, 
  Rating,
  useTheme 
} from '@mui/material';
import { LocationOn, Star } from '@mui/icons-material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';

interface FeaturedPropertyAdProps {
  section: DynamicHomeSection;
}

const FeaturedPropertyAd: React.FC<FeaturedPropertyAdProps> = ({ section }) => {
  const theme = useTheme();
  const navigate = useNavigate();
  
  // استخراج العقار من المحتوى
  const propertyContent = section.content.find(item => item.contentType === 'PROPERTY');
  if (!propertyContent) return null;
  
  const property = propertyContent.data;
  const config = section.sectionConfig || {};
  const styleSettings = config.styleSettings || {};
  const animationSettings = config.animationSettings || {};
  
  const handleClick = () => {
    navigate(`/properties/${property.id}`);
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.6 }}
    >
      <Card
        sx={{
          position: 'relative',
          overflow: 'hidden',
          borderRadius: styleSettings.borderRadius || 3,
          boxShadow: theme.shadows[8],
          cursor: 'pointer',
          '&:hover': {
            transform: 'translateY(-4px)',
            boxShadow: theme.shadows[12],
            transition: 'all 0.3s ease',
          },
        }}
        onClick={handleClick}
      >
        {/* صورة العقار مع تأثير Parallax */}
        <Box sx={{ position: 'relative', height: { xs: 300, md: 400 }, overflow: 'hidden' }}>
          <CardMedia
            component={motion.img}
            image={property.mainImageUrl || property.images?.[0]}
            alt={property.name}
            sx={{ height: '100%', width: '100%', objectFit: 'cover' }}
            whileHover={{ scale: 1.05 }}
            transition={{ duration: 0.3 }}
          />
          
          {/* طبقة التدرج */}
          <Box
            sx={{
              position: 'absolute',
              bottom: 0,
              left: 0,
              right: 0,
              height: '50%',
              background: 'linear-gradient(to top, rgba(0,0,0,0.8), transparent)',
            }}
          />
          
          {/* الشارات */}
          <Box sx={{ position: 'absolute', top: 16, left: 16, display: 'flex', gap: 1 }}>
            {property.isFeatured && (
              <Chip
                label="مميز"
                icon={<Star />}
                sx={{
                  backgroundColor: 'warning.main',
                  color: 'white',
                  fontWeight: 'bold',
                }}
              />
            )}
            {property.discountPercentage && (
              <Chip
                label={`خصم ${property.discountPercentage}%`}
                sx={{
                  backgroundColor: 'error.main',
                  color: 'white',
                  fontWeight: 'bold',
                }}
              />
            )}
          </Box>
          
          {/* معلومات العقار */}
          <Box
            sx={{
              position: 'absolute',
              bottom: 0,
              left: 0,
              right: 0,
              p: 3,
              color: 'white',
            }}
          >
            <Typography variant="h4" fontWeight="bold" gutterBottom>
              {property.name}
            </Typography>
            
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <LocationOn sx={{ mr: 1 }} />
              <Typography variant="body1">
                {property.address}, {property.city}
              </Typography>
            </Box>
            
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Rating value={property.averageRating} precision={0.5} readOnly sx={{ color: 'warning.main' }} />
                <Typography variant="body2" sx={{ ml: 1 }}>
                  ({property.averageRating})
                </Typography>
              </Box>
              
              {property.basePrice && (
                <Box>
                  <Typography variant="h5" fontWeight="bold">
                    {property.basePrice.toLocaleString()} {property.currency}
                  </Typography>
                  <Typography variant="caption">لليلة الواحدة</Typography>
                </Box>
              )}
            </Box>
          </Box>
        </Box>
        
        {/* الرسالة الترويجية */}
        {property.promotionalMessage && (
          <CardContent>
            <Typography variant="body1" color="primary" fontWeight="medium" textAlign="center">
              {property.promotionalMessage}
            </Typography>
          </CardContent>
        )}
      </Card>
    </motion.div>
  );
};

export default FeaturedPropertyAd;
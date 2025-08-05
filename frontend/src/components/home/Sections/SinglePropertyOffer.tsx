// frontend/src/components/Home/Sections/SinglePropertyOffer.tsx
import React from 'react';
import { Box, Paper, Grid, Typography, Button, Chip } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import { LocationOn, Star, LocalOffer } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';

interface SinglePropertyOfferProps {
  section: DynamicHomeSection;
}

const SinglePropertyOffer: React.FC<SinglePropertyOfferProps> = ({ section }) => {
  const navigate = useNavigate();
  
  const offerContent = section.content.find(
    item => item.contentType === 'OFFER' || item.contentType === 'PROPERTY'
  );
  
  if (!offerContent) return null;
  
  const data = offerContent.contentData;
  const config = section.sectionConfig || {};

  return (
    <motion.div
      initial={{ opacity: 0, y: 30 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
    >
      <Paper
        elevation={4}
        sx={{
          overflow: 'hidden',
          backgroundColor: config.backgroundColor || 'background.paper',
        }}
      >
        <Grid container>
          <Grid item xs={12} md={7}>
            <Box
              component="img"
              src={data.mainImageUrl || data.customImageUrl || data.images?.[0]}
              alt={data.name || data.title}
              sx={{
                width: '100%',
                height: { xs: 300, md: 400 },
                objectFit: 'cover',
              }}
            />
          </Grid>
          
          <Grid item xs={12} md={5}>
            <Box sx={{ p: 4, height: '100%', display: 'flex', flexDirection: 'column' }}>
              {/* العنوان والعنوان الفرعي */}
              <Typography variant="h4" fontWeight="bold" gutterBottom>
                {data.name || data.title || section.title}
              </Typography>
              
              {(data.subtitle || section.subtitle) && (
                <Typography variant="body1" color="text.secondary" paragraph>
                  {data.subtitle || section.subtitle}
                </Typography>
              )}
              
              {/* الموقع والتقييم */}
              {data.city && (
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                  <LocationOn sx={{ mr: 1, color: 'text.secondary' }} />
                  <Typography variant="body2" color="text.secondary">
                    {data.address ? `${data.address}, ` : ''}{data.city}
                  </Typography>
                </Box>
              )}
              
              {data.averageRating && (
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
                  <Star sx={{ color: 'warning.main', mr: 1 }} />
                  <Typography variant="body1">
                    {data.averageRating} ({data.reviewCount || 0} تقييم)
                  </Typography>
                </Box>
              )}
              
              {/* السعر والخصم */}
              <Box sx={{ my: 3 }}>
                {data.discountPercentage && (
                  <Chip
                    icon={<LocalOffer />}
                    label={`خصم ${data.discountPercentage}%`}
                    color="error"
                    sx={{ mb: 2 }}
                  />
                )}
                
                {data.basePrice && (
                  <Box>
                    <Typography variant="h3" color="primary" fontWeight="bold">
                      {data.basePrice.toLocaleString()} {data.currency || 'ريال'}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      لليلة الواحدة
                    </Typography>
                  </Box>
                )}
              </Box>
              
              {/* الوصف */}
              {data.description && (
                <Typography variant="body2" paragraph sx={{ flexGrow: 1 }}>
                  {data.description}
                </Typography>
              )}
              
              {/* زر الإجراء */}
              <Button
                variant="contained"
                size="large"
                fullWidth
                onClick={() => {
                  if (data.ctaAction === 'navigate' && data.ctaData?.route) {
                    navigate(data.ctaData.route);
                  } else {
                    navigate(`/properties/${data.id}`);
                  }
                }}
                startIcon={<LocalOffer />}
              >
                {data.ctaText || 'احجز الآن'}
              </Button>
            </Box>
          </Grid>
        </Grid>
      </Paper>
    </motion.div>
  );
};

export default SinglePropertyOffer;
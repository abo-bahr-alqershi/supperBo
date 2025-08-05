// frontend/src/components/Home/Sections/UnitShowcaseAd.tsx
import React from 'react';
import { Box, Grid, Typography, Card, CardMedia, CardContent, Button } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import { Bed, Bathtub, AspectRatio, People } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

interface UnitShowcaseAdProps {
  section: DynamicHomeSection;
}

const UnitShowcaseAd: React.FC<UnitShowcaseAdProps> = ({ section }) => {
  const navigate = useNavigate();
  
  const unitContent = section.content.find(item => item.contentType === 'PROPERTY');
  if (!unitContent) return null;
  
  const unit = unitContent.contentData;

  return (
    <Card sx={{ overflow: 'hidden' }}>
      <Grid container>
        <Grid item xs={12} md={6}>
          <CardMedia
            component="img"
            height="400"
            image={unit.mainImageUrl || unit.images?.[0]}
            alt={unit.name}
          />
        </Grid>
        
        <Grid item xs={12} md={6}>
          <CardContent sx={{ p: 4, height: '100%', display: 'flex', flexDirection: 'column' }}>
            <Typography variant="h4" fontWeight="bold" gutterBottom>
              {unit.name}
            </Typography>
            
            <Typography variant="body1" color="text.secondary" paragraph>
              {unit.description}
            </Typography>
            
            {/* مواصفات الوحدة */}
            <Grid container spacing={2} sx={{ my: 3 }}>
              <Grid item xs={6} sm={3}>
                <Box sx={{ textAlign: 'center' }}>
                  <Bed color="primary" />
                  <Typography variant="body2">{unit.bedrooms} غرف نوم</Typography>
                </Box>
              </Grid>
              <Grid item xs={6} sm={3}>
                <Box sx={{ textAlign: 'center' }}>
                  <Bathtub color="primary" />
                  <Typography variant="body2">{unit.bathrooms} حمام</Typography>
                </Box>
              </Grid>
              <Grid item xs={6} sm={3}>
                <Box sx={{ textAlign: 'center' }}>
                  <AspectRatio color="primary" />
                  <Typography variant="body2">{unit.area} م²</Typography>
                </Box>
              </Grid>
              <Grid item xs={6} sm={3}>
                <Box sx={{ textAlign: 'center' }}>
                  <People color="primary" />
                  <Typography variant="body2">{unit.maxGuests} ضيوف</Typography>
                </Box>
              </Grid>
            </Grid>
            
            {/* السعر */}
            {unit.basePrice && (
              <Box sx={{ my: 3 }}>
                <Typography variant="h3" color="primary" fontWeight="bold">
                  {unit.basePrice.toLocaleString()} {unit.currency || 'ريال'}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  لليلة الواحدة
                </Typography>
              </Box>
            )}
            
            <Button
              variant="contained"
              size="large"
              fullWidth
              onClick={() => navigate(`/properties/${unit.id}`)}
              sx={{ mt: 'auto' }}
            >
              عرض التفاصيل والحجز
            </Button>
          </CardContent>
        </Grid>
      </Grid>
    </Card>
  );
};

export default UnitShowcaseAd;
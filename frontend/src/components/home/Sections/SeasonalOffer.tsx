// frontend/src/components/Home/Sections/SeasonalOffer.tsx
import React from 'react';
import { Box, Paper, Typography, Grid, Button, Chip } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import { AcUnit, WbSunny, LocalFlorist, BeachAccess } from '@mui/icons-material';
import PropertyCard from '../Properties/PropertyCard';
import { useNavigate } from 'react-router-dom';

interface SeasonalOfferProps {
  section: DynamicHomeSection;
}

const seasonThemes = {
  winter: {
    icon: <AcUnit />,
    gradient: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    bgPattern: 'url("/patterns/snow.svg")',
  },
  summer: {
    icon: <WbSunny />,
    gradient: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
    bgPattern: 'url("/patterns/sun.svg")',
  },
  spring: {
    icon: <LocalFlorist />,
    gradient: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
    bgPattern: 'url("/patterns/flowers.svg")',
  },
  beach: {
    icon: <BeachAccess />,
    gradient: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
    bgPattern: 'url("/patterns/waves.svg")',
  },
};

const SeasonalOffer: React.FC<SeasonalOfferProps> = ({ section }) => {
  const navigate = useNavigate();
  
  const config = section.config || {};
  const season = config.season || 'summer';
  const theme = seasonThemes[season as keyof typeof seasonThemes] || seasonThemes.summer;
  
  const properties = section.content
    .filter(item => item.contentType === 'PROPERTY')
    .map(item => item.data);

  return (
    <Paper
      elevation={4}
      sx={{
        p: 4,
        background: theme.gradient,
        backgroundImage: theme.bgPattern,
        backgroundSize: 'cover',
        backgroundBlendMode: 'overlay',
        color: 'white',
        position: 'relative',
        overflow: 'hidden',
      }}
    >
      {/* رأس القسم */}
      <Box sx={{ textAlign: 'center', mb: 4 }}>
        <Box sx={{ display: 'flex', justifyContent: 'center', mb: 2 }}>
          {React.cloneElement(theme.icon, { sx: { fontSize: 60, opacity: 0.8 } })}
        </Box>
        
        <Typography variant="h3" fontWeight="bold" gutterBottom>
          {section.title}
        </Typography>
        
        {section.subtitle && (
          <Typography variant="h6" sx={{ opacity: 0.9, mb: 2 }}>
            {section.subtitle}
          </Typography>
        )}
        
        {config.offerText && (
          <Chip
            label={config.offerText}
            size="large"
            sx={{
              backgroundColor: 'rgba(255, 255, 255, 0.2)',
              color: 'white',
              fontSize: '1rem',
              py: 3,
              px: 4,
            }}
          />
        )}
      </Box>
      
      {/* العقارات */}
      {properties.length > 0 && (
        <Box sx={{ mb: 4 }}>
          <Grid container spacing={3}>
            {properties.slice(0, 3).map((property, index) => (
              <Grid key={property.id || index} item xs={12} md={4}>
                <Box
                  sx={{
                    backgroundColor: 'rgba(255, 255, 255, 0.95)',
                    borderRadius: 2,
                    overflow: 'hidden',
                    transform: 'translateY(0)',
                    transition: 'transform 0.3s ease',
                    '&:hover': {
                      transform: 'translateY(-8px)',
                    },
                  }}
                >
                  <PropertyCard property={property} />
                </Box>
              </Grid>
            ))}
          </Grid>
        </Box>
      )}
      
      {/* زر الإجراء */}
      {config.ctaText && (
        <Box sx={{ textAlign: 'center' }}>
          <Button
            variant="contained"
            size="large"
            onClick={() => navigate('/offers/seasonal')}
            sx={{
              backgroundColor: 'white',
              color: 'primary.main',
              px: 6,
              py: 2,
              fontSize: '1.1rem',
              '&:hover': {
                backgroundColor: 'grey.100',
              },
            }}
          >
            {config.ctaText}
          </Button>
        </Box>
      )}
    </Paper>
  );
};

export default SeasonalOffer;
// frontend/src/components/Home/Sections/MultiPropertyAd.tsx
import React from 'react';
import { Box, Grid, Typography, Paper, Button } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import PropertyCard from '../Properties/PropertyCard';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';

interface MultiPropertyAdProps {
  section: DynamicHomeSection;
}

const MultiPropertyAd: React.FC<MultiPropertyAdProps> = ({ section }) => {
  const navigate = useNavigate();
  
  const properties = section.content
    .filter(item => item.contentType === 'PROPERTY')
    .map(item => item.data)
    .slice(0, 4); // حد أقصى 4 عقارات

  if (properties.length === 0) {
    return null;
  }

  const config = section.config || {};
  const styleSettings = config.styleSettings || {};

  return (
    <Paper
      elevation={3}
      sx={{
        p: 4,
        backgroundColor: styleSettings.backgroundColor || 'background.paper',
        borderRadius: styleSettings.borderRadius || 2,
      }}
    >
      {(section.title || section.subtitle) && (
        <Box sx={{ textAlign: 'center', mb: 4 }}>
          {section.title && (
            <Typography variant="h4" fontWeight="bold" gutterBottom>
              {section.title}
            </Typography>
          )}
          {section.subtitle && (
            <Typography variant="body1" color="text.secondary">
              {section.subtitle}
            </Typography>
          )}
        </Box>
      )}
      
      <Grid container spacing={2}>
        {properties.map((property, index) => (
          <Grid key={property.id || index} item xs={12} sm={6}>
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: index * 0.1 }}
            >
              <PropertyCard
                property={property}
                variant="vertical"
                showPrice={true}
                showRating={true}
              />
            </motion.div>
          </Grid>
        ))}
      </Grid>
      
      {config.ctaText && (
        <Box sx={{ mt: 4, textAlign: 'center' }}>
          <Button
            variant="contained"
            size="large"
            onClick={() => navigate('/properties')}
          >
            {config.ctaText}
          </Button>
        </Box>
      )}
    </Paper>
  );
};

export default MultiPropertyAd;
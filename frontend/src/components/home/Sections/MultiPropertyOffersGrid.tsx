// frontend/src/components/Home/Sections/MultiPropertyOffersGrid.tsx
import React from 'react';
import { Box, Grid, Typography, Paper } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import OfferCard from '../../Offers/OfferCard';
import SectionHeader from '../Common/SectionHeader';
import { motion } from 'framer-motion';

interface MultiPropertyOffersGridProps {
  section: DynamicHomeSection;
}

const MultiPropertyOffersGrid: React.FC<MultiPropertyOffersGridProps> = ({ section }) => {
  const offers = section.content
    .filter(item => item.contentType === 'OFFER' || item.contentType === 'PROPERTY')
    .map(item => item.data);

  if (offers.length === 0) {
    return null;
  }

  const config = section.config || {};
  const layoutSettings = config.layoutSettings || {};
  const columnsCount = layoutSettings.columnsCount || 3;

  return (
    <Box>
      <SectionHeader
        title={section.title}
        subtitle={section.subtitle}
        actionText="جميع العروض"
        onActionClick={() => {/* Navigate to offers */}}
      />
      
      <Grid container spacing={3} sx={{ mt: 1 }}>
        {offers.map((offer, index) => (
          <Grid key={offer.id || index} item xs={12} sm={6} md={12 / columnsCount}>
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: index * 0.1 }}
              style={{ height: '100%' }}
            >
              <OfferCard offer={offer} />
            </motion.div>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};

export default MultiPropertyOffersGrid;
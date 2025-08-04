// frontend/src/components/Home/Sections/CityCardsGrid.tsx
import React from 'react';
import { Box, Grid } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import CityCard from '../Destinations/CityCard';
import SectionHeader from '../Common/SectionHeader';

interface CityCardsGridProps {
  section: DynamicHomeSection;
}

const CityCardsGrid: React.FC<CityCardsGridProps> = ({ section }) => {
  // استخراج المدن من المحتوى
  const cities = section.content
    .filter(item => item.contentType === 'DESTINATION')
    .map(item => item.data);

  if (cities.length === 0) {
    return null;
  }

  const config = section.config || {};
  const layoutSettings = config.layoutSettings || {};
  const columnsCount = layoutSettings.columnsCount || 2;

  return (
    <Box>
      <SectionHeader
        title={section.title}
        subtitle={section.subtitle}
        actionText="جميع الوجهات"
        onActionClick={() => {/* Navigate to destinations */}}
      />
      
      <Grid container spacing={3} sx={{ mt: 1 }}>
        {cities.map((city, index) => (
          <Grid key={city.id || index} item xs={12} sm={6} md={12 / columnsCount}>
            <CityCard city={city} />
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};

export default CityCardsGrid;
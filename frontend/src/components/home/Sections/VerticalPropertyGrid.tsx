// frontend/src/components/Home/Sections/VerticalPropertyGrid.tsx
import React from 'react';
import { Box, Grid } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import PropertyCard from '../Properties/PropertyCard';
import SectionHeader from '../Common/SectionHeader';

interface VerticalPropertyGridProps {
  section: DynamicHomeSection;
}

const VerticalPropertyGrid: React.FC<VerticalPropertyGridProps> = ({ section }) => {
  // استخراج العقارات من المحتوى
  const properties = section.content
    .filter(item => item.contentType === 'PROPERTY')
    .map(item => item.data);

  if (properties.length === 0) {
    return null;
  }

  const config = section.config || {};
  const layoutSettings = config.layoutSettings || {};
  const displaySettings = config.displaySettings || {};
  
  const columnsCount = layoutSettings.columnsCount || 2;
  const spacing = layoutSettings.spacing || 2;

  return (
    <Box>
      <SectionHeader
        title={section.title}
        subtitle={section.subtitle}
        actionText={displaySettings.showViewAll ? 'عرض الكل' : undefined}
        onActionClick={() => {/* Navigate to all properties */}}
      />
      
      <Grid container spacing={spacing} sx={{ mt: 2 }}>
        {properties.map((property, index) => (
          <Grid key={property.id || index} item xs={12} sm={6} md={12 / columnsCount}>
            <PropertyCard
              property={property}
              showBadge={displaySettings.showBadge}
              showPrice={displaySettings.showPrice !== false}
              showRating={displaySettings.showRating !== false}
              variant="vertical"
            />
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};

export default VerticalPropertyGrid;
// frontend/src/components/Home/Sections/MixedLayoutList.tsx
import React from 'react';
import { Box, Grid } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import PropertyCard from '../../Properties/PropertyCard';
import SectionHeader from '../Common/SectionHeader';
import { motion } from 'framer-motion';

interface MixedLayoutListProps {
  section: DynamicHomeSection;
}

const MixedLayoutList: React.FC<MixedLayoutListProps> = ({ section }) => {
  const properties = section.content
    .filter(item => item.contentType === 'PROPERTY')
    .map(item => item.data);

  if (properties.length === 0) {
    return null;
  }

  return (
    <Box>
      <SectionHeader
        title={section.title}
        subtitle={section.subtitle}
      />
      
      <Grid container spacing={3} sx={{ mt: 1 }}>
        {/* العقار الأول - كبير */}
        {properties[0] && (
          <Grid item xs={12} md={8}>
            <motion.div
              initial={{ opacity: 0, x: -20 }}
              whileInView={{ opacity: 1, x: 0 }}
              viewport={{ once: true }}
            >
              <PropertyCard property={properties[0]} variant="horizontal" />
            </motion.div>
          </Grid>
        )}
        
        {/* العقارات التالية - صغيرة */}
        <Grid item xs={12} md={4}>
          <Grid container spacing={2}>
            {properties.slice(1, 3).map((property, index) => (
              <Grid key={property.id || index} item xs={12}>
                <motion.div
                  initial={{ opacity: 0, x: 20 }}
                  whileInView={{ opacity: 1, x: 0 }}
                  viewport={{ once: true }}
                  transition={{ delay: (index + 1) * 0.1 }}
                >
                  <PropertyCard property={property} variant="vertical" />
                </motion.div>
              </Grid>
            ))}
          </Grid>
        </Grid>
        
        {/* باقي العقارات - شبكة عادية */}
        {properties.slice(3).map((property, index) => (
          <Grid key={property.id || index} item xs={12} sm={6} md={4}>
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ delay: (index + 3) * 0.1 }}
            >
              <PropertyCard property={property} variant="vertical" />
            </motion.div>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};

export default MixedLayoutList;
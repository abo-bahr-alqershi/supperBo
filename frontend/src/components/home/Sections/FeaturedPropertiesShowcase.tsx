// frontend/src/components/Home/Sections/FeaturedPropertiesShowcase.tsx
import React from 'react';
import { Box, useTheme } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import SectionHeader from '../Common/SectionHeader';
import { Swiper, SwiperSlide } from 'swiper/react';
import { Navigation, Pagination, Autoplay, EffectCoverflow } from 'swiper/modules';
import PropertyShowcaseCard from './PropertyShowcaseCard';
import 'swiper/css/effect-coverflow';

interface FeaturedPropertiesShowcaseProps {
  section: DynamicHomeSection;
}

const FeaturedPropertiesShowcase: React.FC<FeaturedPropertiesShowcaseProps> = ({ section }) => {
  const theme = useTheme();
  
  const properties = section.content
    .filter(item => item.contentType === 'PROPERTY')
    .map(item => item.contentData);

  if (properties.length === 0) {
    return null;
  }

  const config = section.sectionConfig || {};
  const behaviorSettings = config.behaviorSettings || {};

  return (
    <Box sx={{ py: 4, backgroundColor: 'grey.50' }}>
      <SectionHeader
        title={section.title}
        subtitle={section.subtitle}
      />
      
      <Box sx={{ mt: 4, mx: -3 }}>
        <Swiper
          modules={[Navigation, Pagination, Autoplay, EffectCoverflow]}
          effect="coverflow"
          grabCursor={true}
          centeredSlides={true}
          slidesPerView={'auto'}
          coverflowEffect={{
            rotate: 50,
            stretch: 0,
            depth: 100,
            modifier: 1,
            slideShadows: true,
          }}
          navigation
          pagination={{ clickable: true }}
          autoplay={behaviorSettings.autoPlay ? {
            delay: 4000,
            disableOnInteraction: false,
          } : false}
          style={{ paddingBottom: '50px' }}
        >
          {properties.map((property, index) => (
            <SwiperSlide
              key={property.id || index}
              style={{ width: '600px', maxWidth: '90vw' }}
            >
              <PropertyShowcaseCard property={property} />
            </SwiperSlide>
          ))}
        </Swiper>
      </Box>
    </Box>
  );
};

export default FeaturedPropertiesShowcase;
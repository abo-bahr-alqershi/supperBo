// frontend/src/components/Home/Sections/DestinationCarousel.tsx
import React from 'react';
import { Box, useTheme, useMediaQuery } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import CityCard from '../Destinations/CityCard';
import SectionHeader from '../Common/SectionHeader';
import { Swiper, SwiperSlide } from 'swiper/react';
import { Navigation, Pagination, Autoplay } from 'swiper/modules';

interface DestinationCarouselProps {
  section: DynamicHomeSection;
}

const DestinationCarousel: React.FC<DestinationCarouselProps> = ({ section }) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const isTablet = useMediaQuery(theme.breakpoints.down('md'));

  const destinations = section.content
    .filter(item => item.contentType === 'DESTINATION')
    .map(item => item.data);

  if (destinations.length === 0) {
    return null;
  }

  const config = section.config || {};
  const behaviorSettings = config.behaviorSettings || {};
  const slidesPerView = isMobile ? 1 : isTablet ? 2 : 3;

  return (
    <Box>
      <SectionHeader
        title={section.title}
        subtitle={section.subtitle}
        actionText="جميع الوجهات"
        onActionClick={() => {/* Navigate to destinations */}}
      />
      
      <Box sx={{ mt: 3 }}>
        <Swiper
          modules={[Navigation, Pagination, Autoplay]}
          spaceBetween={20}
          slidesPerView={slidesPerView}
          navigation={!isMobile}
          pagination={{ clickable: true }}
          autoplay={behaviorSettings.autoPlay ? {
            delay: 5000,
            disableOnInteraction: false,
          } : false}
          loop={destinations.length > slidesPerView}
        >
          {destinations.map((destination, index) => (
            <SwiperSlide key={destination.id || index}>
              <CityCard city={destination} />
            </SwiperSlide>
          ))}
        </Swiper>
      </Box>
    </Box>
  );
};

export default DestinationCarousel;
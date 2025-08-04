// frontend/src/components/Home/Sections/HorizontalPropertyList.tsx
import React, { useRef } from 'react';
import { Box, Typography, IconButton, useTheme, useMediaQuery } from '@mui/material';
import { ChevronLeft, ChevronRight } from '@mui/icons-material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import PropertyCard from '../../Properties/PropertyCard';
import SectionHeader from '../Common/SectionHeader';
import { Swiper, SwiperSlide } from 'swiper/react';
import { Navigation, Pagination } from 'swiper/modules';
import 'swiper/css';
import 'swiper/css/navigation';
import 'swiper/css/pagination';

interface HorizontalPropertyListProps {
  section: DynamicHomeSection;
}

const HorizontalPropertyList: React.FC<HorizontalPropertyListProps> = ({ section }) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const isTablet = useMediaQuery(theme.breakpoints.down('md'));
  
  const navigationPrevRef = useRef(null);
  const navigationNextRef = useRef(null);

  // استخراج العقارات من المحتوى
  const properties = section.content
    .filter(item => item.contentType === 'PROPERTY')
    .map(item => item.data);

  if (properties.length === 0) {
    return null;
  }

  const config = section.config || {};
  const displaySettings = config.displaySettings || {};
  const layoutSettings = config.layoutSettings || {};

  const slidesPerView = isMobile ? 1.2 : isTablet ? 2.5 : 4;
  const spaceBetween = layoutSettings.itemSpacing || 16;

  return (
    <Box>
      <SectionHeader
        title={section.title}
        subtitle={section.subtitle}
        actionText={displaySettings.showViewAll ? 'عرض الكل' : undefined}
        onActionClick={() => {/* Navigate to all properties */}}
      />
      
      <Box sx={{ position: 'relative', mt: 3 }}>
        <Swiper
          modules={[Navigation, Pagination]}
          spaceBetween={spaceBetween}
          slidesPerView={slidesPerView}
          navigation={{
            prevEl: navigationPrevRef.current,
            nextEl: navigationNextRef.current,
          }}
          pagination={isMobile ? { clickable: true } : false}
          onBeforeInit={(swiper) => {
            swiper.params.navigation.prevEl = navigationPrevRef.current;
            swiper.params.navigation.nextEl = navigationNextRef.current;
          }}
          style={{ paddingBottom: isMobile ? '40px' : '0' }}
        >
          {properties.map((property, index) => (
            <SwiperSlide key={property.id || index}>
              <PropertyCard
                property={property}
                showBadge={displaySettings.showBadge}
                showPrice={displaySettings.showPrice !== false}
                showRating={displaySettings.showRating !== false}
                variant="horizontal"
              />
            </SwiperSlide>
          ))}
        </Swiper>

        {!isMobile && (
          <>
            <IconButton
              ref={navigationPrevRef}
              sx={{
                position: 'absolute',
                left: -24,
                top: '50%',
                transform: 'translateY(-50%)',
                backgroundColor: 'background.paper',
                boxShadow: 2,
                '&:hover': {
                  backgroundColor: 'background.paper',
                  boxShadow: 4,
                },
                zIndex: 10,
              }}
            >
              <ChevronRight />
            </IconButton>
            <IconButton
              ref={navigationNextRef}
              sx={{
                position: 'absolute',
                right: -24,
                top: '50%',
                transform: 'translateY(-50%)',
                backgroundColor: 'background.paper',
                boxShadow: 2,
                '&:hover': {
                  backgroundColor: 'background.paper',
                  boxShadow: 4,
                },
                zIndex: 10,
              }}
            >
              <ChevronLeft />
            </IconButton>
          </>
        )}
      </Box>
    </Box>
  );
};

export default HorizontalPropertyList;
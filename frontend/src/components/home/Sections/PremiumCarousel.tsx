// frontend/src/components/Home/Sections/PremiumCarousel.tsx
import React from 'react';
import { Box, Typography, IconButton } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import { ChevronLeft, ChevronRight } from '@mui/icons-material';
import { Swiper, SwiperSlide } from 'swiper/react';
import { Navigation, Pagination, Autoplay, EffectCreative } from 'swiper/modules';
import FeaturedPropertyAd from './FeaturedPropertyAd';
import 'swiper/css/effect-creative';

interface PremiumCarouselProps {
  section: DynamicHomeSection;
}

const PremiumCarousel: React.FC<PremiumCarouselProps> = ({ section }) => {
  const items = section.content;
  
  if (items.length === 0) {
    return null;
  }

  const config = section.sectionConfig || {};
  const behaviorSettings = config.behaviorSettings || {};

  return (
    <Box
      sx={{
        position: 'relative',
        backgroundColor: 'grey.900',
        py: 6,
        overflow: 'hidden',
      }}
    >
      {/* خلفية متحركة */}
      <Box
        sx={{
          position: 'absolute',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          opacity: 0.1,
          background: 'radial-gradient(circle at 20% 50%, #fff 0%, transparent 50%)',
          animation: 'moveBackground 20s ease-in-out infinite',
        }}
      />
      
      <Box sx={{ position: 'relative', zIndex: 1 }}>
        {(section.title || section.subtitle) && (
          <Box sx={{ textAlign: 'center', mb: 6 }}>
            {section.title && (
              <Typography variant="h3" fontWeight="bold" color="white" gutterBottom>
                {section.title}
              </Typography>
            )}
            {section.subtitle && (
              <Typography variant="h6" color="grey.300">
                {section.subtitle}
              </Typography>
            )}
          </Box>
        )}
        
        <Box sx={{ maxWidth: 1200, mx: 'auto', px: 3 }}>
          <Swiper
            modules={[Navigation, Pagination, Autoplay, EffectCreative]}
            effect="creative"
            creativeEffect={{
              prev: {
                shadow: true,
                translate: ['-20%', 0, -1],
              },
              next: {
                translate: ['100%', 0, 0],
              },
            }}
            navigation
            pagination={{ clickable: true }}
            autoplay={behaviorSettings.autoPlay ? {
              delay: 5000,
              disableOnInteraction: false,
            } : false}
            loop={items.length > 1}
            spaceBetween={50}
            style={{ paddingBottom: '60px' }}
          >
            {items.map((item, index) => (
              <SwiperSlide key={item.id || index}>
                <Box sx={{ px: 2 }}>
                  <FeaturedPropertyAd
                    section={{
                      ...section,
                      content: [item],
                    }}
                  />
                </Box>
              </SwiperSlide>
            ))}
          </Swiper>
        </Box>
      </Box>
      
      <style>
        {`
          @keyframes moveBackground {
            0% { transform: translateX(0) translateY(0); }
            25% { transform: translateX(10px) translateY(-10px); }
            50% { transform: translateX(0) translateY(-20px); }
            75% { transform: translateX(-10px) translateY(-10px); }
            100% { transform: translateX(0) translateY(0); }
          }
        `}
      </style>
    </Box>
  );
};

export default PremiumCarousel;
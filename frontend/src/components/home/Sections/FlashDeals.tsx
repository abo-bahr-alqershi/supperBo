// frontend/src/components/Home/Sections/FlashDeals.tsx
import React, { useState, useEffect } from 'react';
import { Box, Typography, Paper, Chip } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import PropertyCard from '../Properties/PropertyCard';
import SectionHeader from '../Common/SectionHeader';
import { Swiper, SwiperSlide } from 'swiper/react';
import { Navigation, Autoplay } from 'swiper/modules';
import { FlashOn, Timer } from '@mui/icons-material';
import CountdownTimer from '../Common/CountdownTimer';

interface FlashDealsProps {
  section: DynamicHomeSection;
}

const FlashDeals: React.FC<FlashDealsProps> = ({ section }) => {
  const deals = section.content
    .filter(item => item.contentType === 'PROPERTY' || item.contentType === 'OFFER')
    .map(item => item.contentData);

  if (deals.length === 0) {
    return null;
  }

  // حساب وقت انتهاء العروض السريعة
  const flashEndTime = section.expiresAt || 
    new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(); // 24 ساعة افتراضياً

  return (
    <Paper
      elevation={3}
      sx={{
        p: 3,
        background: 'linear-gradient(135deg, #ff6b6b 0%, #ee5a24 100%)',
        color: 'white',
        position: 'relative',
        overflow: 'hidden',
      }}
    >
      {/* خلفية متحركة */}
      <Box
        sx={{
          position: 'absolute',
          top: -50,
          right: -50,
          width: 200,
          height: 200,
          borderRadius: '50%',
          backgroundColor: 'rgba(255, 255, 255, 0.1)',
          animation: 'pulse 2s infinite',
        }}
      />
      
      <Box sx={{ position: 'relative', zIndex: 1 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <FlashOn sx={{ fontSize: 40 }} />
            <Box>
              <Typography variant="h4" fontWeight="bold">
                {section.title || 'عروض سريعة'}
              </Typography>
              {section.subtitle && (
                <Typography variant="body1">
                  {section.subtitle}
                </Typography>
              )}
            </Box>
          </Box>
          
          <Box>
            <Typography variant="body2" sx={{ mb: 1, textAlign: 'center' }}>
              ينتهي العرض خلال:
            </Typography>
            <CountdownTimer endDate={flashEndTime} />
          </Box>
        </Box>
        
        <Swiper
          modules={[Navigation, Autoplay]}
          spaceBetween={20}
          slidesPerView={1}
          breakpoints={{
            640: { slidesPerView: 2 },
            768: { slidesPerView: 3 },
            1024: { slidesPerView: 4 },
          }}
          navigation
          autoplay={{
            delay: 3000,
            disableOnInteraction: false,
          }}
        >
          {deals.map((deal, index) => (
            <SwiperSlide key={deal.id || index}>
              <Box sx={{ position: 'relative' }}>
                <PropertyCard
                  property={deal}
                  showBadge={false}
                />
                <Chip
                  label="عرض سريع"
                  size="small"
                  sx={{
                    position: 'absolute',
                    top: 8,
                    left: 8,
                    backgroundColor: 'warning.main',
                    color: 'white',
                    fontWeight: 'bold',
                    animation: 'pulse 1.5s infinite',
                  }}
                />
              </Box>
            </SwiperSlide>
          ))}
        </Swiper>
      </Box>
      
      <style>
        {`
          @keyframes pulse {
            0% {
              transform: scale(1);
              opacity: 1;
            }
            50% {
              transform: scale(1.05);
              opacity: 0.8;
            }
            100% {
              transform: scale(1);
              opacity: 1;
            }
          }
        `}
      </style>
    </Paper>
  );
};

export default FlashDeals;
// frontend/src/components/Home/Sections/OffersCarousel.tsx
import React from 'react';
import { Box } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import OfferCard from '../Offers/OfferCard';
import SectionHeader from '../Common/SectionHeader';
import { Swiper, SwiperSlide } from 'swiper/react';
import { Autoplay, Pagination, EffectFade } from 'swiper/modules';

interface OffersCarouselProps {
  section: DynamicHomeSection;
}

const OffersCarousel: React.FC<OffersCarouselProps> = ({ section }) => {
  // استخراج العروض من المحتوى
  const offers = section.content
    .filter(item => item.contentType === 'OFFER' || item.contentType === 'ADVERTISEMENT')
    .map(item => item.data);

  if (offers.length === 0) {
    return null;
  }

  const config = section.config || {};
  const behaviorSettings = config.behaviorSettings || {};
  const autoPlay = behaviorSettings.autoPlay !== false;
  const autoPlayDelay = behaviorSettings.autoPlayDelay || 5000;

  return (
    <Box>
      <SectionHeader
        title={section.title}
        subtitle={section.subtitle}
      />
      
      <Box sx={{ mt: 3 }}>
        <Swiper
          modules={[Autoplay, Pagination, EffectFade]}
          spaceBetween={20}
          slidesPerView={1}
          effect="fade"
          autoplay={autoPlay ? {
            delay: autoPlayDelay,
            disableOnInteraction: false,
          } : false}
          pagination={{ clickable: true }}
          style={{ paddingBottom: '40px' }}
        >
          {offers.map((offer, index) => (
            <SwiperSlide key={offer.id || index}>
              <OfferCard offer={offer} fullWidth />
            </SwiperSlide>
          ))}
        </Swiper>
      </Box>
    </Box>
  );
};

export default OffersCarousel;
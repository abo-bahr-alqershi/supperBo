// frontend/src/components/Home/DynamicSection.tsx
import React from 'react';
import { Box } from '@mui/material';
import type { DynamicHomeSection, DynamicHomeConfig } from '../../types/homeSections.types';
import { SectionType } from '../../types/enums';

// استيراد جميع مكونات الأقسام
import HorizontalPropertyList from './Sections/HorizontalPropertyList';
import VerticalPropertyGrid from './Sections/VerticalPropertyGrid';
import FeaturedPropertyAd from './Sections/FeaturedPropertyAd';
import OffersCarousel from './Sections/OffersCarousel';
import CityCardsGrid from './Sections/CityCardsGrid';
import DestinationCarousel from './Sections/DestinationCarousel';
import FeaturedPropertiesShowcase from './Sections/FeaturedPropertiesShowcase';
import FlashDeals from './Sections/FlashDeals';
import MultiPropertyAd from './Sections/MultiPropertyAd';
import SinglePropertyOffer from './Sections/SinglePropertyOffer';
import LimitedTimeOffer from './Sections/LimitedTimeOffer';
import SeasonalOffer from './Sections/SeasonalOffer';
import MultiPropertyOffersGrid from './Sections/MultiPropertyOffersGrid';
import ExploreCities from './Sections/ExploreCities';
import PremiumCarousel from './Sections/PremiumCarousel';
import InteractiveShowcase from './Sections/InteractiveShowcase';

interface DynamicSectionProps {
  section: DynamicHomeSection;
  config: DynamicHomeConfig;
}

const DynamicSection: React.FC<DynamicSectionProps> = ({ section, config }) => {
  // التحقق من أن القسم نشط
  if (!section.isActive) {
    return null;
  }

  // التحقق من تاريخ البدء والانتهاء
  const now = new Date();
  if (section.scheduledAt && new Date(section.scheduledAt) > now) {
    return null;
  }
  if (section.expiresAt && new Date(section.expiresAt) < now) {
    return null;
  }

  // عرض المكون المناسب بناءً على نوع القسم
  const renderSection = () => {
    switch (section.sectionType) {
      case SectionType.HORIZONTAL_PROPERTY_LIST:
        return <HorizontalPropertyList section={section} />;
      
      case SectionType.VERTICAL_PROPERTY_GRID:
        return <VerticalPropertyGrid section={section} />;
      
      case SectionType.SINGLE_PROPERTY_AD:
      case SectionType.FEATURED_PROPERTY_AD:
        return <FeaturedPropertyAd section={section} />;
      
      case SectionType.MULTI_PROPERTY_AD:
        return <MultiPropertyAd section={section} />;
      
      case SectionType.OFFERS_CAROUSEL:
        return <OffersCarousel section={section} />;
      
      case SectionType.CITY_CARDS_GRID:
        return <CityCardsGrid section={section} />;
      
      case SectionType.DESTINATION_CAROUSEL:
        return <DestinationCarousel section={section} />;
      
      case SectionType.FEATURED_PROPERTIES_SHOWCASE:
        return <FeaturedPropertiesShowcase section={section} />;
      
      case SectionType.FLASH_DEALS:
        return <FlashDeals section={section} />;
      
      case SectionType.SINGLE_PROPERTY_OFFER:
        return <SinglePropertyOffer section={section} />;
      
      case SectionType.LIMITED_TIME_OFFER:
        return <LimitedTimeOffer section={section} />;
      
      case SectionType.SEASONAL_OFFER:
        return <SeasonalOffer section={section} />;
      
      case SectionType.MULTI_PROPERTY_OFFERS_GRID:
        return <MultiPropertyOffersGrid section={section} />;
      
      case SectionType.EXPLORE_CITIES:
        return <ExploreCities section={section} />;
      
      case SectionType.PREMIUM_CAROUSEL:
        return <PremiumCarousel section={section} />;
      
      case SectionType.INTERACTIVE_SHOWCASE:
        return <InteractiveShowcase section={section} />;
      
      default:
        console.warn(`نوع القسم غير مدعوم: ${section.sectionType}`);
        return null;
    }
  };

  return (
    <Box
      id={`section-${section.id}`}
      data-section-type={section.sectionType}
      data-section-priority={section.priority}
    >
      {renderSection()}
    </Box>
  );
};

export default DynamicSection;
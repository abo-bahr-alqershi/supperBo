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
import UnitShowcaseAd from './Sections/UnitShowcaseAd';

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

  // تصفية وفرز عناصر المحتوى حسب الصلاحية وترتيب العرض
  const filteredContent = (section.content || [])
    .filter((c) => (c.isActive ?? true) && (!c.expiresAt || new Date(c.expiresAt) >= now))
    .sort((a, b) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0));

  if (filteredContent.length === 0) {
    return null;
  }

  const effectiveSection: DynamicHomeSection = { ...section, content: filteredContent };

  // عرض المكون المناسب بناءً على نوع القسم
  const renderSection = () => {
    switch (effectiveSection.sectionType) {
      case SectionType.HORIZONTAL_PROPERTY_LIST:
        return <HorizontalPropertyList section={effectiveSection} />;
      
      case SectionType.VERTICAL_PROPERTY_GRID:
        return <VerticalPropertyGrid section={effectiveSection} />;
      
      case SectionType.SINGLE_PROPERTY_AD:
      case SectionType.FEATURED_PROPERTY_AD:
        return <FeaturedPropertyAd section={effectiveSection} />;
      
      case SectionType.MULTI_PROPERTY_AD:
        return <MultiPropertyAd section={effectiveSection} />;

      case SectionType.UNIT_SHOWCASE_AD:
        return <UnitShowcaseAd section={effectiveSection} />;
      
      case SectionType.OFFERS_CAROUSEL:
        return <OffersCarousel section={effectiveSection} />;
      
      case SectionType.CITY_CARDS_GRID:
        return <CityCardsGrid section={effectiveSection} />;
      
      case SectionType.DESTINATION_CAROUSEL:
        return <DestinationCarousel section={effectiveSection} />;
      
      case SectionType.FEATURED_PROPERTIES_SHOWCASE:
        return <FeaturedPropertiesShowcase section={effectiveSection} />;
      
      case SectionType.FLASH_DEALS:
        return <FlashDeals section={effectiveSection} />;
      
      case SectionType.SINGLE_PROPERTY_OFFER:
        return <SinglePropertyOffer section={effectiveSection} />;
      
      case SectionType.LIMITED_TIME_OFFER:
        return <LimitedTimeOffer section={effectiveSection} />;
      
      case SectionType.SEASONAL_OFFER:
        return <SeasonalOffer section={effectiveSection} />;
      
      case SectionType.MULTI_PROPERTY_OFFERS_GRID:
        return <MultiPropertyOffersGrid section={effectiveSection} />;
      
      case SectionType.EXPLORE_CITIES:
        return <ExploreCities section={effectiveSection} />;
      
      case SectionType.PREMIUM_CAROUSEL:
        return <PremiumCarousel section={effectiveSection} />;
      
      case SectionType.INTERACTIVE_SHOWCASE:
        return <InteractiveShowcase section={effectiveSection} />;
      
      default:
        return null;
    }
  };

  return (
    <Box
      id={`section-${effectiveSection.id}`}
      data-section-type={effectiveSection.sectionType}
      data-section-priority={effectiveSection.priority}
    >
      {renderSection()}
    </Box>
  );
};

export default DynamicSection;
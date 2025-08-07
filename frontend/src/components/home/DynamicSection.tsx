// frontend/src/components/home/DynamicSection.tsx
import React, { memo } from 'react';
import { Box, Typography, Grid, Button, Card, CardMedia, CardContent } from '@mui/material';
import type { DynamicHomeSection, DynamicHomeConfig, DynamicContent } from '../../types/homeSections.types';
import { SectionType } from '../../types/enums';

interface DynamicSectionProps {
  section: DynamicHomeSection;
  config: DynamicHomeConfig;
}

// Lightweight primitives
type SectionContainerProps = {
  title?: string;
  subtitle?: string;
  rightAction?: React.ReactNode;
  children?: React.ReactNode;
};

const SectionContainer: React.FC<SectionContainerProps>
  = memo(({ title, subtitle, rightAction, children }) => (
  <Box sx={{ mb: 3 }}>
    {(title || subtitle || rightAction) && (
      <Box sx={{ display: 'flex', alignItems: 'flex-end', justifyContent: 'space-between', mb: 2 }}>
        <Box>
          {title && (
            <Typography variant="h5" fontWeight={700} sx={{ mb: 0.5 }}>{title}</Typography>
          )}
          {subtitle && (
            <Typography variant="body2" color="text.secondary">{subtitle}</Typography>
          )}
        </Box>
        {rightAction}
      </Box>
    )}
    {children}
  </Box>
));

const PropertyCardCompact: React.FC<{ data: any }>= memo(({ data }) => (
  <Card sx={{ height: '100%' }}>
    <CardMedia component="img" height={160} image={data.mainImageUrl || data.images?.[0]} alt={data.name} />
    <CardContent sx={{ py: 1.5 }}>
      <Typography variant="subtitle1" noWrap fontWeight={600}>{data.name}</Typography>
      {data.city && (
        <Typography variant="caption" color="text.secondary">{data.city}</Typography>
      )}
      {data.basePrice && (
        <Typography variant="subtitle2" color="primary" fontWeight={700} sx={{ mt: 0.5 }}>
          {data.basePrice.toLocaleString()} {data.currency || 'ريال'}
        </Typography>
      )}
    </CardContent>
  </Card>
));

const CityCardCompact: React.FC<{ data: any }>= memo(({ data }) => (
  <Card sx={{ height: '100%' }}>
    <CardMedia component="img" height={140} image={data.imageUrl} alt={data.nameAr || data.name} />
    <CardContent sx={{ py: 1.5 }}>
      <Typography variant="subtitle1" noWrap fontWeight={600}>{data.nameAr || data.name}</Typography>
      {data.countryAr && (
        <Typography variant="caption" color="text.secondary">{data.countryAr}</Typography>
      )}
    </CardContent>
  </Card>
));

const byOrder = (a: DynamicContent, b: DynamicContent) => (a.displayOrder ?? 0) - (b.displayOrder ?? 0);
const isActiveAndValid = (c: DynamicContent) => (c.isActive ?? true) && (!c.expiresAt || new Date(c.expiresAt) >= new Date());

const DynamicSection: React.FC<DynamicSectionProps> = memo(({ section, config }) => {
  // Visibility checks
  if (!section.isActive) return null;
  const now = new Date();
  if (section.scheduledAt && new Date(section.scheduledAt) > now) return null;
  if (section.expiresAt && new Date(section.expiresAt) < now) return null;

  // Normalize content
  const content = (section.content || []).filter(isActiveAndValid).sort(byOrder);
  if (content.length === 0) return null;

  const layout = section.sectionConfig?.layoutSettings || {};
  const display = section.sectionConfig?.displaySettings || {};
  const columns = layout.columnsCount || 2;

  const rightAction = display.showViewAllButton
    ? (<Button size="small" variant="text">عرض الكل</Button>)
    : undefined;

  const renderGrid = (items: any[], renderItem: (d: any) => React.ReactNode) => (
    <Grid container spacing={layout.itemSpacing ?? 2}>
      {items.map((d, idx) => (
        <Grid key={d.id || idx} item xs={12} sm={6} md={12 / columns}>
          {renderItem(d)}
        </Grid>
      ))}
    </Grid>
  );

  switch (section.sectionType) {
    case SectionType.HORIZONTAL_PROPERTY_LIST:
    case SectionType.VERTICAL_PROPERTY_GRID:
    case SectionType.FEATURED_PROPERTIES_SHOWCASE:
    case SectionType.MULTI_PROPERTY_AD: {
      const properties = content.filter((c) => c.contentType === 'PROPERTY').map((c) => c.contentData);
      if (properties.length === 0) return null;
      return (
        <SectionContainer title={section.title} subtitle={section.subtitle} rightAction={rightAction}>
          {renderGrid(properties, (d) => <PropertyCardCompact data={d} />)}
        </SectionContainer>
      );
    }
    case SectionType.CITY_CARDS_GRID:
    case SectionType.DESTINATION_CAROUSEL:
    case SectionType.EXPLORE_CITIES: {
      const cities = content.filter((c) => c.contentType === 'DESTINATION').map((c) => c.contentData);
      if (cities.length === 0) return null;
      return (
        <SectionContainer title={section.title} subtitle={section.subtitle} rightAction={rightAction}>
          {renderGrid(cities, (d) => <CityCardCompact data={d} />)}
        </SectionContainer>
      );
    }
    case SectionType.SINGLE_PROPERTY_AD:
    case SectionType.FEATURED_PROPERTY_AD:
    case SectionType.SINGLE_PROPERTY_OFFER:
    case SectionType.LIMITED_TIME_OFFER:
    case SectionType.SEASONAL_OFFER:
    case SectionType.MULTI_PROPERTY_OFFERS_GRID:
    case SectionType.OFFERS_CAROUSEL:
    case SectionType.FLASH_DEALS:
    case SectionType.PREMIUM_CAROUSEL:
    case SectionType.INTERACTIVE_SHOWCASE:
    default: {
      // Fallback to simple property grid if properties exist, else cities
      const properties = content.filter((c) => c.contentType === 'PROPERTY').map((c) => c.contentData);
      if (properties.length > 0) {
        return (
          <SectionContainer title={section.title} subtitle={section.subtitle} rightAction={rightAction}>
            {renderGrid(properties, (d) => <PropertyCardCompact data={d} />)}
          </SectionContainer>
        );
      }
      const cities = content.filter((c) => c.contentType === 'DESTINATION').map((c) => c.contentData);
      if (cities.length > 0) {
        return (
          <SectionContainer title={section.title} subtitle={section.subtitle} rightAction={rightAction}>
            {renderGrid(cities, (d) => <CityCardCompact data={d} />)}
          </SectionContainer>
        );
      }
      return null;
    }
  }
});

export default DynamicSection;
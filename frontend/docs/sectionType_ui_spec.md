# SectionType UI Specification

This document details each `SectionType` from the mobile app, its data shape (`DynamicHomeSection` + `DynamicContent`), default config keys, content fields, and recommended front-end management UI (pages/components/forms).

---

## Group A: Sponsored Ads

### 1. singlePropertyAd
- Value: `"SINGLE_PROPERTY_AD"`  (Category: Sponsored)
- **Config keys**: `maxItems` (1), `autoPlay` (false), `parallaxEnabled` (true), `animationType` (`"parallax"`)
- **Content item**: one `DynamicContent` with
  - `contentType`: `ADVERTISEMENT`
  - `data`: `{ propertyId: string; ctaText: string; ctaAction: string; imageUrl?: string }`
- **UI Form**:
  - Property selector dropdown
  - CTA text input
  - CTA action input (e.g. URL)
  - Image upload
  - Config panel: Parallax toggle, animation type select

### 2. featuredPropertyAd
- Value: `"FEATURED_PROPERTY_AD"`
- **Config keys**: `showBadge` (true), `badgeText` (`"FEATURED"`), `animationType` (`"scale"`)
- **Content**: same shape as singlePropertyAd
- **UI Form**:
  - Same as singlePropertyAd
  - Badge text input
  - Show badge toggle
  - Animation type select

### 3. multiPropertyAd
- Value: `"MULTI_PROPERTY_AD"`
- **Config keys**: `maxItems` (4), `autoPlay` (true), `autoPlayDuration` (number), `layoutType` (`"grid"`)
- **Content**: multiple items, each with `PROPERTY` data
- **UI Form**:
  - Multi-select property picker
  - Auto-play toggle + duration slider
  - Layout type selector (grid vs list)

### 4. unitShowcaseAd
- Value: `"UNIT_SHOWCASE_AD"`
- **Config keys**: default same as multiPropertyAd
- **Content**: `data` includes `{ propertyId: string; unitId: string; details: any }`
- **UI Form**:
  - Property > Unit dropdown cascader
  - Same config as multiPropertyAd

---
## Group B: Special Offers

### 5. singlePropertyOffer
- Value: `"SINGLE_PROPERTY_OFFER"`
- **Config**: default
- **Content**: `data` = `{ propertyId, discountPercentage, ctaText, ctaAction }`
- **UI**: property picker, discount input, CTA inputs

### 6. limitedTimeOffer
- Value: `"LIMITED_TIME_OFFER"`  *(isTimeSensitive=true)*
- **Config**: default + `{ showCountdown: true }`
- **Content**: same as singlePropertyOffer
- **UI**: schedule (start/end), countdown toggle, discount, CTA

### 7. seasonalOffer
- Value: `"SEASONAL_OFFER"`
- **Config**: default
- **Content**: `{ propertyIds[], seasonLabel, discount }`
- **UI**: multi-property, season label input, discount

### 8. multiPropertyOffersGrid
- Value: `"MULTI_PROPERTY_OFFERS_GRID"`
- **UI**: grid settings (columns), items per page

### 9. offersCarousel
- Value: `"OFFERS_CAROUSEL"`
- **Config**: `autoPlay`, `autoPlayDuration`, `showIndicators`
- **UI**: indicator toggle, autoplay controls

### 10. flashDeals
- Value: `"FLASH_DEALS"`
- **Config**: `{ showCountdown, animationType: 'pulse' }`
- **UI**: countdown, pulse animation toggle

---
## Group C: Property Listings

### 11. horizontalPropertyList
- Value: `"HORIZONTAL_PROPERTY_LIST"`
- **Config**: `maxItems`, `layoutType: 'horizontal'`
- **Content**: multiple `{ property data }`
- **UI**: items per view, scroll behavior

### 12. verticalPropertyGrid
- Value: `"VERTICAL_PROPERTY_GRID"`
- **UI**: columns count, spacing

### 13. mixedLayoutList
- Value: `"MIXED_LAYOUT_LIST"`
- **UI**: toggle mixed styles per item

### 14. compactPropertyList
- Value: `"COMPACT_PROPERTY_LIST"`
- **UI**: compact mode toggle

### 15. featuredPropertiesShowcase
- Value: `"FEATURED_PROPERTIES_SHOWCASE"`
- **UI**: badge text, highlight toggle

---
## Group D: Destinations

### 16. cityCardsGrid
- Value: `"CITY_CARDS_GRID"`
- **Config**: `columnsCount`, `cardHeight`
- **Content**: CityDestination data
- **UI**: columns selector, card style editor

### 17. destinationCarousel
- Value: `"DESTINATION_CAROUSEL"`
- **Config**: `autoPlay`, `indicators`
- **UI**: carousel controls

### 18. exploreCities
- Value: `"EXPLORE_CITIES"`
- **Content**: enriched city data
- **UI**: map vs grid toggle

---
## Group E: Premium Carousels

### 19. premiumCarousel
- Value: `"PREMIUM_CAROUSEL"`
- **Config**: high priority settings
- **UI**: styling editor (shadow, border)

### 20. interactiveShowcase
- Value: `"INTERACTIVE_SHOWCASE"`
- **UI**: drag-to-interact, custom JS

---

### Global UI Components

- **SectionListPage**: table or DnD canvas of sections
- **SectionForm**: dynamic form generator based on `type` schema
- **ConfigPage**: JSON editor with live preview
- **DestinationsPage**: CRUD table + stats modal
- **AdsPage**: preview cards + stats

**Note**: Each `SectionForm` should:
1. Load defaultConfig from enum
2. Render common fields (id, order, schedule)
3. Render `content` editor per `contentType` schema
4. Render `config` panel (form fields matching defaultConfig keys)

Use this spec as blueprint for building detailed pages and reusable components for each `SectionType`.
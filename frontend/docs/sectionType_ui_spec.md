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

---

## Detailed Data Model Reference

Below is a field-by-field breakdown of the core interfaces in `homeSections.types.ts`.

### DynamicHomeSection
```ts
interface DynamicHomeSection {
  id: string;            // section UUID
  type: string;          // one of SectionType values (e.g. 'SINGLE_PROPERTY_AD')
  order: number;         // display order index
  isActive: boolean;     // toggles visibility
  title?: string;        // localized title
  subtitle?: string;     // localized subtitle
  config: Record<string, any>;        // JSON object for layout/style/behavior settings
  metadata: Record<string, any>;      // analytics/tags
  scheduledAt?: string;  // ISO8601 show start
  expiresAt?: string;    // ISO8601 show end
  targetAudience: string[];          // e.g. ['Guest', 'User']
  priority: number;      // tie-breaker ordering
  content: DynamicContent[];         // list of content items
  createdAt: string;     // ISO8601 created timestamp
  updatedAt: string;     // ISO8601 last updated
}
```

### DynamicContent
```ts
interface DynamicContent {
  id: string;            // content UUID
  sectionId: string;     // back-reference to DynamicHomeSection.id
  contentType: string;   // e.g. 'PROPERTY', 'ADVERTISEMENT', 'DESTINATION'
  data: Record<string, any>;       // payload used by renderComponent()
  metadata: Record<string, any>;   // e.g. { priority: 1, tags: [...] }
  expiresAt?: string;    // optional content expiry
  createdAt: string;
  updatedAt: string;
}
```

### DynamicHomeConfig
```ts
interface DynamicHomeConfig {
  id: string;
  version: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  publishedAt?: string;
  globalSettings: Record<string, any>;
  themeSettings: Record<string, any>;
  layoutSettings: Record<string, any>;
  cacheSettings: Record<string, any>;
  analyticsSettings: Record<string, any>;
  enabledFeatures: string[];
  experimentalFeatures: Record<string, any>;
}
```

### CityDestination
```ts
interface CityDestination {
  id: string;
  name: string;
  nameAr: string;
  country: string;
  countryAr: string;
  description?: string;
  descriptionAr?: string;
  imageUrl: string;
  additionalImages: string[];
  latitude: number;
  longitude: number;
  propertyCount: number;
  averagePrice: number;
  currency: string;
  averageRating: number;
  reviewCount: number;
  isPopular: boolean;
  isFeatured: boolean;
  priority: number;
  highlights: string[];
  highlightsAr: string[];
  weatherData: Record<string, any>;
  attractionsData: Record<string, any>;
  metadata: Record<string, any>;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
  localizedFullName: string;
}
```

### SponsoredAd
```ts
interface SponsoredAd {
  id: string;
  title: string;
  subtitle?: string;
  description?: string;
  property?: PropertySummary;    // first property summary when includePropertyDetails = true
  propertyIds: string[];
  customImageUrl?: string;
  backgroundColor?: string;
  textColor?: string;
  styling: Record<string, any>;
  ctaText: string;
  ctaAction: string;
  ctaData: Record<string, any>;
  startDate: string;
  endDate: string;
  priority: number;
  targetingData: Record<string, any>;
  analyticsData: Record<string, any>;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  impressionCount: number;
  clickCount: number;
  conversionRate: number;
}
```

## Existing Front-End Components

Below is a list of reusable React components in `src/components` that you can leverage when building pages/forms for each SectionType.

### DynamicComponents
- **`Carousel.tsx`**: Generic carousel wrapper—supports `items: ComponentPreview[]`, autoplay, indicators.
- **`CategoryGrid.tsx`**: Grid layout for category cards—accepts `items: { id,name,imageUrl }[]`, `columns` prop.
- **`FilterBar.tsx`**: Provides search & multi-select filters—useful for searching properties or destinations.
- **`ImageGallery.tsx`**: Gallery view—supports lightbox, thumbnails.
- **`MapView.tsx`**: Interactive map—accepts `markers: { lat,lng }[]` for city destinations.
- **`OfferCard.tsx`**: Card UI for offers & flash deals—displays discount, countdown timer.
- **`PropertyList.tsx`**: List or grid of properties—pagination, sorting.
- **`SearchBar.tsx`**: Text search input—use for property search when adding content.
- **`TextBlock.tsx`**: Rich text renderer—use for announcement or banner sections.
- **`Banner.tsx`**: Single-image banner with overlay text.

### HomeScreenBuilder (Admin)
- **`Canvas.tsx`**: Drag-and-drop canvas for arranging sections & components.
- **`ComponentPalette.tsx`**: Sidebar with available components grouped by category.
- **`ComponentWrapper.tsx`**: Draggable wrapper that renders each component in builder.
- **`SectionContainer.tsx`**: Wrapper for each section—supports collapse/expand, reorder drag handle.
- **`PropertyPanel.tsx`**: Dynamic form panel—renders fields for section/config/content editing.
- **`PreviewModal.tsx`**: Displays live preview (via `usePreview`) inside a modal viewport.
- **`TemplateManager.tsx`**: Manages templates list, create/update/delete operations.
- **`index.tsx`**: Entry point wire-up of all builder subcomponents.

Use the above components as building blocks by passing in the data shapes defined in the Data Model Reference. For each SectionType:
1. Choose the right DynamicComponents sub-component (e.g., `Carousel` for carousel types).
2. In the Admin side, wrap with drag-and-drop (`Canvas`, `SectionContainer`) and use `PropertyPanel` to edit section-specific fields.
3. In the Client-side preview, map `DynamicContent` items to `ComponentPreview` via `renderComponent()` util.

With these models, endpoints, and existing components documented, the agent has full context to generate pages and forms accurately for each SectionType and workflow.
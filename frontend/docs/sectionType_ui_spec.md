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
-
- **`SectionListPage`**: table or DnD canvas of sections
- **`SectionForm`**: dynamic form generator based on `type` schema
- **`ConfigPage`**: JSON editor with live preview
- **`DestinationsPage`**: CRUD table + stats modal
- **`AdsPage`**: preview cards + stats
-
- **Note**: Each `SectionForm` should:
- 1. Load defaultConfig from enum
- 2. Render common fields (id, order, schedule)
- 3. Render `content` editor per `contentType` schema
- 4. Render `config` panel (form fields matching defaultConfig keys)

## Suggested UI Pages & Components
The following pages and components should be generated to manage Home Sections:

### Pages
- **DynamicSectionsPage**: lists all dynamic sections, supports filtering, search, pagination
- **DynamicSectionFormPage**: create/edit form for a single dynamic section with section-specific inputs and JSON config panel
- **HomeConfigPage**: lists config versions, allows create/edit and publish
- **CityDestinationsPage**: CRUD table of city destinations with inline stats editor modal
- **SponsoredAdsPage**: CRUD list of sponsored ads with card previews and interaction stats

### Reusable Components
For each SectionType, create a dedicated form component to handle its unique fields and content:
- **SinglePropertyAdForm**, **FeaturedPropertyAdForm**, **MultiPropertyAdForm**, **UnitShowcaseAdForm**
- **SinglePropertyOfferForm**, **LimitedTimeOfferForm**, **SeasonalOfferForm**, **FlashDealsForm**
- **HorizontalPropertyListForm**, **VerticalPropertyGridForm**, **MixedLayoutListForm**, etc.
- **CityCardsGridForm**, **DestinationCarouselForm**, **ExploreCitiesForm**
- **PremiumCarouselForm**, **InteractiveShowcaseForm**

Each form component should:
1. Load its defaultConfig from `SectionType.defaultConfig`
2. Expose inputs for all `DynamicHomeSection` fields (order, schedule, title, audience, etc.)
3. Provide controls to add/remove `DynamicContent` items, with nested editors based on `contentType`
4. Include a JSON viewer/editor for advanced `config` and `metadata` fields
5. Validate inputs according to the data model (e.g. date ranges, required fields)
6. On submission, call the corresponding service hook (e.g. `useCreateDynamicSection`, `useUpdateDynamicSection`)

Use this updated spec as the foundation for the AI agent to generate robust, type-safe management pages and components, ensuring 100% compatibility with the back-end contract.

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
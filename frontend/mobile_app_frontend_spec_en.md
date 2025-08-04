# Yemen Booking Mobile App: Dynamic Home Screen Frontend Specification

## 1. Introduction

This document provides a comprehensive technical specification for building the user interface (UI) for the Yemen Booking mobile application's dynamic home screen. The home screen is not static; it is rendered dynamically based on a configuration and a series of "sections" fetched from the backend API.

The purpose of this document is to serve as the single source of truth for frontend developers. It details the data structures, entity models, enumerations, and rendering logic the mobile application expects. Adhering to this specification will ensure full compatibility with the backend services and the mobile application's architecture.

## 2. Core Concepts

The home screen's structure is defined by three main components:

1.  **Home Configuration (`HomeConfig`)**: A global object that defines the overall behavior, theme, and settings for the home screen (e.g., refresh intervals, caching rules, global styles).
2.  **Home Sections (`HomeSection`)**: An ordered list of content blocks that make up the home screen. Each section has a specific `type` (e.g., horizontal list of properties, grid of cities, a featured ad) and its own configuration.
3.  **Dynamic Content (`DynamicContent`)**: The actual data items displayed within a section (e.g., a specific property, an offer, a city destination).

The rendering flow is as follows:
1.  The mobile app first fetches the `HomeConfig`.
2.  It then fetches the list of `HomeSection` objects.
3.  The app iterates through the `HomeSection` list and, based on the `type` of each section, renders the appropriate UI component.
4.  Each UI component is populated with data from the `content` array within its corresponding `HomeSection`.

---

## 3. Data Model & Entity Specification

This section details every data model the mobile application receives from the backend.

### 3.1. `HomeConfig` Entity

This entity defines the global settings for the home screen.

| Property | Type | Description | Example |
| :--- | :--- | :--- | :--- |
| `id` | `String` | Unique identifier for the configuration. | `"config_v1.2"` |
| `version` | `String` | The version string for this configuration. | `"1.2.0"` |
| `isActive` | `bool` | Whether this configuration is currently active. | `true` |
| `publishedAt` | `DateTime?` | The timestamp when this config was made live. | `"2025-08-04T10:00:00Z"` |
| `globalSettings`| `Map<String, dynamic>` | Key-value pairs for global app behavior. | `{"refreshInterval": 300}` |
| `themeSettings` | `Map<String, dynamic>` | Key-value pairs for app-wide theming. | `{"primaryColor": "#2E7D32"}` |
| `layoutSettings`| `Map<String, dynamic>` | Key-value pairs for global layout rules. | `{"sectionSpacing": 16.0}` |
| `cacheSettings` | `Map<String, dynamic>` | Key-value pairs for caching strategies. | `{"maxAge": 3600}` |
| `analyticsSettings`| `Map<String, dynamic>`| Key-value pairs for analytics tracking. | `{"enabled": true}` |
| `enabledFeatures`| `List<String>` | A list of feature flags that are enabled. | `["chat", "booking"]` |
| `experimentalFeatures`| `Map<String, dynamic>`| Key-value pairs for experimental features. | `{"newSearchUI": true}` |

### 3.2. `HomeSection` Entity

This is the primary entity for a single content block on the home screen.

| Property | Type | Description | Example |
| :--- | :--- | :--- | :--- |
| `id` | `String` | Unique identifier for the section. | `"sec_featured_properties"` |
| `type` | `SectionType` (Enum) | **CRITICAL**: The type of section, which determines the UI component to render. See Section 4.1 for all possible values. | `"HORIZONTAL_PROPERTY_LIST"` |
| `order` | `int` | The display order of the section on the screen (0-indexed). | `0` |
| `isActive` | `bool` | Whether this section should be displayed. | `true` |
| `title` | `String?` | The display title for the section (e.g., "Featured Properties"). | `"عقارات مميزة"` |
| `subtitle` | `String?` | The display subtitle for the section. | `"توصياتنا لك"` |
| `config` | `SectionConfig` | The detailed configuration for this specific section. See section 3.3. | `{...}` |
| `content` | `List<DynamicContent>` | **CRITICAL**: The list of content items to render within this section. See section 3.4. | `[...]` |
| `metadata` | `Map<String, dynamic>` | Any additional metadata for the section. | `{"source": "manual"}` |
| `scheduledAt` | `DateTime?` | If set, the section will only be visible after this time. | `null` |
| `expiresAt` | `DateTime?` | If set, the section will be hidden after this time. | `null` |
| `targetAudience`| `List<String>` | Defines which user segments should see this section. | `["new_users", "has_favorites"]` |
| `priority` | `int` | A priority number for ranking or sorting. | `100` |

### 3.3. `SectionConfig` Entity

This entity, nested within `HomeSection`, provides detailed configuration for how a section should look and behave.

| Property | Type | Description | Example |
| :--- | :--- | :--- | :--- |
| `id` | `String` | Unique ID for the configuration object itself. | `"cfg_hlist_1"` |
| `sectionType` | `SectionType` (Enum) | The type of section this config applies to. | `"HORIZONTAL_PROPERTY_LIST"` |
| `displaySettings`| `Map<String, dynamic>` | Controls what is displayed. | `{"showBadge": true}` |
| `layoutSettings` | `Map<String, dynamic>` | Controls the component layout. | `{"itemSpacing": 8.0}` |
| `styleSettings` | `Map<String, dynamic>` | Controls the visual style. | `{"borderRadius": 12.0}` |
| `behaviorSettings`| `Map<String, dynamic>` | Controls interactive behavior. | `{"autoPlay": true}` |
| `animationSettings`| `Map<String, dynamic>`| Controls animations. | `{"animationType": "FADE"}` |
| `cacheSettings` | `Map<String, dynamic>` | Caching rules for this section's content. | `{"enableCache": true}` |
| `propertyIds` | `List<String>` | A list of property IDs to be dynamically fetched and displayed. | `["prop_1", "prop_2"]` |
| `title` | `String?` | English title (overrides section title). | `"Featured"` |
| `titleAr` | `String?` | Arabic title (overrides section title). | `"مميز"` |
| `subtitle` | `String?` | English subtitle. | `"Our recommendations"` |
| `subtitleAr` | `String?` | Arabic subtitle. | `"توصياتنا"` |
| `backgroundColor`| `String?` | Hex color code for the section background. | `"#F5F5F5"` |
| `textColor` | `String?` | Hex color code for the section's text. | `"#212121"` |
| `customImage` | `String?` | URL for a custom background or banner image. | `"https://.../banner.png"` |
| `customData` | `Map<String, dynamic>` | Any other custom data. | `{}` |

### 3.4. `DynamicContent` Entity

This entity represents a single item within a section's `content` list.

| Property | Type | Description | Example |
| :--- | :--- | :--- | :--- |
| `id` | `String` | Unique ID for the content item. | `"prop_123"` |
| `sectionId` | `String` | The ID of the section this content belongs to. | `"sec_featured_properties"` |
| `contentType` | `String` | **CRITICAL**: The type of data held in the `data` field. Can be `PROPERTY`, `OFFER`, `ADVERTISEMENT`, `DESTINATION`, etc. | `"PROPERTY"` |
| `data` | `Map<String, dynamic>` | **CRITICAL**: The actual data payload. The structure of this map depends on `contentType`. It will match one of the main entities like `FeaturedProperty`, `CityDestination`, etc. | `{ "id": "prop_123", "name": "...", ... }` |
| `metadata` | `Map<String, dynamic>` | Additional metadata for this content item. | `{"score": 0.98}` |
| `expiresAt` | `DateTime?` | If set, this content item will expire. | `null` |
| `createdAt` | `DateTime` | Timestamp of creation. | `"2025-08-04T10:00:00Z"` |
| `updatedAt` | `DateTime` | Timestamp of last update. | `"2025-08-04T10:00:00Z"` |

### 3.5. `FeaturedProperty` Entity

This is the primary entity for representing a property. It's used in most listing sections.

| Property | Type | Description | Example |
| :--- | :--- | :--- | :--- |
| `id` | `String` | Unique property identifier. | `"prop_12345"` |
| `name` | `String` | The name of the property. | `"فندق وأجنحة سبأ"` |
| `address` | `String` | The street address of the property. | `"شارع الزبيري، صنعاء"` |
| `city` | `String` | The city where the property is located. | `"صنعاء"` |
| `latitude` | `double` | GPS latitude. | `15.3547` |
| `longitude` | `double` | GPS longitude. | `44.2068` |
| `starRating` | `int` | The star rating of the property (1-5). | `4` |
| `description` | `String` | A brief description of the property. | `"فندق فاخر بقلب العاصمة..."` |
| `images` | `List<String>` | A list of image URLs for the property. | `["url1.jpg", "url2.jpg"]` |
| `basePrice` | `double?` | The base price per night. | `25000.0` |
| `currency` | `String?` | The currency code. | `"YER"` |
| `amenities` | `List<String>` | A list of top amenity names. | `["Wi-Fi", "Parking", "Pool"]` |
| `averageRating`| `double` | The average user rating (0.0 - 5.0). | `4.7` |
| `viewCount` | `int` | The number of times the property has been viewed. | `1205` |
| `bookingCount`| `int` | The number of times the property has been booked. | `88` |
| `mainImageUrl`| `String?` | The primary image URL to display in lists. | `"url1.jpg"` |
| `isFeatured` | `bool` | Indicates if the property is featured. | `true` |
| `discountPercentage`| `double?` | A discount percentage, if any. | `15.0` |
| `propertyType`| `String?` | The type of property (e.g., "Hotel", "Apartment"). | `"Hotel"` |
| `featuredReason`| `String?` | Reason for being featured. | `"Popular Choice"` |
| `badgeText` | `String?` | Text for a promotional badge (e.g., "SALE"). | `"عرض خاص"` |
| `badgeColor` | `String?` | Hex color for the badge background. | `"#FF6F00"` |
| `promotionalMessage`| `String?` | A short promotional message. | `"خصم 15% لفترة محدودة"` |

### 3.6. `CityDestination` Entity

Represents a city that can be featured as a destination.

| Property | Type | Description | Example |
| :--- | :--- | :--- | :--- |
| `id` | `String` | Unique city identifier. | `"city_sanaa"` |
| `name` | `String` | English name of the city. | `"Sana'a"` |
| `nameAr` | `String` | Arabic name of the city. | `"صنعاء"` |
| `country` | `String` | English name of the country. | `"Yemen"` |
| `countryAr` | `String` | Arabic name of the country. | `"اليمن"` |
| `imageUrl` | `String` | A representative image URL for the city. | `"sanaa_image.jpg"` |
| `propertyCount`| `int` | The number of properties available in that city. | `250` |
| `averagePrice` | `double` | The average property price in the city. | `18000.0` |
| `isPopular` | `bool` | Whether this is a popular destination. | `true` |
| `isFeatured` | `bool` | Whether this destination is featured. | `false` |

### 3.7. `SponsoredAd` Entity

Represents a sponsored advertisement that can appear as a section.

| Property | Type | Description | Example |
| :--- | :--- | :--- | :--- |
| `id` | `String` | Unique ad identifier. | `"ad_summer_promo"` |
| `title` | `String` | The main title of the ad. | `"عروض الصيف"` |
| `subtitle` | `String?` | The subtitle of the ad. | `"خصومات تصل إلى 30%"` |
| `property` | `PropertySummary?` | A summary of a single linked property. | `{ "id": "prop_5", "name": "..." }` |
| `propertyIds` | `List<String>` | A list of property IDs this ad relates to. | `["prop_5", "prop_6"]` |
| `customImageUrl`| `String?` | A custom banner image for the ad. | `"ad_banner.png"` |
| `backgroundColor`| `String?` | Hex color for the ad background. | `"#E3F2FD"` |
| `ctaText` | `String` | Call-to-action button text. | `"اكتشف الآن"` |
| `ctaAction` | `String` | The action to perform on tap (e.g., "navigate", "open_url"). | `"navigate"` |
| `ctaData` | `Map<String, dynamic>` | Data for the CTA action (e.g., `{"route": "/offers"}`). | `{"route": "/offers/summer"}` |
| `startDate` | `DateTime` | The date the ad becomes active. | `"2025-08-01T00:00:00Z"` |
| `endDate` | `DateTime` | The date the ad expires. | `"2025-09-01T00:00:00Z"` |

---

## 4. Enumerations

The mobile app must correctly interpret these enums to render the UI as intended.

### 4.1. `SectionType` Enum

This is the most critical enum. It determines which UI component to render for a `HomeSection`.

| Value | Suggested UI Component | Description |
| :--- | :--- | :--- |
| `SINGLE_PROPERTY_AD` | `FeaturedPropertyCard` | A large, prominent card for a single sponsored property. Often uses parallax effects. |
| `FEATURED_PROPERTY_AD` | `FeaturedPropertyCard` | Similar to `SINGLE_PROPERTY_AD` but might have a "Featured" badge. |
| `MULTI_PROPERTY_AD` | `PropertiesGrid` | A grid (e.g., 2x2) showcasing multiple sponsored properties. |
| `UNIT_SHOWCASE_AD` | `UnitShowcaseWidget` | A special component to showcase specific *units* within a property. |
| `SINGLE_PROPERTY_OFFER` | `OfferCard` | A card displaying a special offer for a single property. |
| `LIMITED_TIME_OFFER` | `OfferCard` with Countdown | An offer card that includes a countdown timer to create urgency. |
| `SEASONAL_OFFER` | `ThemedOfferCard` | An offer card with seasonal theming (e.g., summer, Ramadan). |
| `MULTI_PROPERTY_OFFERS_GRID`| `OffersGrid` | A grid displaying offers from multiple properties. |
| `OFFERS_CAROUSEL` | `CarouselSlider` of `OfferCard` | A horizontally scrolling carousel of offer cards. |
| `FLASH_DEALS` | `FlashDealsCarousel` | A carousel for urgent, short-term deals, often with a prominent timer. |
| `HORIZONTAL_PROPERTY_LIST` | `HorizontalListView` of `PropertyCard` | A standard horizontally scrolling list of properties. |
| `VERTICAL_PROPERTY_GRID` | `VerticalGridView` of `PropertyCard` | A vertically scrolling grid of properties (e.g., 2 columns). |
| `MIXED_LAYOUT_LIST` | `ComplexListView` | A vertical list with items of varying sizes and layouts for visual interest. |
| `COMPACT_PROPERTY_LIST` | `CompactListView` of `CompactPropertyCard` | A vertical list using smaller, more compact cards to show more items. |
| `FEATURED_PROPERTIES_SHOWCASE`| `LargeCarouselSlider` | A large, visually rich carousel for showcasing top-tier properties. |
| `CITY_CARDS_GRID` | `GridView` of `CityCard` | A grid displaying city destinations. |
| `DESTINATION_CAROUSEL` | `CarouselSlider` of `CityCard` | A horizontally scrolling carousel of city destinations. |
| `EXPLORE_CITIES` | `LargeCityList` | A visually engaging list or grid for exploring cities. |
| `PREMIUM_CAROUSEL` | `PremiumCarousel` | A highly styled carousel, possibly with 3D effects or unique transitions. |
| `INTERACTIVE_SHOWCASE` | `InteractiveWidget` | A section that could contain interactive elements like a map or a quiz. |

### 4.2. `SectionAnimation` Enum

Defines the entry animation for a section or its items.

| Value | Description |
| :--- | :--- |
| `NONE` | No animation. |
| `FADE` | Simple fade-in effect. |
| `SLIDE` | Items slide in from the side or bottom. |
| `SCALE` | Items scale up from a smaller size. |
| `ROTATE` | Items rotate into view. |
| `PARALLAX` | Background image moves at a different speed than the foreground content. |
| `SHIMMER` | A shimmering effect used as a loading placeholder. |
| `PULSE` | A subtle pulsing effect to draw attention. |
| `BOUNCE` | Items bounce into place. |
| `FLIP` | Items flip into view. |

### 4.3. `SectionSize` Enum

Defines a relative size for a section, which can affect padding, font sizes, and item heights.

| Value | Description |
| :--- | :--- |
| `COMPACT` | Smaller than normal, for dense information. |
| `SMALL` | Slightly smaller than the default. |
| `MEDIUM` | The default, standard size. |
| `LARGE` | Larger than the default, for emphasis. |
| `EXTRA_LARGE` | Very large, for hero sections. |
| `FULL_SCREEN` | Takes up the full screen viewport. |

---

## 5. UI Component Rendering Logic

This section provides guidance on how to render the UI based on the data.

### 5.1. General Rendering Flow

1.  **Create a `HomeScreen` widget.** This widget will be responsible for fetching the data and managing the state (e.g., using `flutter_bloc`).
2.  **Use a `ListView.builder` or `CustomScrollView`** as the main scrollable body of the `HomeScreen`.
3.  The `itemCount` of the list will be the length of the `HomeSection` list from the API.
4.  The `itemBuilder` will receive a `HomeSection` object for each index.
5.  **Inside the `itemBuilder`, use a `switch` statement on `section.type` (the `SectionType` enum).**
6.  Each `case` in the switch will return a specific UI component widget corresponding to that section type (e.g., `case SectionType.horizontalPropertyList: return HorizontalPropertyListWidget(section: section);`).

### 5.2. Component-Specific Logic

#### `HorizontalPropertyListWidget` (`HORIZONTAL_PROPERTY_LIST`)
-   Should contain a `Column` with a title (`section.title`) and a horizontally scrolling `ListView.builder`.
-   The `ListView`'s `itemCount` is `section.content.length`.
-   Each item in the `ListView` is a `PropertyCard` widget.
-   The data for the `PropertyCard` comes from `section.content[index].data`, which should be cast to a `FeaturedProperty` map.

#### `VerticalPropertyGridWidget` (`VERTICAL_PROPERTY_GRID`)
-   Should contain a title and a `GridView.builder`.
-   Use a `SliverGridDelegateWithFixedCrossAxisCount` with `crossAxisCount` typically set to 2.
-   The `itemCount` is `section.content.length`.
-   Each item is a `PropertyCard`.

#### `OffersCarouselWidget` (`OFFERS_CAROUSEL`)
-   Use a third-party package like `carousel_slider`.
-   The items in the carousel are `OfferCard` widgets.
-   The `OfferCard` should display the offer's title, discount, and a countdown timer if the `type` is `LIMITED_TIME_OFFER`. The remaining time can be calculated from the `expiresAt` field in the `DynamicContent`'s `data`.

#### `CityCardsGridWidget` (`CITY_CARDS_GRID`)
-   Similar to the property grid, but uses `CityCard` widgets.
-   The `CityCard` should display the city's image (`imageUrl`), its Arabic name (`nameAr`), and the property count (`propertyCount`).

### 5.3. Handling Localization

-   The mobile app uses an `AppLocalizations` class to determine the current language (e.g., `isArabic`).
-   When displaying text from the API, always check for the localized version first.
-   **Example:** For a `CityDestination`, the name to display should be `isArabic ? city.nameAr : city.name`.

---

## 6. API Endpoint Summary (for context)

This is a summary of the API endpoints the mobile app will call.

-   **`GET /api/client/HomeSections/config`**
    -   **Description:** Fetches the global home screen configuration.
    -   **Response:** A single `HomeConfig` object.

-   **`GET /api/client/HomeSections/sections`**
    -   **Description:** Fetches the ordered list of sections to display on the home screen.
    -   **Response:** A `List<HomeSection>`.

-   **`GET /api/client/HomeSections/sponsored-ads`**
    -   **Description:** Fetches a list of active sponsored ads.
    -   **Response:** A `List<SponsoredAd>`.

-   **`GET /api/client/HomeSections/destinations`**
    -   **Description:** Fetches a list of city destinations.
    -   **Response:** A `List<CityDestination>`.

-   **`POST /api/client/HomeSections/sponsored-ads/{adId}/impression`**
    -   **Description:** Records that a user has seen a specific ad. This should be called when an ad section is visible on the screen for a certain duration (e.g., 2 seconds).

-   **`POST /api/client/HomeSections/sponsored-ads/{adId}/click`**
    -   **Description:** Records that a user has clicked on a specific ad. This should be called when a user taps on a sponsored ad component.

---

## 7. Example Scenario: Rendering a Home Screen

**1. The mobile app calls `GET /api/client/HomeSections/sections` and receives the following JSON:**

```json
[
  {
    "id": "sec_1",
    "type": "FEATURED_PROPERTIES_SHOWCASE",
    "order": 0,
    "isActive": true,
    "title": "عقارات مميزة",
    "config": { "autoPlay": true, "layoutType": "carousel" },
    "content": [
      {
        "id": "prop_101",
        "contentType": "PROPERTY",
        "data": {
          "id": "prop_101",
          "name": "فندق موفنبيك صنعاء",
          "mainImageUrl": "movenpick.jpg",
          "city": "صنعاء",
          "averageRating": 4.8,
          "basePrice": 45000,
          "currency": "YER"
        }
      },
      {
        "id": "prop_102",
        "contentType": "PROPERTY",
        "data": {
          "id": "prop_102",
          "name": "فندق شمر",
          "mainImageUrl": "shemar.jpg",
          "city": "صنعاء",
          "averageRating": 4.5,
          "basePrice": 20000,
          "currency": "YER"
        }
      }
    ]
  },
  {
    "id": "sec_2",
    "type": "CITY_CARDS_GRID",
    "order": 1,
    "isActive": true,
    "title": "استكشف المدن",
    "config": { "layoutType": "grid", "columnsCount": 2 },
    "content": [
      {
        "id": "city_sanaa",
        "contentType": "DESTINATION",
        "data": {
          "id": "city_sanaa",
          "nameAr": "صنعاء",
          "imageUrl": "sanaa.jpg",
          "propertyCount": 250
        }
      },
      {
        "id": "city_aden",
        "contentType": "DESTINATION",
        "data": {
          "id": "city_aden",
          "nameAr": "عدن",
          "imageUrl": "aden.jpg",
          "propertyCount": 180
        }
      }
    ]
  }
]
```

**2. The app's `HomeScreen` would:**
-   Create a vertical `ListView`.
-   **For the first item (index 0):**
    -   It sees the `type` is `FEATURED_PROPERTIES_SHOWCASE`.
    -   It renders a `FeaturedShowcaseWidget`.
    -   This widget would be a large, auto-playing carousel.
    -   It would contain two slides, one for "فندق موفنبيك صنعاء" and one for "فندق شمر", using the data from the `content` array.
-   **For the second item (index 1):**
    -   It sees the `type` is `CITY_CARDS_GRID`.
    -   It renders a `CityGridWidget`.
    -   This widget would be a `GridView` with 2 columns.
    -   It would contain two `CityCard` widgets, one for "صنعاء" and one for "عدن".

This dynamic approach allows the backend to completely control the layout and content of the home screen without requiring an app update.

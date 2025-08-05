// frontend/src/types/homeSections.types.ts

export interface DynamicContent {
  id: string;
  sectionId: string;
  contentType: string;
  contentData: Record<string, any>; // Match backend naming
  metadata: Record<string, any>;
  expiresAt?: string;
  displayOrder: number; // Match backend
  isActive: boolean; // Match backend
  createdAt: string;
  updatedAt: string;
  // Computed properties from backend
  isValid?: boolean;
  isExpired?: boolean;
}

export interface DynamicHomeSection {
  id: string;
  sectionType: string; // Match backend naming
  order: number;
  isActive: boolean;
  title?: string;
  subtitle?: string;
  titleAr?: string; // Match backend
  subtitleAr?: string; // Match backend
  sectionConfig: Record<string, any>; // Match backend naming
  metadata: Record<string, any>;
  scheduledAt?: string;
  expiresAt?: string;
  targetAudience: string[];
  priority: number;
  content: DynamicContent[];
  createdAt: string;
  updatedAt: string;
  // Computed properties from backend
  isVisible?: boolean;
  isExpired?: boolean;
  isScheduled?: boolean;
  isTimeSensitive?: boolean;
}

export interface DynamicHomeConfig {
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

export interface CityDestination {
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

export interface PropertySummary {
  id: string;
  name: string;
  mainImageUrl?: string;
  basePrice?: number;
  currency?: string;
  averageRating?: number;
}

export interface SponsoredAd {
  id: string;
  title: string;
  subtitle?: string;
  description?: string;
  property?: PropertySummary;
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

// Command types for dynamic sections - Match backend DTOs
export interface CreateDynamicSectionCommand {
  sectionType: string;
  order: number;
  title?: string;
  subtitle?: string;
  titleAr?: string;
  subtitleAr?: string;
  sectionConfig?: Record<string, any>;
  metadata?: Record<string, any>;
  scheduledAt?: string;
  expiresAt?: string;
  targetAudience?: string[];
  priority?: number;
  content?: CreateDynamicSectionContentCommand[];
}

export interface CreateDynamicSectionContentCommand {
  contentType: string;
  contentData: Record<string, any>;
  metadata?: Record<string, any>;
  displayOrder: number;
  expiresAt?: string;
}

export interface UpdateDynamicSectionCommand {
  title?: string;
  subtitle?: string;
  titleAr?: string;
  subtitleAr?: string;
  sectionConfig?: Record<string, any>;
  metadata?: Record<string, any>;
  scheduledAt?: string;
  expiresAt?: string;
  targetAudience?: string[];
  priority?: number;
  content?: UpdateDynamicSectionContentCommand[];
}

export interface UpdateDynamicSectionContentCommand {
  id?: string; // null for new content
  contentType: string;
  contentData: Record<string, any>;
  metadata?: Record<string, any>;
  displayOrder: number;
  expiresAt?: string;
  isDeleted?: boolean; // to mark for deletion
}

export interface ToggleSectionStatusCommand {
  id: string;
  setActive?: boolean;
}

export interface DeleteDynamicSectionCommand {
  id: string;
}

export interface ReorderDynamicSectionsCommand {
  sections: { sectionId: string; newOrder: number }[];
}

// Command types for dynamic config
export interface CreateDynamicConfigCommand {
  version: string;
  globalSettings?: Record<string, any>;
  themeSettings?: Record<string, any>;
  layoutSettings?: Record<string, any>;
  cacheSettings?: Record<string, any>;
  analyticsSettings?: Record<string, any>;
  enabledFeatures?: string[];
  experimentalFeatures?: Record<string, any>;
  description?: string;
}

export interface UpdateDynamicConfigCommand {
  id: string;
  globalSettings?: Record<string, any>;
  themeSettings?: Record<string, any>;
  layoutSettings?: Record<string, any>;
  cacheSettings?: Record<string, any>;
  analyticsSettings?: Record<string, any>;
  enabledFeatures?: string[];
  experimentalFeatures?: Record<string, any>;
  description?: string;
}

export interface PublishDynamicConfigCommand {
  id: string;
}

// Command types for city destinations
export interface CreateCityDestinationCommand {
  name: string;
  nameAr: string;
  country: string;
  countryAr: string;
  imageUrl: string;
  latitude: number;
  longitude: number;
  currency: string;
  description?: string;
  descriptionAr?: string;
  additionalImages?: string[];
  highlights?: string[];
  highlightsAr?: string[];
  weatherData?: Record<string, any>;
  attractionsData?: Record<string, any>;
  metadata?: Record<string, any>;
}

export interface UpdateCityDestinationCommand extends CreateCityDestinationCommand {
  id: string;
}

export interface UpdateCityDestinationStatsCommand {
  id: string;
  propertyCount: number;
  averagePrice: number;
  averageRating: number;
  reviewCount: number;
}

// Command types for sponsored ads
export interface CreateSponsoredAdCommand {
  title: string;
  subtitle?: string;
  description?: string;
  propertyIds?: string[];
  customImageUrl?: string;
  backgroundColor?: string;
  textColor?: string;
  styling?: Record<string, any>;
  ctaText: string;
  ctaAction: string;
  ctaData?: Record<string, any>;
  startDate: string;
  endDate: string;
  priority?: number;
  targetingData?: Record<string, any>;
  analyticsData?: Record<string, any>;
}

export interface UpdateSponsoredAdCommand extends CreateSponsoredAdCommand {
  id: string;
}

export interface RecordAdInteractionCommand {
  adId: string;
  interactionType: 'impression' | 'click';
  userId?: string;
  userAgent?: string;
  ipAddress?: string;
  additionalData?: string;
}

// frontend/src/types/enums.ts
export enum SectionType {
  SINGLE_PROPERTY_AD = 'SINGLE_PROPERTY_AD',
  FEATURED_PROPERTY_AD = 'FEATURED_PROPERTY_AD',
  MULTI_PROPERTY_AD = 'MULTI_PROPERTY_AD',
  UNIT_SHOWCASE_AD = 'UNIT_SHOWCASE_AD',
  SINGLE_PROPERTY_OFFER = 'SINGLE_PROPERTY_OFFER',
  LIMITED_TIME_OFFER = 'LIMITED_TIME_OFFER',
  SEASONAL_OFFER = 'SEASONAL_OFFER',
  MULTI_PROPERTY_OFFERS_GRID = 'MULTI_PROPERTY_OFFERS_GRID',
  OFFERS_CAROUSEL = 'OFFERS_CAROUSEL',
  FLASH_DEALS = 'FLASH_DEALS',
  HORIZONTAL_PROPERTY_LIST = 'HORIZONTAL_PROPERTY_LIST',
  VERTICAL_PROPERTY_GRID = 'VERTICAL_PROPERTY_GRID',
  MIXED_LAYOUT_LIST = 'MIXED_LAYOUT_LIST',
  COMPACT_PROPERTY_LIST = 'COMPACT_PROPERTY_LIST',
  FEATURED_PROPERTIES_SHOWCASE = 'FEATURED_PROPERTIES_SHOWCASE',
  CITY_CARDS_GRID = 'CITY_CARDS_GRID',
  DESTINATION_CAROUSEL = 'DESTINATION_CAROUSEL',
  EXPLORE_CITIES = 'EXPLORE_CITIES',
  PREMIUM_CAROUSEL = 'PREMIUM_CAROUSEL',
  INTERACTIVE_SHOWCASE = 'INTERACTIVE_SHOWCASE',
}

export enum SectionAnimation {
  NONE = 'NONE',
  FADE = 'FADE',
  SLIDE = 'SLIDE',
  SCALE = 'SCALE',
  ROTATE = 'ROTATE',
  PARALLAX = 'PARALLAX',
  SHIMMER = 'SHIMMER',
  PULSE = 'PULSE',
  BOUNCE = 'BOUNCE',
  FLIP = 'FLIP',
}

export enum SectionSize {
  COMPACT = 'COMPACT',
  SMALL = 'SMALL',
  MEDIUM = 'MEDIUM',
  LARGE = 'LARGE',
  EXTRA_LARGE = 'EXTRA_LARGE',
  FULL_SCREEN = 'FULL_SCREEN',
}
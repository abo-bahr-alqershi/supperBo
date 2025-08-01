import type { ComponentPreview } from './component.types';

export interface HomeScreenTemplate {
  id: string;
  name: string;
  description: string;
  version: string;
  isActive: boolean;
  isDefault: boolean;
  publishedAt?: string;
  publishedBy?: string;
  publishedByName?: string;
  platform: Platform;
  targetAudience: TargetAudience;
  metaData?: string;
  customizationData?: string;
  userPreferences?: string;
  createdAt: string;
  updatedAt?: string;
  sections: HomeScreenSection[];
}

export interface HomeScreenSection {
  id: string;
  templateId: string;
  name: string;
  title: string;
  subtitle?: string;
  order: number;
  isVisible: boolean;
  backgroundColor?: string;
  backgroundImage?: string;
  padding?: string;
  margin?: string;
  minHeight?: number;
  maxHeight?: number;
  customStyles?: string;
  conditions?: string;
  components: HomeScreenComponent[];
}

export interface HomeScreenComponent {
  id: string;
  sectionId: string;
  componentType: ComponentType;
  name: string;
  order: number;
  isVisible: boolean;
  colSpan: number;
  rowSpan: number;
  alignment: Alignment;
  customClasses?: string;
  animationType?: AnimationType;
  animationDuration?: number;
  conditions?: string;
  properties: ComponentProperty[];
  styles: ComponentStyle[];
  actions: ComponentAction[];
  dataSource?: ComponentDataSource;
}

export interface ComponentProperty {
  id: string;
  propertyKey: string;
  propertyName: string;
  propertyType: PropertyType;
  value?: any;
  defaultValue?: any;
  isRequired: boolean;
  validationRules?: string;
  options?: string;
  helpText?: string;
  order: number;
}

export interface ComponentStyle {
  id: string;
  styleKey: string;
  styleValue: string;
  unit?: string;
  isImportant: boolean;
  mediaQuery?: string;
  state?: StyleState;
  platform: Platform;
}

export interface ComponentAction {
  id: string;
  actionType: ActionType;
  actionTrigger: ActionTrigger;
  actionTarget: string;
  actionParams?: string;
  conditions?: string;
  requiresAuth: boolean;
  animationType?: string;
  priority: number;
}

export interface ComponentDataSource {
  id: string;
  sourceType: DataSourceType;
  dataEndpoint?: string;
  httpMethod?: HttpMethod;
  headers?: string;
  queryParams?: string;
  requestBody?: string;
  dataMapping?: string;
  cacheKey?: string;
  cacheDuration?: number;
  refreshTrigger?: RefreshTrigger;
  refreshInterval?: number;
  errorHandling?: string;
  mockData?: string;
  useMockInDev: boolean;
}

export type Platform = 'iOS' | 'Android' | 'All';
export type TargetAudience = 'Guest' | 'User' | 'Premium' | 'All';
export type ComponentType = 'Banner' | 'Carousel' | 'CategoryGrid' | 'PropertyList' | 
  'SearchBar' | 'OfferCard' | 'TextBlock' | 'ImageGallery' | 'MapView' | 'FilterBar';
export type Alignment = 'left' | 'center' | 'right';
export type AnimationType = 'fade' | 'slide' | 'zoom' | 'bounce' | 'rotate';
export type PropertyType = 'text' | 'number' | 'boolean' | 'select' | 'multiselect' | 
  'color' | 'image' | 'date' | 'object';
export type StyleState = 'normal' | 'hover' | 'active' | 'focus' | 'disabled';
export type ActionType = 'Navigate' | 'OpenModal' | 'CallAPI' | 'Share' | 'Download';
export type ActionTrigger = 'Click' | 'LongPress' | 'Swipe' | 'Load';
export type DataSourceType = 'Static' | 'API' | 'Database' | 'Cache';
export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE';
export type RefreshTrigger = 'OnLoad' | 'OnFocus' | 'Manual' | 'Timer';

// Command interfaces
export interface CreateHomeScreenTemplateCommand {
  name: string;
  description: string;
  version: string;
  platform: Platform;
  targetAudience: TargetAudience;
  metaData: string;
}

export interface UpdateHomeScreenTemplateCommand {
  name: string;
  description: string;
  metaData: string;
}

export interface CreateHomeScreenSectionCommand {
  templateId: string;
  name: string;
  title: string;
  subtitle: string;
  order: number;
  backgroundColor: string;
  backgroundImage: string;
  padding: string;
  margin: string;
  minHeight: number;
  maxHeight: number;
  customStyles: string;
  conditions: string;
}

export interface UpdateHomeScreenSectionCommand {
  name: string;
  title: string;
  subtitle: string;
  backgroundColor: string;
  backgroundImage: string;
  padding: string;
  margin: string;
  minHeight: number;
  maxHeight: number;
  customStyles: string;
  conditions: string;
}

export interface CreateHomeScreenComponentCommand {
  sectionId: string;
  componentType: string;
  name: string;
  order: number;
  colSpan: number;
  rowSpan: number;
  alignment: string;
  customClasses: string;
  animationType: string;
  animationDuration: number;
  conditions: string;
}

export interface UpdateHomeScreenComponentCommand {
  name: string;
  colSpan: number;
  rowSpan: number;
  alignment: string;
  customClasses: string;
  animationType: string;
  animationDuration: number;
  conditions: string;
}

export interface ReorderSectionsCommand {
  templateId: string;
  sections: Array<{ sectionId: string; newOrder: number }>;
}

export interface ReorderComponentsCommand {
  sectionId: string;
  components: Array<{ componentId: string; newOrder: number }>;
}

// Preview DTO interfaces
export interface HomeScreenPreviewDto {
  templateId: string;
  templateName: string;
  platform: string;
  deviceType: string;
  sections: HomeScreenSectionPreviewDto[];
  metadata: PreviewMetadata;
}

export interface HomeScreenSectionPreviewDto {
  id: string;
  name: string;
  title: string;
  subtitle: string;
  order: number;
  isVisible: boolean;
  styles: Record<string, string>;
  components: ComponentPreview[];
}

export interface PreviewMetadata {
  generatedAt: string;
  totalSections: number;
  totalComponents: number;
  estimatedLoadTime: number;
  usedMockData: boolean;
}

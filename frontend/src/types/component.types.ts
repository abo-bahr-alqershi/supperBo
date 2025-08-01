export interface ComponentTypeDefinition {
  type: string;
  name: string;
  description: string;
  icon: string;
  category: ComponentCategory;
  defaultColSpan: number;
  defaultRowSpan: number;
  allowResize: boolean;
  properties: ComponentPropertyMetadata[];
  supportedPlatforms: string[];
  defaultStyles?: Record<string, any>;
}

export interface ComponentPropertyMetadata {
  key: string;
  name: string;
  type: string;
  isRequired: boolean;
  defaultValue?: any;
  options?: string[];
  description?: string;
  validationPattern?: string;
}

export interface DataSourceDefinition {
  id: string;
  name: string;
  type: string;
  description: string;
  endpoint?: string;
  isAvailable: boolean;
  requiresAuth: boolean;
  supportedComponents: string[];
  parameters?: DataSourceParameter[];
  cacheDuration?: number;
}

export interface DataSourceParameter {
  key: string;
  name: string;
  type: string;
  defaultValue?: any;
  isRequired: boolean;
  description?: string;
  options?: string[];
}

export interface ComponentPreview {
  id: string;
  type: string;
  name: string;
  order: number;
  colSpan: number;
  rowSpan: number;
  alignment: string;
  properties: Record<string, any>;
  styles: Record<string, string>;
  data?: any;
  animation?: AnimationConfig;
}

export interface AnimationConfig {
  type: string;
  duration: number;
  delay: number;
  easing?: string;
}

export type ComponentCategory = 'Display' | 'Navigation' | 'Input' | 'Data' | 'Layout';

import { useState, useCallback, useEffect } from 'react';
import type { ComponentProperty, HomeScreenComponent } from '../types/homeScreen.types';
import type { ComponentPropertyMetadata } from '../types/component.types';
import componentService from '../services/componentService';
import { useDebounce } from './useDebounce';

interface UseComponentPropertiesOptions {
  component: HomeScreenComponent | null;
  propertyDefinitions: ComponentPropertyMetadata[];
  onChange?: (propertyKey: string, value: any) => void;
  autoSave?: boolean;
  debounceMs?: number;
}

export const useComponentProperties = ({
  component,
  propertyDefinitions,
  onChange,
  autoSave = true,
  debounceMs = 500
}: UseComponentPropertiesOptions) => {
  const [properties, setProperties] = useState<Record<string, any>>({});
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isDirty, setIsDirty] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  const debouncedProperties = useDebounce(properties, debounceMs);

  // Initialize properties from component
  useEffect(() => {
    if (component) {
      const initialProps: Record<string, any> = {};
      
      // Set values from component properties
      component.properties.forEach(prop => {
        initialProps[prop.propertyKey] = prop.value ?? prop.defaultValue;
      });

      // Add default values for missing properties
      propertyDefinitions.forEach(def => {
        if (!(def.key in initialProps)) {
          initialProps[def.key] = def.defaultValue;
        }
      });

      setProperties(initialProps);
      setIsDirty(false);
    }
  }, [component, propertyDefinitions]);

  // Validate property value
  const validateProperty = useCallback((
    key: string, 
    value: any, 
    definition: ComponentPropertyMetadata
  ): string | null => {
    // Required validation
    if (definition.isRequired && !value) {
      return `${definition.name} is required`;
    }

    // Type-specific validation
    switch (definition.type) {
      case 'number':
        if (value && isNaN(Number(value))) {
          return `${definition.name} must be a number`;
        }
        break;

      case 'url':
        if (value && !isValidUrl(value)) {
          return `${definition.name} must be a valid URL`;
        }
        break;

      case 'email':
        if (value && !isValidEmail(value)) {
          return `${definition.name} must be a valid email`;
        }
        break;

      case 'color':
        if (value && !isValidColor(value)) {
          return `${definition.name} must be a valid color`;
        }
        break;
    }

    // Custom validation pattern
    if (definition.validationPattern && value) {
      const regex = new RegExp(definition.validationPattern);
      if (!regex.test(value)) {
        return `${definition.name} format is invalid`;
      }
    }

    return null;
  }, []);

  // Update property value
  const updateProperty = useCallback((key: string, value: any) => {
    const definition = propertyDefinitions.find(d => d.key === key);
    if (!definition) return;

    // Validate
    const error = validateProperty(key, value, definition);
    setErrors(prev => ({
      ...prev,
      [key]: error || ''
    }));

    // Update value
    setProperties(prev => ({
      ...prev,
      [key]: value
    }));
    setIsDirty(true);

    // Notify change
    onChange?.(key, value);
  }, [propertyDefinitions, validateProperty, onChange]);

  // Reset property to default
  const resetProperty = useCallback((key: string) => {
    const definition = propertyDefinitions.find(d => d.key === key);
    if (definition) {
      updateProperty(key, definition.defaultValue);
    }
  }, [propertyDefinitions, updateProperty]);

  // Reset all properties
  const resetAllProperties = useCallback(() => {
    const resetProps: Record<string, any> = {};
    propertyDefinitions.forEach(def => {
      resetProps[def.key] = def.defaultValue;
    });
    setProperties(resetProps);
    setErrors({});
    setIsDirty(true);
  }, [propertyDefinitions]);

  // Auto-save debounced properties
  useEffect(() => {
    if (autoSave && isDirty && component && Object.keys(debouncedProperties).length > 0) {
      const hasErrors = Object.values(errors).some(error => error);
      if (!hasErrors) {
        saveProperties();
      }
    }
  }, [debouncedProperties, autoSave, isDirty, component, errors]);

  // Save properties to server
  const saveProperties = useCallback(async () => {
    if (!component) return;

    setIsSaving(true);
    try {
      // Update each changed property
      const updatePromises = Object.entries(properties).map(([key, value]) => {
        const property = component.properties.find(p => p.propertyKey === key);
        if (property && property.value !== value) {
          return componentService.updateComponentProperty(
            component.id,
            property.id,
            value
          );
        }
        return Promise.resolve();
      });

      await Promise.all(updatePromises);
      setIsDirty(false);
    } catch (error) {
      console.error('Failed to save properties:', error);
    } finally {
      setIsSaving(false);
    }
  }, [component, properties]);

  // Get property value with type coercion
  const getPropertyValue = useCallback((key: string): any => {
    const value = properties[key];
    const definition = propertyDefinitions.find(d => d.key === key);
    
    if (!definition) return value;

    switch (definition.type) {
      case 'number':
        return value ? Number(value) : 0;
      case 'boolean':
        return Boolean(value);
      case 'array':
        return Array.isArray(value) ? value : [];
      case 'object':
        return typeof value === 'object' ? value : {};
      default:
        return value;
    }
  }, [properties, propertyDefinitions]);

  return {
    properties,
    errors,
    isDirty,
    isSaving,
    updateProperty,
    resetProperty,
    resetAllProperties,
    saveProperties,
    getPropertyValue
  };
};

// Validation helpers
const isValidUrl = (url: string): boolean => {
  try {
    new URL(url);
    return true;
  } catch {
    return false;
  }
};

const isValidEmail = (email: string): boolean => {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
};

const isValidColor = (color: string): boolean => {
  return /^#[0-9A-F]{6}$/i.test(color) || 
         /^rgb\(\d+,\s*\d+,\s*\d+\)$/.test(color) ||
         /^rgba\(\d+,\s*\d+,\s*\d+,\s*[\d.]+\)$/.test(color);
};
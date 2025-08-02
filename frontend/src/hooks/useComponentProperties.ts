import { useState, useCallback, useEffect, useMemo } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { debounce } from 'lodash';
import { toast } from 'react-hot-toast';
import homeScreenService from '../services/homeScreenService';
import type {
  HomeScreenComponent,
  ComponentProperty,
  ComponentStyle,
  ComponentAction,
  ComponentDataSource,
  UpdateHomeScreenComponentCommand
} from '../types/homeScreen.types';
import { validateComponent } from '../utils/componentFactory';
import { parseStyleValue, isValidColor } from '../utils/styleGenerator';

interface UseComponentPropertiesOptions {
  component: HomeScreenComponent | null;
  templateId?: string;
  autoSave?: boolean;
  autoSaveDelay?: number;
  onUpdate?: (component: HomeScreenComponent) => void;
}

export const useComponentProperties = (options: UseComponentPropertiesOptions) => {
  const {
    component,
    templateId,
    autoSave = true,
    autoSaveDelay = 1000,
    onUpdate
  } = options;
  
  const queryClient = useQueryClient();
  const [localComponent, setLocalComponent] = useState<HomeScreenComponent | null>(component);
  const [errors, setErrors] = useState<string[]>([]);
  const [isDirty, setIsDirty] = useState(false);
  
  // Update local component when prop changes
  useEffect(() => {
    setLocalComponent(component);
    setIsDirty(false);
  }, [component]);
  
  // Update component mutation
  const updateComponentMutation = useMutation({
    mutationFn: ({ id, command }: { id: string; command: UpdateHomeScreenComponentCommand }) =>
      homeScreenService.updateComponent(id, command),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      setIsDirty(false);
      onUpdate?.(data);
    },
    onError: () => {
      toast.error('Failed to update component properties');
    }
  });
  
  // Debounced save function
  const debouncedSave = useMemo(
    () => debounce((componentToSave: HomeScreenComponent) => {
      if (!autoSave || !componentToSave) return;
      
      const command: UpdateHomeScreenComponentCommand = {
        name: componentToSave.name,
        colSpan: componentToSave.colSpan,
        rowSpan: componentToSave.rowSpan,
        alignment: componentToSave.alignment,
        customClasses: componentToSave.customClasses || '',
        animationType: componentToSave.animationType || '',
        animationDuration: componentToSave.animationDuration || 0,
        conditions: componentToSave.conditions || ''
      };
      
      updateComponentMutation.mutate({ id: componentToSave.id, command });
    }, autoSaveDelay),
    [autoSave, autoSaveDelay, updateComponentMutation]
  );
  
  // Update basic properties
  const updateBasicProperty = useCallback((key: keyof HomeScreenComponent, value: any) => {
    if (!localComponent) return;
    
    const updated = { ...localComponent, [key]: value };
    setLocalComponent(updated);
    setIsDirty(true);
    
    // Validate
    const validationErrors = validateComponent(updated);
    setErrors(validationErrors);
    
    if (validationErrors.length === 0) {
      debouncedSave(updated);
    }
  }, [localComponent, debouncedSave]);
  
  // Update component property
  const updateProperty = useCallback((propertyKey: string, value: any) => {
    if (!localComponent) return;
    
    const updatedProperties = localComponent.properties.map(prop =>
      prop.propertyKey === propertyKey ? { ...prop, value } : prop
    );
    
    const updated = { ...localComponent, properties: updatedProperties };
    setLocalComponent(updated);
    setIsDirty(true);
    
    // Validate property
    const property = updatedProperties.find(p => p.propertyKey === propertyKey);
    if (property?.isRequired && !value) {
      setErrors([`Property "${property.propertyName}" is required`]);
    } else {
      setErrors([]);
      debouncedSave(updated);
    }
  }, [localComponent, debouncedSave]);
  
  // Update style
  const updateStyle = useCallback((styleKey: string, styleValue: string, unit?: string) => {
    if (!localComponent) return;
    
    // Validate style value
    if (styleKey.includes('color') && !isValidColor(styleValue)) {
      setErrors([`Invalid color value: ${styleValue}`]);
      return;
    }
    
    const existingStyleIndex = localComponent.styles.findIndex(s => s.styleKey === styleKey);
    let updatedStyles: ComponentStyle[];
    
    if (existingStyleIndex >= 0) {
      updatedStyles = localComponent.styles.map((style, index) =>
        index === existingStyleIndex
          ? { ...style, styleValue, unit }
          : style
      );
    } else {
      const newStyle: ComponentStyle = {
        id: `style_${Date.now()}`,
        styleKey,
        styleValue,
        unit,
        isImportant: false,
        platform: 'All'
      };
      updatedStyles = [...localComponent.styles, newStyle];
    }
    
    const updated = { ...localComponent, styles: updatedStyles };
    setLocalComponent(updated);
    setIsDirty(true);
    setErrors([]);
    debouncedSave(updated);
  }, [localComponent, debouncedSave]);
  
  // Remove style
  const removeStyle = useCallback((styleKey: string) => {
    if (!localComponent) return;
    
    const updatedStyles = localComponent.styles.filter(s => s.styleKey !== styleKey);
    const updated = { ...localComponent, styles: updatedStyles };
    setLocalComponent(updated);
    setIsDirty(true);
    debouncedSave(updated);
  }, [localComponent, debouncedSave]);
  
  // Update action
  const updateAction = useCallback((actionId: string, updates: Partial<ComponentAction>) => {
    if (!localComponent) return;
    
    const updatedActions = localComponent.actions.map(action =>
      action.id === actionId ? { ...action, ...updates } : action
    );
    
    const updated = { ...localComponent, actions: updatedActions };
    setLocalComponent(updated);
    setIsDirty(true);
    debouncedSave(updated);
  }, [localComponent, debouncedSave]);
  
  // Add action
  const addAction = useCallback((action: Omit<ComponentAction, 'id'>) => {
    if (!localComponent) return;
    
    const newAction: ComponentAction = {
      ...action,
      id: `action_${Date.now()}`
    };
    
    const updated = {
      ...localComponent,
      actions: [...localComponent.actions, newAction]
    };
    setLocalComponent(updated);
    setIsDirty(true);
    debouncedSave(updated);
  }, [localComponent, debouncedSave]);
  
  // Remove action
  const removeAction = useCallback((actionId: string) => {
    if (!localComponent) return;
    
    const updatedActions = localComponent.actions.filter(a => a.id !== actionId);
    const updated = { ...localComponent, actions: updatedActions };
    setLocalComponent(updated);
    setIsDirty(true);
    debouncedSave(updated);
  }, [localComponent, debouncedSave]);
  
  // Update data source
  const updateDataSource = useCallback((dataSource: Partial<ComponentDataSource>) => {
    if (!localComponent) return;
    
    const updated = {
      ...localComponent,
      dataSource: localComponent.dataSource
        ? { ...localComponent.dataSource, ...dataSource }
        : { id: `ds_${Date.now()}`, ...dataSource } as ComponentDataSource
    };
    setLocalComponent(updated);
    setIsDirty(true);
    debouncedSave(updated);
  }, [localComponent, debouncedSave]);
  
  // Remove data source
  const removeDataSource = useCallback(() => {
    if (!localComponent) return;
    
    const updated = { ...localComponent, dataSource: undefined };
    setLocalComponent(updated);
    setIsDirty(true);
    debouncedSave(updated);
  }, [localComponent, debouncedSave]);
  
  // Reset changes
  const resetChanges = useCallback(() => {
    setLocalComponent(component);
    setIsDirty(false);
    setErrors([]);
  }, [component]);
  
  // Save changes immediately
  const saveChanges = useCallback(() => {
    if (!localComponent || !isDirty) return;
    
    debouncedSave.cancel();
    
    const command: UpdateHomeScreenComponentCommand = {
      name: localComponent.name,
      colSpan: localComponent.colSpan,
      rowSpan: localComponent.rowSpan,
      alignment: localComponent.alignment,
      customClasses: localComponent.customClasses || '',
      animationType: localComponent.animationType || '',
      animationDuration: localComponent.animationDuration || 0,
      conditions: localComponent.conditions || ''
    };
    
    updateComponentMutation.mutate({ id: localComponent.id, command });
  }, [localComponent, isDirty, debouncedSave, updateComponentMutation]);
  
  // Get property value
  const getPropertyValue = useCallback((propertyKey: string): any => {
    const property = localComponent?.properties.find(p => p.propertyKey === propertyKey);
    return property?.value ?? property?.defaultValue;
  }, [localComponent]);
  
  // Get style value
  const getStyleValue = useCallback((styleKey: string): string | undefined => {
    const style = localComponent?.styles.find(s => s.styleKey === styleKey);
    return style ? `${style.styleValue}${style.unit || ''}` : undefined;
  }, [localComponent]);
  
  // Get computed styles
  const getComputedStyles = useCallback((): Record<string, string> => {
    if (!localComponent) return {};
    
    return localComponent.styles.reduce((acc, style) => {
      const value = style.unit ? `${style.styleValue}${style.unit}` : style.styleValue;
      acc[style.styleKey] = value;
      return acc;
    }, {} as Record<string, string>);
  }, [localComponent]);
  
  return {
    component: localComponent,
    errors,
    isDirty,
    isSaving: updateComponentMutation.isPending,
    
    // Basic properties
    updateBasicProperty,
    
    // Properties
    updateProperty,
    getPropertyValue,
    
    // Styles
    updateStyle,
    removeStyle,
    getStyleValue,
    getComputedStyles,
    
    // Actions
    updateAction,
    addAction,
    removeAction,
    
    // Data source
    updateDataSource,
    removeDataSource,
    
    // General
    resetChanges,
    saveChanges
  };
};
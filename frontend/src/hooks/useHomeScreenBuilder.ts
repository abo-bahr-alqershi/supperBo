import { useState, useCallback, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import type {
  HomeScreenTemplate,
  HomeScreenSection,
  HomeScreenComponent
} from '../types/homeScreen.types';
import homeScreenService from '../services/homeScreenService';
import { useNotification } from './useNotification';

export interface HomeScreenBuilderState {
  template: HomeScreenTemplate | null;
  selectedSection: HomeScreenSection | null;
  selectedComponent: HomeScreenComponent | null;
  isLoading: boolean;
  isSaving: boolean;
  hasUnsavedChanges: boolean;
  errors: Record<string, string>;
}

export const useHomeScreenBuilder = (templateId?: string) => {
  const [state, setState] = useState<HomeScreenBuilderState>({
    template: null,
    selectedSection: null,
    selectedComponent: null,
    isLoading: false,
    isSaving: false,
    hasUnsavedChanges: false,
    errors: {}
  });

  const { showSuccess, showError } = useNotification();

  // Load template
  const loadTemplate = useCallback(async (id: string) => {
    setState(prev => ({ ...prev, isLoading: true, errors: {} }));
    try {
      const template = await homeScreenService.getTemplateById(id);
      setState(prev => ({
        ...prev,
        template,
        isLoading: false
      }));
    } catch (error) {
      setState(prev => ({
        ...prev,
        isLoading: false,
        errors: { load: 'Failed to load template' }
      }));
      showError('Failed to load template');
    }
  }, [showError]);

  // Create section
  const createSection = useCallback(async (section: Partial<HomeScreenSection>) => {
    if (!state.template) return;

    setState(prev => ({ ...prev, isSaving: true }));
    try {
      const newSection = await homeScreenService.createSection({
        ...section,
        templateId: state.template.id,
        order: state.template.sections.length
      });

      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: [...prev.template!.sections, newSection]
        },
        selectedSection: newSection,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Section created successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to create section');
    }
  }, [state.template, showSuccess, showError]);

  // Update section
  const updateSection = useCallback(async (
    sectionId: string, 
    updates: Partial<HomeScreenSection>
  ) => {
    setState(prev => ({ ...prev, isSaving: true }));
    try {
      const updatedSection = await homeScreenService.updateSection(sectionId, updates);
      
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: prev.template!.sections.map(s => 
            s.id === sectionId ? updatedSection : s
          )
        },
        selectedSection: prev.selectedSection?.id === sectionId 
          ? updatedSection 
          : prev.selectedSection,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Section updated successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to update section');
    }
  }, [showSuccess, showError]);

  // Delete section
  const deleteSection = useCallback(async (sectionId: string) => {
    setState(prev => ({ ...prev, isSaving: true }));
    try {
      await homeScreenService.deleteSection(sectionId);
      
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: prev.template!.sections.filter(s => s.id !== sectionId)
        },
        selectedSection: prev.selectedSection?.id === sectionId 
          ? null 
          : prev.selectedSection,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Section deleted successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to delete section');
    }
  }, [showSuccess, showError]);

  // Create component
  const createComponent = useCallback(async (
    sectionId: string,
    component: Partial<HomeScreenComponent>
  ) => {
    setState(prev => ({ ...prev, isSaving: true }));
    try {
      const section = state.template?.sections.find(s => s.id === sectionId);
      if (!section) throw new Error('Section not found');

      const newComponent = await homeScreenService.createComponent({
        ...component,
        sectionId,
        order: section.components.length
      });

      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: prev.template!.sections.map(s => 
            s.id === sectionId 
              ? { ...s, components: [...s.components, newComponent] }
              : s
          )
        },
        selectedComponent: newComponent,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Component added successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to add component');
    }
  }, [state.template, showSuccess, showError]);

  // Update component
  const updateComponent = useCallback(async (
    componentId: string,
    updates: Partial<HomeScreenComponent>
  ) => {
    setState(prev => ({ ...prev, isSaving: true }));
    try {
      const updatedComponent = await homeScreenService.updateComponent(componentId, updates);
      
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: prev.template!.sections.map(section => ({
            ...section,
            components: section.components.map(c => 
              c.id === componentId ? updatedComponent : c
            )
          }))
        },
        selectedComponent: prev.selectedComponent?.id === componentId 
          ? updatedComponent 
          : prev.selectedComponent,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Component updated successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to update component');
    }
  }, [showSuccess, showError]);

  // Delete component
  const deleteComponent = useCallback(async (componentId: string) => {
    setState(prev => ({ ...prev, isSaving: true }));
    try {
      await homeScreenService.deleteComponent(componentId);
      
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: prev.template!.sections.map(section => ({
            ...section,
            components: section.components.filter(c => c.id !== componentId)
          }))
        },
        selectedComponent: prev.selectedComponent?.id === componentId 
          ? null 
          : prev.selectedComponent,
        isSaving: false,
        hasUnsavedChanges: false
      }));

      showSuccess('Component deleted successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to delete component');
    }
  }, [showSuccess, showError]);

  // Reorder sections
  const reorderSections = useCallback(async (
    sections: Array<{ sectionId: string; newOrder: number }>
  ) => {
    if (!state.template) return;

    const originalSections = [...state.template.sections];
    
    // Optimistic update
    setState(prev => ({
      ...prev,
      template: {
        ...prev.template!,
        sections: sections
          .sort((a, b) => a.newOrder - b.newOrder)
          .map(({ sectionId }) => 
            prev.template!.sections.find(s => s.id === sectionId)!
          )
      },
      hasUnsavedChanges: true
    }));

    try {
      await homeScreenService.reorderSections(state.template.id, sections);
      setState(prev => ({ ...prev, hasUnsavedChanges: false }));
    } catch (error) {
      // Revert on error
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          sections: originalSections
        }
      }));
      showError('Failed to reorder sections');
    }
  }, [state.template, showError]);

  // Select section
  const selectSection = useCallback((section: HomeScreenSection | null) => {
    setState(prev => ({
      ...prev,
      selectedSection: section,
      selectedComponent: null
    }));
  }, []);

  // Select component
  const selectComponent = useCallback((component: HomeScreenComponent | null) => {
    setState(prev => ({
      ...prev,
      selectedComponent: component,
      selectedSection: component 
        ? prev.template?.sections.find(s => 
            s.components.some(c => c.id === component.id)
          ) || null
        : prev.selectedSection
    }));
  }, []);

  // Publish template
  const publishTemplate = useCallback(async (deactivateOthers = true) => {
    if (!state.template) return;

    setState(prev => ({ ...prev, isSaving: true }));
    try {
      await homeScreenService.publishTemplate(state.template.id, deactivateOthers);
      
      setState(prev => ({
        ...prev,
        template: {
          ...prev.template!,
          isActive: true,
          publishedAt: new Date()
        },
        isSaving: false
      }));

      showSuccess('Template published successfully');
    } catch (error) {
      setState(prev => ({ ...prev, isSaving: false }));
      showError('Failed to publish template');
    }
  }, [state.template, showSuccess, showError]);

  useEffect(() => {
    if (templateId) {
      loadTemplate(templateId);
    }
  }, [templateId, loadTemplate]);

  return {
    ...state,
    actions: {
      loadTemplate,
      createSection,
      updateSection,
      deleteSection,
      createComponent,
      updateComponent,
      deleteComponent,
      reorderSections,
      selectSection,
      selectComponent,
      publishTemplate
    }
  };
};
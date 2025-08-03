import { useState, useCallback, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';
import homeScreenService from '../services/homeScreenService';
import componentService from '../services/componentService';
import type {
  HomeScreenTemplate,
  HomeScreenSection,
  HomeScreenComponent,
  CreateHomeScreenTemplateCommand,
  UpdateHomeScreenTemplateCommand,
  CreateHomeScreenSectionCommand,
  UpdateHomeScreenSectionCommand,
  CreateHomeScreenComponentCommand,
  UpdateHomeScreenComponentCommand,
  ReorderSectionsCommand,
  ReorderComponentsCommand
} from '../types/homeScreen.types';
import { generateComponentId } from '../utils/componentFactory';

interface UseHomeScreenBuilderOptions {
  templateId?: string;
  autoSave?: boolean;
  autoSaveInterval?: number;
}

export const useHomeScreenBuilder = (options: UseHomeScreenBuilderOptions = {}) => {
  const { templateId, autoSave = true, autoSaveInterval = 30000 } = options;
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  
  const [selectedComponentId, setSelectedComponentId] = useState<string | null>(null);
  const [selectedSectionId, setSelectedSectionId] = useState<string | null>(null);
  const [isDirty, setIsDirty] = useState(false);
  const [lastSaveTime, setLastSaveTime] = useState<Date | null>(null);

  // Fetch template data
  const { data: template, isLoading: templateLoading, error: templateError } = useQuery<HomeScreenTemplate | null, Error>({
    queryKey: ['homeScreenTemplate', templateId],
    queryFn: () => templateId ? homeScreenService.getTemplateById(templateId) : null,
    enabled: !!templateId
  });

  // Fetch component types
  const { data: componentTypes = [] } = useQuery({
    queryKey: ['componentTypes'],
    queryFn: () => componentService.getComponentTypes()
  });

  // Template mutations
  const createTemplateMutation = useMutation({
    mutationFn: (command: CreateHomeScreenTemplateCommand) => 
      homeScreenService.createTemplate(command),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplates'] });
      toast.success('Template created successfully');
      navigate(`/home-screen-builder/${data.id}`);
    },
    onError: () => {
      toast.error('Failed to create template');
    }
  });

  const updateTemplateMutation = useMutation({
    mutationFn: ({ id, command }: { id: string; command: UpdateHomeScreenTemplateCommand }) =>
      homeScreenService.updateTemplate(id, command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      setIsDirty(false);
      setLastSaveTime(new Date());
      toast.success('Template updated successfully');
    },
    onError: () => {
      toast.error('Failed to update template');
    }
  });

  const deleteTemplateMutation = useMutation({
    mutationFn: (id: string) => homeScreenService.deleteTemplate(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplates'] });
      toast.success('Template deleted successfully');
      navigate('/home-screen-builder');
    },
    onError: () => {
      toast.error('Failed to delete template');
    }
  });

  const publishTemplateMutation = useMutation({
    mutationFn: ({ id, deactivateOthers }: { id: string; deactivateOthers: boolean }) =>
      homeScreenService.publishTemplate(id, deactivateOthers),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      toast.success('Template published successfully');
    },
    onError: () => {
      toast.error('Failed to publish template');
    }
  });

  // Section mutations
  const createSectionMutation = useMutation({
    mutationFn: (command: CreateHomeScreenSectionCommand) =>
      homeScreenService.createSection(command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      setIsDirty(true);
      toast.success('Section added successfully');
    },
    onError: () => {
      toast.error('Failed to add section');
    }
  });

  const updateSectionMutation = useMutation({
    mutationFn: ({ id, command }: { id: string; command: UpdateHomeScreenSectionCommand }) =>
      homeScreenService.updateSection(id, command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      setIsDirty(true);
    },
    onError: () => {
      toast.error('Failed to update section');
    }
  });

  const deleteSectionMutation = useMutation({
    mutationFn: (id: string) => homeScreenService.deleteSection(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      setIsDirty(true);
      toast.success('Section deleted successfully');
    },
    onError: () => {
      toast.error('Failed to delete section');
    }
  });

  const reorderSectionsMutation = useMutation({
    mutationFn: (command: ReorderSectionsCommand) =>
      homeScreenService.reorderSections(command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      setIsDirty(true);
    },
    onError: () => {
      toast.error('Failed to reorder sections');
    }
  });

  // Component mutations
  const createComponentMutation = useMutation({
    mutationFn: (command: CreateHomeScreenComponentCommand) =>
      homeScreenService.createComponent(command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      setIsDirty(true);
      toast.success('Component added successfully');
    },
    onError: () => {
      toast.error('Failed to add component');
    }
  });

  const updateComponentMutation = useMutation({
    mutationFn: ({ id, command }: { id: string; command: UpdateHomeScreenComponentCommand }) =>
      homeScreenService.updateComponent(id, command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      setIsDirty(true);
    },
    onError: () => {
      toast.error('Failed to update component');
    }
  });

  const deleteComponentMutation = useMutation({
    mutationFn: (id: string) => homeScreenService.deleteComponent(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      setIsDirty(true);
      setSelectedComponentId(null);
      toast.success('Component deleted successfully');
    },
    onError: () => {
      toast.error('Failed to delete component');
    }
  });

  const reorderComponentsMutation = useMutation({
    mutationFn: (command: ReorderComponentsCommand) =>
      homeScreenService.reorderComponents(command),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplate', templateId] });
      setIsDirty(true);
    },
    onError: () => {
      toast.error('Failed to reorder components');
    }
  });

  // Helper functions
  const addSection = useCallback(async (name: string, title: string) => {
    if (!templateId) return;
    
    const order = template?.sections?.length || 0;
    await createSectionMutation.mutateAsync({
      templateId,
      name,
      title,
      subtitle: '',
      order,
      backgroundColor: '#ffffff',
      backgroundImage: '',
      padding: '20px',
      margin: '0',
      minHeight: 100,
      maxHeight: 0,
      customStyles: '',
      conditions: ''
    });
  }, [templateId, template, createSectionMutation]);

  const addComponent = useCallback(async (
    sectionId: string,
    componentType: string,
    position?: { order: number }
  ) => {
    const section = template?.sections?.find(s => s.id === sectionId);
    if (!section) return;
    
    const order = position?.order ?? section.components.length;
    const componentTypeDefinition = componentTypes.find(ct => ct.type === componentType);
    
    await createComponentMutation.mutateAsync({
      sectionId,
      componentType,
      name: `${componentTypeDefinition?.name || componentType} ${Date.now()}`,
      order,
      colSpan: componentTypeDefinition?.defaultColSpan || 12,
      rowSpan: componentTypeDefinition?.defaultRowSpan || 1,
      alignment: 'left',
      customClasses: '',
      animationType: '',
      animationDuration: 0,
      conditions: ''
    });
  }, [template, componentTypes, createComponentMutation]);

  const moveComponent = useCallback(async (
    componentId: string,
    fromSectionId: string,
    toSectionId: string,
    newOrder: number
  ) => {
    if (fromSectionId === toSectionId) {
      // Reorder within same section
      const section = template?.sections?.find(s => s.id === fromSectionId);
      if (!section) return;
      
      const components = section.components.map((comp, index) => ({
        componentId: comp.id,
        newOrder: comp.id === componentId ? newOrder : 
          index < newOrder ? index : index + 1
      }));
      
      await reorderComponentsMutation.mutateAsync({
        sectionId: fromSectionId,
        components
      });
    } else {
      // Move to different section
      const component = template?.sections
        ?.find(s => s.id === fromSectionId)
        ?.components.find(c => c.id === componentId);
      
      if (!component) return;
      
      // Delete from current section
      await deleteComponentMutation.mutateAsync(componentId);
      
      // Create in new section
      await createComponentMutation.mutateAsync({
        sectionId: toSectionId,
        componentType: component.componentType,
        name: component.name,
        order: newOrder,
        colSpan: component.colSpan,
        rowSpan: component.rowSpan,
        alignment: component.alignment,
        customClasses: component.customClasses || '',
        animationType: component.animationType || '',
        animationDuration: component.animationDuration || 0,
        conditions: component.conditions || ''
      });
    }
  }, [template, reorderComponentsMutation, deleteComponentMutation, createComponentMutation]);

  const duplicateComponent = useCallback(async (componentId: string) => {
    const component = template?.sections
      ?.flatMap(s => s.components)
      .find(c => c.id === componentId);
    
    if (!component) return;
    
    const section = template?.sections.find(s => 
      s.components.some(c => c.id === componentId)
    );
    
    if (!section) return;
    
    await createComponentMutation.mutateAsync({
      sectionId: section.id,
      componentType: component.componentType,
      name: `${component.name} (Copy)`,
      order: component.order + 1,
      colSpan: component.colSpan,
      rowSpan: component.rowSpan,
      alignment: component.alignment,
      customClasses: component.customClasses || '',
      animationType: component.animationType || '',
      animationDuration: component.animationDuration || 0,
      conditions: component.conditions || ''
    });
  }, [template, createComponentMutation]);

  const selectComponent = useCallback((componentId: string | null) => {
    setSelectedComponentId(componentId);
    if (componentId) {
      const section = template?.sections.find(s =>
        s.components.some(c => c.id === componentId)
      );
      setSelectedSectionId(section?.id || null);
    }
  }, [template]);

  const getSelectedComponent = useCallback((): HomeScreenComponent | null => {
    if (!selectedComponentId || !template) return null;
    
    return template.sections
      .flatMap(s => s.components)
      .find(c => c.id === selectedComponentId) || null;
  }, [selectedComponentId, template]);

  const getSelectedSection = useCallback((): HomeScreenSection | null => {
    if (!selectedSectionId || !template) return null;
    
    return template.sections.find(s => s.id === selectedSectionId) || null;
  }, [selectedSectionId, template]);

  const saveTemplate = useCallback(async () => {
    if (!template || !templateId) return;
    
    await updateTemplateMutation.mutateAsync({
      id: templateId,
      command: {
        name: template.name,
        description: template.description,
        metaData: template.metaData || ''
      }
    });
  }, [template, templateId, updateTemplateMutation]);

  // Auto-save functionality
  useEffect(() => {
    if (!autoSave || !isDirty || !template) return;
    
    const timer = setTimeout(() => {
      saveTemplate();
    }, autoSaveInterval);
    
    return () => clearTimeout(timer);
  }, [autoSave, isDirty, template, autoSaveInterval, saveTemplate]);

  // Keyboard shortcuts
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      // Save: Ctrl/Cmd + S
      if ((e.ctrlKey || e.metaKey) && e.key === 's') {
        e.preventDefault();
        saveTemplate();
      }
      
      // Delete: Delete key
      if (e.key === 'Delete' && selectedComponentId) {
        e.preventDefault();
        deleteComponentMutation.mutate(selectedComponentId);
      }
      
      // Duplicate: Ctrl/Cmd + D
      if ((e.ctrlKey || e.metaKey) && e.key === 'd' && selectedComponentId) {
        e.preventDefault();
        duplicateComponent(selectedComponentId);
      }
      
      // Deselect: Escape
      if (e.key === 'Escape') {
        e.preventDefault();
        selectComponent(null);
        setSelectedSectionId(null);
      }
    };
    
    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [selectedComponentId, saveTemplate, deleteComponentMutation, duplicateComponent, selectComponent]);

  return {
    // State
    template,
    componentTypes,
    selectedComponentId,
    selectedSectionId,
    isDirty,
    lastSaveTime,
    isLoading: templateLoading,
    error: templateError,
    
    // Template actions
    createTemplate: createTemplateMutation.mutate,
    updateTemplate: updateTemplateMutation.mutate,
    deleteTemplate: deleteTemplateMutation.mutate,
    publishTemplate: publishTemplateMutation.mutate,
    saveTemplate,
    
    // Section actions
    addSection,
    updateSection: updateSectionMutation.mutate,
    deleteSection: deleteSectionMutation.mutate,
    reorderSections: reorderSectionsMutation.mutate,
    
    // Component actions
    addComponent,
    updateComponent: updateComponentMutation.mutate,
    deleteComponent: deleteComponentMutation.mutate,
    moveComponent,
    duplicateComponent,
    
    // Selection
    selectComponent,
    getSelectedComponent,
    getSelectedSection,
    
    // Mutation states
    isSaving: updateTemplateMutation.isPending,
    isCreating: createTemplateMutation.isPending,
    isPublishing: publishTemplateMutation.isPending
  };
};
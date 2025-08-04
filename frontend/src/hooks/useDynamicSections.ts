// frontend/src/hooks/useDynamicSections.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import homeSectionsService from '../services/homeSectionsService';
import type {
  DynamicHomeSection,
  CreateDynamicSectionCommand,
  UpdateDynamicSectionCommand,
  ToggleSectionStatusCommand,
  DeleteDynamicSectionCommand,
  ReorderDynamicSectionsCommand
} from '../types/homeSections.types';

// Fetch dynamic home sections
export const useDynamicHomeSections = (params?: {
  language?: string;
  targetAudience?: string[];
  includeContent?: boolean;
  onlyActive?: boolean;
}) => {
  return useQuery<DynamicHomeSection[], Error>({
    queryKey: ['dynamicHomeSections', params],
    queryFn: () => homeSectionsService.getDynamicSections(params),
  });
};

// Create dynamic home section
export const useCreateDynamicSection = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (command: CreateDynamicSectionCommand) => homeSectionsService.createDynamicSection(command),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['dynamicHomeSections'] }),
  });
};

// Update dynamic section
export const useUpdateDynamicSection = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, command }: { id: string; command: UpdateDynamicSectionCommand }) =>
      homeSectionsService.updateDynamicSection(id, command),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['dynamicHomeSections'] }),
  });
};

// Toggle section status
export const useToggleDynamicSection = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, setActive }: ToggleSectionStatusCommand) =>
      homeSectionsService.toggleDynamicSection(id, setActive),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['dynamicHomeSections'] }),
  });
};

// Delete section
export const useDeleteDynamicSection = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => homeSectionsService.deleteDynamicSection(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['dynamicHomeSections'] }),
  });
};

// Reorder sections
export const useReorderDynamicSections = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (command: ReorderDynamicSectionsCommand) => homeSectionsService.reorderDynamicSections(command),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['dynamicHomeSections'] }),
  });
};
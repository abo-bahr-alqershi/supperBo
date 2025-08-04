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
  return useQuery(['dynamicHomeSections', params], () => homeSectionsService.getDynamicSections(params));
};

// Create dynamic home section
export const useCreateDynamicSection = () => {
  const queryClient = useQueryClient();
  return useMutation(
    (command: CreateDynamicSectionCommand) => homeSectionsService.createDynamicSection(command),
    {
      onSuccess: () => queryClient.invalidateQueries(['dynamicHomeSections']),
    }
  );
};

// Update dynamic section
export const useUpdateDynamicSection = () => {
  const queryClient = useQueryClient();
  return useMutation(
    ({ id, command }: { id: string; command: UpdateDynamicSectionCommand }) =>
      homeSectionsService.updateDynamicSection(id, command),
    {
      onSuccess: () => queryClient.invalidateQueries(['dynamicHomeSections']),
    }
  );
};

// Toggle section status
export const useToggleDynamicSection = () => {
  const queryClient = useQueryClient();
  return useMutation(
    ({ id, setActive }: ToggleSectionStatusCommand) =>
      homeSectionsService.toggleDynamicSection(id, setActive),
    {
      onSuccess: () => queryClient.invalidateQueries(['dynamicHomeSections']),
    }
  );
};

// Delete section
export const useDeleteDynamicSection = () => {
  const queryClient = useQueryClient();
  return useMutation(
    (id: string) => homeSectionsService.deleteDynamicSection(id),
    {
      onSuccess: () => queryClient.invalidateQueries(['dynamicHomeSections']),
    }
  );
};

// Reorder sections
export const useReorderDynamicSections = () => {
  const queryClient = useQueryClient();
  return useMutation(
    (command: ReorderDynamicSectionsCommand) => homeSectionsService.reorderDynamicSections(command),
    {
      onSuccess: () => queryClient.invalidateQueries(['dynamicHomeSections']),
    }
  );
};
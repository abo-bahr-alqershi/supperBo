// frontend/src/hooks/useDynamicSections.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import homeSectionsService from '../services/homeSectionsService';
import type {
  DynamicHomeSection,
  CreateDynamicSectionCommand,
  UpdateDynamicSectionCommand,
  ToggleSectionStatusCommand,
  DeleteDynamicSectionCommand,
  ReorderDynamicSectionsCommand,
  DynamicHomeConfig,
  CityDestination,
  SponsoredAd
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

// Add hooks for city destinations, sponsored ads, and home config
export const useCityDestinations = (params?: {
  language?: string;
  onlyActive?: boolean;
  onlyPopular?: boolean;
  onlyFeatured?: boolean;
  limit?: number;
  sortBy?: string;
}) => {
  return useQuery<CityDestination[], Error>({
    queryKey: ['cityDestinations', params],
    queryFn: () => homeSectionsService.getCityDestinations(params),
  });
};

export const useSponsoredAds = (params?: {
  onlyActive?: boolean;
  limit?: number;
  includePropertyDetails?: boolean;
  targetAudience?: string[];
}) => {
  return useQuery<SponsoredAd[], Error>({
    queryKey: ['sponsoredAds', params],
    queryFn: () => homeSectionsService.getSponsoredAds(params),
  });
};

export const useHomeConfig = (version?: string) => {
  return useQuery<DynamicHomeConfig, Error>({
    queryKey: ['homeConfig', version],
    queryFn: () => homeSectionsService.getHomeConfig(version),
  });
};
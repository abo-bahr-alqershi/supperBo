// frontend/src/hooks/useSponsoredAds.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import homeSectionsService from '../services/homeSectionsService';
import type {
  SponsoredAd,
  CreateSponsoredAdCommand,
  UpdateSponsoredAdCommand,
  RecordAdInteractionCommand
} from '../types/homeSections.types';

// Fetch sponsored ads
export const useSponsoredAds = (params?: {
  onlyActive?: boolean;
  limit?: number;
  includePropertyDetails?: boolean;
  targetAudience?: string[];
}) => {
  return useQuery(['sponsoredAds', params], () => homeSectionsService.getSponsoredAds(params));
};

// Create sponsored ad
export const useCreateSponsoredAd = () => {
  const queryClient = useQueryClient();
  return useMutation(
    (command: CreateSponsoredAdCommand) => homeSectionsService.createSponsoredAd(command),
    {
      onSuccess: () => queryClient.invalidateQueries(['sponsoredAds']),
    }
  );
};

// Update sponsored ad
export const useUpdateSponsoredAd = () => {
  const queryClient = useQueryClient();
  return useMutation(
    ({ id, command }: { id: string; command: UpdateSponsoredAdCommand }) =>
      homeSectionsService.updateSponsoredAd(id, command),
    {
      onSuccess: () => queryClient.invalidateQueries(['sponsoredAds']),
    }
  );
};

// Record ad interaction (impression/click)
export const useRecordAdInteraction = () => {
  return useMutation(
    (command: RecordAdInteractionCommand) => homeSectionsService.recordAdInteraction(command)
  );
};
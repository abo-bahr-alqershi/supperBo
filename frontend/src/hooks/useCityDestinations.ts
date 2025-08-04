// frontend/src/hooks/useCityDestinations.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import homeSectionsService from '../services/homeSectionsService';
import type {
  CityDestination,
  CreateCityDestinationCommand,
  UpdateCityDestinationCommand,
  UpdateCityDestinationStatsCommand
} from '../types/homeSections.types';

// Fetch city destinations
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

// Create city destination
export const useCreateCityDestination = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (command: CreateCityDestinationCommand) => homeSectionsService.createCityDestination(command),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['cityDestinations'] }),
  });
};

// Update city destination
export const useUpdateCityDestination = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, command }: { id: string; command: UpdateCityDestinationCommand }) =>
      homeSectionsService.updateCityDestination(id, command),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['cityDestinations'] }),
  });
};

// Update city destination stats
export const useUpdateCityDestinationStats = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (command: UpdateCityDestinationStatsCommand) =>
      homeSectionsService.updateCityDestinationStats(command),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['cityDestinations'] }),
  });
};
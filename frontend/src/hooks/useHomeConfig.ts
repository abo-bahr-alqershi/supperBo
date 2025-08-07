// frontend/src/hooks/useHomeConfig.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import homeSectionsService from '../services/homeSectionsService';
import type {
  DynamicHomeConfig,
  CreateDynamicConfigCommand,
  UpdateDynamicConfigCommand,
  PublishDynamicConfigCommand
} from '../types/homeSections.types';

// Fetch home config
export const useHomeConfig = (version?: string) => {
  return useQuery<DynamicHomeConfig, Error>({
    queryKey: ['homeConfig', version],
    queryFn: () => homeSectionsService.getClientHomeConfig(version),
  });
};

// Create home config
export const useCreateHomeConfig = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (command: CreateDynamicConfigCommand) => homeSectionsService.createHomeConfig(command),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['homeConfig'] }),
  });
};

// Update home config
export const useUpdateHomeConfig = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, command }: { id: string; command: UpdateDynamicConfigCommand }) =>
      homeSectionsService.updateHomeConfig(id, command),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['homeConfig'] }),
  });
};

// Publish home config
export const usePublishHomeConfig = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (command: PublishDynamicConfigCommand) => homeSectionsService.publishHomeConfig(command.id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['homeConfig'] }),
  });
};
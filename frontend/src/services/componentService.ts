import { apiClient } from './api.service';
import type { ComponentTypeDefinition } from '../types/component.types';
import type { HomeScreenComponent, CreateHomeScreenComponentCommand, UpdateHomeScreenComponentCommand, ReorderComponentsCommand } from '../types/homeScreen.types';

const API_BASE_URL = '/api';

export class ComponentService {
  async getComponentTypes(platform?: string): Promise<ComponentTypeDefinition[]> {
    const response = await apiClient.get(`${API_BASE_URL}/home-screens/component-types`, {
      params: { platform }
    });
    return response.data.data;
  }

  // CRUD and reorder operations
  async createComponent(command: CreateHomeScreenComponentCommand): Promise<string> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screens/components`, command);
    return response.data.data;
  }

  async updateComponent(id: string, command: UpdateHomeScreenComponentCommand): Promise<HomeScreenComponent> {
    const response = await apiClient.put(`${API_BASE_URL}/home-screens/components/${id}`, command);
    return response.data.data;
  }

  async deleteComponent(id: string): Promise<boolean> {
    const response = await apiClient.delete(`${API_BASE_URL}/home-screens/components/${id}`);
    return response.data.data;
  }

  async reorderComponents(command: ReorderComponentsCommand): Promise<boolean> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screens/components/reorder`, command);
    return response.data.data;
  }
}
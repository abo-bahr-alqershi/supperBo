import { apiClient } from './api.service';
import type { ComponentTypeDefinition } from '../types/component.types';
import type { ComponentProperty, ComponentStyle, ComponentAction, ComponentDataSource } from '../types/homeScreen.types';
import type { HomeScreenComponent } from '../types/homeScreen.types';

const API_BASE_URL = '/api';

export class ComponentService {
  async getComponentTypes(platform?: string): Promise<ComponentTypeDefinition[]> {
    const response = await apiClient.get(`${API_BASE_URL}/home-screens/component-types`, {
      params: { platform }
    });
    return response.data;
  }

  async updateComponentProperty(componentId: string, propertyId: string, value: any): Promise<void> {
    await apiClient.put(`${API_BASE_URL}/home-screens/components/${componentId}/properties/${propertyId}`, {
      value
    });
  }

  async addComponentStyle(componentId: string, style: Partial<ComponentStyle>): Promise<ComponentStyle> {
    const response = await apiClient.post(
      `${API_BASE_URL}/home-screens/components/${componentId}/styles`, 
      style
    );
    return response.data;
  }

  async updateComponentStyle(componentId: string, styleId: string, updates: Partial<ComponentStyle>): Promise<void> {
    await apiClient.put(
      `${API_BASE_URL}/home-screens/components/${componentId}/styles/${styleId}`, 
      updates
    );
  }

  async deleteComponentStyle(componentId: string, styleId: string): Promise<void> {
    await apiClient.delete(`${API_BASE_URL}/home-screens/components/${componentId}/styles/${styleId}`);
  }

  async addComponentAction(componentId: string, action: Partial<ComponentAction>): Promise<ComponentAction> {
    const response = await apiClient.post(
      `${API_BASE_URL}/home-screens/components/${componentId}/actions`, 
      action
    );
    return response.data;
  }

  async updateComponentAction(componentId: string, actionId: string, updates: Partial<ComponentAction>): Promise<void> {
    await apiClient.put(
      `${API_BASE_URL}/home-screens/components/${componentId}/actions/${actionId}`, 
      updates
    );
  }

  async deleteComponentAction(componentId: string, actionId: string): Promise<void> {
    await apiClient.delete(`${API_BASE_URL}/home-screens/components/${componentId}/actions/${actionId}`);
  }

  async setComponentDataSource(componentId: string, dataSource: Partial<ComponentDataSource>): Promise<ComponentDataSource> {
    const response = await apiClient.post(
      `${API_BASE_URL}/home-screens/components/${componentId}/data-source`, 
      dataSource
    );
    return response.data;
  }

  // Added methods for component CRUD and reorder operations
  async createComponent(component: Partial<HomeScreenComponent>): Promise<string> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screens/components`, component);
    return response.data;
  }

  async updateComponent(id: string, updates: Partial<HomeScreenComponent>): Promise<HomeScreenComponent> {
    const response = await apiClient.put(`${API_BASE_URL}/home-screens/components/${id}`, updates);
    return response.data;
  }

  async deleteComponent(id: string): Promise<void> {
    await apiClient.delete(`${API_BASE_URL}/home-screens/components/${id}`);
  }

  async reorderComponents(sectionId: string, components: Array<{ componentId: string; newOrder: number }>): Promise<void> {
    await apiClient.post(`${API_BASE_URL}/home-screens/components/reorder`, { sectionId, components });
  }
}
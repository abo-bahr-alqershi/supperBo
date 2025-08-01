import { apiClient } from './api.service';
import type { ComponentTypeDefinition } from '../types/component.types';
import type { ComponentProperty, ComponentStyle, ComponentAction, ComponentDataSource } from '../types/homeScreen.types';

const API_BASE_URL = '/api';

export class ComponentService {
  async getComponentTypes(platform?: string): Promise<ComponentTypeDefinition[]> {
    const response = await axios.get(`${API_BASE_URL}/home-screen/component-types`, {
      params: { platform }
    });
    return response.data;
  }

  async updateComponentProperty(componentId: string, propertyId: string, value: any): Promise<void> {
    await axios.put(`${API_BASE_URL}/home-screen/components/${componentId}/properties/${propertyId}`, {
      value
    });
  }

  async addComponentStyle(componentId: string, style: Partial<ComponentStyle>): Promise<ComponentStyle> {
    const response = await axios.post(
      `${API_BASE_URL}/home-screen/components/${componentId}/styles`, 
      style
    );
    return response.data;
  }

  async updateComponentStyle(componentId: string, styleId: string, updates: Partial<ComponentStyle>): Promise<void> {
    await axios.put(
      `${API_BASE_URL}/home-screen/components/${componentId}/styles/${styleId}`, 
      updates
    );
  }

  async deleteComponentStyle(componentId: string, styleId: string): Promise<void> {
    await axios.delete(`${API_BASE_URL}/home-screen/components/${componentId}/styles/${styleId}`);
  }

  async addComponentAction(componentId: string, action: Partial<ComponentAction>): Promise<ComponentAction> {
    const response = await axios.post(
      `${API_BASE_URL}/home-screen/components/${componentId}/actions`, 
      action
    );
    return response.data;
  }

  async updateComponentAction(componentId: string, actionId: string, updates: Partial<ComponentAction>): Promise<void> {
    await axios.put(
      `${API_BASE_URL}/home-screen/components/${componentId}/actions/${actionId}`, 
      updates
    );
  }

  async deleteComponentAction(componentId: string, actionId: string): Promise<void> {
    await axios.delete(`${API_BASE_URL}/home-screen/components/${componentId}/actions/${actionId}`);
  }

  async setComponentDataSource(componentId: string, dataSource: Partial<ComponentDataSource>): Promise<ComponentDataSource> {
    const response = await axios.post(
      `${API_BASE_URL}/home-screen/components/${componentId}/data-source`, 
      dataSource
    );
    return response.data;
  }
import { apiClient } from './api.service';
import type {
  HomeScreenTemplate,
  HomeScreenSection,
  HomeScreenComponent
} from '../types/homeScreen.types';

const API_BASE_URL = '/api';

export class HomeScreenService {
  // Template operations
  async getTemplates(params?: {
    platform?: string;
    targetAudience?: string;
    isActive?: boolean;
    includeDeleted?: boolean;
  }): Promise<HomeScreenTemplate[]> {
    const response = await apiClient.get(`${API_BASE_URL}/home-screen/templates`, { params });
    return response.data;
  }

  async getTemplateById(id: string, includeHierarchy = true): Promise<HomeScreenTemplate> {
    const response = await apiClient.get(`${API_BASE_URL}/home-screen/templates/${id}`, {
      params: { includeHierarchy }
    });
    return response.data;
  }

  async createTemplate(template: Partial<HomeScreenTemplate>): Promise<HomeScreenTemplate> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screen/templates`, template);
    return response.data;
  }

  async updateTemplate(id: string, updates: Partial<HomeScreenTemplate>): Promise<HomeScreenTemplate> {
    const response = await apiClient.put(`${API_BASE_URL}/home-screen/templates/${id}`, updates);
    return response.data;
  }

  async deleteTemplate(id: string): Promise<void> {
    await apiClient.delete(`${API_BASE_URL}/home-screen/templates/${id}`);
  }

  async duplicateTemplate(id: string, newName: string, newDescription?: string): Promise<HomeScreenTemplate> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screen/templates/${id}/duplicate`, {
      newName,
      newDescription
    });
    return response.data;
  }

  async publishTemplate(id: string, deactivateOthers = true): Promise<void> {
    await apiClient.post(`${API_BASE_URL}/home-screen/templates/${id}/publish`, {
      deactivateOthers
    });
  }

  // Section operations
  async createSection(section: Partial<HomeScreenSection>): Promise<HomeScreenSection> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screen/sections`, section);
    return response.data;
  }

  async updateSection(id: string, updates: Partial<HomeScreenSection>): Promise<HomeScreenSection> {
    const response = await apiClient.put(`${API_BASE_URL}/home-screen/sections/${id}`, updates);
    return response.data;
  }

  async deleteSection(id: string): Promise<void> {
    await apiClient.delete(`${API_BASE_URL}/home-screen/sections/${id}`);
  }

  async reorderSections(templateId: string, sections: Array<{ sectionId: string; newOrder: number }>): Promise<void> {
    await apiClient.post(`${API_BASE_URL}/home-screen/templates/${templateId}/reorder-sections`, {
      sections
    });
  }

  // Component operations
  async createComponent(component: Partial<HomeScreenComponent>): Promise<HomeScreenComponent> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screen/components`, component);
    return response.data;
  }

  async updateComponent(id: string, updates: Partial<HomeScreenComponent>): Promise<HomeScreenComponent> {
    const response = await apiClient.put(`${API_BASE_URL}/home-screen/components/${id}`, updates);
    return response.data;
  }

  async deleteComponent(id: string): Promise<void> {
    await apiClient.delete(`${API_BASE_URL}/home-screen/components/${id}`);
  }

  async reorderComponents(sectionId: string, components: Array<{ componentId: string; newOrder: number }>): Promise<void> {
    await apiClient.post(`${API_BASE_URL}/home-screen/sections/${sectionId}/reorder-components`, {
      components
    });
  }

  // Preview
  async previewTemplate(templateId: string, options?: {
    platform?: string;
    deviceType?: string;
    useMockData?: boolean;
  }): Promise<any> {
    const response = await apiClient.get(`${API_BASE_URL}/home-screen/templates/${templateId}/preview`, {
      params: options
    });
    return response.data;
  }

  // Active home screen
  async getActiveHomeScreen(platform: string, targetAudience: string): Promise<HomeScreenTemplate> {
    const response = await apiClient.get(`${API_BASE_URL}/home-screen/active`, {
      params: { platform, targetAudience }
    });
    return response.data;
  }
}

export default new HomeScreenService();

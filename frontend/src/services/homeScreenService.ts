import { apiClient } from './api.service';
import type {
  HomeScreenTemplate,
  HomeScreenSection,
  HomeScreenComponent,
  CreateHomeScreenTemplateCommand,
  UpdateHomeScreenTemplateCommand,
  CreateHomeScreenSectionCommand,
  UpdateHomeScreenSectionCommand,
  CreateHomeScreenComponentCommand,
  UpdateHomeScreenComponentCommand,
  ReorderSectionsCommand,
  ReorderComponentsCommand,
  HomeScreenPreviewDto
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
    const { platform = 'All', targetAudience = 'All', isActive, includeDeleted } = params || {};
    const response = await apiClient.get(`${API_BASE_URL}/home-screens/templates`, {
      params: { platform, targetAudience, isActive, includeDeleted }
    });
    return response.data.data;
  }

  async getTemplateById(id: string, includeHierarchy = true): Promise<HomeScreenTemplate> {
    const response = await apiClient.get(`${API_BASE_URL}/home-screens/templates/${id}`, {
      params: { includeHierarchy }
    });
    return response.data.data;
  }

  async createTemplate(command: CreateHomeScreenTemplateCommand): Promise<HomeScreenTemplate> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screens/templates`, command);
    return response.data.data;
  }

  async updateTemplate(id: string, command: UpdateHomeScreenTemplateCommand): Promise<HomeScreenTemplate> {
    const response = await apiClient.put(`${API_BASE_URL}/home-screens/templates/${id}`, command);
    return response.data.data;
  }

  async deleteTemplate(id: string): Promise<boolean> {
    const response = await apiClient.delete(`${API_BASE_URL}/home-screens/templates/${id}`);
    return response.data.data;
  }

  async duplicateTemplate(id: string, newName: string, newDescription?: string): Promise<HomeScreenTemplate> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screens/templates/${id}/duplicate`, null, {
      params: { newName, newDescription }
    });
    return response.data.data;
  }

  async publishTemplate(id: string, deactivateOthers = true): Promise<boolean> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screens/templates/${id}/publish`, null, {
      params: { deactivateOthers }
    });
    return response.data.data;
  }

  // Section operations
  async createSection(command: CreateHomeScreenSectionCommand): Promise<HomeScreenSection> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screens/sections`, command);
    return response.data.data;
  }

  async updateSection(id: string, command: UpdateHomeScreenSectionCommand): Promise<HomeScreenSection> {
    const response = await apiClient.put(`${API_BASE_URL}/home-screens/sections/${id}`, command);
    return response.data.data;
  }

  async deleteSection(id: string): Promise<boolean> {
    const response = await apiClient.delete(`${API_BASE_URL}/home-screens/sections/${id}`);
    return response.data.data;
  }

  async reorderSections(command: ReorderSectionsCommand): Promise<boolean> {
    const response = await apiClient.post(`${API_BASE_URL}/home-screens/sections/reorder`, command);
    return response.data.data;
  }

  // Component operations
  async createComponent(command: CreateHomeScreenComponentCommand): Promise<HomeScreenComponent> {
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

  // Preview
  async previewTemplate(templateId: string, options?: {
    platform?: string;
    deviceType?: string;
    useMockData?: boolean;
  }): Promise<HomeScreenPreviewDto> {
    const response = await apiClient.get(`${API_BASE_URL}/home-screens/preview`, {
      params: { templateId, ...options }
    });
    return response.data.data;
  }

  // Active home screen
  async getActiveHomeScreen(platform: string, targetAudience: string, userId?: string): Promise<HomeScreenTemplate> {
    const response = await apiClient.get(`${API_BASE_URL}/home-screens/active`, {
      params: { platform, targetAudience, userId }
    });
    return response.data.data;
  }
}

export default new HomeScreenService();

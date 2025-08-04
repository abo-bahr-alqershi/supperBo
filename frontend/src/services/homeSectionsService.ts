// frontend/src/services/homeSectionsService.ts
import { apiClient } from './api.service';
import type {
  DynamicHomeSection,
  CreateDynamicSectionCommand,
  UpdateDynamicSectionCommand,
  ToggleSectionStatusCommand,
  DeleteDynamicSectionCommand,
  ReorderDynamicSectionsCommand,
  DynamicHomeConfig,
  CreateDynamicConfigCommand,
  UpdateDynamicConfigCommand,
  PublishDynamicConfigCommand,
  CityDestination,
  CreateCityDestinationCommand,
  UpdateCityDestinationCommand,
  UpdateCityDestinationStatsCommand,
  SponsoredAd,
  CreateSponsoredAdCommand,
  UpdateSponsoredAdCommand,
  RecordAdInteractionCommand
} from '../types/homeSections.types';

const API_BASE = '/api';

class HomeSectionsService {
  // Dynamic Sections (Admin)
  async getDynamicSections(params?: {
    language?: string;
    targetAudience?: string[];
    includeContent?: boolean;
    onlyActive?: boolean;
  }): Promise<DynamicHomeSection[]> {
    const response = await apiClient.get(`${API_BASE}/admin/home-sections/dynamic-sections`, { params });
    return response.data;
  }

  async createDynamicSection(command: CreateDynamicSectionCommand): Promise<string> {
    const response = await apiClient.post(`${API_BASE}/admin/home-sections/dynamic-sections`, command);
    return response.data;
  }

  async updateDynamicSection(id: string, command: UpdateDynamicSectionCommand): Promise<boolean> {
    const response = await apiClient.put(`${API_BASE}/admin/home-sections/dynamic-sections/${id}`, command);
    return response.data;
  }

  async toggleDynamicSection(id: string, setActive?: boolean): Promise<boolean> {
    const response = await apiClient.post(`${API_BASE}/admin/home-sections/dynamic-sections/${id}/toggle`, null, { params: { setActive } });
    return response.data;
  }

  async deleteDynamicSection(id: string): Promise<boolean> {
    const response = await apiClient.delete(`${API_BASE}/admin/home-sections/dynamic-sections/${id}`);
    return response.data;
  }

  async reorderDynamicSections(command: ReorderDynamicSectionsCommand): Promise<boolean> {
    const response = await apiClient.post(`${API_BASE}/admin/home-sections/dynamic-sections/reorder`, command);
    return response.data;
  }

  // Dynamic Config (Admin)
  async getHomeConfig(version?: string): Promise<DynamicHomeConfig> {
    const response = await apiClient.get(`${API_BASE}/admin/home-sections/dynamic-config`, { params: { version } });
    return response.data;
  }

  async createHomeConfig(command: CreateDynamicConfigCommand): Promise<string> {
    const response = await apiClient.post(`${API_BASE}/admin/home-sections/dynamic-config`, command);
    return response.data;
  }

  async updateHomeConfig(id: string, command: UpdateDynamicConfigCommand): Promise<boolean> {
    const response = await apiClient.put(`${API_BASE}/admin/home-sections/dynamic-config/${id}`, command);
    return response.data;
  }

  async publishHomeConfig(id: string): Promise<boolean> {
    const response = await apiClient.post(`${API_BASE}/admin/home-sections/dynamic-config/${id}/publish`);
    return response.data;
  }

  // City Destinations (Admin)
  async getCityDestinations(params?: {
    language?: string;
    onlyActive?: boolean;
    onlyPopular?: boolean;
    onlyFeatured?: boolean;
    limit?: number;
    sortBy?: string;
  }): Promise<CityDestination[]> {
    const response = await apiClient.get(`${API_BASE}/admin/home-sections/city-destinations`, { params });
    return response.data;
  }

  async createCityDestination(command: CreateCityDestinationCommand): Promise<string> {
    const response = await apiClient.post(`${API_BASE}/admin/home-sections/city-destinations`, command);
    return response.data;
  }

  async updateCityDestination(id: string, command: UpdateCityDestinationCommand): Promise<boolean> {
    const response = await apiClient.put(`${API_BASE}/admin/home-sections/city-destinations/${id}`, command);
    return response.data;
  }

  async updateCityDestinationStats(command: UpdateCityDestinationStatsCommand): Promise<boolean> {
    const { id, ...body } = command;
    const response = await apiClient.put(`${API_BASE}/admin/home-sections/city-destinations/${id}/stats`, body);
    return response.data;
  }

  // Sponsored Ads (Admin)
  async getSponsoredAds(params?: { onlyActive?: boolean; limit?: number; includePropertyDetails?: boolean; targetAudience?: string[] }): Promise<SponsoredAd[]> {
    const response = await apiClient.get(`${API_BASE}/admin/home-sections/sponsored-ads`, { params });
    return response.data;
  }

  async createSponsoredAd(command: CreateSponsoredAdCommand): Promise<string> {
    const response = await apiClient.post(`${API_BASE}/admin/home-sections/sponsored-ads`, command);
    return response.data;
  }

  async updateSponsoredAd(id: string, command: UpdateSponsoredAdCommand): Promise<boolean> {
    const response = await apiClient.put(`${API_BASE}/admin/home-sections/sponsored-ads/${id}`, command);
    return response.data;
  }

  async recordAdInteraction(command: RecordAdInteractionCommand): Promise<boolean> {
    const { adId, interactionType, ...body } = command;
    const response = await apiClient.post(`${API_BASE}/client/homeSections/sponsored-ads/${adId}/${interactionType}`, body);
    return response.data;
  }
}

export default new HomeSectionsService();
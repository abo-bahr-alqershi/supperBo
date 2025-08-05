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

// Base URLs for Home Sections admin and client endpoints
const ADMIN_BASE = '/api/admin/home-sections';
const CLIENT_BASE = '/api/client/home-sections';

class HomeSectionsService {
  // Dynamic Sections (Admin)
  async getDynamicSections(params?: {
    language?: string;
    targetAudience?: string[];
    includeContent?: boolean;
    onlyActive?: boolean;
  }): Promise<DynamicHomeSection[]> {
    const response = await apiClient.get(`${ADMIN_BASE}/dynamic-sections`, { params });
    const rawList: any[] = response.data;
    return rawList.map(raw => {
      const { type, content, ...rest } = raw;
      return {
        ...rest,
        sectionType: type,
        content: content.map((c: any) => ({ ...c, contentData: c.data, metadata: c.metadata }))
      } as DynamicHomeSection;
    });
  }

  async createDynamicSection(command: CreateDynamicSectionCommand): Promise<string> {
    const payload = {
      ...command,
      sectionConfig: JSON.stringify(command.sectionConfig || {}),
      metadata: JSON.stringify(command.metadata || {}),
      content: command.content?.map(c => ({
        contentType: c.contentType,
        contentData: JSON.stringify(c.contentData),
        metadata: JSON.stringify(c.metadata || {}),
        displayOrder: c.displayOrder,
        expiresAt: c.expiresAt
      }))
    };
    const response = await apiClient.post(`${ADMIN_BASE}/dynamic-sections`, payload);
    return response.data;
  }

  async updateDynamicSection(id: string, command: UpdateDynamicSectionCommand): Promise<boolean> {
    const payload = {
      ...command,
      sectionConfig: JSON.stringify(command.sectionConfig || {}),
      metadata: JSON.stringify(command.metadata || {}),
      content: command.content?.map(c => ({
        id: c.id,
        contentType: c.contentType,
        contentData: JSON.stringify(c.contentData),
        metadata: JSON.stringify(c.metadata || {}),
        displayOrder: c.displayOrder,
        expiresAt: c.expiresAt,
        isDeleted: c.isDeleted
      }))
    };
    const response = await apiClient.put(`${ADMIN_BASE}/dynamic-sections/${id}`, payload);
    return response.data;
  }

  async toggleDynamicSection(id: string, setActive?: boolean): Promise<boolean> {
    const response = await apiClient.post(`${ADMIN_BASE}/dynamic-sections/${id}/toggle`, null, { params: { setActive } });
    return response.data;
  }

  async deleteDynamicSection(id: string): Promise<boolean> {
    const response = await apiClient.delete(`${ADMIN_BASE}/dynamic-sections/${id}`);
    return response.data;
  }

  async reorderDynamicSections(command: ReorderDynamicSectionsCommand): Promise<boolean> {
    const payload = {
      sections: command.sections.map(s => ({ id: s.sectionId, newOrder: s.newOrder }))
    };
    const response = await apiClient.post(`${ADMIN_BASE}/dynamic-sections/reorder`, payload);
    return response.data;
  }

  // Dynamic Config (Admin)
  async getHomeConfig(version?: string): Promise<DynamicHomeConfig> {
    const response = await apiClient.get(`${ADMIN_BASE}/dynamic-config`, { params: { version } });
    return response.data;
  }

  async createHomeConfig(command: CreateDynamicConfigCommand): Promise<string> {
    const payload = {
      version: command.version,
      globalSettings: JSON.stringify(command.globalSettings || {}),
      themeSettings: JSON.stringify(command.themeSettings || {}),
      layoutSettings: JSON.stringify(command.layoutSettings || {}),
      cacheSettings: JSON.stringify(command.cacheSettings || {}),
      analyticsSettings: JSON.stringify(command.analyticsSettings || {}),
      enabledFeatures: JSON.stringify(command.enabledFeatures || []),
      experimentalFeatures: JSON.stringify(command.experimentalFeatures || {}),
      description: command.description
    };
    const response = await apiClient.post(`${ADMIN_BASE}/dynamic-config`, payload);
    return response.data;
  }

  async updateHomeConfig(id: string, command: UpdateDynamicConfigCommand): Promise<boolean> {
    const payload = {
      globalSettings: JSON.stringify(command.globalSettings || {}),
      themeSettings: JSON.stringify(command.themeSettings || {}),
      layoutSettings: JSON.stringify(command.layoutSettings || {}),
      cacheSettings: JSON.stringify(command.cacheSettings || {}),
      analyticsSettings: JSON.stringify(command.analyticsSettings || {}),
      enabledFeatures: JSON.stringify(command.enabledFeatures || []),
      experimentalFeatures: JSON.stringify(command.experimentalFeatures || {}),
      description: command.description
    };
    const response = await apiClient.put(`${ADMIN_BASE}/dynamic-config/${id}`, payload);
    return response.data;
  }

  async publishHomeConfig(id: string): Promise<boolean> {
    const response = await apiClient.post(`${ADMIN_BASE}/dynamic-config/${id}/publish`);
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
    const response = await apiClient.get(`${ADMIN_BASE}/city-destinations`, { params });
    return response.data;
  }

  async createCityDestination(command: CreateCityDestinationCommand): Promise<string> {
    const payload = {
      ...command,
      metadata: JSON.stringify(command.metadata || {}),
      weatherData: JSON.stringify((command as any).weatherData || {}),
      attractionsData: JSON.stringify((command as any).attractionsData || {})
    };
    const response = await apiClient.post(`${ADMIN_BASE}/city-destinations`, payload);
    return response.data;
  }

  async updateCityDestination(id: string, command: UpdateCityDestinationCommand): Promise<boolean> {
    const payload = {
      ...command,
      metadata: JSON.stringify(command.metadata || {}),
      weatherData: JSON.stringify((command as any).weatherData || {}),
      attractionsData: JSON.stringify((command as any).attractionsData || {})
    };
    const response = await apiClient.put(`${ADMIN_BASE}/city-destinations/${id}`, payload);
    return response.data;
  }

  async updateCityDestinationStats(command: UpdateCityDestinationStatsCommand): Promise<boolean> {
    const { id, ...body } = command;
    const response = await apiClient.put(`${ADMIN_BASE}/city-destinations/${id}/stats`, body);
    return response.data;
  }

  // Sponsored Ads (Admin)
  async getSponsoredAds(params?: { onlyActive?: boolean; limit?: number; includePropertyDetails?: boolean; targetAudience?: string[] }): Promise<SponsoredAd[]> {
    const response = await apiClient.get(`${ADMIN_BASE}/sponsored-ads`, { params });
    return response.data;
  }

  async createSponsoredAd(command: CreateSponsoredAdCommand): Promise<string> {
    const payload = {
      ...command,
      styling: JSON.stringify(command.styling || {}),
      ctaData: JSON.stringify(command.ctaData || {}),
      targetingData: JSON.stringify(command.targetingData || {}),
      analyticsData: JSON.stringify(command.analyticsData || {})
    };
    const response = await apiClient.post(`${ADMIN_BASE}/sponsored-ads`, payload);
    return response.data;
  }

  async updateSponsoredAd(id: string, command: UpdateSponsoredAdCommand): Promise<boolean> {
    const payload = {
      ...command,
      styling: JSON.stringify(command.styling || {}),
      ctaData: JSON.stringify(command.ctaData || {}),
      targetingData: JSON.stringify(command.targetingData || {}),
      analyticsData: JSON.stringify(command.analyticsData || {})
    };
    const response = await apiClient.put(`${ADMIN_BASE}/sponsored-ads/${id}`, payload);
    return response.data;
  }

  async recordAdInteraction(command: RecordAdInteractionCommand): Promise<boolean> {
    const { adId, interactionType, ...body } = command;
    const response = await apiClient.post(`${CLIENT_BASE}/sponsored-ads/${adId}/${interactionType}`, body);
    return response.data;
  }
}

export default new HomeSectionsService();
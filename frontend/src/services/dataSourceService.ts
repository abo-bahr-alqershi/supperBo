import { apiClient } from './api.service';
import type { DataSourceDefinition } from '../types/component.types';

const API_BASE_URL = '/api';

export class DataSourceService {
  async getDataSources(componentType?: string): Promise<DataSourceDefinition[]> {
    const response = await apiClient.get(`${API_BASE_URL}/home-screens/data-sources`, {
      params: { componentType }
    });
    return response.data.data;
  }

  async fetchData(
    endpoint: string,
    method: string = 'GET',
    params?: Record<string, any>,
    headers?: Record<string, string>
  ): Promise<any> {
    const response = await apiClient.request({
      method,
      url: `${API_BASE_URL}${endpoint}`,
      params: method === 'GET' ? params : undefined,
      data: method !== 'GET' ? params : undefined,
      headers
    });
    return response.data;
  }

  async validateDataMapping(
    data: any,
    mapping: string
  ): Promise<{ isValid: boolean; errors?: string[] }> {
    try {
      const mappingObj = JSON.parse(mapping);
      // Validate mapping logic here
      return { isValid: true };
    } catch (error) {
      return { 
        isValid: false, 
        errors: ['Invalid mapping JSON format'] 
      };
    }
  }
}

export default new DataSourceService();

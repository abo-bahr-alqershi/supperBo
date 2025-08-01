import { apiClient } from './api.service';
import type { ResultDto, PaginatedResult } from '../types/common.types';
import type {
  PropertyDto,
  CreatePropertyCommand,
  UpdatePropertyCommand,
  DeletePropertyCommand,
  GetPropertyByIdQuery,
  GetPropertyDetailsQuery,
  GetPropertyForEditQuery,
  GetPropertiesByCityQuery,
  GetPropertiesByOwnerQuery,
  GetPropertiesByTypeQuery,
  GetPropertyRatingStatsQuery,
  GetPropertyAmenitiesQuery,
  PropertyDetailsDto,
  PropertyEditDto,
  FieldGroupWithFieldsDto,
  PropertyRatingStatsDto,
} from '../types/property.types';
import type { AmenityDto } from '../types/amenity.types';

// Base URL for property owner endpoints
const API_BASE = '/api/property/properties';

export const PropertyPropertiesService = {
  /** Get property by ID */
  getById: (query: GetPropertyByIdQuery) =>
    apiClient.get<ResultDto<PropertyDto>>(`${API_BASE}/${query.propertyId}`).then(res => res.data),

  /** Create a new property */
  create: (data: CreatePropertyCommand) =>
    apiClient.post<ResultDto<string>>(API_BASE, data).then(res => res.data),

  /** Update an existing property */
  update: (propertyId: string, data: UpdatePropertyCommand) =>
    apiClient.put<ResultDto<boolean>>(`${API_BASE}/${propertyId}`, data).then(res => res.data),

  /** Delete a property */
  delete: (propertyId: string) =>
    apiClient.delete<ResultDto<boolean>>(`${API_BASE}/${propertyId}`).then(res => res.data),

  /** Get property details including units and dynamic fields */
  getDetails: (query: GetPropertyDetailsQuery) =>
    apiClient
      .get<ResultDto<PropertyDetailsDto>>(
        `${API_BASE}/${query.propertyId}/details`,
        { params: { includeUnits: query.includeUnits } }
      )
      .then(res => res.data),

  /** Get property data for edit form */
  getForEdit: (query: GetPropertyForEditQuery) =>
    apiClient
      .get<ResultDto<PropertyEditDto>>(
        `${API_BASE}/${query.propertyId}/for-edit`,
        { params: { ownerId: query.ownerId } }
      )
      .then(res => res.data),

  /** Get property rating statistics */
  getRatingStats: (query: GetPropertyRatingStatsQuery) =>
    apiClient
      .get<ResultDto<PropertyRatingStatsDto>>(
        `${API_BASE}/${query.propertyId}/rating-stats`
      )
      .then(res => res.data),

  /** Get property amenities */
  getAmenities: (query: GetPropertyAmenitiesQuery) =>
    apiClient
      .get<ResultDto<AmenityDto[]>>(
        `${API_BASE}/${query.propertyId}/amenities`
      )
      .then(res => res.data),
}; 
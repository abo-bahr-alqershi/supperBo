import { useState, useEffect, useCallback } from 'react';
import homeSectionsService from '../services/homeSectionsService';
import type { DynamicHomeSection } from '../types/homeSections.types';

export interface UseHomeSectionsParams {
  language?: string;
  targetAudience?: string[];
  includeContent?: boolean;
  onlyActive?: boolean;
}

export function useHomeSections(params: UseHomeSectionsParams = {}): {
  sections: DynamicHomeSection[];
  loading: boolean;
  error: string | null;
  refetch: () => void;
} {
  const [sections, setSections] = useState<DynamicHomeSection[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetch = useCallback(() => {
    setLoading(true);
    homeSectionsService.getDynamicSections(params)
      .then(data => {
        setSections(data.sort((a, b) => a.order - b.order));
        setError(null);
      })
      .catch(err => {
        setError(err.message || 'Failed to load sections');
      })
      .finally(() => {
        setLoading(false);
      });
  }, [JSON.stringify(params)]);

  useEffect(() => {
    fetch();
  }, [fetch]);

  return { sections, loading, error, refetch: fetch };
}
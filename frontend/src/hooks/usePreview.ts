import { useState, useCallback, useEffect } from 'react';
import type { HomeScreenTemplate } from '../types/homeScreen.types';
import homeScreenService from '../services/homeScreenService';

interface PreviewOptions {
  platform: 'iOS' | 'Android' | 'Web';
  deviceType: 'mobile' | 'tablet' | 'desktop';
  orientation: 'portrait' | 'landscape';
  useMockData: boolean;
  darkMode: boolean;
}

interface PreviewState {
  isOpen: boolean;
  isLoading: boolean;
  previewData: any;
  options: PreviewOptions;
  error: string | null;
}

export const usePreview = (template: HomeScreenTemplate | null) => {
  const [state, setState] = useState<PreviewState>({
    isOpen: false,
    isLoading: false,
    previewData: null,
    options: {
      platform: 'iOS',
      deviceType: 'mobile',
      orientation: 'portrait',
      useMockData: true,
      darkMode: false
    },
    error: null
  });

  const [deviceDimensions, setDeviceDimensions] = useState({
    width: 375,
    height: 812
  });

  // Update device dimensions based on options
  useEffect(() => {
    const dimensions = getDeviceDimensions(
      state.options.deviceType,
      state.options.orientation
    );
    setDeviceDimensions(dimensions);
  }, [state.options.deviceType, state.options.orientation]);

  // Load preview data
  const loadPreview = useCallback(async () => {
    if (!template) return;

    setState(prev => ({ ...prev, isLoading: true, error: null }));
    try {
      const preview = await homeScreenService.previewTemplate(template.id, {
        platform: state.options.platform,
        deviceType: state.options.deviceType,
        useMockData: state.options.useMockData
      });

      setState(prev => ({
        ...prev,
        previewData: preview,
        isLoading: false
      }));
    } catch (error) {
      setState(prev => ({
        ...prev,
        isLoading: false,
        error: 'Failed to load preview'
      }));
    }
  }, [template, state.options]);

  // Open preview
  const openPreview = useCallback(() => {
    setState(prev => ({ ...prev, isOpen: true }));
    loadPreview();
  }, [loadPreview]);

  // Close preview
  const closePreview = useCallback(() => {
    setState(prev => ({
      ...prev,
      isOpen: false,
      previewData: null,
      error: null
    }));
  }, []);

  // Update preview options
  const updateOptions = useCallback((updates: Partial<PreviewOptions>) => {
    setState(prev => ({
      ...prev,
      options: { ...prev.options, ...updates }
    }));
  }, []);

  // Refresh preview
  const refreshPreview = useCallback(() => {
    loadPreview();
  }, [loadPreview]);

  // Export preview as image
  const exportAsImage = useCallback(async (format: 'png' | 'jpg' = 'png') => {
    // Implementation would use html2canvas or similar
    console.log('Export preview as', format);
  }, []);

  // Share preview link
  const sharePreview = useCallback(async () => {
    if (!template) return;
    
    try {
      const shareUrl = `${window.location.origin}/preview/${template.id}`;
      
      if (navigator.share) {
        await navigator.share({
          title: `Preview: ${template.name}`,
          text: template.description,
          url: shareUrl
        });
      } else {
        // Fallback: copy to clipboard
        await navigator.clipboard.writeText(shareUrl);
        alert('Preview link copied to clipboard');
      }
    } catch (error) {
      console.error('Failed to share preview:', error);
    }
  }, [template]);

  return {
    ...state,
    deviceDimensions,
    actions: {
      openPreview,
      closePreview,
      updateOptions,
      refreshPreview,
      exportAsImage,
      sharePreview
    }
  };
};

// Device dimension presets
const getDeviceDimensions = (
  deviceType: string,
  orientation: string
): { width: number; height: number } => {
  const dimensions = {
    mobile: { width: 375, height: 812 },    // iPhone X
    tablet: { width: 768, height: 1024 },   // iPad
    desktop: { width: 1440, height: 900 }   // Desktop
  };

  const { width, height } = dimensions[deviceType as keyof typeof dimensions];
  
  return orientation === 'landscape' 
    ? { width: height, height: width }
    : { width, height };
};

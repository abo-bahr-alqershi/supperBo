import { useState, useCallback, useEffect, useMemo } from 'react';
import { useQuery } from '@tanstack/react-query';
import homeScreenService from '../services/homeScreenService';
import dataSourceService from '../services/dataSourceService';
import type { 
  HomeScreenTemplate, 
  Platform,
  HomeScreenPreviewDto 
} from '../types/homeScreen.types';
import type { ComponentPreview } from '../types/component.types';
import { renderComponent } from '../utils/componentFactory';
import { generateStyleString, generateResponsiveStyles } from '../utils/styleGenerator';

interface UsePreviewOptions {
  template?: HomeScreenTemplate | null;
  platform?: Platform;
  deviceType?: 'mobile' | 'tablet' | 'desktop';
  useMockData?: boolean;
}

interface DeviceInfo {
  name: string;
  width: number;
  height: number;
  scale: number;
}

const DEVICES: Record<string, DeviceInfo> = {
  mobile: { name: 'iPhone 12', width: 390, height: 844, scale: 0.5 },
  tablet: { name: 'iPad', width: 768, height: 1024, scale: 0.75 },
  desktop: { name: 'Desktop', width: 1920, height: 1080, scale: 1 }
};

export const usePreview = (options: UsePreviewOptions) => {
  const {
    template,
    platform = 'All',
    deviceType = 'mobile',
    useMockData = true
  } = options;
  
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const [previewData, setPreviewData] = useState<HomeScreenPreviewDto | null>(null);
  const [currentDevice, setCurrentDevice] = useState<DeviceInfo>(DEVICES[deviceType]);
  const [zoom, setZoom] = useState(1);
  const [orientation, setOrientation] = useState<'portrait' | 'landscape'>('portrait');
  
  // Fetch preview data
  const { data: serverPreview, refetch: refetchPreview } = useQuery({
    queryKey: ['homeScreenPreview', template?.id, platform, deviceType, useMockData],
    queryFn: () => template?.id 
      ? homeScreenService.previewTemplate(template.id, { platform, deviceType, useMockData })
      : null,
    enabled: !!template?.id && !useMockData
  });
  
  // Generate client-side preview
  const generateClientPreview = useCallback(async (): Promise<HomeScreenPreviewDto | null> => {
    if (!template) return null;
    
    setIsLoading(true);
    setError(null);
    
    try {
      const sections = await Promise.all(
        template.sections
          .filter(section => section.isVisible)
          .sort((a, b) => a.order - b.order)
          .map(async section => {
            const components = await Promise.all(
              section.components
                .filter(component => component.isVisible)
                .sort((a, b) => a.order - b.order)
                .map(async component => {
                  let data = null;
                  
                  // Fetch component data
                  if (component.dataSource && !useMockData) {
                    try {
                      const { dataEndpoint, httpMethod, queryParams } = component.dataSource;
                      if (dataEndpoint) {
                        data = await dataSourceService.fetchData(
                          dataEndpoint,
                          httpMethod || 'GET',
                          queryParams ? JSON.parse(queryParams) : undefined
                        );
                      }
                    } catch (err) {
                      console.error('Failed to fetch component data:', err);
                    }
                  } else if (component.dataSource?.mockData) {
                    data = JSON.parse(component.dataSource.mockData);
                  }
                  
                  // Generate component preview
                  const preview: ComponentPreview = {
                    id: component.id,
                    type: component.componentType,
                    name: component.name,
                    order: component.order,
                    colSpan: component.colSpan,
                    rowSpan: component.rowSpan,
                    alignment: component.alignment,
                    properties: component.properties.reduce((acc, prop) => {
                      acc[prop.propertyKey] = prop.value ?? prop.defaultValue;
                      return acc;
                                        }, {} as Record<string, any>),
                    styles: component.styles.reduce((acc, style) => {
                      const value = style.unit ? `${style.styleValue}${style.unit}` : style.styleValue;
                      acc[style.styleKey] = value;
                      return acc;
                    }, {} as Record<string, string>),
                    data,
                    animation: component.animationType ? {
                      type: component.animationType,
                      duration: component.animationDuration || 300,
                      delay: 0
                    } : undefined
                  };
                  
                  return preview;
                })
            );
            
            return {
              id: section.id,
              name: section.name,
              title: section.title,
              subtitle: section.subtitle || '',
              order: section.order,
              isVisible: section.isVisible,
              styles: {
                backgroundColor: section.backgroundColor || '#ffffff',
                backgroundImage: section.backgroundImage || '',
                padding: section.padding || '20px',
                margin: section.margin || '0',
                minHeight: section.minHeight ? `${section.minHeight}px` : 'auto',
                maxHeight: section.maxHeight ? `${section.maxHeight}px` : 'none'
              },
              components
            };
          })
      );
      
      const preview: HomeScreenPreviewDto = {
        templateId: template.id,
        templateName: template.name,
        platform: template.platform,
        deviceType,
        sections,
        metadata: {
          generatedAt: new Date().toISOString(),
          totalSections: sections.length,
          totalComponents: sections.reduce((sum, section) => sum + section.components.length, 0),
          estimatedLoadTime: sections.length * 100 + sections.reduce((sum, section) => 
            sum + section.components.length * 50, 0
          ),
          usedMockData: useMockData
        }
      };
      
      setPreviewData(preview);
      return preview;
    } catch (err) {
      setError(err as Error);
      return null;
    } finally {
      setIsLoading(false);
    }
  }, [template, deviceType, useMockData]);
  
  // Initialize preview
  useEffect(() => {
    if (template) {
      if (useMockData) {
        generateClientPreview();
      } else if (serverPreview) {
        setPreviewData(serverPreview);
      }
    }
  }, [template, useMockData, serverPreview, generateClientPreview]);
  
  // Device management
  const changeDevice = useCallback((newDeviceType: keyof typeof DEVICES) => {
    setCurrentDevice(DEVICES[newDeviceType]);
  }, []);
  
  const toggleOrientation = useCallback(() => {
    setOrientation(prev => prev === 'portrait' ? 'landscape' : 'portrait');
    setCurrentDevice(prev => ({
      ...prev,
      width: prev.height,
      height: prev.width
    }));
  }, []);
  
  const zoomIn = useCallback(() => {
    setZoom(prev => Math.min(prev + 0.1, 2));
  }, []);
  
  const zoomOut = useCallback(() => {
    setZoom(prev => Math.max(prev - 0.1, 0.5));
  }, []);
  
  const resetZoom = useCallback(() => {
    setZoom(1);
  }, []);
  
  const fitToScreen = useCallback((containerWidth: number, containerHeight: number) => {
    const { width, height } = currentDevice;
    const scaleX = containerWidth / width;
    const scaleY = containerHeight / height;
    const scale = Math.min(scaleX, scaleY) * 0.9; // 90% to leave some padding
    setZoom(scale);
  }, [currentDevice]);
  
  // Generate preview styles
  const previewStyles = useMemo(() => {
    const { width, height, scale } = currentDevice;
    const finalWidth = orientation === 'portrait' ? width : height;
    const finalHeight = orientation === 'portrait' ? height : width;
    
    return {
      container: {
        position: 'relative' as const,
        width: '100%',
        height: '100%',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        overflow: 'auto',
        backgroundColor: '#f5f5f5'
      },
      device: {
        width: finalWidth,
        height: finalHeight,
        transform: `scale(${zoom * scale})`,
        transformOrigin: 'center center',
        transition: 'transform 0.3s ease',
        backgroundColor: '#ffffff',
        boxShadow: '0 10px 40px rgba(0, 0, 0, 0.2)',
        borderRadius: deviceType === 'mobile' ? '40px' : '8px',
        overflow: 'hidden',
        position: 'relative' as const
      },
      screen: {
        width: '100%',
        height: '100%',
        overflow: 'auto',
        position: 'relative' as const,
        backgroundColor: '#ffffff'
      },
      statusBar: {
        height: deviceType === 'mobile' ? '44px' : '0',
        backgroundColor: '#000000',
        color: '#ffffff',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        padding: '0 20px',
        fontSize: '12px'
      }
    };
  }, [currentDevice, orientation, zoom, deviceType]);
  
  // Export preview as image
  const exportAsImage = useCallback(async (format: 'png' | 'jpeg' = 'png'): Promise<Blob | null> => {
    try {
      // This would use html2canvas or similar library
      // For now, returning null as placeholder
      return null;
    } catch (err) {
      console.error('Failed to export preview:', err);
      return null;
    }
  }, []);
  
  // Get preview HTML
  const getPreviewHTML = useCallback((): string => {
    if (!previewData) return '';
    
    let html = `<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>${previewData.templateName} Preview</title>
  <style>
    * { margin: 0; padding: 0; box-sizing: border-box; }
    body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; }
    .section { position: relative; }
    .component { position: relative; }
  </style>
</head>
<body>\n`;
    
    previewData.sections.forEach(section => {
      const sectionStyles = Object.entries(section.styles)
        .map(([key, value]) => `${camelToKebab(key)}: ${value}`)
        .join('; ');
      
      html += `  <div class="section" style="${sectionStyles}">\n`;
      
      if (section.title) {
        html += `    <h2>${section.title}</h2>\n`;
      }
      
      if (section.subtitle) {
        html += `    <p>${section.subtitle}</p>\n`;
      }
      
      section.components.forEach(component => {
        const componentStyles = Object.entries(component.styles)
          .map(([key, value]) => `${camelToKebab(key)}: ${value}`)
          .join('; ');
        
        html += `    <div class="component ${component.type}" style="${componentStyles}">\n`;
        html += `      <!-- ${component.type} component -->\n`;
        html += `    </div>\n`;
      });
      
      html += `  </div>\n`;
    });
    
    html += `</body>\n</html>`;
    return html;
  }, [previewData]);
  
  // Refresh preview
  const refreshPreview = useCallback(() => {
    if (useMockData) {
      generateClientPreview();
    } else {
      refetchPreview();
    }
  }, [useMockData, generateClientPreview, refetchPreview]);
  
  return {
    // State
    previewData,
    isLoading,
    error,
    currentDevice,
    zoom,
    orientation,
    
    // Device controls
    changeDevice,
    toggleOrientation,
    
    // Zoom controls
    zoomIn,
    zoomOut,
    resetZoom,
    fitToScreen,
    
    // Preview data
    previewStyles,
    refreshPreview,
    
    // Export
    exportAsImage,
    getPreviewHTML,
    
    // Utilities
    deviceInfo: DEVICES,
    isPortrait: orientation === 'portrait'
  };
};

// Helper function
const camelToKebab = (str: string): string => {
  return str.replace(/[A-Z]/g, letter => `-${letter.toLowerCase()}`);
};
// frontend/src/components/Home/HomeScreenLayout.tsx
import React, { useMemo } from 'react';
import { Box, Container } from '@mui/material';
import type { DynamicHomeConfig, DynamicHomeSection } from '../../types/homeSections.types';
import DynamicSection from './DynamicSection.tsx';

interface HomeScreenLayoutProps {
  config: DynamicHomeConfig;
  sections: DynamicHomeSection[];
}

const HomeScreenLayout: React.FC<HomeScreenLayoutProps> = ({ config, sections }) => {
  // ترتيب الأقسام حسب order
  const sortedSections = useMemo(() => {
    return [...sections].sort((a, b) => a.order - b.order);
  }, [sections]);

  // تطبيق إعدادات التخطيط من config
  const layoutSettings = config.layoutSettings || {};
  const sectionSpacing = layoutSettings.sectionSpacing || 24;

  return (
    <Box>
      {sortedSections.map((section, index) => (
        <Box
          key={section.id}
          sx={{
            mb: index < sortedSections.length - 1 ? `${sectionSpacing}px` : 0,
          }}
        >
          <DynamicSection section={section} config={config} />
        </Box>
      ))}
    </Box>
  );
};

export default HomeScreenLayout;
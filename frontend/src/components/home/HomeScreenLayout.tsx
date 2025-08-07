// frontend/src/components/home/HomeScreenLayout.tsx
import React, { useMemo } from 'react';
import { Box } from '@mui/material';
import type { DynamicHomeConfig, DynamicHomeSection } from '../../types/homeSections.types';
import DynamicSection from './DynamicSection';

interface HomeScreenLayoutProps {
  config: DynamicHomeConfig;
  sections: DynamicHomeSection[];
}

const HomeScreenLayout: React.FC<HomeScreenLayoutProps> = ({ config, sections }) => {
  const sorted = useMemo(() => [...(sections || [])].sort((a, b) => a.order - b.order), [sections]);
  const spacing = config.layoutSettings?.sectionSpacing ?? 24;

  return (
    <Box>
      {sorted.map((s, i) => (
        <Box key={s.id} sx={{ mb: i < sorted.length - 1 ? `${spacing}px` : 0 }}>
          <DynamicSection section={s} config={config} />
        </Box>
      ))}
    </Box>
  );
};

export default HomeScreenLayout;
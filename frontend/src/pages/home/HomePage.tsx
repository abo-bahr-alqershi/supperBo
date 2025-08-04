// frontend/src/pages/Home/HomePage.tsx
import React, { useEffect } from 'react';
import { Box } from '@mui/material';
import { useHomeConfig, useDynamicHomeSections } from '../../hooks/homeSections';
import HomeScreenLayout from '../../components/Home/HomeScreenLayout';
import LoadingScreen from '../../components/common/LoadingScreen';
import ErrorScreen from '../../components/common/ErrorScreen';

const HomePage: React.FC = () => {
  const { data: config, isLoading: configLoading, error: configError } = useHomeConfig();
  const { 
    data: sections, 
    isLoading: sectionsLoading, 
    error: sectionsError 
  } = useDynamicHomeSections({
    onlyActive: true,
    includeContent: true,
    language: 'ar' // يمكن الحصول عليها من سياق اللغة
  });

  const isLoading = configLoading || sectionsLoading;
  const error = configError || sectionsError;

  if (isLoading) {
    return <LoadingScreen />;
  }

  if (error) {
    return <ErrorScreen error={error} />;
  }

  if (!config || !sections) {
    return <ErrorScreen error={new Error('لم يتم العثور على البيانات')} />;
  }

  return (
    <Box sx={{ minHeight: '100vh', backgroundColor: 'background.default' }}>
      <HomeScreenLayout config={config} sections={sections} />
    </Box>
  );
};

export default HomePage;
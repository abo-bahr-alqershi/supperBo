import React, { useState, useEffect } from 'react';
import { Box, Tabs, Tab } from '@mui/material';
import HomeScreenManagement from './HomeScreenManagement';
import AdminHomeConfig from './AdminHomeConfig';
import AdminCityDestinations from './AdminCityDestinations';
import AdminSponsoredAds from './AdminSponsoredAds';

const AdminHomeSectionsPage: React.FC = () => {
  const [tabIndex, setTabIndex] = useState(0);
  const handleTabChange = (_: React.SyntheticEvent, newIndex: number) => setTabIndex(newIndex);

  return (
    <Box>
      <Tabs value={tabIndex} onChange={handleTabChange} aria-label="Home Sections Tabs">
        <Tab label="Sections" />
        <Tab label="Config" />
        <Tab label="City Destinations" />
        <Tab label="Sponsored Ads" />
      </Tabs>
      <Box mt={2}>
        {tabIndex === 0 && <HomeScreenManagement />}
        {tabIndex === 1 && <AdminHomeConfig />}
        {tabIndex === 2 && <AdminCityDestinations />}
        {tabIndex === 3 && <AdminSponsoredAds />}
      </Box>
    </Box>
  );
};

export default AdminHomeSectionsPage;
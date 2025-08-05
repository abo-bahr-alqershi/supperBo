// frontend/src/components/Home/Sections/ExploreCities.tsx
import React, { useState } from 'react';
import {
  Box,
  Typography,
  Grid,
  Card,
  CardMedia,
  CardContent,
  Tabs,
  Tab,
  Chip,
} from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import { Place, Hotel, Star } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';

interface ExploreCitiesProps {
  section: DynamicHomeSection;
}

const ExploreCities: React.FC<ExploreCitiesProps> = ({ section }) => {
  const navigate = useNavigate();
  const [selectedCategory, setSelectedCategory] = useState(0);
  
  const cities = section.content
    .filter(item => item.contentType === 'DESTINATION')
    .map(item => item.contentData);

  if (cities.length === 0) {
    return null;
  }

  // تصنيف المدن
  const categories = ['الكل', 'الأكثر شعبية', 'وجهات ساحلية', 'مدن تاريخية'];
  
  const filteredCities = cities.filter(city => {
    if (selectedCategory === 0) return true;
    if (selectedCategory === 1) return city.isPopular;
    // يمكن إضافة المزيد من المنطق للتصنيف
    return true;
  });

  return (
    <Box>
      <Box sx={{ textAlign: 'center', mb: 4 }}>
        <Typography variant="h3" fontWeight="bold" gutterBottom>
          {section.title || 'استكشف مدن اليمن'}
        </Typography>
        {section.subtitle && (
          <Typography variant="h6" color="text.secondary">
            {section.subtitle}
          </Typography>
        )}
      </Box>
      
      {/* التبويبات */}
      <Box sx={{ mb: 4, display: 'flex', justifyContent: 'center' }}>
        <Tabs
          value={selectedCategory}
          onChange={(_, value) => setSelectedCategory(value)}
          variant="scrollable"
          scrollButtons="auto"
        >
          {categories.map((category, index) => (
            <Tab key={index} label={category} />
          ))}
        </Tabs>
      </Box>
      
      {/* شبكة المدن */}
      <AnimatePresence mode="wait">
        <Grid container spacing={3}>
          {filteredCities.map((city, index) => (
            <Grid key={city.id} item xs={12} sm={6} md={4} lg={3}>
              <motion.div
                initial={{ opacity: 0, scale: 0.9 }}
                animate={{ opacity: 1, scale: 1 }}
                exit={{ opacity: 0, scale: 0.9 }}
                transition={{ duration: 0.3, delay: index * 0.05 }}
                whileHover={{ y: -10 }}
              >
                <Card
                  sx={{
                    height: '100%',
                    cursor: 'pointer',
                    overflow: 'hidden',
                    '&:hover': {
                      '& .city-image': {
                        transform: 'scale(1.1)',
                      },
                    },
                  }}
                  onClick={() => navigate(`/destinations/${city.id}`)}
                >
                  <Box sx={{ position: 'relative', paddingTop: '75%' }}>
                    <CardMedia
                      component="img"
                      image={city.imageUrl}
                      alt={city.nameAr}
                      className="city-image"
                      sx={{
                        position: 'absolute',
                        top: 0,
                        left: 0,
                        width: '100%',
                        height: '100%',
                        objectFit: 'cover',
                        transition: 'transform 0.3s ease',
                      }}
                    />
                    
                    {city.isFeatured && (
                      <Chip
                        label="مميز"
                        size="small"
                        sx={{
                          position: 'absolute',
                          top: 8,
                          left: 8,
                          backgroundColor: 'warning.main',
                          color: 'white',
                        }}
                      />
                    )}
                  </Box>
                  
                  <CardContent>
                    <Typography variant="h6" fontWeight="bold" gutterBottom>
                      {city.nameAr}
                    </Typography>
                    
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 1 }}>
                      <Box sx={{ display: 'flex', alignItems: 'center' }}>
                        <Hotel sx={{ fontSize: 16, mr: 0.5, color: 'text.secondary' }} />
                        <Typography variant="body2" color="text.secondary">
                          {city.propertyCount} عقار
                        </Typography>
                      </Box>
                      
                      {city.averageRating > 0 && (
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                          <Star sx={{ fontSize: 16, mr: 0.5, color: 'warning.main' }} />
                          <Typography variant="body2" color="text.secondary">
                            {city.averageRating}
                          </Typography>
                        </Box>
                      )}
                    </Box>
                    
                    {city.averagePrice && (
                      <Typography variant="body2" color="primary" fontWeight="medium">
                        متوسط السعر: {city.averagePrice.toLocaleString()} {city.currency}
                      </Typography>
                    )}
                  </CardContent>
                </Card>
              </motion.div>
            </Grid>
          ))}
        </Grid>
      </AnimatePresence>
    </Box>
  );
};

export default ExploreCities;
import React, { useState } from 'react';
import {
  Box,
  Grid,
  Card,
  CardMedia,
  CardContent,
  Typography,
  Skeleton,
  Chip,
  IconButton,
  useTheme,
  useMediaQuery
} from '@mui/material';
import {
  ArrowForward as ArrowIcon,
  GridView as GridIcon
} from '@mui/icons-material';
import { motion } from 'framer-motion';

interface Category {
  id: string;
  name: string;
  description?: string;
  imageUrl: string;
  itemCount?: number;
  color?: string;
  icon?: string;
  url?: string;
}

interface CategoryGridProps {
  title?: string;
  subtitle?: string;
  categories?: Category[];
  columns?: number;
  cardStyle?: 'default' | 'compact' | 'overlay' | 'minimal';
  showItemCount?: boolean;
  showDescription?: boolean;
  imageHeight?: number;
  spacing?: number;
  data?: Category[];
  isPreview?: boolean;
  onCategoryClick?: (category: Category) => void;
}

const CategoryGrid: React.FC<CategoryGridProps> = ({
  title = 'Browse Categories',
  subtitle,
  categories = [],
  columns = 3,
  cardStyle = 'default',
  showItemCount = true,
  showDescription = true,
  imageHeight = 150,
  spacing = 2,
  data,
  isPreview = false,
  onCategoryClick
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const isTablet = useMediaQuery(theme.breakpoints.down('md'));
  const [imageLoadStates, setImageLoadStates] = useState<Record<string, boolean>>({});

  const displayCategories = data || categories || generateMockCategories();
  const responsiveColumns = isMobile ? 2 : isTablet ? Math.min(columns, 3) : columns;

  const handleImageLoad = (categoryId: string) => {
    setImageLoadStates(prev => ({ ...prev, [categoryId]: true }));
  };

  const container = {
    hidden: { opacity: 0 },
    show: {
      opacity: 1,
      transition: {
        staggerChildren: 0.1
      }
    }
  };

  const item = {
    hidden: { y: 20, opacity: 0 },
    show: { y: 0, opacity: 1 }
  };

  const renderCard = (category: Category) => {
    switch (cardStyle) {
      case 'compact':
        return (
          <Card
            sx={{
              cursor: isPreview ? 'default' : 'pointer',
              transition: 'all 0.3s',
              '&:hover': {
                transform: isPreview ? 'none' : 'translateY(-4px)',
                boxShadow: isPreview ? 1 : 4
              }
            }}
            onClick={() => !isPreview && onCategoryClick?.(category)}
          >
            <Box sx={{ display: 'flex', alignItems: 'center', p: 2 }}>
              <Box
                sx={{
                  width: 60,
                  height: 60,
                  borderRadius: 2,
                  overflow: 'hidden',
                  flexShrink: 0,
                  mr: 2,
                  backgroundColor: category.color || 'primary.light'
                }}
              >
                {category.icon ? (
                  <Box
                    sx={{
                      width: '100%',
                      height: '100%',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontSize: 24
                    }}
                  >
                    {category.icon}
                  </Box>
                ) : (
                  <>
                    {!imageLoadStates[category.id] && (
                      <Skeleton variant="rectangular" width="100%" height="100%" />
                    )}
                    <img
                      src={category.imageUrl}
                      alt={category.name}
                      onLoad={() => handleImageLoad(category.id)}
                      style={{
                        width: '100%',
                        height: '100%',
                        objectFit: 'cover',
                        display: imageLoadStates[category.id] ? 'block' : 'none'
                      }}
                    />
                  </>
                )}
              </Box>
              <Box sx={{ flexGrow: 1 }}>
                <Typography variant="body1" fontWeight="medium">
                  {category.name}
                </Typography>
                {showItemCount && category.itemCount !== undefined && (
                  <Typography variant="caption" color="text.secondary">
                    {category.itemCount} items
                  </Typography>
                )}
              </Box>
              <ArrowIcon color="action" />
            </Box>
          </Card>
        );

      case 'overlay':
        return (
          <Card
            sx={{
              position: 'relative',
              height: imageHeight,
              cursor: isPreview ? 'default' : 'pointer',
              overflow: 'hidden',
              '&:hover .overlay': {
                backgroundColor: 'rgba(0,0,0,0.7)'
              },
              '&:hover .content': {
                transform: 'translateY(0)'
              }
            }}
            onClick={() => !isPreview && onCategoryClick?.(category)}
          >
            {!imageLoadStates[category.id] && (
              <Skeleton variant="rectangular" width="100%" height="100%" />
            )}
            <CardMedia
              component="img"
              height="100%"
              image={category.imageUrl}
              alt={category.name}
              onLoad={() => handleImageLoad(category.id)}
              sx={{
                display: imageLoadStates[category.id] ? 'block' : 'none'
              }}
            />
            <Box
              className="overlay"
              sx={{
                position: 'absolute',
                inset: 0,
                backgroundColor: 'rgba(0,0,0,0.4)',
                transition: 'background-color 0.3s'
              }}
            />
            <Box
              className="content"
              sx={{
                position: 'absolute',
                bottom: 0,
                left: 0,
                right: 0,
                p: 2,
                color: 'white',
                transform: 'translateY(20px)',
                transition: 'transform 0.3s'
              }}
            >
              <Typography variant="h6" fontWeight="bold">
                {category.name}
              </Typography>
              {showItemCount && category.itemCount !== undefined && (
                <Typography variant="body2">
                  {category.itemCount} items
                </Typography>
              )}
            </Box>
          </Card>
        );

      case 'minimal':
        return (
          <Box
            sx={{
              textAlign: 'center',
              cursor: isPreview ? 'default' : 'pointer',
              '&:hover .icon-wrapper': {
                transform: isPreview ? 'none' : 'scale(1.1)'
              }
            }}
            onClick={() => !isPreview && onCategoryClick?.(category)}
          >
            <Box
              className="icon-wrapper"
              sx={{
                width: 80,
                height: 80,
                borderRadius: '50%',
                overflow: 'hidden',
                mx: 'auto',
                mb: 1,
                backgroundColor: category.color || 'primary.light',
                transition: 'transform 0.3s'
              }}
            >
              {category.icon ? (
                <Box
                  sx={{
                    width: '100%',
                    height: '100%',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: 32
                  }}
                >
                  {category.icon}
                </Box>
              ) : (
                <>
                  {!imageLoadStates[category.id] && (
                    <Skeleton variant="circular" width="100%" height="100%" />
                  )}
                  <img
                    src={category.imageUrl}
                    alt={category.name}
                    onLoad={() => handleImageLoad(category.id)}
                    style={{
                      width: '100%',
                      height: '100%',
                      objectFit: 'cover',
                      display: imageLoadStates[category.id] ? 'block' : 'none'
                    }}
                  />
                </>
              )}
            </Box>
            <Typography variant="body2" fontWeight="medium">
              {category.name}
            </Typography>
          </Box>
        );

      default:
        return (
          <Card
            sx={{
              cursor: isPreview ? 'default' : 'pointer',
              transition: 'all 0.3s',
              '&:hover': {
                transform: isPreview ? 'none' : 'translateY(-4px)',
                boxShadow: isPreview ? 1 : 4
              }
            }}
            onClick={() => !isPreview && onCategoryClick?.(category)}
          >
            <Box sx={{ position: 'relative', height: imageHeight }}>
              {!imageLoadStates[category.id] && (
                <Skeleton variant="rectangular" width="100%" height="100%" />
              )}
              <CardMedia
                component="img"
                height="100%"
                image={category.imageUrl}
                alt={category.name}
                onLoad={() => handleImageLoad(category.id)}
                sx={{
                  display: imageLoadStates[category.id] ? 'block' : 'none',
                  objectFit: 'cover'
                }}
              />
              {category.itemCount !== undefined && showItemCount && (
                <Chip
                  label={`${category.itemCount} items`}
                  size="small"
                  sx={{
                    position: 'absolute',
                    top: 8,
                    right: 8,
                    backgroundColor: 'rgba(255,255,255,0.9)'
                  }}
                />
              )}
            </Box>
            <CardContent>
              <Typography variant="h6" component="h3" gutterBottom>
                {category.name}
              </Typography>
              {showDescription && category.description && (
                <Typography variant="body2" color="text.secondary">
                  {category.description}
                </Typography>
              )}
            </CardContent>
          </Card>
        );
    }
  };

  return (
    <Box sx={{ width: '100%' }}>
      {/* Header */}
      {(title || subtitle) && (
        <Box sx={{ mb: 3 }}>
          {title && (
            <Typography variant="h5" component="h2" fontWeight="bold" gutterBottom>
              {title}
            </Typography>
          )}
          {subtitle && (
            <Typography variant="body1" color="text.secondary">
              {subtitle}
            </Typography>
          )}
        </Box>
      )}

      {/* Grid */}
      <motion.div
        variants={container}
        initial="hidden"
        animate="show"
      >
        <Grid container spacing={spacing}>
          {displayCategories.map((category) => (
            <Grid item xs={12 / responsiveColumns} key={category.id}>
              <motion.div variants={item}>
                {renderCard(category)}
              </motion.div>
            </Grid>
          ))}
        </Grid>
      </motion.div>
    </Box>
  );
};

// Generate mock categories for preview
function generateMockCategories(): Category[] {
  const categories = [
    { name: 'Electronics', icon: '📱', color: '#1976d2' },
    { name: 'Fashion', icon: '👕', color: '#9c27b0' },
    { name: 'Home & Garden', icon: '🏠', color: '#4caf50' },
    { name: 'Sports', icon: '⚽', color: '#ff9800' },
    { name: 'Books', icon: '📚', color: '#795548' },
    { name: 'Toys', icon: '🧸', color: '#e91e63' }
  ];

  return categories.map((cat, index) => ({
    id: `category-${index}`,
    name: cat.name,
    description: `Browse our ${cat.name.toLowerCase()} collection`,
    imageUrl: `/api/placeholder/300/200`,
    itemCount: Math.floor(Math.random() * 100) + 20,
    icon: cat.icon,
    color: cat.color
  }));
}

export default CategoryGrid;
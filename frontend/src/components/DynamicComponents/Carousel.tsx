import React, { useState, useRef, useEffect } from 'react';
import {
  Box,
  Typography,
  IconButton,
  Card,
  CardMedia,
  CardContent,
  Skeleton,
  useTheme,
  useMediaQuery,
  Chip
} from '@mui/material';
import {
  ChevronLeft as ChevronLeftIcon,
  ChevronRight as ChevronRightIcon,
  FavoriteBorder as FavoriteIcon,
  Star as StarIcon
} from '@mui/icons-material';
import { motion, AnimatePresence } from 'framer-motion';
import { Swiper, SwiperSlide } from 'swiper/react';
import { Navigation, Pagination, Autoplay } from 'swiper/modules';
import 'swiper/css';
import 'swiper/css/navigation';
import 'swiper/css/pagination';

interface CarouselItem {
  id: string;
  title: string;
  subtitle?: string;
  imageUrl: string;
  price?: number;
  originalPrice?: number;
  rating?: number;
  badge?: string;
  url?: string;
}

interface CarouselProps {
  title?: string;
  subtitle?: string;
  items?: CarouselItem[];
  itemsPerView?: number;
  spaceBetween?: number;
  autoPlay?: boolean;
  autoPlayDelay?: number;
  showNavigation?: boolean;
  showPagination?: boolean;
  showPrice?: boolean;
  showRating?: boolean;
  cardHeight?: number;
  loop?: boolean;
  data?: CarouselItem[];
  isPreview?: boolean;
  onItemClick?: (item: CarouselItem) => void;
  onFavoriteClick?: (item: CarouselItem) => void;
}

const Carousel: React.FC<CarouselProps> = ({
  title = 'Featured Items',
  subtitle,
  items = [],
  itemsPerView = 4,
  spaceBetween = 20,
  autoPlay = false,
  autoPlayDelay = 3000,
  showNavigation = true,
  showPagination = false,
  showPrice = true,
  showRating = true,
  cardHeight = 280,
  loop = true,
  data,
  isPreview = false,
  onItemClick,
  onFavoriteClick
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const isTablet = useMediaQuery(theme.breakpoints.down('md'));
  const swiperRef = useRef<any>(null);
  const [imageLoadStates, setImageLoadStates] = useState<Record<string, boolean>>({});

  const displayItems = data || items || generateMockItems();

  const responsiveItemsPerView = isMobile ? 1 : isTablet ? 2 : itemsPerView;

  const handleImageLoad = (itemId: string) => {
    setImageLoadStates(prev => ({ ...prev, [itemId]: true }));
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  };

  const renderStars = (rating: number) => {
    return Array.from({ length: 5 }).map((_, index) => (
      <StarIcon
        key={index}
        fontSize="small"
        sx={{
          color: index < Math.floor(rating) ? 'warning.main' : 'action.disabled'
        }}
      />
    ));
  };

  return (
    <Box sx={{ width: '100%', py: 2 }}>
      {/* Header */}
      {(title || subtitle) && (
        <Box sx={{ mb: 3, px: 2 }}>
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

      {/* Carousel */}
      <Box sx={{ position: 'relative' }}>
        <Swiper
          ref={swiperRef}
          modules={[Navigation, Pagination, Autoplay]}
          spaceBetween={spaceBetween}
          slidesPerView={responsiveItemsPerView}
          navigation={showNavigation && !isMobile ? {
            prevEl: '.carousel-prev',
            nextEl: '.carousel-next',
          } : false}
          pagination={showPagination ? { clickable: true } : false}
          autoplay={autoPlay && !isPreview ? {
            delay: autoPlayDelay,
            disableOnInteraction: false
          } : false}
          loop={loop && displayItems.length > responsiveItemsPerView}
          style={{ paddingBottom: showPagination ? '40px' : '0' }}
        >
          {displayItems.map((item) => (
            <SwiperSlide key={item.id}>
              <Card
                sx={{
                  height: cardHeight,
                  cursor: isPreview ? 'default' : 'pointer',
                  transition: 'all 0.3s',
                  '&:hover': {
                    transform: isPreview ? 'none' : 'translateY(-4px)',
                    boxShadow: isPreview ? 1 : 4
                  }
                }}
                onClick={() => !isPreview && onItemClick?.(item)}
              >
                <Box sx={{ position: 'relative', height: '60%' }}>
                  {!imageLoadStates[item.id] && (
                    <Skeleton
                      variant="rectangular"
                      width="100%"
                      height="100%"
                      animation="wave"
                    />
                  )}
                  <CardMedia
                    component="img"
                    height="100%"
                    image={item.imageUrl}
                    alt={item.title}
                    onLoad={() => handleImageLoad(item.id)}
                    sx={{
                      display: imageLoadStates[item.id] ? 'block' : 'none',
                      objectFit: 'cover'
                    }}
                  />
                  
                  {/* Badge */}
                  {item.badge && (
                    <Chip
                      label={item.badge}
                      size="small"
                      color="error"
                      sx={{
                        position: 'absolute',
                        top: 8,
                        left: 8,
                        fontWeight: 'bold'
                      }}
                    />
                  )}
                  
                  {/* Favorite Button */}
                  {!isPreview && (
                    <IconButton
                      size="small"
                      onClick={(e) => {
                        e.stopPropagation();
                        onFavoriteClick?.(item);
                      }}
                      sx={{
                        position: 'absolute',
                        top: 8,
                        right: 8,
                        backgroundColor: 'rgba(255,255,255,0.8)',
                        '&:hover': {
                          backgroundColor: 'rgba(255,255,255,0.9)'
                        }
                      }}
                    >
                      <FavoriteIcon />
                    </IconButton>
                  )}
                </Box>

                <CardContent sx={{ height: '40%', p: 2 }}>
                  <Typography
                    variant="body1"
                    component="h3"
                    fontWeight="medium"
                    gutterBottom
                    sx={{
                      overflow: 'hidden',
                      textOverflow: 'ellipsis',
                      whiteSpace: 'nowrap'
                    }}
                  >
                    {item.title}
                  </Typography>
                  
                  {item.subtitle && (
                    <Typography
                      variant="body2"
                      color="text.secondary"
                      sx={{
                        mb: 1,
                        overflow: 'hidden',
                        textOverflow: 'ellipsis',
                        whiteSpace: 'nowrap'
                      }}
                    >
                      {item.subtitle}
                    </Typography>
                  )}
                  
                  {/* Rating */}
                  {showRating && item.rating && (
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                      {renderStars(item.rating)}
                      <Typography variant="caption" sx={{ ml: 0.5 }}>
                        ({item.rating})
                      </Typography>
                    </Box>
                  )}
                  
                  {/* Price */}
                  {showPrice && item.price && (
                    <Box sx={{ display: 'flex', alignItems: 'baseline', gap: 1 }}>
                      <Typography variant="h6" color="primary" fontWeight="bold">
                        {formatPrice(item.price)}
                      </Typography>
                      {item.originalPrice && item.originalPrice > item.price && (
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          sx={{ textDecoration: 'line-through' }}
                        >
                          {formatPrice(item.originalPrice)}
                        </Typography>
                      )}
                    </Box>
                  )}
                </CardContent>
              </Card>
            </SwiperSlide>
          ))}
        </Swiper>

        {/* Custom Navigation Buttons */}
        {showNavigation && !isMobile && displayItems.length > responsiveItemsPerView && (
          <>
            <IconButton
              className="carousel-prev"
              sx={{
                position: 'absolute',
                left: -20,
                top: '50%',
                transform: 'translateY(-50%)',
                backgroundColor: 'background.paper',
                boxShadow: 2,
                zIndex: 10,
                '&:hover': {
                  backgroundColor: 'background.paper'
                }
              }}
            >
              <ChevronLeftIcon />
            </IconButton>
            <IconButton
              className="carousel-next"
              sx={{
                position: 'absolute',
                right: -20,
                top: '50%',
                transform: 'translateY(-50%)',
                backgroundColor: 'background.paper',
                boxShadow: 2,
                zIndex: 10,
                '&:hover': {
                  backgroundColor: 'background.paper'
                }
              }}
            >
              <ChevronRightIcon />
            </IconButton>
          </>
        )}
      </Box>
    </Box>
  );
};

// Generate mock items for preview
function generateMockItems(): CarouselItem[] {
  return Array.from({ length: 8 }).map((_, index) => ({
    id: `item-${index}`,
    title: `Item ${index + 1}`,
    subtitle: 'Category Name',
    imageUrl: `/api/placeholder/300/200`,
    price: 99.99 + index * 10,
    originalPrice: 129.99 + index * 10,
    rating: 4.5,
    badge: index === 0 ? 'New' : index === 1 ? 'Sale' : undefined
  }));
}

export default Carousel;
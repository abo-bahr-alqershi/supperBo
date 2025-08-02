import React, { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  IconButton,
  Skeleton,
  useTheme,
  useMediaQuery
} from '@mui/material';
import {
  ChevronLeft as ChevronLeftIcon,
  ChevronRight as ChevronRightIcon,
  Circle as CircleIcon
} from '@mui/icons-material';
import { motion, AnimatePresence } from 'framer-motion';

interface BannerProps {
  title?: string;
  subtitle?: string;
  imageUrl?: string;
  imageAlt?: string;
  height?: number | string;
  backgroundPosition?: string;
  backgroundSize?: 'cover' | 'contain' | 'auto';
  overlayColor?: string;
  overlayOpacity?: number;
  textColor?: string;
  textAlign?: 'left' | 'center' | 'right';
  buttonText?: string;
  buttonUrl?: string;
  buttonVariant?: 'text' | 'outlined' | 'contained';
  autoPlay?: boolean;
  autoPlayInterval?: number;
  showIndicators?: boolean;
  showArrows?: boolean;
  data?: Array<{
    id: string;
    title: string;
    subtitle?: string;
    imageUrl: string;
    buttonText?: string;
    buttonUrl?: string;
  }>;
  isPreview?: boolean;
  onBannerClick?: (banner: any) => void;
  onButtonClick?: (url: string) => void;
}

const Banner: React.FC<BannerProps> = ({
  title = 'Welcome to Our App',
  subtitle = 'Discover amazing features',
  imageUrl = '/api/placeholder/800/400',
  imageAlt = 'Banner Image',
  height = 400,
  backgroundPosition = 'center',
  backgroundSize = 'cover',
  overlayColor = '#000000',
  overlayOpacity = 0.4,
  textColor = '#ffffff',
  textAlign = 'center',
  buttonText = 'Learn More',
  buttonUrl = '#',
  buttonVariant = 'contained',
  autoPlay = true,
  autoPlayInterval = 5000,
  showIndicators = true,
  showArrows = true,
  data,
  isPreview = false,
  onBannerClick,
  onButtonClick
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [currentIndex, setCurrentIndex] = useState(0);
  const [imageLoaded, setImageLoaded] = useState(false);

  const banners = data && data.length > 0 ? data : [{
    id: 'default',
    title,
    subtitle,
    imageUrl,
    buttonText,
    buttonUrl
  }];

  useEffect(() => {
    if (!autoPlay || banners.length <= 1 || isPreview) return;

    const interval = setInterval(() => {
      setCurrentIndex((prev) => (prev + 1) % banners.length);
    }, autoPlayInterval);

    return () => clearInterval(interval);
  }, [autoPlay, autoPlayInterval, banners.length, isPreview]);

  const handlePrevious = () => {
    setCurrentIndex((prev) => (prev - 1 + banners.length) % banners.length);
  };

  const handleNext = () => {
    setCurrentIndex((prev) => (prev + 1) % banners.length);
  };

  const handleIndicatorClick = (index: number) => {
    setCurrentIndex(index);
  };

  const handleButtonClick = (e: React.MouseEvent, url?: string) => {
    e.stopPropagation();
    if (isPreview) return;
    
    if (onButtonClick && url) {
      onButtonClick(url);
    } else if (url && !isPreview) {
      window.location.href = url;
    }
  };

  const currentBanner = banners[currentIndex];

  return (
    <Box
      sx={{
        position: 'relative',
        width: '100%',
        height: typeof height === 'number' ? `${height}px` : height,
        overflow: 'hidden',
        cursor: isPreview ? 'default' : 'pointer',
        borderRadius: 1
      }}
      onClick={() => !isPreview && onBannerClick?.(currentBanner)}
    >
      <AnimatePresence mode="wait">
        <motion.div
          key={currentBanner.id}
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.5 }}
          style={{
            position: 'absolute',
            inset: 0
          }}
        >
          {/* Background Image */}
          <Box
            sx={{
              position: 'absolute',
              inset: 0,
              backgroundImage: imageLoaded ? `url(${currentBanner.imageUrl})` : undefined,
              backgroundPosition,
              backgroundSize,
              backgroundRepeat: 'no-repeat',
              '&::after': {
                content: '""',
                position: 'absolute',
                inset: 0,
                backgroundColor: overlayColor,
                opacity: overlayOpacity
              }
            }}
          >
            {!imageLoaded && (
              <Skeleton
                variant="rectangular"
                width="100%"
                height="100%"
                animation="wave"
              />
            )}
            <img
              src={currentBanner.imageUrl}
              alt={imageAlt}
              style={{ display: 'none' }}
              onLoad={() => setImageLoaded(true)}
              onError={() => setImageLoaded(true)}
            />
          </Box>

          {/* Content */}
          <Box
            sx={{
              position: 'relative',
              height: '100%',
              display: 'flex',
              flexDirection: 'column',
              justifyContent: 'center',
              alignItems: textAlign === 'left' ? 'flex-start' : 
                         textAlign === 'right' ? 'flex-end' : 'center',
              px: { xs: 3, sm: 6, md: 8 },
              py: 4,
              textAlign,
              zIndex: 1
            }}
          >
            <motion.div
              initial={{ y: 30, opacity: 0 }}
              animate={{ y: 0, opacity: 1 }}
              transition={{ delay: 0.2, duration: 0.5 }}
            >
              <Typography
                variant={isMobile ? 'h4' : 'h3'}
                component="h1"
                sx={{
                  color: textColor,
                  fontWeight: 'bold',
                  mb: 2,
                  textShadow: '0 2px 4px rgba(0,0,0,0.3)'
                }}
              >
                {currentBanner.title}
              </Typography>
            </motion.div>

            {currentBanner.subtitle && (
              <motion.div
                initial={{ y: 30, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ delay: 0.3, duration: 0.5 }}
              >
                <Typography
                  variant={isMobile ? 'body1' : 'h6'}
                  sx={{
                    color: textColor,
                    mb: 4,
                    maxWidth: 600,
                    textShadow: '0 1px 2px rgba(0,0,0,0.3)'
                  }}
                >
                  {currentBanner.subtitle}
                </Typography>
              </motion.div>
            )}

            {currentBanner.buttonText && (
              <motion.div
                initial={{ y: 30, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ delay: 0.4, duration: 0.5 }}
              >
                <Button
                  variant={buttonVariant}
                  size={isMobile ? 'medium' : 'large'}
                  onClick={(e) => handleButtonClick(e, currentBanner.buttonUrl)}
                  sx={{
                    color: buttonVariant === 'contained' ? undefined : textColor,
                    borderColor: buttonVariant === 'outlined' ? textColor : undefined,
                    '&:hover': {
                      borderColor: buttonVariant === 'outlined' ? textColor : undefined
                    }
                  }}
                >
                  {currentBanner.buttonText}
                </Button>
              </motion.div>
            )}
          </Box>
        </motion.div>
      </AnimatePresence>

      {/* Navigation Arrows */}
      {showArrows && banners.length > 1 && !isPreview && (
        <>
          <IconButton
            onClick={(e) => {
              e.stopPropagation();
              handlePrevious();
            }}
            sx={{
              position: 'absolute',
              left: 16,
              top: '50%',
              transform: 'translateY(-50%)',
              backgroundColor: 'rgba(0,0,0,0.5)',
              color: 'white',
              '&:hover': {
                backgroundColor: 'rgba(0,0,0,0.7)'
              }
            }}
          >
            <ChevronLeftIcon />
          </IconButton>
          <IconButton
            onClick={(e) => {
              e.stopPropagation();
              handleNext();
            }}
            sx={{
              position: 'absolute',
              right: 16,
              top: '50%',
              transform: 'translateY(-50%)',
              backgroundColor: 'rgba(0,0,0,0.5)',
              color: 'white',
              '&:hover': {
                backgroundColor: 'rgba(0,0,0,0.7)'
              }
            }}
          >
            <ChevronRightIcon />
          </IconButton>
        </>
      )}

      {/* Indicators */}
      {showIndicators && banners.length > 1 && (
        <Box
          sx={{
            position: 'absolute',
            bottom: 20,
            left: '50%',
            transform: 'translateX(-50%)',
            display: 'flex',
            gap: 1,
            zIndex: 1
          }}
        >
          {banners.map((_, index) => (
            <IconButton
              key={index}
              size="small"
              onClick={(e) => {
                e.stopPropagation();
                handleIndicatorClick(index);
              }}
              sx={{
                p: 0.5,
                color: 'white',
                opacity: index === currentIndex ? 1 : 0.5,
                transition: 'opacity 0.3s'
              }}
            >
              <CircleIcon fontSize="small" />
            </IconButton>
          ))}
        </Box>
      )}
    </Box>
  );
};

export default Banner;
import React, { useState } from 'react';
import {
  Box,
  ImageList,
  ImageListItem,
  ImageListItemBar,
  IconButton,
  Dialog,
  DialogContent,
  Typography,
  Skeleton,
  useTheme,
  useMediaQuery,
  Zoom,
  Fab
} from '@mui/material';
import {
  Close as CloseIcon,
  ChevronLeft as PrevIcon,
  ChevronRight as NextIcon,
  ZoomIn as ZoomInIcon,
  ZoomOut as ZoomOutIcon,
  Download as DownloadIcon,
  Share as ShareIcon,
  Fullscreen as FullscreenIcon,
  GridOn as GridIcon
} from '@mui/icons-material';
import { motion, AnimatePresence } from 'framer-motion';
import { Swiper, SwiperSlide } from 'swiper/react';
import { Navigation, Pagination, Thumbs, Zoom as SwiperZoom } from 'swiper/modules';
import 'swiper/css';
import 'swiper/css/navigation';
import 'swiper/css/pagination';
import 'swiper/css/thumbs';
import 'swiper/css/zoom';

interface GalleryImage {
  id: string;
  url: string;
  thumbnailUrl?: string;
  title?: string;
  description?: string;
  tags?: string[];
}

interface ImageGalleryProps {
  images?: GalleryImage[];
  layout?: 'grid' | 'masonry' | 'carousel' | 'single';
  columns?: number;
  gap?: number;
  height?: number | string;
  showTitles?: boolean;
  showDescriptions?: boolean;
  enableLightbox?: boolean;
  enableZoom?: boolean;
  enableDownload?: boolean;
  enableShare?: boolean;
  autoPlay?: boolean;
  autoPlayInterval?: number;
  data?: GalleryImage[];
  isPreview?: boolean;
  onImageClick?: (image: GalleryImage) => void;
  onDownload?: (image: GalleryImage) => void;
  onShare?: (image: GalleryImage) => void;
}

const ImageGallery: React.FC<ImageGalleryProps> = ({
  images = [],
  layout = 'grid',
  columns = 3,
  gap = 8,
  height = 400,
  showTitles = true,
  showDescriptions = false,
  enableLightbox = true,
  enableZoom = true,
  enableDownload = true,
  enableShare = true,
  autoPlay = false,
  autoPlayInterval = 3000,
  data,
  isPreview = false,
  onImageClick,
  onDownload,
  onShare
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const isTablet = useMediaQuery(theme.breakpoints.down('md'));
  
  const [lightboxOpen, setLightboxOpen] = useState(false);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [imageLoadStates, setImageLoadStates] = useState<Record<string, boolean>>({});
  const [thumbsSwiper, setThumbsSwiper] = useState(null);
  const [zoom, setZoom] = useState(1);

  const displayImages = data || images || generateMockImages();
  const responsiveColumns = isMobile ? 2 : isTablet ? Math.min(columns, 3) : columns;

  const handleImageLoad = (imageId: string) => {
    setImageLoadStates(prev => ({ ...prev, [imageId]: true }));
  };

  const handleImageClick = (image: GalleryImage, index: number) => {
    if (isPreview) return;
    
    if (enableLightbox) {
      setCurrentImageIndex(index);
      setLightboxOpen(true);
    }
    
    onImageClick?.(image);
  };

  const handleDownload = (image: GalleryImage) => {
    if (isPreview) return;
    
    if (onDownload) {
      onDownload(image);
    } else {
      // Default download behavior
      const link = document.createElement('a');
      link.href = image.url;
      link.download = image.title || 'image';
      link.click();
    }
  };

  const handleShare = (image: GalleryImage) => {
    if (isPreview) return;
    
    if (onShare) {
      onShare(image);
    } else if (navigator.share) {
      navigator.share({
        title: image.title,
        text: image.description,
        url: image.url
      });
    }
  };

  const handleZoomIn = () => {
    setZoom(prev => Math.min(prev + 0.5, 3));
  };

  const handleZoomOut = () => {
    setZoom(prev => Math.max(prev - 0.5, 1));
  };

  const renderGallery = () => {
    switch (layout) {
      case 'carousel':
        return (
          <Box sx={{ height: typeof height === 'number' ? `${height}px` : height }}>
            <Swiper
              modules={[Navigation, Pagination, Thumbs, SwiperZoom]}
              navigation
              pagination={{ clickable: true }}
              thumbs={{ swiper: thumbsSwiper }}
              zoom={enableZoom}
              autoplay={autoPlay && !isPreview ? {
                delay: autoPlayInterval,
                disableOnInteraction: false
              } : false}
              style={{ height: '80%' }}
            >
              {displayImages.map((image) => (
                <SwiperSlide key={image.id}>
                  <Box
                    sx={{
                      width: '100%',
                      height: '100%',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      backgroundColor: 'black',
                      cursor: enableLightbox ? 'pointer' : 'default'
                    }}
                    onClick={() => handleImageClick(image, displayImages.indexOf(image))}
                  >
                    {!imageLoadStates[image.id] && (
                      <Skeleton variant="rectangular" width="100%" height="100%" />
                    )}
                    <img
                      src={image.url}
                      alt={image.title || ''}
                      style={{
                        maxWidth: '100%',
                        maxHeight: '100%',
                        objectFit: 'contain',
                        display: imageLoadStates[image.id] ? 'block' : 'none'
                      }}
                      onLoad={() => handleImageLoad(image.id)}
                    />
                  </Box>
                  {showTitles && image.title && (
                    <Box
                      sx={{
                        position: 'absolute',
                        bottom: 0,
                        left: 0,
                        right: 0,
                        p: 2,
                        background: 'linear-gradient(to top, rgba(0,0,0,0.8), transparent)',
                        color: 'white'
                      }}
                    >
                      <Typography variant="h6">{image.title}</Typography>
                      {showDescriptions && image.description && (
                        <Typography variant="body2">{image.description}</Typography>
                      )}
                    </Box>
                  )}
                </SwiperSlide>
              ))}
            </Swiper>
            
            {/* Thumbnails */}
            <Box sx={{ height: '20%', pt: 1 }}>
              <Swiper
                onSwiper={setThumbsSwiper}
                spaceBetween={10}
                slidesPerView={isMobile ? 4 : 6}
                freeMode
                watchSlidesProgress
              >
                {displayImages.map((image) => (
                  <SwiperSlide key={image.id}>
                    <Box
                      sx={{
                        width: '100%',
                        height: 60,
                        cursor: 'pointer',
                        opacity: 0.6,
                        transition: 'opacity 0.3s',
                        '&:hover': { opacity: 1 }
                      }}
                    >
                      <img
                        src={image.thumbnailUrl || image.url}
                        alt={image.title || ''}
                        style={{
                          width: '100%',
                          height: '100%',
                          objectFit: 'cover'
                        }}
                      />
                    </Box>
                  </SwiperSlide>
                ))}
              </Swiper>
            </Box>
          </Box>
        );

      case 'masonry':
        return (
          <Box sx={{ width: '100%', height: typeof height === 'number' ? `${height}px` : height, overflow: 'auto' }}>
            <ImageList variant="masonry" cols={responsiveColumns} gap={gap}>
              {displayImages.map((image, index) => (
                <ImageListItem key={image.id}>
                  <motion.div
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    transition={{ duration: 0.3, delay: index * 0.05 }}
                  >
                    {!imageLoadStates[image.id] && (
                      <Skeleton variant="rectangular" width="100%" height={200 + Math.random() * 200} />
                    )}
                    <img
                      src={image.url}
                      alt={image.title || ''}
                      loading="lazy"
                      style={{
                        display: imageLoadStates[image.id] ? 'block' : 'none',
                        cursor: enableLightbox ? 'pointer' : 'default'
                      }}
                      onLoad={() => handleImageLoad(image.id)}
                      onClick={() => handleImageClick(image, index)}
                    />
                    {showTitles && image.title && (
                      <ImageListItemBar
                        title={image.title}
                        subtitle={showDescriptions ? image.description : undefined}
                      />
                    )}
                  </motion.div>
                </ImageListItem>
              ))}
            </ImageList>
          </Box>
        );

      case 'single':
        const singleImage = displayImages[0];
        if (!singleImage) return null;
        
        return (
          <Box
            sx={{
              width: '100%',
              height: typeof height === 'number' ? `${height}px` : height,
              position: 'relative',
              overflow: 'hidden',
              borderRadius: 1,
              cursor: enableLightbox ? 'pointer' : 'default'
            }}
            onClick={() => handleImageClick(singleImage, 0)}
          >
            {!imageLoadStates[singleImage.id] && (
              <Skeleton variant="rectangular" width="100%" height="100%" />
            )}
            <img
              src={singleImage.url}
              alt={singleImage.title || ''}
              style={{
                width: '100%',
                height: '100%',
                objectFit: 'cover',
                display: imageLoadStates[singleImage.id] ? 'block' : 'none'
              }}
              onLoad={() => handleImageLoad(singleImage.id)}
            />
            {showTitles && singleImage.title && (
              <Box
                sx={{
                  position: 'absolute',
                  bottom: 0,
                  left: 0,
                  right: 0,
                  p: 2,
                  background: 'linear-gradient(to top, rgba(0,0,0,0.8), transparent)',
                  color: 'white'
                }}
              >
                <Typography variant="h6">{singleImage.title}</Typography>
                {showDescriptions && singleImage.description && (
                  <Typography variant="body2">{singleImage.description}</Typography>
                )}
              </Box>
            )}
          </Box>
        );

      default: // grid
        return (
          <ImageList cols={responsiveColumns} gap={gap} sx={{ height: typeof height === 'number' ? `${height}px` : height }}>
            {displayImages.map((image, index) => (
              <ImageListItem key={image.id}>
                <motion.div
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.3, delay: index * 0.05 }}
                  style={{ height: '100%' }}
                >
                  {!imageLoadStates[image.id] && (
                    <Skeleton variant="rectangular" width="100%" height="100%" />
                  )}
                  <img
                    src={image.url}
                    alt={image.title || ''}
                    loading="lazy"
                    style={{
                      width: '100%',
                      height: '100%',
                      objectFit: 'cover',
                      display: imageLoadStates[image.id] ? 'block' : 'none',
                      cursor: enableLightbox ? 'pointer' : 'default'
                    }}
                    onLoad={() => handleImageLoad(image.id)}
                    onClick={() => handleImageClick(image, index)}
                  />
                  {showTitles && image.title && (
                    <ImageListItemBar
                      title={image.title}
                      subtitle={showDescriptions ? image.description : undefined}
                      actionIcon={
                        <Box>
                          {enableShare && (
                            <IconButton
                              sx={{ color: 'rgba(255, 255, 255, 0.54)' }}
                              onClick={(e) => {
                                e.stopPropagation();
                                handleShare(image);
                              }}
                            >
                              <ShareIcon />
                            </IconButton>
                          )}
                        </Box>
                      }
                    />
                  )}
                </motion.div>
              </ImageListItem>
            ))}
          </ImageList>
        );
    }
  };

  return (
    <>
      {renderGallery()}

      {/* Lightbox Dialog */}
      <Dialog
        fullScreen
        open={lightboxOpen && !isPreview}
        onClose={() => setLightboxOpen(false)}
        TransitionComponent={Zoom}
      >
        <Box
          sx={{
            position: 'relative',
            width: '100%',
            height: '100%',
            backgroundColor: 'black',
            display: 'flex',
            flexDirection: 'column'
          }}
        >
          {/* Toolbar */}
          <Box
            sx={{
              position: 'absolute',
              top: 0,
              left: 0,
              right: 0,
              p: 2,
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              background: 'linear-gradient(to bottom, rgba(0,0,0,0.8), transparent)',
              zIndex: 10
            }}
          >
            <Typography variant="h6" color="white">
              {displayImages[currentImageIndex]?.title || `Image ${currentImageIndex + 1} of ${displayImages.length}`}
            </Typography>
            
            <Box sx={{ display: 'flex', gap: 1 }}>
              {enableZoom && (
                <>
                  <IconButton color="inherit" onClick={handleZoomOut}>
                    <ZoomOutIcon />
                  </IconButton>
                  <IconButton color="inherit" onClick={handleZoomIn}>
                    <ZoomInIcon />
                  </IconButton>
                </>
              )}
              {enableDownload && (
                <IconButton
                  color="inherit"
                  onClick={() => handleDownload(displayImages[currentImageIndex])}
                >
                  <DownloadIcon />
                </IconButton>
              )}
              {enableShare && (
                <IconButton
                  color="inherit"
                  onClick={() => handleShare(displayImages[currentImageIndex])}
                >
                  <ShareIcon />
                </IconButton>
              )}
              <IconButton color="inherit" onClick={() => setLightboxOpen(false)}>
                <CloseIcon />
              </IconButton>
            </Box>
          </Box>

          {/* Image Viewer */}
          <Box sx={{ flex: 1, position: 'relative' }}>
            <Swiper
              modules={[Navigation, Pagination, SwiperZoom]}
              navigation
              pagination={{ clickable: true }}
              zoom={enableZoom}
              initialSlide={currentImageIndex}
              onSlideChange={(swiper) => setCurrentImageIndex(swiper.activeIndex)}
              style={{ height: '100%' }}
            >
              {displayImages.map((image) => (
                <SwiperSlide key={image.id}>
                  <div className="swiper-zoom-container">
                    <img
                      src={image.url}
                      alt={image.title || ''}
                      style={{
                        maxWidth: '100%',
                        maxHeight: '100%',
                        objectFit: 'contain',
                        transform: `scale(${zoom})`
                      }}
                    />
                  </div>
                </SwiperSlide>
              ))}
            </Swiper>
          </Box>

          {/* Description */}
          {showDescriptions && displayImages[currentImageIndex]?.description && (
            <Box
              sx={{
                position: 'absolute',
                bottom: 0,
                left: 0,
                right: 0,
                p: 3,
                background: 'linear-gradient(to top, rgba(0,0,0,0.8), transparent)',
                color: 'white'
              }}
            >
              <Typography variant="body1">
                {displayImages[currentImageIndex].description}
              </Typography>
            </Box>
          )}
        </Box>
      </Dialog>
    </>
  );
};

// Generate mock images for preview
function generateMockImages(): GalleryImage[] {
  return Array.from({ length: 9 }).map((_, index) => ({
    id: `image-${index}`,
    url: `/api/placeholder/400/300`,
    thumbnailUrl: `/api/placeholder/100/75`,
    title: `Image ${index + 1}`,
    description: `Beautiful image number ${index + 1}`,
    tags: ['nature', 'landscape']
  }));
}

export default ImageGallery;
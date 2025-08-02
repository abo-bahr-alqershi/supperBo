import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Chip,
  LinearProgress,
  IconButton,
  Skeleton,
  useTheme,
  useMediaQuery
} from '@mui/material';
import {
  Timer as TimerIcon,
  LocalOffer as OfferIcon,
  ContentCopy as CopyIcon,
  Share as ShareIcon,
  ArrowForward as ArrowIcon
} from '@mui/icons-material';
import { motion } from 'framer-motion';

interface OfferCardProps {
  title?: string;
  description?: string;
  code?: string;
  discount?: number;
  discountType?: 'percentage' | 'fixed';
  validUntil?: string;
  termsAndConditions?: string;
  backgroundColor?: string;
  accentColor?: string;
  imageUrl?: string;
  showTimer?: boolean;
  showProgress?: boolean;
  usageLimit?: number;
  usageCount?: number;
  cardStyle?: 'default' | 'minimal' | 'gradient' | 'bordered';
  data?: {
    id: string;
    title: string;
    description: string;
    code: string;
    discount: number;
    discountType: 'percentage' | 'fixed';
    validUntil: string;
    usageLimit?: number;
    usageCount?: number;
  };
  isPreview?: boolean;
  onApply?: (code: string) => void;
  onShare?: (offer: any) => void;
}

const OfferCard: React.FC<OfferCardProps> = ({
  title = 'Special Offer',
  description = 'Get amazing discounts on your next purchase',
  code = 'SAVE20',
  discount = 20,
  discountType = 'percentage',
  validUntil = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
  termsAndConditions = 'Terms and conditions apply',
  backgroundColor = '#f5f5f5',
  accentColor = '#1976d2',
  imageUrl,
  showTimer = true,
  showProgress = true,
  usageLimit = 100,
  usageCount = 30,
  cardStyle = 'default',
  data,
  isPreview = false,
  onApply,
  onShare
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [copied, setCopied] = useState(false);
  const [timeLeft, setTimeLeft] = useState<{
    days: number;
    hours: number;
    minutes: number;
    seconds: number;
  }>({ days: 0, hours: 0, minutes: 0, seconds: 0 });

  const offerData = data || {
    title,
    description,
    code,
    discount,
    discountType,
    validUntil,
    usageLimit,
    usageCount
  };

  useEffect(() => {
    if (!showTimer || isPreview) return;

    const calculateTimeLeft = () => {
      const difference = new Date(offerData.validUntil).getTime() - new Date().getTime();
      
      if (difference > 0) {
        setTimeLeft({
          days: Math.floor(difference / (1000 * 60 * 60 * 24)),
          hours: Math.floor((difference / (1000 * 60 * 60)) % 24),
          minutes: Math.floor((difference / 1000 / 60) % 60),
          seconds: Math.floor((difference / 1000) % 60)
        });
      }
    };

    calculateTimeLeft();
    const timer = setInterval(calculateTimeLeft, 1000);

    return () => clearInterval(timer);
  }, [offerData.validUntil, showTimer, isPreview]);

  const handleCopyCode = () => {
    if (isPreview) return;
    
    navigator.clipboard.writeText(offerData.code);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const formatDiscount = () => {
    if (offerData.discountType === 'percentage') {
      return `${offerData.discount}% OFF`;
    } else {
      return `$${offerData.discount} OFF`;
    }
  };

  const usagePercentage = offerData.usageLimit 
    ? (offerData.usageCount / offerData.usageLimit) * 100 
    : 0;

  const renderCardContent = () => {
    switch (cardStyle) {
      case 'minimal':
        return (
          <Card sx={{ overflow: 'hidden' }}>
            <CardContent sx={{ p: 3 }}>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', mb: 2 }}>
                <Box>
                  <Typography variant="h6" gutterBottom>
                    {offerData.title}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {offerData.description}
                  </Typography>
                </Box>
                <Chip
                  label={formatDiscount()}
                  color="primary"
                  sx={{ fontWeight: 'bold', fontSize: 16 }}
                />
              </Box>

              <Box
                sx={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: 2,
                  p: 2,
                  backgroundColor: 'grey.100',
                  borderRadius: 1,
                  mb: 2
                }}
              >
                <Typography variant="body1" fontWeight="medium">
                  {offerData.code}
                </Typography>
                <IconButton size="small" onClick={handleCopyCode}>
                  <CopyIcon fontSize="small" />
                </IconButton>
              </Box>

              {showTimer && timeLeft.days >= 0 && (
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <TimerIcon fontSize="small" color="action" />
                  <Typography variant="body2" color="text.secondary">
                    Expires in {timeLeft.days}d {timeLeft.hours}h {timeLeft.minutes}m
                  </Typography>
                </Box>
              )}
            </CardContent>
          </Card>
        );

      case 'gradient':
        return (
          <Card
            sx={{
              background: `linear-gradient(135deg, ${accentColor} 0%, ${theme.palette.primary.dark} 100%)`,
              color: 'white',
              overflow: 'hidden'
            }}
          >
            <CardContent sx={{ p: 3 }}>
              <Box sx={{ textAlign: 'center', mb: 3 }}>
                <Typography variant="h3" fontWeight="bold" gutterBottom>
                  {formatDiscount()}
                </Typography>
                <Typography variant="h5" gutterBottom>
                  {offerData.title}
                </Typography>
                <Typography variant="body1" sx={{ opacity: 0.9 }}>
                  {offerData.description}
                </Typography>
              </Box>

              <Box
                sx={{
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  gap: 2,
                  p: 2,
                  backgroundColor: 'rgba(255,255,255,0.2)',
                  borderRadius: 2,
                  backdropFilter: 'blur(10px)',
                  mb: 3
                }}
              >
                <OfferIcon />
                <Typography variant="h6" fontWeight="bold">
                  {offerData.code}
                </Typography>
                <IconButton
                  size="small"
                  onClick={handleCopyCode}
                  sx={{ color: 'white' }}
                >
                  <CopyIcon />
                </IconButton>
              </Box>

              <Button
                fullWidth
                variant="contained"
                size="large"
                onClick={() => !isPreview && onApply?.(offerData.code)}
                sx={{
                  backgroundColor: 'white',
                  color: accentColor,
                  '&:hover': {
                    backgroundColor: 'rgba(255,255,255,0.9)'
                  }
                }}
              >
                Apply Code
              </Button>
            </CardContent>
          </Card>
        );

      case 'bordered':
        return (
          <Card
            sx={{
              border: 2,
              borderColor: accentColor,
              borderStyle: 'dashed',
              backgroundColor: 'transparent'
            }}
          >
            <CardContent sx={{ p: 3, textAlign: 'center' }}>
              <Box
                sx={{
                  display: 'inline-flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  width: 80,
                  height: 80,
                  borderRadius: '50%',
                  backgroundColor: accentColor,
                  color: 'white',
                  mb: 2
                }}
              >
                <Typography variant="h5" fontWeight="bold">
                  {offerData.discount}{offerData.discountType === 'percentage' ? '%' : '$'}
                </Typography>
              </Box>

              <Typography variant="h6" gutterBottom>
                {offerData.title}
              </Typography>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                {offerData.description}
              </Typography>

              <Chip
                label={offerData.code}
                onClick={handleCopyCode}
                icon={<CopyIcon />}
                sx={{
                  mt: 2,
                  mb: 2,
                  fontSize: 16,
                  py: 2,
                  backgroundColor: backgroundColor
                }}
              />

              {showTimer && (
                <Typography variant="caption" color="text.secondary" display="block">
                  Valid for {timeLeft.days} days
                </Typography>
              )}
            </CardContent>
          </Card>
        );

      default:
        return (
          <Card sx={{ overflow: 'hidden', backgroundColor }}>
            {imageUrl && (
              <Box
                sx={{
                  height: 120,
                  backgroundImage: `url(${imageUrl})`,
                  backgroundSize: 'cover',
                  backgroundPosition: 'center',
                  position: 'relative'
                }}
              >
                <Box
                  sx={{
                    position: 'absolute',
                    top: 16,
                    right: 16,
                    backgroundColor: accentColor,
                    color: 'white',
                    px: 2,
                    py: 1,
                    borderRadius: 2,
                    fontWeight: 'bold'
                  }}
                >
                  {formatDiscount()}
                </Box>
              </Box>
            )}

            <CardContent sx={{ p: 3 }}>
              {!imageUrl && (
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', mb: 2 }}>
                  <OfferIcon sx={{ color: accentColor, fontSize: 40 }} />
                  <Chip
                    label={formatDiscount()}
                    sx={{
                      backgroundColor: accentColor,
                      color: 'white',
                      fontWeight: 'bold',
                      fontSize: 18
                    }}
                  />
                </Box>
              )}

              <Typography variant="h6" gutterBottom>
                {offerData.title}
              </Typography>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                {offerData.description}
              </Typography>

              <Box
                sx={{
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'space-between',
                  p: 2,
                  my: 2,
                  backgroundColor: theme.palette.grey[100],
                  borderRadius: 1,
                  border: `1px dashed ${theme.palette.grey[400]}`
                }}
              >
                <Box>
                  <Typography variant="caption" color="text.secondary">
                    Promo Code
                  </Typography>
                  <Typography variant="h6">{offerData.code}</Typography>
                </Box>
                <IconButton onClick={handleCopyCode} color="primary">
                  {copied ? '✓' : <CopyIcon />}
                </IconButton>
              </Box>

              {showProgress && offerData.usageLimit && (
                <Box sx={{ mb: 2 }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                    <Typography variant="caption" color="text.secondary">
                      {offerData.usageCount} used
                    </Typography>
                    <Typography variant="caption" color="text.secondary">
                      {offerData.usageLimit - offerData.usageCount} left
                    </Typography>
                  </Box>
                  <LinearProgress
                    variant="determinate"
                    value={usagePercentage}
                    sx={{
                      height: 6,
                      borderRadius: 3,
                      backgroundColor: theme.palette.grey[200],
                      '& .MuiLinearProgress-bar': {
                        backgroundColor: accentColor
                      }
                    }}
                  />
                </Box>
              )}

              {showTimer && timeLeft.days >= 0 && (
                <Box
                  sx={{
                    display: 'flex',
                    justifyContent: 'space-around',
                    p: 2,
                    backgroundColor: theme.palette.grey[50],
                    borderRadius: 1,
                    mb: 2
                  }}
                >
                  {[
                    { label: 'Days', value: timeLeft.days },
                    { label: 'Hours', value: timeLeft.hours },
                    { label: 'Min', value: timeLeft.minutes },
                    { label: 'Sec', value: timeLeft.seconds }
                  ].map((unit) => (
                    <Box key={unit.label} sx={{ textAlign: 'center' }}>
                      <Typography variant="h6" fontWeight="bold">
                        {String(unit.value).padStart(2, '0')}
                      </Typography>
                      <Typography variant="caption" color="text.secondary">
                        {unit.label}
                      </Typography>
                    </Box>
                  ))}
                </Box>
              )}

              <Box sx={{ display: 'flex', gap: 1 }}>
                <Button
                  fullWidth
                  variant="contained"
                  onClick={() => !isPreview && onApply?.(offerData.code)}
                  sx={{ backgroundColor: accentColor }}
                >
                  Apply Now
                </Button>
                <IconButton
                  onClick={() => !isPreview && onShare?.(offerData)}
                  sx={{ border: 1, borderColor: 'divider' }}
                >
                  <ShareIcon />
                </IconButton>
              </Box>

              {termsAndConditions && (
                <Typography variant="caption" color="text.secondary" sx={{ mt: 2, display: 'block' }}>
                  * {termsAndConditions}
                </Typography>
              )}
            </CardContent>
          </Card>
        );
    }
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
    >
      {renderCardContent()}
    </motion.div>
  );
};

export default OfferCard;
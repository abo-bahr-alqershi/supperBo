// frontend/src/components/Home/Sections/LimitedTimeOffer.tsx
import React from 'react';
import { Box, Paper, Typography, Button, LinearProgress } from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import { Timer, LocalOffer } from '@mui/icons-material';
import CountdownTimer from '../Common/CountdownTimer';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';

interface LimitedTimeOfferProps {
  section: DynamicHomeSection;
}

const LimitedTimeOffer: React.FC<LimitedTimeOfferProps> = ({ section }) => {
  const navigate = useNavigate();
  
  const offerContent = section.content[0];
  if (!offerContent) return null;
  
  const offer = offerContent.contentData;
  const expiresAt = offerContent.expiresAt || section.expiresAt || 
    new Date(Date.now() + 48 * 60 * 60 * 1000).toISOString();

  // حساب نسبة الوقت المتبقي
  const totalTime = new Date(expiresAt).getTime() - new Date(offer.startDate || Date.now()).getTime();
  const remainingTime = new Date(expiresAt).getTime() - Date.now();
  const progressPercentage = Math.max(0, Math.min(100, (remainingTime / totalTime) * 100));

  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.95 }}
      animate={{ opacity: 1, scale: 1 }}
      transition={{ duration: 0.4 }}
    >
      <Paper
        elevation={6}
        sx={{
          p: 4,
          background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
          color: 'white',
          position: 'relative',
          overflow: 'hidden',
        }}
      >
        {/* خلفية متحركة */}
        <Box
          sx={{
            position: 'absolute',
            top: -100,
            right: -100,
            width: 300,
            height: 300,
            borderRadius: '50%',
            backgroundColor: 'rgba(255, 255, 255, 0.1)',
            animation: 'float 6s ease-in-out infinite',
          }}
        />
        
        <Box sx={{ position: 'relative', zIndex: 1 }}>
          {/* رأس العرض */}
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 4 }}>
            <Box>
              <Typography variant="h4" fontWeight="bold" gutterBottom>
                {offer.title || section.title}
              </Typography>
              {(offer.subtitle || section.subtitle) && (
                <Typography variant="body1" sx={{ opacity: 0.9 }}>
                  {offer.subtitle || section.subtitle}
                </Typography>
              )}
            </Box>
            
            <Timer sx={{ fontSize: 60, opacity: 0.3 }} />
          </Box>
          
          {/* العد التنازلي */}
          <Box sx={{ mb: 4 }}>
            <Typography variant="body2" sx={{ mb: 2, opacity: 0.9 }}>
              الوقت المتبقي للعرض:
            </Typography>
            <CountdownTimer endDate={expiresAt} />
            
            <LinearProgress
              variant="determinate"
              value={progressPercentage}
              sx={{
                mt: 2,
                height: 8,
                borderRadius: 4,
                backgroundColor: 'rgba(255, 255, 255, 0.3)',
                '& .MuiLinearProgress-bar': {
                  borderRadius: 4,
                  backgroundColor: 'white',
                },
              }}
            />
          </Box>
          
          {/* تفاصيل العرض */}
          {offer.property && (
            <Paper
              sx={{
                p: 3,
                backgroundColor: 'rgba(255, 255, 255, 0.1)',
                backdropFilter: 'blur(10px)',
                mb: 3,
              }}
            >
              <Typography variant="h6" fontWeight="medium" gutterBottom>
                {offer.property.name}
              </Typography>
              {offer.property.basePrice && (
                <Box sx={{ display: 'flex', alignItems: 'baseline', gap: 2 }}>
                  <Typography variant="h4" fontWeight="bold">
                    {offer.property.basePrice.toLocaleString()} {offer.property.currency}
                  </Typography>
                  <Typography variant="body2">لليلة الواحدة</Typography>
                </Box>
              )}
            </Paper>
          )}
          
          {/* زر الإجراء */}
          <Button
            variant="contained"
            size="large"
            fullWidth
            onClick={() => {
              if (offer.ctaAction === 'navigate' && offer.ctaData?.route) {
                navigate(offer.ctaData.route);
              }
            }}
            startIcon={<LocalOffer />}
            sx={{
              backgroundColor: 'white',
              color: 'primary.main',
              '&:hover': {
                backgroundColor: 'grey.100',
              },
            }}
          >
            {offer.ctaText || 'احصل على العرض'}
          </Button>
        </Box>
        
        <style>
          {`
            @keyframes float {
              0% { transform: translateY(0px); }
              50% { transform: translateY(-20px); }
              100% { transform: translateY(0px); }
            }
          `}
        </style>
      </Paper>
    </motion.div>
  );
};

export default LimitedTimeOffer;
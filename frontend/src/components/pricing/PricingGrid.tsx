import React, { useState, useCallback } from 'react';
import {
  Box,
  Grid,
  Card,
  CardContent,
  Typography,
  Chip,
  IconButton,
  Tooltip,
  Menu,
  MenuItem,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Alert,
  CircularProgress,
  Badge,
  Zoom,
  LinearProgress
} from '@mui/material';
import {
  TrendingUp as TrendingUpIcon,
  TrendingDown as TrendingDownIcon,
  AttachMoney as AttachMoneyIcon,
  MoreVert as MoreVertIcon,
  Edit as EditIcon,
  Visibility as VisibilityIcon,
  Warning as WarningIcon,
  Star as StarIcon,
  Schedule as ScheduleIcon
} from '@mui/icons-material';
import { styled } from '@mui/material/styles';
import type { 
  UnitManagementData, 
  PricingTier, 
  PricingRule,
  PricingError 
} from '../../types/availability_types';
import ResponsiveGrid from '../common/ResponsiveGrid';

// ===== الأنماط المخصصة =====
const StyledCard = styled(Card, {
  shouldForwardProp: (prop) => prop !== 'pricingTier' && prop !== 'hasActiveRules'
})<{ pricingTier: PricingTier; hasActiveRules?: boolean }>(({ theme, pricingTier, hasActiveRules }) => {
  const getTierColor = () => {
    switch (pricingTier) {
      case 'normal':
        return {
          background: 'linear-gradient(135deg, rgba(76, 175, 80, 0.1), rgba(76, 175, 80, 0.2))',
          border: `2px solid ${theme.palette.success.main}`,
          color: theme.palette.success.main
        };
      case 'high':
        return {
          background: 'linear-gradient(135deg, rgba(255, 193, 7, 0.1), rgba(255, 193, 7, 0.2))',
          border: `2px solid ${theme.palette.warning.main}`,
          color: theme.palette.warning.main
        };
      case 'peak':
        return {
          background: 'linear-gradient(135deg, rgba(244, 67, 54, 0.1), rgba(244, 67, 54, 0.2))',
          border: `2px solid ${theme.palette.error.main}`,
          color: theme.palette.error.main
        };
      case 'discount':
        return {
          background: 'linear-gradient(135deg, rgba(33, 150, 243, 0.1), rgba(33, 150, 243, 0.2))',
          border: `2px solid ${theme.palette.primary.main}`,
          color: theme.palette.primary.main
        };
      case 'custom':
        return {
          background: 'linear-gradient(135deg, rgba(156, 39, 176, 0.1), rgba(156, 39, 176, 0.2))',
          border: `2px solid ${theme.palette.secondary.main}`,
          color: theme.palette.secondary.main
        };
      default:
        return {
          background: theme.palette.grey[50],
          border: `2px solid ${theme.palette.grey[300]}`,
          color: theme.palette.grey[600]
        };
    }
  };

  const tierColors = getTierColor();

  return {
    ...tierColors,
    cursor: 'pointer',
    transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
    position: 'relative',
    overflow: 'visible',
    '&:hover': {
      transform: 'translateY(-4px)',
      boxShadow: theme.shadows[8],
      '& .action-buttons': {
        opacity: 1,
        transform: 'translateY(0)'
      },
      '& .price-overlay': {
        opacity: 1
      }
    },
    ...(hasActiveRules && {
      '&::after': {
        content: '""',
        position: 'absolute',
        top: 4,
        right: 4,
        width: 12,
        height: 12,
        backgroundColor: theme.palette.success.main,
        borderRadius: '50%',
        animation: 'pulse 2s infinite'
      }
    })
  };
});

const ActionButtonsContainer = styled(Box)(({ theme }) => ({
  position: 'absolute',
  top: theme.spacing(1),
  right: theme.spacing(1),
  opacity: 0,
  transform: 'translateY(-10px)',
  transition: 'all 0.3s ease',
  display: 'flex',
  gap: theme.spacing(1),
  zIndex: 10
}));

const PriceOverlay = styled(Box)(({ theme }) => ({
  position: 'absolute',
  bottom: 0,
  left: 0,
  right: 0,
  background: 'linear-gradient(180deg, transparent, rgba(0, 0, 0, 0.8))',
  color: 'white',
  padding: theme.spacing(2),
  opacity: 0,
  transition: 'all 0.3s ease',
  borderRadius: `0 0 ${theme.shape.borderRadius}px ${theme.shape.borderRadius}px`
}));

const PricingTierChip = styled(Chip, {
  shouldForwardProp: (prop) => prop !== 'tier'
})<{ tier: PricingTier }>(({ theme, tier }) => {
  const getTierChipColors = () => {
    switch (tier) {
      case 'normal':
        return { backgroundColor: theme.palette.success.main, color: 'white' };
      case 'high':
        return { backgroundColor: theme.palette.warning.main, color: 'white' };
      case 'peak':
        return { backgroundColor: theme.palette.error.main, color: 'white' };
      case 'discount':
        return { backgroundColor: theme.palette.primary.main, color: 'white' };
      case 'custom':
        return { backgroundColor: theme.palette.secondary.main, color: 'white' };
      default:
        return { backgroundColor: theme.palette.grey[300], color: theme.palette.grey[800] };
    }
  };

  return {
    ...getTierChipColors(),
    fontWeight: 600,
    fontSize: '0.75rem'
  };
});

// ===== أنواع البيانات للمكون =====
interface PricingGridProps {
  units: UnitManagementData[];
  loading?: boolean;
  error?: PricingError | null;
  onUnitClick: (unit: UnitManagementData) => void;
  onUnitEdit?: (unit: UnitManagementData) => void;
  onViewDetails?: (unit: UnitManagementData) => void;
  onOptimizePrice?: (unit: UnitManagementData) => void;
  selectedUnits?: string[];
  onSelectionChange?: (selectedIds: string[]) => void;
  showHighPricesOnly?: boolean;
  compact?: boolean;
  showPriceComparison?: boolean;
}

// ===== دوال مساعدة =====
const getPricingTier = (currentPrice: number, basePrice: number): PricingTier => {
  const ratio = currentPrice / basePrice;
  
  if (ratio >= 1.5) return 'peak';
  if (ratio >= 1.2) return 'high';
  if (ratio <= 0.8) return 'discount';
  if (ratio !== 1) return 'custom';
  return 'normal';
};

const getPricingTierText = (tier: PricingTier): string => {
  switch (tier) {
    case 'normal':
      return 'عادي';
    case 'high':
      return 'مرتفع';
    case 'peak':
      return 'ذروة';
    case 'discount':
      return 'خصم';
    case 'custom':
      return 'مخصص';
    default:
      return 'غير محدد';
  }
};

const getPricingTierIcon = (tier: PricingTier) => {
  switch (tier) {
    case 'peak':
      return <TrendingUpIcon />;
    case 'high':
      return <TrendingUpIcon />;
    case 'discount':
      return <TrendingDownIcon />;
    case 'custom':
      return <StarIcon />;
    default:
      return <AttachMoneyIcon />;
  }
};

const formatPrice = (price: number): string => {
  return new Intl.NumberFormat('ar-SA', {
    style: 'currency',
    currency: 'SAR',
    minimumFractionDigits: 0
  }).format(price);
};

const calculatePriceChange = (currentPrice: number, basePrice: number): { percentage: number; direction: 'up' | 'down' | 'same' } => {
  const change = ((currentPrice - basePrice) / basePrice) * 100;
  return {
    percentage: Math.abs(change),
    direction: change > 0 ? 'up' : change < 0 ? 'down' : 'same'
  };
};

const getActivePricingRule = (unit: UnitManagementData): PricingRule | null => {
  const now = new Date();
  return unit.activePricingRules?.find(rule => 
    new Date(rule.startDate) <= now && 
    new Date(rule.endDate) >= now &&
    rule.isActive
  ) || null;
};

// ===== المكون الرئيسي =====
const PricingGrid: React.FC<PricingGridProps> = ({
  units,
  loading = false,
  error = null,
  onUnitClick,
  onUnitEdit,
  onViewDetails,
  onOptimizePrice,
  selectedUnits = [],
  onSelectionChange,
  showHighPricesOnly = false,
  compact = false,
  showPriceComparison = true
}) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [selectedUnit, setSelectedUnit] = useState<UnitManagementData | null>(null);
  const [optimizeDialogOpen, setOptimizeDialogOpen] = useState(false);

  // تصفية الوحدات حسب إعدادات العرض
  const filteredUnits = units.filter(unit => {
    if (showHighPricesOnly) {
      const activePricing = getActivePricingRule(unit);
      const currentPrice = activePricing?.priceAmount || unit.unit.basePrice;
      const tier = getPricingTier(currentPrice, unit.unit.basePrice);
      return tier === 'high' || tier === 'peak';
    }
    return true;
  });

  // معالجة النقر على الوحدة
  const handleUnitClick = useCallback((unit: UnitManagementData, event: React.MouseEvent) => {
    event.stopPropagation();
    onUnitClick(unit);
  }, [onUnitClick]);

  // معالجة قائمة الإجراءات
  const handleMenuClick = useCallback((event: React.MouseEvent, unit: UnitManagementData) => {
    event.stopPropagation();
    setAnchorEl(event.currentTarget as HTMLElement);
    setSelectedUnit(unit);
  }, []);

  const handleMenuClose = useCallback(() => {
    setAnchorEl(null);
    setSelectedUnit(null);
  }, []);

  // معالجة الإجراءات
  const handleEdit = useCallback(() => {
    if (selectedUnit && onUnitEdit) {
      onUnitEdit(selectedUnit);
    }
    handleMenuClose();
  }, [selectedUnit, onUnitEdit, handleMenuClose]);

  const handleViewDetails = useCallback(() => {
    if (selectedUnit && onViewDetails) {
      onViewDetails(selectedUnit);
    }
    handleMenuClose();
  }, [selectedUnit, onViewDetails, handleMenuClose]);

  const handleOptimize = useCallback(() => {
    if (selectedUnit) {
      setOptimizeDialogOpen(true);
    }
    handleMenuClose();
  }, [selectedUnit, handleMenuClose]);

  const handleOptimizeConfirm = useCallback(() => {
    if (selectedUnit && onOptimizePrice) {
      onOptimizePrice(selectedUnit);
    }
    setOptimizeDialogOpen(false);
  }, [selectedUnit, onOptimizePrice]);

  // معالجة حالة التحميل
  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight={400}>
        <CircularProgress size={60} />
        <Typography variant="h6" sx={{ ml: 2 }}>
          جاري تحميل بيانات التسعير...
        </Typography>
      </Box>
    );
  }

  // معالجة حالة الخطأ
  if (error) {
    return (
      <Alert 
        severity="error" 
        sx={{ mb: 2 }}
        action={
          <Button color="inherit" size="small">
            إعادة المحاولة
          </Button>
        }
      >
        <Typography variant="subtitle1" gutterBottom>
          خطأ في تحميل بيانات التسعير
        </Typography>
        <Typography variant="body2">
          {error.message}
        </Typography>
        {error.suggested_action && (
          <Typography variant="caption" display="block" sx={{ mt: 1 }}>
            الحل المقترح: {error.suggested_action}
          </Typography>
        )}
      </Alert>
    );
  }

  // معالجة حالة عدم وجود بيانات
  if (filteredUnits.length === 0) {
    return (
      <Box textAlign="center" py={6}>
        <Typography variant="h6" color="text.secondary" gutterBottom>
          لا توجد وحدات متاحة
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {showHighPricesOnly 
            ? 'لا توجد وحدات بأسعار مرتفعة حالياً'
            : 'لم يتم العثور على أي وحدات تطابق المعايير المحددة'
          }
        </Typography>
      </Box>
    );
  }

  return (
    <>
      <ResponsiveGrid container spacing={compact ? 2 : 3}>
        {filteredUnits.map((unit) => {
          const activePricing = getActivePricingRule(unit);
          const currentPrice = activePricing?.priceAmount || unit.unit.basePrice;
          const pricingTier = getPricingTier(currentPrice, unit.unit.basePrice);
          const priceChange = calculatePriceChange(currentPrice, unit.unit.basePrice);
          const isSelected = selectedUnits.includes(unit.unit.unitId);
          const hasActiveRules = unit.activePricingRules && unit.activePricingRules.length > 0;
          
          return (
            <ResponsiveGrid 
              item 
              xs={12} 
              sm={compact ? 4 : 6} 
              md={compact ? 3 : 4} 
              lg={compact ? 2 : 3} 
              key={unit.unit.unitId}
            >
              <Zoom in={true} timeout={300}>
                <StyledCard
                  pricingTier={pricingTier}
                  hasActiveRules={hasActiveRules}
                  onClick={(e) => handleUnitClick(unit, e)}
                  elevation={isSelected ? 8 : 2}
                  sx={{
                    height: compact ? 200 : 240,
                    position: 'relative',
                    ...(isSelected && {
                      transform: 'scale(1.02)',
                      zIndex: 10
                    })
                  }}
                >
                  {/* أزرار الإجراءات */}
                  <ActionButtonsContainer className="action-buttons">
                    <Tooltip title="المزيد من الخيارات">
                      <IconButton
                        size="small"
                        onClick={(e) => handleMenuClick(e, unit)}
                        sx={{ 
                          backgroundColor: 'rgba(255, 255, 255, 0.9)',
                          '&:hover': { backgroundColor: 'white' }
                        }}
                      >
                        <MoreVertIcon fontSize="small" />
                      </IconButton>
                    </Tooltip>
                  </ActionButtonsContainer>

                  {/* مؤشر القواعد النشطة */}
                  {hasActiveRules && (
                    <Badge
                      badgeContent={unit.activePricingRules?.length || 0}
                      color="primary"
                      sx={{
                        position: 'absolute',
                        top: 8,
                        left: 8,
                        zIndex: 10
                      }}
                    >
                      <Tooltip title="يوجد قواعد تسعير نشطة">
                        <ScheduleIcon color="primary" />
                      </Tooltip>
                    </Badge>
                  )}

                  <CardContent sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                    {/* معلومات الوحدة */}
                    <Box flex={1}>
                      <Typography 
                        variant={compact ? "subtitle1" : "h6"} 
                        component="h3" 
                        gutterBottom
                        sx={{ 
                          fontWeight: 600,
                          overflow: 'hidden',
                          textOverflow: 'ellipsis',
                          whiteSpace: 'nowrap'
                        }}
                      >
                        {unit.unit.unitName}
                      </Typography>
                      
                      <Typography 
                        variant="body2" 
                        color="text.secondary" 
                        gutterBottom
                        sx={{ 
                          overflow: 'hidden',
                          textOverflow: 'ellipsis',
                          whiteSpace: 'nowrap'
                        }}
                      >
                        {unit.unit.unitType} • {unit.unit.capacity} أشخاص
                      </Typography>

                      {/* السعر الحالي */}
                      <Box sx={{ mt: 2 }}>
                        <Typography 
                          variant="h5" 
                          sx={{ 
                            fontWeight: 700,
                            color: 'primary.main',
                            display: 'flex',
                            alignItems: 'center',
                            gap: 1
                          }}
                        >
                          {formatPrice(currentPrice)}
                          {getPricingTierIcon(pricingTier)}
                        </Typography>
                        
                        {/* السعر الأساسي والتغيير */}
                        {showPriceComparison && currentPrice !== unit.unit.basePrice && (
                          <Box display="flex" alignItems="center" gap={1} mt={0.5}>
                            <Typography 
                              variant="body2" 
                              color="text.secondary"
                              sx={{ textDecoration: 'line-through' }}
                            >
                              {formatPrice(unit.unit.basePrice)}
                            </Typography>
                            <Chip
                              size="small"
                              label={`${priceChange.direction === 'up' ? '+' : '-'}${priceChange.percentage.toFixed(1)}%`}
                              color={priceChange.direction === 'up' ? 'error' : 'success'}
                              sx={{ fontSize: '0.7rem', height: 20 }}
                            />
                          </Box>
                        )}
                      </Box>
                    </Box>

                    {/* فئة التسعير */}
                    <Box 
                      display="flex" 
                      alignItems="center" 
                      justifyContent="space-between"
                      mt={2}
                    >
                      <Box display="flex" alignItems="center" gap={1}>
                        {getPricingTierIcon(pricingTier)}
                        <PricingTierChip
                          tier={pricingTier}
                          label={getPricingTierText(pricingTier)}
                          size="small"
                        />
                      </Box>

                      {/* مؤشر الطلب */}
                      {unit.upcomingBookings && unit.upcomingBookings.length > 0 && (
                        <Tooltip title="مستوى الطلب">
                          <Box sx={{ minWidth: 50 }}>
                            <LinearProgress 
                              variant="determinate" 
                              value={Math.min(unit.upcomingBookings.length * 20, 100)}
                              color={unit.upcomingBookings.length > 3 ? 'error' : 'primary'}
                              sx={{ height: 6, borderRadius: 3 }}
                            />
                          </Box>
                        </Tooltip>
                      )}
                    </Box>

                    {/* معلومات إضافية عند التمرير */}
                    {activePricing && (
                      <PriceOverlay className="price-overlay">
                        <Typography variant="caption" display="block">
                          قاعدة نشطة: {activePricing.description || activePricing.priceType}
                        </Typography>
                        <Typography variant="caption" display="block">
                          صالحة حتى: {new Date(activePricing.endDate).toLocaleDateString('ar-SA')}
                        </Typography>
                      </PriceOverlay>
                    )}
                  </CardContent>
                </StyledCard>
              </Zoom>
            </ResponsiveGrid>
          );
        })}
      </ResponsiveGrid>

      {/* قائمة الإجراءات */}
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
      >
        {onViewDetails && (
          <MenuItem onClick={handleViewDetails}>
            <VisibilityIcon sx={{ mr: 1 }} fontSize="small" />
            عرض تفاصيل التسعير
          </MenuItem>
        )}
        {onUnitEdit && (
          <MenuItem onClick={handleEdit}>
            <EditIcon sx={{ mr: 1 }} fontSize="small" />
            تعديل التسعير
          </MenuItem>
        )}
        {onOptimizePrice && (
          <MenuItem onClick={handleOptimize}>
            <TrendingUpIcon sx={{ mr: 1 }} fontSize="small" />
            تحسين السعر
          </MenuItem>
        )}
      </Menu>

      {/* مربع حوار تحسين الأسعار */}
      <Dialog
        open={optimizeDialogOpen}
        onClose={() => setOptimizeDialogOpen(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>
          تحسين التسعير التلقائي
        </DialogTitle>
        <DialogContent>
          <Alert severity="info" sx={{ mb: 2 }}>
            سيتم تحليل السوق والطلب لاقتراح أفضل الأسعار لهذه الوحدة.
          </Alert>
          
          {selectedUnit && (
            <Box>
              <Typography variant="subtitle1" gutterBottom>
                الوحدة: {selectedUnit.unit.unitName}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                السعر الحالي: {formatPrice(getActivePricingRule(selectedUnit)?.priceAmount || selectedUnit.unit.basePrice)}
              </Typography>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOptimizeDialogOpen(false)}>
            إلغاء
          </Button>
          <Button 
            onClick={handleOptimizeConfirm} 
            variant="contained" 
            color="primary"
          >
            تطبيق التحسين
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default PricingGrid;
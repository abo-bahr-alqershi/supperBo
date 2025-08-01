import React, { useState, useEffect, useCallback } from 'react';
import { 
  Box, 
  Grid, 
  Card, 
  CardContent, 
  Typography, 
  Chip, 
  IconButton, 
  Tooltip, 
  Badge,
  Menu,
  MenuItem,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Alert,
  CircularProgress,
  Fade,
  Zoom
} from '@mui/material';
import {
  CheckCircle as CheckCircleIcon,
  Cancel as CancelIcon,
  Build as BuildIcon,
  Block as BlockIcon,
  Event as EventIcon,
  MoreVert as MoreVertIcon,
  Edit as EditIcon,
  Visibility as VisibilityIcon,
  Warning as WarningIcon
} from '@mui/icons-material';
import { styled } from '@mui/material/styles';
import type { 
  UnitManagementData, 
  AvailabilityStatus, 
  BookingConflict,
  AvailabilityError 
} from '../../types/availability_types';

// ===== الأنماط المخصصة =====
const StyledCard = styled(Card, {
  shouldForwardProp: (prop) => prop !== 'status' && prop !== 'hasConflicts' && prop !== 'compact' && prop !== 'isActive'
})<{ status: AvailabilityStatus; hasConflicts?: boolean }>(({ theme, status, hasConflicts }) => {
  const getStatusColor = () => {
    switch (status) {
      case 'available':
        return {
          background: 'linear-gradient(135deg, rgba(76, 175, 80, 0.1), rgba(76, 175, 80, 0.2))',
          border: `2px solid ${theme.palette.success.main}`,
          color: theme.palette.success.main
        };
      case 'unavailable':
        return {
          background: 'linear-gradient(135deg, rgba(244, 67, 54, 0.1), rgba(244, 67, 54, 0.2))',
          border: `2px solid ${theme.palette.error.main}`,
          color: theme.palette.error.main
        };
      case 'maintenance':
        return {
          background: 'linear-gradient(135deg, rgba(255, 152, 0, 0.1), rgba(255, 152, 0, 0.2))',
          border: `2px solid ${theme.palette.warning.main}`,
          color: theme.palette.warning.main
        };
      case 'blocked':
        return {
          background: 'linear-gradient(135deg, rgba(158, 158, 158, 0.1), rgba(158, 158, 158, 0.2))',
          border: `2px solid ${theme.palette.grey[600]}`,
          color: theme.palette.grey[600]
        };
      case 'booked':
        return {
          background: 'linear-gradient(135deg, rgba(33, 150, 243, 0.1), rgba(33, 150, 243, 0.2))',
          border: `2px solid ${theme.palette.primary.main}`,
          color: theme.palette.primary.main
        };
      default:
        return {
          background: theme.palette.grey[50],
          border: `2px solid ${theme.palette.grey[300]}`,
          color: theme.palette.grey[600]
        };
    }
  };

  const statusColors = getStatusColor();

  return {
    ...statusColors,
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
      }
    },
    ...(hasConflicts && {
      '&::before': {
        content: '""',
        position: 'absolute',
        top: -2,
        left: -2,
        right: -2,
        bottom: -2,
        background: `linear-gradient(45deg, ${theme.palette.error.main}, ${theme.palette.warning.main})`,
        borderRadius: `${theme.shape.borderRadius}px`,
        zIndex: -1,
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

const StatusChip = styled(Chip, {
  shouldForwardProp: (prop) => prop !== 'status' && prop !== 'compact' && prop !== 'isActive'
})<{ status: AvailabilityStatus }>(({ theme, status }) => {
  const getChipColors = () => {
    switch (status) {
      case 'available':
        return { backgroundColor: theme.palette.success.main, color: 'white' };
      case 'unavailable':
        return { backgroundColor: theme.palette.error.main, color: 'white' };
      case 'maintenance':
        return { backgroundColor: theme.palette.warning.main, color: 'white' };
      case 'blocked':
        return { backgroundColor: theme.palette.grey[600], color: 'white' };
      case 'booked':
        return { backgroundColor: theme.palette.primary.main, color: 'white' };
      default:
        return { backgroundColor: theme.palette.grey[300], color: theme.palette.grey[800] };
    }
  };

  return {
    ...getChipColors(),
    fontWeight: 600,
    fontSize: '0.75rem'
  };
});

// ===== أنواع البيانات للمكون =====
interface AvailabilityGridProps {
  units: UnitManagementData[];
  loading?: boolean;
  error?: AvailabilityError | null;
  onUnitClick: (unit: UnitManagementData) => void;
  onUnitEdit?: (unit: UnitManagementData) => void;
  onViewDetails?: (unit: UnitManagementData) => void;
  onResolveConflict?: (conflict: BookingConflict) => void;
  selectedUnits?: string[];
  onSelectionChange?: (selectedIds: string[]) => void;
  showConflictsOnly?: boolean;
  compact?: boolean;
}

// ===== دوال مساعدة =====
const getStatusIcon = (status: AvailabilityStatus) => {
  switch (status) {
    case 'available':
      return <CheckCircleIcon />;
    case 'unavailable':
      return <CancelIcon />;
    case 'maintenance':
      return <BuildIcon />;
    case 'blocked':
      return <BlockIcon />;
    case 'booked':
      return <EventIcon />;
    default:
      return <CheckCircleIcon />;
  }
};

const getStatusText = (status: AvailabilityStatus): string => {
  switch (status) {
    case 'available':
      return 'متاح';
    case 'unavailable':
      return 'غير متاح';
    case 'maintenance':
      return 'صيانة';
    case 'blocked':
      return 'محجوب';
    case 'booked':
      return 'محجوز';
    default:
      return 'غير محدد';
  }
};

// Helper to detect active conflicts, guarding against null inputs
const hasActiveConflicts = (unit: UnitManagementData | null | undefined): boolean => {
  if (!unit) return false;
  return (unit.upcomingBookings?.some(booking => 
    booking.status === 'confirmed' || booking.status === 'paid'
  ) ?? false);
};

// ===== المكون الرئيسي =====
const AvailabilityGrid: React.FC<AvailabilityGridProps> = ({
  units,
  loading = false,
  error = null,
  onUnitClick,
  onUnitEdit,
  onViewDetails,
  onResolveConflict,
  selectedUnits = [],
  onSelectionChange,
  showConflictsOnly = false,
  compact = false
}) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [selectedUnit, setSelectedUnit] = useState<UnitManagementData | null>(null);
  const [conflictDialogOpen, setConflictDialogOpen] = useState(false);
  const [conflictToResolve, setConflictToResolve] = useState<BookingConflict | null>(null);

  // Exclude any null entries and apply conflict filter if requested
  const filteredUnits = units
    .filter((unit): unit is UnitManagementData => unit != null)
    .filter(unit => !showConflictsOnly || hasActiveConflicts(unit));

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

  const handleConflictResolve = useCallback((conflict: BookingConflict) => {
    setConflictToResolve(conflict);
    setConflictDialogOpen(true);
  }, []);

  const handleConflictDialogClose = useCallback(() => {
    setConflictDialogOpen(false);
    setConflictToResolve(null);
  }, []);

  const handleConfirmResolveConflict = useCallback(() => {
    if (conflictToResolve && onResolveConflict) {
      onResolveConflict(conflictToResolve);
    }
    handleConflictDialogClose();
  }, [conflictToResolve, onResolveConflict, handleConflictDialogClose]);

  // معالجة حالة التحميل
  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight={400}>
        <CircularProgress size={60} />
        <Typography variant="h6" sx={{ ml: 2 }}>
          جاري تحميل البيانات...
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
          خطأ في تحميل البيانات
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
          {showConflictsOnly 
            ? 'لا توجد وحدات تحتوي على تعارضات حالياً'
            : 'لم يتم العثور على أي وحدات تطابق المعايير المحددة'
          }
        </Typography>
      </Box>
    );
  }

  return (
    <>
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: compact ? 2 : 3 }}>
        {filteredUnits.map((unit) => {
          const hasConflicts = hasActiveConflicts(unit);
          const isSelected = selectedUnits.includes(unit.unit.unitId);
          
          return (
            <Box 
              sx={{
                width: { xs: '100%', sm: compact ? '33.33%' : '50%', md: compact ? '25%' : '33.33%', lg: compact ? '20%' : '33.33%' },
                p: 1
              }}
              key={unit.unit.unitId}
            >
              <Zoom in={true} timeout={300}>
                <StyledCard
                  status={unit.currentAvailability}
                  hasConflicts={hasConflicts}
                  onClick={(e) => handleUnitClick(unit, e)}
                  elevation={isSelected ? 8 : 2}
                  sx={{
                    height: compact ? 180 : 220,
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

                  {/* مؤشر التعارضات */}
                  {hasConflicts && (
                    <Badge
                      badgeContent={unit.upcomingBookings?.length || 0}
                      color="error"
                      sx={{
                        position: 'absolute',
                        top: 8,
                        left: 8,
                        zIndex: 10
                      }}
                    >
                      <Tooltip title="يوجد تعارضات في الحجوزات">
                        <WarningIcon color="error" />
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
                        النوع: {unit.unit.unitType}
                      </Typography>

                      {!compact && (
                        <Typography variant="body2" color="text.secondary" gutterBottom>
                          السعة: {unit.unit.capacity} أشخاص
                        </Typography>
                      )}

                      {/* السعر الحالي */}
                      <Typography 
                        variant="subtitle1" 
                        sx={{ 
                          fontWeight: 600,
                          color: 'primary.main',
                          mt: 1
                        }}
                      >
                        {unit.unit.basePrice} ر.ي / ليلة
                      </Typography>
                    </Box>

                    {/* حالة الإتاحة */}
                    <Box 
                      display="flex" 
                      alignItems="center" 
                      justifyContent="space-between"
                      mt={2}
                    >
                      <Box display="flex" alignItems="center" gap={1}>
                        {getStatusIcon(unit.currentAvailability)}
                        <StatusChip
                          status={unit.currentAvailability}
                          label={getStatusText(unit.currentAvailability)}
                          size="small"
                        />
                      </Box>

                      {/* الحجوزات القادمة */}
                      {unit.upcomingBookings && unit.upcomingBookings.length > 0 && (
                        <Tooltip title={`${unit.upcomingBookings.length} حجز قادم`}>
                          <Chip
                            icon={<EventIcon />}
                            label={unit.upcomingBookings.length}
                            size="small"
                            variant="outlined"
                          />
                        </Tooltip>
                      )}
                    </Box>
                  </CardContent>
                </StyledCard>
              </Zoom>
            </Box>
          );
        })}
      </Box>

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
            عرض التفاصيل
          </MenuItem>
        )}
        {onUnitEdit && (
          <MenuItem onClick={handleEdit}>
            <EditIcon sx={{ mr: 1 }} fontSize="small" />
            تعديل الإتاحة
          </MenuItem>
        )}
        {hasActiveConflicts(selectedUnit!) && (
          <MenuItem 
            onClick={() => selectedUnit && handleConflictResolve({} as BookingConflict)}
            sx={{ color: 'error.main' }}
          >
            <WarningIcon sx={{ mr: 1 }} fontSize="small" />
            حل التعارضات
          </MenuItem>
        )}
      </Menu>

      {/* مربع حوار حل التعارضات */}
      <Dialog
        open={conflictDialogOpen}
        onClose={handleConflictDialogClose}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>
          حل التعارضات
        </DialogTitle>
        <DialogContent>
          <Alert severity="warning" sx={{ mb: 2 }}>
            يوجد تعارض في الحجوزات لهذه الوحدة. يرجى اختيار الإجراء المناسب.
          </Alert>
          
          {conflictToResolve && (
            <Box>
              <Typography variant="subtitle1" gutterBottom>
                تفاصيل التعارض:
              </Typography>
              <Typography variant="body2">
                نوع التعارض: {conflictToResolve.conflictType === 'availability' ? 'إتاحة' : 'تسعير'}
              </Typography>
              <Typography variant="body2">
                مستوى التأثير: {conflictToResolve.impactLevel}
              </Typography>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleConflictDialogClose}>
            إلغاء
          </Button>
          <Button 
            onClick={handleConfirmResolveConflict} 
            variant="contained" 
            color="primary"
          >
            حل التعارض
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default AvailabilityGrid;
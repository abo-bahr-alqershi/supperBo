// frontend/src/components/admin/ContentManagementPanel.tsx

import React, { useState, useEffect, useMemo } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Card,
  CardContent,
  CardMedia,
  CardActions,
  CardActionArea,
  Button,
  IconButton,
  Chip,
  Stack,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Avatar,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  ListItemSecondaryAction,
  Checkbox,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Tabs,
  Tab,
  Badge,
  Tooltip,
  Alert,
  CircularProgress,
  Divider,
  ToggleButtonGroup,
  ToggleButton,
  Rating,
  Skeleton,
  Collapse,
  Switch,
  FormControlLabel,
  Slider,
  InputAdornment,
  Fade,
  Zoom,
  alpha,
  useTheme,
  Autocomplete,
  LinearProgress,
} from '@mui/material';
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  Edit as EditIcon,
  Search as SearchIcon,
  FilterList as FilterIcon,
  ViewModule as GridViewIcon,
  ViewList as ListViewIcon,
  ViewCarousel as CarouselViewIcon,
  Home as PropertyIcon,
  LocationCity as CityIcon,
  LocalOffer as OfferIcon,
  Campaign as AdIcon,
  Star as StarIcon,
  Schedule as ScheduleIcon,
  AttachMoney as PriceIcon,
  Image as ImageIcon,
  DragIndicator as DragIcon,
  ContentCopy as DuplicateIcon,
  Visibility as VisibilityIcon,
  VisibilityOff as VisibilityOffIcon,
  Info as InfoIcon,
  Warning as WarningIcon,
  CheckCircle as CheckCircleIcon,
  Cancel as CancelIcon,
  Refresh as RefreshIcon,
  Upload as UploadIcon,
  Download as DownloadIcon,
  BusinessCenter as BusinessIcon,
  Apartment as ApartmentIcon,
  Villa as VillaIcon,
  Landscape as LandIcon,
  Store as StoreIcon,
  Hotel as HotelIcon,
  Weekend as FurnitureIcon,
  Bathtub as BathroomIcon,
  KingBed as BedroomIcon,
  DirectionsCar as ParkingIcon,
  Pool as PoolIcon,
  FitnessCenter as GymIcon,
  Elevator as ElevatorIcon,
  Security as SecurityIcon,
  LocalParking as GarageIcon,
  Deck as BalconyIcon,
  Yard as GardenIcon,
  AcUnit as AcIcon,
  Kitchen as KitchenIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
  TrendingUp as TrendingIcon,
  NewReleases as NewIcon,
  Timer as TimerIcon,
  FlashOn as FlashIcon,
  Loyalty as LoyaltyIcon,
  CardGiftcard as GiftIcon,
  Percent as PercentIcon,
  TouchApp as TouchIcon,
} from '@mui/icons-material';
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragEndEvent,
} from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
  horizontalListSortingStrategy,
} from '@dnd-kit/sortable';
import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { useQuery } from '@tanstack/react-query';
import type { DynamicContent } from '../../types/homeSections.types';
import { AdminPropertiesService } from '../../services/admin-properties.service';
import { CitySettingsService } from '../../services/city-settings.service';
import HomeSectionsService from '../../services/homeSectionsService';

// Professional color palette
const COLORS = {
  primary: '#1a237e',
  secondary: '#004d40',
  success: '#1b5e20',
  warning: '#e65100',
  info: '#01579b',
  error: '#b71c1c',
  accent: '#6a1b9a',
  background: {
    light: '#f8f9fa',
    main: '#ffffff',
    dark: '#eceff1',
  },
  text: {
    primary: '#263238',
    secondary: '#546e7a',
    disabled: '#90a4ae',
  },
  border: '#e0e0e0',
};

// Property Types with icons
const PROPERTY_TYPES = [
  { value: 'apartment', label: 'شقة', icon: <ApartmentIcon />, color: COLORS.info },
  { value: 'villa', label: 'فيلا', icon: <VillaIcon />, color: COLORS.success },
  { value: 'land', label: 'أرض', icon: <LandIcon />, color: COLORS.warning },
  { value: 'office', label: 'مكتب', icon: <BusinessIcon />, color: COLORS.primary },
  { value: 'store', label: 'متجر', icon: <StoreIcon />, color: COLORS.secondary },
  { value: 'hotel', label: 'فندق', icon: <HotelIcon />, color: COLORS.accent },
];

// Amenities
const AMENITIES = [
  { value: 'pool', label: 'مسبح', icon: <PoolIcon /> },
  { value: 'gym', label: 'صالة رياضية', icon: <GymIcon /> },
  { value: 'parking', label: 'موقف سيارات', icon: <ParkingIcon /> },
  { value: 'elevator', label: 'مصعد', icon: <ElevatorIcon /> },
  { value: 'security', label: 'أمن', icon: <SecurityIcon /> },
  { value: 'garden', label: 'حديقة', icon: <GardenIcon /> },
  { value: 'balcony', label: 'شرفة', icon: <BalconyIcon /> },
  { value: 'ac', label: 'تكييف', icon: <AcIcon /> },
  { value: 'kitchen', label: 'مطبخ', icon: <KitchenIcon /> },
];

// Offer Types
const OFFER_TYPES = [
  { value: 'discount', label: 'خصم', icon: <PercentIcon />, color: COLORS.error },
  { value: 'flash', label: 'عرض برق', icon: <FlashIcon />, color: COLORS.warning },
  { value: 'seasonal', label: 'موسمي', icon: <TimerIcon />, color: COLORS.info },
  { value: 'new', label: 'جديد', icon: <NewIcon />, color: COLORS.success },
  { value: 'featured', label: 'مميز', icon: <StarIcon />, color: COLORS.accent },
  { value: 'gift', label: 'هدية', icon: <GiftIcon />, color: COLORS.secondary },
];

// Mock data service
const mockDataService = {
  getProperties: async (filters?: any) => {
    await new Promise(resolve => setTimeout(resolve, 500));
    
    const properties = [
      {
        id: '1',
        name: 'فيلا فاخرة بالرياض',
        nameEn: 'Luxury Villa in Riyadh',
        mainImageUrl: 'https://via.placeholder.com/400x300/4CAF50/ffffff?text=Villa',
        images: ['https://via.placeholder.com/400x300'],
        basePrice: 1500000,
        currency: 'SAR',
        location: 'الرياض - حي النخيل',
        locationEn: 'Riyadh - Al Nakheel',
        bedrooms: 5,
        bathrooms: 4,
        area: 450,
        rating: 4.8,
        reviewCount: 23,
        propertyType: 'villa',
        featured: true,
        hasOffer: true,
        discountPercentage: 15,
        amenities: ['pool', 'gym', 'parking', 'garden'],
        status: 'available',
        yearBuilt: 2022,
      },
      {
        id: '2',
        name: 'شقة عصرية بجدة',
        nameEn: 'Modern Apartment in Jeddah',
        mainImageUrl: 'https://via.placeholder.com/400x300/2196F3/ffffff?text=Apartment',
        images: ['https://via.placeholder.com/400x300'],
        basePrice: 750000,
        currency: 'SAR',
        location: 'جدة - الكورنيش',
        locationEn: 'Jeddah - Corniche',
        bedrooms: 3,
        bathrooms: 2,
        area: 180,
        rating: 4.5,
        reviewCount: 15,
        propertyType: 'apartment',
        featured: false,
        hasOffer: false,
        amenities: ['elevator', 'parking', 'ac'],
        status: 'available',
        yearBuilt: 2023,
      },
      {
        id: '3',
        name: 'أرض تجارية بالدمام',
        nameEn: 'Commercial Land in Dammam',
        mainImageUrl: 'https://via.placeholder.com/400x300/FF9800/ffffff?text=Land',
        images: ['https://via.placeholder.com/400x300'],
        basePrice: 2000000,
        currency: 'SAR',
        location: 'الدمام - حي الأعمال',
        locationEn: 'Dammam - Business District',
        area: 1000,
        rating: 4.2,
        reviewCount: 8,
        propertyType: 'land',
        featured: true,
        hasOffer: false,
        status: 'available',
      },
      {
        id: '4',
        name: 'مكتب إداري بالخبر',
        nameEn: 'Office Space in Khobar',
        mainImageUrl: 'https://via.placeholder.com/400x300/9C27B0/ffffff?text=Office',
        images: ['https://via.placeholder.com/400x300'],
        basePrice: 500000,
        currency: 'SAR',
        location: 'الخبر - شارع الملك فهد',
        locationEn: 'Khobar - King Fahd Street',
        area: 200,
        rating: 4.6,
        reviewCount: 12,
        propertyType: 'office',
        featured: false,
        hasOffer: true,
        discountPercentage: 10,
        amenities: ['elevator', 'parking', 'ac', 'security'],
        status: 'available',
        yearBuilt: 2021,
      },
    ];

    // Apply filters
    let filteredProperties = properties;
    
    if (filters?.type && filters.type !== 'all') {
      filteredProperties = filteredProperties.filter(p => p.propertyType === filters.type);
    }
    
    if (filters?.priceRange) {
      filteredProperties = filteredProperties.filter(p => 
        p.basePrice >= filters.priceRange[0] && p.basePrice <= filters.priceRange[1]
      );
    }
    
    if (filters?.hasOffer) {
      filteredProperties = filteredProperties.filter(p => p.hasOffer);
    }
    
    if (filters?.featured) {
      filteredProperties = filteredProperties.filter(p => p.featured);
    }

    return filteredProperties;
  },

  getCities: async () => {
    await new Promise(resolve => setTimeout(resolve, 300));
    
    return [
      {
        id: 'city1',
        name: 'الرياض',
        nameEn: 'Riyadh',
        country: 'السعودية',
        countryEn: 'Saudi Arabia',
        imageUrl: 'https://via.placeholder.com/400x300/1976D2/ffffff?text=Riyadh',
        propertyCount: 234,
        averagePrice: 850000,
        isPopular: true,
        isFeatured: true,
        description: 'عاصمة المملكة العربية السعودية',
        attractions: ['برج المملكة', 'المتحف الوطني', 'وادي حنيفة'],
      },
      {
        id: 'city2',
        name: 'جدة',
        nameEn: 'Jeddah',
        country: 'السعودية',
        countryEn: 'Saudi Arabia',
        imageUrl: 'https://via.placeholder.com/400x300/00BCD4/ffffff?text=Jeddah',
        propertyCount: 189,
        averagePrice: 750000,
        isPopular: true,
        isFeatured: false,
        description: 'عروس البحر الأحمر',
        attractions: ['نافورة الملك فهد', 'البلد التاريخية', 'كورنيش جدة'],
      },
      {
        id: 'city3',
        name: 'الدمام',
        nameEn: 'Dammam',
        country: 'السعودية',
        countryEn: 'Saudi Arabia',
        imageUrl: 'https://via.placeholder.com/400x300/FFC107/ffffff?text=Dammam',
        propertyCount: 145,
        averagePrice: 650000,
        isPopular: false,
        isFeatured: true,
        description: 'مركز صناعي وتجاري مهم',
        attractions: ['واجهة الدمام البحرية', 'جزيرة المرجان'],
      },
    ];
  },

  getOffers: async () => {
    await new Promise(resolve => setTimeout(resolve, 300));
    
    return [
      {
        id: 'offer1',
        title: 'عرض الصيف المذهل',
        titleEn: 'Amazing Summer Offer',
        description: 'خصم 20% على جميع الفلل',
        descriptionEn: '20% off on all villas',
        discountPercentage: 20,
        offerType: 'seasonal',
        startDate: '2024-06-01',
        endDate: '2024-08-31',
        imageUrl: 'https://via.placeholder.com/400x300/FF5722/ffffff?text=20%25+OFF',
        propertyIds: ['1', '3', '5'],
        isActive: true,
        isFlashDeal: false,
        terms: ['العرض ساري لفترة محدودة', 'لا يشمل العقارات المحجوزة'],
        code: 'SUMMER20',
      },
      {
        id: 'offer2',
        title: 'صفقة البرق',
        titleEn: 'Flash Deal',
        description: 'خصم 30% لمدة 48 ساعة فقط',
        descriptionEn: '30% off for 48 hours only',
        discountPercentage: 30,
        offerType: 'flash',
        startDate: '2024-01-15',
        endDate: '2024-01-17',
        imageUrl: 'https://via.placeholder.com/400x300/F44336/ffffff?text=FLASH+DEAL',
        propertyIds: ['2', '4'],
        isActive: true,
        isFlashDeal: true,
        terms: ['العرض محدود المدة', 'أول 10 عملاء فقط'],
        code: 'FLASH30',
      },
    ];
  },

  getAds: async () => {
    await new Promise(resolve => setTimeout(resolve, 300));
    
    return [
      {
        id: 'ad1',
        title: 'اكتشف منزل أحلامك',
        titleEn: 'Discover Your Dream Home',
        description: 'مجموعة حصرية من العقارات الفاخرة',
        ctaText: 'اكتشف المزيد',
        ctaUrl: '/properties/luxury',
        imageUrl: 'https://via.placeholder.com/800x400/3F51B5/ffffff?text=Dream+Home',
        backgroundColor: '#2196F3',
        textColor: '#FFFFFF',
        startDate: '2024-01-01',
        endDate: '2024-12-31',
        priority: 1,
        isActive: true,
        targetAudience: ['all'],
        position: 'hero',
        impressions: 15420,
        clicks: 342,
      },
      {
        id: 'ad2',
        title: 'استثمر في المستقبل',
        titleEn: 'Invest in the Future',
        description: 'فرص استثمارية لا تُفوت',
        ctaText: 'ابدأ الآن',
        ctaUrl: '/investment',
        imageUrl: 'https://via.placeholder.com/800x400/4CAF50/ffffff?text=Investment',
        backgroundColor: '#4CAF50',
        textColor: '#FFFFFF',
        startDate: '2024-01-01',
        endDate: '2024-12-31',
        priority: 2,
        isActive: true,
        targetAudience: ['premium_users'],
        position: 'featured',
        impressions: 8234,
        clicks: 156,
      },
    ];
  },
};

// Sortable Item Component
interface SortableItemProps {
  id: string;
  children: React.ReactNode;
}

const SortableItem: React.FC<SortableItemProps> = ({ id, children }) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  return (
    <div ref={setNodeRef} style={style} {...attributes} {...listeners}>
      {children}
    </div>
  );
};

// Main Content Management Panel Component
interface ContentManagementPanelProps {
  sectionType: string;
  currentContent: DynamicContent[];
  onContentChange: (content: DynamicContent[]) => void;
  maxItems?: number;
}

const ContentManagementPanel: React.FC<ContentManagementPanelProps> = ({
  sectionType,
  currentContent,
  onContentChange,
  maxItems = 10,
}) => {
  const theme = useTheme();
  const [selectedTab, setSelectedTab] = useState(0);
  const [viewMode, setViewMode] = useState<'grid' | 'list' | 'carousel'>('grid');
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedItems, setSelectedItems] = useState<string[]>([]);
  const [contentItems, setContentItems] = useState<DynamicContent[]>(currentContent);
  const [showFilters, setShowFilters] = useState(false);
  const [filters, setFilters] = useState({
    type: 'all',
    priceRange: [0, 5000000],
    rating: 0,
    hasOffer: false,
    featured: false,
  });
  const [detailsDialogOpen, setDetailsDialogOpen] = useState(false);
  const [selectedDetailItem, setSelectedDetailItem] = useState<any>(null);

  // Update content items when currentContent changes
  useEffect(() => {
    setContentItems(currentContent);
  }, [currentContent]);

  // DnD Kit sensors
  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  // Determine content type based on section type
  const getContentType = () => {
    if (sectionType.includes('PROPERTY') || sectionType.includes('OFFER')) return 'property';
    if (sectionType.includes('CITY') || sectionType.includes('DESTINATION')) return 'city';
    if (sectionType.includes('AD')) return 'advertisement';
    return 'mixed';
  };

  const contentType = getContentType();

  // Fetch data based on content type
  const { data: properties, isLoading: propertiesLoading } = useQuery({
    queryKey: ['properties', filters],
    queryFn: () => AdminPropertiesService.getAll({ pageNumber: 1, pageSize: 1000 }).then(res => res.items),
    enabled: contentType === 'property',
  });

  const { data: cities, isLoading: citiesLoading } = useQuery({
    queryKey: ['cities'],
    queryFn: () => CitySettingsService.getCities(),
    enabled: contentType === 'city',
  });

  const offers = OFFER_TYPES;
  const offersLoading = false;

  const { data: ads, isLoading: adsLoading } = useQuery({
    queryKey: ['ads'],
    queryFn: () => HomeSectionsService.getSponsoredAds({ onlyActive: true, includePropertyDetails: true }),
    enabled: contentType === 'advertisement',
  });

  const isLoading = propertiesLoading || citiesLoading || offersLoading || adsLoading;

  // Get appropriate tabs based on content type
  const getTabs = () => {
    // For single property offer section, only show property tab
    if (sectionType === 'SINGLE_PROPERTY_OFFER') {
      return [
        { label: 'العقارات', icon: <PropertyIcon />, count: properties?.length || 0 },
      ];
    }
    if (contentType === 'property') {
      return [
        { label: 'العقارات', icon: <PropertyIcon />, count: properties?.length || 0 },
        { label: 'العروض', icon: <OfferIcon />, count: offers?.length || 0 },
      ];
    }
    if (contentType === 'city') {
      return [{ label: 'المدن والوجهات', icon: <CityIcon />, count: cities?.length || 0 }];
    }
    if (contentType === 'advertisement') {
      return [{ label: 'الإعلانات', icon: <AdIcon />, count: ads?.length || 0 }];
    }
    return [];
  };

  // Handle item selection
  const handleItemSelect = (item: any, type: string) => {
    // Normalize content id and type uppercase for downstream components
    const idKey = (item.id ?? item.value ?? item.name);
    const newContent: DynamicContent = {
      id: `content-${Date.now()}-${idKey}`,
      sectionId: '',
      contentType: type.toUpperCase(),
      contentData: item,
      metadata: {
        addedAt: new Date().toISOString(),
        source: 'manual',
      },
      displayOrder: contentItems.length,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };

    if (contentItems.length < maxItems) {
      const updatedContent = [...contentItems, newContent];
      setContentItems(updatedContent);
      onContentChange(updatedContent);
    }
  };

  // Handle content removal
  const handleRemoveContent = (contentId: string) => {
    const updatedContent = contentItems.filter(item => item.id !== contentId);
    const reorderedContent = updatedContent.map((item, index) => ({
      ...item,
      displayOrder: index,
    }));
    setContentItems(reorderedContent);
    onContentChange(reorderedContent);
  };

  // Handle content reorder
  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;

    if (active.id !== over?.id) {
      const oldIndex = contentItems.findIndex(item => item.id === active.id);
      const newIndex = contentItems.findIndex(item => item.id === over?.id);

      const reorderedItems = arrayMove(contentItems, oldIndex, newIndex);
      const updatedContent = reorderedItems.map((item, index) => ({
        ...item,
        displayOrder: index,
      }));

      setContentItems(updatedContent);
      onContentChange(updatedContent);
    }
  };

  // Handle bulk actions
  const handleBulkDelete = () => {
    const updatedContent = contentItems.filter(
      item => !selectedItems.includes(item.id)
    );
    const reorderedContent = updatedContent.map((item, index) => ({
      ...item,
      displayOrder: index,
    }));
    setContentItems(reorderedContent);
    onContentChange(reorderedContent);
    setSelectedItems([]);
  };

  const handleSelectAll = () => {
    if (selectedItems.length === contentItems.length) {
      setSelectedItems([]);
    } else {
      setSelectedItems(contentItems.map(item => item.id));
    }
  };

  const handleShowDetails = (item: any) => {
    setSelectedDetailItem(item);
    setDetailsDialogOpen(true);
  };

  // Statistics
  const stats = useMemo(() => ({
    total: contentItems.length,
    maxAllowed: maxItems,
    remaining: maxItems - contentItems.length,
    percentage: Math.round((contentItems.length / maxItems) * 100),
  }), [contentItems, maxItems]);

  return (
    <Box>
      {/* Header Section */}
      <Paper
        elevation={0}
        sx={{
          p: 3,
          mb: 3,
          background: `linear-gradient(135deg, ${alpha(COLORS.primary, 0.05)} 0%, ${alpha(COLORS.primary, 0.02)} 100%)`,
          border: `1px solid ${COLORS.border}`,
          borderRadius: 2,
        }}
      >
        <Stack direction="row" alignItems="center" justifyContent="space-between" flexWrap="wrap" gap={2}>
          <Box>
            <Typography variant="h5" sx={{ fontWeight: 600, color: COLORS.text.primary, mb: 0.5 }}>
              إدارة محتوى القسم
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {sectionType ? `نوع القسم: ${sectionType.replace(/_/g, ' ')}` : 'حدد نوع القسم أولاً'}
            </Typography>
          </Box>

          <Stack direction="row" spacing={2} alignItems="center">
            {/* Progress Indicator */}
            <Box sx={{ position: 'relative', display: 'inline-flex' }}>
              <CircularProgress
                variant="determinate"
                value={stats.percentage}
                size={60}
                thickness={4}
                sx={{
                  color: stats.percentage >= 100 ? COLORS.error : 
                         stats.percentage >= 75 ? COLORS.warning : COLORS.success,
                }}
              />
              <Box
                sx={{
                  top: 0,
                  left: 0,
                  bottom: 0,
                  right: 0,
                  position: 'absolute',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                }}
              >
                <Stack alignItems="center">
                  <Typography variant="caption" sx={{ fontWeight: 600 }}>
                    {stats.total}/{stats.maxAllowed}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    عنصر
                  </Typography>
                </Stack>
              </Box>
            </Box>

            {/* View Mode Toggle */}
            <ToggleButtonGroup
              value={viewMode}
              exclusive
              onChange={(_, value) => value && setViewMode(value)}
              size="small"
            >
              <ToggleButton value="grid">
                <Tooltip title="عرض شبكي">
                  <GridViewIcon />
                </Tooltip>
              </ToggleButton>
              <ToggleButton value="list">
                <Tooltip title="عرض قائمة">
                  <ListViewIcon />
                </Tooltip>
              </ToggleButton>
              <ToggleButton value="carousel">
                <Tooltip title="عرض دوار">
                  <CarouselViewIcon />
                </Tooltip>
              </ToggleButton>
            </ToggleButtonGroup>
          </Stack>
        </Stack>
      </Paper>

      {/* Alert for section type */}
      {!sectionType && (
        <Alert severity="warning" sx={{ mb: 3 }}>
          يرجى اختيار نوع القسم أولاً من تبويب "المعلومات الأساسية" لتتمكن من إضافة المحتوى
        </Alert>
      )}

      {sectionType && (
        <>
          {/* Current Content Section */}
          {contentItems.length > 0 && (
            <Paper elevation={0} sx={{ p: 3, mb: 3, border: `1px solid ${COLORS.border}`, borderRadius: 2 }}>
              <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ mb: 2 }}>
                <Stack direction="row" alignItems="center" spacing={2}>
                  <Typography variant="h6" sx={{ fontWeight: 600 }}>
                    المحتوى الحالي
                  </Typography>
                  <Chip
                    label={`${contentItems.length} عنصر`}
                    color={contentItems.length >= maxItems ? 'error' : 'primary'}
                    size="small"
                  />
                </Stack>

                {selectedItems.length > 0 && (
                  <Stack direction="row" spacing={1}>
                    <Button
                      size="small"
                      startIcon={<DeleteIcon />}
                      onClick={handleBulkDelete}
                      color="error"
                      variant="outlined"
                    >
                      حذف المحدد ({selectedItems.length})
                    </Button>
                    <Button
                      size="small"
                      onClick={() => setSelectedItems([])}
                      variant="text"
                    >
                      إلغاء التحديد
                    </Button>
                  </Stack>
                )}

                {contentItems.length > 1 && (
                  <FormControlLabel
                    control={
                      <Checkbox
                        checked={selectedItems.length === contentItems.length}
                        indeterminate={selectedItems.length > 0 && selectedItems.length < contentItems.length}
                        onChange={handleSelectAll}
                      />
                    }
                    label="تحديد الكل"
                  />
                )}
              </Stack>

              <DndContext
                sensors={sensors}
                collisionDetection={closestCenter}
                onDragEnd={handleDragEnd}
              >
                <SortableContext
                  items={contentItems.map(item => item.id)}
                  strategy={viewMode === 'list' ? verticalListSortingStrategy : horizontalListSortingStrategy}
                >
                  {viewMode === 'list' ? (
                    <List>
                      {contentItems.map((item, index) => (
                        <SortableItem key={item.id} id={item.id}>
                          <ContentListItem
                            content={item}
                            index={index}
                            isSelected={selectedItems.includes(item.id)}
                            onSelect={(selected) => {
                              if (selected) {
                                setSelectedItems([...selectedItems, item.id]);
                              } else {
                                setSelectedItems(selectedItems.filter(id => id !== item.id));
                              }
                            }}
                            onRemove={() => handleRemoveContent(item.id)}
                            onShowDetails={() => handleShowDetails(item.contentData)}
                          />
                        </SortableItem>
                      ))}
                    </List>
                  ) : (
                    <Grid container spacing={2}>
                      {contentItems.map((item, index) => (
                        <Grid item xs={12} sm={6} md={viewMode === 'carousel' ? 3 : 4} key={item.id}>
                          <SortableItem id={item.id}>
                            <ContentCard
                              content={item}
                              index={index}
                              isSelected={selectedItems.includes(item.id)}
                              onSelect={(selected) => {
                                if (selected) {
                                  setSelectedItems([...selectedItems, item.id]);
                                } else {
                                  setSelectedItems(selectedItems.filter(id => id !== item.id));
                                }
                              }}
                              onRemove={() => handleRemoveContent(item.id)}
                              onShowDetails={() => handleShowDetails(item.contentData)}
                              viewMode={viewMode}
                            />
                          </SortableItem>
                        </Grid>
                      ))}
                    </Grid>
                  )}
                </SortableContext>
              </DndContext>
            </Paper>
          )}

          {/* Add Content Section */}
          <Paper elevation={0} sx={{ p: 3, border: `1px solid ${COLORS.border}`, borderRadius: 2 }}>
            <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
              إضافة محتوى جديد
            </Typography>

            {/* Search and Filters */}
            <Stack direction="row" spacing={2} sx={{ mb: 3 }}>
              <TextField
                fullWidth
                placeholder="البحث..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                InputProps={{
                  startAdornment: (
                    <InputAdornment position="start">
                      <SearchIcon sx={{ color: COLORS.text.disabled }} />
                    </InputAdornment>
                  ),
                }}
                size="small"
              />
              
              <Button
                variant="outlined"
                startIcon={showFilters ? <ExpandLessIcon /> : <ExpandMoreIcon />}
                onClick={() => setShowFilters(!showFilters)}
                sx={{ minWidth: 120 }}
              >
                {showFilters ? 'إخفاء' : 'الفلاتر'}
              </Button>
            </Stack>

            {/* Filters Panel */}
            <Collapse in={showFilters}>
              <Paper sx={{ p: 2, mb: 3, bgcolor: COLORS.background.light }}>
                <Grid container spacing={2}>
                  {contentType === 'property' && (
                    <>
                      <Grid item xs={12} sm={6} md={3}>
                        <FormControl fullWidth size="small">
                          <InputLabel>نوع العقار</InputLabel>
                          <Select
                            value={filters.type}
                            onChange={(e) => setFilters({ ...filters, type: e.target.value })}
                            label="نوع العقار"
                          >
                            <MenuItem value="all">الكل</MenuItem>
                            {PROPERTY_TYPES.map((type) => (
                              <MenuItem key={type.value} value={type.value}>
                                <Stack direction="row" alignItems="center" spacing={1}>
                                  {type.icon}
                                  <span>{type.label}</span>
                                </Stack>
                              </MenuItem>
                            ))}
                          </Select>
                        </FormControl>
                      </Grid>
                      
                      <Grid item xs={12} sm={6} md={3}>
                        <Typography variant="caption" gutterBottom>
                          نطاق السعر: {filters.priceRange[0].toLocaleString()} - {filters.priceRange[1].toLocaleString()} ريال
                        </Typography>
                        <Slider
                          value={filters.priceRange}
                          onChange={(_, value) => setFilters({ ...filters, priceRange: value as number[] })}
                          valueLabelDisplay="auto"
                          min={0}
                          max={5000000}
                          step={100000}
                          valueLabelFormat={(value) => `${(value / 1000).toFixed(0)}k`}
                        />
                      </Grid>
                      
                      <Grid item xs={12} sm={6} md={3}>
                        <FormControlLabel
                          control={
                            <Switch
                              checked={filters.hasOffer}
                              onChange={(e) => setFilters({ ...filters, hasOffer: e.target.checked })}
                            />
                          }
                          label="عروض فقط"
                        />
                      </Grid>
                      
                      <Grid item xs={12} sm={6} md={3}>
                        <FormControlLabel
                          control={
                            <Switch
                              checked={filters.featured}
                              onChange={(e) => setFilters({ ...filters, featured: e.target.checked })}
                            />
                          }
                          label="مميز فقط"
                        />
                      </Grid>
                    </>
                  )}
                </Grid>
              </Paper>
            </Collapse>

            {/* Content Tabs */}
            {getTabs().length > 0 && (
              <Tabs 
                value={selectedTab} 
                onChange={(_, value) => setSelectedTab(value)} 
                sx={{ mb: 3, borderBottom: 1, borderColor: 'divider' }}
              >
                {getTabs().map((tab, index) => (
                  <Tab 
                    key={index}
                    label={
                      <Stack direction="row" alignItems="center" spacing={1}>
                        {tab.icon}
                        <span>{tab.label}</span>
                        <Chip size="small" label={tab.count} />
                      </Stack>
                    }
                  />
                ))}
              </Tabs>
            )}

            {/* Content Grid */}
            {isLoading ? (
              <Grid container spacing={2}>
                {[1, 2, 3, 4, 5, 6].map((i) => (
                  <Grid item xs={12} sm={6} md={4} key={i}>
                    <Skeleton variant="rectangular" height={250} sx={{ borderRadius: 2 }} />
                  </Grid>
                ))}
              </Grid>
            ) : (
              <Grid container spacing={2}>
                {/* Properties */}
                {contentType === 'property' && selectedTab === 0 && properties?.map((property) => (
                  <Grid item xs={12} sm={6} md={4} key={property.id}>
                    <PropertyCard
                      property={property}
                      onSelect={() => handleItemSelect(property, 'property')}
                      disabled={contentItems.length >= maxItems}
                      isSelected={contentItems.some(c => c.contentData.id === property.id)}
                    />
                  </Grid>
                ))}

                {/* Offers */}
                {contentType === 'property' && selectedTab === 1 && offers?.map((offer) => (
                  <Grid item xs={12} sm={6} md={4} key={offer.value}>
                    <OfferCard
                      offer={offer}
                      onSelect={() => handleItemSelect(offer, 'offer')}
                      disabled={contentItems.length >= maxItems}
                      isSelected={contentItems.some(c => c.contentData.value === offer.value)}
                    />
                  </Grid>
                ))}

                {/* Cities */}
                {contentType === 'city' && cities?.map((city) => (
                  <Grid item xs={12} sm={6} md={4} key={city.name}>
                    <CityCard
                      city={city}
                      onSelect={() => handleItemSelect(city, 'city')}
                      disabled={contentItems.length >= maxItems}
                      isSelected={contentItems.some(c => c.contentData.name === city.name)}
                    />
                  </Grid>
                ))}

                {/* Ads */}
                {contentType === 'advertisement' && ads?.map((ad) => (
                  <Grid item xs={12} sm={6} md={4} key={ad.id}>
                    <AdCard
                      ad={ad}
                      onSelect={() => handleItemSelect(ad, 'advertisement')}
                      disabled={contentItems.length >= maxItems}
                      isSelected={contentItems.some(c => c.contentData.id === ad.id)}
                    />
                  </Grid>
                ))}
              </Grid>
            )}

            {/* Empty State */}
            {!isLoading && (
              (contentType === 'property' && properties?.length === 0) ||
              (contentType === 'city' && cities?.length === 0) ||
              (contentType === 'advertisement' && ads?.length === 0)
            ) && (
              <Box sx={{ textAlign: 'center', py: 6 }}>
                <InfoIcon sx={{ fontSize: 60, color: COLORS.text.disabled, mb: 2 }} />
                <Typography variant="h6" color="text.secondary">
                  لا توجد عناصر متاحة
                </Typography>
                <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                  جرب تغيير معايير البحث أو الفلاتر
                </Typography>
              </Box>
            )}
          </Paper>

          {/* Max Items Warning */}
          {contentItems.length >= maxItems && (
            <Alert severity="warning" sx={{ mt: 3 }} icon={<WarningIcon />}>
              تم الوصول للحد الأقصى من العناصر ({maxItems}). قم بحذف بعض العناصر لإضافة المزيد.
            </Alert>
          )}
        </>
      )}

      {/* Details Dialog */}
      <Dialog
        open={detailsDialogOpen}
        onClose={() => setDetailsDialogOpen(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>
          تفاصيل العنصر
          <IconButton
            onClick={() => setDetailsDialogOpen(false)}
            sx={{ position: 'absolute', right: 8, top: 8 }}
          >
            <CancelIcon />
          </IconButton>
        </DialogTitle>
        <DialogContent dividers>
          {selectedDetailItem && (
            <Box sx={{ p: 2 }}>
              <pre style={{ whiteSpace: 'pre-wrap', wordBreak: 'break-word' }}>
                {JSON.stringify(selectedDetailItem, null, 2)}
              </pre>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDetailsDialogOpen(false)}>إغلاق</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

// Content Card Component
const ContentCard: React.FC<{
  content: DynamicContent;
  index: number;
  isSelected: boolean;
  onSelect: (selected: boolean) => void;
  onRemove: () => void;
  onShowDetails: () => void;
  viewMode: string;
}> = ({ content, index, isSelected, onSelect, onRemove, onShowDetails, viewMode }) => {
  const data = content.contentData;
  const getPropertyTypeInfo = (type: string) => {
    return PROPERTY_TYPES.find(t => t.value === type) || { label: type, icon: <PropertyIcon />, color: COLORS.primary };
  };
  
  return (
    <Card
      elevation={0}
      sx={{
        position: 'relative',
        border: isSelected ? `2px solid ${COLORS.primary}` : `1px solid ${COLORS.border}`,
        transition: 'all 0.3s ease',
        '&:hover': {
          transform: 'translateY(-4px)',
          boxShadow: 4,
        },
      }}
    >
      {/* Drag Handle */}
      <Box
        sx={{
          position: 'absolute',
          top: 8,
          left: 8,
          zIndex: 1,
          bgcolor: alpha('#fff', 0.9),
          borderRadius: 1,
          p: 0.5,
          cursor: 'grab',
          '&:active': { cursor: 'grabbing' },
        }}
      >
        <DragIcon sx={{ color: COLORS.text.disabled }} />
      </Box>

      {/* Selection Checkbox */}
      <Checkbox
        checked={isSelected}
        onChange={(e) => onSelect(e.target.checked)}
        sx={{
          position: 'absolute',
          top: 8,
          right: 8,
          zIndex: 1,
          bgcolor: alpha('#fff', 0.9),
          borderRadius: 1,
        }}
      />

      {/* Order Badge */}
      <Badge
        badgeContent={index + 1}
        color="primary"
        sx={{
          position: 'absolute',
          top: 16,
          right: 48,
          zIndex: 1,
        }}
      />

      <CardMedia
        component="img"
        height={viewMode === 'carousel' ? 160 : 200}
        image={data.mainImageUrl || data.imageUrl || 'https://via.placeholder.com/400x300'}
        alt={data.name || data.title}
        sx={{ objectFit: 'cover' }}
      />
      
      <CardContent sx={{ pb: 1 }}>
        <Typography variant="subtitle2" noWrap sx={{ fontWeight: 600 }}>
          {data.name || data.title}
        </Typography>
        
        {data.location && (
          <Typography variant="caption" color="text.secondary" noWrap>
            {data.location}
          </Typography>
        )}
        
        <Stack direction="row" spacing={1} sx={{ mt: 1 }}>
          <Chip
            label={content.contentType}
            size="small"
            color={getContentColor(content.contentType)}
          />
          {data.featured && (
            <Chip label="مميز" size="small" color="warning" icon={<StarIcon />} />
          )}
          {data.hasOffer && (
            <Chip label={`خصم ${data.discountPercentage}%`} size="small" color="error" />
          )}
        </Stack>

        {data.basePrice && (
          <Typography variant="subtitle2" sx={{ mt: 1, color: COLORS.primary, fontWeight: 600 }}>
            {data.basePrice.toLocaleString()} {data.currency || 'ريال'}
          </Typography>
        )}
      </CardContent>
      
      <CardActions sx={{ pt: 0 }}>
        <IconButton size="small" onClick={onShowDetails} color="info">
          <InfoIcon />
        </IconButton>
        <IconButton size="small" onClick={onRemove} color="error">
          <DeleteIcon />
        </IconButton>
      </CardActions>
    </Card>
  );
};

// Content List Item Component
const ContentListItem: React.FC<{
  content: DynamicContent;
  index: number;
  isSelected: boolean;
  onSelect: (selected: boolean) => void;
  onRemove: () => void;
  onShowDetails: () => void;
}> = ({ content, index, isSelected, onSelect, onRemove, onShowDetails }) => {
  const data = content.contentData;
  
  return (
    <ListItem
      sx={{
        mb: 1,
        bgcolor: isSelected ? alpha(COLORS.primary, 0.05) : 'background.paper',
        border: isSelected ? `2px solid ${COLORS.primary}` : `1px solid ${COLORS.border}`,
        borderRadius: 2,
        transition: 'all 0.3s ease',
        '&:hover': {
          bgcolor: alpha(COLORS.primary, 0.02),
        },
      }}
    >
      <Box sx={{ mr: 2, cursor: 'grab', '&:active': { cursor: 'grabbing' } }}>
        <DragIcon sx={{ color: COLORS.text.disabled }} />
      </Box>
      
      <Checkbox
        checked={isSelected}
        onChange={(e) => onSelect(e.target.checked)}
      />
      
      <ListItemAvatar>
        <Badge badgeContent={index + 1} color="primary">
          <Avatar
            src={data.mainImageUrl || data.imageUrl}
            variant="rounded"
            sx={{ width: 56, height: 56 }}
          >
            {getContentIcon(content.contentType)}
          </Avatar>
        </Badge>
      </ListItemAvatar>
      
      <ListItemText
        primary={
          <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
            {data.name || data.title}
          </Typography>
        }
        secondary={
          <Stack direction="row" spacing={1} alignItems="center" sx={{ mt: 0.5 }}>
            <Chip
              label={content.contentType}
              size="small"
              color={getContentColor(content.contentType)}
            />
            {data.location && (
              <Typography variant="caption" color="text.secondary">
                {data.location}
              </Typography>
            )}
            {data.basePrice && (
              <Typography variant="caption" sx={{ color: COLORS.primary, fontWeight: 600 }}>
                {data.basePrice.toLocaleString()} {data.currency || 'ريال'}
              </Typography>
            )}
          </Stack>
        }
      />
      
      <ListItemSecondaryAction>
        <Stack direction="row" spacing={1}>
          <IconButton size="small" onClick={onShowDetails} color="info">
            <InfoIcon />
          </IconButton>
          <IconButton size="small" onClick={onRemove} color="error">
            <DeleteIcon />
          </IconButton>
        </Stack>
      </ListItemSecondaryAction>
    </ListItem>
  );
};

// Property Card Component
const PropertyCard: React.FC<{
  property: any;
  onSelect: () => void;
  disabled: boolean;
  isSelected: boolean;
}> = ({ property, onSelect, disabled, isSelected }) => {
  const propertyType = PROPERTY_TYPES.find(t => t.value === property.propertyType);
  
  return (
    <Card
      elevation={0}
      sx={{
        position: 'relative',
        opacity: disabled && !isSelected ? 0.6 : 1,
        border: isSelected ? `2px solid ${COLORS.success}` : `1px solid ${COLORS.border}`,
        transition: 'all 0.3s ease',
        '&:hover': {
          transform: !disabled || isSelected ? 'translateY(-4px)' : 'none',
          boxShadow: !disabled || isSelected ? 4 : 0,
        },
      }}
    >
      {property.hasOffer && (
        <Chip
          label={`خصم ${property.discountPercentage}%`}
          color="error"
          size="small"
          sx={{
            position: 'absolute',
            top: 8,
            right: 8,
            zIndex: 1,
          }}
        />
      )}

      {property.featured && (
        <Chip
          label="مميز"
          color="warning"
          size="small"
          icon={<StarIcon />}
          sx={{
            position: 'absolute',
            top: 8,
            left: 8,
            zIndex: 1,
          }}
        />
      )}

      <CardMedia
        component="img"
        height="180"
        image={property.mainImageUrl}
        alt={property.name}
      />
      
      <CardContent>
        <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ mb: 1 }}>
          <Typography variant="subtitle1" noWrap sx={{ fontWeight: 600, flex: 1 }}>
            {property.name}
          </Typography>
          {propertyType && (
            <Tooltip title={propertyType.label}>
              <Avatar sx={{ bgcolor: alpha(propertyType.color, 0.1), color: propertyType.color, width: 32, height: 32 }}>
                {propertyType.icon}
              </Avatar>
            </Tooltip>
          )}
        </Stack>
        
        <Typography variant="body2" color="text.secondary" noWrap sx={{ mb: 1 }}>
          {property.location}
        </Typography>
        
        <Stack direction="row" spacing={1} sx={{ mb: 1 }}>
          {property.bedrooms && (
            <Chip 
              label={`${property.bedrooms} غرف`} 
              size="small" 
              icon={<BedroomIcon />}
              sx={{ bgcolor: alpha(COLORS.info, 0.1) }}
            />
          )}
          {property.bathrooms && (
            <Chip 
              label={`${property.bathrooms} حمام`} 
              size="small" 
              icon={<BathroomIcon />}
              sx={{ bgcolor: alpha(COLORS.info, 0.1) }}
            />
          )}
          {property.area && (
            <Chip 
              label={`${property.area} م²`} 
              size="small"
              sx={{ bgcolor: alpha(COLORS.info, 0.1) }}
            />
          )}
        </Stack>

        {property.amenities && property.amenities.length > 0 && (
          <Stack direction="row" spacing={0.5} sx={{ mb: 1 }}>
            {property.amenities.slice(0, 3).map((amenity: string) => {
              const amenityInfo = AMENITIES.find(a => a.value === amenity);
              return amenityInfo ? (
                <Tooltip key={amenity} title={amenityInfo.label}>
                  <Avatar sx={{ bgcolor: alpha(COLORS.secondary, 0.1), color: COLORS.secondary, width: 24, height: 24 }}>
                    {React.cloneElement(amenityInfo.icon, { sx: { fontSize: 16 } })}
                  </Avatar>
                </Tooltip>
              ) : null;
            })}
            {property.amenities.length > 3 && (
              <Chip label={`+${property.amenities.length - 3}`} size="small" />
            )}
          </Stack>
        )}
        
        <Stack direction="row" alignItems="center" justifyContent="space-between">
          <Typography variant="h6" sx={{ color: COLORS.primary, fontWeight: 600 }}>
            {property.basePrice.toLocaleString()} {property.currency}
          </Typography>
          
          {property.rating && (
            <Stack direction="row" alignItems="center">
              <Rating value={property.rating} readOnly size="small" precision={0.5} />
              <Typography variant="caption" sx={{ ml: 0.5 }}>
                ({property.reviewCount})
              </Typography>
            </Stack>
          )}
        </Stack>
      </CardContent>
      
      <CardActions>
        <Button
          fullWidth
          variant={isSelected ? 'contained' : 'outlined'}
          onClick={onSelect}
          disabled={disabled && !isSelected}
          color={isSelected ? 'success' : 'primary'}
          startIcon={isSelected ? <CheckCircleIcon /> : <AddIcon />}
        >
          {isSelected ? 'محدد' : 'اختيار'}
        </Button>
      </CardActions>
    </Card>
  );
};

// City Card Component
const CityCard: React.FC<{
  city: any;
  onSelect: () => void;
  disabled: boolean;
  isSelected: boolean;
}> = ({ city, onSelect, disabled, isSelected }) => {
  return (
    <Card
      elevation={0}
      sx={{
        position: 'relative',
        opacity: disabled && !isSelected ? 0.6 : 1,
        border: isSelected ? `2px solid ${COLORS.success}` : `1px solid ${COLORS.border}`,
        transition: 'all 0.3s ease',
        '&:hover': {
          transform: !disabled || isSelected ? 'translateY(-4px)' : 'none',
          boxShadow: !disabled || isSelected ? 4 : 0,
        },
      }}
    >
      {city.isFeatured && (
        <Chip
          label="مميزة"
          color="warning"
          size="small"
          icon={<StarIcon />}
          sx={{
            position: 'absolute',
            top: 8,
            right: 8,
            zIndex: 1,
          }}
        />
      )}

      {city.isPopular && (
        <Chip
          label="رائجة"
          color="info"
          size="small"
          icon={<TrendingIcon />}
          sx={{
            position: 'absolute',
            top: 8,
            left: 8,
            zIndex: 1,
          }}
        />
      )}

      <CardMedia
        component="img"
        height="180"
        image={city.imageUrl}
        alt={city.name}
      />
      
      <CardContent>
        <Typography variant="subtitle1" noWrap sx={{ fontWeight: 600 }}>
          {city.name}
        </Typography>
        
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
          {city.country}
        </Typography>

        {city.description && (
          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
            {city.description}
          </Typography>
        )}
        
        <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
          <Box>
            <Typography variant="h6" sx={{ color: COLORS.primary, fontWeight: 600 }}>
              {city.propertyCount}
            </Typography>
            <Typography variant="caption" color="text.secondary">
              عقار
            </Typography>
          </Box>
          <Divider orientation="vertical" flexItem />
          <Box>
            <Typography variant="h6" sx={{ color: COLORS.secondary, fontWeight: 600 }}>
              {(city.averagePrice / 1000).toFixed(0)}k
            </Typography>
            <Typography variant="caption" color="text.secondary">
              متوسط السعر
            </Typography>
          </Box>
        </Stack>

        {city.attractions && city.attractions.length > 0 && (
          <Stack direction="row" spacing={0.5} sx={{ mt: 2 }}>
            {city.attractions.map((attraction: string, index: number) => (
              <Chip 
                key={index}
                label={attraction} 
                size="small"
                sx={{ bgcolor: alpha(COLORS.info, 0.1) }}
              />
            ))}
          </Stack>
        )}
      </CardContent>
      
      <CardActions>
        <Button
          fullWidth
          variant={isSelected ? 'contained' : 'outlined'}
          onClick={onSelect}
          disabled={disabled && !isSelected}
          color={isSelected ? 'success' : 'primary'}
          startIcon={isSelected ? <CheckCircleIcon /> : <AddIcon />}
        >
          {isSelected ? 'محدد' : 'اختيار'}
        </Button>
      </CardActions>
    </Card>
  );
};

// Offer Card Component
const OfferCard: React.FC<{
  offer: any;
  onSelect: () => void;
  disabled: boolean;
  isSelected: boolean;
}> = ({ offer, onSelect, disabled, isSelected }) => {
  const offerType = OFFER_TYPES.find(t => t.value === offer.value);
  
  return (
    <Card
      elevation={0}
      sx={{
        position: 'relative',
        opacity: disabled && !isSelected ? 0.6 : 1,
        border: isSelected ? `2px solid ${COLORS.success}` : `1px solid ${COLORS.border}`,
        transition: 'all 0.3s ease',
        '&:hover': {
          transform: !disabled || isSelected ? 'translateY(-4px)' : 'none',
          boxShadow: !disabled || isSelected ? 4 : 0,
        },
      }}
    >
      <Box
        sx={{
          position: 'absolute',
          top: 8,
          right: 8,
          zIndex: 1,
          bgcolor: COLORS.error,
          color: 'white',
          borderRadius: '50%',
          width: 70,
          height: 70,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          flexDirection: 'column',
          boxShadow: 3,
        }}
      >
        <Typography variant="h5" sx={{ fontWeight: 700 }}>
          {offer.discountPercentage}%
        </Typography>
        <Typography variant="caption">خصم</Typography>
      </Box>

      {offer.isFlashDeal && (
        <Chip
          label="صفقة برق"
          color="warning"
          size="small"
          icon={<FlashIcon />}
          sx={{
            position: 'absolute',
            top: 8,
            left: 8,
            zIndex: 1,
          }}
        />
      )}

      <CardMedia
        component="img"
        height="180"
        image={offer.imageUrl}
        alt={offer.title}
      />
      
      <CardContent>
        <Stack direction="row" alignItems="center" spacing={1} sx={{ mb: 1 }}>
          {offerType && (
            <Avatar sx={{ bgcolor: alpha(offerType.color, 0.1), color: offerType.color, width: 32, height: 32 }}>
              {offerType.icon}
            </Avatar>
          )}
          <Typography variant="subtitle1" noWrap sx={{ fontWeight: 600 }}>
            {offer.title}
          </Typography>
        </Stack>
        
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          {offer.description}
        </Typography>

        {offer.code && (
          <Paper sx={{ p: 1, bgcolor: alpha(COLORS.primary, 0.05), mb: 2 }}>
            <Stack direction="row" alignItems="center" justifyContent="space-between">
              <Typography variant="caption" color="text.secondary">
                كود الخصم:
              </Typography>
              <Typography variant="subtitle2" sx={{ fontWeight: 600, color: COLORS.primary }}>
                {offer.code}
              </Typography>
            </Stack>
          </Paper>
        )}
        
        {offer.terms && offer.terms.length > 0 && (
          <Stack spacing={0.5} sx={{ mb: 2 }}>
            {offer.terms.map((term: string, index: number) => (
              <Typography key={index} variant="caption" color="text.secondary">
                • {term}
              </Typography>
            ))}
          </Stack>
        )}
        
        <Stack direction="row" alignItems="center" spacing={1}>
          <ScheduleIcon sx={{ fontSize: 16, color: COLORS.text.secondary }} />
          <Typography variant="caption" color="text.secondary">
            صالح حتى: {new Date(offer.endDate).toLocaleDateString('ar-SA')}
          </Typography>
        </Stack>
      </CardContent>
      
      <CardActions>
        <Button
          fullWidth
          variant={isSelected ? 'contained' : 'outlined'}
          onClick={onSelect}
          disabled={disabled && !isSelected}
          color={isSelected ? 'success' : 'primary'}
          startIcon={isSelected ? <CheckCircleIcon /> : <AddIcon />}
        >
          {isSelected ? 'محدد' : 'اختيار'}
        </Button>
      </CardActions>
    </Card>
  );
};

// Ad Card Component
const AdCard: React.FC<{
  ad: any;
  onSelect: () => void;
  disabled: boolean;
  isSelected: boolean;
}> = ({ ad, onSelect, disabled, isSelected }) => {
  return (
    <Card
      elevation={0}
      sx={{
        position: 'relative',
        opacity: disabled && !isSelected ? 0.6 : 1,
        border: isSelected ? `2px solid ${COLORS.success}` : `1px solid ${COLORS.border}`,
        transition: 'all 0.3s ease',
        '&:hover': {
          transform: !disabled || isSelected ? 'translateY(-4px)' : 'none',
          boxShadow: !disabled || isSelected ? 4 : 0,
        },
      }}
    >
      {ad.priority > 0 && (
        <Chip
          label={`أولوية: ${ad.priority}`}
          size="small"
          sx={{
            position: 'absolute',
            top: 8,
            right: 8,
            zIndex: 1,
            bgcolor: 'white',
          }}
        />
      )}

      <Box
        sx={{
          height: 180,
          background: ad.backgroundColor || COLORS.primary,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          position: 'relative',
          overflow: 'hidden',
        }}
      >
        {ad.imageUrl ? (
          <CardMedia
            component="img"
            height="180"
            image={ad.imageUrl}
            alt={ad.title}
            sx={{ objectFit: 'cover' }}
          />
        ) : (
          <Typography
            variant="h5"
            sx={{
              color: ad.textColor || 'white',
              textAlign: 'center',
              p: 2,
              fontWeight: 600,
            }}
          >
            {ad.title}
          </Typography>
        )}
      </Box>
      
      <CardContent>
        <Typography variant="subtitle1" noWrap sx={{ fontWeight: 600 }}>
          {ad.title}
        </Typography>
        
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          {ad.description}
        </Typography>
        
        <Stack direction="row" spacing={1} sx={{ mb: 2 }}>
          <Chip
            label={ad.ctaText}
            size="small"
            color="primary"
            clickable
            icon={<TouchIcon />}
          />
          {ad.position && (
            <Chip label={ad.position} size="small" variant="outlined" />
          )}
        </Stack>

        {ad.impressions && ad.clicks && (
          <Paper sx={{ p: 1.5, bgcolor: COLORS.background.light }}>
            <Grid container spacing={2}>
              <Grid item xs={6}>
                <Stack alignItems="center">
                  <Typography variant="h6" sx={{ fontWeight: 600, color: COLORS.info }}>
                    {ad.impressions.toLocaleString()}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    مشاهدة
                  </Typography>
                </Stack>
              </Grid>
              <Grid item xs={6}>
                <Stack alignItems="center">
                  <Typography variant="h6" sx={{ fontWeight: 600, color: COLORS.success }}>
                    {ad.clicks.toLocaleString()}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    نقرة
                  </Typography>
                </Stack>
              </Grid>
            </Grid>
            <LinearProgress 
              variant="determinate" 
              value={(ad.clicks / ad.impressions) * 100}
              sx={{ mt: 1, height: 6, borderRadius: 3 }}
            />
            <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, display: 'block', textAlign: 'center' }}>
              معدل النقر: {((ad.clicks / ad.impressions) * 100).toFixed(2)}%
            </Typography>
          </Paper>
        )}
      </CardContent>
      
      <CardActions>
        <Button
          fullWidth
          variant={isSelected ? 'contained' : 'outlined'}
          onClick={onSelect}
          disabled={disabled && !isSelected}
          color={isSelected ? 'success' : 'primary'}
          startIcon={isSelected ? <CheckCircleIcon /> : <AddIcon />}
        >
          {isSelected ? 'محدد' : 'اختيار'}
        </Button>
      </CardActions>
    </Card>
  );
};

// Helper functions
const getContentIcon = (type: string) => {
  switch (type) {
    case 'property':
      return <PropertyIcon />;
    case 'city':
      return <CityIcon />;
    case 'offer':
      return <OfferIcon />;
    case 'advertisement':
      return <AdIcon />;
    default:
      return <InfoIcon />;
  }
};

const getContentColor = (type: string): any => {
  switch (type) {
    case 'property':
      return 'primary';
    case 'city':
      return 'secondary';
    case 'offer':
      return 'warning';
    case 'advertisement':
      return 'info';
    default:
      return 'default';
  }
};

export default ContentManagementPanel;
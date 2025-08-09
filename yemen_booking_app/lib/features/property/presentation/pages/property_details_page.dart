import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../injection_container.dart';
import '../bloc/property_bloc.dart';
import '../bloc/property_event.dart';
import '../bloc/property_state.dart';
import '../widgets/property_header_widget.dart';
import '../widgets/property_images_slider_widget.dart';
import '../widgets/property_info_widget.dart';
import '../widgets/amenities_grid_widget.dart';
import '../widgets/units_list_widget.dart';
import '../widgets/reviews_summary_widget.dart';
import '../widgets/policies_widget.dart';
import '../widgets/location_map_widget.dart';

class PropertyDetailsPage extends StatefulWidget {
  final String propertyId;
  final String? userId;

  const PropertyDetailsPage({
    super.key,
    required this.propertyId,
    this.userId,
  });

  @override
  State<PropertyDetailsPage> createState() => _PropertyDetailsPageState();
}

class _PropertyDetailsPageState extends State<PropertyDetailsPage>
    with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final ScrollController _scrollController = ScrollController();
  bool _showFloatingHeader = false;
  double _scrollOffset = 0;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 5, vsync: this);
    _scrollController.addListener(_onScroll);
  }

  @override
  void dispose() {
    _tabController.dispose();
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    setState(() {
      _scrollOffset = _scrollController.offset;
      _showFloatingHeader = _scrollOffset > 250;
    });
  }

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (context) => sl<PropertyBloc>()
        ..add(GetPropertyDetailsEvent(
          propertyId: widget.propertyId,
          userId: widget.userId,
        ))
        ..add(UpdateViewCountEvent(propertyId: widget.propertyId)),
      child: Scaffold(
        backgroundColor: AppColors.background,
        body: BlocBuilder<PropertyBloc, PropertyState>(
          builder: (context, state) {
            if (state is PropertyLoading) {
              return const Center(
                child: LoadingWidget(
                  type: LoadingType.circular,
                  message: 'جاري تحميل تفاصيل العقار...',
                ),
              );
            }

            if (state is PropertyError) {
              return Center(
                child: CustomErrorWidget(
                  message: state.message,
                  onRetry: () {
                    context.read<PropertyBloc>().add(
                      GetPropertyDetailsEvent(
                        propertyId: widget.propertyId,
                        userId: widget.userId,
                      ),
                    );
                  },
                ),
              );
            }

            if (state is PropertyDetailsLoaded) {
              return Stack(
                children: [
                  CustomScrollView(
                    controller: _scrollController,
                    slivers: [
                      _buildSliverAppBar(context, state),
                      SliverToBoxAdapter(
                        child: Column(
                          children: [
                            PropertyHeaderWidget(
                              property: state.property,
                              isFavorite: state.isFavorite,
                              onFavoriteToggle: () => _toggleFavorite(context, state),
                              onShare: () => _shareProperty(state),
                            ),
                            _buildTabBar(),
                            _buildTabContent(state),
                          ],
                        ),
                      ),
                    ],
                  ),
                  if (_showFloatingHeader) _buildFloatingHeader(context, state),
                  _buildBottomBar(context, state),
                ],
              );
            }

            return const SizedBox.shrink();
          },
        ),
      ),
    );
  }

  Widget _buildSliverAppBar(BuildContext context, PropertyDetailsLoaded state) {
    return SliverAppBar(
      expandedHeight: 350,
      pinned: false,
      backgroundColor: AppColors.surface,
      systemOverlayStyle: SystemUiOverlayStyle.light,
      leading: _buildBackButton(context),
      actions: [
        _buildActionButton(
          icon: Icons.share_outlined,
          onPressed: () => _shareProperty(state),
        ),
        _buildActionButton(
          icon: state.isFavorite ? Icons.favorite : Icons.favorite_border,
          color: state.isFavorite ? AppColors.error : null,
          onPressed: () => _toggleFavorite(context, state),
        ),
      ],
      flexibleSpace: FlexibleSpaceBar(
        background: PropertyImagesSliderWidget(
          images: state.property.images,
          onImageTap: (index) => _openGallery(context, state, index),
        ),
      ),
    );
  }

  Widget _buildBackButton(BuildContext context) {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingSmall),
      decoration: BoxDecoration(
        color: AppColors.black.withValues(alpha: 0.5),
        shape: BoxShape.circle,
      ),
      child: IconButton(
        icon: const Icon(Icons.arrow_back, color: AppColors.white),
        onPressed: () => context.pop(),
      ),
    );
  }

  Widget _buildActionButton({
    required IconData icon,
    required VoidCallback onPressed,
    Color? color,
  }) {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingSmall),
      decoration: BoxDecoration(
        color: AppColors.black.withValues(alpha: 0.5),
        shape: BoxShape.circle,
      ),
      child: IconButton(
        icon: Icon(icon, color: color ?? AppColors.white),
        onPressed: onPressed,
      ),
    );
  }

  Widget _buildTabBar() {
    return Container(
      color: AppColors.surface,
      child: TabBar(
        controller: _tabController,
        isScrollable: true,
        labelColor: AppColors.primary,
        unselectedLabelColor: AppColors.textSecondary,
        indicatorColor: AppColors.primary,
        indicatorWeight: 3,
        labelStyle: AppTextStyles.bodyMedium.copyWith(
          fontWeight: FontWeight.bold,
        ),
        unselectedLabelStyle: AppTextStyles.bodyMedium,
        tabs: const [
          Tab(text: 'نظرة عامة'),
          Tab(text: 'الوحدات'),
          Tab(text: 'المرافق'),
          Tab(text: 'التقييمات'),
          Tab(text: 'الموقع'),
        ],
      ),
    );
  }

  Widget _buildTabContent(PropertyDetailsLoaded state) {
    return SizedBox(
      height: MediaQuery.of(context).size.height * 0.6,
      child: TabBarView(
        controller: _tabController,
        children: [
          _buildOverviewTab(state),
          _buildUnitsTab(state),
          _buildAmenitiesTab(state),
          _buildReviewsTab(state),
          _buildLocationTab(state),
        ],
      ),
    );
  }

  Widget _buildOverviewTab(PropertyDetailsLoaded state) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          PropertyInfoWidget(property: state.property),
          const SizedBox(height: AppDimensions.spacingLg),
          if (state.property.services.isNotEmpty) ...[
            _buildSectionTitle('الخدمات المتاحة'),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildServicesList(state),
            const SizedBox(height: AppDimensions.spacingLg),
          ],
          if (state.property.policies.isNotEmpty) ...[
            _buildSectionTitle('السياسات والقوانين'),
            const SizedBox(height: AppDimensions.spacingMd),
            PoliciesWidget(policies: state.property.policies),
          ],
        ],
      ),
    );
  }

  Widget _buildUnitsTab(PropertyDetailsLoaded state) {
    return UnitsListWidget(
      units: state.property.units,
      onUnitSelect: (unit) => _selectUnit(context, unit),
    );
  }

  Widget _buildAmenitiesTab(PropertyDetailsLoaded state) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: AmenitiesGridWidget(amenities: state.property.amenities),
    );
  }

  Widget _buildReviewsTab(PropertyDetailsLoaded state) {
    return ReviewsSummaryWidget(
      propertyId: state.property.id,
      reviewsCount: state.property.reviewsCount,
      averageRating: state.property.averageRating,
      onViewAll: () => _navigateToReviews(context, state),
    );
  }

  Widget _buildLocationTab(PropertyDetailsLoaded state) {
    return LocationMapWidget(
      latitude: state.property.latitude,
      longitude: state.property.longitude,
      propertyName: state.property.name,
      address: state.property.address,
    );
  }

  Widget _buildServicesList(PropertyDetailsLoaded state) {
    return Wrap(
      spacing: AppDimensions.spacingSm,
      runSpacing: AppDimensions.spacingSm,
      children: state.property.services.map((service) {
        return Chip(
          label: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(service.name),
              if (service.price > 0) ...[
                const SizedBox(width: AppDimensions.spacingXs),
                Text(
                  '${service.price} ${service.currency}',
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.primary,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ],
            ],
          ),
          backgroundColor: AppColors.primary.withValues(alpha: 0.1),
        );
      }).toList(),
    );
  }

  Widget _buildSectionTitle(String title) {
    return Text(
      title,
      style: AppTextStyles.heading3.copyWith(
        fontWeight: FontWeight.bold,
      ),
    );
  }

  Widget _buildFloatingHeader(BuildContext context, PropertyDetailsLoaded state) {
    return AnimatedContainer(
      duration: const Duration(milliseconds: 200),
      child: Container(
        color: AppColors.surface,
        child: SafeArea(
          child: Container(
            height: 60,
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingMedium,
            ),
            decoration: const BoxDecoration(
              color: AppColors.surface,
              boxShadow: [
                BoxShadow(
                  color: AppColors.shadow,
                  blurRadius: AppDimensions.blurMedium,
                  offset: Offset(0, 2),
                ),
              ],
            ),
            child: Row(
              children: [
                IconButton(
                  icon: const Icon(Icons.arrow_back),
                  onPressed: () => context.pop(),
                ),
                const SizedBox(width: AppDimensions.spacingSm),
                Expanded(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        state.property.name,
                        style: AppTextStyles.subtitle2.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                      Text(
                        state.property.address,
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ],
                  ),
                ),
                IconButton(
                  icon: Icon(
                    state.isFavorite ? Icons.favorite : Icons.favorite_border,
                    color: state.isFavorite ? AppColors.error : null,
                  ),
                  onPressed: () => _toggleFavorite(context, state),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildBottomBar(BuildContext context, PropertyDetailsLoaded state) {
    final lowestPrice = state.property.units.isNotEmpty
        ? state.property.units
            .map((u) => u.basePrice.amount)
            .reduce((a, b) => a < b ? a : b)
        : 0.0;

    return Positioned(
      bottom: 0,
      left: 0,
      right: 0,
      child: Container(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        decoration: const BoxDecoration(
          color: AppColors.surface,
          boxShadow: [
            BoxShadow(
              color: AppColors.shadow,
              blurRadius: AppDimensions.blurLarge,
              offset: Offset(0, -2),
            ),
          ],
        ),
        child: SafeArea(
          top: false,
          child: Row(
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Text(
                      'يبدأ من',
                      style: AppTextStyles.caption.copyWith(
                        color: AppColors.textSecondary,
                      ),
                    ),
                    Row(
                      children: [
                        Text(
                          lowestPrice.toStringAsFixed(0),
                          style: AppTextStyles.price.copyWith(
                            color: AppColors.primary,
                          ),
                        ),
                        const SizedBox(width: AppDimensions.spacingXs),
                        Text(
                          'ريال / ليلة',
                          style: AppTextStyles.caption.copyWith(
                            color: AppColors.textSecondary,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              ElevatedButton(
                onPressed: () => _navigateToBooking(context, state),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppDimensions.paddingLarge,
                    vertical: AppDimensions.paddingMedium,
                  ),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(
                      AppDimensions.borderRadiusMd,
                    ),
                  ),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.calendar_today, size: 20),
                    const SizedBox(width: AppDimensions.spacingSm),
                    Text(
                      'احجز الآن',
                      style: AppTextStyles.button.copyWith(
                        color: AppColors.white,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  void _toggleFavorite(BuildContext context, PropertyDetailsLoaded state) {
    context.read<PropertyBloc>().add(
      ToggleFavoriteEvent(
        propertyId: state.property.id,
        userId: widget.userId ?? '',
        isFavorite: state.isFavorite,
      ),
    );
  }

  void _shareProperty(PropertyDetailsLoaded state) {
    // Implement share functionality
  }

  void _openGallery(BuildContext context, PropertyDetailsLoaded state, int index) {
    context.push(
      '/property/${state.property.id}/gallery',
      extra: {
        'images': state.property.images,
        'initialIndex': index,
      },
    );
  }

  void _selectUnit(BuildContext context, dynamic unit) {
    // Navigate to booking with selected unit
  }

  void _navigateToReviews(BuildContext context, PropertyDetailsLoaded state) {
    context.push(
      '/property/${state.property.id}/reviews',
      extra: state.property.name,
    );
  }

  void _navigateToBooking(BuildContext context, PropertyDetailsLoaded state) {
    context.push(
      '/booking/form',
      extra: {
        'propertyId': state.property.id,
        'propertyName': state.property.name,
      },
    );
  }
}
// lib/features/home/presentation/pages/home_page.dart

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/error_widget.dart';
import '../bloc/home_bloc.dart';
import '../bloc/analytics_bloc/home_analytics_bloc.dart';
import '../widgets/search_bar_widget.dart';
import '../widgets/choose_required_from_date_to_date_and_capacity_and_city.dart';
import '../widgets/property_categories_widget.dart';
import '../widgets/popular_destinations_widget.dart';
import '../widgets/home_sections_list_widget.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> 
    with AutomaticKeepAliveClientMixin {
  late ScrollController _scrollController;
  double _scrollOffset = 0;
  bool _isSearchExpanded = false;

  @override
  bool get wantKeepAlive => true;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController()
      ..addListener(_onScroll);
    
    // Load home data
    context.read<HomeBloc>().add(const LoadHomeData());
    
    // Track screen view
    context.read<HomeAnalyticsBloc>().add(
      const TrackScreenView(screenName: 'home'),
    );
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    setState(() {
      _scrollOffset = _scrollController.offset;
      
      // Collapse search when scrolling down
      if (_scrollOffset > 100 && _isSearchExpanded) {
        _isSearchExpanded = false;
      }
    });

    // Load more sections when reaching bottom
    if (_scrollController.position.pixels >= 
        _scrollController.position.maxScrollExtent - 200) {
      context.read<HomeBloc>().add(const LoadMoreSections());
    }

    // Track scroll depth
    final scrollDepth = _scrollController.position.pixels / 
                       _scrollController.position.maxScrollExtent;
    if (scrollDepth > 0.5) {
      context.read<HomeAnalyticsBloc>().add(
        TrackScrollDepth(
          depthPercentage: scrollDepth * 100,
          sectionsViewed: 5, // Calculate based on visible sections
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    super.build(context);
    
    return Scaffold(
      backgroundColor: AppColors.background,
      body: BlocBuilder<HomeBloc, HomeState>(
        builder: (context, state) {
          if (state is HomeLoading) {
            return _buildLoadingState();
          }
          
          if (state is HomeError) {
            return _buildErrorState(state);
          }
          
          if (state is HomeLoaded) {
            return _buildLoadedState(state);
          }
          
          return const SizedBox.shrink();
        },
      ),
    );
  }

  Widget _buildLoadingState() {
    return const Center(
      child: LoadingWidget(
        type: LoadingType.circular,
        message: 'جاري تحميل الصفحة الرئيسية...',
      ),
    );
  }

  Widget _buildErrorState(HomeError state) {
    return Center(
      child: CustomErrorWidget(
        message: state.message,
        onRetry: state.canRetry 
            ? () => context.read<HomeBloc>().add(const RetryLoadHome())
            : null,
      ),
    );
  }

  Widget _buildLoadedState(HomeLoaded state) {
    return RefreshIndicator(
      onRefresh: () async {
        context.read<HomeBloc>().add(const RefreshHome());
        await Future.delayed(const Duration(seconds: 1));
      },
      color: AppColors.primary,
      child: CustomScrollView(
        controller: _scrollController,
        physics: const BouncingScrollPhysics(),
        slivers: [
          // Custom App Bar with Search
          _buildSliverAppBar(state),
          
          // Search Filters
          if (_isSearchExpanded)
            SliverToBoxAdapter(
              child: _buildExpandedSearch(state),
            ),
          
          // Property Categories
          SliverToBoxAdapter(
            child: _buildCategoriesSection(),
          ),
          
          // Popular Destinations
          if (state.destinations.isNotEmpty)
            SliverToBoxAdapter(
              child: _buildDestinationsSection(state),
            ),
          
          // Dynamic Sections
          HomeSectionsListWidget(
            sections: state.sections,
            scrollController: _scrollController,
          ),
          
          // Loading More Indicator
          if (state.isLoadingMore)
            const SliverToBoxAdapter(
              child: Padding(
                padding: EdgeInsets.all(AppDimensions.paddingLarge),
                child: Center(
                  child: LoadingWidget(type: LoadingType.circular),
                ),
              ),
            ),
          
          // Bottom Spacing
          const SliverToBoxAdapter(
            child: SizedBox(height: AppDimensions.spacingXl),
          ),
        ],
      ),
    );
  }

  Widget _buildSliverAppBar(HomeLoaded state) {
    final isScrolled = _scrollOffset > 20;
    final appBarHeight = _isSearchExpanded ? 140.0 : 110.0;
    
    return SliverAppBar(
      expandedHeight: appBarHeight,
      floating: true,
      pinned: true,
      elevation: isScrolled ? 4 : 0,
      backgroundColor: AppColors.surface,
      systemOverlayStyle: SystemUiOverlayStyle.dark,
      flexibleSpace: FlexibleSpaceBar(
        background: Container(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              colors: [
                AppColors.surface,
                AppColors.surface.withValues(alpha: 0.95),
              ],
            ),
          ),
          child: SafeArea(
            child: Column(
              children: [
                // App Title
                Padding(
                  padding: const EdgeInsets.only(
                    top: AppDimensions.paddingMedium,
                    left: AppDimensions.paddingMedium,
                    right: AppDimensions.paddingMedium,
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            _getGreeting(),
                            style: AppTextStyles.bodyMedium.copyWith(
                              color: AppColors.textSecondary,
                            ),
                          ),
                          const SizedBox(height: 4),
                          Text(
                            'اكتشف أفضل الأماكن',
                            style: AppTextStyles.heading3.copyWith(
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ],
                      ),
                      // Notification Icon
                      Stack(
                        children: [
                          Container(
                            decoration: const BoxDecoration(
                              color: AppColors.background,
                              shape: BoxShape.circle,
                            ),
                            child: IconButton(
                              icon: const Icon(Icons.notifications_outlined),
                              onPressed: () {
                                // Navigate to notifications
                              },
                              color: AppColors.textPrimary,
                            ),
                          ),
                          Positioned(
                            top: 8,
                            right: 8,
                            child: Container(
                              width: 10,
                              height: 10,
                              decoration: const BoxDecoration(
                                color: AppColors.error,
                                shape: BoxShape.circle,
                              ),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
                
                // Search Bar
                Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppDimensions.paddingMedium,
                    vertical: AppDimensions.paddingSmall,
                  ),
                  child: SearchBarWidget(
                    initialQuery: state.searchQuery,
                    onSearchChanged: (query) {
                      context.read<HomeBloc>().add(UpdateSearchQuery(query));
                    },
                    onTap: () {
                      setState(() {
                        _isSearchExpanded = !_isSearchExpanded;
                      });
                    },
                    isExpanded: _isSearchExpanded,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildExpandedSearch(HomeLoaded state) {
    return AnimatedContainer(
      duration: const Duration(milliseconds: 300),
      curve: Curves.easeInOut,
      decoration: BoxDecoration(
        color: AppColors.surface,
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow.withValues(alpha: 0.1),
            blurRadius: 10,
            offset: const Offset(0, 5),
          ),
        ],
      ),
      child: ChooseRequiredFromDateToDateAndCapacityAndCity(
        selectedCity: state.selectedCity,
        checkInDate: state.checkInDate,
        checkOutDate: state.checkOutDate,
        guestCount: state.guestCount,
        onCityChanged: (city) {
          context.read<HomeBloc>().add(UpdateSelectedCity(city));
        },
        onDateRangeChanged: (checkIn, checkOut) {
          context.read<HomeBloc>().add(UpdateDateRange(
            checkIn: checkIn,
            checkOut: checkOut,
          ));
        },
        onGuestCountChanged: (count) {
          context.read<HomeBloc>().add(UpdateGuestCount(count));
        },
        onSearch: () {
          setState(() {
            _isSearchExpanded = false;
          });
          // Navigate to search results
        },
      ),
    );
  }

  Widget _buildCategoriesSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.all(AppDimensions.paddingMedium),
          child: Text(
            'تصفح حسب النوع',
            style: AppTextStyles.heading2.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        PropertyCategoriesWidget(
          onCategoryTap: (category) {
            // Track analytics
            context.read<HomeAnalyticsBloc>().add(
              TrackItemClick(
                sectionId: 'categories',
                itemId: category.id,
                itemType: 'category',
                position: 0,
              ),
            );
            // Navigate to category
          },
        ),
      ],
    );
  }

  Widget _buildDestinationsSection(HomeLoaded state) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.all(AppDimensions.paddingMedium),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'وجهات شائعة',
                style: AppTextStyles.heading2.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextButton(
                onPressed: () {
                  // Navigate to all destinations
                },
                child: Text(
                  'استكشف المزيد',
                  style: AppTextStyles.bodyMedium.copyWith(
                    color: AppColors.primary,
                  ),
                ),
              ),
            ],
          ),
        ),
        PopularDestinationsWidget(
          destinations: state.destinations,
          onDestinationTap: (destination) {
            // Track analytics
            context.read<HomeAnalyticsBloc>().add(
              TrackItemClick(
                sectionId: 'destinations',
                itemId: destination.id,
                itemType: 'destination',
                position: 0,
              ),
            );
            // Navigate to destination
          },
        ),
      ],
    );
  }

  String _getGreeting() {
    final hour = DateTime.now().hour;
    if (hour < 12) {
      return 'صباح الخير';
    } else if (hour < 18) {
      return 'مساء الخير';
    } else {
      return 'مساء الخير';
    }
  }
}
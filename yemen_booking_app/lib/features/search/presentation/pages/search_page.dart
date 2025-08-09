import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../bloc/search_bloc.dart';
import '../bloc/search_event.dart';
import '../bloc/search_state.dart';
import '../widgets/search_input_widget.dart';
import '../widgets/filter_chips_widget.dart';
import '../widgets/sort_options_widget.dart';
import '../widgets/search_result_list_widget.dart';
import '../widgets/search_result_grid_widget.dart';
import 'search_filters_page.dart';
import 'search_results_map_page.dart';

class SearchPage extends StatefulWidget {
  const SearchPage({super.key});

  @override
  State<SearchPage> createState() => _SearchPageState();
}

class _SearchPageState extends State<SearchPage> {
  final ScrollController _scrollController = ScrollController();
  
  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
    context.read<SearchBloc>().add(const GetSearchFiltersEvent());
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  void _onScroll() {
    if (_isBottom) {
      context.read<SearchBloc>().add(const LoadMoreSearchResultsEvent());
    }
  }

  bool get _isBottom {
    if (!_scrollController.hasClients) return false;
    final maxScroll = _scrollController.position.maxScrollExtent;
    final currentScroll = _scrollController.offset;
    return currentScroll >= (maxScroll * 0.9);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: Column(
          children: [
            _buildHeader(),
            Expanded(
              child: BlocBuilder<SearchBloc, SearchState>(
                builder: (context, state) {
                  if (state is SearchInitial) {
                    return _buildInitialView();
                  } else if (state is SearchLoading) {
                    return _buildLoadingView();
                  } else if (state is SearchSuccess) {
                    return _buildResultsView(state);
                  } else if (state is SearchError) {
                    return _buildErrorView(state.message);
                  }
                  return const SizedBox.shrink();
                },
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Container(
      decoration: const BoxDecoration(
        color: AppColors.surface,
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: AppDimensions.blurSmall,
            offset: Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            child: Row(
              children: [
                IconButton(
                  onPressed: () => Navigator.pop(context),
                  icon: const Icon(Icons.arrow_back_rounded),
                  color: AppColors.textPrimary,
                ),
                Expanded(
                  child: SearchInputWidget(
                    onSubmitted: (query) {
                      context.read<SearchBloc>().add(
                        SearchPropertiesEvent(searchTerm: query),
                      );
                    },
                  ),
                ),
                IconButton(
                  onPressed: _openFilters,
                  icon: Stack(
                    children: [
                      const Icon(Icons.tune_rounded),
                      BlocBuilder<SearchBloc, SearchState>(
                        builder: (context, state) {
                          if (state is SearchSuccess && state.currentFilters.isNotEmpty) {
                            return Positioned(
                              right: 0,
                              top: 0,
                              child: Container(
                                width: 8,
                                height: 8,
                                decoration: const BoxDecoration(
                                  color: AppColors.error,
                                  shape: BoxShape.circle,
                                ),
                              ),
                            );
                          }
                          return const SizedBox.shrink();
                        },
                      ),
                    ],
                  ),
                  color: AppColors.textPrimary,
                ),
              ],
            ),
          ),
          BlocBuilder<SearchBloc, SearchState>(
            builder: (context, state) {
              if (state is SearchSuccess) {
                return Column(
                  children: [
                    FilterChipsWidget(
                      filters: state.currentFilters,
                      onRemoveFilter: (key) {
                        final updatedFilters = Map<String, dynamic>.from(
                          state.currentFilters,
                        )..remove(key);
                        
                        context.read<SearchBloc>().add(
                          UpdateSearchFiltersEvent(filters: updatedFilters),
                        );
                        
                        _performSearch(updatedFilters);
                      },
                    ),
                    Padding(
                      padding: const EdgeInsets.symmetric(
                        horizontal: AppDimensions.paddingMedium,
                        vertical: AppDimensions.paddingSmall,
                      ),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Text(
                            '${state.searchResults.totalCount} نتيجة',
                            style: AppTextStyles.bodyMedium.copyWith(
                              color: AppColors.textSecondary,
                            ),
                          ),
                          Row(
                            children: [
                              SortOptionsWidget(
                                currentSort: state.currentFilters['sortBy'],
                                onSortChanged: (sortBy) {
                                  context.read<SearchBloc>().add(
                                    UpdateSearchFiltersEvent(
                                      filters: {'sortBy': sortBy},
                                    ),
                                  );
                                  _performSearch({'sortBy': sortBy});
                                },
                              ),
                              const SizedBox(width: AppDimensions.spacingSm),
                              IconButton(
                                onPressed: () {
                                  context.read<SearchBloc>().add(
                                    const ToggleViewModeEvent(),
                                  );
                                },
                                icon: Icon(
                                  state.viewMode == ViewMode.list
                                      ? Icons.grid_view_rounded
                                      : state.viewMode == ViewMode.grid
                                          ? Icons.map_rounded
                                          : Icons.list_rounded,
                                ),
                                color: AppColors.primary,
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ],
                );
              }
              return const SizedBox.shrink();
            },
          ),
        ],
      ),
    );
  }

  Widget _buildInitialView() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildPopularDestinations(),
          const SizedBox(height: AppDimensions.spacingXl),
          _buildRecentSearches(),
          const SizedBox(height: AppDimensions.spacingXl),
          _buildSearchCategories(),
        ],
      ),
    );
  }

  Widget _buildPopularDestinations() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'الوجهات الشائعة',
          style: AppTextStyles.heading3,
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        Wrap(
          spacing: AppDimensions.spacingSm,
          runSpacing: AppDimensions.spacingSm,
          children: [
            _buildDestinationChip('صنعاء'),
            _buildDestinationChip('عدن'),
            _buildDestinationChip('تعز'),
            _buildDestinationChip('المكلا'),
            _buildDestinationChip('سيئون'),
          ],
        ),
      ],
    );
  }

  Widget _buildDestinationChip(String destination) {
    return InkWell(
      onTap: () {
        context.read<SearchBloc>().add(
          SearchPropertiesEvent(city: destination),
        );
      },
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingMedium,
          vertical: AppDimensions.paddingSmall,
        ),
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
          border: Border.all(
            color: AppColors.outline,
            width: 1,
          ),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            const Icon(
              Icons.location_on_outlined,
              size: AppDimensions.iconSmall,
              color: AppColors.primary,
            ),
            const SizedBox(width: AppDimensions.spacingXs),
            Text(
              destination,
              style: AppTextStyles.bodyMedium,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildRecentSearches() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              'عمليات البحث الأخيرة',
              style: AppTextStyles.heading3,
            ),
            TextButton(
              onPressed: () {
                // Clear recent searches
              },
              child: const Text('مسح الكل'),
            ),
          ],
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        ListView.builder(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          itemCount: 3,
          itemBuilder: (context, index) {
            return ListTile(
              leading: const Icon(Icons.history_rounded),
              title: const Text('فندق في صنعاء'),
              subtitle: const Text('منذ يومين'),
              trailing: IconButton(
                icon: const Icon(Icons.close_rounded),
                onPressed: () {
                  // Remove this search
                },
              ),
              onTap: () {
                // Perform this search again
              },
            );
          },
        ),
      ],
    );
  }

  Widget _buildSearchCategories() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          'البحث حسب النوع',
          style: AppTextStyles.heading3,
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        GridView.count(
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          crossAxisCount: 2,
          mainAxisSpacing: AppDimensions.spacingMd,
          crossAxisSpacing: AppDimensions.spacingMd,
          childAspectRatio: 2.5,
          children: [
            _buildCategoryCard(
              'فنادق',
              Icons.hotel_rounded,
              AppColors.primary,
            ),
            _buildCategoryCard(
              'شقق',
              Icons.apartment_rounded,
              AppColors.secondary,
            ),
            _buildCategoryCard(
              'فلل',
              Icons.villa_rounded,
              AppColors.accent,
            ),
            _buildCategoryCard(
              'منتجعات',
              Icons.beach_access_rounded,
              AppColors.info,
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildCategoryCard(String title, IconData icon, Color color) {
    return InkWell(
      onTap: () {
        // Search by category
      },
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: Container(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        decoration: BoxDecoration(
          color: color.withValues(alpha: 0.1),
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          border: Border.all(
            color: color.withValues(alpha: 0.3),
            width: 1,
          ),
        ),
        child: Row(
          children: [
            Icon(
              icon,
              color: color,
              size: AppDimensions.iconMedium,
            ),
            const SizedBox(width: AppDimensions.spacingSm),
            Expanded(
              child: Text(
                title,
                style: AppTextStyles.bodyMedium.copyWith(
                  color: color,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildLoadingView() {
    return const Center(
      child: CircularProgressIndicator(),
    );
  }

  Widget _buildResultsView(SearchSuccess state) {
    if (state.viewMode == ViewMode.map) {
      return SearchResultsMapPage(
        results: state.searchResults.items,
        onBackToList: () {
          context.read<SearchBloc>().add(const ToggleViewModeEvent());
        },
      );
    }

    if (state.searchResults.items.isEmpty) {
      return _buildEmptyResults();
    }

    return RefreshIndicator(
      onRefresh: () async {
        _performSearch(state.currentFilters);
      },
      child: state.viewMode == ViewMode.list
          ? SearchResultListWidget(
              results: state.searchResults.items,
              scrollController: _scrollController,
              isLoadingMore: state is SearchLoadingMore,
            )
          : SearchResultGridWidget(
              results: state.searchResults.items,
              scrollController: _scrollController,
              isLoadingMore: state is SearchLoadingMore,
            ),
    );
  }

  Widget _buildEmptyResults() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.search_off_rounded,
            size: 100,
            color: AppColors.textSecondary.withValues(alpha: 0.5),
          ),
          const SizedBox(height: AppDimensions.spacingLg),
          Text(
            'لم يتم العثور على نتائج',
            style: AppTextStyles.heading3.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingSm),
          Text(
            'جرب تغيير معايير البحث',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingXl),
          ElevatedButton.icon(
            onPressed: () {
              context.read<SearchBloc>().add(const ClearSearchEvent());
            },
            icon: const Icon(Icons.refresh_rounded),
            label: const Text('إعادة تعيين البحث'),
          ),
        ],
      ),
    );
  }

  Widget _buildErrorView(String message) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          const Icon(
            Icons.error_outline_rounded,
            size: 100,
            color: AppColors.error,
          ),
          const SizedBox(height: AppDimensions.spacingLg),
          const Text(
            'حدث خطأ',
            style: AppTextStyles.heading3,
          ),
          const SizedBox(height: AppDimensions.spacingSm),
          Text(
            message,
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.textSecondary,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: AppDimensions.spacingXl),
          ElevatedButton.icon(
            onPressed: () {
              context.read<SearchBloc>().add(const SearchPropertiesEvent());
            },
            icon: const Icon(Icons.refresh_rounded),
            label: const Text('إعادة المحاولة'),
          ),
        ],
      ),
    );
  }

  void _openFilters() async {
    final filters = await Navigator.push<Map<String, dynamic>>(
      context,
      MaterialPageRoute(
        builder: (context) => const SearchFiltersPage(),
      ),
    );

    if (filters != null) {
      context.read<SearchBloc>().add(
        UpdateSearchFiltersEvent(filters: filters),
      );
      _performSearch(filters);
    }
  }

  void _performSearch(Map<String, dynamic> filters) {
    context.read<SearchBloc>().add(
      SearchPropertiesEvent(
        searchTerm: filters['searchTerm'],
        city: filters['city'],
        propertyTypeId: filters['propertyTypeId'],
        minPrice: filters['minPrice'],
        maxPrice: filters['maxPrice'],
        minStarRating: filters['minStarRating'],
        requiredAmenities: filters['requiredAmenities'],
        unitTypeId: filters['unitTypeId'],
        serviceIds: filters['serviceIds'],
        dynamicFieldFilters: filters['dynamicFieldFilters'],
        checkIn: filters['checkIn'],
        checkOut: filters['checkOut'],
        guestsCount: filters['guestsCount'],
        latitude: filters['latitude'],
        longitude: filters['longitude'],
        radiusKm: filters['radiusKm'],
        sortBy: filters['sortBy'],
      ),
    );
  }
}
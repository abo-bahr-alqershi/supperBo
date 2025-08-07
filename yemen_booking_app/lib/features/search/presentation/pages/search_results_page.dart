import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../domain/entities/search_result.dart';
import '../bloc/search_bloc.dart';
import '../bloc/search_event.dart';
import '../bloc/search_state.dart';
import '../widgets/search_result_list_widget.dart';
import '../widgets/search_result_grid_widget.dart';

class SearchResultsPage extends StatefulWidget {
  final List<SearchResult> initialResults;
  final Map<String, dynamic> appliedFilters;

  const SearchResultsPage({
    super.key,
    required this.initialResults,
    required this.appliedFilters,
  });

  @override
  State<SearchResultsPage> createState() => _SearchResultsPageState();
}

class _SearchResultsPageState extends State<SearchResultsPage> {
  final ScrollController _scrollController = ScrollController();
  ViewMode _viewMode = ViewMode.list;

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(_onScroll);
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
      appBar: _buildAppBar(),
      body: BlocBuilder<SearchBloc, SearchState>(
        builder: (context, state) {
          if (state is SearchSuccess) {
            return _buildResults(state.searchResults.items);
          }
          return _buildResults(widget.initialResults);
        },
      ),
    );
  }

  PreferredSizeWidget _buildAppBar() {
    return AppBar(
      backgroundColor: AppColors.surface,
      elevation: 0,
      leading: IconButton(
        onPressed: () => Navigator.pop(context),
        icon: const Icon(Icons.arrow_back_rounded),
      ),
      title: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'نتائج البحث',
            style: AppTextStyles.subtitle1,
          ),
          Text(
            '${widget.initialResults.length} نتيجة',
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
      actions: [
        IconButton(
          onPressed: () {
            setState(() {
              _viewMode = _viewMode == ViewMode.list
                  ? ViewMode.grid
                  : ViewMode.list;
            });
          },
          icon: Icon(
            _viewMode == ViewMode.list
                ? Icons.grid_view_rounded
                : Icons.list_rounded,
          ),
        ),
        IconButton(
          onPressed: () {
            // Open sort options
          },
          icon: const Icon(Icons.sort_rounded),
        ),
      ],
      bottom: _buildFilterChips(),
    );
  }

  PreferredSizeWidget? _buildFilterChips() {
    if (widget.appliedFilters.isEmpty) return null;

    return PreferredSize(
      preferredSize: const Size.fromHeight(60),
      child: Container(
        height: 60,
        padding: const EdgeInsets.symmetric(
          vertical: AppDimensions.paddingSmall,
        ),
        child: ListView(
          scrollDirection: Axis.horizontal,
          padding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingMedium,
          ),
          children: widget.appliedFilters.entries.map((entry) {
            return Padding(
              padding: const EdgeInsets.only(right: AppDimensions.spacingSm),
              child: Chip(
                label: Text(_getFilterLabel(entry.key, entry.value)),
                onDeleted: () {
                  // Remove filter
                },
                deleteIcon: const Icon(
                  Icons.close_rounded,
                  size: AppDimensions.iconSmall,
                ),
                backgroundColor: AppColors.primary.withOpacity(0.1),
                deleteIconColor: AppColors.primary,
                labelStyle: AppTextStyles.caption.copyWith(
                  color: AppColors.primary,
                ),
              ),
            );
          }).toList(),
        ),
      ),
    );
  }

  Widget _buildResults(List<SearchResult> results) {
    if (results.isEmpty) {
      return _buildEmptyState();
    }

    return RefreshIndicator(
      onRefresh: () async {
        // Refresh results
      },
      child: _viewMode == ViewMode.list
          ? SearchResultListWidget(
              results: results,
              scrollController: _scrollController,
              isLoadingMore: false,
            )
          : SearchResultGridWidget(
              results: results,
              scrollController: _scrollController,
              isLoadingMore: false,
            ),
    );
  }

  Widget _buildEmptyState() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.search_off_rounded,
            size: 100,
            color: AppColors.textSecondary.withOpacity(0.5),
          ),
          const SizedBox(height: AppDimensions.spacingLg),
          Text(
            'لا توجد نتائج',
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
        ],
      ),
    );
  }

  String _getFilterLabel(String key, dynamic value) {
    switch (key) {
      case 'city':
        return value;
      case 'propertyTypeId':
        return 'نوع العقار';
      case 'minPrice':
      case 'maxPrice':
        return 'السعر';
      case 'minStarRating':
        return '$value نجوم';
      case 'checkIn':
      case 'checkOut':
        return 'التواريخ';
      default:
        return key;
    }
  }
}
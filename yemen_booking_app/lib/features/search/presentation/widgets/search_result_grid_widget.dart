import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../domain/entities/search_result.dart';
import 'search_result_card_widget.dart';

class SearchResultGridWidget extends StatelessWidget {
  final List<SearchResult> results;
  final ScrollController? scrollController;
  final bool isLoadingMore;
  final VoidCallback? onLoadMore;
  final Function(SearchResult)? onItemTap;
  final Function(SearchResult)? onFavoriteToggle;
  final int crossAxisCount;

  const SearchResultGridWidget({
    super.key,
    required this.results,
    this.scrollController,
    this.isLoadingMore = false,
    this.onLoadMore,
    this.onItemTap,
    this.onFavoriteToggle,
    this.crossAxisCount = 2,
  });

  @override
  Widget build(BuildContext context) {
    if (results.isEmpty) {
      return _buildEmptyState();
    }

    return CustomScrollView(
      controller: scrollController,
      slivers: [
        SliverPadding(
          padding: const EdgeInsets.all(AppDimensions.paddingMedium),
          sliver: SliverGrid(
            gridDelegate: SliverGridDelegateWithFixedCrossAxisCount(
              crossAxisCount: crossAxisCount,
              crossAxisSpacing: AppDimensions.spacingMd,
              mainAxisSpacing: AppDimensions.spacingMd,
              childAspectRatio: 0.75,
            ),
            delegate: SliverChildBuilderDelegate(
              (context, index) {
                final result = results[index];
                return SearchResultCardWidget(
                  result: result,
                  onTap: () => onItemTap?.call(result),
                  onFavoriteToggle: () => onFavoriteToggle?.call(result),
                  displayType: CardDisplayType.grid,
                );
              },
              childCount: results.length,
            ),
          ),
        ),
        if (isLoadingMore)
          SliverToBoxAdapter(
            child: _buildLoadingIndicator(),
          ),
      ],
    );
  }

  Widget _buildEmptyState() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.grid_off_rounded,
            size: 80,
            color: AppColors.textSecondary.withOpacity(0.5),
          ),
          const SizedBox(height: AppDimensions.spacingLg),
          Text(
            'لا توجد نتائج',
            style: TextStyle(
              fontSize: 18,
              color: AppColors.textSecondary,
              fontWeight: FontWeight.w500,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildLoadingIndicator() {
    return const Padding(
      padding: EdgeInsets.symmetric(vertical: AppDimensions.paddingLarge),
      child: LoadingWidget(
        type: LoadingType.circular,
        message: 'جاري تحميل المزيد...',
      ),
    );
  }
}
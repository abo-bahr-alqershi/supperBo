import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../domain/entities/search_result.dart';
import 'search_result_card_widget.dart';

class SearchResultListWidget extends StatelessWidget {
  final List<SearchResult> results;
  final ScrollController? scrollController;
  final bool isLoadingMore;
  final VoidCallback? onLoadMore;
  final Function(SearchResult)? onItemTap;
  final Function(SearchResult)? onFavoriteToggle;

  const SearchResultListWidget({
    super.key,
    required this.results,
    this.scrollController,
    this.isLoadingMore = false,
    this.onLoadMore,
    this.onItemTap,
    this.onFavoriteToggle,
  });

  @override
  Widget build(BuildContext context) {
    if (results.isEmpty) {
      return _buildEmptyState();
    }

    return ListView.builder(
      controller: scrollController,
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      itemCount: results.length + (isLoadingMore ? 1 : 0),
      itemBuilder: (context, index) {
        if (index == results.length) {
          return _buildLoadingIndicator();
        }

        final result = results[index];
        return Padding(
          padding: const EdgeInsets.only(bottom: AppDimensions.spacingMd),
          child: SearchResultCardWidget(
            result: result,
            onTap: () => onItemTap?.call(result),
            onFavoriteToggle: () => onFavoriteToggle?.call(result),
            displayType: CardDisplayType.list,
          ),
        );
      },
    );
  }

  Widget _buildEmptyState() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.search_off_rounded,
            size: 80,
            color: AppColors.textSecondary.withValues(alpha: 0.5),
          ),
          const SizedBox(height: AppDimensions.spacingLg),
          const Text(
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
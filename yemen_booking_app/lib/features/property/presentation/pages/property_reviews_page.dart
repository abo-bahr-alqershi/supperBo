import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/empty_widget.dart';
import '../../../../injection_container.dart';
import '../bloc/property_bloc.dart';
import '../bloc/property_event.dart';
import '../bloc/property_state.dart';

class PropertyReviewsPage extends StatefulWidget {
  final String propertyId;
  final String propertyName;

  const PropertyReviewsPage({
    super.key,
    required this.propertyId,
    required this.propertyName,
  });

  @override
  State<PropertyReviewsPage> createState() => _PropertyReviewsPageState();
}

class _PropertyReviewsPageState extends State<PropertyReviewsPage> {
  final ScrollController _scrollController = ScrollController();
  int? _selectedRating;
  String _sortBy = 'CreatedAt';
  String _sortDirection = 'Desc';
  bool _withImagesOnly = false;

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
      // Load more reviews
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
    return BlocProvider(
      create: (context) => sl<PropertyBloc>()
        ..add(GetPropertyReviewsEvent(
          propertyId: widget.propertyId,
          sortBy: _sortBy,
          sortDirection: _sortDirection,
          withImagesOnly: _withImagesOnly,
        )),
      child: Scaffold(
        backgroundColor: AppColors.background,
        appBar: _buildAppBar(),
        body: Column(
          children: [
            _buildFiltersSection(),
            Expanded(
              child: BlocBuilder<PropertyBloc, PropertyState>(
                builder: (context, state) {
                  if (state is PropertyReviewsLoading) {
                    return const Center(
                      child: LoadingWidget(type: LoadingType.circular),
                    );
                  }

                  if (state is PropertyError) {
                    return Center(
                      child: CustomErrorWidget(
                        message: state.message,
                        onRetry: () => _loadReviews(context),
                      ),
                    );
                  }

                  if (state is PropertyReviewsLoaded) {
                    if (state.reviews.isEmpty) {
                      return EmptyWidget(
                        message: 'لا توجد تقييمات حتى الآن',
                        actionWidget: ElevatedButton.icon(
                          onPressed: () {
                            // Navigate to write review
                          },
                          icon: const Icon(Icons.rate_review),
                          label: const Text('كن أول من يقيم'),
                        ),
                      );
                    }

                    return RefreshIndicator(
                      onRefresh: () async => _loadReviews(context),
                      child: ListView.separated(
                        controller: _scrollController,
                        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
                        itemCount: state.hasReachedMax
                            ? state.reviews.length
                            : state.reviews.length + 1,
                        separatorBuilder: (context, index) => const SizedBox(
                          height: AppDimensions.spacingMd,
                        ),
                        itemBuilder: (context, index) {
                          if (index >= state.reviews.length) {
                            return const Center(
                              child: Padding(
                                padding: EdgeInsets.all(AppDimensions.paddingMedium),
                                child: LoadingWidget(type: LoadingType.dots),
                              ),
                            );
                          }
                          
                          final review = state.reviews[index];
                          return _buildReviewCard(review);
                        },
                      ),
                    );
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

  AppBar _buildAppBar() {
    return AppBar(
      backgroundColor: AppColors.surface,
      title: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'التقييمات',
            style: AppTextStyles.heading3,
          ),
          Text(
            widget.propertyName,
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildFiltersSection() {
    return Container(
      color: AppColors.surface,
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: Column(
        children: [
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            child: Row(
              children: [
                _buildFilterChip(
                  label: 'الكل',
                  isSelected: _selectedRating == null,
                  onSelected: (selected) {
                    setState(() {
                      _selectedRating = null;
                    });
                    _applyFilters(context);
                  },
                ),
                const SizedBox(width: AppDimensions.spacingSm),
                ...List.generate(5, (index) {
                  final rating = 5 - index;
                  return Padding(
                    padding: const EdgeInsets.only(right: AppDimensions.spacingSm),
                    child: _buildFilterChip(
                      label: '$rating ⭐',
                      isSelected: _selectedRating == rating,
                      onSelected: (selected) {
                        setState(() {
                          _selectedRating = selected ? rating : null;
                        });
                        _applyFilters(context);
                      },
                    ),
                  );
                }),
              ],
            ),
          ),
          const SizedBox(height: AppDimensions.spacingSm),
          Row(
            children: [
              Expanded(
                child: _buildFilterChip(
                  label: 'مع صور فقط',
                  icon: Icons.photo_library_outlined,
                  isSelected: _withImagesOnly,
                  onSelected: (selected) {
                    setState(() {
                      _withImagesOnly = selected;
                    });
                    _applyFilters(context);
                  },
                ),
              ),
              const SizedBox(width: AppDimensions.spacingSm),
              _buildSortButton(context),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildFilterChip({
    required String label,
    IconData? icon,
    required bool isSelected,
    required Function(bool) onSelected,
  }) {
    return FilterChip(
      label: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (icon != null) ...[
            Icon(icon, size: 16),
            const SizedBox(width: AppDimensions.spacingXs),
          ],
          Text(label),
        ],
      ),
      selected: isSelected,
      onSelected: onSelected,
      selectedColor: AppColors.primary.withValues(alpha: 0.1),
      backgroundColor: AppColors.surface,
      side: BorderSide(
        color: isSelected ? AppColors.primary : AppColors.border,
      ),
      labelStyle: AppTextStyles.bodySmall.copyWith(
        color: isSelected ? AppColors.primary : AppColors.textSecondary,
      ),
    );
  }

  Widget _buildSortButton(BuildContext context) {
    return PopupMenuButton<String>(
      onSelected: (value) {
        final parts = value.split('-');
        setState(() {
          _sortBy = parts[0];
          _sortDirection = parts[1];
        });
        _applyFilters(context);
      },
      itemBuilder: (context) => [
        const PopupMenuItem(
          value: 'CreatedAt-Desc',
          child: Text('الأحدث'),
        ),
        const PopupMenuItem(
          value: 'CreatedAt-Asc',
          child: Text('الأقدم'),
        ),
        const PopupMenuItem(
          value: 'Rating-Desc',
          child: Text('الأعلى تقييماً'),
        ),
        const PopupMenuItem(
          value: 'Rating-Asc',
          child: Text('الأقل تقييماً'),
        ),
      ],
      child: Container(
        padding: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingMedium,
          vertical: AppDimensions.paddingSmall,
        ),
        decoration: BoxDecoration(
          border: Border.all(color: AppColors.border),
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        ),
        child: Row(
          children: [
            const Icon(Icons.sort, size: 20, color: AppColors.textSecondary),
            const SizedBox(width: AppDimensions.spacingXs),
            Text(_getSortLabel(), style: AppTextStyles.bodyMedium),
            const Icon(Icons.arrow_drop_down, color: AppColors.textSecondary),
          ],
        ),
      ),
    );
  }

  Widget _buildReviewCard(dynamic review) {
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
      ),
      child: Padding(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  radius: 24,
                  backgroundColor: AppColors.primary.withValues(alpha: 0.1),
                  child: Text(
                    review.userName[0].toUpperCase(),
                    style: AppTextStyles.subtitle1.copyWith(
                      color: AppColors.primary,
                    ),
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        review.userName,
                        style: AppTextStyles.subtitle2.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(
                        _formatDate(review.createdAt),
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                      ),
                    ],
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppDimensions.paddingSmall,
                    vertical: AppDimensions.paddingXSmall,
                  ),
                  decoration: BoxDecoration(
                    color: _getRatingColor(review.averageRating).withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
                  ),
                  child: Row(
                    children: [
                      Icon(
                        Icons.star,
                        size: 16,
                        color: _getRatingColor(review.averageRating),
                      ),
                      const SizedBox(width: AppDimensions.spacingXs),
                      Text(
                        review.averageRating.toStringAsFixed(1),
                        style: AppTextStyles.bodySmall.copyWith(
                          color: _getRatingColor(review.averageRating),
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            _buildRatingBreakdown(review),
            const SizedBox(height: AppDimensions.spacingMd),
            Text(
              review.comment,
              style: AppTextStyles.bodyMedium.copyWith(height: 1.5),
            ),
            if (review.images.isNotEmpty) ...[
              const SizedBox(height: AppDimensions.spacingMd),
              _buildReviewImages(review.images),
            ],
            if (review.responseText != null) ...[
              const SizedBox(height: AppDimensions.spacingMd),
              _buildManagementResponse(review),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildRatingBreakdown(dynamic review) {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingSmall),
      decoration: BoxDecoration(
        color: AppColors.background,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceAround,
        children: [
          _buildRatingItem('النظافة', review.cleanliness),
          _buildRatingItem('الخدمة', review.service),
          _buildRatingItem('الموقع', review.location),
          _buildRatingItem('القيمة', review.value),
        ],
      ),
    );
  }

  Widget _buildRatingItem(String label, int rating) {
    return Column(
      children: [
        Text(
          label,
          style: AppTextStyles.caption.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingXs),
        Row(
          children: [
            const Icon(
              Icons.star,
              size: 14,
              color: AppColors.ratingStar,
            ),
            const SizedBox(width: 2),
            Text(
              rating.toString(),
              style: AppTextStyles.bodySmall.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildReviewImages(List<dynamic> images) {
    return SizedBox(
      height: 80,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: images.length,
        separatorBuilder: (context, index) => const SizedBox(
          width: AppDimensions.spacingSm,
        ),
        itemBuilder: (context, index) {
          return ClipRRect(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
            child: Image.network(
              images[index].url,
              width: 80,
              height: 80,
              fit: BoxFit.cover,
            ),
          );
        },
      ),
    );
  }

  Widget _buildManagementResponse(dynamic review) {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.primary.withValues(alpha: 0.05),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(
          color: AppColors.primary.withValues(alpha: 0.2),
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              const Icon(
                Icons.business,
                size: 20,
                color: AppColors.primary,
              ),
              const SizedBox(width: AppDimensions.spacingSm),
              Text(
                'رد الإدارة',
                style: AppTextStyles.subtitle2.copyWith(
                  color: AppColors.primary,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ],
          ),
          const SizedBox(height: AppDimensions.spacingSm),
          Text(
            review.responseText!,
            style: AppTextStyles.bodyMedium,
          ),
        ],
      ),
    );
  }

  String _getSortLabel() {
    if (_sortBy == 'CreatedAt' && _sortDirection == 'Desc') return 'الأحدث';
    if (_sortBy == 'CreatedAt' && _sortDirection == 'Asc') return 'الأقدم';
    if (_sortBy == 'Rating' && _sortDirection == 'Desc') return 'الأعلى تقييماً';
    if (_sortBy == 'Rating' && _sortDirection == 'Asc') return 'الأقل تقييماً';
    return 'ترتيب';
  }

  Color _getRatingColor(double rating) {
    if (rating >= 4.5) return AppColors.success;
    if (rating >= 3.5) return AppColors.ratingStar;
    if (rating >= 2.5) return AppColors.warning;
    return AppColors.error;
  }

  String _formatDate(DateTime date) {
    final now = DateTime.now();
    final difference = now.difference(date);

    if (difference.inDays == 0) return 'اليوم';
    if (difference.inDays == 1) return 'أمس';
    if (difference.inDays < 7) return 'منذ ${difference.inDays} أيام';
    if (difference.inDays < 30) return 'منذ ${(difference.inDays / 7).floor()} أسابيع';
    return 'منذ ${(difference.inDays / 30).floor()} أشهر';
  }

  void _loadReviews(BuildContext context) {
    context.read<PropertyBloc>().add(GetPropertyReviewsEvent(
      propertyId: widget.propertyId,
      sortBy: _sortBy,
      sortDirection: _sortDirection,
      withImagesOnly: _withImagesOnly,
    ));
  }

  void _applyFilters(BuildContext context) {
    _loadReviews(context);
  }
}
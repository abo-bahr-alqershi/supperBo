import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/rating_widget.dart';

class ReviewsSummaryWidget extends StatelessWidget {
  final String propertyId;
  final int reviewsCount;
  final double averageRating;
  final VoidCallback onViewAll;

  const ReviewsSummaryWidget({
    super.key,
    required this.propertyId,
    required this.reviewsCount,
    required this.averageRating,
    required this.onViewAll,
  });

  @override
  Widget build(BuildContext context) {
    if (reviewsCount == 0) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.rate_review_outlined,
              size: 48,
              color: AppColors.textSecondary.withOpacity(0.5),
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            Text(
              'لا توجد تقييمات بعد',
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            OutlinedButton.icon(
              onPressed: () {
                // Navigate to write review
              },
              icon: const Icon(Icons.edit),
              label: const Text('كن أول من يقيم'),
            ),
          ],
        ),
      );
    }

    return SingleChildScrollView(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildRatingSummary(),
          const SizedBox(height: AppDimensions.spacingLg),
          _buildRatingBars(),
          const SizedBox(height: AppDimensions.spacingLg),
          _buildRecentReviews(),
          const SizedBox(height: AppDimensions.spacingMd),
          Center(
            child: TextButton.icon(
              onPressed: onViewAll,
              icon: const Icon(Icons.arrow_forward),
              label: Text('عرض جميع التقييمات ($reviewsCount)'),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildRatingSummary() {
    return Card(
      elevation: 0,
      color: AppColors.primary.withOpacity(0.05),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        side: BorderSide(
          color: AppColors.primary.withOpacity(0.2),
        ),
      ),
      child: Padding(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        child: Row(
          children: [
            Column(
              children: [
                Text(
                  averageRating.toStringAsFixed(1),
                  style: AppTextStyles.displaySmall.copyWith(
                    color: AppColors.primary,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: AppDimensions.spacingSm),
                RatingWidget(
                  rating: averageRating,
                  starSize: 20,
                  showLabel: false,
                ),
                const SizedBox(height: AppDimensions.spacingSm),
                Text(
                  '$reviewsCount تقييم',
                  style: AppTextStyles.bodySmall.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
              ],
            ),
            const SizedBox(width: AppDimensions.spacingXl),
            Expanded(
              child: Column(
                children: [
                  _buildRatingCategory('النظافة', 4.5),
                  const SizedBox(height: AppDimensions.spacingSm),
                  _buildRatingCategory('الخدمة', 4.3),
                  const SizedBox(height: AppDimensions.spacingSm),
                  _buildRatingCategory('الموقع', 4.7),
                  const SizedBox(height: AppDimensions.spacingSm),
                  _buildRatingCategory('القيمة', 4.2),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildRatingCategory(String label, double rating) {
    return Row(
      children: [
        SizedBox(
          width: 60,
          child: Text(
            label,
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ),
        const SizedBox(width: AppDimensions.spacingSm),
        Expanded(
          child: ClipRRect(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXs),
            child: LinearProgressIndicator(
              value: rating / 5,
              backgroundColor: AppColors.gray200,
              valueColor: AlwaysStoppedAnimation<Color>(
                _getRatingColor(rating),
              ),
              minHeight: 6,
            ),
          ),
        ),
        const SizedBox(width: AppDimensions.spacingSm),
        Text(
          rating.toStringAsFixed(1),
          style: AppTextStyles.caption.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
      ],
    );
  }

  Widget _buildRatingBars() {
    final ratingDistribution = [
      {'stars': 5, 'count': 45, 'percentage': 0.45},
      {'stars': 4, 'count': 30, 'percentage': 0.30},
      {'stars': 3, 'count': 15, 'percentage': 0.15},
      {'stars': 2, 'count': 7, 'percentage': 0.07},
      {'stars': 1, 'count': 3, 'percentage': 0.03},
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'توزيع التقييمات',
          style: AppTextStyles.subtitle2.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        ...ratingDistribution.map((data) {
          return Padding(
            padding: const EdgeInsets.only(bottom: AppDimensions.spacingSm),
            child: Row(
              children: [
                SizedBox(
                  width: 40,
                  child: Row(
                    children: [
                      Text(
                        data['stars'].toString(),
                        style: AppTextStyles.bodySmall,
                      ),
                      const SizedBox(width: 2),
                      const Icon(
                        Icons.star,
                        size: 14,
                        color: AppColors.ratingStar,
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingSm),
                Expanded(
                  child: ClipRRect(
                    borderRadius: BorderRadius.circular(
                      AppDimensions.borderRadiusXs,
                    ),
                    child: LinearProgressIndicator(
                      value: data['percentage'] as double,
                      backgroundColor: AppColors.gray200,
                      valueColor: const AlwaysStoppedAnimation<Color>(
                        AppColors.ratingStar,
                      ),
                      minHeight: 8,
                    ),
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingSm),
                SizedBox(
                  width: 40,
                  child: Text(
                    '${data['count']}',
                    style: AppTextStyles.caption.copyWith(
                      color: AppColors.textSecondary,
                    ),
                    textAlign: TextAlign.end,
                  ),
                ),
              ],
            ),
          );
        }).toList(),
      ],
    );
  }

  Widget _buildRecentReviews() {
    // Simulated recent reviews
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'أحدث التقييمات',
          style: AppTextStyles.subtitle2.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        // Add 2-3 recent review cards here
        _buildMiniReviewCard(
          userName: 'أحمد محمد',
          rating: 4.5,
          date: 'منذ 3 أيام',
          comment: 'مكان رائع ونظيف، الخدمة ممتازة والموقع مميز. أنصح بشدة بالإقامة هنا.',
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        _buildMiniReviewCard(
          userName: 'فاطمة علي',
          rating: 5.0,
          date: 'منذ أسبوع',
          comment: 'تجربة استثنائية! المكان يفوق التوقعات والموظفون ودودون جداً.',
        ),
      ],
    );
  }

  Widget _buildMiniReviewCard({
    required String userName,
    required double rating,
    required String date,
    required String comment,
  }) {
    return Card(
      elevation: 0,
      color: AppColors.background,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      ),
      child: Padding(
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(
                  radius: 20,
                  backgroundColor: AppColors.primary.withOpacity(0.1),
                  child: Text(
                    userName[0],
                    style: AppTextStyles.bodyMedium.copyWith(
                      color: AppColors.primary,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingMd),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        userName,
                        style: AppTextStyles.bodyMedium.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      Text(
                        date,
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                      ),
                    ],
                  ),
                ),
                RatingWidget(
                  rating: rating,
                  starSize: 16,
                  showLabel: true,
                  showReviewCount: false,
                ),
              ],
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            Text(
              comment,
              style: AppTextStyles.bodySmall.copyWith(
                color: AppColors.textSecondary,
                height: 1.5,
              ),
              maxLines: 3,
              overflow: TextOverflow.ellipsis,
            ),
          ],
        ),
      ),
    );
  }

  Color _getRatingColor(double rating) {
    if (rating >= 4.5) return AppColors.success;
    if (rating >= 3.5) return AppColors.ratingStar;
    if (rating >= 2.5) return AppColors.warning;
    return AppColors.error;
  }
}
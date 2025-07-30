import 'package:flutter/material.dart';
import 'package:flutter_rating_bar/flutter_rating_bar.dart'; // Add to pubspec.yaml
import '../theme/app_colors.dart';

class RatingWidget extends StatelessWidget {
  final double rating;
  final int starCount;
  final double itemSize;
  final bool allowHalfRating;
  final Function(double)? onRatingUpdate; // Callback if rating is interactive
  final Color? starColor;
  final Color? borderColor;
  final double? tapSize; // For larger tap target

  const RatingWidget({
    super.key,
    required this.rating,
    this.starCount = 5,
    this.itemSize = 20.0,
    this.allowHalfRating = true,
    this.onRatingUpdate,
    this.starColor,
    this.borderColor,
    this.tapSize,
  });

  @override
  Widget build(BuildContext context) {
    final effectiveStarColor = starColor ?? Theme.of(context).colorScheme.secondary; // Example: Use a secondary color for stars
    final effectiveBorderColor = borderColor ?? effectiveStarColor.withValues(alpha: 0.5);

    return RatingBar.builder(
      initialRating: rating,
      minRating: 0,
      itemCount: starCount,
      itemSize: itemSize,
      allowHalfRating: allowHalfRating,
      ignoreGestures: onRatingUpdate == null,
      unratedColor: effectiveBorderColor,
      
      itemBuilder: (context, index) {
        if (index < rating.floor()) {
          return Icon(Icons.star_rounded, color: effectiveStarColor);
        } else if (allowHalfRating && index < rating && rating - index >= 0.5) {
          return Icon(Icons.star_half_rounded, color: effectiveStarColor);
        } else {
          return Icon(Icons.star_outline_rounded, color: effectiveBorderColor);
        }
      },
      
      onRatingUpdate: onRatingUpdate ?? (newRating) {
        // If you need to handle the rating update, use the callback
      },
    );
  }
}
import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart'; // Assuming you use SVG for icons
import '../theme/app_colors.dart';
import '../theme/app_text_styles.dart';
import '../utils/validators.dart'; // Assuming you might use validators for retry logic or error messages

class ErrorWidget extends StatelessWidget {
  final String message;
  final VoidCallback? onRetry;
  final String? errorImage; // Path to an error illustration (e.g., SVG or PNG)

  const ErrorWidget({
    super.key,
    required this.message,
    this.onRetry,
    this.errorImage,
  });

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            if (errorImage != null && errorImage!.toLowerCase().endsWith('.svg'))
              SvgPicture.asset(
                errorImage!,
                height: 100, // Adjust size as needed
                // colorFilter: ColorFilter.mode(Theme.of(context).colorScheme.error, BlendMode.srcIn), // Example color tint
              )
            else if (errorImage != null)
              Image.asset(
                errorImage!,
                height: 100, // Adjust size as needed
              ),
            const SizedBox(height: 20.0),
            Text(
              message,
              textAlign: TextAlign.center,
              style: AppTextStyles.bodyLarge.copyWith(color: Theme.of(context).colorScheme.error),
            ),
            if (onRetry != null) ...[
              const SizedBox(height: 24.0),
              ElevatedButton(
                onPressed: onRetry,
                child: Text('إعادة المحاولة'), // Retry button text
              ),
            ],
          ],
        ),
      ),
    );
  }
}
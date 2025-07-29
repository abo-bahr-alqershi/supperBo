import 'package:flutter/material.dart';
import 'package:flutter_spinkit/flutter_spinkit.dart'; // Example: Add dependency flutter_spinkit to pubspec.yaml
import '../theme/app_colors.dart';

class LoadingWidget extends StatelessWidget {
  final double size;
  final Color? color;
  final bool useSystemIndicator; // Use SystemChrome's indicator or custom spinner

  const LoadingWidget({
    super.key,
    this.size = 50.0,
    this.color,
    this.useSystemIndicator = false,
  });

  @override
  Widget build(BuildContext context) {
    if (useSystemIndicator) {
      // Shows the platform-specific loading indicator (e.g., ActivityIndicator on iOS, ProgressBar on Android)
      return Center(
        child: SizedBox(
          width: size,
          height: size,
          child: const CircularProgressIndicator(),
        ),
      );
    } else {
      // Custom spinner using flutter_spinkit (example: SpinKitCircle)
      // Make sure to add flutter_spinkit to your pubspec.yaml dependencies
      return Center(
        child: SpinKitCircle(
          color: color ?? Theme.of(context).primaryColor, // Default to theme primary color
          size: size,
        ),
      );
    }
  }
}
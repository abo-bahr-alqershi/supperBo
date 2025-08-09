// lib/features/home/presentation/widgets/sections/common/gradient_overlay_widget.dart

import 'package:flutter/material.dart';
import 'package:yemen_booking_app/core/utils/color_extensions.dart';

class GradientOverlayWidget extends StatelessWidget {
  final Widget child;
  final List<Color>? colors;
  final AlignmentGeometry begin;
  final AlignmentGeometry end;

  const GradientOverlayWidget({
    super.key,
    required this.child,
    this.colors,
    this.begin = Alignment.topCenter,
    this.end = Alignment.bottomCenter,
  });

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        child,
        Positioned.fill(
          child: Container(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: begin,
                end: end,
                colors: colors ??
                    [
                      Colors.transparent,
                      Colors.black.withValues(alpha: 0.7),
                    ],
              ),
            ),
          ),
        ),
      ],
    );
  }
}
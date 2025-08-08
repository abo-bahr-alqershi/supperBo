// lib/features/home/presentation/widgets/utils/section_visibility_detector.dart

import 'package:flutter/material.dart';
import 'package:visibility_detector/visibility_detector.dart';

class SectionVisibilityDetector extends StatefulWidget {
  final String sectionId;
  final Widget child;
  final Function(bool isVisible)? onVisibilityChanged;
  final Function(double visibleFraction)? onVisibleFractionChanged;
  final double threshold;

  const SectionVisibilityDetector({
    super.key,
    required this.sectionId,
    required this.child,
    this.onVisibilityChanged,
    this.onVisibleFractionChanged,
    this.threshold = 0.5,
  });

  @override
  State<SectionVisibilityDetector> createState() => 
      _SectionVisibilityDetectorState();
}

class _SectionVisibilityDetectorState extends State<SectionVisibilityDetector> {
  bool _isVisible = false;

  @override
  Widget build(BuildContext context) {
    return VisibilityDetector(
      key: Key('visibility_${widget.sectionId}'),
      onVisibilityChanged: (info) {
        widget.onVisibleFractionChanged?.call(info.visibleFraction);
        
        final wasVisible = _isVisible;
        _isVisible = info.visibleFraction >= widget.threshold;
        
        if (wasVisible != _isVisible) {
          widget.onVisibilityChanged?.call(_isVisible);
        }
      },
      child: widget.child,
    );
  }
}
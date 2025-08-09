// lib/features/home/presentation/widgets/sections/base/section_analytics_wrapper.dart

import 'package:flutter/material.dart';
import 'package:visibility_detector/visibility_detector.dart';

class SectionAnalyticsWrapper extends StatefulWidget {
  final String sectionId;
  final String sectionType;
  final Widget child;
  final Function(String sectionId, String sectionType)? onImpressionTracked;
  final double visibilityThreshold;

  const SectionAnalyticsWrapper({
    super.key,
    required this.sectionId,
    required this.sectionType,
    required this.child,
    this.onImpressionTracked,
    this.visibilityThreshold = 0.5,
  });

  @override
  State<SectionAnalyticsWrapper> createState() => 
      _SectionAnalyticsWrapperState();
}

class _SectionAnalyticsWrapperState extends State<SectionAnalyticsWrapper> {
  bool _hasTrackedImpression = false;
  DateTime? _impressionStartTime;

  @override
  Widget build(BuildContext context) {
    return VisibilityDetector(
      key: Key('analytics_${widget.sectionId}'),
      onVisibilityChanged: (info) {
        if (info.visibleFraction >= widget.visibilityThreshold) {
          if (!_hasTrackedImpression) {
            _impressionStartTime = DateTime.now();
            _hasTrackedImpression = true;
            widget.onImpressionTracked?.call(
              widget.sectionId,
              widget.sectionType,
            );
          }
        } else if (info.visibleFraction < 0.1 && _impressionStartTime != null) {
          // Track view duration when section leaves viewport
          final duration = DateTime.now().difference(_impressionStartTime!);
          debugPrint('Section ${widget.sectionId} viewed for ${duration.inSeconds}s');
        }
      },
      child: widget.child,
    );
  }
}
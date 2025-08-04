// lib/core/enums/section_animation_enum.dart

import 'package:flutter/animation.dart';

enum SectionAnimation {
  none('NONE'),
  fade('FADE'),
  slide('SLIDE'),
  scale('SCALE'),
  rotate('ROTATE'),
  parallax('PARALLAX'),
  shimmer('SHIMMER'),
  pulse('PULSE'),
  bounce('BOUNCE'),
  flip('FLIP'),
  slideUp('SLIDE_UP'),
  slideDown('SLIDE_DOWN'),
  slideLeft('SLIDE_LEFT'),
  slideRight('SLIDE_RIGHT'),
  zoomIn('ZOOM_IN'),
  zoomOut('ZOOM_OUT'),
  fadeSlide('FADE_SLIDE'),
  elastic('ELASTIC'),
  custom('CUSTOM');

  final String value;

  const SectionAnimation(this.value);

  static SectionAnimation fromString(String value) {
    return SectionAnimation.values.firstWhere(
      (animation) => animation.value == value,
      orElse: () => SectionAnimation.none,
    );
  }

  static SectionAnimation? tryFromString(String? value) {
    if (value == null) return null;
    try {
      return fromString(value);
    } catch (_) {
      return null;
    }
  }

  // Get animation duration
  Duration get duration {
    switch (this) {
      case SectionAnimation.none:
        return Duration.zero;
      case SectionAnimation.fade:
      case SectionAnimation.slide:
      case SectionAnimation.scale:
        return const Duration(milliseconds: 300);
      case SectionAnimation.rotate:
      case SectionAnimation.flip:
        return const Duration(milliseconds: 400);
      case SectionAnimation.parallax:
        return const Duration(milliseconds: 200);
      case SectionAnimation.shimmer:
        return const Duration(milliseconds: 1500);
      case SectionAnimation.pulse:
        return const Duration(milliseconds: 1000);
      case SectionAnimation.bounce:
      case SectionAnimation.elastic:
        return const Duration(milliseconds: 600);
      case SectionAnimation.slideUp:
      case SectionAnimation.slideDown:
      case SectionAnimation.slideLeft:
      case SectionAnimation.slideRight:
        return const Duration(milliseconds: 350);
      case SectionAnimation.zoomIn:
      case SectionAnimation.zoomOut:
        return const Duration(milliseconds: 400);
      case SectionAnimation.fadeSlide:
        return const Duration(milliseconds: 450);
      case SectionAnimation.custom:
        return const Duration(milliseconds: 500);
    }
  }

  // Get animation curve
  Curve get curve {
    switch (this) {
      case SectionAnimation.none:
        return Curves.linear;
      case SectionAnimation.fade:
      case SectionAnimation.fadeSlide:
        return Curves.easeInOut;
      case SectionAnimation.slide:
      case SectionAnimation.slideUp:
      case SectionAnimation.slideDown:
      case SectionAnimation.slideLeft:
      case SectionAnimation.slideRight:
        return Curves.fastOutSlowIn;
      case SectionAnimation.scale:
      case SectionAnimation.zoomIn:
      case SectionAnimation.zoomOut:
        return Curves.easeOutBack;
      case SectionAnimation.rotate:
      case SectionAnimation.flip:
        return Curves.easeInOutCubic;
      case SectionAnimation.parallax:
        return Curves.linear;
      case SectionAnimation.shimmer:
        return Curves.linear;
      case SectionAnimation.pulse:
        return Curves.easeInOut;
      case SectionAnimation.bounce:
        return Curves.bounceOut;
      case SectionAnimation.elastic:
        return Curves.elasticOut;
      case SectionAnimation.custom:
        return Curves.easeInOut;
    }
  }

  // Get initial offset for slide animations
  Offset get slideOffset {
    switch (this) {
      case SectionAnimation.slide:
      case SectionAnimation.slideRight:
        return const Offset(1.0, 0.0);
      case SectionAnimation.slideLeft:
        return const Offset(-1.0, 0.0);
      case SectionAnimation.slideUp:
        return const Offset(0.0, 1.0);
      case SectionAnimation.slideDown:
        return const Offset(0.0, -1.0);
      case SectionAnimation.fadeSlide:
        return const Offset(0.0, 0.3);
      default:
        return Offset.zero;
    }
  }

  // Get initial scale for scale animations
  double get initialScale {
    switch (this) {
      case SectionAnimation.scale:
        return 0.8;
      case SectionAnimation.zoomIn:
        return 0.5;
      case SectionAnimation.zoomOut:
        return 1.5;
      case SectionAnimation.bounce:
        return 0.3;
      case SectionAnimation.elastic:
        return 0.5;
      default:
        return 1.0;
    }
  }

  // Get initial opacity
  double get initialOpacity {
    switch (this) {
      case SectionAnimation.fade:
      case SectionAnimation.fadeSlide:
      case SectionAnimation.zoomIn:
      case SectionAnimation.zoomOut:
        return 0.0;
      case SectionAnimation.pulse:
        return 0.5;
      default:
        return 1.0;
    }
  }

  // Check if animation involves movement
  bool get hasMovement {
    return [
      SectionAnimation.slide,
      SectionAnimation.slideUp,
      SectionAnimation.slideDown,
      SectionAnimation.slideLeft,
      SectionAnimation.slideRight,
      SectionAnimation.fadeSlide,
      SectionAnimation.parallax,
    ].contains(this);
  }

  // Check if animation involves scaling
  bool get hasScaling {
    return [
      SectionAnimation.scale,
      SectionAnimation.zoomIn,
      SectionAnimation.zoomOut,
      SectionAnimation.bounce,
      SectionAnimation.elastic,
      SectionAnimation.pulse,
    ].contains(this);
  }

  // Check if animation involves rotation
  bool get hasRotation {
    return [
      SectionAnimation.rotate,
      SectionAnimation.flip,
    ].contains(this);
  }

  // Check if animation loops
  bool get isLooping {
    return [
      SectionAnimation.shimmer,
      SectionAnimation.pulse,
    ].contains(this);
  }

  // Get reverse animation type
  SectionAnimation get reverseAnimation {
    switch (this) {
      case SectionAnimation.slideUp:
        return SectionAnimation.slideDown;
      case SectionAnimation.slideDown:
        return SectionAnimation.slideUp;
      case SectionAnimation.slideLeft:
        return SectionAnimation.slideRight;
      case SectionAnimation.slideRight:
        return SectionAnimation.slideLeft;
      case SectionAnimation.zoomIn:
        return SectionAnimation.zoomOut;
      case SectionAnimation.zoomOut:
        return SectionAnimation.zoomIn;
      default:
        return this;
    }
  }
}

// Animation configuration class
class SectionAnimationConfig {
  final SectionAnimation type;
  final Duration? customDuration;
  final Curve? customCurve;
  final double? customDelay;
  final bool reverse;
  final bool autoStart;
  final int? repeatCount;

  const SectionAnimationConfig({
    required this.type,
    this.customDuration,
    this.customCurve,
    this.customDelay,
    this.reverse = false,
    this.autoStart = true,
    this.repeatCount,
  });

  Duration get duration => customDuration ?? type.duration;
  Curve get curve => customCurve ?? type.curve;
  double get delay => customDelay ?? 0.0;
  bool get isInfinite => repeatCount == null && type.isLooping;

  factory SectionAnimationConfig.fromJson(Map<String, dynamic> json) {
    return SectionAnimationConfig(
      type: SectionAnimation.fromString(json['type'] as String? ?? 'NONE'),
      customDuration: json['duration'] != null
          ? Duration(milliseconds: json['duration'] as int)
          : null,
      customDelay: (json['delay'] as num?)?.toDouble(),
      reverse: json['reverse'] as bool? ?? false,
      autoStart: json['autoStart'] as bool? ?? true,
      repeatCount: json['repeatCount'] as int?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'type': type.value,
      if (customDuration != null) 'duration': customDuration!.inMilliseconds,
      if (customDelay != null) 'delay': customDelay,
      'reverse': reverse,
      'autoStart': autoStart,
      if (repeatCount != null) 'repeatCount': repeatCount,
    };
  }
}
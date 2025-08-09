// lib/features/home/presentation/widgets/utils/section_animation_controller.dart

import 'package:flutter/material.dart';
import '../../../../../core/enums/section_animation_enum.dart';

class SectionAnimationController {
  static Animation<double> createAnimation({
    required AnimationController controller,
    required SectionAnimation type,
    Curve curve = Curves.easeInOut,
  }) {
    switch (type) {
      case SectionAnimation.fade:
        return Tween<double>(
          begin: 0.0,
          end: 1.0,
        ).animate(CurvedAnimation(
          parent: controller,
          curve: curve,
        ));
      case SectionAnimation.scale:
        return Tween<double>(
          begin: 0.8,
          end: 1.0,
        ).animate(CurvedAnimation(
          parent: controller,
          curve: curve,
        ));
      default:
        return const AlwaysStoppedAnimation(1.0);
    }
  }

  static Animation<Offset> createSlideAnimation({
    required AnimationController controller,
    Offset begin = const Offset(0, 0.1),
    Offset end = Offset.zero,
    Curve curve = Curves.easeOut,
  }) {
    return Tween<Offset>(
      begin: begin,
      end: end,
    ).animate(CurvedAnimation(
      parent: controller,
      curve: curve,
    ));
  }
}
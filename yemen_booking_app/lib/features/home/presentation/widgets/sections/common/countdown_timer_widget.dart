// lib/features/home/presentation/widgets/sections/common/countdown_timer_widget.dart

import 'dart:async';
import 'package:flutter/material.dart';
import '../../../../../../core/theme/app_colors.dart';
import '../../../../../../core/theme/app_text_styles.dart';

class CountdownTimerWidget extends StatefulWidget {
  final DateTime endTime;
  final TextStyle? textStyle;
  final bool showIcon;

  const CountdownTimerWidget({
    super.key,
    required this.endTime,
    this.textStyle,
    this.showIcon = true,
  });

  @override
  State<CountdownTimerWidget> createState() => _CountdownTimerWidgetState();
}

class _CountdownTimerWidgetState extends State<CountdownTimerWidget> {
  late Timer _timer;
  Duration _remaining = Duration.zero;

  @override
  void initState() {
    super.initState();
    _updateRemaining();
    _timer = Timer.periodic(const Duration(seconds: 1), (_) {
      _updateRemaining();
    });
  }

  @override
  void dispose() {
    _timer.cancel();
    super.dispose();
  }

  void _updateRemaining() {
    final now = DateTime.now();
    if (widget.endTime.isAfter(now)) {
      setState(() {
        _remaining = widget.endTime.difference(now);
      });
    } else {
      setState(() {
        _remaining = Duration.zero;
      });
      _timer.cancel();
    }
  }

  @override
  Widget build(BuildContext context) {
    final hours = _remaining.inHours;
    final minutes = _remaining.inMinutes.remainder(60);
    final seconds = _remaining.inSeconds.remainder(60);

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: AppColors.error.withOpacity(0.1),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (widget.showIcon) ...[
            Icon(
              Icons.timer_outlined,
              size: 16,
              color: AppColors.error,
            ),
            const SizedBox(width: 4),
          ],
          Text(
            '${hours.toString().padLeft(2, '0')}:'
            '${minutes.toString().padLeft(2, '0')}:'
            '${seconds.toString().padLeft(2, '0')}',
            style: widget.textStyle ??
                AppTextStyles.caption.copyWith(
                  color: AppColors.error,
                  fontWeight: FontWeight.bold,
                ),
          ),
        ],
      ),
    );
  }
}
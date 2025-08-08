import 'package:flutter/material.dart';

class SectionAnalyticsWrapper extends StatefulWidget {
  final String sectionId;
  final Widget child;
  final VoidCallback? onImpression;
  const SectionAnalyticsWrapper({super.key, required this.sectionId, required this.child, this.onImpression});

  @override
  State<SectionAnalyticsWrapper> createState() => _SectionAnalyticsWrapperState();
}

class _SectionAnalyticsWrapperState extends State<SectionAnalyticsWrapper> {
  bool _sent = false;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    if (!_sent) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (mounted) {
          _sent = true;
          widget.onImpression?.call();
        }
      });
    }
  }

  @override
  Widget build(BuildContext context) => widget.child;
}
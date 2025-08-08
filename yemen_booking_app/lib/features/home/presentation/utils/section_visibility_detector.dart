import 'package:flutter/widgets.dart';

class SectionVisibilityDetector extends StatefulWidget {
  final String sectionId;
  final Widget child;
  final VoidCallback onVisible;
  const SectionVisibilityDetector({super.key, required this.sectionId, required this.child, required this.onVisible});

  @override
  State<SectionVisibilityDetector> createState() => _SectionVisibilityDetectorState();
}

class _SectionVisibilityDetectorState extends State<SectionVisibilityDetector> with WidgetsBindingObserver {
  bool _reported = false;

  @override
  void didChangeMetrics() {
    _checkVisibility();
  }

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    WidgetsBinding.instance.addPostFrameCallback((_) => _checkVisibility());
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    super.dispose();
  }

  void _checkVisibility() {
    if (_reported) return;
    final renderObject = context.findRenderObject();
    if (renderObject is RenderBox && renderObject.hasSize) {
      final size = renderObject.size;
      if (size.height > 0 && size.width > 0 && mounted) {
        _reported = true;
        widget.onVisible();
      }
    }
  }

  @override
  Widget build(BuildContext context) => widget.child;
}
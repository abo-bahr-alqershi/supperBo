import 'package:flutter/material.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class SearchInputWidget extends StatefulWidget {
  final String? initialValue;
  final String? hintText;
  final Function(String)? onChanged;
  final Function(String)? onSubmitted;
  final VoidCallback? onClear;
  final bool autofocus;
  final bool showSuggestions;
  final List<String>? suggestions;
  final Function(String)? onSuggestionSelected;

  const SearchInputWidget({
    super.key,
    this.initialValue,
    this.hintText,
    this.onChanged,
    this.onSubmitted,
    this.onClear,
    this.autofocus = false,
    this.showSuggestions = true,
    this.suggestions,
    this.onSuggestionSelected,
  });

  @override
  State<SearchInputWidget> createState() => _SearchInputWidgetState();
}

class _SearchInputWidgetState extends State<SearchInputWidget> {
  late TextEditingController _controller;
  late FocusNode _focusNode;
  bool _showClearButton = false;
  OverlayEntry? _overlayEntry;

  @override
  void initState() {
    super.initState();
    _controller = TextEditingController(text: widget.initialValue);
    _focusNode = FocusNode();
    _showClearButton = _controller.text.isNotEmpty;
    
    _controller.addListener(() {
      setState(() {
        _showClearButton = _controller.text.isNotEmpty;
      });
      
      if (widget.onChanged != null) {
        widget.onChanged!(_controller.text);
      }
      
      if (widget.showSuggestions && _controller.text.isNotEmpty) {
        _showSuggestions();
      } else {
        _hideSuggestions();
      }
    });
    
    _focusNode.addListener(() {
      if (!_focusNode.hasFocus) {
        _hideSuggestions();
      }
    });
  }

  @override
  void dispose() {
    _hideSuggestions();
    _controller.dispose();
    _focusNode.dispose();
    super.dispose();
  }

  void _showSuggestions() {
    if (widget.suggestions == null || widget.suggestions!.isEmpty) {
      return;
    }
    
    _hideSuggestions();
    
    _overlayEntry = _createOverlayEntry();
    Overlay.of(context).insert(_overlayEntry!);
  }

  void _hideSuggestions() {
    _overlayEntry?.remove();
    _overlayEntry = null;
  }

  OverlayEntry _createOverlayEntry() {
    RenderBox renderBox = context.findRenderObject() as RenderBox;
    var size = renderBox.size;
    var offset = renderBox.localToGlobal(Offset.zero);

    return OverlayEntry(
      builder: (context) => Positioned(
        left: offset.dx,
        top: offset.dy + size.height + 5,
        width: size.width,
        child: Material(
          elevation: 4,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
          child: Container(
            constraints: const BoxConstraints(maxHeight: 200),
            decoration: BoxDecoration(
              color: AppColors.surface,
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
            ),
            child: ListView.builder(
              padding: EdgeInsets.zero,
              shrinkWrap: true,
              itemCount: widget.suggestions!.length,
              itemBuilder: (context, index) {
                final suggestion = widget.suggestions![index];
                return ListTile(
                  leading: const Icon(
                    Icons.search_rounded,
                    size: AppDimensions.iconSmall,
                    color: AppColors.textSecondary,
                  ),
                  title: Text(
                    suggestion,
                    style: AppTextStyles.bodyMedium,
                  ),
                  onTap: () {
                    _controller.text = suggestion;
                    _hideSuggestions();
                    
                    if (widget.onSuggestionSelected != null) {
                      widget.onSuggestionSelected!(suggestion);
                    } else if (widget.onSubmitted != null) {
                      widget.onSubmitted!(suggestion);
                    }
                  },
                );
              },
            ),
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: AppColors.inputBackground,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
      ),
      child: TextField(
        controller: _controller,
        focusNode: _focusNode,
        autofocus: widget.autofocus,
        decoration: InputDecoration(
          hintText: widget.hintText ?? 'البحث عن العقارات...',
          hintStyle: AppTextStyles.bodyMedium.copyWith(
            color: AppColors.textHint,
          ),
          prefixIcon: const Icon(
            Icons.search_rounded,
            color: AppColors.textSecondary,
          ),
          suffixIcon: _showClearButton
              ? IconButton(
                  onPressed: () {
                    _controller.clear();
                    if (widget.onClear != null) {
                      widget.onClear!();
                    }
                  },
                  icon: const Icon(
                    Icons.clear_rounded,
                    color: AppColors.textSecondary,
                  ),
                )
              : null,
          border: InputBorder.none,
          contentPadding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingMedium,
            vertical: AppDimensions.paddingMedium,
          ),
        ),
        onSubmitted: widget.onSubmitted,
        textInputAction: TextInputAction.search,
      ),
    );
  }
}
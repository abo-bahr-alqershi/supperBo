import 'package:flutter/material.dart';

class SectionHeader extends StatelessWidget {
  final String title;
  final String? subtitle;
  final VoidCallback? onAction;
  final String? actionText;

  const SectionHeader({super.key, required this.title, this.subtitle, this.onAction, this.actionText});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Row(
      crossAxisAlignment: CrossAxisAlignment.end,
      children: [
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(title, style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
              if (subtitle != null && subtitle!.isNotEmpty)
                Padding(
                  padding: const EdgeInsets.only(top: 4),
                  child: Text(subtitle!, style: theme.textTheme.bodySmall?.copyWith(color: theme.hintColor)),
                ),
            ],
          ),
        ),
        if (onAction != null && (actionText?.isNotEmpty ?? false))
          TextButton(onPressed: onAction, child: Text(actionText!)),
      ],
    );
  }
}
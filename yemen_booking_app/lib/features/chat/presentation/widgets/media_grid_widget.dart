import 'package:flutter/material.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/message.dart';

class MediaGridWidget extends StatelessWidget {
  final List<Message> messages;
  final Function(Message) onMediaTap;

  const MediaGridWidget({
    super.key,
    required this.messages,
    required this.onMediaTap,
  });

  @override
  Widget build(BuildContext context) {
    return GridView.builder(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
        crossAxisCount: 3,
        crossAxisSpacing: AppDimensions.spacingSm,
        mainAxisSpacing: AppDimensions.spacingSm,
      ),
      itemCount: messages.length,
      itemBuilder: (context, index) {
        final message = messages[index];
        return _MediaItem(
          message: message,
          onTap: () => onMediaTap(message),
        );
      },
    );
  }
}

class _MediaItem extends StatelessWidget {
  final Message message;
  final VoidCallback onTap;

  const _MediaItem({
    required this.message,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    String? imageUrl;
    bool isVideo = false;

    if (message.messageType == 'image' && message.attachments.isNotEmpty) {
      imageUrl = message.attachments.first.fileUrl;
    } else if (message.messageType == 'video' && message.attachments.isNotEmpty) {
      imageUrl = message.attachments.first.thumbnailUrl;
      isVideo = true;
    }

    if (imageUrl == null) return const SizedBox.shrink();

    return GestureDetector(
      onTap: onTap,
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
        ),
        child: Stack(
          fit: StackFit.expand,
          children: [
            ClipRRect(
              borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
              child: CachedImageWidget(
                imageUrl: imageUrl,
                fit: BoxFit.cover,
              ),
            ),
            if (isVideo)
              Center(
                child: Container(
                  width: 40,
                  height: 40,
                  decoration: BoxDecoration(
                    color: Colors.black.withValues(alpha: 0.7),
                    shape: BoxShape.circle,
                  ),
                  child: const Icon(
                    Icons.play_arrow_rounded,
                    color: Colors.white,
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
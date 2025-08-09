import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/cached_image_widget.dart';
import '../../domain/entities/attachment.dart';

class AttachmentPreviewWidget extends StatelessWidget {
  final Attachment attachment;
  final bool isMe;
  final VoidCallback? onTap;
  final VoidCallback? onDownload;

  const AttachmentPreviewWidget({
    super.key,
    required this.attachment,
    required this.isMe,
    this.onTap,
    this.onDownload,
  });

  @override
  Widget build(BuildContext context) {
    if (attachment.isImage) {
      return _buildImagePreview();
    } else if (attachment.isVideo) {
      return _buildVideoPreview();
    } else if (attachment.isAudio) {
      return _buildAudioPreview();
    } else {
      return _buildDocumentPreview();
    }
  }

  Widget _buildImagePreview() {
    return GestureDetector(
      onTap: () {
        HapticFeedback.lightImpact();
        onTap?.call();
      },
      child: Container(
        constraints: const BoxConstraints(
          maxHeight: 300,
          minHeight: 150,
        ),
        margin: const EdgeInsets.all(AppDimensions.paddingSmall),
        child: ClipRRect(
          borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
          child: Stack(
            fit: StackFit.expand,
            children: [
              CachedImageWidget(
                imageUrl: attachment.url,
                fit: BoxFit.cover,
              ),
              if (attachment.downloadProgress != null &&
                  attachment.downloadProgress! < 1.0)
                _buildProgressOverlay(),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildVideoPreview() {
    return GestureDetector(
      onTap: () {
        HapticFeedback.lightImpact();
        onTap?.call();
      },
      child: Container(
        height: 200,
        margin: const EdgeInsets.all(AppDimensions.paddingSmall),
        child: Stack(
          fit: StackFit.expand,
          children: [
            ClipRRect(
              borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
              child: attachment.thumbnailUrl != null
                  ? CachedImageWidget(
                      imageUrl: attachment.thumbnailUrl!,
                      fit: BoxFit.cover,
                    )
                  : Container(
                      color: AppColors.surface,
                      child: const Icon(
                        Icons.videocam_rounded,
                        size: 48,
                        color: AppColors.textSecondary,
                      ),
                    ),
            ),
            Center(
              child: Container(
                width: 56,
                height: 56,
                decoration: BoxDecoration(
                  color: Colors.black.withValues(alpha: 0.7),
                  shape: BoxShape.circle,
                ),
                child: const Icon(
                  Icons.play_arrow_rounded,
                  color: Colors.white,
                  size: 32,
                ),
              ),
            ),
            Positioned(
              bottom: 8,
              right: 8,
              child: Container(
                padding: const EdgeInsets.symmetric(
                  horizontal: AppDimensions.paddingSmall,
                  vertical: AppDimensions.paddingXSmall,
                ),
                decoration: BoxDecoration(
                  color: Colors.black.withValues(alpha: 0.7),
                  borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
                ),
                child: Text(
                  _formatDuration(attachment.duration),
                  style: AppTextStyles.caption.copyWith(
                    color: Colors.white,
                  ),
                ),
              ),
            ),
            if (attachment.downloadProgress != null &&
                attachment.downloadProgress! < 1.0)
              _buildProgressOverlay(),
          ],
        ),
      ),
    );
  }

  Widget _buildAudioPreview() {
    return Container(
      margin: const EdgeInsets.all(AppDimensions.paddingSmall),
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: isMe
            ? Colors.white.withValues(alpha: 0.2)
            : AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
      ),
      child: Row(
        children: [
          GestureDetector(
            onTap: () {
              HapticFeedback.lightImpact();
              onTap?.call();
            },
            child: Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                color: isMe ? Colors.white : AppColors.primary,
                shape: BoxShape.circle,
              ),
              child: Icon(
                Icons.play_arrow_rounded,
                color: isMe ? AppColors.primary : Colors.white,
              ),
            ),
          ),
          const SizedBox(width: AppDimensions.spacingMd),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                _buildAudioWaveform(),
                const SizedBox(height: AppDimensions.spacingXs),
                Text(
                  _formatDuration(attachment.duration),
                  style: AppTextStyles.caption.copyWith(
                    color: isMe
                        ? Colors.white.withValues(alpha: 0.7)
                        : AppColors.textSecondary,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAudioWaveform() {
    return SizedBox(
      height: 30,
      child: CustomPaint(
        painter: _WaveformPainter(
          color: isMe ? Colors.white : AppColors.primary,
          progress: 0.0,
        ),
        child: const SizedBox.expand(),
      ),
    );
  }

  Widget _buildDocumentPreview() {
    return GestureDetector(
      onTap: () {
        HapticFeedback.lightImpact();
        onTap?.call();
      },
      child: Container(
        margin: const EdgeInsets.all(AppDimensions.paddingSmall),
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        decoration: BoxDecoration(
          color: isMe
              ? Colors.white.withValues(alpha: 0.2)
              : AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
          border: Border.all(
            color: isMe
                ? Colors.white.withValues(alpha: 0.3)
                : AppColors.border,
            width: 1,
          ),
        ),
        child: Row(
          children: [
            Container(
              width: 48,
              height: 48,
              decoration: BoxDecoration(
                color: _getFileIconColor().withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
              ),
              child: Icon(
                _getFileIcon(),
                color: _getFileIconColor(),
                size: 24,
              ),
            ),
            const SizedBox(width: AppDimensions.spacingMd),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    attachment.fileName,
                    style: AppTextStyles.bodyMedium.copyWith(
                      color: isMe ? Colors.white : AppColors.textPrimary,
                      fontWeight: FontWeight.w500,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: AppDimensions.spacingXs),
                  Text(
                    '${attachment.fileExtension.toUpperCase()} • ${_formatFileSize(attachment.fileSize)}',
                    style: AppTextStyles.caption.copyWith(
                      color: isMe
                          ? Colors.white.withValues(alpha: 0.7)
                          : AppColors.textSecondary,
                    ),
                  ),
                ],
              ),
            ),
            if (onDownload != null)
              IconButton(
                onPressed: () {
                  HapticFeedback.lightImpact();
                  onDownload!();
                },
                icon: Icon(
                  Icons.download_rounded,
                  color: isMe ? Colors.white : AppColors.primary,
                ),
              ),
          ],
        ),
      ),
    );
  }

  Widget _buildProgressOverlay() {
    return Container(
      decoration: BoxDecoration(
        color: Colors.black.withValues(alpha: 0.5),
        borderRadius: BorderRadius.circular(AppDimensions.radiusSmall),
      ),
      child: Center(
        child: CircularProgressIndicator(
          value: attachment.downloadProgress,
          strokeWidth: 3,
          valueColor: const AlwaysStoppedAnimation<Color>(Colors.white),
        ),
      ),
    );
  }

  IconData _getFileIcon() {
    switch (attachment.fileExtension.toLowerCase()) {
      case 'pdf':
        return Icons.picture_as_pdf_rounded;
      case 'doc':
      case 'docx':
        return Icons.description_rounded;
      case 'xls':
      case 'xlsx':
        return Icons.table_chart_rounded;
      case 'ppt':
      case 'pptx':
        return Icons.slideshow_rounded;
      case 'zip':
      case 'rar':
        return Icons.folder_zip_rounded;
      default:
        return Icons.insert_drive_file_rounded;
    }
  }

  Color _getFileIconColor() {
    switch (attachment.fileExtension.toLowerCase()) {
      case 'pdf':
        return Colors.red;
      case 'doc':
      case 'docx':
        return Colors.blue;
      case 'xls':
      case 'xlsx':
        return Colors.green;
      case 'ppt':
      case 'pptx':
        return Colors.orange;
      case 'zip':
      case 'rar':
        return Colors.purple;
      default:
        return AppColors.textSecondary;
    }
  }

  String _formatDuration(int? seconds) {
    if (seconds == null) return '0:00';
    final minutes = seconds ~/ 60;
    final remainingSeconds = seconds % 60;
    return '$minutes:${remainingSeconds.toString().padLeft(2, '0')}';
  }

  String _formatFileSize(int bytes) {
    if (bytes < 1024) return '$bytes B';
    if (bytes < 1024 * 1024) return '${(bytes / 1024).toStringAsFixed(1)} KB';
    if (bytes < 1024 * 1024 * 1024) {
      return '${(bytes / (1024 * 1024)).toStringAsFixed(1)} MB';
    }
    return '${(bytes / (1024 * 1024 * 1024)).toStringAsFixed(1)} GB';
  }
}

class _WaveformPainter extends CustomPainter {
  final Color color;
  final double progress;

  _WaveformPainter({
    required this.color,
    required this.progress,
  });

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = color.withValues(alpha: 0.3)
      ..strokeWidth = 2
      ..strokeCap = StrokeCap.round;

    final progressPaint = Paint()
      ..color = color
      ..strokeWidth = 2
      ..strokeCap = StrokeCap.round;

    const barCount = 30;
    final barWidth = size.width / barCount;
    
    for (int i = 0; i < barCount; i++) {
      final x = i * barWidth + barWidth / 2;
      final height = (i % 3 == 0 ? 0.3 : i % 2 == 0 ? 0.5 : 0.7) * size.height;
      final y1 = (size.height - height) / 2;
      final y2 = y1 + height;
      
      final currentPaint = i / barCount <= progress ? progressPaint : paint;
      canvas.drawLine(Offset(x, y1), Offset(x, y2), currentPaint);
    }
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => true;
}
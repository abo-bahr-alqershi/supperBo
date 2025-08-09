import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:image_picker/image_picker.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:record/record.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../domain/entities/message.dart';

class MessageInputWidget extends StatefulWidget {
  final TextEditingController controller;
  final FocusNode focusNode;
  final String conversationId;
  final String? replyToMessageId;
  final Message? editingMessage;
  final Function(String) onSend;
  final VoidCallback? onAttachment;
  final VoidCallback? onLocation;
  final VoidCallback? onCancelReply;
  final VoidCallback? onCancelEdit;

  const MessageInputWidget({
    super.key,
    required this.controller,
    required this.focusNode,
    required this.conversationId,
    this.replyToMessageId,
    this.editingMessage,
    required this.onSend,
    this.onAttachment,
    this.onLocation,
    this.onCancelReply,
    this.onCancelEdit,
  });

  @override
  State<MessageInputWidget> createState() => _MessageInputWidgetState();
}

class _MessageInputWidgetState extends State<MessageInputWidget>
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _sendButtonAnimation;
  
  final AudioRecorder _audioRecorder = AudioRecorder();
  bool _isRecording = false;
  bool _showAttachmentOptions = false;
  String _recordingPath = '';
  Duration _recordingDuration = Duration.zero;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 200),
      vsync: this,
    );
    _sendButtonAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeInOut,
    ));
    
    widget.controller.addListener(_onTextChanged);
  }

  @override
  void dispose() {
    _animationController.dispose();
    _audioRecorder.dispose();
    super.dispose();
  }

  void _onTextChanged() {
    if (widget.controller.text.isNotEmpty && _sendButtonAnimation.value == 0) {
      _animationController.forward();
    } else if (widget.controller.text.isEmpty && _sendButtonAnimation.value == 1) {
      _animationController.reverse();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.only(
        left: AppDimensions.paddingMedium,
        right: AppDimensions.paddingMedium,
        top: AppDimensions.paddingSmall,
        bottom: MediaQuery.of(context).viewInsets.bottom > 0
            ? AppDimensions.paddingSmall
            : AppDimensions.paddingMedium,
      ),
      decoration: BoxDecoration(
        color: Theme.of(context).cardColor,
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow.withValues(alpha: 0.05),
            blurRadius: 10,
            offset: const Offset(0, -2),
          ),
        ],
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (_showAttachmentOptions) _buildAttachmentOptions(),
          Row(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              _buildAttachmentButton(),
              const SizedBox(width: AppDimensions.spacingSm),
              Expanded(child: _buildInputField()),
              const SizedBox(width: AppDimensions.spacingSm),
              _buildActionButton(),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildAttachmentOptions() {
    return Container(
      height: 100,
      margin: const EdgeInsets.only(bottom: AppDimensions.spacingMd),
      child: ListView(
        scrollDirection: Axis.horizontal,
        children: [
          _AttachmentOption(
            icon: Icons.image_rounded,
            label: 'صورة',
            color: Colors.blue,
            onTap: () => _pickImage(ImageSource.gallery),
          ),
          _AttachmentOption(
            icon: Icons.camera_alt_rounded,
            label: 'كاميرا',
            color: Colors.green,
            onTap: () => _pickImage(ImageSource.camera),
          ),
          _AttachmentOption(
            icon: Icons.videocam_rounded,
            label: 'فيديو',
            color: Colors.red,
            onTap: _pickVideo,
          ),
          _AttachmentOption(
            icon: Icons.attach_file_rounded,
            label: 'ملف',
            color: Colors.orange,
            onTap: _pickFile,
          ),
          _AttachmentOption(
            icon: Icons.location_on_rounded,
            label: 'موقع',
            color: Colors.purple,
            onTap: () {
              setState(() {
                _showAttachmentOptions = false;
              });
              widget.onLocation?.call();
            },
          ),
        ],
      ),
    );
  }

  Widget _buildAttachmentButton() {
    return AnimatedRotation(
      duration: const Duration(milliseconds: 200),
      turns: _showAttachmentOptions ? 0.125 : 0,
      child: IconButton(
        onPressed: () {
          HapticFeedback.lightImpact();
          setState(() {
            _showAttachmentOptions = !_showAttachmentOptions;
          });
        },
        icon: Icon(
          Icons.add_circle_outline_rounded,
          color: _showAttachmentOptions 
              ? AppColors.primary 
              : AppColors.textSecondary,
          size: 28,
        ),
      ),
    );
  }

  Widget _buildInputField() {
    return Container(
      constraints: const BoxConstraints(
        minHeight: 48,
        maxHeight: 120,
      ),
      decoration: BoxDecoration(
        color: AppColors.inputBackground,
        borderRadius: BorderRadius.circular(AppDimensions.radiusLarge),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.end,
        children: [
          Expanded(
            child: TextField(
              controller: widget.controller,
              focusNode: widget.focusNode,
              maxLines: null,
              textInputAction: TextInputAction.newline,
              style: AppTextStyles.bodyMedium,
              decoration: InputDecoration(
                hintText: widget.editingMessage != null
                    ? 'تعديل الرسالة...'
                    : widget.replyToMessageId != null
                        ? 'اكتب ردك...'
                        : 'اكتب رسالة...',
                hintStyle: AppTextStyles.bodyMedium.copyWith(
                  color: AppColors.textHint,
                ),
                border: InputBorder.none,
                contentPadding: const EdgeInsets.symmetric(
                  horizontal: AppDimensions.paddingMedium,
                  vertical: AppDimensions.paddingMedium,
                ),
              ),
              onSubmitted: (text) {
                if (text.trim().isNotEmpty) {
                  widget.onSend(text);
                }
              },
            ),
          ),
          IconButton(
            onPressed: _insertEmoji,
            icon: Icon(
              Icons.emoji_emotions_outlined,
              color: AppColors.textSecondary.withValues(alpha: 0.7),
              size: 24,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildActionButton() {
    return AnimatedBuilder(
      animation: _sendButtonAnimation,
      builder: (context, child) {
        final showSend = _sendButtonAnimation.value > 0.5;
        
        return GestureDetector(
          onTap: showSend ? _sendMessage : null,
          onLongPress: !showSend ? _startRecording : null,
          onLongPressEnd: !showSend ? (_) => _stopRecording() : null,
          child: AnimatedContainer(
            duration: const Duration(milliseconds: 200),
            width: 48,
            height: 48,
            decoration: BoxDecoration(
              color: showSend || _isRecording
                  ? AppColors.primary
                  : AppColors.transparent,
              shape: BoxShape.circle,
            ),
            child: Center(
              child: AnimatedSwitcher(
                duration: const Duration(milliseconds: 200),
                child: _isRecording
                    ? _buildRecordingIndicator()
                    : Icon(
                        showSend
                            ? Icons.send_rounded
                            : Icons.mic_rounded,
                        color: showSend || _isRecording
                            ? Colors.white
                            : AppColors.textSecondary,
                        size: 24,
                        key: ValueKey(showSend),
                      ),
              ),
            ),
          ),
        );
      },
    );
  }

  Widget _buildRecordingIndicator() {
    return TweenAnimationBuilder<double>(
      tween: Tween(begin: 0.8, end: 1.2),
      duration: const Duration(milliseconds: 800),
      builder: (context, value, child) {
        return Transform.scale(
          scale: value,
          child: Container(
            width: 20,
            height: 20,
            decoration: BoxDecoration(
              color: Colors.red,
              shape: BoxShape.circle,
              boxShadow: [
                BoxShadow(
                  color: Colors.red.withValues(alpha: 0.5),
                  blurRadius: 10,
                  spreadRadius: 2,
                ),
              ],
            ),
          ),
        );
      },
      onEnd: () {},
    );
  }

  void _sendMessage() {
    final text = widget.controller.text.trim();
    if (text.isNotEmpty) {
      HapticFeedback.lightImpact();
      widget.onSend(text);
    }
  }

  Future<void> _startRecording() async {
    HapticFeedback.mediumImpact();
    
    final permission = await Permission.microphone.request();
    if (!permission.isGranted) return;

    if (await _audioRecorder.hasPermission()) {
      final directory = Directory.systemTemp;
      _recordingPath = '${directory.path}/audio_${DateTime.now().millisecondsSinceEpoch}.m4a';
      
      await _audioRecorder.start(
        const RecordConfig(),
        path: _recordingPath,
      );
      
      setState(() {
        _isRecording = true;
      });
    }
  }

  Future<void> _stopRecording() async {
    if (!_isRecording) return;
    
    HapticFeedback.mediumImpact();
    
    final path = await _audioRecorder.stop();
    
    setState(() {
      _isRecording = false;
    });
    
    if (path != null) {
      // Send audio message
      _sendAudioMessage(path);
    }
  }

  void _sendAudioMessage(String path) {
    // Implement audio message sending
  }

  Future<void> _pickImage(ImageSource source) async {
    final picker = ImagePicker();
    final image = await picker.pickImage(
      source: source,
      maxWidth: 1920,
      maxHeight: 1920,
      imageQuality: 85,
    );
    
    if (image != null) {
      setState(() {
        _showAttachmentOptions = false;
      });
      // Send image
    }
  }

  Future<void> _pickVideo() async {
    final picker = ImagePicker();
    final video = await picker.pickVideo(
      source: ImageSource.gallery,
      maxDuration: const Duration(minutes: 5),
    );
    
    if (video != null) {
      setState(() {
        _showAttachmentOptions = false;
      });
      // Send video
    }
  }

  void _pickFile() {
    setState(() {
      _showAttachmentOptions = false;
    });
    // Implement file picker
  }

  void _insertEmoji() {
    HapticFeedback.lightImpact();
    // Show emoji picker
  }
}

class _AttachmentOption extends StatelessWidget {
  final IconData icon;
  final String label;
  final Color color;
  final VoidCallback onTap;

  const _AttachmentOption({
    required this.icon,
    required this.label,
    required this.color,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: () {
        HapticFeedback.lightImpact();
        onTap();
      },
      child: Container(
        width: 80,
        margin: const EdgeInsets.symmetric(
          horizontal: AppDimensions.spacingSm,
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Container(
              width: 56,
              height: 56,
              decoration: BoxDecoration(
                color: color.withValues(alpha: 0.1),
                shape: BoxShape.circle,
              ),
              child: Icon(
                icon,
                color: color,
                size: 28,
              ),
            ),
            const SizedBox(height: AppDimensions.spacingSm),
            Text(
              label,
              style: AppTextStyles.caption,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
            ),
          ],
        ),
      ),
    );
  }
}
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/empty_widget.dart';
import '../../domain/entities/conversation.dart';
import '../bloc/chat_bloc.dart';
import '../widgets/participant_item_widget.dart';

class NewConversationPage extends StatefulWidget {
  const NewConversationPage({super.key});

  @override
  State<NewConversationPage> createState() => _NewConversationPageState();
}

class _NewConversationPageState extends State<NewConversationPage> {
  final TextEditingController _searchController = TextEditingController();
  final TextEditingController _groupNameController = TextEditingController();
  final Set<ChatUser> _selectedUsers = {};
  bool _isCreatingGroup = false;
  List<ChatUser> _availableUsers = [];
  List<ChatUser> _filteredUsers = [];

  @override
  void initState() {
    super.initState();
    _loadAvailableUsers();
    _searchController.addListener(_filterUsers);
  }

  @override
  void dispose() {
    _searchController.dispose();
    _groupNameController.dispose();
    super.dispose();
  }

  void _loadAvailableUsers() {
    context.read<ChatBloc>().add(const LoadAvailableUsersEvent());
  }

  void _filterUsers() {
    final query = _searchController.text.toLowerCase();
    setState(() {
      if (query.isEmpty) {
        _filteredUsers = _availableUsers;
      } else {
        _filteredUsers = _availableUsers
            .where((user) => user.name.toLowerCase().contains(query))
            .toList();
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: Text(
          _isCreatingGroup ? 'إنشاء مجموعة' : 'محادثة جديدة',
          style: AppTextStyles.heading3,
        ),
        leading: IconButton(
          onPressed: () {
            HapticFeedback.lightImpact();
            if (_isCreatingGroup && _selectedUsers.isNotEmpty) {
              setState(() {
                _isCreatingGroup = false;
              });
            } else {
              Navigator.pop(context);
            }
          },
          icon: const Icon(Icons.arrow_back_ios_new_rounded),
        ),
        actions: [
          if (_selectedUsers.length > 1 && !_isCreatingGroup)
            TextButton(
              onPressed: () {
                setState(() {
                  _isCreatingGroup = true;
                });
              },
              child: const Text('التالي'),
            ),
        ],
      ),
      body: BlocListener<ChatBloc, ChatState>(
        listener: (context, state) {
          if (state is ChatLoaded && state.availableUsers.isNotEmpty) {
            setState(() {
              _availableUsers = state.availableUsers;
              _filteredUsers = state.availableUsers;
            });
          }
        },
        child: _isCreatingGroup
            ? _buildGroupCreationView()
            : _buildUserSelectionView(),
      ),
      floatingActionButton: _buildFAB(),
    );
  }

  Widget _buildUserSelectionView() {
    return Column(
      children: [
        _buildSearchBar(),
        if (_selectedUsers.isNotEmpty) _buildSelectedUsers(),
        Expanded(
          child: BlocBuilder<ChatBloc, ChatState>(
            builder: (context, state) {
              if (state is ChatLoading) {
                return const LoadingWidget(
                  type: LoadingType.circular,
                  message: 'جاري تحميل جهات الاتصال...',
                );
              }

              if (_filteredUsers.isEmpty) {
                return const EmptyWidget(
                  message: 'لا توجد جهات اتصال',
                );
              }

              return ListView.builder(
                padding: const EdgeInsets.symmetric(
                  vertical: AppDimensions.paddingSmall,
                ),
                itemCount: _filteredUsers.length,
                itemBuilder: (context, index) {
                  final user = _filteredUsers[index];
                  final isSelected = _selectedUsers.contains(user);

                  return CheckboxListTile(
                    value: isSelected,
                    onChanged: (value) {
                      setState(() {
                        if (value == true) {
                          _selectedUsers.add(user);
                        } else {
                          _selectedUsers.remove(user);
                        }
                      });

                      if (_selectedUsers.length == 1) {
                        _startDirectChat(_selectedUsers.first);
                      }
                    },
                    secondary: _buildUserAvatar(user),
                    title: Text(
                      user.name,
                      style: AppTextStyles.bodyLarge.copyWith(
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                    subtitle: user.bio != null
                        ? Text(
                            user.bio!,
                            style: AppTextStyles.bodySmall.copyWith(
                              color: AppColors.textSecondary,
                            ),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          )
                        : null,
                    activeColor: AppColors.primary,
                  );
                },
              );
            },
          ),
        ),
      ],
    );
  }

  Widget _buildGroupCreationView() {
    return Padding(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: Column(
        children: [
          TextField(
            controller: _groupNameController,
            autofocus: true,
            decoration: InputDecoration(
              labelText: 'اسم المجموعة',
              hintText: 'أدخل اسم المجموعة',
              prefixIcon: const Icon(Icons.group_rounded),
              border: OutlineInputBorder(
                borderRadius: BorderRadius.circular(AppDimensions.radiusMedium),
              ),
            ),
          ),
          const SizedBox(height: AppDimensions.spacingLg),
          Expanded(
            child: ListView.builder(
              itemCount: _selectedUsers.length,
              itemBuilder: (context, index) {
                final user = _selectedUsers.elementAt(index);
                return ParticipantItemWidget(
                  participant: user,
                  onRemove: () {
                    setState(() {
                      _selectedUsers.remove(user);
                      if (_selectedUsers.length < 2) {
                        _isCreatingGroup = false;
                      }
                    });
                  },
                );
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSearchBar() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      color: AppColors.surface,
      child: TextField(
        controller: _searchController,
        decoration: InputDecoration(
          hintText: 'البحث عن جهات الاتصال...',
          prefixIcon: const Icon(Icons.search_rounded),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(AppDimensions.radiusLarge),
            borderSide: BorderSide.none,
          ),
          filled: true,
          fillColor: AppColors.inputBackground,
        ),
      ),
    );
  }

  Widget _buildSelectedUsers() {
    return Container(
      height: 100,
      padding: const EdgeInsets.symmetric(
        vertical: AppDimensions.paddingSmall,
      ),
      child: ListView.builder(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(
          horizontal: AppDimensions.paddingMedium,
        ),
        itemCount: _selectedUsers.length,
        itemBuilder: (context, index) {
          final user = _selectedUsers.elementAt(index);
          return Padding(
            padding: const EdgeInsets.only(right: AppDimensions.spacingMd),
            child: Column(
              children: [
                Stack(
                  children: [
                    _buildUserAvatar(user),
                    Positioned(
                      top: 0,
                      right: 0,
                      child: GestureDetector(
                        onTap: () {
                          setState(() {
                            _selectedUsers.remove(user);
                          });
                        },
                        child: Container(
                          width: 20,
                          height: 20,
                          decoration: const BoxDecoration(
                            color: AppColors.error,
                            shape: BoxShape.circle,
                          ),
                          child: const Icon(
                            Icons.close_rounded,
                            color: Colors.white,
                            size: 14,
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: AppDimensions.spacingSm),
                SizedBox(
                  width: 60,
                  child: Text(
                    user.name,
                    style: AppTextStyles.caption,
                    textAlign: TextAlign.center,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildUserAvatar(ChatUser user) {
    return Container(
      width: 48,
      height: 48,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        color: AppColors.primary.withValues(alpha: 0.1),
      ),
      child: Center(
        child: Text(
          _getInitials(user.name),
          style: AppTextStyles.subtitle1.copyWith(
            color: AppColors.primary,
            fontWeight: FontWeight.w600,
          ),
        ),
      ),
    );
  }

  Widget? _buildFAB() {
    if (!_isCreatingGroup || _groupNameController.text.isEmpty) {
      return null;
    }

    return FloatingActionButton(
      onPressed: _createGroup,
      backgroundColor: AppColors.primary,
      child: const Icon(
        Icons.check_rounded,
        color: Colors.white,
      ),
    );
  }

  void _startDirectChat(ChatUser user) {
    HapticFeedback.mediumImpact();
    context.read<ChatBloc>().add(
      CreateConversationEvent(
        participantIds: [user.id],
        conversationType: 'direct',
      ),
    );
    Navigator.pop(context);
  }

  void _createGroup() {
    if (_groupNameController.text.isEmpty || _selectedUsers.length < 2) {
      return;
    }

    HapticFeedback.mediumImpact();
    context.read<ChatBloc>().add(
      CreateConversationEvent(
        participantIds: _selectedUsers.map((u) => u.id).toList(),
        conversationType: 'group',
        title: _groupNameController.text,
      ),
    );
    Navigator.pop(context);
  }

  String _getInitials(String name) {
    final parts = name.trim().split(' ');
    if (parts.isEmpty) return '';
    if (parts.length == 1) return parts.first[0].toUpperCase();
    return '${parts.first[0]}${parts.last[0]}'.toUpperCase();
  }
}
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../core/theme/app_colors.dart';
import '../../features/home/presentation/pages/home_page.dart';
import '../../features/search/presentation/pages/search_page.dart';
import '../../features/booking/presentation/pages/my_bookings_page.dart';
import '../../features/chat/presentation/pages/conversations_page.dart';
import '../../features/auth/presentation/pages/profile_page.dart';
import '../../features/notifications/presentation/bloc/notification_bloc.dart';
import '../../features/notifications/presentation/bloc/notification_event.dart';
import '../../features/notifications/presentation/bloc/notification_state.dart';

class MainScreen extends StatefulWidget {
  const MainScreen({super.key});

  @override
  State<MainScreen> createState() => _MainScreenState();
}

class _MainScreenState extends State<MainScreen> with TickerProviderStateMixin {
  int _currentIndex = 0;
  late PageController _pageController;
  late AnimationController _fabAnimationController;
  late AnimationController _borderAnimationController;
  
  final List<Widget> _pages = [
    const HomePage(),
    const SearchPage(),
    const MyBookingsPage(),
    const ConversationsPage(),
    const ProfilePage(),
  ];

  @override
  void initState() {
    super.initState();
    _pageController = PageController(initialPage: _currentIndex);
    _fabAnimationController = AnimationController(
      duration: const Duration(milliseconds: 500),
      vsync: this,
    );
    _borderAnimationController = AnimationController(
      duration: const Duration(milliseconds: 300),
      vsync: this,
    );
    
    // تحميل الإشعارات
    context.read<NotificationBloc>().add(const LoadNotificationsEvent());
  }

  @override
  void dispose() {
    _pageController.dispose();
    _fabAnimationController.dispose();
    _borderAnimationController.dispose();
    super.dispose();
  }

  void _onTabTapped(int index) {
    setState(() {
      _currentIndex = index;
    });
    
    _pageController.animateToPage(
      index,
      duration: const Duration(milliseconds: 400),
      curve: Curves.easeOutCubic,
    );
    
    // تشغيل الحركة عند التبديل
    _borderAnimationController.forward().then((_) {
      _borderAnimationController.reverse();
    });
    
    // إضافة haptic feedback
    HapticFeedback.lightImpact();
  }

  void _onPageChanged(int index) {
    setState(() {
      _currentIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: Stack(
        children: [
          // الصفحات الرئيسية
          PageView(
            controller: _pageController,
            onPageChanged: _onPageChanged,
            physics: const NeverScrollableScrollPhysics(),
            children: _pages,
          ),
          
          // شريط التنقل السفلي المخصص
          Positioned(
            left: 0,
            right: 0,
            bottom: 0,
            child: _buildCustomBottomBar(),
          ),
        ],
      ),
    );
  }

  Widget _buildCustomBottomBar() {
    return Container(
      height: 85,
      decoration: BoxDecoration(
        gradient: LinearGradient(
          begin: Alignment.topCenter,
          end: Alignment.bottomCenter,
          colors: [
            Colors.transparent,
            Colors.black.withValues(alpha: 0.02),
          ],
        ),
      ),
      child: Stack(
        children: [
          // الخلفية البيضاء مع الظل
          Positioned(
            bottom: 0,
            left: 0,
            right: 0,
            child: Container(
              height: 75,
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: const BorderRadius.only(
                  topLeft: Radius.circular(25),
                  topRight: Radius.circular(25),
                ),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withValues(alpha: 0.08),
                    blurRadius: 20,
                    offset: const Offset(0, -5),
                  ),
                ],
              ),
            ),
          ),
          
          // العناصر
          Positioned(
            bottom: 0,
            left: 0,
            right: 0,
            child: SafeArea(
              child: Container(
                height: 65,
                padding: const EdgeInsets.symmetric(horizontal: 8),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceAround,
                  children: List.generate(5, (index) => _buildNavItem(index)),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildNavItem(int index) {
    final isSelected = _currentIndex == index;
    final icons = [
      Icons.home_rounded,
      Icons.explore_rounded,
      Icons.calendar_month_rounded,
      Icons.message_rounded,
      Icons.person_rounded,
    ];
    
    final labels = [
      'الرئيسية',
      'استكشف',
      'حجوزاتي',
      'الرسائل',
      'حسابي',
    ];

    return GestureDetector(
      onTap: () => _onTabTapped(index),
      behavior: HitTestBehavior.opaque,
      child: Container(
        width: 65,
        padding: const EdgeInsets.symmetric(vertical: 8),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            // حاوي الأيقونة
            AnimatedContainer(
              duration: const Duration(milliseconds: 300),
              curve: Curves.easeOutBack,
              width: isSelected ? 48 : 40,
              height: isSelected ? 32 : 28,
              decoration: BoxDecoration(
                color: isSelected 
                  ? AppColors.primary.withValues(alpha: 0.1)
                  : Colors.transparent,
                borderRadius: BorderRadius.circular(isSelected ? 16 : 12),
              ),
              child: Stack(
                alignment: Alignment.center,
                children: [
                  // الأيقونة
                  Icon(
                    icons[index],
                    size: isSelected ? 24 : 22,
                    color: isSelected 
                      ? AppColors.primary 
                      : AppColors.textSecondary.withValues(alpha: 0.7),
                  ),
                  
                  // شارة الإشعارات للرسائل
                  if (index == 3)
                    BlocBuilder<NotificationBloc, NotificationState>(
                      builder: (context, state) {
                        int unreadCount = 0;
                        if (state is NotificationLoaded) {
                          unreadCount = state.notifications
                              .where((n) => !n.isRead)
                              .length;
                        }
                        
                        if (unreadCount == 0) return const SizedBox.shrink();
                        
                        return Positioned(
                          top: 0,
                          right: 8,
                          child: AnimatedContainer(
                            duration: const Duration(milliseconds: 300),
                            padding: const EdgeInsets.all(2),
                            decoration: BoxDecoration(
                              gradient: LinearGradient(
                                colors: [
                                  AppColors.error,
                                  AppColors.error.withValues(alpha: 0.8),
                                ],
                              ),
                              borderRadius: BorderRadius.circular(8),
                              border: Border.all(
                                color: Colors.white,
                                width: 1.5,
                              ),
                            ),
                            constraints: const BoxConstraints(
                              minWidth: 16,
                              minHeight: 16,
                            ),
                            child: Text(
                              unreadCount > 9 ? '9+' : unreadCount.toString(),
                              style: const TextStyle(
                                color: Colors.white,
                                fontSize: 9,
                                fontWeight: FontWeight.bold,
                                height: 1,
                              ),
                              textAlign: TextAlign.center,
                            ),
                          ),
                        );
                      },
                    ),
                ],
              ),
            ),
            
            const SizedBox(height: 4),
            
            // النص
            AnimatedDefaultTextStyle(
              duration: const Duration(milliseconds: 300),
              style: TextStyle(
                fontSize: isSelected ? 11 : 10,
                fontWeight: isSelected ? FontWeight.w600 : FontWeight.w400,
                color: isSelected 
                  ? AppColors.primary 
                  : AppColors.textSecondary.withValues(alpha: 0.7),
                height: 1,
              ),
              child: Text(labels[index]),
            ),
            
            // مؤشر النشاط
            AnimatedContainer(
              duration: const Duration(milliseconds: 300),
              curve: Curves.easeOutBack,
              margin: const EdgeInsets.only(top: 4),
              width: isSelected ? 5 : 0,
              height: isSelected ? 5 : 0,
              decoration: BoxDecoration(
                color: AppColors.primary,
                borderRadius: BorderRadius.circular(2.5),
                boxShadow: isSelected ? [
                  BoxShadow(
                    color: AppColors.primary.withValues(alpha: 0.3),
                    blurRadius: 4,
                    offset: const Offset(0, 2),
                  ),
                ] : [],
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// إضافة widget مخصص للحركات الإضافية
class AnimatedNavIcon extends StatelessWidget {
  final IconData icon;
  final bool isSelected;
  final Animation<double> animation;

  const AnimatedNavIcon({
    super.key,
    required this.icon,
    required this.isSelected,
    required this.animation,
  });

  @override
  Widget build(BuildContext context) {
    return AnimatedBuilder(
      animation: animation,
      builder: (context, child) {
        return Transform.scale(
          scale: isSelected ? 1.0 + (animation.value * 0.1) : 1.0,
          child: Icon(
            icon,
            size: isSelected ? 24 : 22,
            color: isSelected 
              ? AppColors.primary 
              : AppColors.textSecondary.withValues(alpha: 0.7),
          ),
        );
      },
    );
  }
}

// Widget للإضافة تأثير التموج المخصص
class RippleAnimation extends StatefulWidget {
  final Widget child;
  final VoidCallback onTap;

  const RippleAnimation({
    super.key,
    required this.child,
    required this.onTap,
  });

  @override
  State<RippleAnimation> createState() => _RippleAnimationState();
}

class _RippleAnimationState extends State<RippleAnimation>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _scaleAnimation;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      duration: const Duration(milliseconds: 200),
      vsync: this,
    );
    _scaleAnimation = Tween<double>(
      begin: 1.0,
      end: 0.95,
    ).animate(CurvedAnimation(
      parent: _controller,
      curve: Curves.easeInOut,
    ));
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _handleTap() {
    _controller.forward().then((_) {
      _controller.reverse();
    });
    widget.onTap();
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: _handleTap,
      child: AnimatedBuilder(
        animation: _scaleAnimation,
        builder: (context, child) {
          return Transform.scale(
            scale: _scaleAnimation.value,
            child: widget.child,
          );
        },
      ),
    );
  }
}
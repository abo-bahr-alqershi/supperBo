import 'package:flutter/material.dart';
import '../../core/utils/color_extensions.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import '../../core/theme/app_colors.dart';
import '../../core/theme/app_text_styles.dart';
import '../../features/auth/presentation/bloc/auth_bloc.dart';
import '../../features/auth/presentation/bloc/auth_state.dart';

class SplashScreen extends StatefulWidget {
  const SplashScreen({super.key});

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen>
    with TickerProviderStateMixin {
  late AnimationController _fadeController;
  late AnimationController _scaleController;
  late AnimationController _shimmerController;
  late Animation<double> _fadeAnimation;
  late Animation<double> _scaleAnimation;
  late Animation<double> _shimmerAnimation;

  @override
  void initState() {
    super.initState();
    _initializeAnimations();
    _startAnimations();
    _navigateAfterDelay();
  }

  void _initializeAnimations() {
    // تحكم بالظهور التدريجي
    _fadeController = AnimationController(
      duration: const Duration(milliseconds: 2000),
      vsync: this,
    );

    // تحكم بالحجم
    _scaleController = AnimationController(
      duration: const Duration(milliseconds: 1500),
      vsync: this,
    );

    // تحكم بتأثير اللمعان
    _shimmerController = AnimationController(
      duration: const Duration(milliseconds: 2000),
      vsync: this,
    )..repeat(reverse: true);

    // حركة الظهور التدريجي
    _fadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _fadeController,
      curve: const Interval(0.0, 0.5, curve: Curves.easeIn),
    ));

    // حركة التكبير البسيطة
    _scaleAnimation = Tween<double>(
      begin: 0.8,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _scaleController,
      curve: Curves.easeOutBack,
    ));

    // حركة اللمعان
    _shimmerAnimation = Tween<double>(
      begin: 0.3,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _shimmerController,
      curve: Curves.easeInOut,
    ));
  }

  void _startAnimations() {
    _fadeController.forward();
    _scaleController.forward();
  }

  void _navigateAfterDelay() {
    Future.delayed(const Duration(milliseconds: 3500), () {
      if (mounted) {
        _checkAuthAndNavigate();
      }
    });
  }

  void _checkAuthAndNavigate() {
    final authState = context.read<AuthBloc>().state;
    
    if (authState is AuthAuthenticated) {
      context.go('/main');
    } else if (authState is AuthUnauthenticated) {
      context.go('/login');
    } else {
      context.go('/main');
    }
  }

  @override
  void dispose() {
    _fadeController.dispose();
    _scaleController.dispose();
    _shimmerController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    SystemChrome.setSystemUIOverlayStyle(
      const SystemUiOverlayStyle(
        statusBarColor: Colors.transparent,
        statusBarIconBrightness: Brightness.dark,
      ),
    );

    return Scaffold(
      backgroundColor: Colors.white,
      body: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [
              Colors.white,
              AppColors.primary.withValues(alpha: 0.05),
            ],
          ),
        ),
        child: Stack(
          children: [
            // خلفية بنمط هندسي خفيف
            Positioned.fill(
              child: CustomPaint(
                painter: _BackgroundPatternPainter(
                  color: AppColors.primary.withValues(alpha: 0.03),
                ),
              ),
            ),
            
            // المحتوى الرئيسي
            Center(
              child: AnimatedBuilder(
                animation: Listenable.merge([
                  _fadeAnimation,
                  _scaleAnimation,
                ]),
                builder: (context, child) {
                  return FadeTransition(
                    opacity: _fadeAnimation,
                    child: Transform.scale(
                      scale: _scaleAnimation.value,
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          // الشعار
                          _buildLogo(),
                          
                          const SizedBox(height: 48),
                          
                          // اسم التطبيق
                          _buildAppName(),
                          
                          const SizedBox(height: 80),
                          
                          // مؤشر التحميل المخصص
                          _buildLoadingIndicator(),
                        ],
                      ),
                    ),
                  );
                },
              ),
            ),
            
            // النص السفلي
            Positioned(
              bottom: 60,
              left: 0,
              right: 0,
              child: AnimatedBuilder(
                animation: _fadeAnimation,
                builder: (context, child) {
                  return FadeTransition(
                    opacity: _fadeAnimation,
                    child: Text(
                      'تجربة حجز استثنائية',
                      style: AppTextStyles.bodySmall.copyWith(
                        color: AppColors.primary.withValues(alpha: 0.4),
                        letterSpacing: 1.2,
                      ),
                      textAlign: TextAlign.center,
                    ),
                  );
                },
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildLogo() {
    return AnimatedBuilder(
      animation: _shimmerAnimation,
      builder: (context, child) {
        return Container(
          width: 100,
          height: 100,
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
              colors: [
                AppColors.primary,
                AppColors.primary.withValues(alpha: _shimmerAnimation.value),
              ],
            ),
            shape: BoxShape.circle,
            boxShadow: [
              BoxShadow(
                color: AppColors.primary.withValues(alpha: 0.15),
                blurRadius: 30,
                offset: const Offset(0, 15),
              ),
            ],
          ),
          child: Stack(
            alignment: Alignment.center,
            children: [
              // حلقة خارجية
              Container(
                width: 100,
                height: 100,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  border: Border.all(
                    color: Colors.white.withValues(alpha: 0.2),
                    width: 1,
                  ),
                ),
              ),
              // الأيقونة
              const Icon(
                Icons.apartment_rounded,
                size: 45,
                color: Colors.white,
              ),
            ],
          ),
        );
      },
    );
  }

  Widget _buildAppName() {
    return Column(
      children: [
        Text(
          'حجوزات اليمن',
          style: AppTextStyles.displayLarge.copyWith(
            color: AppColors.primary,
            fontWeight: FontWeight.w600,
            fontSize: 28,
            letterSpacing: 0.5,
          ),
        ),
        const SizedBox(height: 8),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
          decoration: BoxDecoration(
            border: Border.all(
              color: AppColors.primary.withValues(alpha: 0.2),
              width: 1,
            ),
            borderRadius: BorderRadius.circular(20),
          ),
          child: Text(
            'YEMEN BOOKING',
            style: AppTextStyles.bodySmall.copyWith(
              color: AppColors.primary.withValues(alpha: 0.6),
              fontSize: 11,
              letterSpacing: 2,
              fontWeight: FontWeight.w500,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildLoadingIndicator() {
    return Column(
      children: [
        // مؤشر تحميل مخصص
        SizedBox(
          width: 50,
          height: 2,
          child: LinearProgressIndicator(
            backgroundColor: AppColors.primary.withValues(alpha: 0.1),
            valueColor: AlwaysStoppedAnimation<Color>(
              AppColors.primary.withValues(alpha: 0.6),
            ),
          ),
        ),
        const SizedBox(height: 16),
        AnimatedBuilder(
          animation: _shimmerAnimation,
          builder: (context, child) {
            return Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: List.generate(3, (index) {
                return Container(
                  margin: const EdgeInsets.symmetric(horizontal: 3),
                  width: 6,
                  height: 6,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    color: AppColors.primary.withValues(alpha: 
                      index == 0 ? _shimmerAnimation.value
                        : index == 1 ? (1 - _shimmerAnimation.value)
                        : 0.3,
                    ),
                  ),
                );
              }),
            );
          },
        ),
      ],
    );
  }
}

// رسام النمط الهندسي للخلفية
class _BackgroundPatternPainter extends CustomPainter {
  final Color color;

  _BackgroundPatternPainter({required this.color});

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = color
      ..style = PaintingStyle.stroke
      ..strokeWidth = 0.5;

    // رسم خطوط هندسية خفيفة
    const spacing = 50.0;
    
    // خطوط عمودية
    for (double x = 0; x < size.width; x += spacing) {
      canvas.drawLine(
        Offset(x, 0),
        Offset(x, size.height),
        paint,
      );
    }
    
    // خطوط أفقية
    for (double y = 0; y < size.height; y += spacing) {
      canvas.drawLine(
        Offset(0, y),
        Offset(size.width, y),
        paint,
      );
    }

    // دوائر زخرفية في الزوايا
    final circlePaint = Paint()
      ..color = color.withValues(alpha: 0.5)
      ..style = PaintingStyle.stroke
      ..strokeWidth = 0.5;

    canvas.drawCircle(
      Offset(size.width * 0.1, size.height * 0.15),
      80,
      circlePaint,
    );
    
    canvas.drawCircle(
      Offset(size.width * 0.9, size.height * 0.85),
      100,
      circlePaint,
    );
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
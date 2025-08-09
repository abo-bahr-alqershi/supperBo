import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:package_info_plus/package_info_plus.dart';
import '../../../../core/constants/app_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class AboutPage extends StatefulWidget {
  const AboutPage({super.key});

  @override
  State<AboutPage> createState() => _AboutPageState();
}

class _AboutPageState extends State<AboutPage> with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _fadeAnimation;
  PackageInfo? _packageInfo;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 800),
      vsync: this,
    );
    _fadeAnimation = CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeInOut,
    );
    _loadPackageInfo();
    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  Future<void> _loadPackageInfo() async {
    final info = await PackageInfo.fromPlatform();
    setState(() {
      _packageInfo = info;
    });
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final isDark = theme.brightness == Brightness.dark;

    return Scaffold(
      backgroundColor: theme.scaffoldBackgroundColor,
      appBar: AppBar(
        title: const Text('حول التطبيق'),
        centerTitle: true,
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_rounded),
          onPressed: () => context.pop(),
        ),
      ),
      body: FadeTransition(
        opacity: _fadeAnimation,
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(AppDimensions.paddingMedium),
          child: Column(
            children: [
              const SizedBox(height: AppDimensions.spacingXl),
              
              // App Logo and Name
              _buildAppHeader(isDark),
              
              const SizedBox(height: AppDimensions.spacingXl),
              
              // App Description
              _buildDescription(),
              
              const SizedBox(height: AppDimensions.spacingLg),
              
              // Features Section
              _buildFeaturesSection(),
              
              const SizedBox(height: AppDimensions.spacingLg),
              
              // Contact Section
              _buildContactSection(),
              
              const SizedBox(height: AppDimensions.spacingLg),
              
              // Social Media Section
              _buildSocialMediaSection(),
              
              const SizedBox(height: AppDimensions.spacingLg),
              
              // Legal Section
              _buildLegalSection(),
              
              const SizedBox(height: AppDimensions.spacingXl),
              
              // Version Info
              _buildVersionInfo(),
              
              const SizedBox(height: AppDimensions.spacingXl),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildAppHeader(bool isDark) {
    return Column(
      children: [
        Container(
          width: 120,
          height: 120,
          decoration: BoxDecoration(
            gradient: const LinearGradient(
              colors: [
                AppColors.primary,
                AppColors.primaryLight,
              ],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusXl),
            boxShadow: [
              BoxShadow(
                color: AppColors.primary.withValues(alpha: 0.3),
                blurRadius: 20,
                offset: const Offset(0, 10),
              ),
            ],
          ),
          child: const Center(
            child: Icon(
              Icons.hotel_rounded,
              size: 60,
              color: AppColors.white,
            ),
          ),
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        Text(
          AppConstants.appName,
          style: AppTextStyles.heading1.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingXs),
        Text(
          'منصتك الموثوقة للحجوزات في اليمن',
          style: AppTextStyles.bodyMedium.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
      ],
    );
  }

  Widget _buildDescription() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        border: Border.all(
          color: AppColors.divider,
          width: 1,
        ),
      ),
      child: Text(
        '''تطبيق يمن بوكينج هو المنصة الرائدة للحجوزات الفندقية والسياحية في اليمن. نسعى لتوفير تجربة حجز سهلة وموثوقة، مع مجموعة واسعة من الخيارات التي تناسب جميع الاحتياجات والميزانيات.

نعمل مع أفضل الفنادق والمنشآت السياحية في جميع أنحاء اليمن لضمان حصولك على أفضل الأسعار والخدمات. سواء كنت تبحث عن إقامة فاخرة أو خيارات اقتصادية، ستجد ما يناسبك معنا.''',
        style: AppTextStyles.bodyMedium.copyWith(
          height: 1.6,
        ),
        textAlign: TextAlign.justify,
      ),
    );
  }

  Widget _buildFeaturesSection() {
    final features = [
      {
        'icon': Icons.search_rounded,
        'title': 'بحث متقدم',
        'description': 'ابحث بسهولة عن الفنادق والوحدات المناسبة',
      },
      {
        'icon': Icons.security_rounded,
        'title': 'حجز آمن',
        'description': 'نظام دفع آمن ومشفر لحماية معلوماتك',
      },
      {
        'icon': Icons.support_agent_rounded,
        'title': 'دعم 24/7',
        'description': 'فريق دعم متخصص لمساعدتك في أي وقت',
      },
      {
        'icon': Icons.star_rounded,
        'title': 'تقييمات موثوقة',
        'description': 'تقييمات حقيقية من نزلاء سابقين',
      },
    ];

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'المميزات الرئيسية',
          style: AppTextStyles.subtitle1.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        ...features.map((feature) => _buildFeatureItem(
          icon: feature['icon'] as IconData,
          title: feature['title'] as String,
          description: feature['description'] as String,
        )),
      ],
    );
  }

  Widget _buildFeatureItem({
    required IconData icon,
    required String title,
    required String description,
  }) {
    return Padding(
      padding: const EdgeInsets.only(bottom: AppDimensions.spacingMd),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            width: 48,
            height: 48,
            decoration: BoxDecoration(
              color: AppColors.primary.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
            ),
            child: Icon(
              icon,
              color: AppColors.primary,
              size: 24,
            ),
          ),
          const SizedBox(width: AppDimensions.spacingMd),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: AppTextStyles.bodyLarge.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: AppDimensions.spacingXs),
                Text(
                  description,
                  style: AppTextStyles.bodySmall.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildContactSection() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        border: Border.all(
          color: AppColors.divider,
          width: 1,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'تواصل معنا',
            style: AppTextStyles.subtitle1.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildContactItem(
            icon: Icons.email_rounded,
            label: 'البريد الإلكتروني',
            value: 'support@yemenbooking.com',
            onTap: () => _launchUrl('mailto:support@yemenbooking.com'),
          ),
          _buildContactItem(
            icon: Icons.phone_rounded,
            label: 'الهاتف',
            value: '+967 777 123 456',
            onTap: () => _launchUrl('tel:+967777123456'),
          ),
          _buildContactItem(
            icon: Icons.language_rounded,
            label: 'الموقع الإلكتروني',
            value: 'www.yemenbooking.com',
            onTap: () => _launchUrl('https://www.yemenbooking.com'),
          ),
        ],
      ),
    );
  }

  Widget _buildContactItem({
    required IconData icon,
    required String label,
    required String value,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
      child: Padding(
        padding: const EdgeInsets.symmetric(
          vertical: AppDimensions.paddingSmall,
        ),
        child: Row(
          children: [
            Icon(
              icon,
              size: 20,
              color: AppColors.textSecondary,
            ),
            const SizedBox(width: AppDimensions.spacingMd),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    label,
                    style: AppTextStyles.caption.copyWith(
                      color: AppColors.textSecondary,
                    ),
                  ),
                  Text(
                    value,
                    style: AppTextStyles.bodyMedium.copyWith(
                      color: AppColors.primary,
                    ),
                  ),
                ],
              ),
            ),
            const Icon(
              Icons.arrow_forward_ios_rounded,
              size: 16,
              color: AppColors.textSecondary,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSocialMediaSection() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'تابعنا على',
          style: AppTextStyles.subtitle1.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            _buildSocialButton(
              icon: Icons.facebook,
              color: const Color(0xFF1877F2),
              onTap: () => _launchUrl('https://facebook.com/yemenbooking'),
            ),
            const SizedBox(width: AppDimensions.spacingMd),
            _buildSocialButton(
              icon: Icons.telegram,
              color: const Color(0xFF0088CC),
              onTap: () => _launchUrl('https://t.me/yemenbooking'),
            ),
            const SizedBox(width: AppDimensions.spacingMd),
            _buildSocialButton(
              icon: Icons.alternate_email,
              color: const Color(0xFF000000),
              onTap: () => _launchUrl('https://twitter.com/yemenbooking'),
            ),
            const SizedBox(width: AppDimensions.spacingMd),
            _buildSocialButton(
              icon: Icons.camera_alt,
              color: const Color(0xFFE4405F),
              onTap: () => _launchUrl('https://instagram.com/yemenbooking'),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildSocialButton({
    required IconData icon,
    required Color color,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
      child: Container(
        width: 56,
        height: 56,
        decoration: BoxDecoration(
          color: color.withValues(alpha: 0.1),
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        ),
        child: Icon(
          icon,
          color: color,
          size: 28,
        ),
      ),
    );
  }

  Widget _buildLegalSection() {
    return Column(
      children: [
        ListTile(
          leading: const Icon(Icons.description_outlined),
          title: const Text('الشروط والأحكام'),
          trailing: const Icon(Icons.arrow_forward_ios_rounded, size: 16),
          onTap: () {
            // Navigate to terms page
          },
        ),
        ListTile(
          leading: const Icon(Icons.privacy_tip_outlined),
          title: const Text('سياسة الخصوصية'),
          trailing: const Icon(Icons.arrow_forward_ios_rounded, size: 16),
          onTap: () => context.push('/settings/privacy-policy'),
        ),
        ListTile(
          leading: const Icon(Icons.copyright_outlined),
          title: const Text('حقوق الطبع والنشر'),
          trailing: const Icon(Icons.arrow_forward_ios_rounded, size: 16),
          onTap: () {
            showDialog(
              context: context,
              builder: (context) => AlertDialog(
                title: const Text('حقوق الطبع والنشر'),
                content: const Text(
                  'جميع الحقوق محفوظة © 2024 يمن بوكينج',
                ),
                actions: [
                  TextButton(
                    onPressed: () => Navigator.pop(context),
                    child: const Text('حسناً'),
                  ),
                ],
              ),
            );
          },
        ),
      ],
    );
  }

  Widget _buildVersionInfo() {
    return Column(
      children: [
        Text(
          'الإصدار',
          style: AppTextStyles.caption.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingXs),
        Text(
          '${_packageInfo?.version ?? AppConstants.appVersion} (${_packageInfo?.buildNumber ?? AppConstants.appBuildNumber})',
          style: AppTextStyles.bodyMedium.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: AppDimensions.spacingMd),
        Text(
          'صُنع بـ ❤️ في اليمن',
          style: AppTextStyles.bodySmall.copyWith(
            color: AppColors.textSecondary,
          ),
        ),
      ],
    );
  }

  Future<void> _launchUrl(String url) async {
    final uri = Uri.parse(url);
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri, mode: LaunchMode.externalApplication);
    } else {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('لا يمكن فتح الرابط: $url'),
            backgroundColor: AppColors.error,
          ),
        );
      }
    }
  }
}
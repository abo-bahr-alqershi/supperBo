import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:yemen_booking_app/features/settings/domain/entities/app_settings.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/error_widget.dart';
import '../bloc/settings_bloc.dart';
import '../bloc/settings_event.dart';
import '../bloc/settings_state.dart';
import '../widgets/settings_item_widget.dart';
import '../widgets/theme_selector_widget.dart';

class SettingsPage extends StatelessWidget {
  const SettingsPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Theme.of(context).brightness == Brightness.dark
          ? AppColors.backgroundDark
          : AppColors.background,
      body: CustomScrollView(
        slivers: [
          _buildAppBar(context),
          SliverToBoxAdapter(
            child: BlocBuilder<SettingsBloc, SettingsState>(
              builder: (context, state) {
                if (state is SettingsLoading) {
                  return const SizedBox(
                    height: 400,
                    child: LoadingWidget(
                      type: LoadingType.circular,
                    ),
                  );
                }

                if (state is SettingsError) {
                  return SizedBox(
                    height: 400,
                    child: CustomErrorWidget(
                      message: state.message,
                      onRetry: () {
                        context.read<SettingsBloc>().add(LoadSettingsEvent());
                      },
                    ),
                  );
                }

                if (state is SettingsLoaded ||
                    state is SettingsUpdated ||
                    state is SettingsUpdating) {
                  final settings = (state is SettingsLoaded)
                      ? state.settings
                      : (state is SettingsUpdated)
                          ? state.settings
                          : (state as SettingsUpdating).currentSettings;

                  return _buildSettingsContent(context, settings);
                }

                return const SizedBox.shrink();
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAppBar(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    
    return SliverAppBar(
      expandedHeight: 120,
      floating: false,
      pinned: true,
      backgroundColor: isDark ? AppColors.surfaceDark : AppColors.white,
      flexibleSpace: FlexibleSpaceBar(
        title: Text(
          'الإعدادات',
          style: AppTextStyles.heading3.copyWith(
            color: isDark ? AppColors.textPrimaryDark : AppColors.textPrimary,
          ),
        ),
        centerTitle: true,
        background: Container(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: Alignment.topCenter,
              end: Alignment.bottomCenter,
              colors: [
                AppColors.primary.withValues(alpha: 0.1),
                (isDark ? AppColors.surfaceDark : AppColors.white).withValues(alpha: 0.95),
              ],
            ),
          ),
        ),
      ),
      leading: IconButton(
        icon: Icon(
          Icons.arrow_back_ios_rounded,
          color: isDark ? AppColors.textPrimaryDark : AppColors.textPrimary,
        ),
        onPressed: () => context.pop(),
      ),
    );
  }

  Widget _buildSettingsContent(BuildContext context, AppSettings settings) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    
    return Column(
      children: [
        // Account Section
        _buildSectionHeader('الحساب', isDark),
        Container(
          margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
          decoration: BoxDecoration(
            color: isDark ? AppColors.surfaceDark : AppColors.white,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
            boxShadow: [
              BoxShadow(
                color: AppColors.shadow.withValues(alpha: 0.05),
                blurRadius: 10,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: Column(
            children: [
              SettingsItemWidget(
                icon: Icons.person_outline_rounded,
                title: 'الملف الشخصي',
                subtitle: 'تعديل معلوماتك الشخصية',
                onTap: () => context.push('/profile'),
                showArrow: true,
              ),
              _buildDivider(),
              SettingsItemWidget(
                icon: Icons.lock_outline_rounded,
                title: 'كلمة المرور',
                subtitle: 'تغيير كلمة المرور',
                onTap: () => context.push('/profile/change-password'),
                showArrow: true,
              ),
            ],
          ),
        ),

        const SizedBox(height: AppDimensions.spacingLg),

        // Preferences Section
        _buildSectionHeader('التفضيلات', isDark),
        Container(
          margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
          decoration: BoxDecoration(
            color: isDark ? AppColors.surfaceDark : AppColors.white,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
            boxShadow: [
              BoxShadow(
                color: AppColors.shadow.withValues(alpha: 0.05),
                blurRadius: 10,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: Column(
            children: [
              SettingsItemWidget(
                icon: Icons.language_rounded,
                title: 'اللغة',
                subtitle: settings.preferredLanguage == 'ar' ? 'العربية' : 'English',
                onTap: () => context.push('/settings/language'),
                showArrow: true,
              ),
              _buildDivider(),
              SettingsItemWidget(
                icon: Icons.dark_mode_outlined,
                title: 'المظهر',
                subtitle: settings.darkMode ? 'الوضع الليلي' : 'الوضع النهاري',
                trailing: ThemeSelectorWidget(
                  isDarkMode: settings.darkMode,
                  onChanged: (isDark) {
                    context.read<SettingsBloc>().add(UpdateThemeEvent(isDark));
                  },
                ),
              ),
              _buildDivider(),
              SettingsItemWidget(
                icon: Icons.attach_money_rounded,
                title: 'العملة',
                subtitle: _getCurrencyName(settings.preferredCurrency),
                onTap: () => _showCurrencySelector(context, settings.preferredCurrency),
                showArrow: true,
              ),
              _buildDivider(),
              SettingsItemWidget(
                icon: Icons.notifications_outlined,
                title: 'الإشعارات',
                subtitle: 'إدارة إعدادات الإشعارات',
                onTap: () => context.push('/notifications/settings'),
                showArrow: true,
              ),
            ],
          ),
        ),

        const SizedBox(height: AppDimensions.spacingLg),

        // Support Section
        _buildSectionHeader('الدعم والمساعدة', isDark),
        Container(
          margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
          decoration: BoxDecoration(
            color: isDark ? AppColors.surfaceDark : AppColors.white,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
            boxShadow: [
              BoxShadow(
                color: AppColors.shadow.withValues(alpha: 0.05),
                blurRadius: 10,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: Column(
            children: [
              SettingsItemWidget(
                icon: Icons.help_outline_rounded,
                title: 'مركز المساعدة',
                subtitle: 'الأسئلة الشائعة والدعم',
                onTap: () => context.push('/help'),
                showArrow: true,
              ),
              _buildDivider(),
              SettingsItemWidget(
                icon: Icons.privacy_tip_outlined,
                title: 'سياسة الخصوصية',
                subtitle: 'اقرأ سياسة الخصوصية',
                onTap: () => context.push('/settings/privacy-policy'),
                showArrow: true,
              ),
              _buildDivider(),
              SettingsItemWidget(
                icon: Icons.description_outlined,
                title: 'الشروط والأحكام',
                subtitle: 'اقرأ الشروط والأحكام',
                onTap: () => context.push('/terms'),
                showArrow: true,
              ),
            ],
          ),
        ),

        const SizedBox(height: AppDimensions.spacingLg),

        // About Section
        _buildSectionHeader('حول', isDark),
        Container(
          margin: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
          decoration: BoxDecoration(
            color: isDark ? AppColors.surfaceDark : AppColors.white,
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
            boxShadow: [
              BoxShadow(
                color: AppColors.shadow.withValues(alpha: 0.05),
                blurRadius: 10,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: Column(
            children: [
              SettingsItemWidget(
                icon: Icons.info_outline_rounded,
                title: 'عن التطبيق',
                subtitle: 'معلومات حول التطبيق',
                onTap: () => context.push('/settings/about'),
                showArrow: true,
              ),
              _buildDivider(),
              SettingsItemWidget(
                icon: Icons.star_outline_rounded,
                title: 'قيم التطبيق',
                subtitle: 'شاركنا رأيك',
                onTap: () => _rateApp(),
                showArrow: true,
              ),
              _buildDivider(),
              SettingsItemWidget(
                icon: Icons.share_outlined,
                title: 'شارك التطبيق',
                subtitle: 'شارك التطبيق مع أصدقائك',
                onTap: () => _shareApp(),
                showArrow: true,
              ),
            ],
          ),
        ),

        const SizedBox(height: AppDimensions.spacingLg),

        // Logout Button
        Container(
          margin: const EdgeInsets.all(AppDimensions.paddingMedium),
          child: OutlinedButton.icon(
            onPressed: () => _showLogoutDialog(context),
            icon: const Icon(Icons.logout_rounded, color: AppColors.error),
            label: Text(
              'تسجيل الخروج',
              style: AppTextStyles.button.copyWith(color: AppColors.error),
            ),
            style: OutlinedButton.styleFrom(
              side: const BorderSide(color: AppColors.error),
              padding: const EdgeInsets.symmetric(
                horizontal: AppDimensions.paddingLarge,
                vertical: AppDimensions.paddingMedium,
              ),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
              ),
              minimumSize: const Size(double.infinity, 48),
            ),
          ),
        ),

        const SizedBox(height: AppDimensions.spacingXl),
      ],
    );
  }

  Widget _buildSectionHeader(String title, bool isDark) {
    return Container(
      alignment: AlignmentDirectional.centerStart,
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingLarge,
        vertical: AppDimensions.paddingSmall,
      ),
      child: Text(
        title,
        style: AppTextStyles.bodyMedium.copyWith(
          color: isDark ? AppColors.textSecondaryDark : AppColors.textSecondary,
          fontWeight: FontWeight.w600,
        ),
      ),
    );
  }

  Widget _buildDivider() {
    return const Divider(
      height: 1,
      thickness: 0.5,
      indent: AppDimensions.paddingLarge + AppDimensions.iconMedium + AppDimensions.paddingMedium,
      endIndent: AppDimensions.paddingMedium,
      color: AppColors.divider,
    );
  }

  String _getCurrencyName(String code) {
    switch (code) {
      case 'YER':
        return 'الريال اليمني';
      case 'USD':
        return 'الدولار الأمريكي';
      case 'SAR':
        return 'الريال السعودي';
      case 'EUR':
        return 'اليورو';
      default:
        return code;
    }
  }

  void _showCurrencySelector(BuildContext context, String currentCurrency) {
    showModalBottomSheet(
      context: context,
      backgroundColor: Theme.of(context).brightness == Brightness.dark
          ? AppColors.surfaceDark
          : AppColors.white,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(
          top: Radius.circular(AppDimensions.borderRadiusXl),
        ),
      ),
      builder: (bottomSheetContext) {
        return Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Container(
              margin: const EdgeInsets.only(top: AppDimensions.paddingSmall),
              width: 40,
              height: 4,
              decoration: BoxDecoration(
                color: AppColors.divider,
                borderRadius: BorderRadius.circular(2),
              ),
            ),
            const Padding(
              padding: EdgeInsets.all(AppDimensions.paddingLarge),
              child: Text(
                'اختر العملة',
                style: AppTextStyles.heading3,
              ),
            ),
            ...[
              {'code': 'YER', 'name': 'الريال اليمني', 'symbol': '﷼'},
              {'code': 'USD', 'name': 'الدولار الأمريكي', 'symbol': '\$'},
              {'code': 'SAR', 'name': 'الريال السعودي', 'symbol': 'ر.س'},
              {'code': 'EUR', 'name': 'اليورو', 'symbol': '€'},
            ].map((currency) {
              return ListTile(
                leading: CircleAvatar(
                  backgroundColor: AppColors.primary.withValues(alpha: 0.1),
                  child: Text(
                    currency['symbol']!,
                    style: AppTextStyles.button.copyWith(
                      color: AppColors.primary,
                    ),
                  ),
                ),
                title: Text(currency['name']!),
                trailing: currentCurrency == currency['code']
                    ? const Icon(Icons.check_circle, color: AppColors.primary)
                    : null,
                onTap: () {
                  context.read<SettingsBloc>().add(
                    UpdateCurrencyEvent(currency['code']!),
                  );
                  Navigator.pop(bottomSheetContext);
                },
              );
            }),
            const SizedBox(height: AppDimensions.paddingLarge),
          ],
        );
      },
    );
  }

  void _showLogoutDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (dialogContext) {
        return AlertDialog(
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
          ),
          title: const Text('تسجيل الخروج'),
          content: const Text('هل أنت متأكد من رغبتك في تسجيل الخروج؟'),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(dialogContext),
              child: const Text('إلغاء'),
            ),
            ElevatedButton(
              onPressed: () {
                Navigator.pop(dialogContext);
                // TODO: Implement logout
                context.go('/login');
              },
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.error,
              ),
              child: const Text('تسجيل الخروج'),
            ),
          ],
        );
      },
    );
  }

  void _rateApp() {
    // TODO: Implement app rating
  }

  void _shareApp() {
    // TODO: Implement app sharing
  }
}
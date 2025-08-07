import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../bloc/settings_bloc.dart';
import '../bloc/settings_event.dart';

class PrivacyPolicyPage extends StatefulWidget {
  final bool isFirstTime;

  const PrivacyPolicyPage({
    super.key,
    this.isFirstTime = false,
  });

  @override
  State<PrivacyPolicyPage> createState() => _PrivacyPolicyPageState();
}

class _PrivacyPolicyPageState extends State<PrivacyPolicyPage> {
  bool _acceptTerms = false;
  final ScrollController _scrollController = ScrollController();

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final isDark = Theme.of(context).brightness == Brightness.dark;
    
    return Scaffold(
      backgroundColor: isDark ? AppColors.backgroundDark : AppColors.background,
      appBar: AppBar(
        backgroundColor: isDark ? AppColors.surfaceDark : AppColors.white,
        elevation: 0,
        title: Text(
          'سياسة الخصوصية',
          style: AppTextStyles.heading3.copyWith(
            color: isDark ? AppColors.textPrimaryDark : AppColors.textPrimary,
          ),
        ),
        leading: widget.isFirstTime
            ? null
            : IconButton(
                icon: Icon(
                  Icons.arrow_back_ios_rounded,
                  color: isDark ? AppColors.textPrimaryDark : AppColors.textPrimary,
                ),
                onPressed: () => context.pop(),
              ),
        actions: [
          if (!widget.isFirstTime)
            IconButton(
              icon: Icon(
                Icons.share_outlined,
                color: isDark ? AppColors.textPrimaryDark : AppColors.textPrimary,
              ),
              onPressed: _sharePrivacyPolicy,
            ),
        ],
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(1),
          child: Container(
            height: 1,
            color: AppColors.divider,
          ),
        ),
      ),
      body: Column(
        children: [
          // Last Updated Banner
          Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingMedium,
              vertical: AppDimensions.paddingSmall,
            ),
            color: AppColors.info.withOpacity(0.1),
            child: Row(
              children: [
                Icon(
                  Icons.info_outline_rounded,
                  size: 16,
                  color: AppColors.info,
                ),
                const SizedBox(width: AppDimensions.spacingXs),
                Text(
                  'آخر تحديث: يناير 2025',
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.info,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ],
            ),
          ),
          
          // Content
          Expanded(
            child: SingleChildScrollView(
              controller: _scrollController,
              padding: const EdgeInsets.all(AppDimensions.paddingLarge),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  _buildSection(
                    context: context,
                    title: '1. مقدمة',
                    content: '''نحن في تطبيق حجوزات اليمن نحترم خصوصيتك ونلتزم بحماية معلوماتك الشخصية. توضح هذه السياسة كيفية جمع واستخدام وحماية المعلومات التي نحصل عليها منك.

هذه السياسة تنطبق على جميع المستخدمين لتطبيقنا وخدماتنا. باستخدامك للتطبيق، فإنك توافق على ممارساتنا الموضحة في هذه السياسة.''',
                    isDark: isDark,
                    isExpanded: true,
                  ),
                  
                  const SizedBox(height: AppDimensions.spacingMd),
                  
                  _buildSection(
                    context: context,
                    title: '2. المعلومات التي نجمعها',
                    content: '''نقوم بجمع المعلومات التالية:

• المعلومات الشخصية: الاسم الكامل، البريد الإلكتروني، رقم الهاتف، تاريخ الميلاد
• معلومات الحساب: اسم المستخدم، كلمة المرور المشفرة، صورة الملف الشخصي
• معلومات الحجز: التواريخ، الوجهات، تفضيلات السكن، عدد الضيوف
• معلومات الدفع: تفاصيل البطاقة الائتمانية (مشفرة بالكامل)، سجل المعاملات
• معلومات الجهاز: نوع الجهاز، نظام التشغيل، معرف الجهاز، عنوان IP
• معلومات الاستخدام: الصفحات المزارة، الوقت المستغرق، التفاعلات''',
                    isDark: isDark,
                  ),
                  
                  const SizedBox(height: AppDimensions.spacingMd),
                  
                  _buildSection(
                    context: context,
                    title: '3. كيف نستخدم معلوماتك',
                    content: '''نستخدم المعلومات المجمعة للأغراض التالية:

• معالجة الحجوزات وإتمام المعاملات
• التواصل معك بخصوص حجوزاتك وخدماتنا
• تحسين خدماتنا وتجربة المستخدم
• تخصيص المحتوى والعروض حسب اهتماماتك
• إرسال التحديثات والعروض الترويجية (بموافقتك)
• ضمان الأمان ومنع الاحتيال
• تحليل استخدام التطبيق لتحسين الأداء
• الامتثال للمتطلبات القانونية والتنظيمية''',
                    isDark: isDark,
                  ),
                  
                  const SizedBox(height: AppDimensions.spacingMd),
                  
                  _buildSection(
                    context: context,
                    title: '4. مشاركة المعلومات',
                    content: '''لا نبيع أو نؤجر معلوماتك الشخصية لأطراف ثالثة. قد نشارك معلوماتك فقط في الحالات التالية:

• مع مقدمي الخدمات: الفنادق والمنشآت السياحية لإتمام الحجوزات
• مع شركاء الدفع: لمعالجة المعاملات المالية بشكل آمن
• للامتثال القانوني: عند الضرورة القانونية أو بأمر من المحكمة
• لحماية الحقوق: لحماية حقوقنا وحقوق المستخدمين الآخرين
• بموافقتك الصريحة: عندما توافق على مشاركة معلوماتك''',
                    isDark: isDark,
                  ),
                  
                  const SizedBox(height: AppDimensions.spacingMd),
                  
                  _buildSection(
                    context: context,
                    title: '5. حماية المعلومات',
                    content: '''نحن نتخذ إجراءات أمنية مناسبة لحماية معلوماتك الشخصية:

• التشفير: نستخدم تشفير SSL/TLS لحماية البيانات المنقولة
• التحكم في الوصول: نقيد الوصول للموظفين المصرح لهم فقط
• المراقبة: نراقب أنظمتنا على مدار الساعة للكشف عن أي اختراقات
• التحديثات: نحدث إجراءاتنا الأمنية بانتظام
• النسخ الاحتياطي: نحتفظ بنسخ احتياطية آمنة لبياناتك
• المصادقة الثنائية: نوفر خيار المصادقة الثنائية لحسابك''',
                    isDark: isDark,
                  ),
                  
                  const SizedBox(height: AppDimensions.spacingMd),
                  
                  _buildSection(
                    context: context,
                    title: '6. ملفات تعريف الارتباط',
                    content: '''نستخدم ملفات تعريف الارتباط وتقنيات التتبع المماثلة:

• الملفات الأساسية: ضرورية لعمل التطبيق بشكل صحيح
• ملفات الأداء: تساعدنا في فهم كيفية استخدام التطبيق
• ملفات التفضيلات: تحفظ إعداداتك وتفضيلاتك الشخصية
• ملفات التسويق: تستخدم لعرض إعلانات مخصصة (بموافقتك)

يمكنك إدارة تفضيلات ملفات تعريف الارتباط من إعدادات التطبيق أو المتصفح.''',
                    isDark: isDark,
                  ),
                  
                  const SizedBox(height: AppDimensions.spacingMd),
                  
                  _buildSection(
                    context: context,
                    title: '7. حقوقك',
                    content: '''لديك الحقوق التالية فيما يتعلق بمعلوماتك الشخصية:

• حق الوصول: طلب نسخة من المعلومات التي نحتفظ بها عنك
• حق التصحيح: تصحيح أي معلومات غير دقيقة أو قديمة
• حق الحذف: طلب حذف معلوماتك الشخصية
• حق التقييد: تقييد معالجة معلوماتك في حالات معينة
• حق النقل: نقل بياناتك إلى مزود خدمة آخر
• حق الاعتراض: الاعتراض على معالجة معلوماتك للتسويق المباشر
• حق سحب الموافقة: سحب موافقتك في أي وقت

للممارسة أي من هذه الحقوق، يرجى التواصل معنا عبر: privacy@yemenbooking.com''',
                    isDark: isDark,
                  ),
                  
                  const SizedBox(height: AppDimensions.spacingMd),
                  
                  _buildSection(
                    context: context,
                    title: '8. الأطفال',
                    content: '''تطبيقنا غير مخصص للأطفال دون سن 18 عامًا. نحن لا نجمع عن قصد معلومات شخصية من الأطفال. إذا علمنا أننا جمعنا معلومات من طفل دون السن القانونية، سنتخذ خطوات فورية لحذف هذه المعلومات.

إذا كنت والدًا أو وصيًا وتعتقد أن طفلك قد زودنا بمعلومات شخصية، يرجى الاتصال بنا فورًا.''',
                    isDark: isDark,
                  ),
                  
                  const SizedBox(height: AppDimensions.spacingMd),
                  
                  _buildSection(
                    context: context,
                    title: '9. التغييرات على السياسة',
                    content: '''قد نقوم بتحديث سياسة الخصوصية من وقت لآخر. عند إجراء تغييرات جوهرية:

• سنخطرك عبر البريد الإلكتروني أو إشعار داخل التطبيق
• سنعرض إشعارًا بارزًا في التطبيق قبل سريان التغييرات
• سنحدث تاريخ "آخر تحديث" في أعلى هذه السياسة

نوصيك بمراجعة هذه السياسة بشكل دوري. استمرارك في استخدام التطبيق بعد التغييرات يعني موافقتك عليها.''',
                    isDark: isDark,
                  ),
                  
                  const SizedBox(height: AppDimensions.spacingMd),
                  
                  _buildSection(
                    context: context,
                    title: '10. الاتصال بنا',
                    content: '''إذا كان لديك أي أسئلة أو استفسارات حول سياسة الخصوصية:

📧 البريد الإلكتروني: privacy@yemenbooking.com
📱 الهاتف: +967 777 123 456
📍 العنوان: صنعاء، شارع الستين، اليمن

ساعات العمل:
الأحد - الخميس: 9:00 ص - 6:00 م
الجمعة - السبت: 10:00 ص - 4:00 م

نلتزم بالرد على استفساراتك خلال 48 ساعة عمل.''',
                    isDark: isDark,
                  ),
                  
                  const SizedBox(height: AppDimensions.spacingXl),
                  
                  // Accept Terms Section (for first time users)
                  if (widget.isFirstTime) ...[
                    Container(
                      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
                      decoration: BoxDecoration(
                        color: AppColors.primary.withOpacity(0.05),
                        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
                        border: Border.all(
                          color: AppColors.primary.withOpacity(0.2),
                        ),
                      ),
                      child: Column(
                        children: [
                          Row(
                            children: [
                              Checkbox(
                                value: _acceptTerms,
                                onChanged: (value) {
                                  setState(() {
                                    _acceptTerms = value ?? false;
                                  });
                                },
                                activeColor: AppColors.primary,
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(4),
                                ),
                              ),
                              Expanded(
                                child: Text(
                                  'لقد قرأت وأوافق على سياسة الخصوصية والشروط والأحكام',
                                  style: AppTextStyles.bodyMedium.copyWith(
                                    color: isDark ? AppColors.textPrimaryDark : AppColors.textPrimary,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: AppDimensions.spacingMd),
                          SizedBox(
                            width: double.infinity,
                            child: ElevatedButton(
                              onPressed: _acceptTerms
                                  ? () {
                                      // Save acceptance
                                      context.read<SettingsBloc>().add(
                                        const AcceptPrivacyPolicyEvent(),
                                      );
                                      // Navigate to main app
                                      context.go('/home');
                                    }
                                  : null,
                              style: ElevatedButton.styleFrom(
                                backgroundColor: AppColors.primary,
                                padding: const EdgeInsets.symmetric(
                                  vertical: AppDimensions.paddingMedium,
                                ),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(
                                    AppDimensions.borderRadiusMd,
                                  ),
                                ),
                              ),
                              child: Text(
                                'موافق ومتابعة',
                                style: AppTextStyles.button.copyWith(
                                  color: AppColors.white,
                                ),
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: AppDimensions.spacingXl),
                  ],
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildSection({
    required BuildContext context,
    required String title,
    required String content,
    required bool isDark,
    bool isExpanded = false,
  }) {
    if (isExpanded) {
      // Always expanded section
      return Container(
        decoration: BoxDecoration(
          color: isDark ? AppColors.surfaceDark : AppColors.white,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
          border: Border.all(
            color: AppColors.divider,
            width: 1,
          ),
        ),
        padding: const EdgeInsets.all(AppDimensions.paddingMedium),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Container(
                  width: 4,
                  height: 20,
                  decoration: BoxDecoration(
                    color: AppColors.primary,
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
                const SizedBox(width: AppDimensions.spacingSm),
                Expanded(
                  child: Text(
                    title,
                    style: AppTextStyles.subtitle1.copyWith(
                      fontWeight: FontWeight.bold,
                      color: AppColors.primary,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            Text(
              content.trim(),
              style: AppTextStyles.bodyMedium.copyWith(
                height: 1.7,
                color: isDark ? AppColors.textPrimaryDark : AppColors.textPrimary,
              ),
            ),
          ],
        ),
      );
    }

    // Expandable section
    return Container(
      decoration: BoxDecoration(
        color: isDark ? AppColors.surfaceDark : AppColors.white,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
        border: Border.all(
          color: AppColors.divider,
          width: 1,
        ),
      ),
      child: Theme(
        data: Theme.of(context).copyWith(
          dividerColor: Colors.transparent,
        ),
        child: ExpansionTile(
          tilePadding: const EdgeInsets.symmetric(
            horizontal: AppDimensions.paddingMedium,
            vertical: AppDimensions.paddingSmall,
          ),
          childrenPadding: const EdgeInsets.only(
            left: AppDimensions.paddingMedium,
            right: AppDimensions.paddingMedium,
            bottom: AppDimensions.paddingMedium,
          ),
          title: Row(
            children: [
              Container(
                width: 4,
                height: 20,
                decoration: BoxDecoration(
                  color: AppColors.primary,
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
              const SizedBox(width: AppDimensions.spacingSm),
              Expanded(
                child: Text(
                  title,
                  style: AppTextStyles.subtitle1.copyWith(
                    fontWeight: FontWeight.bold,
                    color: isDark ? AppColors.textPrimaryDark : AppColors.textPrimary,
                  ),
                ),
              ),
            ],
          ),
          expandedCrossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              content.trim(),
              style: AppTextStyles.bodyMedium.copyWith(
                height: 1.7,
                color: isDark ? AppColors.textSecondaryDark : AppColors.textSecondary,
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _sharePrivacyPolicy() {
    // Implement share functionality
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: const Text('جاري مشاركة سياسة الخصوصية...'),
        backgroundColor: AppColors.info,
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
        ),
      ),
    );
  }
}

// Add this event to settings_event.dart
class AcceptPrivacyPolicyEvent extends SettingsEvent {
  const AcceptPrivacyPolicyEvent();
}
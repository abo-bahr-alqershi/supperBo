import 'package:flutter/material.dart';
import 'package:yemen_booking_app/features/property/domain/entities/property_policy.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class PoliciesWidget extends StatelessWidget {
  final List<PropertyPolicy> policies;

  const PoliciesWidget({
    super.key,
    required this.policies,
  });

  @override
  Widget build(BuildContext context) {
    if (policies.isEmpty) {
      return const SizedBox.shrink();
    }

    // Group policies by type
    final groupedPolicies = <String, List<PropertyPolicy>>{};
    for (final policy in policies) {
      final type = policy.policyType;
      if (!groupedPolicies.containsKey(type)) {
        groupedPolicies[type] = [];
      }
      groupedPolicies[type]!.add(policy);
    }

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: groupedPolicies.entries.map((entry) {
        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildPolicyTypeHeader(entry.key),
            const SizedBox(height: AppDimensions.spacingSm),
            ...entry.value.map((policy) => _buildPolicyItem(policy)),
            const SizedBox(height: AppDimensions.spacingMd),
          ],
        );
      }).toList(),
    );
  }

  Widget _buildPolicyTypeHeader(String type) {
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppDimensions.paddingMedium,
        vertical: AppDimensions.paddingSmall,
      ),
      decoration: BoxDecoration(
        color: _getPolicyTypeColor(type).withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusSm),
      ),
      child: Row(
        children: [
          Icon(
            _getPolicyTypeIcon(type),
            size: 20,
            color: _getPolicyTypeColor(type),
          ),
          const SizedBox(width: AppDimensions.spacingSm),
          Text(
            _getPolicyTypeTitle(type),
            style: AppTextStyles.subtitle2.copyWith(
              fontWeight: FontWeight.bold,
              color: _getPolicyTypeColor(type),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPolicyItem(PropertyPolicy policy) {
    return Container(
      margin: const EdgeInsets.only(bottom: AppDimensions.spacingSm),
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
        border: Border.all(
          color: policy.isActive 
              ? AppColors.error.withValues(alpha: 0.3)
              : AppColors.border,
        ),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  policy.policyContent,
                  style: AppTextStyles.bodyMedium.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              if (policy.isActive)
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppDimensions.paddingSmall,
                    vertical: AppDimensions.paddingXSmall,
                  ),
                  decoration: BoxDecoration(
                    color: AppColors.error.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(
                      AppDimensions.borderRadiusXs,
                    ),
                  ),
                  child: Text(
                    'إلزامي',
                    style: AppTextStyles.caption.copyWith(
                      color: AppColors.error,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
            ],
          ),
          const SizedBox(height: AppDimensions.spacingSm),
          Text(
            policy.description,
            style: AppTextStyles.bodySmall.copyWith(
              color: AppColors.textSecondary,
              height: 1.5,
            ),
          ),
          // Removed details block as PropertyPolicy has no details field
        ],
      ),
    );
  }

  Widget _buildPolicyDetails(String details) {
    final detailsList = details.split('\n').where((d) => d.trim().isNotEmpty).toList();
    
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: detailsList.map((detail) {
        return Padding(
          padding: const EdgeInsets.only(bottom: AppDimensions.spacingXs),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Container(
                margin: const EdgeInsets.only(top: 6),
                width: 4,
                height: 4,
                decoration: const BoxDecoration(
                  color: AppColors.textSecondary,
                  shape: BoxShape.circle,
                ),
              ),
              const SizedBox(width: AppDimensions.spacingSm),
              Expanded(
                child: Text(
                  detail.trim(),
                  style: AppTextStyles.caption.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
              ),
            ],
          ),
        );
      }).toList(),
    );
  }

  IconData _getPolicyTypeIcon(String type) {
    switch (type.toLowerCase()) {
      case 'checkin':
      case 'check-in':
        return Icons.login;
      case 'checkout':
      case 'check-out':
        return Icons.logout;
      case 'cancellation':
        return Icons.cancel_outlined;
      case 'payment':
        return Icons.payment;
      case 'house_rules':
      case 'rules':
        return Icons.rule;
      case 'safety':
        return Icons.security;
      case 'health':
        return Icons.health_and_safety;
      default:
        return Icons.policy;
    }
  }

  Color _getPolicyTypeColor(String type) {
    switch (type.toLowerCase()) {
      case 'checkin':
      case 'check-in':
      case 'checkout':
      case 'check-out':
        return AppColors.info;
      case 'cancellation':
        return AppColors.warning;
      case 'payment':
        return AppColors.success;
      case 'house_rules':
      case 'rules':
        return AppColors.primary;
      case 'safety':
      case 'health':
        return AppColors.error;
      default:
        return AppColors.textSecondary;
    }
  }

  String _getPolicyTypeTitle(String type) {
    switch (type.toLowerCase()) {
      case 'checkin':
      case 'check-in':
        return 'سياسة تسجيل الدخول';
      case 'checkout':
      case 'check-out':
        return 'سياسة تسجيل الخروج';
      case 'cancellation':
        return 'سياسة الإلغاء';
      case 'payment':
        return 'سياسة الدفع';
      case 'house_rules':
      case 'rules':
        return 'قوانين المكان';
      case 'safety':
        return 'السلامة والأمان';
      case 'health':
        return 'الصحة والنظافة';
      default:
        return 'سياسات أخرى';
    }
  }
}
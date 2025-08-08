// lib/features/home/presentation/pages/section_detail_page.dart

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../domain/entities/home_section.dart';
import '../bloc/section_bloc/section_bloc.dart';
import '../widgets/section_builder_widget.dart';

class SectionDetailPage extends StatefulWidget {
  final HomeSection section;

  const SectionDetailPage({
    super.key,
    required this.section,
  });

  @override
  State<SectionDetailPage> createState() => _SectionDetailPageState();
}

class _SectionDetailPageState extends State<SectionDetailPage> {
  late ScrollController _scrollController;

  @override
  void initState() {
    super.initState();
    _scrollController = ScrollController();
    
    // Load section data
    context.read<SectionBloc>().add(LoadSectionData(widget.section));
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: CustomScrollView(
        controller: _scrollController,
        slivers: [
          // App Bar
          _buildAppBar(),
          
          // Content
          SliverToBoxAdapter(
            child: BlocBuilder<SectionBloc, SectionState>(
              builder: (context, state) {
                if (state is SectionLoading) {
                  return _buildLoadingState();
                }
                
                if (state is SectionError) {
                  return _buildErrorState(state);
                }
                
                if (state is SectionLoaded) {
                  return _buildLoadedState(state);
                }
                
                return const SizedBox.shrink();
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAppBar() {
    return SliverAppBar(
      pinned: true,
      elevation: 0,
      backgroundColor: AppColors.surface,
      leading: IconButton(
        icon: const Icon(Icons.arrow_back_ios),
        onPressed: () => Navigator.of(context).pop(),
        color: AppColors.textPrimary,
      ),
      title: Text(
        widget.section.title ?? 'التفاصيل',
        style: AppTextStyles.heading3.copyWith(
          color: AppColors.textPrimary,
        ),
      ),
      actions: [
        IconButton(
          icon: const Icon(Icons.filter_list),
          onPressed: () {
            _showFilterOptions();
          },
          color: AppColors.textPrimary,
        ),
        IconButton(
          icon: const Icon(Icons.sort),
          onPressed: () {
            _showSortOptions();
          },
          color: AppColors.textPrimary,
        ),
      ],
    );
  }

  Widget _buildLoadingState() {
    return Container(
      height: MediaQuery.of(context).size.height * 0.6,
      alignment: Alignment.center,
      child: const LoadingWidget(
        type: LoadingType.circular,
        message: 'جاري تحميل المحتوى...',
      ),
    );
  }

  Widget _buildErrorState(SectionError state) {
    return Container(
      height: MediaQuery.of(context).size.height * 0.6,
      padding: const EdgeInsets.all(AppDimensions.paddingLarge),
      alignment: Alignment.center,
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.error_outline,
            size: 64,
            color: AppColors.error.withValues(alpha: 0.5),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          Text(
            state.message,
            style: AppTextStyles.bodyLarge.copyWith(
              color: AppColors.textSecondary,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: AppDimensions.spacingLg),
          ElevatedButton(
            onPressed: () {
              context.read<SectionBloc>().add(
                LoadSectionData(widget.section),
              );
            },
            child: const Text('إعادة المحاولة'),
          ),
        ],
      ),
    );
  }

  Widget _buildLoadedState(SectionLoaded state) {
    return Padding(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Section Description
          if (widget.section.subtitle != null)
            Padding(
              padding: const EdgeInsets.only(
                bottom: AppDimensions.spacingMd,
              ),
              child: Text(
                widget.section.subtitle!,
                style: AppTextStyles.bodyLarge.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
            ),
          
          // Results Count
          Container(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingMedium,
              vertical: AppDimensions.paddingSmall,
            ),
            decoration: BoxDecoration(
              color: AppColors.primary.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(
                AppDimensions.borderRadiusSm,
              ),
            ),
            child: Text(
              'عرض ${state.data?.totalItems ?? 0} نتيجة',
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppColors.primary,
                fontWeight: FontWeight.w600,
              ),
            ),
          ),
          
          const SizedBox(height: AppDimensions.spacingMd),
          
          // Section Content
          SectionBuilderWidget(
            section: state.section,
            data: state.data,
            isFullScreen: true,
            onItemTap: (item) {
              // Handle item tap
            },
          ),
          
          // Load More Button
          if (!state.hasReachedEnd)
            Center(
              child: Padding(
                padding: const EdgeInsets.only(
                  top: AppDimensions.spacingLg,
                ),
                child: state.isLoadingMore
                    ? const LoadingWidget(
                        type: LoadingType.circular,
                        size: 24,
                      )
                    : ElevatedButton(
                        onPressed: () {
                          context.read<SectionBloc>().add(
                            LoadMoreSectionItems(state.section.id),
                          );
                        },
                        child: const Text('عرض المزيد'),
                      ),
              ),
            ),
        ],
      ),
    );
  }

  void _showFilterOptions() {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(
          top: Radius.circular(AppDimensions.borderRadiusLg),
        ),
      ),
      builder: (context) {
        return Container(
          padding: const EdgeInsets.all(AppDimensions.paddingLarge),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Handle
              Center(
                child: Container(
                  width: 40,
                  height: 4,
                  decoration: BoxDecoration(
                    color: AppColors.gray200,
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
              ),
              const SizedBox(height: AppDimensions.spacingMd),
              
              const Text(
                'تصفية النتائج',
                style: AppTextStyles.heading3,
              ),
              
              const SizedBox(height: AppDimensions.spacingLg),
              
              // Filter options would go here
              const Text('خيارات التصفية'),
              
              const SizedBox(height: AppDimensions.spacingLg),
              
              // Apply Button
              SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  onPressed: () {
                    Navigator.of(context).pop();
                  },
                  child: const Text('تطبيق'),
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  void _showSortOptions() {
    showModalBottomSheet(
      context: context,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(
          top: Radius.circular(AppDimensions.borderRadiusLg),
        ),
      ),
      builder: (context) {
        return Container(
          padding: const EdgeInsets.all(AppDimensions.paddingLarge),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Handle
              Center(
                child: Container(
                  width: 40,
                  height: 4,
                  decoration: BoxDecoration(
                    color: AppColors.gray200,
                    borderRadius: BorderRadius.circular(2),
                  ),
                ),
              ),
              const SizedBox(height: AppDimensions.spacingMd),
              
              const Text(
                'ترتيب حسب',
                style: AppTextStyles.heading3,
              ),
              
              const SizedBox(height: AppDimensions.spacingLg),
              
              // Sort options
              _buildSortOption('الأكثر صلة', true),
              _buildSortOption('السعر: من الأقل إلى الأعلى', false),
              _buildSortOption('السعر: من الأعلى إلى الأقل', false),
              _buildSortOption('التقييم', false),
              _buildSortOption('الأحدث', false),
            ],
          ),
        );
      },
    );
  }

  Widget _buildSortOption(String title, bool isSelected) {
    return ListTile(
      contentPadding: EdgeInsets.zero,
      title: Text(
        title,
        style: AppTextStyles.bodyLarge.copyWith(
          color: isSelected ? AppColors.primary : AppColors.textPrimary,
          fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
        ),
      ),
      trailing: isSelected
          ? const Icon(
              Icons.check,
              color: AppColors.primary,
            )
          : null,
      onTap: () {
        Navigator.of(context).pop();
      },
    );
  }
}
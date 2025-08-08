// lib/features/home/presentation/widgets/home_sections_list_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:visibility_detector/visibility_detector.dart';

import '../../../../core/theme/app_dimensions.dart';
import '../../domain/entities/home_section.dart';
import '../bloc/section_bloc/section_bloc.dart';
import '../bloc/analytics_bloc/home_analytics_bloc.dart';
import 'section_builder_widget.dart';

class HomeSectionsListWidget extends StatelessWidget {
  final List<HomeSection> sections;
  final ScrollController? scrollController;

  const HomeSectionsListWidget({
    super.key,
    required this.sections,
    this.scrollController,
  });

  @override
  Widget build(BuildContext context) {
    return SliverList(
      delegate: SliverChildBuilderDelegate(
        (context, index) {
          final section = sections[index];
          
          return VisibilityDetector(
            key: Key('section_${section.id}'),
            onVisibilityChanged: (info) {
              if (info.visibleFraction > 0.5) {
                // Track section impression
                context.read<HomeAnalyticsBloc>().add(
                  TrackSectionView(
                    sectionId: section.id,
                    sectionType: section.sectionType.value,
                    position: index,
                  ),
                );
                
                // Update section visibility
                context.read<SectionBloc>().add(
                  UpdateSectionVisibility(
                    sectionId: section.id,
                    isVisible: true,
                  ),
                );
              }
            },
            child: AnimatedSectionWrapper(
              section: section,
              index: index,
              child: Padding(
                padding: const EdgeInsets.only(
                  bottom: AppDimensions.spacingLg,
                ),
                child: BlocProvider(
                  create: (context) => SectionBloc(
                    getSectionData: context.read(),
                    trackImpression: context.read(),
                    trackInteraction: context.read(),
                  )..add(LoadSectionData(section)),
                  child: SectionBuilderWidget(
                    section: section,
                    onItemTap: (item) {
                      // Track item interaction
                      context.read<HomeAnalyticsBloc>().add(
                        TrackItemClick(
                          sectionId: section.id,
                          itemId: item.id,
                          itemType: section.sectionType.value,
                          position: index,
                        ),
                      );
                    },
                  ),
                ),
              ),
            ),
          );
        },
        childCount: sections.length,
      ),
    );
  }
}

class AnimatedSectionWrapper extends StatefulWidget {
  final HomeSection section;
  final int index;
  final Widget child;

  const AnimatedSectionWrapper({
    super.key,
    required this.section,
    required this.index,
    required this.child,
  });

  @override
  State<AnimatedSectionWrapper> createState() => 
      _AnimatedSectionWrapperState();
}

class _AnimatedSectionWrapperState extends State<AnimatedSectionWrapper> 
    with SingleTickerProviderStateMixin {
  late AnimationController _animationController;
  late Animation<double> _fadeAnimation;
  late Animation<Offset> _slideAnimation;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: Duration(milliseconds: 500 + (widget.index * 50)),
      vsync: this,
    );
    
    _fadeAnimation = Tween<double>(
      begin: 0.0,
      end: 1.0,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeIn,
    ));
    
    _slideAnimation = Tween<Offset>(
      begin: const Offset(0, 0.1),
      end: Offset.zero,
    ).animate(CurvedAnimation(
      parent: _animationController,
      curve: Curves.easeOut,
    ));
    
    _animationController.forward();
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return FadeTransition(
      opacity: _fadeAnimation,
      child: SlideTransition(
        position: _slideAnimation,
        child: widget.child,
      ),
    );
  }
}
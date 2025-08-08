// lib/features/home/presentation/widgets/section_builder_widget.dart

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/enums/section_type_enum.dart';
import '../../domain/entities/home_section.dart';
import '../bloc/section_bloc/section_bloc.dart';
import 'sections/base/section_placeholder.dart';
import 'sections/base/section_error_widget.dart';
import 'section_factory.dart';

class SectionBuilderWidget extends StatelessWidget {
  final HomeSection section;
  final dynamic data;
  final bool isFullScreen;
  final Function(dynamic)? onItemTap;

  const SectionBuilderWidget({
    super.key,
    required this.section,
    this.data,
    this.isFullScreen = false,
    this.onItemTap,
  });

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<SectionBloc, SectionState>(
      builder: (context, state) {
        if (state is SectionLoading) {
          return _buildLoadingState();
        }
        
        if (state is SectionError) {
          return _buildErrorState(context, state);
        }
        
        if (state is SectionLoaded) {
          return _buildLoadedState(state);
        }
        
        return const SizedBox.shrink();
      },
    );
  }

  Widget _buildLoadingState() {
    return SectionPlaceholder(
      sectionType: section.sectionType,
      height: _getSectionHeight(),
    );
  }

  Widget _buildErrorState(BuildContext context, SectionError state) {
    return SectionErrorWidget(
      message: state.message,
      onRetry: () {
        context.read<SectionBloc>().add(LoadSectionData(section));
      },
    );
  }

  Widget _buildLoadedState(SectionLoaded state) {
    final sectionData = data ?? state.data;
    
    if (sectionData == null || 
        (sectionData is List && sectionData.isEmpty)) {
      return const SizedBox.shrink();
    }
    
    return SectionFactory.createSection(
      section: section,
      data: sectionData,
      isFullScreen: isFullScreen,
      onItemTap: onItemTap,
    );
  }

  double _getSectionHeight() {
    switch (section.sectionType) {
      case SectionType.singlePropertyAd:
        return 320;
      case SectionType.multiPropertyAd:
        return 280;
      case SectionType.horizontalPropertyList:
        return 280;
      case SectionType.verticalPropertyGrid:
        return 400;
      case SectionType.cityCardsGrid:
        return 200;
      case SectionType.premiumCarousel:
        return 380;
      default:
        return 300;
    }
  }
}
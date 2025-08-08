import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import '../../../../core/widgets/app_bar_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../core/widgets/error_widget.dart' as core_err;
import '../../domain/entities/home_section.dart';
import '../bloc/home_bloc.dart';
import '../bloc/home_event.dart';
import '../bloc/home_state.dart';
import '../widgets/search_bar_widget.dart';
import '../widgets/home_sections_list_widget.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const AppBarWidget(title: 'الرئيسية', automaticallyImplyLeading: false),
      body: SafeArea(
        child: BlocBuilder<HomeBloc, HomeState>(
          builder: (context, state) {
            if (state is HomeInitial) {
              context.read<HomeBloc>().add(const LoadHomeData());
              return const LoadingWidget();
            }
            if (state is HomeLoading) {
              return const LoadingWidget();
            }
            if (state is HomeError) {
              return core_err.ErrorDisplayWidget(message: state.message);
            }
            if (state is HomeLoaded) {
              final List<HomeSection> sections = state.sections;
              return CustomScrollView(
                slivers: [
                  SliverToBoxAdapter(
                    child: Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 12.0),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: const [
                          SearchBarWidget(),
                          SizedBox(height: 12),
                        ],
                      ),
                    ),
                  ),
                  HomeSectionsListSliver(sections: sections),
                ],
              );
            }
            return const SizedBox.shrink();
          },
        ),
      ),
    );
  }
}
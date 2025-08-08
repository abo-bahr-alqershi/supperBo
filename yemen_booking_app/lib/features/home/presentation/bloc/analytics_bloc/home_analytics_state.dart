import 'package:equatable/equatable.dart';

abstract class HomeAnalyticsState extends Equatable {
  const HomeAnalyticsState();
  @override
  List<Object?> get props => [];
}

class HomeAnalyticsIdle extends HomeAnalyticsState {}

class HomeAnalyticsLogging extends HomeAnalyticsState {}

class HomeAnalyticsError extends HomeAnalyticsState {
  final String message;
  const HomeAnalyticsError(this.message);
  @override
  List<Object> get props => [message];
}
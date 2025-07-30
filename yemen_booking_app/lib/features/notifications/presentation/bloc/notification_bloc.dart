import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:equatable/equatable.dart';

part 'notification_event.dart';
part 'notification_state.dart';

class NotificationBloc extends Bloc<NotificationEvent, NotificationState> {
  NotificationBloc() : super(NotificationInitial()) {
    on<LoadNotificationsEvent>(_onLoadNotifications);
    on<MarkAsReadEvent>(_onMarkAsRead);
    on<ClearAllNotificationsEvent>(_onClearAll);
  }

  void _onLoadNotifications(LoadNotificationsEvent event, Emitter<NotificationState> emit) {
    emit(NotificationLoading());
    
    // TODO: Implement loading notifications from repository
    emit(NotificationLoaded(notifications: []));
  }

  void _onMarkAsRead(MarkAsReadEvent event, Emitter<NotificationState> emit) {
    if (state is NotificationLoaded) {
      final currentState = state as NotificationLoaded;
      // TODO: Implement mark as read logic
      emit(currentState);
    }
  }

  void _onClearAll(ClearAllNotificationsEvent event, Emitter<NotificationState> emit) {
    emit(NotificationLoaded(notifications: []));
  }
}
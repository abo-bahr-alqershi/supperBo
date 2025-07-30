import 'package:equatable/equatable.dart';

abstract class Failure extends Equatable {
  final String message;
  final int? code;
  final dynamic data;

  const Failure({
    required this.message,
    this.code,
    this.data,
  });

  @override
  List<Object?> get props => [message, code, data];
}

// General failures
class ServerFailure extends Failure {
  const ServerFailure({
    required super.message,
    super.code,
    super.data,
  });
}

class NetworkFailure extends Failure {
  const NetworkFailure({
    super.message = 'لا يوجد اتصال بالإنترنت',
    super.code,
    super.data,
  });
}

class CacheFailure extends Failure {
  const CacheFailure({
    super.message = 'خطأ في التخزين المحلي',
    super.code,
    super.data,
  });
}

// Auth failures
class AuthenticationFailure extends Failure {
  const AuthenticationFailure({
    required super.message,
    super.code,
    super.data,
  });
}

class UnauthorizedFailure extends Failure {
  const UnauthorizedFailure({
    super.message = 'غير مصرح بالوصول',
    super.code = 401,
    super.data,
  });
}

class SessionExpiredFailure extends Failure {
  const SessionExpiredFailure({
    super.message = 'انتهت صلاحية الجلسة',
    super.code = 401,
    super.data,
  });
}

// Validation failures
class ValidationFailure extends Failure {
  final Map<String, List<String>>? errors;

  const ValidationFailure({
    super.message = 'خطأ في البيانات المدخلة',
    super.code = 422,
    this.errors,
    super.data,
  });

  @override
  List<Object?> get props => [message, code, errors, data];
}

// Business logic failures
class BusinessLogicFailure extends Failure {
  const BusinessLogicFailure({
    required super.message,
    super.code,
    super.data,
  });
}

class NotFoundFailure extends Failure {
  const NotFoundFailure({
    super.message = 'لم يتم العثور على البيانات',
    super.code = 404,
    super.data,
  });
}

class PermissionDeniedFailure extends Failure {
  const PermissionDeniedFailure({
    super.message = 'ليس لديك صلاحية للقيام بهذا الإجراء',
    super.code = 403,
    super.data,
  });
}

// Other failures
class UnknownFailure extends Failure {
  const UnknownFailure({
    super.message = 'حدث خطأ غير متوقع',
    super.code,
    super.data,
  });
}

class TimeoutFailure extends Failure {
  const TimeoutFailure({
    super.message = 'انتهت مهلة الاتصال',
    super.code,
    super.data,
  });
}

class MaintenanceFailure extends Failure {
  const MaintenanceFailure({
    super.message = 'الخدمة تحت الصيانة حالياً',
    super.code = 503,
    super.data,
  });
}
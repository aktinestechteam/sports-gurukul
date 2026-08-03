import 'package:sports_gurukul/core/failures/base_failure.dart';

/// The root of the application exception hierarchy.
///
/// Exceptions represent *unrecoverable* or *unexpected* runtime errors that
/// occur at infrastructure boundaries (I/O, parsing, transport). They are
/// distinct from [BaseFailure] values: infrastructure may throw an
/// [AppException], but the boundary between infrastructure and the rest of
/// the app maps it to a failure so features never surface raw exceptions.
///
/// The hierarchy is sealed; every exception type is defined in this library.
sealed class AppException implements Exception {
  const AppException({required this.message, this.cause});

  /// Developer-facing description of the error.
  final String message;

  /// The underlying error, when one is available.
  final Object? cause;

  @override
  // Deliberate: include the type name for readable exception messages.
  // ignore: no_runtimetype_tostring
  String toString() => '$runtimeType: $message';
}

/// Signals a failure produced by an HTTP/API call.
final class ApiException extends AppException {
  const ApiException({
    super.message = 'API request failed',
    this.statusCode,
    this.code,
    super.cause,
  });

  /// The HTTP status code of the failed response, when available.
  final int? statusCode;

  /// A stable machine-readable error code, when the server provides one.
  final String? code;
}

/// Signals a failure reading from or writing to a cache.
final class CacheException extends AppException {
  const CacheException({
    super.message = 'Cache operation failed',
    super.cause,
  });
}

/// Signals a failure reading from or writing to local storage.
final class StorageException extends AppException {
  const StorageException({
    super.message = 'Storage operation failed',
    super.cause,
  });
}

/// Signals that an operation exceeded its allotted time.
///
/// This deliberately shadows `dart:async`'s `TimeoutException`. Consumers
/// that also import `dart:async` should hide or prefix one of the two.
final class TimeoutException extends AppException {
  const TimeoutException({
    super.message = 'Operation timed out',
    super.cause,
  });
}

/// Signals that a value could not be parsed into its target representation.
final class ParsingException extends AppException {
  const ParsingException({
    super.message = 'Failed to parse value',
    super.cause,
  });
}

/// The root of the application failure hierarchy.
///
/// Failures are immutable, value-like descriptions of *why* an operation
/// did not succeed. They are deliberately not exceptions: they flow through
/// `Result` and `OperationResult` as data so the UI can switch on them
/// exhaustively and map them to user-facing messages without try/catch.
///
/// The hierarchy is sealed; every failure type is defined in this library
/// and `switch` over a [BaseFailure] is exhaustive at compile time.
sealed class BaseFailure {
  const BaseFailure({required this.message, this.code, this.cause});

  /// Human-readable, developer-facing description of the failure.
  ///
  /// Not intended for direct display; the UI must map failures to localized
  /// messages by type and [code].
  final String message;

  /// Stable machine-readable error code (e.g. an l10n key or API code).
  final String? code;

  /// The underlying error, when one is available.
  final Object? cause;

  @override
  // Deliberate: include the type name for readable failure messages.
  // ignore: no_runtimetype_tostring
  String toString() => '$runtimeType(message: $message, code: $code)';
}

/// Signals that input failed one or more validation rules.
final class ValidationFailure extends BaseFailure {
  const ValidationFailure({
    super.message = 'Validation failed',
    super.code,
    super.cause,
  });
}

/// Signals that the user is not authenticated or the session is invalid.
final class AuthenticationFailure extends BaseFailure {
  const AuthenticationFailure({
    super.message = 'Authentication failed',
    super.code,
    super.cause,
  });
}

/// Signals that a network request could not be completed.
final class NetworkFailure extends BaseFailure {
  const NetworkFailure({
    super.message = 'Network request failed',
    super.code,
    super.cause,
  });
}

/// Signals that the server responded with an error.
final class ServerFailure extends BaseFailure {
  const ServerFailure({
    super.message = 'Server error',
    super.code,
    super.cause,
  });
}

/// Signals that the user lacks permission to perform the operation.
final class PermissionFailure extends BaseFailure {
  const PermissionFailure({
    super.message = 'Permission denied',
    super.code,
    super.cause,
  });
}

/// Signals that a local storage operation failed.
final class StorageFailure extends BaseFailure {
  const StorageFailure({
    super.message = 'Storage operation failed',
    super.code,
    super.cause,
  });
}

/// Signals that a cache read or write failed.
final class CacheFailure extends BaseFailure {
  const CacheFailure({
    super.message = 'Cache operation failed',
    super.code,
    super.cause,
  });
}

/// Signals an unexpected failure that has no more specific classification.
final class UnknownFailure extends BaseFailure {
  const UnknownFailure({
    super.message = 'An unexpected error occurred',
    super.code,
    super.cause,
  });
}

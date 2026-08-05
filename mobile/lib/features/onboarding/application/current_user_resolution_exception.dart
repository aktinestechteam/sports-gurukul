import 'package:sports_gurukul/core/failures/base_failure.dart';

/// Thrown from current-user resolution so a typed [BaseFailure] can cross the
/// async provider boundary (which only accepts objects implementing
/// [Exception] or [Error]).
class CurrentUserResolutionException implements Exception {
  const CurrentUserResolutionException(this.failure);

  /// The failure that describes why resolution did not succeed.
  final BaseFailure failure;

  @override
  String toString() => 'CurrentUserResolutionException($failure)';
}

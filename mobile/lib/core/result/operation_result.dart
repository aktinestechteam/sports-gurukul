import 'package:flutter/foundation.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';

/// The outcome of an operation that has no meaningful return value.
///
/// `OperationResult` is the void counterpart of `Result`: it distinguishes
/// a successful side effect from a failed one without forcing callers to
/// construct a `Success<void>(null)`. The hierarchy is sealed, so `switch`
/// over an [OperationResult] is exhaustive at compile time.
@immutable
sealed class OperationResult {
  const OperationResult();

  /// Creates a successful [OperationResult].
  const factory OperationResult.success() = OperationSuccess;

  /// Creates a failed [OperationResult] carrying a [BaseFailure].
  const factory OperationResult.failure(BaseFailure failure) = OperationFailure;

  /// Whether this [OperationResult] succeeded.
  bool get isSuccess => switch (this) {
    OperationSuccess() => true,
    OperationFailure() => false,
  };

  /// Whether this [OperationResult] failed.
  bool get isFailure => !isSuccess;

  /// The failure, or `null` when this [OperationResult] succeeded.
  BaseFailure? get failureOrNull => switch (this) {
    OperationSuccess() => null,
    OperationFailure(:final failure) => failure,
  };

  /// Reduces this [OperationResult] to a single value of type [R].
  R fold<R>(
    R Function() onSuccess,
    R Function(BaseFailure failure) onFailure,
  ) => switch (this) {
    OperationSuccess() => onSuccess(),
    OperationFailure(:final failure) => onFailure(failure),
  };

  /// Named-argument variant of [fold].
  R when<R>({
    required R Function() onSuccess,
    required R Function(BaseFailure failure) onFailure,
  }) => fold(onSuccess, onFailure);

  /// Invokes [action] when this operation succeeded and returns `this` for
  /// chaining.
  OperationResult onSuccess(void Function() action) {
    if (this is OperationSuccess) {
      action();
    }
    // Deliberate: fluent chaining API mirrors Dart's `Iterable` builders.
    // ignore: avoid_returning_this
    return this;
  }

  /// Invokes [action] with the failure when this operation failed and
  /// returns `this` for chaining.
  OperationResult onFailure(void Function(BaseFailure failure) action) {
    if (this case OperationFailure(:final failure)) {
      action(failure);
    }
    // Deliberate: fluent chaining API mirrors Dart's `Iterable` builders.
    // ignore: avoid_returning_this
    return this;
  }
}

/// The successful variant of [OperationResult].
@immutable
final class OperationSuccess extends OperationResult {
  const OperationSuccess();

  @override
  bool operator ==(Object other) => other is OperationSuccess;

  @override
  int get hashCode => runtimeType.hashCode;

  @override
  String toString() => 'OperationSuccess';
}

/// The failed variant of [OperationResult], carrying a [failure].
@immutable
final class OperationFailure extends OperationResult {
  const OperationFailure(this.failure);

  /// The failure that describes why the operation did not succeed.
  final BaseFailure failure;

  @override
  bool operator ==(Object other) =>
      other is OperationFailure && other.failure == failure;

  @override
  int get hashCode => Object.hash(runtimeType, failure);

  @override
  String toString() => 'OperationFailure(failure: $failure)';
}

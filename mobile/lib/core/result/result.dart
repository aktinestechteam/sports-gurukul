import 'package:flutter/foundation.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';

/// The outcome of an operation that may produce a value or fail.
///
/// A [Result] is a sealed discriminated union of [Success] and
/// [FailureResult]. Features return `Result<T>` from repositories and use
/// cases; raw exceptions are never surfaced to the UI. Because the hierarchy
/// is sealed, `switch` over a [Result] is exhaustive at compile time.
///
/// ```dart
/// final result = await repository.fetchProfile();
/// final name = result.fold(
///   (profile) => profile.name,
///   (failure) => 'Guest',
/// );
/// ```
@immutable
sealed class Result<T> {
  const Result();

  /// Creates a successful [Result] holding [value].
  const factory Result.success(T value) = Success<T>;

  /// Creates a failed [Result] carrying a [BaseFailure].
  const factory Result.failure(BaseFailure failure) = FailureResult<T>;

  /// Whether this [Result] holds a value.
  bool get isSuccess => switch (this) {
    Success<T>() => true,
    FailureResult<T>() => false,
  };

  /// Whether this [Result] carries a failure.
  bool get isFailure => !isSuccess;

  /// The successful value, or `null` when this is a [FailureResult].
  T? get valueOrNull => switch (this) {
    Success<T>(:final value) => value,
    FailureResult<T>() => null,
  };

  /// The failure, or `null` when this is a [Success].
  BaseFailure? get failureOrNull => switch (this) {
    Success<T>() => null,
    FailureResult<T>(:final failure) => failure,
  };

  /// Returns the successful value, throwing a [StateError] when this
  /// [Result] is a [FailureResult].
  ///
  /// Prefer [fold] or [when] for normal control flow; this is an escape
  /// hatch for cases where a failure is provably impossible.
  T requireValue() => switch (this) {
    Success<T>(:final value) => value,
    FailureResult<T>(:final failure) => throw StateError(
      'requireValue() called on a failed result: '
      '${failure.message}',
    ),
  };

  /// Transforms the value with [onSuccess] and the failure with
  /// [onFailure], producing a single result of type [R].
  R fold<R>(
    R Function(T value) onSuccess,
    R Function(BaseFailure failure) onFailure,
  ) => switch (this) {
    Success<T>(:final value) => onSuccess(value),
    FailureResult<T>(:final failure) => onFailure(failure),
  };

  /// Named-argument variant of [fold].
  R when<R>({
    required R Function(T value) onSuccess,
    required R Function(BaseFailure failure) onFailure,
  }) => fold(onSuccess, onFailure);

  /// Maps only the successful value, preserving the failure unchanged.
  Result<R> map<R>(R Function(T value) transform) => switch (this) {
    Success<T>(:final value) => Success<R>(transform(value)),
    FailureResult<T>(:final failure) => FailureResult<R>(failure),
  };

  /// Invokes [action] with the value when this is a [Success] and returns
  /// `this` for chaining.
  Result<T> onSuccess(void Function(T value) action) {
    if (this case Success<T>(:final value)) {
      action(value);
    }
    // Deliberate: fluent chaining API mirrors Dart's `Iterable` builders.
    // ignore: avoid_returning_this
    return this;
  }

  /// Invokes [action] with the failure when this is a [FailureResult] and
  /// returns `this` for chaining.
  Result<T> onFailure(void Function(BaseFailure failure) action) {
    if (this case FailureResult<T>(:final failure)) {
      action(failure);
    }
    // Deliberate: fluent chaining API mirrors Dart's `Iterable` builders.
    // ignore: avoid_returning_this
    return this;
  }

  /// Recovers from a failure by producing a fallback value.
  Result<T> recoverWith(T Function(BaseFailure failure) onFailure) =>
      switch (this) {
        Success<T>() => this,
        FailureResult<T>(:final failure) => Success<T>(onFailure(failure)),
      };
}

/// The successful variant of [Result], holding [value].
@immutable
final class Success<T> extends Result<T> {
  const Success(this.value);

  /// The successful value.
  final T value;

  @override
  bool operator ==(Object other) => other is Success<T> && other.value == value;

  @override
  int get hashCode => Object.hash(runtimeType, value);

  @override
  String toString() => 'Success(value: $value)';
}

/// The failed variant of [Result], carrying a [failure].
@immutable
final class FailureResult<T> extends Result<T> {
  const FailureResult(this.failure);

  /// The failure that describes why the operation did not succeed.
  final BaseFailure failure;

  @override
  bool operator ==(Object other) =>
      other is FailureResult<T> && other.failure == failure;

  @override
  int get hashCode => Object.hash(runtimeType, failure);

  @override
  String toString() => 'FailureResult(failure: $failure)';
}

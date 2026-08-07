import 'package:collection/collection.dart';
import 'package:flutter/foundation.dart';

/// Describes why a value failed validation.
///
/// Validators never return user-facing strings; they return a stable [code]
/// (an l10n key) plus optional [params] so the UI can resolve localized
/// messages without leaking validation internals.
@immutable
class ValidationError {
  const ValidationError(this.code, {this.params});

  /// Stable localization key for this error.
  final String code;

  /// Optional named parameters interpolated into the localized message.
  final Map<String, Object?>? params;

  @override
  bool operator ==(Object other) =>
      other is ValidationError &&
      other.code == code &&
      const DeepCollectionEquality().equals(other.params, params);

  @override
  int get hashCode =>
      Object.hash(code, const DeepCollectionEquality().hash(params));

  @override
  String toString() => 'ValidationError($code)';
}

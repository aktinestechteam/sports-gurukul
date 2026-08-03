import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/core/validators/validator.dart';

/// Ensures a value is present.
///
/// `null`, blank strings, empty collections and empty maps are considered
/// missing. Works for any type; format validators do not cover this case.
class RequiredValidator<T> extends Validator<T> {
  const RequiredValidator({this.code = 'validation.required'});

  /// Localization key for the "required" error.
  final String code;

  @override
  ValidationError? validate(T? value) {
    if (value == null) {
      return ValidationError(code);
    }
    if (value is String && value.trim().isEmpty) {
      return ValidationError(code);
    }
    if (value is Iterable<Object?> && value.isEmpty) {
      return ValidationError(code);
    }
    if (value is Map<Object?, Object?> && value.isEmpty) {
      return ValidationError(code);
    }
    return null;
  }
}

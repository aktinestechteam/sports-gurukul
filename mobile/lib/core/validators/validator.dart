import 'package:sports_gurukul/core/validators/validation_error.dart';

/// Generic contract for a validator of values of type [T].
///
/// A validator returns a [ValidationError] when `value` is invalid and
/// `null` when it is valid. Format validators treat empty input as valid —
/// emptiness is the responsibility of `RequiredValidator`, composed with
/// others via [CompositeValidator].
abstract class Validator<T> {
  const Validator();

  /// Returns an error describing why [value] is invalid, or `null` when it
  /// is valid.
  ValidationError? validate(T? value);

  /// Whether [value] passes this validator.
  bool isValid(T? value) => validate(value) == null;
}

/// Runs several validators in order and returns the first error found.
///
/// Enables composition of the generic validator framework, e.g. a required
/// email field composed of `RequiredValidator` and an email-format check.
class CompositeValidator<T> extends Validator<T> {
  const CompositeValidator(this._validators);

  /// Validators run in declaration order; the first failure wins.
  final List<Validator<T>> _validators;

  @override
  ValidationError? validate(T? value) {
    for (final validator in _validators) {
      final error = validator.validate(value);
      if (error != null) {
        return error;
      }
    }
    return null;
  }
}

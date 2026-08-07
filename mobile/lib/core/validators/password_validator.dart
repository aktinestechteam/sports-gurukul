import 'package:sports_gurukul/core/constants/regex_constants.dart';
import 'package:sports_gurukul/core/constants/validation_constants.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/core/validators/validator.dart';

/// Validates password length and optional complexity requirements.
class PasswordValidator extends Validator<String> {
  const PasswordValidator({
    this.minLength = ValidationConstants.minPasswordLength,
    this.maxLength = ValidationConstants.maxPasswordLength,
    this.requireUppercase = false,
    this.requireLowercase = false,
    this.requireDigit = false,
    this.requireSpecialCharacter = false,
  });

  /// Minimum accepted length.
  final int minLength;

  /// Maximum accepted length.
  final int maxLength;

  /// Whether at least one uppercase letter is required.
  final bool requireUppercase;

  /// Whether at least one lowercase letter is required.
  final bool requireLowercase;

  /// Whether at least one digit is required.
  final bool requireDigit;

  /// Whether at least one special character is required.
  final bool requireSpecialCharacter;

  @override
  ValidationError? validate(String? value) {
    final candidate = value ?? '';
    if (candidate.isEmpty) {
      return null;
    }
    if (candidate.length < minLength) {
      return ValidationError(
        'validation.password.tooShort',
        params: {
          'min': minLength,
        },
      );
    }
    if (candidate.length > maxLength) {
      return ValidationError(
        'validation.password.tooLong',
        params: {
          'max': maxLength,
        },
      );
    }
    if (requireUppercase && !RegexConstants.hasUppercase.hasMatch(candidate)) {
      return const ValidationError('validation.password.uppercase');
    }
    if (requireLowercase && !RegexConstants.hasLowercase.hasMatch(candidate)) {
      return const ValidationError('validation.password.lowercase');
    }
    if (requireDigit && !RegexConstants.hasDigit.hasMatch(candidate)) {
      return const ValidationError('validation.password.digit');
    }
    if (requireSpecialCharacter &&
        !RegexConstants.hasSpecialCharacter.hasMatch(candidate)) {
      return const ValidationError('validation.password.special');
    }
    return null;
  }
}

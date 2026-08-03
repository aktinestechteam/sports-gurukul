import 'package:sports_gurukul/core/constants/regex_constants.dart';
import 'package:sports_gurukul/core/constants/validation_constants.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/core/validators/validator.dart';

/// Validates the format of an email address.
class EmailValidator extends Validator<String> {
  const EmailValidator({this.code = 'validation.email.invalid'});

  /// Localization key for the invalid-email error.
  final String code;

  @override
  ValidationError? validate(String? value) {
    final candidate = value?.trim();
    if (candidate == null || candidate.isEmpty) {
      return null;
    }
    if (candidate.length > ValidationConstants.maxEmailLength) {
      return ValidationError(code);
    }
    if (!RegexConstants.email.hasMatch(candidate)) {
      return ValidationError(code);
    }
    return null;
  }
}

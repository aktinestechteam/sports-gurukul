import 'package:sports_gurukul/core/constants/regex_constants.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/core/validators/validator.dart';

/// Validates an Indian mobile number (10 digits starting with 6-9).
class PhoneValidator extends Validator<String> {
  const PhoneValidator({this.code = 'validation.phone.invalid'});

  /// Localization key for the invalid-phone error.
  final String code;

  @override
  ValidationError? validate(String? value) {
    final candidate = value?.trim();
    if (candidate == null || candidate.isEmpty) {
      return null;
    }
    if (!RegexConstants.indianMobile.hasMatch(candidate)) {
      return ValidationError(code);
    }
    return null;
  }
}

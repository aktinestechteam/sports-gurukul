import 'package:sports_gurukul/core/constants/validation_constants.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/core/validators/validator.dart';

/// Validates a numeric one-time password.
class OtpValidator extends Validator<String> {
  const OtpValidator({
    this.length = ValidationConstants.otpLength,
    this.code = 'validation.otp.invalid',
  });

  /// Expected number of digits.
  final int length;

  /// Localization key for the invalid-OTP error.
  final String code;

  @override
  ValidationError? validate(String? value) {
    final candidate = value?.trim();
    if (candidate == null || candidate.isEmpty) {
      return null;
    }
    if (candidate.length != length || int.tryParse(candidate) == null) {
      return ValidationError(code, params: {'length': length});
    }
    return null;
  }
}

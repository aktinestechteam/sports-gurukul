/// Validation rules shared by the `core/validators` module.
abstract final class ValidationConstants {
  /// Minimum accepted password length.
  static const int minPasswordLength = 8;

  /// Maximum accepted password length.
  static const int maxPasswordLength = 64;

  /// Length of a numeric one-time password.
  static const int otpLength = 6;

  /// Minimum length of an OTP input while typing.
  static const int minOtpLength = 4;

  /// Maximum length of an OTP input while typing.
  static const int maxOtpLength = 6;

  /// Length of an Indian mobile number without the country code.
  static const int indianMobileLength = 10;

  /// Minimum length of a person or academy name.
  static const int minNameLength = 2;

  /// Maximum length of a person or academy name.
  static const int maxNameLength = 64;

  /// Maximum length of an email address (RFC 5321 limit).
  static const int maxEmailLength = 254;

  /// Minimum characters before search triggers.
  static const int minSearchQueryLength = 2;

  /// Maximum accepted upload size for images (5 MiB).
  static const int maxImageUploadBytes = 5 * 1024 * 1024;

  /// Maximum accepted upload size for documents (10 MiB).
  static const int maxFileUploadBytes = 10 * 1024 * 1024;
}

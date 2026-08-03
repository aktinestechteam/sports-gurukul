/// Centralized regular expressions used by validators and utilities.
///
/// `RegExp` has no const constructor, so these are `final` rather than
/// `const`. They are created once on first access and reused everywhere.
abstract final class RegexConstants {
  /// Basic email pattern: `user@domain.tld`.
  static final RegExp email = RegExp(r'^[\w.+-]+@[\w-]+(\.[\w-]+)+$');

  /// Indian mobile number: 10 digits starting with 6-9.
  static final RegExp indianMobile = RegExp(r'^[6-9]\d{9}$');

  /// Exactly six digits (numeric OTP).
  static final RegExp otp = RegExp(r'^\d{6}$');

  /// Digits only.
  static final RegExp onlyDigits = RegExp(r'^\d+$');

  /// Latin letters only.
  static final RegExp onlyLetters = RegExp(r'^[A-Za-z]+$');

  /// Latin letters and digits only.
  static final RegExp alphanumeric = RegExp(r'^[A-Za-z0-9]+$');

  /// Contains at least one uppercase letter.
  static final RegExp hasUppercase = RegExp('[A-Z]');

  /// Contains at least one lowercase letter.
  static final RegExp hasLowercase = RegExp('[a-z]');

  /// Contains at least one digit.
  static final RegExp hasDigit = RegExp(r'\d');

  /// Contains at least one non-alphanumeric, non-whitespace character.
  static final RegExp hasSpecialCharacter = RegExp(r'[^A-Za-z0-9\s]');

  /// URL-friendly slug: lowercase words joined by single hyphens.
  static final RegExp slug = RegExp(r'^[a-z0-9]+(?:-[a-z0-9]+)*$');

  /// Characters that are invalid in file names on common platforms.
  static final RegExp invalidFileName = RegExp(r'[\\/:*?"<>|\s]');

  /// Loose HTTP(S) URL pattern.
  static final RegExp url = RegExp(
    r'^(https?://)?([\w-]+\.)+[\w-]+(/[\w\-./?%&=]*)?$',
  );
}

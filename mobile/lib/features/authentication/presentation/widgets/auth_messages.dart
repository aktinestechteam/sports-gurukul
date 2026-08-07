import 'package:flutter/material.dart';

import 'package:sports_gurukul/core/constants/validation_constants.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/error/auth_error_mapper.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Localizes form-validation and API failures for the authentication screens.
///
/// Validation maps the core validator error codes to l10n strings; API
/// failures map the stable [BaseFailure.code] set from [AuthErrorMapper] and
/// fall back to the server's detail message when no code is matched.
abstract final class AuthMessages {
  /// Localized text for a [ValidationError], or null when valid.
  static String? validation(BuildContext context, ValidationError? error) {
    if (error == null) {
      return null;
    }
    final l10n = AppLocalizations.of(context);
    return switch (error.code) {
      'validation.required' => l10n.validationRequired,
      'validation.email.invalid' => l10n.validationEmailInvalid,
      'validation.password.tooShort' => l10n.validationPasswordTooShort(
        (error.params?['min'] as int?) ?? ValidationConstants.minPasswordLength,
      ),
      'validation.password.tooLong' => l10n.validationPasswordTooLong(
        (error.params?['max'] as int?) ?? ValidationConstants.maxPasswordLength,
      ),
      'validation.password.uppercase' => l10n.validationPasswordUppercase,
      'validation.password.lowercase' => l10n.validationPasswordLowercase,
      'validation.password.digit' => l10n.validationPasswordDigit,
      'validation.password.special' => l10n.validationPasswordSpecial,
      'validation.password.mismatch' => l10n.validationPasswordMismatch,
      'validation.phone.invalid' => l10n.validationPhoneInvalid,
      _ => error.code,
    };
  }

  /// Localized user-facing text for an API [failure].
  static String apiFailure(BuildContext context, BaseFailure failure) {
    final l10n = AppLocalizations.of(context);
    return switch (failure.code) {
      AuthErrorCodes.invalidCredentials => l10n.authErrorsInvalidCredentials,
      AuthErrorCodes.accountLocked => l10n.authErrorsAccountLocked,
      AuthErrorCodes.sessionExpired => l10n.authErrorsSessionExpired,
      AuthErrorCodes.badRequest => l10n.authErrorsBadRequest,
      // The backend returns a specific reason (e.g. duplicate email/phone);
      // prefer it over the generic fallback.
      AuthErrorCodes.registrationFailed =>
        failure.message.isNotEmpty
            ? failure.message
            : l10n.authErrorsBadRequest,
      AuthErrorCodes.network => l10n.authErrorsNetwork,
      AuthErrorCodes.rateLimited => l10n.authErrorsRateLimited,
      AuthErrorCodes.server => l10n.authErrorsServer,
      AuthErrorCodes.notFound => l10n.authErrorsNetwork,
      AuthErrorCodes.unknown =>
        failure.message.isNotEmpty ? failure.message : l10n.authErrorsUnknown,
      _ =>
        failure.message.isNotEmpty ? failure.message : l10n.authErrorsUnknown,
    };
  }
}

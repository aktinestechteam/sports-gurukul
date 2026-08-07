import 'package:flutter/material.dart';

import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/error/create_academy_error_mapper.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Localizes create-academy validation and API failures.
///
/// Validation maps the core validator error codes to l10n strings; API
/// failures map the stable [BaseFailure.code] set from
/// [CreateAcademyErrorMapper] and fall back to the server's detail message
/// when no code is matched.
abstract final class AcademyCreateMessages {
  /// Localized text for a [ValidationError], or null when valid.
  static String? validation(BuildContext context, ValidationError? error) {
    if (error == null) {
      return null;
    }
    final l10n = AppLocalizations.of(context);
    return switch (error.code) {
      'validation.required' => l10n.validationRequired,
      'validation.email.invalid' => l10n.validationEmailInvalid,
      'validation.phone.invalid' => l10n.validationPhoneInvalid,
      'validation.url.invalid' => l10n.validationUrlInvalid,
      'validation.postalCode.invalid' => l10n.validationPostalCodeInvalid,
      'academy.validation.sport' => l10n.academySelectAtLeastOneSport,
      'academy.validation.logo' => l10n.academyLogoRequired,
      _ => error.code,
    };
  }

  /// Localized user-facing text for an API [failure].
  static String failure(BuildContext context, BaseFailure failure) {
    final l10n = AppLocalizations.of(context);
    return switch (failure.code) {
      CreateAcademyErrorCodes.network => l10n.academyErrorsNetwork,
      CreateAcademyErrorCodes.server => l10n.academyErrorsServer,
      CreateAcademyErrorCodes.validation => l10n.academyErrorsValidation,
      CreateAcademyErrorCodes.permissionDenied => l10n.academyErrorsPermission,
      _ =>
        failure.message.isNotEmpty
            ? failure.message
            : l10n.academyErrorsUnknown,
    };
  }
}

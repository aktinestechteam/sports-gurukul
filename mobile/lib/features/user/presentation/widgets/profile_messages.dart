import 'package:flutter/material.dart';

import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/features/user/infrastructure/error/user_profile_error_mapper.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Localizes user-profile validation and API failures.
///
/// Validation maps the core validator error codes to l10n strings; API
/// failures map the stable [BaseFailure.code] set from
/// [UserProfileErrorMapper] and fall back to the server's detail message when
/// no code is matched.
abstract final class ProfileMessages {
  /// Localized text for a [ValidationError], or null when valid.
  static String? validation(BuildContext context, ValidationError? error) {
    if (error == null) {
      return null;
    }
    final l10n = AppLocalizations.of(context);
    return switch (error.code) {
      'validation.required' => l10n.validationRequired,
      'validation.phone.invalid' => l10n.validationPhoneInvalid,
      'validation.date.invalid' => l10n.validationDateInvalid,
      'validation.date.future' => l10n.validationDateFuture,
      'validation.number.invalid' => l10n.validationNumberInvalid,
      'validation.postalCode.invalid' => l10n.validationPostalCodeInvalid,
      _ => error.code,
    };
  }

  /// Localized user-facing text for an API [failure].
  static String failure(BuildContext context, BaseFailure failure) {
    final l10n = AppLocalizations.of(context);
    return switch (failure.code) {
      UserProfileErrorCodes.notFound => l10n.profileErrorsNotFound,
      UserProfileErrorCodes.network => l10n.profileErrorsNetwork,
      UserProfileErrorCodes.server => l10n.profileErrorsServer,
      UserProfileErrorCodes.validation => l10n.profileErrorsValidation,
      UserProfileErrorCodes.photoUploadFailed => l10n.profileErrorsPhotoUpload,
      UserProfileErrorCodes.photoNotFound => l10n.profileErrorsPhotoNotFound,
      UserProfileErrorCodes.permissionDenied => l10n.profileErrorsPermission,
      UserProfileErrorCodes.unknown =>
        failure.message.isNotEmpty
            ? failure.message
            : l10n.profileErrorsUnknown,
      _ =>
        failure.message.isNotEmpty
            ? failure.message
            : l10n.profileErrorsUnknown,
    };
  }
}

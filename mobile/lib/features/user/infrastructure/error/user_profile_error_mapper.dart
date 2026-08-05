import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';

/// Failure codes surfaced to the UI. Each maps to an l10n key.
abstract final class UserProfileErrorCodes {
  static const String notFound = 'profile.errors.notFound';
  static const String network = 'profile.errors.network';
  static const String server = 'profile.errors.server';
  static const String validation = 'profile.errors.validation';
  static const String photoUploadFailed = 'profile.errors.photoUploadFailed';
  static const String photoNotFound = 'profile.errors.photoNotFound';
  static const String permissionDenied = 'profile.errors.permissionDenied';
  static const String unknown = 'profile.errors.unknown';
}

/// Operation identifier attached to [ApiException] by the datasource.
abstract final class UserProfileOperations {
  static const String getCurrentProfile = 'user.getCurrentProfile';
  static const String updateProfile = 'user.updateProfile';
  static const String updatePreferences = 'user.updatePreferences';
  static const String uploadPhoto = 'user.uploadPhoto';
  static const String getPhoto = 'user.getPhoto';
  static const String deletePhoto = 'user.deletePhoto';
}

/// Maps user-profile [ApiException]s to typed [BaseFailure]s for the UI.
abstract final class UserProfileErrorMapper {
  static BaseFailure map(ApiException error) {
    final status = error.statusCode;

    if (status == null) {
      return NetworkFailure(
        message: error.message,
        code: UserProfileErrorCodes.network,
        cause: error,
      );
    }

    return switch (status) {
      >= 500 => ServerFailure(
        message: error.message,
        code: UserProfileErrorCodes.server,
        cause: error,
      ),
      429 => NetworkFailure(
        message: error.message,
        code: UserProfileErrorCodes.network,
        cause: error,
      ),
      404 => NetworkFailure(
        message: error.message,
        code: UserProfileErrorCodes.notFound,
        cause: error,
      ),
      400 || 422 => ValidationFailure(
        message: error.message,
        code: UserProfileErrorCodes.validation,
        cause: error,
      ),
      403 => PermissionFailure(
        message: error.message,
        code: UserProfileErrorCodes.permissionDenied,
        cause: error,
      ),
      _ => UnknownFailure(
        message: error.message,
        code: UserProfileErrorCodes.unknown,
        cause: error,
      ),
    };
  }
}

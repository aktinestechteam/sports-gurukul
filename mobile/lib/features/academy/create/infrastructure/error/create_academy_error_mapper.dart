import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';

/// Failure codes surfaced to the UI. Each maps to an l10n key.
abstract final class CreateAcademyErrorCodes {
  static const String network = 'academy.errors.network';
  static const String server = 'academy.errors.server';
  static const String validation = 'academy.errors.validation';
  static const String permissionDenied = 'academy.errors.permission';
  static const String unknown = 'academy.errors.unknown';
}

/// Operation identifier attached to [ApiException] by the datasource.
abstract final class CreateAcademyOperations {
  static const String createAcademy = 'academy.createAcademy';
  static const String getAcademy = 'academy.getAcademy';
  static const String getMyAcademy = 'academy.getMyAcademy';
  static const String updateAcademy = 'academy.updateAcademy';
  static const String updateContact = 'academy.updateContact';
  static const String uploadLogo = 'academy.uploadLogo';
  static const String uploadBanner = 'academy.uploadBanner';
}

/// Maps academy-creation [ApiException]s to typed [BaseFailure]s for the UI.
abstract final class CreateAcademyErrorMapper {
  static BaseFailure map(ApiException error) {
    final status = error.statusCode;

    if (status == null) {
      return NetworkFailure(
        message: error.message,
        code: CreateAcademyErrorCodes.network,
        cause: error,
      );
    }

    return switch (status) {
      >= 500 => ServerFailure(
        message: error.message,
        code: CreateAcademyErrorCodes.server,
        cause: error,
      ),
      429 => NetworkFailure(
        message: error.message,
        code: CreateAcademyErrorCodes.network,
        cause: error,
      ),
      400 || 422 => ValidationFailure(
        message: error.message,
        code: CreateAcademyErrorCodes.validation,
        cause: error,
      ),
      403 => PermissionFailure(
        message: error.message,
        code: CreateAcademyErrorCodes.permissionDenied,
        cause: error,
      ),
      _ => UnknownFailure(
        message: error.message,
        code: CreateAcademyErrorCodes.unknown,
        cause: error,
      ),
    };
  }
}

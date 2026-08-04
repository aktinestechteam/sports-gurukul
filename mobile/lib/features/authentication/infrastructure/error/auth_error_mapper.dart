import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';

/// Failure codes surfaced to the UI. Each maps to an l10n key.
abstract final class AuthErrorCodes {
  static const String invalidCredentials = 'auth.errors.invalidCredentials';
  static const String accountLocked = 'auth.errors.accountLocked';
  static const String sessionExpired = 'auth.errors.sessionExpired';
  static const String badRequest = 'auth.errors.badRequest';
  static const String registrationFailed = 'auth.errors.registrationFailed';
  static const String notFound = 'auth.errors.notFound';
  static const String network = 'auth.errors.network';
  static const String rateLimited = 'auth.errors.rateLimited';
  static const String server = 'auth.errors.server';
  static const String unknown = 'auth.errors.unknown';
}

/// Operation identifier attached to [ApiException] by the auth datasource.
abstract final class AuthOperations {
  static const String login = 'auth.login';
  static const String register = 'auth.register';
  static const String refresh = 'auth.refresh';
  static const String logout = 'auth.logout';
  static const String forgotPassword = 'auth.forgotPassword';
  static const String resetPassword = 'auth.resetPassword';
  static const String sendVerificationEmail = 'auth.sendVerificationEmail';
  static const String verifyEmail = 'auth.verifyEmail';
}

/// Maps auth [ApiException]s to typed [BaseFailure]s for the UI.
///
/// The failure carries a stable l10n [BaseFailure.code] plus the server's
/// detail message for diagnostics. 401s are classified by the operation that
/// produced them: login failures mean bad credentials or a locked account,
/// while refresh failures mean the session has expired.
abstract final class AuthErrorMapper {
  static BaseFailure map(ApiException error) {
    final operation = error.code;
    final status = error.statusCode;

    if (status == null) {
      return NetworkFailure(
        message: error.message,
        code: AuthErrorCodes.network,
        cause: error,
      );
    }

    if (status == 401) {
      return _mapUnauthorized(error, operation);
    }

    // Register failures (duplicate email/phone, rejected details) carry a
    // server-specific message that should reach the user instead of the
    // generic bad-request copy.
    if (status == 400 && operation == AuthOperations.register) {
      return ValidationFailure(
        message: error.message,
        code: AuthErrorCodes.registrationFailed,
        cause: error,
      );
    }

    return switch (status) {
      >= 500 => ServerFailure(
        message: error.message,
        code: AuthErrorCodes.server,
        cause: error,
      ),
      429 => NetworkFailure(
        message: error.message,
        code: AuthErrorCodes.rateLimited,
        cause: error,
      ),
      404 => NetworkFailure(
        message: error.message,
        code: AuthErrorCodes.notFound,
        cause: error,
      ),
      400 => ValidationFailure(
        message: error.message,
        code: AuthErrorCodes.badRequest,
        cause: error,
      ),
      403 => PermissionFailure(
        message: error.message,
        code: AuthErrorCodes.badRequest,
        cause: error,
      ),
      _ => UnknownFailure(
        message: error.message,
        code: AuthErrorCodes.unknown,
        cause: error,
      ),
    };
  }

  static BaseFailure _mapUnauthorized(ApiException error, String? operation) {
    if (operation == AuthOperations.refresh) {
      return AuthenticationFailure(
        message: error.message,
        code: AuthErrorCodes.sessionExpired,
        cause: error,
      );
    }
    final isLocked = error.message.toLowerCase().contains('locked');
    return AuthenticationFailure(
      message: error.message,
      code: isLocked
          ? AuthErrorCodes.accountLocked
          : AuthErrorCodes.invalidCredentials,
      cause: error,
    );
  }
}

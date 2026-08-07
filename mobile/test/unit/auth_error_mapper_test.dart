import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/error/auth_error_mapper.dart';

void main() {
  ApiException error({
    int? statusCode,
    String? code,
    String message = 'boom',
  }) => ApiException(
    statusCode: statusCode,
    code: code,
    message: message,
  );

  group('AuthErrorMapper', () {
    test('maps a missing status to a network failure', () {
      final failure = AuthErrorMapper.map(error(code: AuthOperations.login));
      expect(failure, isA<NetworkFailure>());
      expect(failure.code, AuthErrorCodes.network);
    });

    test('maps login 401 without lock details to invalid credentials', () {
      final failure = AuthErrorMapper.map(
        error(statusCode: 401, code: AuthOperations.login),
      );
      expect(failure, isA<AuthenticationFailure>());
      expect(failure.code, AuthErrorCodes.invalidCredentials);
    });

    test('maps login 401 with a locked account message', () {
      final failure = AuthErrorMapper.map(
        error(
          statusCode: 401,
          code: AuthOperations.login,
          message: 'Account is locked. Try again in 15 minutes.',
        ),
      );
      expect(failure, isA<AuthenticationFailure>());
      expect(failure.code, AuthErrorCodes.accountLocked);
    });

    test('maps refresh 401 to a session expired failure', () {
      final failure = AuthErrorMapper.map(
        error(statusCode: 401, code: AuthOperations.refresh),
      );
      expect(failure, isA<AuthenticationFailure>());
      expect(failure.code, AuthErrorCodes.sessionExpired);
    });

    test('maps server errors', () {
      final failure = AuthErrorMapper.map(
        error(statusCode: 503, code: AuthOperations.login),
      );
      expect(failure, isA<ServerFailure>());
      expect(failure.code, AuthErrorCodes.server);
    });

    test('maps rate limiting', () {
      final failure = AuthErrorMapper.map(
        error(statusCode: 429, code: AuthOperations.login),
      );
      expect(failure, isA<NetworkFailure>());
      expect(failure.code, AuthErrorCodes.rateLimited);
    });

    test('maps missing resources', () {
      final failure = AuthErrorMapper.map(
        error(statusCode: 404, code: AuthOperations.resetPassword),
      );
      expect(failure, isA<NetworkFailure>());
      expect(failure.code, AuthErrorCodes.notFound);
    });

    test('maps bad requests', () {
      final failure = AuthErrorMapper.map(
        error(statusCode: 400, code: AuthOperations.resetPassword),
      );
      expect(failure, isA<ValidationFailure>());
      expect(failure.code, AuthErrorCodes.badRequest);
    });

    test('maps register 400 to a registration failure with server message', () {
      final failure = AuthErrorMapper.map(
        error(
          statusCode: 400,
          code: AuthOperations.register,
          message: 'An account with this email already exists.',
        ),
      );
      expect(failure, isA<ValidationFailure>());
      expect(failure.code, AuthErrorCodes.registrationFailed);
      expect(failure.message, 'An account with this email already exists.');
    });

    test('maps forbidden responses', () {
      final failure = AuthErrorMapper.map(
        error(statusCode: 403, code: AuthOperations.register),
      );
      expect(failure, isA<PermissionFailure>());
    });

    test('falls back to unknown for unexpected statuses', () {
      final failure = AuthErrorMapper.map(
        error(statusCode: 418, code: AuthOperations.login),
      );
      expect(failure, isA<UnknownFailure>());
      expect(failure.code, AuthErrorCodes.unknown);
    });
  });
}

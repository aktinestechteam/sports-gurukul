import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/features/user/infrastructure/error/user_profile_error_mapper.dart';

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

  group('UserProfileErrorMapper', () {
    test('maps a missing status to a network failure', () {
      final failure = UserProfileErrorMapper.map(
        error(code: UserProfileOperations.getCurrentProfile),
      );
      expect(failure, isA<NetworkFailure>());
      expect(failure.code, UserProfileErrorCodes.network);
    });

    test('maps a missing profile to a not-found failure', () {
      final failure = UserProfileErrorMapper.map(
        error(statusCode: 404, code: UserProfileOperations.getCurrentProfile),
      );
      expect(failure, isA<NetworkFailure>());
      expect(failure.code, UserProfileErrorCodes.notFound);
    });

    test('maps server errors', () {
      final failure = UserProfileErrorMapper.map(
        error(statusCode: 503, code: UserProfileOperations.updateProfile),
      );
      expect(failure, isA<ServerFailure>());
      expect(failure.code, UserProfileErrorCodes.server);
    });

    test('maps rate limiting to a network failure', () {
      final failure = UserProfileErrorMapper.map(
        error(statusCode: 429, code: UserProfileOperations.updatePreferences),
      );
      expect(failure, isA<NetworkFailure>());
      expect(failure.code, UserProfileErrorCodes.network);
    });

    test('maps bad requests and unprocessable bodies to validation', () {
      for (final statusCode in <int>[400, 422]) {
        final failure = UserProfileErrorMapper.map(
          error(
            statusCode: statusCode,
            code: UserProfileOperations.updateProfile,
          ),
        );
        expect(failure, isA<ValidationFailure>(), reason: 'status $statusCode');
        expect(failure.code, UserProfileErrorCodes.validation);
      }
    });

    test('maps forbidden responses to a permission failure', () {
      final failure = UserProfileErrorMapper.map(
        error(statusCode: 403, code: UserProfileOperations.uploadPhoto),
      );
      expect(failure, isA<PermissionFailure>());
      expect(failure.code, UserProfileErrorCodes.permissionDenied);
    });

    test('falls back to unknown for unexpected statuses', () {
      final failure = UserProfileErrorMapper.map(
        error(statusCode: 418, code: UserProfileOperations.getPhoto),
      );
      expect(failure, isA<UnknownFailure>());
      expect(failure.code, UserProfileErrorCodes.unknown);
    });

    test('preserves the server message and operation code', () {
      final failure = UserProfileErrorMapper.map(
        error(
          statusCode: 404,
          code: UserProfileOperations.getPhoto,
          message: 'No profile photo exists',
        ),
      );
      expect(failure.message, 'No profile photo exists');
      expect(failure.code, UserProfileErrorCodes.notFound);
    });
  });
}

import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/error/create_academy_error_mapper.dart';

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

  group('CreateAcademyErrorMapper', () {
    test('maps a missing status to a network failure', () {
      final failure = CreateAcademyErrorMapper.map(
        error(code: CreateAcademyOperations.createAcademy),
      );
      expect(failure, isA<NetworkFailure>());
      expect(failure.code, CreateAcademyErrorCodes.network);
    });

    test('maps server errors', () {
      final failure = CreateAcademyErrorMapper.map(
        error(statusCode: 503, code: CreateAcademyOperations.createAcademy),
      );
      expect(failure, isA<ServerFailure>());
      expect(failure.code, CreateAcademyErrorCodes.server);
    });

    test('maps rate limiting to a network failure', () {
      final failure = CreateAcademyErrorMapper.map(
        error(statusCode: 429, code: CreateAcademyOperations.createAcademy),
      );
      expect(failure, isA<NetworkFailure>());
      expect(failure.code, CreateAcademyErrorCodes.network);
    });

    test('maps bad requests and unprocessable bodies to validation', () {
      for (final statusCode in <int>[400, 422]) {
        final failure = CreateAcademyErrorMapper.map(
          error(
            statusCode: statusCode,
            code: CreateAcademyOperations.createAcademy,
          ),
        );
        expect(failure, isA<ValidationFailure>(), reason: 'status $statusCode');
        expect(failure.code, CreateAcademyErrorCodes.validation);
      }
    });

    test('maps forbidden responses to a permission failure', () {
      final failure = CreateAcademyErrorMapper.map(
        error(statusCode: 403, code: CreateAcademyOperations.createAcademy),
      );
      expect(failure, isA<PermissionFailure>());
      expect(failure.code, CreateAcademyErrorCodes.permissionDenied);
    });

    test('falls back to unknown for unexpected statuses', () {
      final failure = CreateAcademyErrorMapper.map(
        error(statusCode: 418, code: CreateAcademyOperations.getAcademy),
      );
      expect(failure, isA<UnknownFailure>());
      expect(failure.code, CreateAcademyErrorCodes.unknown);
    });

    test('preserves the server message and operation code', () {
      final failure = CreateAcademyErrorMapper.map(
        error(
          statusCode: 400,
          code: CreateAcademyOperations.createAcademy,
          message: 'Name is required',
        ),
      );
      expect(failure.message, 'Name is required');
      expect(failure.code, CreateAcademyErrorCodes.validation);
    });
  });
}

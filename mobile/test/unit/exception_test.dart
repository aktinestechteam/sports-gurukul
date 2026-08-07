import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/exceptions/app_exception.dart';

void main() {
  group('AppException hierarchy', () {
    test('defaults provide a message and null cause', () {
      const exception = CacheException();
      expect(exception.message, isNotEmpty);
      expect(exception.cause, isNull);
      expect(exception, isA<Exception>());
    });

    test('custom fields propagate', () {
      final cause = StateError('root');
      final exception = ApiException(
        message: 'boom',
        statusCode: 500,
        code: 'internal_error',
        cause: cause,
      );
      expect(exception.message, 'boom');
      expect(exception.statusCode, 500);
      expect(exception.code, 'internal_error');
      expect(exception.cause, cause);
    });

    test('ApiException exposes statusCode and code', () {
      const exception = ApiException(statusCode: 401, code: 'unauthorized');
      expect(exception.statusCode, 401);
      expect(exception.code, 'unauthorized');
    });

    test('each concrete type has a distinct runtimeType', () {
      final exceptions = <AppException>[
        const ApiException(),
        const CacheException(),
        const StorageException(),
        const TimeoutException(),
        const ParsingException(),
      ];
      expect(
        exceptions.map((e) => e.runtimeType).toSet().length,
        exceptions.length,
      );
    });

    test('toString includes the message', () {
      const exception = TimeoutException();
      expect(exception.toString(), contains('TimeoutException'));
    });
  });

  test('switch over AppException is exhaustive', () {
    const AppException exception = ParsingException();
    final label = switch (exception) {
      ApiException() => 'api',
      CacheException() => 'cache',
      StorageException() => 'storage',
      TimeoutException() => 'timeout',
      ParsingException() => 'parsing',
    };
    expect(label, 'parsing');
  });
}

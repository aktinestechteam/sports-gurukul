import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';

void main() {
  const causes = Object();

  group('BaseFailure hierarchy', () {
    test('defaults provide a message and no code', () {
      const failure = UnknownFailure();
      expect(failure.message, isNotEmpty);
      expect(failure.code, isNull);
      expect(failure.cause, isNull);
    });

    test('custom fields propagate', () {
      const failure = ValidationFailure(
        message: 'custom',
        code: 'validation.email.invalid',
        cause: causes,
      );
      expect(failure.message, 'custom');
      expect(failure.code, 'validation.email.invalid');
      expect(failure.cause, causes);
    });

    test('each concrete type has a distinct runtimeType', () {
      final failures = <BaseFailure>[
        const ValidationFailure(),
        const AuthenticationFailure(),
        const NetworkFailure(),
        const ServerFailure(),
        const PermissionFailure(),
        const StorageFailure(),
        const CacheFailure(),
        const UnknownFailure(),
      ];
      expect(
        failures.map((f) => f.runtimeType).toSet().length,
        failures.length,
      );
    });

    test('toString includes message and code', () {
      const failure = NetworkFailure(code: 'net.timeout');
      expect(failure.toString(), contains('NetworkFailure'));
      expect(failure.toString(), contains('net.timeout'));
    });
  });

  test('switch over BaseFailure is exhaustive', () {
    const BaseFailure failure = ServerFailure();
    final label = switch (failure) {
      ValidationFailure() => 'validation',
      AuthenticationFailure() => 'auth',
      NetworkFailure() => 'network',
      ServerFailure() => 'server',
      PermissionFailure() => 'permission',
      StorageFailure() => 'storage',
      CacheFailure() => 'cache',
      UnknownFailure() => 'unknown',
    };
    expect(label, 'server');
  });
}

import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/core/result/result.dart';

void main() {
  const value = 42;
  const failure = UnknownFailure(code: 'unit.error');

  group('Result<T>', () {
    test('Result.success and Success are equivalent', () {
      const result = Result<int>.success(value);
      expect(result, isA<Success<int>>());
      expect(result.isSuccess, isTrue);
      expect(result.isFailure, isFalse);
      expect(result.valueOrNull, value);
      expect(result.failureOrNull, isNull);
    });

    test('Result.failure and FailureResult are equivalent', () {
      const result = Result<int>.failure(failure);
      expect(result, isA<FailureResult<int>>());
      expect(result.isSuccess, isFalse);
      expect(result.isFailure, isTrue);
      expect(result.valueOrNull, isNull);
      expect(result.failureOrNull, failure);
    });

    test('fold reduces to the value on success', () {
      const result = Result<int>.success(value);
      expect(
        result.fold((v) => v * 2, (f) => -1),
        84,
      );
    });

    test('fold reduces to the failure result on failure', () {
      const result = Result<int>.failure(failure);
      expect(
        result.fold((v) => v * 2, (f) => f.code),
        'unit.error',
      );
    });

    test('when is an ergonomic alias of fold', () {
      const result = Result<int>.success(value);
      expect(
        result.when(onSuccess: (v) => 'value $v', onFailure: (f) => 'failed'),
        'value 42',
      );
    });

    test('map transforms only the success value', () {
      final ok = (const Result<int>.success(value)).map((v) => v.toString());
      final failed = (const Result<int>.failure(failure)).map(
        (v) => v.toString(),
      );

      expect(ok, const Result<String>.success('42'));
      expect(failed, isA<FailureResult<String>>());
      expect(failed.failureOrNull, failure);
    });

    test('onSuccess and onFailure run the matching side effect', () {
      final okCalls = <int>[];
      final failCalls = <String>[];

      (const Result<int>.success(
        value,
      )).onSuccess(okCalls.add).onFailure((f) => failCalls.add(f.code!));
      (const Result<int>.failure(
        failure,
      )).onSuccess(okCalls.add).onFailure((f) => failCalls.add(f.code!));

      expect(okCalls, [42]);
      expect(failCalls, ['unit.error']);
    });

    test('recoverWith returns a fallback value on failure', () {
      final recovered = (const Result<int>.failure(failure)).recoverWith(
        (f) => -1,
      );
      expect(recovered, isA<Success<int>>());
      expect(recovered.valueOrNull, -1);
    });

    test('requireValue returns the value or throws', () {
      expect((const Result<int>.success(value)).requireValue(), value);
      expect(
        () => (const Result<int>.failure(failure)).requireValue(),
        throwsStateError,
      );
    });

    test('success and failure compare by value', () {
      expect(
        const Result<int>.success(value),
        const Result<int>.success(value),
      );
      expect(
        const Result<int>.success(1),
        isNot(const Result<int>.success(2)),
      );
      expect(
        const Result<int>.failure(failure),
        const Result<int>.failure(failure),
      );
      expect(
        const Result<int>.success(value),
        isNot(const Result<int>.failure(failure)),
      );
    });

    test('switch over Result is exhaustive', () {
      const result = Result<int>.success(value);
      final label = switch (result) {
        Success<int>(:final value) => 'ok:$value',
        FailureResult<int>() => 'err',
      };
      expect(label, 'ok:42');
    });
  });

  group('OperationResult', () {
    test('success variant is reported correctly', () {
      const result = OperationResult.success();
      expect(result.isSuccess, isTrue);
      expect(result.isFailure, isFalse);
      expect(result.failureOrNull, isNull);
    });

    test('failure variant carries the failure', () {
      const result = OperationResult.failure(failure);
      expect(result.isSuccess, isFalse);
      expect(result.isFailure, isTrue);
      expect(result.failureOrNull, failure);
    });

    test('fold reduces either variant', () {
      expect(
        const OperationResult.success().fold(() => 1, (_) => 0),
        1,
      );
      expect(
        (const OperationResult.failure(failure)).fold(() => 1, (_) => 0),
        0,
      );
    });

    test('onSuccess and onFailure run the matching side effect', () {
      var succeeded = false;
      BaseFailure? reported;

      const OperationResult.success().onSuccess(() => succeeded = true);
      (const OperationResult.failure(failure)).onFailure((f) => reported = f);

      expect(succeeded, isTrue);
      expect(reported, failure);
    });
  });
}

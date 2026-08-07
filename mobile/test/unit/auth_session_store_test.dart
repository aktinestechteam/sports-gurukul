import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sports_gurukul/core/constants/storage_keys.dart';
import 'package:sports_gurukul/core/storage/secure_storage.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/session/auth_session_store.dart';

import '../helpers/auth_test_helper.dart';
import '../mocks/flutter_secure_storage_mock.dart';

void main() {
  late FlutterSecureStorageMock storage;
  late AuthSessionStore store;

  setUp(() {
    storage = FlutterSecureStorageMock();
    store = AuthSessionStore(storage: SecureStorage(storage: storage));
  });

  void stubWrite({void Function(String value)? onWrite}) {
    when(
      () => storage.write(
        key: any(named: 'key'),
        value: any(named: 'value'),
      ),
    ).thenAnswer((invocation) async {
      onWrite?.call(invocation.namedArguments[#value] as String);
    });
  }

  test('read returns null when nothing is stored', () async {
    when(() => storage.read(key: any(named: 'key'))).thenAnswer(
      (_) async => null,
    );
    expect(await store.read(), isNull);
  });

  test('read returns null for corrupt JSON', () async {
    when(() => storage.read(key: any(named: 'key'))).thenAnswer(
      (_) async => 'not-json',
    );
    expect(await store.read(), isNull);
  });

  test('write then read round-trips a session', () async {
    final session = testAuthSession();
    String? persisted;
    stubWrite(onWrite: (value) => persisted = value);
    when(() => storage.read(key: any(named: 'key'))).thenAnswer(
      (_) async => persisted,
    );

    await store.write(session);
    final restored = await store.read();

    expect(restored, session);
    expect(restored, isNotNull);
    expect(restored!.user.roles, const <String>['Player']);
  });

  test('restores a session with seven-digit fractional expiry', () async {
    final payload = <String, Object?>{
      'userId': 'user-1',
      'email': 'player@example.com',
      'fullName': 'Test Player',
      'roles': <String>['Player'],
      'accessToken': 'access-token',
      'refreshToken': 'refresh-token',
      'accessTokenExpiresAt': '2099-01-01T00:00:00.1234567Z',
    };
    when(() => storage.read(key: any(named: 'key'))).thenAnswer(
      (_) async => jsonEncode(payload),
    );

    final restored = await store.read();

    expect(restored, isNotNull);
    expect(
      restored!.accessTokenExpiresAt,
      DateTime.utc(2099, 1, 1, 0, 0, 0, 0, 123456),
    );
    expect(restored.hasExpiredAccessToken, isFalse);
  });

  test('clear removes the session entry', () async {
    when(() => storage.delete(key: any(named: 'key'))).thenAnswer(
      (_) async {},
    );

    await store.clear();

    verify(
      () => storage.delete(key: StorageKeys.authSession),
    ).called(1);
  });

  test('AuthSession hasExpiredAccessToken reflects the expiry', () {
    final session = testAuthSession();
    expect(session.hasExpiredAccessToken, isFalse);

    final expired = AuthSession(
      user: session.user,
      accessToken: session.accessToken,
      refreshToken: session.refreshToken,
      accessTokenExpiresAt: DateTime.utc(2020),
    );
    expect(expired.hasExpiredAccessToken, isTrue);
  });
}

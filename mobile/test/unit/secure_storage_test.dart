import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:sports_gurukul/core/storage/secure_storage.dart';

import '../mocks/flutter_secure_storage_mock.dart';

void main() {
  late FlutterSecureStorageMock backing;
  late SecureStorage storage;

  setUp(() {
    backing = FlutterSecureStorageMock();
    storage = SecureStorage(storage: backing);
  });

  group('SecureStorage', () {
    test('read delegates to the underlying store', () async {
      when(() => backing.read(key: 'token')).thenAnswer((_) async => 'abc');

      expect(await storage.read('token'), 'abc');
      verify(() => backing.read(key: 'token')).called(1);
    });

    test('write delegates to the underlying store', () async {
      when(
        () => backing.write(key: 'token', value: 'abc'),
      ).thenAnswer((_) async {});

      await storage.write('token', 'abc');
      verify(() => backing.write(key: 'token', value: 'abc')).called(1);
    });

    test('delete delegates to the underlying store', () async {
      when(() => backing.delete(key: 'token')).thenAnswer((_) async {});

      await storage.delete('token');
      verify(() => backing.delete(key: 'token')).called(1);
    });

    test('clear delegates to deleteAll', () async {
      when(() => backing.deleteAll()).thenAnswer((_) async {});

      await storage.clear();
      verify(() => backing.deleteAll()).called(1);
    });
  });
}

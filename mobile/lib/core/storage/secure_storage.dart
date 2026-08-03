import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// Secure, encrypted key-value storage.
///
/// Intended for tokens, credentials and other secrets only - never for
/// general preferences (see `PreferenceStorage`). Non-sensitive settings must
/// go through shared preferences so they survive re-install and stay out of
/// the platform keychain.
class SecureStorage {
  SecureStorage({FlutterSecureStorage? storage})
    : _storage = storage ?? const FlutterSecureStorage();

  final FlutterSecureStorage _storage;

  /// Reads a value by [key], or `null` when absent.
  Future<String?> read(String key) => _storage.read(key: key);

  /// Writes [value] under [key].
  Future<void> write(String key, String value) =>
      _storage.write(key: key, value: value);

  /// Removes the entry stored under [key].
  Future<void> delete(String key) => _storage.delete(key: key);

  /// Removes every entry held in secure storage.
  Future<void> clear() => _storage.deleteAll();
}

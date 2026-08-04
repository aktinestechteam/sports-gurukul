import 'package:sports_gurukul/core/constants/storage_keys.dart';
import 'package:sports_gurukul/core/storage/secure_storage.dart';

/// Reads and writes the JWT token pair in secure storage.
///
/// The access and refresh tokens are the only credentials kept on device and
/// must never leave the platform keychain. The `AuthInterceptor` reads from
/// this store to attach the bearer token; the auth feature writes to it after
/// login, register and refresh.
class TokenStore {
  TokenStore({required SecureStorage storage}) : _storage = storage;

  final SecureStorage _storage;

  /// The current access token, or `null` when none is stored.
  Future<String?> readAccessToken() => _storage.read(StorageKeys.authToken);

  /// The current refresh token, or `null` when none is stored.
  Future<String?> readRefreshToken() => _storage.read(StorageKeys.refreshToken);

  /// Persists the token pair after a successful authentication or refresh.
  Future<void> writeTokens({
    required String accessToken,
    required String refreshToken,
  }) async {
    await _storage.write(StorageKeys.authToken, accessToken);
    await _storage.write(StorageKeys.refreshToken, refreshToken);
  }

  /// Removes both tokens, invalidating the local session.
  Future<void> clear() async {
    await _storage.delete(StorageKeys.authToken);
    await _storage.delete(StorageKeys.refreshToken);
  }
}

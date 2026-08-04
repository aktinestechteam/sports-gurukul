import 'dart:convert';

import 'package:sports_gurukul/core/constants/storage_keys.dart';
import 'package:sports_gurukul/core/storage/secure_storage.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_user.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/date_time_converter.dart';

/// Persists the authenticated session in secure storage for auto-login.
///
/// The full session (identity + tokens) is cached so the app can restore it
/// without a network round trip; token refreshes update the cache in place.
class AuthSessionStore {
  AuthSessionStore({required SecureStorage storage}) : _storage = storage;

  final SecureStorage _storage;

  /// The persisted session, or `null` when the user is signed out.
  Future<AuthSession?> read() async {
    final raw = await _storage.read(StorageKeys.authSession);
    if (raw == null || raw.isEmpty) {
      return null;
    }
    try {
      final json = jsonDecode(raw);
      if (json is! Map<String, dynamic>) {
        return null;
      }
      return _fromJson(json);
    } on Object {
      return null;
    }
  }

  /// Persists [session] for future auto-login.
  Future<void> write(AuthSession session) =>
      _storage.write(StorageKeys.authSession, jsonEncode(_toJson(session)));

  /// Removes the persisted session.
  Future<void> clear() => _storage.delete(StorageKeys.authSession);

  static Map<String, dynamic> _toJson(AuthSession session) => <String, dynamic>{
    'userId': session.user.id,
    'email': session.user.email,
    'fullName': session.user.fullName,
    'roles': session.user.roles,
    'accessToken': session.accessToken,
    'refreshToken': session.refreshToken,
    'accessTokenExpiresAt': session.accessTokenExpiresAt
        .toUtc()
        .toIso8601String(),
  };

  static AuthSession _fromJson(Map<String, dynamic> json) {
    final expiresAt = json['accessTokenExpiresAt'];
    final parsedExpiry = expiresAt is String
        ? FlexibleDateTimeConverter.parse(expiresAt)
        : null;
    return AuthSession(
      user: AuthUser(
        id: json['userId'] as String,
        email: json['email'] as String,
        fullName: json['fullName'] as String,
        roles: (json['roles'] as List<dynamic>? ?? <dynamic>[])
            .whereType<String>()
            .toList(),
      ),
      accessToken: json['accessToken'] as String,
      refreshToken: json['refreshToken'] as String,
      accessTokenExpiresAt:
          parsedExpiry ?? DateTime.fromMillisecondsSinceEpoch(0),
    );
  }
}

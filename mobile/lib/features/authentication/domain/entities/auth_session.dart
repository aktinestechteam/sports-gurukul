import 'package:flutter/foundation.dart';

import 'package:sports_gurukul/features/authentication/domain/entities/auth_user.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/token_pair.dart';

/// A fully authenticated session: the user identity plus the live token pair.
///
/// Produced by login and registration (`AuthResponse`/`LoginResponse`
/// contract) and restored locally for auto-login. The access token expires
/// after [accessTokenExpiresAt]; a new pair is obtained through
/// [TokenPair]-returning refresh calls.
@immutable
class AuthSession {
  const AuthSession({
    required this.user,
    required this.accessToken,
    required this.refreshToken,
    required this.accessTokenExpiresAt,
  });

  /// The authenticated user.
  final AuthUser user;

  /// The JWT bearer token attached to authorized requests.
  final String accessToken;

  /// The rotation-capable refresh token.
  final String refreshToken;

  /// When [accessToken] stops being accepted by the server.
  final DateTime accessTokenExpiresAt;

  /// Whether the access token has already expired.
  bool get hasExpiredAccessToken =>
      !accessTokenExpiresAt.isAfter(DateTime.now());

  /// Returns a copy with the token pair replaced (used after a refresh).
  AuthSession withTokenPair(TokenPair pair) => AuthSession(
    user: user,
    accessToken: pair.accessToken,
    refreshToken: pair.refreshToken,
    accessTokenExpiresAt: pair.accessTokenExpiresAt,
  );

  @override
  bool operator ==(Object other) =>
      other is AuthSession &&
      other.user == user &&
      other.accessToken == accessToken &&
      other.refreshToken == refreshToken &&
      other.accessTokenExpiresAt == accessTokenExpiresAt;

  @override
  int get hashCode =>
      Object.hash(user, accessToken, refreshToken, accessTokenExpiresAt);

  @override
  String toString() =>
      'AuthSession(user: $user, accessTokenExpiresAt: $accessTokenExpiresAt)';
}

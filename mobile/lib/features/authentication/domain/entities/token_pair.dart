import 'package:flutter/foundation.dart';

/// An access/refresh token pair with the access token's expiry.
///
/// Mirrors the backend `TokenResponse` contract returned by the
/// `refresh-token` endpoint.
@immutable
class TokenPair {
  const TokenPair({
    required this.accessToken,
    required this.refreshToken,
    required this.accessTokenExpiresAt,
  });

  /// The new JWT bearer token.
  final String accessToken;

  /// The rotated refresh token.
  final String refreshToken;

  /// When the new access token expires.
  final DateTime accessTokenExpiresAt;

  @override
  bool operator ==(Object other) =>
      other is TokenPair &&
      other.accessToken == accessToken &&
      other.refreshToken == refreshToken &&
      other.accessTokenExpiresAt == accessTokenExpiresAt;

  @override
  int get hashCode =>
      Object.hash(accessToken, refreshToken, accessTokenExpiresAt);

  @override
  String toString() => 'TokenPair(accessTokenExpiresAt: $accessTokenExpiresAt)';
}

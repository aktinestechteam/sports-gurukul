import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_user.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/token_pair.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/auth_session_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/token_pair_dto.dart';

/// Converts auth DTOs into domain entities.
///
/// Mapping happens only at the repository boundary: DTOs never leave
/// infrastructure and domain entities never travel to the wire.
abstract final class AuthMappers {
  /// Maps an [AuthSessionDto] (login/register payload) to an [AuthSession].
  static AuthSession toSession(AuthSessionDto dto) => AuthSession(
    user: AuthUser(
      id: dto.userId,
      email: dto.email,
      fullName: dto.fullName,
      roles: dto.roles,
    ),
    accessToken: dto.accessToken,
    refreshToken: dto.refreshToken,
    accessTokenExpiresAt: dto.accessTokenExpiresAt,
  );

  /// Maps a [TokenPairDto] (refresh payload) to a [TokenPair].
  static TokenPair toTokenPair(TokenPairDto dto) => TokenPair(
    accessToken: dto.accessToken,
    refreshToken: dto.refreshToken,
    accessTokenExpiresAt: dto.accessTokenExpiresAt,
  );
}

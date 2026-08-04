import 'package:freezed_annotation/freezed_annotation.dart';

import 'package:sports_gurukul/features/authentication/infrastructure/models/date_time_converter.dart';

part 'auth_session_dto.freezed.dart';
part 'auth_session_dto.g.dart';

/// Response payload for `POST /api/v1/auth/login` and
/// `POST /api/v1/auth/register` (`AuthResponse`/`LoginResponse` contract).
@freezed
abstract class AuthSessionDto with _$AuthSessionDto {
  const factory AuthSessionDto({
    required String userId,
    required String email,
    required String fullName,
    required String accessToken,
    required String refreshToken,
    @FlexibleDateTimeConverter() required DateTime accessTokenExpiresAt,
    @Default(<String>[]) List<String> roles,
  }) = _AuthSessionDto;

  factory AuthSessionDto.fromJson(Map<String, dynamic> json) =>
      _$AuthSessionDtoFromJson(json);
}

import 'package:freezed_annotation/freezed_annotation.dart';

import 'package:sports_gurukul/features/authentication/infrastructure/models/date_time_converter.dart';

part 'token_pair_dto.freezed.dart';
part 'token_pair_dto.g.dart';

/// Response payload for `POST /api/v1/auth/refresh-token`
/// (`TokenResponse` contract).
@freezed
abstract class TokenPairDto with _$TokenPairDto {
  const factory TokenPairDto({
    required String accessToken,
    required String refreshToken,
    @FlexibleDateTimeConverter() required DateTime accessTokenExpiresAt,
  }) = _TokenPairDto;

  factory TokenPairDto.fromJson(Map<String, dynamic> json) =>
      _$TokenPairDtoFromJson(json);
}

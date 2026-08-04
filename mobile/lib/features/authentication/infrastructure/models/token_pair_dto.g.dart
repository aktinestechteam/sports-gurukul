// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'token_pair_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_TokenPairDto _$TokenPairDtoFromJson(Map<String, dynamic> json) =>
    _TokenPairDto(
      accessToken: json['accessToken'] as String,
      refreshToken: json['refreshToken'] as String,
      accessTokenExpiresAt: const FlexibleDateTimeConverter().fromJson(
        json['accessTokenExpiresAt'] as String,
      ),
    );

Map<String, dynamic> _$TokenPairDtoToJson(_TokenPairDto instance) =>
    <String, dynamic>{
      'accessToken': instance.accessToken,
      'refreshToken': instance.refreshToken,
      'accessTokenExpiresAt': const FlexibleDateTimeConverter().toJson(
        instance.accessTokenExpiresAt,
      ),
    };

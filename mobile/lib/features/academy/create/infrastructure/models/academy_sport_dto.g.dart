// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'academy_sport_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_AcademySportDto _$AcademySportDtoFromJson(Map<String, dynamic> json) =>
    _AcademySportDto(
      id: json['id'] as String?,
      sportId: json['sportId'] as String?,
      name: json['name'] as String?,
      code: json['code'] as String?,
      isPrimarySport: json['isPrimarySport'] as bool? ?? false,
    );

Map<String, dynamic> _$AcademySportDtoToJson(_AcademySportDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'sportId': instance.sportId,
      'name': instance.name,
      'code': instance.code,
      'isPrimarySport': instance.isPrimarySport,
    };

// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'sample_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_SampleModel _$SampleModelFromJson(Map<String, dynamic> json) => _SampleModel(
  id: json['id'] as String,
  name: json['name'] as String,
  count: (json['count'] as num?)?.toInt(),
);

Map<String, dynamic> _$SampleModelToJson(_SampleModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'count': instance.count,
    };

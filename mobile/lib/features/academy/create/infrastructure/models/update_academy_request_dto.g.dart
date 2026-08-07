// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_academy_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_UpdateAcademyRequestDto _$UpdateAcademyRequestDtoFromJson(
  Map<String, dynamic> json,
) => _UpdateAcademyRequestDto(
  name: json['name'] as String?,
  legalName: json['legalName'] as String?,
  description: json['description'] as String?,
  registrationNumber: json['registrationNumber'] as String?,
  gstNumber: json['gstNumber'] as String?,
  establishedDate: json['establishedDate'] as String?,
  website: json['website'] as String?,
  email: json['email'] as String?,
  phone: json['phone'] as String?,
);

Map<String, dynamic> _$UpdateAcademyRequestDtoToJson(
  _UpdateAcademyRequestDto instance,
) => <String, dynamic>{
  'name': instance.name,
  'legalName': instance.legalName,
  'description': instance.description,
  'registrationNumber': instance.registrationNumber,
  'gstNumber': instance.gstNumber,
  'establishedDate': instance.establishedDate,
  'website': instance.website,
  'email': instance.email,
  'phone': instance.phone,
};

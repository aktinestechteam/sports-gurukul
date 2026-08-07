// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_academy_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_CreateAcademyRequestDto _$CreateAcademyRequestDtoFromJson(
  Map<String, dynamic> json,
) => _CreateAcademyRequestDto(
  name: json['name'] as String,
  email: json['email'] as String,
  phone: json['phone'] as String,
  legalName: json['legalName'] as String?,
  description: json['description'] as String?,
  registrationNumber: json['registrationNumber'] as String?,
  gstNumber: json['gstNumber'] as String?,
  establishedDate: json['establishedDate'] as String?,
  website: json['website'] as String?,
  academyType: json['academyType'] as String?,
  sportNames:
      (json['sportNames'] as List<dynamic>?)
          ?.map((e) => e as String)
          .toList() ??
      const <String>[],
  primaryContactName: json['primaryContactName'] as String?,
  address: json['address'] as String?,
  country: json['country'] as String?,
  state: json['state'] as String?,
  city: json['city'] as String?,
  postalCode: json['postalCode'] as String?,
);

Map<String, dynamic> _$CreateAcademyRequestDtoToJson(
  _CreateAcademyRequestDto instance,
) => <String, dynamic>{
  'name': instance.name,
  'email': instance.email,
  'phone': instance.phone,
  'legalName': instance.legalName,
  'description': instance.description,
  'registrationNumber': instance.registrationNumber,
  'gstNumber': instance.gstNumber,
  'establishedDate': instance.establishedDate,
  'website': instance.website,
  'academyType': instance.academyType,
  'sportNames': instance.sportNames,
  'primaryContactName': instance.primaryContactName,
  'address': instance.address,
  'country': instance.country,
  'state': instance.state,
  'city': instance.city,
  'postalCode': instance.postalCode,
};

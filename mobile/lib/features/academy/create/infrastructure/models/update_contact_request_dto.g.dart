// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_contact_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_UpdateContactRequestDto _$UpdateContactRequestDtoFromJson(
  Map<String, dynamic> json,
) => _UpdateContactRequestDto(
  primaryContactName: json['primaryContactName'] as String?,
  primaryPhone: json['primaryPhone'] as String?,
  primaryEmail: json['primaryEmail'] as String?,
  secondaryContactName: json['secondaryContactName'] as String?,
  secondaryPhone: json['secondaryPhone'] as String?,
  secondaryEmail: json['secondaryEmail'] as String?,
  address: json['address'] as String?,
  country: json['country'] as String?,
  state: json['state'] as String?,
  city: json['city'] as String?,
  postalCode: json['postalCode'] as String?,
);

Map<String, dynamic> _$UpdateContactRequestDtoToJson(
  _UpdateContactRequestDto instance,
) => <String, dynamic>{
  'primaryContactName': instance.primaryContactName,
  'primaryPhone': instance.primaryPhone,
  'primaryEmail': instance.primaryEmail,
  'secondaryContactName': instance.secondaryContactName,
  'secondaryPhone': instance.secondaryPhone,
  'secondaryEmail': instance.secondaryEmail,
  'address': instance.address,
  'country': instance.country,
  'state': instance.state,
  'city': instance.city,
  'postalCode': instance.postalCode,
};

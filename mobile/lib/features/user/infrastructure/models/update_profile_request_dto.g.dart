// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_profile_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_UpdateProfileRequestDto _$UpdateProfileRequestDtoFromJson(
  Map<String, dynamic> json,
) => _UpdateProfileRequestDto(
  dateOfBirth: json['dateOfBirth'] as String?,
  gender: $enumDecodeNullable(
    _$GenderDtoEnumMap,
    json['gender'],
    unknownValue: GenderDto.preferNotToSay,
  ),
  bio: json['bio'] as String?,
  height: json['height'] as String?,
  weight: json['weight'] as String?,
  preferredSport: json['preferredSport'] as String?,
  experienceLevel: json['experienceLevel'] as String?,
  primaryPhoneCountryCode: json['primaryPhoneCountryCode'] as String?,
  primaryPhoneNumber: json['primaryPhoneNumber'] as String?,
  addressLine1: json['addressLine1'] as String?,
  addressLine2: json['addressLine2'] as String?,
  city: json['city'] as String?,
  state: json['state'] as String?,
  country: json['country'] as String?,
  postalCode: json['postalCode'] as String?,
  addressType: $enumDecodeNullable(
    _$AddressTypeDtoEnumMap,
    json['addressType'],
    unknownValue: AddressTypeDto.home,
  ),
);

Map<String, dynamic> _$UpdateProfileRequestDtoToJson(
  _UpdateProfileRequestDto instance,
) => <String, dynamic>{
  'dateOfBirth': instance.dateOfBirth,
  'gender': _$GenderDtoEnumMap[instance.gender],
  'bio': instance.bio,
  'height': instance.height,
  'weight': instance.weight,
  'preferredSport': instance.preferredSport,
  'experienceLevel': instance.experienceLevel,
  'primaryPhoneCountryCode': instance.primaryPhoneCountryCode,
  'primaryPhoneNumber': instance.primaryPhoneNumber,
  'addressLine1': instance.addressLine1,
  'addressLine2': instance.addressLine2,
  'city': instance.city,
  'state': instance.state,
  'country': instance.country,
  'postalCode': instance.postalCode,
  'addressType': _$AddressTypeDtoEnumMap[instance.addressType],
};

const _$GenderDtoEnumMap = {
  GenderDto.male: 0,
  GenderDto.female: 1,
  GenderDto.nonBinary: 2,
  GenderDto.preferNotToSay: 3,
};

const _$AddressTypeDtoEnumMap = {
  AddressTypeDto.home: 0,
  AddressTypeDto.work: 1,
  AddressTypeDto.academy: 2,
  AddressTypeDto.other: 3,
};

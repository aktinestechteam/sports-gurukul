// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'address_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_AddressDto _$AddressDtoFromJson(Map<String, dynamic> json) => _AddressDto(
  id: json['id'] as String,
  addressType: $enumDecode(
    _$AddressTypeDtoEnumMap,
    json['addressType'],
    unknownValue: AddressTypeDto.other,
  ),
  line1: json['line1'] as String,
  city: json['city'] as String,
  state: json['state'] as String,
  country: json['country'] as String,
  line2: json['line2'] as String?,
  postalCode: json['postalCode'] as String?,
  isPrimary: json['isPrimary'] as bool? ?? false,
  latitude: (json['latitude'] as num?)?.toDouble(),
  longitude: (json['longitude'] as num?)?.toDouble(),
);

Map<String, dynamic> _$AddressDtoToJson(_AddressDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'addressType': _$AddressTypeDtoEnumMap[instance.addressType]!,
      'line1': instance.line1,
      'city': instance.city,
      'state': instance.state,
      'country': instance.country,
      'line2': instance.line2,
      'postalCode': instance.postalCode,
      'isPrimary': instance.isPrimary,
      'latitude': instance.latitude,
      'longitude': instance.longitude,
    };

const _$AddressTypeDtoEnumMap = {
  AddressTypeDto.home: 0,
  AddressTypeDto.work: 1,
  AddressTypeDto.academy: 2,
  AddressTypeDto.other: 3,
};

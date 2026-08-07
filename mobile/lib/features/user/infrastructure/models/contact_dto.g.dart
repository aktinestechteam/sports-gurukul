// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'contact_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_ContactDto _$ContactDtoFromJson(Map<String, dynamic> json) => _ContactDto(
  id: json['id'] as String,
  primaryPhoneCountryCode: json['primaryPhoneCountryCode'] as String?,
  primaryPhoneNumber: json['primaryPhoneNumber'] as String?,
  primaryPhoneVerified: json['primaryPhoneVerified'] as bool? ?? false,
  secondaryPhoneCountryCode: json['secondaryPhoneCountryCode'] as String?,
  secondaryPhoneNumber: json['secondaryPhoneNumber'] as String?,
  secondaryPhoneVerified: json['secondaryPhoneVerified'] as bool? ?? false,
  websiteUrl: json['websiteUrl'] as String?,
  facebookUrl: json['facebookUrl'] as String?,
  twitterUrl: json['twitterUrl'] as String?,
  instagramUrl: json['instagramUrl'] as String?,
  linkedInUrl: json['linkedInUrl'] as String?,
  youTubeUrl: json['youTubeUrl'] as String?,
);

Map<String, dynamic> _$ContactDtoToJson(_ContactDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'primaryPhoneCountryCode': instance.primaryPhoneCountryCode,
      'primaryPhoneNumber': instance.primaryPhoneNumber,
      'primaryPhoneVerified': instance.primaryPhoneVerified,
      'secondaryPhoneCountryCode': instance.secondaryPhoneCountryCode,
      'secondaryPhoneNumber': instance.secondaryPhoneNumber,
      'secondaryPhoneVerified': instance.secondaryPhoneVerified,
      'websiteUrl': instance.websiteUrl,
      'facebookUrl': instance.facebookUrl,
      'twitterUrl': instance.twitterUrl,
      'instagramUrl': instance.instagramUrl,
      'linkedInUrl': instance.linkedInUrl,
      'youTubeUrl': instance.youTubeUrl,
    };

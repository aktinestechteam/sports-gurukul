// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_preferences_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_UpdatePreferencesRequestDto _$UpdatePreferencesRequestDtoFromJson(
  Map<String, dynamic> json,
) => _UpdatePreferencesRequestDto(
  language: json['language'] as String?,
  theme: $enumDecodeNullable(
    _$ThemeDtoEnumMap,
    json['theme'],
    unknownValue: ThemeDto.system,
  ),
  timeZone: json['timeZone'] as String?,
  emailNotifications: json['emailNotifications'] as bool?,
  pushNotifications: json['pushNotifications'] as bool?,
  smsNotifications: json['smsNotifications'] as bool?,
  marketingEmails: json['marketingEmails'] as bool?,
  profileVisibility: json['profileVisibility'] as bool?,
  showOnlineStatus: json['showOnlineStatus'] as bool?,
);

Map<String, dynamic> _$UpdatePreferencesRequestDtoToJson(
  _UpdatePreferencesRequestDto instance,
) => <String, dynamic>{
  'language': instance.language,
  'theme': _$ThemeDtoEnumMap[instance.theme],
  'timeZone': instance.timeZone,
  'emailNotifications': instance.emailNotifications,
  'pushNotifications': instance.pushNotifications,
  'smsNotifications': instance.smsNotifications,
  'marketingEmails': instance.marketingEmails,
  'profileVisibility': instance.profileVisibility,
  'showOnlineStatus': instance.showOnlineStatus,
};

const _$ThemeDtoEnumMap = {
  ThemeDto.light: 0,
  ThemeDto.dark: 1,
  ThemeDto.system: 2,
};

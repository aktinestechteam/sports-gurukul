// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_preference_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_UserPreferenceDto _$UserPreferenceDtoFromJson(Map<String, dynamic> json) =>
    _UserPreferenceDto(
      id: json['id'] as String,
      language: json['language'] as String? ?? 'en',
      theme:
          $enumDecodeNullable(
            _$ThemeDtoEnumMap,
            json['theme'],
            unknownValue: ThemeDto.system,
          ) ??
          ThemeDto.system,
      timeZone: json['timeZone'] as String? ?? 'UTC',
      emailNotifications: json['emailNotifications'] as bool? ?? true,
      pushNotifications: json['pushNotifications'] as bool? ?? true,
      smsNotifications: json['smsNotifications'] as bool? ?? false,
      marketingEmails: json['marketingEmails'] as bool? ?? false,
      profileVisibility: json['profileVisibility'] as bool? ?? true,
      showOnlineStatus: json['showOnlineStatus'] as bool? ?? true,
    );

Map<String, dynamic> _$UserPreferenceDtoToJson(_UserPreferenceDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'language': instance.language,
      'theme': _$ThemeDtoEnumMap[instance.theme]!,
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

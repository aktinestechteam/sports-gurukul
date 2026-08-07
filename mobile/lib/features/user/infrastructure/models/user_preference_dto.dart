import 'package:freezed_annotation/freezed_annotation.dart';

part 'user_preference_dto.freezed.dart';
part 'user_preference_dto.g.dart';

/// DTO matching the backend `UserPreferenceDto` schema.
@freezed
abstract class UserPreferenceDto with _$UserPreferenceDto {
  const factory UserPreferenceDto({
    required String id,
    @Default('en') String language,
    @JsonKey(unknownEnumValue: ThemeDto.system)
    @Default(ThemeDto.system)
    ThemeDto theme,
    @Default('UTC') String timeZone,
    @Default(true) bool emailNotifications,
    @Default(true) bool pushNotifications,
    @Default(false) bool smsNotifications,
    @Default(false) bool marketingEmails,
    @Default(true) bool profileVisibility,
    @Default(true) bool showOnlineStatus,
  }) = _UserPreferenceDto;

  factory UserPreferenceDto.fromJson(Map<String, dynamic> json) =>
      _$UserPreferenceDtoFromJson(json);
}

/// Theme enum matching the backend `Theme`.
@JsonEnum(valueField: 'value')
enum ThemeDto {
  @JsonValue(0)
  light,
  @JsonValue(1)
  dark,
  @JsonValue(2)
  system,
}

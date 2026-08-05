import 'package:freezed_annotation/freezed_annotation.dart';

import 'package:sports_gurukul/features/user/infrastructure/models/user_preference_dto.dart';

part 'update_preferences_request_dto.freezed.dart';
part 'update_preferences_request_dto.g.dart';

/// Request body for `PUT /api/v1/users/preferences`.
///
/// All fields are nullable so only supplied values are sent to the backend
/// (partial update). The backend ignores null fields.
@freezed
abstract class UpdatePreferencesRequestDto with _$UpdatePreferencesRequestDto {
  const factory UpdatePreferencesRequestDto({
    String? language,
    @JsonKey(unknownEnumValue: ThemeDto.system)
    ThemeDto? theme,
    String? timeZone,
    bool? emailNotifications,
    bool? pushNotifications,
    bool? smsNotifications,
    bool? marketingEmails,
    bool? profileVisibility,
    bool? showOnlineStatus,
  }) = _UpdatePreferencesRequestDto;

  factory UpdatePreferencesRequestDto.fromJson(Map<String, dynamic> json) =>
      _$UpdatePreferencesRequestDtoFromJson(json);
}

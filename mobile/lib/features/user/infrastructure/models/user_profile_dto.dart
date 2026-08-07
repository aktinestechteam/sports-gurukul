import 'package:freezed_annotation/freezed_annotation.dart';

import 'package:sports_gurukul/features/user/infrastructure/models/address_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/contact_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_preference_dto.dart';

part 'user_profile_dto.freezed.dart';
part 'user_profile_dto.g.dart';

/// DTO matching the backend `UserProfileDto` schema.
@freezed
abstract class UserProfileDto with _$UserProfileDto {
  const factory UserProfileDto({
    required String id,
    required String userId,
    required String fullName,
    required String email,
    required String createdAt,
    String? phoneNumber,
    String? dateOfBirth,
    @JsonKey(unknownEnumValue: GenderDto.preferNotToSay)
    @Default(GenderDto.preferNotToSay)
    GenderDto gender,
    String? bio,
    String? profileImageUrl,
    String? coverImageUrl,
    String? height,
    String? weight,
    String? preferredSport,
    String? experienceLevel,
    @JsonKey(unknownEnumValue: UserStatusDto.active)
    @Default(UserStatusDto.active)
    UserStatusDto status,
    @Default(false) bool isEmailVerified,
    String? updatedAt,
    @Default(0) int profileCompletionPercentage,
    @Default([]) List<AddressDto> addresses,
    ContactDto? contactInformation,
    UserPreferenceDto? preferences,
    @Default([]) List<String> roles,
    @Default(true) bool hasProfile,
  }) = _UserProfileDto;

  factory UserProfileDto.fromJson(Map<String, dynamic> json) =>
      _$UserProfileDtoFromJson(json);
}

/// Gender enum matching the backend `Gender`.
@JsonEnum(valueField: 'value')
enum GenderDto {
  @JsonValue(0)
  male,
  @JsonValue(1)
  female,
  @JsonValue(2)
  nonBinary,
  @JsonValue(3)
  preferNotToSay,
}

/// User status enum matching the backend `UserStatus`.
@JsonEnum(valueField: 'value')
enum UserStatusDto {
  @JsonValue(0)
  active,
  @JsonValue(1)
  inactive,
  @JsonValue(2)
  suspended,
  @JsonValue(3)
  locked,
}

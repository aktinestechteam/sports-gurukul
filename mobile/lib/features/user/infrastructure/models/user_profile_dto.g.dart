// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_profile_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_UserProfileDto _$UserProfileDtoFromJson(
  Map<String, dynamic> json,
) => _UserProfileDto(
  id: json['id'] as String,
  userId: json['userId'] as String,
  fullName: json['fullName'] as String,
  email: json['email'] as String,
  createdAt: json['createdAt'] as String,
  phoneNumber: json['phoneNumber'] as String?,
  dateOfBirth: json['dateOfBirth'] as String?,
  gender:
      $enumDecodeNullable(
        _$GenderDtoEnumMap,
        json['gender'],
        unknownValue: GenderDto.preferNotToSay,
      ) ??
      GenderDto.preferNotToSay,
  bio: json['bio'] as String?,
  profileImageUrl: json['profileImageUrl'] as String?,
  coverImageUrl: json['coverImageUrl'] as String?,
  height: json['height'] as String?,
  weight: json['weight'] as String?,
  preferredSport: json['preferredSport'] as String?,
  experienceLevel: json['experienceLevel'] as String?,
  status:
      $enumDecodeNullable(
        _$UserStatusDtoEnumMap,
        json['status'],
        unknownValue: UserStatusDto.active,
      ) ??
      UserStatusDto.active,
  isEmailVerified: json['isEmailVerified'] as bool? ?? false,
  updatedAt: json['updatedAt'] as String?,
  profileCompletionPercentage:
      (json['profileCompletionPercentage'] as num?)?.toInt() ?? 0,
  addresses:
      (json['addresses'] as List<dynamic>?)
          ?.map((e) => AddressDto.fromJson(e as Map<String, dynamic>))
          .toList() ??
      const [],
  contactInformation: json['contactInformation'] == null
      ? null
      : ContactDto.fromJson(json['contactInformation'] as Map<String, dynamic>),
  preferences: json['preferences'] == null
      ? null
      : UserPreferenceDto.fromJson(json['preferences'] as Map<String, dynamic>),
  roles:
      (json['roles'] as List<dynamic>?)?.map((e) => e as String).toList() ??
      const [],
);

Map<String, dynamic> _$UserProfileDtoToJson(_UserProfileDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'userId': instance.userId,
      'fullName': instance.fullName,
      'email': instance.email,
      'createdAt': instance.createdAt,
      'phoneNumber': instance.phoneNumber,
      'dateOfBirth': instance.dateOfBirth,
      'gender': _$GenderDtoEnumMap[instance.gender]!,
      'bio': instance.bio,
      'profileImageUrl': instance.profileImageUrl,
      'coverImageUrl': instance.coverImageUrl,
      'height': instance.height,
      'weight': instance.weight,
      'preferredSport': instance.preferredSport,
      'experienceLevel': instance.experienceLevel,
      'status': _$UserStatusDtoEnumMap[instance.status]!,
      'isEmailVerified': instance.isEmailVerified,
      'updatedAt': instance.updatedAt,
      'profileCompletionPercentage': instance.profileCompletionPercentage,
      'addresses': instance.addresses,
      'contactInformation': instance.contactInformation,
      'preferences': instance.preferences,
      'roles': instance.roles,
    };

const _$GenderDtoEnumMap = {
  GenderDto.male: 0,
  GenderDto.female: 1,
  GenderDto.nonBinary: 2,
  GenderDto.preferNotToSay: 3,
};

const _$UserStatusDtoEnumMap = {
  UserStatusDto.active: 0,
  UserStatusDto.inactive: 1,
  UserStatusDto.suspended: 2,
  UserStatusDto.locked: 3,
};

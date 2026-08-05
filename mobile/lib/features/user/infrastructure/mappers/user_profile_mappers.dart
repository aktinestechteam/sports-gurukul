import 'package:sports_gurukul/features/user/domain/entities/address.dart';
import 'package:sports_gurukul/features/user/domain/entities/contact_information.dart';
import 'package:sports_gurukul/features/user/domain/entities/profile_photo.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_preference.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/address_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/contact_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/profile_photo_response_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_preference_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_profile_dto.dart';

/// Converts infrastructure DTOs into domain entities.
///
/// Mapping happens only at the repository boundary: DTOs never leave
/// infrastructure and domain entities never travel to the wire.
abstract final class UserProfileMappers {
  static UserProfile toProfile(UserProfileDto dto) => UserProfile(
    id: dto.id,
    userId: dto.userId,
    fullName: dto.fullName,
    email: dto.email,
    phoneNumber: dto.phoneNumber,
    dateOfBirth: dto.dateOfBirth != null
        ? DateTime.tryParse(dto.dateOfBirth!)
        : null,
    gender: _mapGender(dto.gender),
    bio: dto.bio,
    profileImageUrl: dto.profileImageUrl,
    coverImageUrl: dto.coverImageUrl,
    height: dto.height,
    weight: dto.weight,
    preferredSport: dto.preferredSport,
    experienceLevel: dto.experienceLevel,
    status: _mapStatus(dto.status),
    isEmailVerified: dto.isEmailVerified,
    createdAt: DateTime.parse(dto.createdAt),
    updatedAt: dto.updatedAt != null ? DateTime.tryParse(dto.updatedAt!) : null,
    profileCompletionPercentage: dto.profileCompletionPercentage,
    addresses: dto.addresses.map(toAddress).toList(),
    contactInformation: dto.contactInformation != null
        ? toContactInformation(dto.contactInformation!)
        : null,
    preferences: dto.preferences != null
        ? toPreference(dto.preferences!)
        : null,
    roles: dto.roles,
  );

  static Address toAddress(AddressDto dto) => Address(
    id: dto.id,
    addressType: _mapAddressType(dto.addressType),
    line1: dto.line1,
    line2: dto.line2,
    city: dto.city,
    state: dto.state,
    country: dto.country,
    postalCode: dto.postalCode,
    isPrimary: dto.isPrimary,
    latitude: dto.latitude,
    longitude: dto.longitude,
  );

  static ContactInformation toContactInformation(ContactDto dto) =>
      ContactInformation(
        id: dto.id,
        primaryPhoneCountryCode: dto.primaryPhoneCountryCode,
        primaryPhoneNumber: dto.primaryPhoneNumber,
        primaryPhoneVerified: dto.primaryPhoneVerified,
        secondaryPhoneCountryCode: dto.secondaryPhoneCountryCode,
        secondaryPhoneNumber: dto.secondaryPhoneNumber,
        secondaryPhoneVerified: dto.secondaryPhoneVerified,
        websiteUrl: dto.websiteUrl,
        facebookUrl: dto.facebookUrl,
        twitterUrl: dto.twitterUrl,
        instagramUrl: dto.instagramUrl,
        linkedInUrl: dto.linkedInUrl,
        youTubeUrl: dto.youTubeUrl,
      );

  static UserPreference toPreference(UserPreferenceDto dto) => UserPreference(
    id: dto.id,
    language: dto.language,
    theme: _mapTheme(dto.theme),
    timeZone: dto.timeZone,
    emailNotifications: dto.emailNotifications,
    pushNotifications: dto.pushNotifications,
    smsNotifications: dto.smsNotifications,
    marketingEmails: dto.marketingEmails,
    profileVisibility: dto.profileVisibility,
    showOnlineStatus: dto.showOnlineStatus,
  );

  static ProfilePhoto toPhoto(ProfilePhotoResponseDto dto) => ProfilePhoto(
    fileId: dto.fileId,
    url: dto.url,
    fileName: dto.fileName,
    fileSize: dto.fileSize,
    contentType: dto.contentType,
    uploadedAt: DateTime.parse(dto.uploadedAt),
  );

  static Gender _mapGender(GenderDto dto) => switch (dto) {
    GenderDto.male => Gender.male,
    GenderDto.female => Gender.female,
    GenderDto.nonBinary => Gender.nonBinary,
    GenderDto.preferNotToSay => Gender.preferNotToSay,
  };

  static UserStatus _mapStatus(UserStatusDto dto) => switch (dto) {
    UserStatusDto.active => UserStatus.active,
    UserStatusDto.inactive => UserStatus.inactive,
    UserStatusDto.suspended => UserStatus.suspended,
    UserStatusDto.locked => UserStatus.locked,
  };

  static AddressType _mapAddressType(AddressTypeDto dto) => switch (dto) {
    AddressTypeDto.home => AddressType.home,
    AddressTypeDto.work => AddressType.work,
    AddressTypeDto.academy => AddressType.academy,
    AddressTypeDto.other => AddressType.other,
  };

  static AppTheme _mapTheme(ThemeDto dto) => switch (dto) {
    ThemeDto.light => AppTheme.light,
    ThemeDto.dark => AppTheme.dark,
    ThemeDto.system => AppTheme.system,
  };

  // Reverse mappings for outbound DTOs.

  static GenderDto mapGenderToDto(Gender gender) => switch (gender) {
    Gender.male => GenderDto.male,
    Gender.female => GenderDto.female,
    Gender.nonBinary => GenderDto.nonBinary,
    Gender.preferNotToSay => GenderDto.preferNotToSay,
  };

  static AddressTypeDto mapAddressTypeToDto(AddressType type) => switch (type) {
    AddressType.home => AddressTypeDto.home,
    AddressType.work => AddressTypeDto.work,
    AddressType.academy => AddressTypeDto.academy,
    AddressType.other => AddressTypeDto.other,
  };

  static ThemeDto mapThemeToDto(AppTheme theme) => switch (theme) {
    AppTheme.light => ThemeDto.light,
    AppTheme.dark => ThemeDto.dark,
    AppTheme.system => ThemeDto.system,
  };
}

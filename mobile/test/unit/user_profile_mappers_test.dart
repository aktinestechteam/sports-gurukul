import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/user/domain/entities/address.dart';
import 'package:sports_gurukul/features/user/domain/entities/profile_photo.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_preference.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/infrastructure/mappers/user_profile_mappers.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/address_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/contact_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/profile_photo_response_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_preference_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_profile_dto.dart';

void main() {
  group('UserProfileMappers', () {
    test('toProfile maps every field into the domain entity', () {
      const dto = UserProfileDto(
        id: 'profile-1',
        userId: 'user-1',
        fullName: 'Aarav Sharma',
        email: 'aarav@example.com',
        phoneNumber: '+919000000000',
        dateOfBirth: '2000-01-15',
        gender: GenderDto.male,
        bio: 'Cricketer',
        profileImageUrl: 'https://cdn.example.com/avatar.png',
        coverImageUrl: 'https://cdn.example.com/cover.png',
        height: '178',
        weight: '72',
        preferredSport: 'cricket',
        experienceLevel: 'intermediate',
        isEmailVerified: true,
        createdAt: '2026-01-01T00:00:00.0000000Z',
        updatedAt: '2026-02-01T00:00:00.0000000Z',
        profileCompletionPercentage: 85,
        addresses: <AddressDto>[
          AddressDto(
            id: 'addr-1',
            addressType: AddressTypeDto.home,
            line1: '12 MG Road',
            line2: 'Apt 3B',
            city: 'Mumbai',
            state: 'MH',
            country: 'IN',
            postalCode: '400001',
            isPrimary: true,
          ),
        ],
        contactInformation: ContactDto(
          id: 'contact-1',
          primaryPhoneCountryCode: '+91',
          primaryPhoneNumber: '9000000000',
          primaryPhoneVerified: true,
          websiteUrl: 'https://aarav.dev',
        ),
        preferences: UserPreferenceDto(
          id: 'pref-1',
          timeZone: 'Asia/Kolkata',
          pushNotifications: false,
          smsNotifications: true,
          showOnlineStatus: false,
        ),
        roles: <String>['Player', 'Member'],
      );

      final profile = UserProfileMappers.toProfile(dto);

      expect(profile.id, 'profile-1');
      expect(profile.userId, 'user-1');
      expect(profile.fullName, 'Aarav Sharma');
      expect(profile.email, 'aarav@example.com');
      expect(profile.phoneNumber, '+919000000000');
      expect(profile.dateOfBirth, DateTime(2000, 1, 15));
      expect(profile.gender, Gender.male);
      expect(profile.status, UserStatus.active);
      expect(profile.isEmailVerified, isTrue);
      expect(profile.profileCompletionPercentage, 85);
      expect(profile.roles, <String>['Player', 'Member']);
      expect(profile.hasProfile, isTrue);

      expect(profile.addresses, hasLength(1));
      expect(profile.addresses.first.addressType, AddressType.home);
      expect(profile.addresses.first.isPrimary, isTrue);
      expect(profile.addresses.first.postalCode, '400001');

      expect(profile.contactInformation?.primaryPhoneNumber, '9000000000');
      expect(profile.contactInformation?.websiteUrl, 'https://aarav.dev');

      expect(profile.preferences?.theme, AppTheme.system);
      expect(profile.preferences?.pushNotifications, isFalse);
      expect(profile.preferences?.smsNotifications, isTrue);
    });

    test('toProfile handles missing optional data', () {
      const dto = UserProfileDto(
        id: 'profile-2',
        userId: 'user-2',
        fullName: 'Priya',
        email: 'priya@example.com',
        createdAt: '2026-03-01T10:00:00.0000000Z',
        hasProfile: false,
      );

      final profile = UserProfileMappers.toProfile(dto);

      expect(profile.phoneNumber, isNull);
      expect(profile.dateOfBirth, isNull);
      expect(profile.bio, isNull);
      expect(profile.gender, Gender.preferNotToSay);
      expect(profile.status, UserStatus.active);
      expect(profile.addresses, isEmpty);
      expect(profile.contactInformation, isNull);
      expect(profile.preferences, isNull);
      expect(profile.hasProfile, isFalse);
    });

    test('toPhoto maps photo metadata', () {
      const dto = ProfilePhotoResponseDto(
        fileId: 'file-1',
        url: 'https://cdn.example.com/p.png',
        fileName: 'p.png',
        fileSize: 4096,
        contentType: 'image/png',
        uploadedAt: '2026-04-01T00:00:00.0000000Z',
      );

      final photo = UserProfileMappers.toPhoto(dto);

      expect(photo, isA<ProfilePhoto>());
      expect(photo.fileId, 'file-1');
      expect(photo.url, 'https://cdn.example.com/p.png');
      expect(photo.contentType, 'image/png');
      expect(photo.uploadedAt, DateTime.utc(2026, 4));
    });

    test('enum reverse mappings round-trip', () {
      expect(
        UserProfileMappers.mapGenderToDto(Gender.female),
        GenderDto.female,
      );
      expect(
        UserProfileMappers.mapAddressTypeToDto(AddressType.work),
        AddressTypeDto.work,
      );
      expect(UserProfileMappers.mapThemeToDto(AppTheme.dark), ThemeDto.dark);
    });

    test('toPreference maps theme enum', () {
      const dto = UserPreferenceDto(
        id: 'pref-2',
        theme: ThemeDto.dark,
      );

      final preference = UserProfileMappers.toPreference(dto);

      expect(preference.theme, AppTheme.dark);
      expect(preference.language, 'en');
    });
  });
}

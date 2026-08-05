import 'dart:io';

import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/user/domain/entities/profile_photo.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_preference.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/infrastructure/datasources/user_profile_remote_datasource.dart';
import 'package:sports_gurukul/features/user/infrastructure/error/user_profile_error_mapper.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/address_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/profile_photo_response_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/update_preferences_request_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/update_profile_request_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_preference_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_profile_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/repositories/user_profile_repository_impl.dart';

class FakeUserProfileRemoteDataSource implements UserProfileRemoteDataSource {
  final Map<Type, Object> _responses = <Type, Object>{};
  final List<Object> _capturedRequests = <Object>[];
  ApiException? _deleteError;
  int _calls = 0;

  void respondWith<T extends Object>(T value) => _responses[T] = value;

  void failWith<T>(ApiException error) => _responses[T] = error;

  // Deliberate: test helpers mirror the datasource method vocabulary.
  // ignore: use_setters_to_change_properties
  void failDeleteWith(ApiException error) => _deleteError = error;

  List<Object> get capturedRequests =>
      List<Object>.unmodifiable(_capturedRequests);

  int get calls => _calls;

  @override
  Future<UserProfileDto> getCurrentProfile() async {
    _calls++;
    return _next<UserProfileDto>();
  }

  @override
  Future<UserProfileDto> updateProfile(UpdateProfileRequestDto request) async {
    _calls++;
    _capturedRequests.add(request);
    return _next<UserProfileDto>();
  }

  @override
  Future<UserPreferenceDto> updatePreferences(
    UpdatePreferencesRequestDto request,
  ) async {
    _calls++;
    _capturedRequests.add(request);
    return _next<UserPreferenceDto>();
  }

  @override
  Future<ProfilePhotoResponseDto> uploadProfilePhoto({
    required String fileName,
    required String contentType,
    required List<int> fileBytes,
  }) async {
    _calls++;
    _capturedRequests.add(<String, Object>{
      'fileName': fileName,
      'contentType': contentType,
      'fileBytes': fileBytes,
    });
    return _next<ProfilePhotoResponseDto>();
  }

  @override
  Future<ProfilePhotoResponseDto> getProfilePhoto() async {
    _calls++;
    return _next<ProfilePhotoResponseDto>();
  }

  @override
  Future<void> deleteProfilePhoto() async {
    _calls++;
    final failure = _deleteError;
    if (failure != null) {
      throw failure;
    }
  }

  T _next<T>() {
    final value = _responses[T];
    if (value is ApiException) {
      throw value;
    }
    return value as T;
  }
}

void main() {
  late FakeUserProfileRemoteDataSource remote;
  late UserProfileRepositoryImpl repository;

  const profileDto = UserProfileDto(
    id: 'profile-1',
    userId: 'user-1',
    fullName: 'Aarav Sharma',
    email: 'aarav@example.com',
    createdAt: '2026-01-01T00:00:00.0000000Z',
  );

  const preferenceDto = UserPreferenceDto(id: 'pref-1');

  const photoDto = ProfilePhotoResponseDto(
    fileId: 'file-1',
    url: 'https://cdn.example.com/p.png',
    fileName: 'p.png',
    fileSize: 10,
    contentType: 'image/png',
    uploadedAt: '2026-01-01T00:00:00.0000000Z',
  );

  setUp(() {
    remote = FakeUserProfileRemoteDataSource();
    repository = UserProfileRepositoryImpl(remote: remote);
  });

  group('UserProfileRepositoryImpl', () {
    test('getCurrentProfile returns the mapped profile', () async {
      remote.respondWith<UserProfileDto>(profileDto);

      final result = await repository.getCurrentProfile();

      expect(result, isA<Success<UserProfile>>());
      expect(result.requireValue().fullName, 'Aarav Sharma');
      expect(remote.calls, 1);
    });

    test('getCurrentProfile maps transport errors to failures', () async {
      remote.failWith<UserProfileDto>(
        const ApiException(
          statusCode: 503,
          code: UserProfileOperations.getCurrentProfile,
          message: 'down',
        ),
      );

      final result = await repository.getCurrentProfile();

      expect(result, isA<FailureResult<UserProfile>>());
      final failure = result.failureOrNull;
      expect(failure, isA<ServerFailure>());
      expect(failure?.code, UserProfileErrorCodes.server);
    });

    test(
      'updateProfile sends a partial request and maps the response',
      () async {
        remote.respondWith<UserProfileDto>(profileDto);

        final result = await repository.updateProfile(
          bio: 'All-rounder',
          gender: 'male',
          preferredSport: 'cricket',
          addressType: 'home',
        );

        expect(result, isA<Success<UserProfile>>());
        final request = remote.capturedRequests.single
            as UpdateProfileRequestDto;
        expect(request.bio, 'All-rounder');
        expect(request.preferredSport, 'cricket');
        expect(request.gender, GenderDto.male);
        expect(request.addressType, AddressTypeDto.home);
        expect(request.city, isNull);
      },
    );

    test(
      'updateProfile falls back to default enums for unknown values',
      () async {
        remote.respondWith<UserProfileDto>(profileDto);

        await repository.updateProfile(
          gender: 'alien',
          addressType: 'spaceship',
        );

        final request = remote.capturedRequests.single
            as UpdateProfileRequestDto;
        expect(request.gender, GenderDto.preferNotToSay);
        expect(request.addressType, AddressTypeDto.home);
      },
    );

    test('updatePreferences forwards the mapped theme', () async {
      remote.respondWith<UserPreferenceDto>(preferenceDto);

      final result = await repository.updatePreferences(
        theme: 'dark',
        pushNotifications: false,
      );

      expect(result, isA<Success<UserPreference>>());
      final request = remote.capturedRequests.single
          as UpdatePreferencesRequestDto;
      expect(request.theme, ThemeDto.dark);
      expect(request.pushNotifications, isFalse);
    });

    test(
      'uploadProfilePhoto sends bytes with a guessed content type',
      () async {
        remote.respondWith<ProfilePhotoResponseDto>(photoDto);

        final file = File(
          '${Directory.systemTemp.path}${Platform.pathSeparator}avatar.png',
        );
        await file.writeAsBytes(<int>[1, 2, 3]);

        final result = await repository.uploadProfilePhoto(file);

        expect(result, isA<Success<ProfilePhoto>>());
        expect(result.requireValue().url, 'https://cdn.example.com/p.png');

        final payload = remote.capturedRequests.single as Map<String, Object>;
        expect(payload['fileName'], 'avatar.png');
        expect(payload['contentType'], 'image/png');
        expect(payload['fileBytes'], <int>[1, 2, 3]);
      },
    );

    test('deleteProfilePhoto returns success on clean delete', () async {
      final result = await repository.deleteProfilePhoto();

      expect(result, isA<OperationSuccess>());
      expect(remote.calls, 1);
    });

    test('deleteProfilePhoto maps failures', () async {
      remote.failDeleteWith(
        const ApiException(
          statusCode: 404,
          code: UserProfileOperations.deletePhoto,
        ),
      );

      final result = await repository.deleteProfilePhoto();

      expect(result, isA<OperationFailure>());
      expect(result.failureOrNull?.code, UserProfileErrorCodes.notFound);
    });
  });
}

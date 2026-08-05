import 'dart:io';

import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/user/domain/entities/profile_photo.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_preference.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/domain/repositories/user_profile_repository.dart';
import 'package:sports_gurukul/features/user/infrastructure/datasources/user_profile_remote_datasource.dart';
import 'package:sports_gurukul/features/user/infrastructure/error/user_profile_error_mapper.dart';
import 'package:sports_gurukul/features/user/infrastructure/mappers/user_profile_mappers.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/address_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/update_preferences_request_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/update_profile_request_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_preference_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_profile_dto.dart';

/// [UserProfileRepository] implementation backed by the remote datasource.
///
/// Transport and server failures are mapped to typed [BaseFailure]s at this
/// boundary; features only ever see `Result`/`OperationResult` values.
class UserProfileRepositoryImpl implements UserProfileRepository {
  UserProfileRepositoryImpl({
    required UserProfileRemoteDataSource remote,
  }) : _remote = remote;

  final UserProfileRemoteDataSource _remote;

  @override
  Future<Result<UserProfile>> getCurrentProfile() async {
    try {
      final dto = await _remote.getCurrentProfile();
      return Result.success(UserProfileMappers.toProfile(dto));
    } on ApiException catch (error) {
      return Result.failure(UserProfileErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<Result<UserProfile>> updateProfile({
    DateTime? dateOfBirth,
    String? gender,
    String? bio,
    String? height,
    String? weight,
    String? preferredSport,
    String? experienceLevel,
    String? primaryPhoneCountryCode,
    String? primaryPhoneNumber,
    String? addressLine1,
    String? addressLine2,
    String? city,
    String? state,
    String? country,
    String? postalCode,
    String? addressType,
  }) async {
    try {
      final request = UpdateProfileRequestDto(
        dateOfBirth: dateOfBirth?.toUtc().toIso8601String().split('T').first,
        gender: gender != null
            ? GenderDto.values.firstWhere(
                (e) => e.name == gender,
                orElse: () => GenderDto.preferNotToSay,
              )
            : null,
        bio: bio,
        height: height,
        weight: weight,
        preferredSport: preferredSport,
        experienceLevel: experienceLevel,
        primaryPhoneCountryCode: primaryPhoneCountryCode,
        primaryPhoneNumber: primaryPhoneNumber,
        addressLine1: addressLine1,
        addressLine2: addressLine2,
        city: city,
        state: state,
        country: country,
        postalCode: postalCode,
        addressType: addressType != null
            ? AddressTypeDto.values.firstWhere(
                (e) => e.name == addressType,
                orElse: () => AddressTypeDto.home,
              )
            : null,
      );
      final dto = await _remote.updateProfile(request);
      return Result.success(UserProfileMappers.toProfile(dto));
    } on ApiException catch (error) {
      return Result.failure(UserProfileErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<Result<UserPreference>> updatePreferences({
    String? language,
    String? theme,
    String? timeZone,
    bool? emailNotifications,
    bool? pushNotifications,
    bool? smsNotifications,
    bool? marketingEmails,
    bool? profileVisibility,
    bool? showOnlineStatus,
  }) async {
    try {
      final request = UpdatePreferencesRequestDto(
        language: language,
        theme: theme != null
            ? ThemeDto.values.firstWhere(
                (e) => e.name == theme,
                orElse: () => ThemeDto.system,
              )
            : null,
        timeZone: timeZone,
        emailNotifications: emailNotifications,
        pushNotifications: pushNotifications,
        smsNotifications: smsNotifications,
        marketingEmails: marketingEmails,
        profileVisibility: profileVisibility,
        showOnlineStatus: showOnlineStatus,
      );
      final dto = await _remote.updatePreferences(request);
      return Result.success(UserProfileMappers.toPreference(dto));
    } on ApiException catch (error) {
      return Result.failure(UserProfileErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<Result<ProfilePhoto>> uploadProfilePhoto(File imageFile) async {
    try {
      final bytes = await imageFile.readAsBytes();
      final fileName = imageFile.path.split(RegExp(r'[\\/]')).last;
      final contentType = _guessContentType(fileName);
      final dto = await _remote.uploadProfilePhoto(
        fileName: fileName,
        contentType: contentType,
        fileBytes: bytes,
      );
      return Result.success(UserProfileMappers.toPhoto(dto));
    } on ApiException catch (error) {
      return Result.failure(UserProfileErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<Result<ProfilePhoto>> getProfilePhoto() async {
    try {
      final dto = await _remote.getProfilePhoto();
      return Result.success(UserProfileMappers.toPhoto(dto));
    } on ApiException catch (error) {
      return Result.failure(UserProfileErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<OperationResult> deleteProfilePhoto() async {
    try {
      await _remote.deleteProfilePhoto();
      return const OperationResult.success();
    } on ApiException catch (error) {
      return OperationResult.failure(UserProfileErrorMapper.map(error));
    } on Object catch (error) {
      return OperationResult.failure(_unexpected(error));
    }
  }

  static String _guessContentType(String fileName) {
    final lower = fileName.toLowerCase();
    if (lower.endsWith('.png')) return 'image/png';
    if (lower.endsWith('.webp')) return 'image/webp';
    return 'image/jpeg';
  }

  static BaseFailure _unexpected(Object error) => UnknownFailure(
    message: 'Unexpected user profile failure',
    cause: error,
  );
}

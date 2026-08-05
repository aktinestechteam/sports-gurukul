import 'dart:io';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/user/application/user_profile_use_case_providers.dart';
import 'package:sports_gurukul/features/user/domain/entities/profile_photo.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_preference.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/infrastructure/error/user_profile_error_mapper.dart';

/// The profile-screen loading state.
sealed class ProfileState {
  const ProfileState();
}

/// Initial state before any data has been fetched.
final class ProfileInitial extends ProfileState {
  const ProfileInitial();
}

/// Profile is being fetched from the backend.
final class ProfileLoading extends ProfileState {
  const ProfileLoading();
}

/// Profile has been loaded successfully.
final class ProfileLoaded extends ProfileState {
  const ProfileLoaded(this.profile);

  final UserProfile profile;
}

/// Profile fetch failed.
final class ProfileError extends ProfileState {
  const ProfileError(this.message, {this.canCreate = false});

  final String message;

  /// Whether the failure means no profile exists yet, so the user can
  /// create one.
  final bool canCreate;
}

/// Owns the profile data lifecycle and exposes it to the UI.
///
/// Fetches, caches and refreshes the current user's profile. UI widgets
/// watch [profileControllerProvider] for loading / loaded / error states.
class ProfileController extends Notifier<ProfileState> {
  @override
  ProfileState build() => const ProfileInitial();

  /// Fetches the current user's profile from the backend.
  Future<void> loadProfile() async {
    state = const ProfileLoading();
    final result = await ref.read(getCurrentProfileProvider).call();
    state = switch (result) {
      Success(value: final profile) => ProfileLoaded(profile),
      FailureResult(:final failure) => ProfileError(
        failure.message.isNotEmpty ? failure.message : 'Failed to load profile',
        canCreate: failure.code == UserProfileErrorCodes.notFound,
      ),
    };
  }

  /// Refreshes the profile (pull-to-refresh).
  Future<void> refreshProfile() => loadProfile();

  /// Updates the user's profile via the backend and reloads.
  Future<OperationResult> updateProfile({
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
    String? region,
    String? country,
    String? postalCode,
    String? addressType,
  }) async {
    final result = await ref
        .read(updateProfileProvider)
        .call(
          dateOfBirth: dateOfBirth,
          gender: gender,
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
          state: region,
          country: country,
          postalCode: postalCode,
          addressType: addressType,
        );
    if (result is Success<UserProfile>) {
      state = ProfileLoaded(result.value);
    }
    return switch (result) {
      Success() => const OperationResult.success(),
      FailureResult(failure: final f) => OperationResult.failure(f),
    };
  }

  /// Updates the user's preferences via the backend and reloads.
  Future<OperationResult> updatePreferences({
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
    final result = await ref
        .read(updatePreferencesProvider)
        .call(
          language: language,
          theme: theme,
          timeZone: timeZone,
          emailNotifications: emailNotifications,
          pushNotifications: pushNotifications,
          smsNotifications: smsNotifications,
          marketingEmails: marketingEmails,
          profileVisibility: profileVisibility,
          showOnlineStatus: showOnlineStatus,
        );
    // After updating preferences, reload the full profile to stay in sync.
    if (result is Success<UserPreference>) {
      await loadProfile();
    }
    return switch (result) {
      Success() => const OperationResult.success(),
      FailureResult(failure: final f) => OperationResult.failure(f),
    };
  }

  /// Uploads a new profile photo and reloads the profile.
  Future<OperationResult> uploadPhoto(File imageFile) async {
    final result = await ref.read(uploadProfilePhotoProvider).call(imageFile);
    if (result is Success<ProfilePhoto>) {
      await loadProfile();
    }
    return switch (result) {
      Success() => const OperationResult.success(),
      FailureResult(failure: final f) => OperationResult.failure(f),
    };
  }

  /// Deletes the profile photo and reloads the profile.
  Future<OperationResult> deletePhoto() async {
    final result = await ref.read(deleteProfilePhotoProvider).call();
    if (result is OperationSuccess) {
      await loadProfile();
    }
    return result;
  }
}

/// Provides the profile controller.
final profileControllerProvider =
    NotifierProvider<ProfileController, ProfileState>(
      ProfileController.new,
    );

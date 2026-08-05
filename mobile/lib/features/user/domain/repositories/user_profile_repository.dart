import 'dart:io';

import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/user/domain/entities/profile_photo.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_preference.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';

/// Contract for user-profile operations backed by the backend users API.
///
/// Implementations in infrastructure talk to the network; domain and
/// application layers depend only on this abstraction. Every remote
/// operation returns a [Result] or [OperationResult] so failures flow to
/// the UI as data, never as raw exceptions.
abstract interface class UserProfileRepository {
  /// Fetches the full profile of the currently authenticated user.
  Future<Result<UserProfile>> getCurrentProfile();

  /// Updates the profile of the currently authenticated user.
  ///
  /// Only non-null parameters are sent; the backend applies partial updates.
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
  });

  /// Updates the preferences of the currently authenticated user.
  ///
  /// Only non-null parameters are sent; the backend applies partial updates.
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
  });

  /// Uploads a new profile photo for the currently authenticated user.
  Future<Result<ProfilePhoto>> uploadProfilePhoto(File imageFile);

  /// Gets the profile photo metadata for the currently authenticated user.
  Future<Result<ProfilePhoto>> getProfilePhoto();

  /// Deletes the profile photo of the currently authenticated user.
  Future<OperationResult> deleteProfilePhoto();
}

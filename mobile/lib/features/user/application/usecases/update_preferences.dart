import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_preference.dart';
import 'package:sports_gurukul/features/user/domain/repositories/user_profile_repository.dart';

/// Updates the preferences of the currently authenticated user.
///
/// Only non-null parameters are sent; the backend applies partial updates.
class UpdatePreferences {
  const UpdatePreferences(this._repository);

  final UserProfileRepository _repository;

  Future<Result<UserPreference>> call({
    String? language,
    String? theme,
    String? timeZone,
    bool? emailNotifications,
    bool? pushNotifications,
    bool? smsNotifications,
    bool? marketingEmails,
    bool? profileVisibility,
    bool? showOnlineStatus,
  }) => _repository.updatePreferences(
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
}

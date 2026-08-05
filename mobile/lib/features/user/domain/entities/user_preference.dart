import 'package:flutter/foundation.dart';

/// UI theme options matching the backend `Theme` enum.
enum AppTheme { light, dark, system }

/// User preference settings (notifications, theme, language, etc.).
@immutable
class UserPreference {
  const UserPreference({
    required this.id,
    this.language = 'en',
    this.theme = AppTheme.system,
    this.timeZone = 'UTC',
    this.emailNotifications = true,
    this.pushNotifications = true,
    this.smsNotifications = false,
    this.marketingEmails = false,
    this.profileVisibility = true,
    this.showOnlineStatus = true,
  });

  final String id;
  final String language;
  final AppTheme theme;
  final String timeZone;
  final bool emailNotifications;
  final bool pushNotifications;
  final bool smsNotifications;
  final bool marketingEmails;
  final bool profileVisibility;
  final bool showOnlineStatus;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is UserPreference &&
          runtimeType == other.runtimeType &&
          id == other.id &&
          language == other.language &&
          theme == other.theme &&
          timeZone == other.timeZone &&
          emailNotifications == other.emailNotifications &&
          pushNotifications == other.pushNotifications &&
          smsNotifications == other.smsNotifications &&
          marketingEmails == other.marketingEmails &&
          profileVisibility == other.profileVisibility &&
          showOnlineStatus == other.showOnlineStatus;

  @override
  int get hashCode => Object.hash(
    id,
    language,
    theme,
    timeZone,
    emailNotifications,
    pushNotifications,
    smsNotifications,
    marketingEmails,
    profileVisibility,
    showOnlineStatus,
  );
}

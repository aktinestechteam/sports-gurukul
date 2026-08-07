/// Well-known keys for key-value storage (preferences and secure storage).
///
/// Storage reads and writes must reference these constants instead of
/// spelling keys inline. Keys are snake_case and namespaced by feature where
/// the owner is known.
abstract final class StorageKeys {
  /// Auth tokens (secure storage).
  static const String authToken = 'auth_token';
  static const String refreshToken = 'refresh_token';

  /// Cached authenticated session (secure storage).
  static const String authSession = 'auth_session';

  /// Stable device identifier (secure storage).
  static const String deviceId = 'device_id';

  /// UI preferences (shared preferences).
  static const String themeMode = 'theme_mode';
  static const String locale = 'locale';

  /// Onboarding & lifecycle (shared preferences).
  static const String onboardingCompleted = 'onboarding_completed';
  static const String lastSyncAt = 'last_sync_at';

  /// Cacheable payloads (shared preferences).
  static const String cachedDashboard = 'cached_dashboard';
}

/// Centralized application route paths.
///
/// Paths are snake_case and never hardcoded inside features. The app-layer
/// `RoutePaths` type aliases this class, so this remains the single source
/// of truth. Feature routes are registered by their owning feature.
abstract final class RouteConstants {
  /// Bootstrap splash screen.
  static const String splash = '/';

  /// Email/password sign-in.
  static const String login = '/login';

  /// Account creation.
  static const String signUp = '/sign-up';

  /// Password reset email request.
  static const String forgotPassword = '/forgot-password';

  /// New password entry (token supplied via `?token=...`).
  static const String resetPassword = '/reset-password';

  /// Welcome/onboarding path for brand-new users.
  static const String welcome = '/welcome';

  /// Create-academy flow.
  static const String createAcademy = '/create-academy';

  /// Edit-academy flow (academy admins).
  static const String editAcademy = '/academy/edit';

  /// Join-academy flow (navigation placeholder until the feature lands).
  static const String joinAcademy = '/join-academy';

  /// Placeholder dashboard.
  static const String dashboard = '/dashboard';

  /// Academy-admin dashboard reached after creating an academy (placeholder
  /// until the academy dashboard feature lands).
  static const String academyDashboard = '/academy-dashboard';

  /// Current user's profile.
  static const String profile = '/profile';

  /// Edit current user's profile.
  static const String editProfile = '/profile/edit';

  /// Edit current user's preferences.
  static const String editPreferences = '/profile/preferences';
}

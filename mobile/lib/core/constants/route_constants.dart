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

  /// Placeholder dashboard.
  static const String dashboard = '/dashboard';
}

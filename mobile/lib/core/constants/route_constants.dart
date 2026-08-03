/// Centralized application route paths.
///
/// Paths are snake_case and never hardcoded inside features. The app-layer
/// `RoutePaths` type aliases this class, so this remains the single source
/// of truth. Feature routes are registered by their owning feature.
abstract final class RouteConstants {
  /// Bootstrap splash screen.
  static const String splash = '/';

  /// Placeholder dashboard.
  static const String dashboard = '/dashboard';
}

/// Centralized route paths for the Sports Gurukul application.
///
/// Route paths are snake_case and never hardcoded inside features.
/// Feature routes are registered by their owning feature in a later sprint;
/// app-level bootstrap routes (splash, dashboard) live here.
abstract final class RoutePaths {
  static const String splash = '/';
  static const String dashboard = '/dashboard';
}

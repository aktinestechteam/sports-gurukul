/// Route guard contract for the Sports Gurukul application.
///
/// Implementations will enforce authentication and role-based authorization
/// (Athlete, Parent, Coach, Academy, Super Admin) in a later sprint.
///
/// Reference: docs/mobile/09-Implementation/07-Navigation.md
abstract class RouteGuards {
  const RouteGuards();

  /// Returns the redirect target when the route is not allowed, or null
  /// to allow navigation.
  String? redirect();
}

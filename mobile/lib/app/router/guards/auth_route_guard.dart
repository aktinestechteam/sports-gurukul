import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';

/// Redirects navigation based on the resolved authentication state.
///
/// While the session is unknown (splash in progress) nothing is redirected.
/// Once resolved, guest routes redirect signed-in users to the dashboard and
/// protected routes redirect signed-out users to login. The splash itself
/// hands off to the dashboard or the login screen.
class AuthRouteGuard {
  const AuthRouteGuard();

  /// The redirect target for [location] under [state], or null to proceed.
  String? redirect(String location, AuthState state) {
    if (location == RoutePaths.splash) {
      return switch (state) {
        AuthUnknown() => null,
        AuthAuthenticated() => RoutePaths.dashboard,
        AuthUnauthenticated() => RoutePaths.login,
      };
    }

    final isGuestRoute = switch (location) {
      RoutePaths.login ||
      RoutePaths.signUp ||
      RoutePaths.forgotPassword ||
      RoutePaths.resetPassword => true,
      _ => false,
    };
    final isProtectedRoute = switch (location) {
      RoutePaths.dashboard ||
      RoutePaths.profile ||
      RoutePaths.editProfile ||
      RoutePaths.editPreferences => true,
      _ => false,
    };

    return switch (state) {
      AuthUnknown() => null,
      AuthAuthenticated() when isGuestRoute => RoutePaths.dashboard,
      AuthUnauthenticated() when isProtectedRoute => RoutePaths.login,
      _ => null,
    };
  }
}

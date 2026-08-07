import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/onboarding/presentation/providers/onboarding_controller.dart';

/// Redirects navigation based on the resolved authentication state.
///
/// While the session is unknown, guest routes proceed unredirected but
/// protected routes hand back to the splash so session restore always runs.
/// Once resolved, guest routes redirect signed-in users to the dashboard and
/// protected routes redirect signed-out users to login. The splash itself
/// hands off to login, the welcome screen (brand-new users) or the dashboard
/// once the onboarding state resolves.
class AuthRouteGuard {
  const AuthRouteGuard();

  /// The redirect target for [location] under [authState] (and the resolved
  /// [OnboardingState]), or null to proceed.
  String? redirect(
    String location,
    AuthState authState, {
    OnboardingState? onboardingState,
  }) {
    if (location == RoutePaths.splash) {
      return switch (authState) {
        AuthUnknown() => null,
        AuthAuthenticated() => _postAuthTarget(onboardingState),
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
      RoutePaths.welcome ||
      RoutePaths.createAcademy ||
      RoutePaths.editAcademy ||
      RoutePaths.joinAcademy ||
      RoutePaths.dashboard ||
      RoutePaths.academyDashboard ||
      RoutePaths.profile ||
      RoutePaths.editProfile ||
      RoutePaths.editPreferences => true,
      _ => false,
    };

    return switch (authState) {
      // While the session is unknown, protected routes must hand back to the
      // splash so session restore always runs; otherwise a web refresh or deep
      // link can render the welcome screen against an unresolved session and
      // get stuck on its empty state.
      AuthUnknown() when isProtectedRoute => RoutePaths.splash,
      AuthUnknown() => null,
      AuthAuthenticated() when isGuestRoute => _postAuthTarget(onboardingState),
      // An established user must never be stuck on the welcome screen: it only
      // exists for brand-new users that have not picked an onboarding path.
      AuthAuthenticated()
          when isProtectedRoute &&
              location == RoutePaths.welcome &&
              _isEstablishedUser(onboardingState) =>
        RoutePaths.dashboard,
      // A signed-in user that has not finished onboarding (including one whose
      // current-user resolution failed) must not reach any protected route
      // other than the welcome screen itself; otherwise a brand-new account
      // could land on the normal dashboard.
      AuthAuthenticated()
          when isProtectedRoute &&
              location != RoutePaths.welcome &&
              _isIncompleteNewUser(onboardingState) =>
        RoutePaths.welcome,
      AuthUnauthenticated() when isProtectedRoute => RoutePaths.login,
      _ => null,
    };
  }

  /// Whether [onboardingState] describes a signed-in brand-new user that has
  /// not picked an onboarding path yet, or a user whose application session
  /// could not be resolved (treated as incomplete until a retry succeeds).
  bool _isIncompleteNewUser(OnboardingState? onboardingState) =>
      (onboardingState is OnboardingResolved &&
          onboardingState.session.isNewUser) ||
      onboardingState is OnboardingError;

  /// Whether [onboardingState] describes a signed-in established user that
  /// must not stay on the welcome screen.
  bool _isEstablishedUser(OnboardingState? onboardingState) =>
      (onboardingState is OnboardingResolved &&
          !onboardingState.session.isNewUser) ||
      onboardingState is OnboardingCompleted;

  /// Where a signed-in user lands after the splash: the welcome screen for
  /// brand-new users that have not picked an onboarding path and for users
  /// whose session could not be resolved (the welcome screen renders the
  /// retry state), the dashboard otherwise. Returns null while the onboarding
  /// state is still resolving so the splash stays up.
  String? _postAuthTarget(OnboardingState? onboardingState) {
    if (onboardingState is OnboardingLoading ||
        onboardingState is OnboardingIdle) {
      return null;
    }
    if (onboardingState is OnboardingResolved &&
        onboardingState.session.isNewUser) {
      return RoutePaths.welcome;
    }
    if (onboardingState is OnboardingError) {
      return RoutePaths.welcome;
    }
    return RoutePaths.dashboard;
  }
}

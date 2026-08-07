import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/bootstrap/splash_page.dart';
import 'package:sports_gurukul/app/router/guards/auth_route_guard.dart';
import 'package:sports_gurukul/app/router/route_names.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/features/academy/create/presentation/pages/create_academy_page.dart';
import 'package:sports_gurukul/features/academy/create/presentation/pages/edit_academy_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/forgot_password_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/login_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/reset_password_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/sign_up_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/dashboard/presentation/pages/dashboard_page.dart';
import 'package:sports_gurukul/features/onboarding/presentation/pages/join_academy_page.dart';
import 'package:sports_gurukul/features/onboarding/presentation/pages/welcome_page.dart';
import 'package:sports_gurukul/features/onboarding/presentation/providers/onboarding_controller.dart';
import 'package:sports_gurukul/features/user/presentation/pages/edit_preferences_page.dart';
import 'package:sports_gurukul/features/user/presentation/pages/edit_profile_page.dart';
import 'package:sports_gurukul/features/user/presentation/pages/my_profile_page.dart';

/// Notifies GoRouter (via `refreshListenable`) that redirects should be
/// re-evaluated in place.
class _RouterRefresh extends ChangeNotifier {
  void refresh() => notifyListeners();
}

/// Application-level [GoRouter] configuration.
///
/// The router watches the authentication state: the splash hands off to login
/// or the dashboard once the session resolves, and [AuthRouteGuard] keeps
/// guest and protected routes out of reach for the wrong audience. Deep links
/// carry the password reset token via the `token` query parameter.
///
/// Onboarding transitions only re-evaluate redirects through the GoRouter
/// `refreshListenable`; the router instance must not be recreated for
/// them, otherwise the route stack would reset and a user-initiated handoff
/// (e.g. the welcome screen navigating to `/create-academy`) would be lost.
final appRouterProvider = Provider<GoRouter>((ref) {
  const guard = AuthRouteGuard();
  final onboardingRefresh = _RouterRefresh();
  ref
    ..watch(authControllerProvider)
    ..onDispose(onboardingRefresh.dispose)
    ..listen(
      onboardingControllerProvider,
      (_, _) => onboardingRefresh.refresh(),
    );

  return GoRouter(
    initialLocation: RoutePaths.splash,
    refreshListenable: onboardingRefresh,
    redirect: (context, state) {
      final authState = ref.read(authControllerProvider);
      final onboardingState = ref.read(onboardingControllerProvider);
      return guard.redirect(
        state.matchedLocation,
        authState,
        onboardingState: onboardingState,
      );
    },
    routes: <RouteBase>[
      GoRoute(
        name: RouteNames.splash,
        path: RoutePaths.splash,
        builder: (_, _) => const SplashPage(),
      ),
      GoRoute(
        name: RouteNames.login,
        path: RoutePaths.login,
        builder: (_, _) => const LoginPage(),
      ),
      GoRoute(
        name: RouteNames.signUp,
        path: RoutePaths.signUp,
        builder: (_, _) => const SignUpPage(),
      ),
      GoRoute(
        name: RouteNames.forgotPassword,
        path: RoutePaths.forgotPassword,
        builder: (_, _) => const ForgotPasswordPage(),
      ),
      GoRoute(
        name: RouteNames.resetPassword,
        path: RoutePaths.resetPassword,
        builder: (_, state) => ResetPasswordPage(
          token: state.uri.queryParameters['token'],
        ),
      ),
      GoRoute(
        name: RouteNames.welcome,
        path: RoutePaths.welcome,
        builder: (_, _) => const WelcomePage(),
      ),
      GoRoute(
        name: RouteNames.createAcademy,
        path: RoutePaths.createAcademy,
        builder: (_, _) => const CreateAcademyPage(),
      ),
      GoRoute(
        name: RouteNames.editAcademy,
        path: RoutePaths.editAcademy,
        builder: (_, _) => const EditAcademyPage(),
      ),
      GoRoute(
        name: RouteNames.joinAcademy,
        path: RoutePaths.joinAcademy,
        builder: (_, _) => const JoinAcademyPage(),
      ),
      GoRoute(
        name: RouteNames.dashboard,
        path: RoutePaths.dashboard,
        builder: (_, _) => const DashboardPage(),
      ),
      GoRoute(
        name: RouteNames.academyDashboard,
        path: RoutePaths.academyDashboard,
        builder: (_, _) => const DashboardPage(),
      ),
      GoRoute(
        name: RouteNames.profile,
        path: RoutePaths.profile,
        builder: (_, _) => const MyProfilePage(),
      ),
      GoRoute(
        name: RouteNames.editProfile,
        path: RoutePaths.editProfile,
        builder: (_, _) => const EditProfilePage(),
      ),
      GoRoute(
        name: RouteNames.editPreferences,
        path: RoutePaths.editPreferences,
        builder: (_, _) => const EditPreferencesPage(),
      ),
    ],
  );
});

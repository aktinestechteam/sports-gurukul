import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/bootstrap/splash_page.dart';
import 'package:sports_gurukul/app/router/guards/auth_route_guard.dart';
import 'package:sports_gurukul/app/router/route_names.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/forgot_password_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/login_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/reset_password_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/sign_up_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/dashboard/presentation/pages/dashboard_page.dart';

/// Application-level [GoRouter] configuration.
///
/// The router watches the authentication state: the splash hands off to login
/// or the dashboard once the session resolves, and [AuthRouteGuard] keeps
/// guest and protected routes out of reach for the wrong audience. Deep links
/// carry the password reset token via the `token` query parameter.
final appRouterProvider = Provider<GoRouter>((ref) {
  const guard = AuthRouteGuard();
  ref.watch(authControllerProvider);

  return GoRouter(
    initialLocation: RoutePaths.splash,
    redirect: (context, state) {
      final authState = ref.read(authControllerProvider);
      return guard.redirect(state.matchedLocation, authState);
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
        name: RouteNames.dashboard,
        path: RoutePaths.dashboard,
        builder: (_, _) => const DashboardPage(),
      ),
    ],
  );
});

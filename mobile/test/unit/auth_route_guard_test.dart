import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/app/router/guards/auth_route_guard.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';

import '../helpers/auth_test_helper.dart';

void main() {
  const guard = AuthRouteGuard();
  final authenticated = AuthAuthenticated(testAuthSession());

  group('AuthRouteGuard splash handoff', () {
    test('lets the splash wait while state is unknown', () {
      expect(guard.redirect(RoutePaths.splash, const AuthUnknown()), isNull);
    });

    test('sends signed-in users from the splash to the dashboard', () {
      expect(
        guard.redirect(RoutePaths.splash, authenticated),
        RoutePaths.dashboard,
      );
    });

    test('sends signed-out users from the splash to login', () {
      expect(
        guard.redirect(RoutePaths.splash, const AuthUnauthenticated()),
        RoutePaths.login,
      );
    });
  });

  group('AuthRouteGuard guest routes', () {
    test('allows guests onto the login route', () {
      expect(
        guard.redirect(RoutePaths.login, const AuthUnauthenticated()),
        isNull,
      );
    });

    test('keeps signed-in users off guest routes', () {
      expect(
        guard.redirect(RoutePaths.login, authenticated),
        RoutePaths.dashboard,
      );
      expect(
        guard.redirect(RoutePaths.forgotPassword, authenticated),
        RoutePaths.dashboard,
      );
      expect(
        guard.redirect(RoutePaths.resetPassword, authenticated),
        RoutePaths.dashboard,
      );
    });

    test('does not interrupt while state is unknown', () {
      expect(guard.redirect(RoutePaths.login, const AuthUnknown()), isNull);
    });
  });

  group('AuthRouteGuard protected routes', () {
    test('allows signed-in users onto the dashboard', () {
      expect(guard.redirect(RoutePaths.dashboard, authenticated), isNull);
    });

    test('redirects signed-out users to login', () {
      expect(
        guard.redirect(RoutePaths.dashboard, const AuthUnauthenticated()),
        RoutePaths.login,
      );
    });

    test('waits for state resolution', () {
      expect(guard.redirect(RoutePaths.dashboard, const AuthUnknown()), isNull);
    });
  });
}

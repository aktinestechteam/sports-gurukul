import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/app/router/guards/auth_route_guard.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/onboarding/presentation/providers/onboarding_controller.dart';

import '../helpers/auth_test_helper.dart';
import '../helpers/onboarding_test_helper.dart';

void main() {
  const guard = AuthRouteGuard();
  final authenticated = AuthAuthenticated(testAuthSession());
  const onboardingLoading = OnboardingLoading();
  final onboardingNewUser = OnboardingResolved(testNewUserSession());
  final onboardingMember = OnboardingResolved(testMemberSession());
  final onboardingCompleted = OnboardingCompleted(testNewUserSession());
  const onboardingError = OnboardingError(UnknownFailure(message: 'boom'));

  group('AuthRouteGuard splash handoff', () {
    test('lets the splash wait while state is unknown', () {
      expect(
        guard.redirect(RoutePaths.splash, const AuthUnknown()),
        isNull,
      );
    });

    test('keeps the splash up while onboarding is still resolving', () {
      expect(
        guard.redirect(
          RoutePaths.splash,
          authenticated,
          onboardingState: onboardingLoading,
        ),
        isNull,
      );
    });

    test('sends brand-new users from the splash to the welcome screen', () {
      expect(
        guard.redirect(
          RoutePaths.splash,
          authenticated,
          onboardingState: onboardingNewUser,
        ),
        RoutePaths.welcome,
      );
    });

    test(
      'sends users whose session failed to resolve to the welcome screen',
      () {
        expect(
          guard.redirect(
            RoutePaths.splash,
            authenticated,
            onboardingState: onboardingError,
          ),
          RoutePaths.welcome,
        );
      },
    );

    test('sends established users from the splash to the dashboard', () {
      expect(
        guard.redirect(
          RoutePaths.splash,
          authenticated,
          onboardingState: onboardingMember,
        ),
        RoutePaths.dashboard,
      );
    });

    test('does not send a completed onboarding back to welcome', () {
      expect(
        guard.redirect(
          RoutePaths.splash,
          authenticated,
          onboardingState: onboardingCompleted,
        ),
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
        guard.redirect(
          RoutePaths.login,
          authenticated,
          onboardingState: onboardingMember,
        ),
        RoutePaths.dashboard,
      );
      expect(
        guard.redirect(
          RoutePaths.forgotPassword,
          authenticated,
          onboardingState: onboardingMember,
        ),
        RoutePaths.dashboard,
      );
      expect(
        guard.redirect(
          RoutePaths.resetPassword,
          authenticated,
          onboardingState: onboardingMember,
        ),
        RoutePaths.dashboard,
      );
    });

    test('sends brand-new users from guest routes to the welcome screen', () {
      expect(
        guard.redirect(
          RoutePaths.login,
          authenticated,
          onboardingState: onboardingNewUser,
        ),
        RoutePaths.welcome,
      );
    });

    test('does not interrupt guests while state is unknown', () {
      expect(guard.redirect(RoutePaths.login, const AuthUnknown()), isNull);
    });
  });

  group('AuthRouteGuard protected routes', () {
    test('allows signed-in users onto the dashboard', () {
      expect(
        guard.redirect(
          RoutePaths.dashboard,
          authenticated,
          onboardingState: onboardingMember,
        ),
        isNull,
      );
    });

    test('allows brand-new users onto the welcome screen', () {
      expect(
        guard.redirect(
          RoutePaths.welcome,
          authenticated,
          onboardingState: onboardingNewUser,
        ),
        isNull,
      );
    });

    test('sends incomplete-onboarding new users back from the dashboard', () {
      expect(
        guard.redirect(
          RoutePaths.dashboard,
          authenticated,
          onboardingState: onboardingNewUser,
        ),
        RoutePaths.welcome,
      );
      expect(
        guard.redirect(
          RoutePaths.profile,
          authenticated,
          onboardingState: onboardingNewUser,
        ),
        RoutePaths.welcome,
      );
    });

    test('sends incomplete-onboarding new users back from academy flows', () {
      expect(
        guard.redirect(
          RoutePaths.createAcademy,
          authenticated,
          onboardingState: onboardingNewUser,
        ),
        RoutePaths.welcome,
      );
      expect(
        guard.redirect(
          RoutePaths.joinAcademy,
          authenticated,
          onboardingState: onboardingNewUser,
        ),
        RoutePaths.welcome,
      );
    });

    test('lets completed-onboarding users onto the dashboard', () {
      expect(
        guard.redirect(
          RoutePaths.dashboard,
          authenticated,
          onboardingState: onboardingCompleted,
        ),
        isNull,
      );
    });

    test(
      'sends unresolved users back from the dashboard to the welcome screen',
      () {
        expect(
          guard.redirect(
            RoutePaths.dashboard,
            authenticated,
            onboardingState: onboardingError,
          ),
          RoutePaths.welcome,
        );
        expect(
          guard.redirect(
            RoutePaths.profile,
            authenticated,
            onboardingState: onboardingError,
          ),
          RoutePaths.welcome,
        );
      },
    );

    test('keeps an unresolved user on the welcome screen for retry', () {
      expect(
        guard.redirect(
          RoutePaths.welcome,
          authenticated,
          onboardingState: onboardingError,
        ),
        isNull,
      );
    });

    test('sends established users away from the welcome screen', () {
      expect(
        guard.redirect(
          RoutePaths.welcome,
          authenticated,
          onboardingState: onboardingMember,
        ),
        RoutePaths.dashboard,
      );
      expect(
        guard.redirect(
          RoutePaths.welcome,
          authenticated,
          onboardingState: onboardingCompleted,
        ),
        RoutePaths.dashboard,
      );
    });

    test('allows members onto the academy placeholder routes', () {
      expect(
        guard.redirect(
          RoutePaths.createAcademy,
          authenticated,
          onboardingState: onboardingMember,
        ),
        isNull,
      );
      expect(
        guard.redirect(
          RoutePaths.joinAcademy,
          authenticated,
          onboardingState: onboardingMember,
        ),
        isNull,
      );
    });

    test('redirects signed-out users from protected routes to login', () {
      expect(
        guard.redirect(RoutePaths.dashboard, const AuthUnauthenticated()),
        RoutePaths.login,
      );
      expect(
        guard.redirect(RoutePaths.welcome, const AuthUnauthenticated()),
        RoutePaths.login,
      );
      expect(
        guard.redirect(RoutePaths.createAcademy, const AuthUnauthenticated()),
        RoutePaths.login,
      );
      expect(
        guard.redirect(RoutePaths.joinAcademy, const AuthUnauthenticated()),
        RoutePaths.login,
      );
    });

    test(
      'sends an unresolved session from protected routes back to the splash',
      () {
        expect(
          guard.redirect(RoutePaths.dashboard, const AuthUnknown()),
          RoutePaths.splash,
        );
        expect(
          guard.redirect(RoutePaths.welcome, const AuthUnknown()),
          RoutePaths.splash,
        );
        expect(
          guard.redirect(RoutePaths.createAcademy, const AuthUnknown()),
          RoutePaths.splash,
        );
      },
    );
  });
}

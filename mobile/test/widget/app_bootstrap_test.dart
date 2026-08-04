import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';

import '../helpers/auth_test_helper.dart';

/// Verifies the bootstrap flow around authentication:
/// splash branding renders, then the auth-aware router hands off to login for
/// signed-out users and to the dashboard for signed-in users.
void main() {
  Future<void> pumpApp(WidgetTester tester, {AuthState? state}) async {
    await tester.pumpWidget(buildTestApp(state: state));
    await tester.pump();
  }

  testWidgets('shows splash branding on startup', (tester) async {
    await pumpApp(tester);

    expect(find.text('Sports Gurukul'), findsOneWidget);
    expect(find.text('Train • Compete • Excel'), findsOneWidget);
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
  });

  testWidgets('signed-out startup routes to login after initialization', (
    tester,
  ) async {
    await pumpApp(tester, state: const AuthUnauthenticated());

    await tester.pump(const Duration(seconds: 2));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.text('Welcome!'), findsOneWidget);
    expect(find.byType(CircularProgressIndicator), findsNothing);
  });

  testWidgets(
    'signed-in startup routes to the dashboard after initialization',
    (tester) async {
      await pumpApp(tester, state: AuthAuthenticated(testAuthSession()));

      await tester.pump(const Duration(seconds: 2));
      await tester.pump();
      await tester.pump(const Duration(milliseconds: 400));
      await tester.pump(const Duration(milliseconds: 400));

      expect(
        find.text('Your performance, one glance away.'),
        findsOneWidget,
      );
      expect(find.byType(CircularProgressIndicator), findsNothing);
    },
  );

  testWidgets('signed-out users cannot reach the dashboard directly', (
    tester,
  ) async {
    await pumpApp(tester, state: const AuthUnauthenticated());
    final BuildContext context = tester.element(find.byType(Scaffold).first);
    GoRouter.of(context).go(RoutePaths.dashboard);
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.text('Welcome!'), findsOneWidget);
  });

  testWidgets('dashboard route is reachable directly when signed in', (
    tester,
  ) async {
    await pumpApp(tester, state: AuthAuthenticated(testAuthSession()));
    final BuildContext context = tester.element(find.byType(Scaffold).first);
    GoRouter.of(context).go(RoutePaths.dashboard);
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.text('Your performance, one glance away.'), findsOneWidget);
  });
}

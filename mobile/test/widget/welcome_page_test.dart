import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/features/academy/create/presentation/pages/create_academy_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/dashboard/presentation/pages/dashboard_page.dart';
import 'package:sports_gurukul/features/onboarding/presentation/pages/join_academy_page.dart';
import 'package:sports_gurukul/features/onboarding/presentation/pages/welcome_page.dart';

import '../helpers/auth_test_helper.dart';
import '../helpers/onboarding_test_helper.dart';

/// Verifies the welcome screen flow for brand-new users: the identity header,
/// the three onboarding actions and the navigation-only behavior.
void main() {
  Future<void> pumpWelcome(WidgetTester tester) async {
    await tester.pumpWidget(
      buildTestApp(
        state: AuthAuthenticated(testAuthSession()),
        currentUser: testNewUserCurrentUser(),
      ),
    );
    await tester.pump(const Duration(seconds: 2));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));
  }

  /// Unmounts the previous app so a fresh [ProviderScope] (and therefore a
  /// fresh onboarding lifecycle) is created on the next pump.
  Future<void> tearDownApp(WidgetTester tester) async {
    await tester.pumpWidget(const SizedBox.shrink());
    await tester.pump();
  }

  testWidgets('renders identity and the three onboarding actions', (
    tester,
  ) async {
    await pumpWelcome(tester);

    expect(find.byType(WelcomePage), findsOneWidget);
    expect(find.text('Welcome to Sports Gurukul!'), findsOneWidget);
    expect(find.text('Test Player'), findsOneWidget);
    expect(find.text('player@example.com'), findsOneWidget);
    expect(find.text('Create My Academy'), findsOneWidget);
    expect(find.text('Join Existing Academy'), findsOneWidget);
    expect(find.text('Explore Application'), findsOneWidget);
  });

  testWidgets('creating an academy routes to the create-academy placeholder', (
    tester,
  ) async {
    await pumpWelcome(tester);

    await tester.ensureVisible(find.text('Create My Academy'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Create My Academy'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.byType(CreateAcademyPage), findsOneWidget);
    expect(find.byType(WelcomePage), findsNothing);

    await tearDownApp(tester);
  });

  testWidgets('joining an academy routes to the join-academy placeholder', (
    tester,
  ) async {
    await pumpWelcome(tester);

    await tester.ensureVisible(find.text('Join Existing Academy'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Join Existing Academy'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.byType(JoinAcademyPage), findsOneWidget);
    expect(find.byType(WelcomePage), findsNothing);

    await tearDownApp(tester);
  });

  testWidgets('exploring routes to the dashboard and completes onboarding', (
    tester,
  ) async {
    await pumpWelcome(tester);

    await tester.ensureVisible(find.text('Explore Application'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Explore Application'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.byType(DashboardPage), findsOneWidget);
  });

  testWidgets('every action completes onboarding and leaves the welcome path', (
    tester,
  ) async {
    for (final action in <String>[
      'Create My Academy',
      'Join Existing Academy',
      'Explore Application',
    ]) {
      await pumpWelcome(tester);

      await tester.ensureVisible(find.text(action));
      await tester.pumpAndSettle();
      await tester.tap(find.text(action));
      await tester.pump();
      await tester.pump(const Duration(milliseconds: 400));
      await tester.pump(const Duration(milliseconds: 400));

      expect(find.byType(WelcomePage), findsNothing);

      await tearDownApp(tester);
    }
  });

  testWidgets('welcome route stays reachable directly for brand-new users', (
    tester,
  ) async {
    await pumpWelcome(tester);
    expect(find.byType(WelcomePage), findsOneWidget);
    expect(find.text('Welcome to Sports Gurukul!'), findsOneWidget);
  });

  test('welcome route path and name are wired', () {
    expect(RoutePaths.welcome, '/welcome');
    expect(RoutePaths.createAcademy, '/create-academy');
    expect(RoutePaths.joinAcademy, '/join-academy');
  });
}

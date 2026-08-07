import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/app/app.dart';
import 'package:sports_gurukul/features/academy/create/application/my_academy_provider.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/dashboard/presentation/pages/dashboard_page.dart';
import 'package:sports_gurukul/features/dashboard/presentation/widgets/new_user_dashboard.dart';
import 'package:sports_gurukul/features/onboarding/application/onboarding_providers.dart';

import '../helpers/auth_test_helper.dart';
import '../helpers/onboarding_test_helper.dart';

/// Verifies the academy-admin dashboard header branding:
/// - an academy with a logo shows the academy name (replacing the generic
///   subtitle) and renders the logo in place of the initials avatar;
/// - an admin without an academy keeps the initials avatar and subtitle.
void main() {
  final academy = Academy(
    id: 'ac-1',
    academyCode: 'SG-0001',
    name: 'Aurora Sports Academy',
    email: 'academy@example.com',
    phone: '9876543210',
    status: 'Active',
    verificationStatus: 'Verified',
    logoUrl: 'https://cdn.example.com/logo.png',
    createdAt: DateTime.utc(2026),
  );

  Future<void> pumpAcademyDashboard(
    WidgetTester tester, {
    required Academy? myAcademy,
  }) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          authControllerProvider.overrideWith(
            () => FakeAuthController(
              AuthAuthenticated(testAuthSession()),
            ),
          ),
          currentUserProvider.overrideWith(
            (ref) async => testAcademyAdminCurrentUser(),
          ),
          myAcademyProvider.overrideWith((ref) async => myAcademy),
        ],
        child: const SportsGurukulApp(),
      ),
    );
    await tester.pump(const Duration(seconds: 2));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));
  }

  testWidgets('an academy admin with an academy sees the logo and name', (
    tester,
  ) async {
    await pumpAcademyDashboard(tester, myAcademy: academy);

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.byType(NewUserDashboard), findsNothing);
    expect(find.text('Aurora Sports Academy'), findsOneWidget);
    expect(find.text('Your performance, one glance away.'), findsNothing);
    expect(find.byType(Image), findsOneWidget);
    expect(find.text('TP'), findsOneWidget);
  });

  testWidgets('an academy admin without an academy keeps the initials header', (
    tester,
  ) async {
    await pumpAcademyDashboard(tester, myAcademy: null);

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.text('Aurora Sports Academy'), findsNothing);
    expect(find.text('Your performance, one glance away.'), findsOneWidget);
    expect(find.text('TP'), findsOneWidget);
  });
}

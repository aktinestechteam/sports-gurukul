import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/dashboard/presentation/pages/dashboard_page.dart';
import 'package:sports_gurukul/features/dashboard/presentation/widgets/new_user_dashboard.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/current_user.dart';
import 'package:sports_gurukul/features/onboarding/presentation/pages/welcome_page.dart';

import '../helpers/auth_test_helper.dart';
import '../helpers/onboarding_test_helper.dart';

/// Verifies the profile role badge across every user state:
/// - a brand-new user is labelled "New User", never "Athlete", even though the
///   backend assigns the default `Athlete` role at registration;
/// - established business roles (Coach, Academy Admin, Athlete) and the
///   pending-approval state drive the badge from the resolved session.
void main() {
  Future<void> pumpApp(
    WidgetTester tester, {
    required CurrentUser? currentUser,
  }) async {
    await tester.pumpWidget(
      buildTestApp(
        state: AuthAuthenticated(testAuthSession()),
        currentUser: currentUser,
      ),
    );
    await tester.pump(const Duration(seconds: 2));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));
  }

  Future<void> explore(WidgetTester tester) async {
    await tester.ensureVisible(find.text('Explore Application'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Explore Application'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));
  }

  testWidgets('a brand-new user is never labelled Athlete', (tester) async {
    await pumpApp(tester, currentUser: testNewUserCurrentUser());

    expect(find.byType(WelcomePage), findsOneWidget);
    expect(find.text('New User'), findsOneWidget);
    expect(find.text('Athlete'), findsNothing);

    await explore(tester);

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.byType(NewUserDashboard), findsOneWidget);
    expect(find.text('New User'), findsOneWidget);
    expect(find.text('Athlete'), findsNothing);
  });

  testWidgets('a coach sees the Coach badge on the full dashboard', (
    tester,
  ) async {
    await pumpApp(tester, currentUser: testMemberCurrentUser());

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.text('Coach'), findsOneWidget);
    expect(find.text('New User'), findsNothing);
  });

  testWidgets('an academy admin sees the Academy Admin badge', (tester) async {
    await pumpApp(tester, currentUser: testAcademyAdminCurrentUser());

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.text('Academy Admin'), findsOneWidget);
  });

  testWidgets('an academy-assigned athlete sees the Athlete badge', (
    tester,
  ) async {
    await pumpApp(tester, currentUser: testAthleteMemberCurrentUser());

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.text('Athlete'), findsOneWidget);
  });

  testWidgets('a pending membership shows the Pending Approval badge', (
    tester,
  ) async {
    await pumpApp(tester, currentUser: testPendingApprovalCurrentUser());

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.text('Pending Approval'), findsOneWidget);
    expect(find.text('Athlete'), findsNothing);
  });
}

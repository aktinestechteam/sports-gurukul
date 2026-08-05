import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/dashboard/presentation/pages/dashboard_page.dart';
import 'package:sports_gurukul/features/dashboard/presentation/widgets/new_user_dashboard.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/current_user.dart';
import 'package:sports_gurukul/features/onboarding/presentation/pages/welcome_page.dart';

import '../helpers/auth_test_helper.dart';
import '../helpers/onboarding_test_helper.dart';

/// Verifies the onboarding-aware dashboard:
/// - a brand-new user is routed to the welcome screen and, after exploring,
///   reaches the dashboard still in the limited new-user state (onboarding
///   actions instead of the normal dashboard content);
/// - an established academy member sees the normal dashboard.
void main() {
  Future<void> pumpDashboard(
    WidgetTester tester, {
    required CurrentUser currentUser,
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

  testWidgets('new user without an academy sees onboarding actions', (
    tester,
  ) async {
    await pumpDashboard(tester, currentUser: testNewUserCurrentUser());

    expect(find.byType(WelcomePage), findsOneWidget);
    expect(find.byType(DashboardPage), findsNothing);
    expect(find.text('Create My Academy'), findsOneWidget);
    expect(find.text('Join Existing Academy'), findsOneWidget);
    expect(find.text('Explore Application'), findsOneWidget);
    expect(find.text('Your performance, one glance away.'), findsNothing);

    await tester.ensureVisible(find.text('Explore Application'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Explore Application'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.byType(NewUserDashboard), findsOneWidget);
    expect(find.text('Create My Academy'), findsOneWidget);
    expect(find.text('Join Existing Academy'), findsOneWidget);
    expect(find.text('Explore Application'), findsOneWidget);
    expect(find.text('Your performance, one glance away.'), findsNothing);
  });

  testWidgets('academy member sees the normal dashboard', (tester) async {
    await pumpDashboard(tester, currentUser: testMemberCurrentUser());

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.byType(NewUserDashboard), findsNothing);
    expect(find.text('Your performance, one glance away.'), findsOneWidget);
    expect(find.text('Quick actions'), findsOneWidget);
  });
}

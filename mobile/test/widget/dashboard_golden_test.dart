import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/dashboard/presentation/pages/dashboard_page.dart';

import '../helpers/auth_test_helper.dart';
import '../helpers/onboarding_test_helper.dart';

/// Golden verification of the placeholder dashboard.
///
/// Uses Flutter's built-in `matchesGoldenFile` (the discontinued
/// `golden_toolkit` package is intentionally not used). Goldens are rendered
/// with the test font (Ahem) and are platform-specific - regenerate with
/// `flutter test --update-goldens test/widget/dashboard_golden_test.dart`.
void main() {
  testWidgets('dashboard matches the golden image', (tester) async {
    await tester.pumpWidget(
      buildTestApp(
        state: AuthAuthenticated(testAuthSession()),
        currentUser: testMemberCurrentUser(),
      ),
    );
    await tester.pump(const Duration(seconds: 2));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.byType(DashboardPage), findsOneWidget);

    await expectLater(
      find.byType(DashboardPage),
      matchesGoldenFile('goldens/dashboard_page.png'),
    );
  });
}

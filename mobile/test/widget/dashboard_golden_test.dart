import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/app/app.dart';
import 'package:sports_gurukul/features/dashboard/presentation/pages/dashboard_page.dart';

/// Golden verification of the placeholder dashboard.
///
/// Uses Flutter's built-in `matchesGoldenFile` (the discontinued
/// `golden_toolkit` package is intentionally not used). Goldens are rendered
/// with the test font (Ahem) and are platform-specific - regenerate with
/// `flutter test --update-goldens test/widget/dashboard_golden_test.dart`.
void main() {
  testWidgets('dashboard matches the golden image', (tester) async {
    await tester.pumpWidget(const ProviderScope(child: SportsGurukulApp()));
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

import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'package:sports_gurukul/app/bootstrap/splash_page.dart';
import 'package:sports_gurukul/main.dart' as app;

/// Smoke test: launches the real application and verifies the bootstrap
/// leaves the splash and hands off to a post-auth screen.
///
/// Run on a connected device/emulator:
///   `flutter test integration_test -d <device-id>`
void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets('app bootstraps past the splash screen', (tester) async {
    await app.main();
    await tester.pumpAndSettle(
      const Duration(milliseconds: 2000),
    );

    expect(find.byType(SplashPage), findsNothing);
  });
}

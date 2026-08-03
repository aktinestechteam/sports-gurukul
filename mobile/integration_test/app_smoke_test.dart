import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'package:sports_gurukul/main.dart' as app;

/// Smoke test: launches the real application and verifies the bootstrap
/// reaches the placeholder dashboard.
///
/// Run on a connected device/emulator:
///   `flutter test integration_test -d <device-id>`
void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  testWidgets('app bootstraps to the placeholder dashboard', (
    tester,
  ) async {
    await app.main();
    await tester.pumpAndSettle(
      const Duration(milliseconds: 2000),
    );

    expect(find.text('Project Initialized Successfully'), findsOneWidget);
  });
}

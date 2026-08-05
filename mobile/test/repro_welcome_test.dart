import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:sports_gurukul/app/app.dart';
import 'package:sports_gurukul/app/bootstrap/splash_page.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/onboarding/application/onboarding_providers.dart';
import 'package:sports_gurukul/features/onboarding/presentation/pages/welcome_page.dart';

import 'helpers/auth_test_helper.dart';

void main() {
  testWidgets('signed-in + AsyncData(null) renders the idle message', (
    tester,
  ) async {
    // auth = Authenticated, but currentUserProvider resolves to null:
    // the onboarding controller sees AsyncData(null) -> OnboardingIdle.
    final overrides = [
      authControllerProvider.overrideWith(
        () => FakeAuthController(AuthAuthenticated(testAuthSession())),
      ),
      currentUserProvider.overrideWith((ref) async => null),
    ];

    await tester.pumpWidget(
      ProviderScope(overrides: overrides, child: const SportsGurukulApp()),
    );
    await tester.pump();
    await tester.pump(const Duration(seconds: 2));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    // Simulate the web refresh landing directly on /welcome.
    final context = tester.element(find.byType(SplashPage));
    GoRouter.of(context).go(RoutePaths.welcome);
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.byType(WelcomePage), findsOneWidget);
    expect(find.text('Nothing to resolve yet.'), findsOneWidget);
  });
}

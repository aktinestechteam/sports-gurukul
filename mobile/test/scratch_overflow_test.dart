import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/forgot_password_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/login_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/reset_password_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/sign_up_page.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

import 'helpers/fake_asset_bundle.dart';

Widget _wrap(Widget home) => ProviderScope(
  child: MaterialApp(
    localizationsDelegates: AppLocalizations.localizationsDelegates,
    supportedLocales: AppLocalizations.supportedLocales,
    builder: (context, child) =>
        DefaultAssetBundle(bundle: FakeAssetBundle(), child: child!),
    home: home,
  ),
);

void main() {
  testWidgets('no overflow across sizes with the floating logo', (
    tester,
  ) async {
    Future<void> pumpAt(Size size, {double keyboard = 0}) async {
      tester.view.physicalSize = size;
      tester.view.devicePixelRatio = 1.0;
      tester.view.viewInsets = FakeViewPadding(bottom: keyboard);
      addTearDown(tester.view.reset);
      await tester.pumpWidget(_wrap(const LoginPage()));
      await tester.pumpAndSettle();
      expect(
        tester.takeException(),
        isNull,
        reason: 'overflow at ${size.width}x${size.height} kb=$keyboard',
      );
    }

    await pumpAt(const Size(320, 568)); // small phone portrait
    await pumpAt(const Size(360, 640), keyboard: 300); // keyboard open
    await pumpAt(const Size(844, 390)); // tablet landscape
    await pumpAt(const Size(1024, 768)); // tablet portrait
    await pumpAt(const Size(1440, 900)); // desktop
  });

  testWidgets('form fields and sign-up link remain reachable', (tester) async {
    tester.view.physicalSize = const Size(320, 568);
    tester.view.devicePixelRatio = 1.0;
    addTearDown(tester.view.reset);
    await tester.pumpWidget(_wrap(const LoginPage()));
    await tester.pumpAndSettle();
    expect(find.text('Sports Gurukul'), findsOneWidget);
    expect(find.byType(TextFormField), findsNWidgets(2));
    await tester.ensureVisible(find.text('Sign Up'));
    expect(find.text('Sign Up'), findsOneWidget);
  });

  testWidgets('auth screens share the themed card without overflow', (
    tester,
  ) async {
    tester.view.physicalSize = const Size(360, 640);
    tester.view.devicePixelRatio = 1.0;
    addTearDown(tester.view.reset);

    final screens = <String, Widget>{
      'sign-up': const SignUpPage(),
      'forgot-password': const ForgotPasswordPage(),
      'reset-password': const ResetPasswordPage(),
    };

    for (final entry in screens.entries) {
      await tester.pumpWidget(_wrap(entry.value));
      await tester.pumpAndSettle();
      expect(
        tester.takeException(),
        isNull,
        reason: '${entry.key} page overflow',
      );
      expect(find.text('Sports Gurukul'), findsOneWidget);
      expect(find.byType(TextFormField), findsWidgets);
    }

    await tester.pumpWidget(_wrap(const SignUpPage()));
    await tester.pumpAndSettle();
    expect(find.byType(TextFormField), findsNWidgets(5));
    await tester.ensureVisible(find.text('Create account'));
    expect(find.text('Create account'), findsOneWidget);
    await tester.ensureVisible(find.text('Sign In'));
    expect(find.text('Sign In'), findsOneWidget);
  });
}

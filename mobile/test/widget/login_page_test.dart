import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/login_page.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/buttons/gradient_button.dart';

Widget _wrap() => const ProviderScope(
  child: MaterialApp(
    localizationsDelegates: AppLocalizations.localizationsDelegates,
    supportedLocales: AppLocalizations.supportedLocales,
    home: LoginPage(),
  ),
);

void main() {
  testWidgets('renders the email and password form', (tester) async {
    await tester.pumpWidget(_wrap());

    expect(find.text('Welcome!'), findsOneWidget);
    expect(find.byType(TextFormField), findsNWidgets(2));
    expect(find.text('Sign in'), findsOneWidget);
  });

  testWidgets('renders the brand header and sports backdrop', (tester) async {
    await tester.pumpWidget(_wrap());

    expect(find.text('Sports Gurukul'), findsOneWidget);
    expect(
      find.text('Sign in to continue to Sports Gurukul.'),
      findsOneWidget,
    );
    expect(find.byType(GradientButton), findsOneWidget);
  });

  testWidgets('floating labels remain visible when fields are focused', (
    tester,
  ) async {
    await tester.pumpWidget(_wrap());
    await tester.pumpAndSettle();

    final emailField = find.byType(TextFormField).first;
    await tester.ensureVisible(emailField);
    await tester.tap(emailField);
    await tester.pump();

    expect(tester.takeException(), isNull);
    expect(find.text('Email'), findsOneWidget);
    expect(find.text('Password'), findsOneWidget);
    expect(tester.getSize(find.text('Email')).height, greaterThan(0));
    expect(tester.getSize(find.text('Password')).height, greaterThan(0));
  });

  testWidgets('shows required-field errors on empty submit', (tester) async {
    await tester.pumpWidget(_wrap());
    await tester.pumpAndSettle();

    await tester.ensureVisible(find.byType(GradientButton));
    await tester.tap(find.byType(GradientButton));
    await tester.pump();

    expect(find.text('This field is required.'), findsNWidgets(2));
  });

  testWidgets('rejects malformed email addresses', (tester) async {
    await tester.pumpWidget(_wrap());
    await tester.pumpAndSettle();

    await tester.enterText(find.byType(TextFormField).first, 'not-an-email');
    await tester.ensureVisible(find.byType(GradientButton));
    await tester.tap(find.byType(GradientButton));
    await tester.pump();

    expect(find.text('Enter a valid email address.'), findsOneWidget);
  });

  testWidgets('toggles password visibility', (tester) async {
    await tester.pumpWidget(_wrap());

    expect(find.byIcon(Icons.visibility_off), findsOneWidget);
    await tester.tap(find.byType(IconButton));
    await tester.pump();

    expect(find.byIcon(Icons.visibility), findsOneWidget);
  });

  testWidgets('no overflow on small phones, landscape and with keyboard', (
    tester,
  ) async {
    Future<void> pumpAt(Size size, {double keyboard = 0}) async {
      tester.view.physicalSize = size;
      tester.view.devicePixelRatio = 1.0;
      tester.view.viewInsets = FakeViewPadding(bottom: keyboard);
      addTearDown(tester.view.reset);
      await tester.pumpWidget(_wrap());
      await tester.pumpAndSettle();
      expect(
        tester.takeException(),
        isNull,
        reason: 'layout must not overflow at ${size.width}x${size.height}',
      );
    }

    await pumpAt(const Size(320, 568)); // small phone portrait
    await pumpAt(const Size(844, 390)); // tablet landscape
    await pumpAt(const Size(360, 640), keyboard: 300); // open keyboard
  });
}

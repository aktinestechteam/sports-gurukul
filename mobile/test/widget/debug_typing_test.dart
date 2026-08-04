import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/sign_up_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

import '../helpers/auth_test_helper.dart';
import '../helpers/fake_asset_bundle.dart';

Widget _wrap() => ProviderScope(
  overrides: [
    authControllerProvider.overrideWith(
      () => FakeAuthController(const AuthUnauthenticated()),
    ),
  ],
  child: MaterialApp(
    localizationsDelegates: AppLocalizations.localizationsDelegates,
    supportedLocales: AppLocalizations.supportedLocales,
    builder: (context, child) =>
        DefaultAssetBundle(bundle: FakeAssetBundle(), child: child!),
    home: const SignUpPage(),
  ),
);

void main() {
  Future<void> pumpPage(WidgetTester tester) async {
    await tester.pumpWidget(_wrap());
    await tester.pumpAndSettle();
  }

  Future<FocusNode?> tapPasswordField(WidgetTester tester) async {
    final passwordField = find.byType(TextFormField).at(3);
    final editable = find.descendant(
      of: passwordField,
      matching: find.byType(EditableText),
    );
    await tester.ensureVisible(editable);
    await tester.pumpAndSettle();
    await tester.tap(editable, warnIfMissed: true);
    await tester.pump();
    await tester.pump();
    final ets = tester.state<EditableTextState>(editable);
    debugPrint(
      'EDITABLE hasFocus=${ets.widget.focusNode!.hasFocus} '
      'canRequestFocus=${ets.widget.focusNode!.canRequestFocus}',
    );
    return FocusManager.instance.primaryFocus;
  }

  testWidgets('fresh page: tapping password focuses it', (tester) async {
    await pumpPage(tester);
    final focus = await tapPasswordField(tester);
    debugPrint('FRESH FOCUS: ${focus?.debugLabel ?? 'null'}');
    expect(focus, isNotNull);
  });

  testWidgets('after validation error: tapping password focuses it', (
    tester,
  ) async {
    await pumpPage(tester);
    await tester.ensureVisible(find.text('Create account'));
    await tester.tap(find.text('Create account'));
    await tester.pump();
    expect(find.text('This field is required.'), findsNWidgets(4));

    final focus = await tapPasswordField(tester);
    debugPrint('AFTER-ERROR FOCUS: ${focus?.debugLabel ?? 'null'}');
    expect(focus, isNotNull);
  });
}

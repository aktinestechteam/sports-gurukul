import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
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
  Future<TextEditingController> focusAndType(
    WidgetTester tester, {
    bool afterError = false,
  }) async {
    await tester.pumpWidget(_wrap());
    await tester.pumpAndSettle();
    if (afterError) {
      await tester.ensureVisible(find.text('Create account'));
      await tester.tap(find.text('Create account'));
      await tester.pump();
      expect(find.text('This field is required.'), findsWidgets);
    }
    final passwordField = find.byType(TextFormField).at(3);
    final editable = find.descendant(
      of: passwordField,
      matching: find.byType(EditableText),
    );
    await tester.ensureVisible(editable);
    await tester.pumpAndSettle();
    // Synthetic key events carry no printable character, so character input
    // goes through the IME (`enterText`) while backspace is a real key event.
    await tester.showKeyboard(editable);
    await tester.enterText(editable, 'ab');
    await tester.pump();
    await tester.sendKeyEvent(LogicalKeyboardKey.backspace);
    await tester.pump();
    final ets = tester.state<EditableTextState>(editable);
    return ets.widget.controller;
  }

  testWidgets('fresh: password field accepts input', (tester) async {
    final c = await focusAndType(tester);
    debugPrint('FRESH after input: "${c.text}"');
    expect(c.text, 'a');
  });

  testWidgets('after error: password field accepts input', (tester) async {
    final c = await focusAndType(tester, afterError: true);
    debugPrint('AFTER-ERROR after input: "${c.text}"');
    expect(c.text, 'a');
  });
}

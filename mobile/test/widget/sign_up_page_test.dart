import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/presentation/pages/sign_up_page.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

import '../helpers/auth_test_helper.dart';
import '../helpers/fake_asset_bundle.dart';

class _RecordingAuthController extends AuthController {
  final List<Map<String, Object?>> recorded = <Map<String, Object?>>[];

  @override
  AuthState build() => const AuthUnauthenticated();

  @override
  Future<Result<AuthSession>> register({
    required String fullName,
    required String email,
    required String password,
    required String confirmPassword,
    String? phoneNumber,
  }) async {
    recorded.add(<String, Object?>{
      'fullName': fullName,
      'email': email,
      'password': password,
      'confirmPassword': confirmPassword,
      'phoneNumber': phoneNumber,
    });
    return Result.success(testAuthSession(email: email));
  }
}

Widget _wrap(AuthController controller) => ProviderScope(
  overrides: [authControllerProvider.overrideWith(() => controller)],
  child: MaterialApp(
    localizationsDelegates: AppLocalizations.localizationsDelegates,
    supportedLocales: AppLocalizations.supportedLocales,
    builder: (context, child) =>
        DefaultAssetBundle(bundle: FakeAssetBundle(), child: child!),
    home: const SignUpPage(),
  ),
);

Future<void> _submit(WidgetTester tester) async {
  await tester.ensureVisible(find.text('Create account'));
  await tester.tap(find.text('Create account'));
  await tester.pump();
}

void main() {
  testWidgets('renders the five sign-up fields and the submit button', (
    tester,
  ) async {
    await tester.pumpWidget(_wrap(_RecordingAuthController()));
    await tester.pumpAndSettle();
    expect(find.byType(TextFormField), findsNWidgets(5));
    await tester.ensureVisible(find.text('Create account'));
    expect(find.text('Create account'), findsOneWidget);
    expect(find.text('Sports Gurukul'), findsOneWidget);
  });

  testWidgets('flags required fields while keeping the phone optional', (
    tester,
  ) async {
    await tester.pumpWidget(_wrap(_RecordingAuthController()));
    await tester.pumpAndSettle();
    await _submit(tester);
    expect(find.text('This field is required.'), findsNWidgets(4));
  });

  testWidgets('rejects an invalid email', (tester) async {
    await tester.pumpWidget(_wrap(_RecordingAuthController()));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextFormField).at(1), 'not-an-email');
    await _submit(tester);
    expect(find.text('Enter a valid email address.'), findsOneWidget);
  });

  testWidgets('rejects an invalid phone number', (tester) async {
    await tester.pumpWidget(_wrap(_RecordingAuthController()));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextFormField).at(2), '12345');
    await _submit(tester);
    expect(find.text('Enter a valid 10-digit mobile number.'), findsOneWidget);
  });

  testWidgets('rejects mismatched confirmation passwords', (tester) async {
    await tester.pumpWidget(_wrap(_RecordingAuthController()));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextFormField).at(3), 'Abcdef1!');
    await tester.enterText(find.byType(TextFormField).at(4), 'Different1!');
    await _submit(tester);
    expect(find.text('Passwords do not match.'), findsOneWidget);
  });

  testWidgets('submits the phone number and confirms success', (tester) async {
    final controller = _RecordingAuthController();
    await tester.pumpWidget(_wrap(controller));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextFormField).at(0), 'Test Player');
    await tester.enterText(
      find.byType(TextFormField).at(1),
      'player@example.com',
    );
    await tester.enterText(find.byType(TextFormField).at(2), '9876543210');
    await tester.enterText(find.byType(TextFormField).at(3), 'Abcdef1!');
    await tester.enterText(find.byType(TextFormField).at(4), 'Abcdef1!');
    await _submit(tester);
    await tester.pumpAndSettle();

    expect(controller.recorded, hasLength(1));
    expect(controller.recorded.single['fullName'], 'Test Player');
    expect(controller.recorded.single['email'], 'player@example.com');
    expect(controller.recorded.single['phoneNumber'], '9876543210');
    expect(
      find.text('Account created successfully. You are signed in.'),
      findsOneWidget,
    );
  });

  testWidgets('submits a null phone number when left blank', (tester) async {
    final controller = _RecordingAuthController();
    await tester.pumpWidget(_wrap(controller));
    await tester.pumpAndSettle();
    await tester.enterText(find.byType(TextFormField).at(0), 'Test Player');
    await tester.enterText(
      find.byType(TextFormField).at(1),
      'player@example.com',
    );
    await tester.enterText(find.byType(TextFormField).at(3), 'Abcdef1!');
    await tester.enterText(find.byType(TextFormField).at(4), 'Abcdef1!');
    await _submit(tester);
    await tester.pumpAndSettle();

    expect(controller.recorded, hasLength(1));
    expect(controller.recorded.single['phoneNumber'], isNull);
  });
}

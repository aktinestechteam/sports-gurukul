import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart' show Override;
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/material_theme/app_theme.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/application/create_academy_use_case_providers.dart';
import 'package:sports_gurukul/features/academy/create/application/my_academy_provider.dart';
import 'package:sports_gurukul/features/academy/create/application/usecases/update_academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_contact.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';
import 'package:sports_gurukul/features/academy/create/domain/repositories/create_academy_repository.dart';
import 'package:sports_gurukul/features/academy/create/presentation/pages/edit_academy_page.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

class _NoopRepository implements CreateAcademyRepository {
  const _NoopRepository();

  @override
  Future<Result<Academy>> createAcademy(CreateAcademyParams params) async =>
      const Result.failure(UnknownFailure());

  @override
  Future<Result<Academy>> getAcademy(String academyId) async =>
      const Result.failure(UnknownFailure());

  @override
  Future<Result<Academy?>> getMyAcademy() async =>
      const Result.failure(UnknownFailure());

  @override
  Future<Result<Academy>> updateAcademy(
    String academyId,
    CreateAcademyParams params,
  ) async => Result.success(_academy);

  @override
  Future<Result<AcademyContact>> updateAcademyContact(
    String academyId,
    CreateAcademyParams params,
  ) async => const Result.success(AcademyContact());
}

class _FakeUpdateAcademy extends UpdateAcademy {
  _FakeUpdateAcademy() : super(const _NoopRepository());

  int calls = 0;
  String? lastAcademyId;

  @override
  Future<Result<Academy>> call(
    String academyId,
    CreateAcademyParams params,
  ) async {
    calls++;
    lastAcademyId = academyId;
    return Result.success(_academy);
  }
}

class _FakeUpdateAcademyContact extends UpdateAcademyContact {
  _FakeUpdateAcademyContact() : super(const _NoopRepository());

  int calls = 0;

  @override
  Future<Result<AcademyContact>> call(
    String academyId,
    CreateAcademyParams params,
  ) async {
    calls++;
    return const Result.success(AcademyContact());
  }
}

final _academy = Academy(
  id: 'ac-1',
  academyCode: 'SG-0001',
  name: 'Warriors Cricket Academy',
  email: 'team@warriors.in',
  phone: '9876543210',
  status: 'Active',
  verificationStatus: 'Pending',
  createdAt: DateTime.utc(2026, 2, 3),
  logoUrl: 'https://cdn.example.com/logo.png',
  description: 'Grassroots cricket training.',
  website: 'https://warriors.in',
  academyType: 'MultiSport',
  sports: const <String>['Cricket', 'Football'],
  primaryContactName: 'Aarav Sharma',
  address: 'MG Road',
  country: 'India',
  state: 'Maharashtra',
  city: 'Pune',
  postalCode: '411001',
);

void main() {
  late _FakeUpdateAcademy coreUseCase;
  late _FakeUpdateAcademyContact contactUseCase;

  Future<void> pumpEditPage(WidgetTester tester) async {
    coreUseCase = _FakeUpdateAcademy();
    contactUseCase = _FakeUpdateAcademyContact();
    final router = GoRouter(
      initialLocation: RoutePaths.editAcademy,
      routes: <RouteBase>[
        GoRoute(
          path: RoutePaths.editAcademy,
          builder: (_, _) => const EditAcademyPage(),
        ),
        GoRoute(
          path: RoutePaths.academyDashboard,
          builder: (_, _) =>
              const Scaffold(body: Text('Academy Dashboard')),
        ),
      ],
    );
    await tester.pumpWidget(
      ProviderScope(
        overrides: <Override>[
          myAcademyProvider.overrideWithValue(AsyncData<Academy?>(_academy)),
          updateAcademyUseCaseProvider.overrideWithValue(coreUseCase),
          updateAcademyContactUseCaseProvider.overrideWithValue(contactUseCase),
        ],
        child: MaterialApp.router(
          routerConfig: router,
          theme: AppTheme.light,
          localizationsDelegates: AppLocalizations.localizationsDelegates,
          supportedLocales: AppLocalizations.supportedLocales,
          locale: const Locale('en'),
        ),
      ),
    );
    await tester.pumpAndSettle();
  }

  testWidgets('renders the prefilled basic step with read-only type and sports',
      (tester) async {
    await pumpEditPage(tester);

    expect(find.byType(EditAcademyPage), findsOneWidget);
    expect(find.text('Edit Academy'), findsOneWidget);
    expect(find.text('Step 1 of 5'), findsOneWidget);
    expect(find.text('Warriors Cricket Academy'), findsOneWidget);
    expect(find.text('Multi-sport'), findsOneWidget);
    expect(find.text('Cricket'), findsOneWidget);
    expect(find.text('Football'), findsOneWidget);
    expect(
      find.text('Academy type and sports cannot be changed after creation.'),
      findsOneWidget,
    );
  });

  testWidgets('Continue advances through the editable steps', (tester) async {
    await pumpEditPage(tester);

    await tester.ensureVisible(find.text('Continue'));
    await tester.tap(find.text('Continue'));
    await tester.pumpAndSettle();

    expect(find.text('Step 2 of 5'), findsOneWidget);
    expect(
      find.widgetWithText(TextFormField, 'Contact person'),
      findsOneWidget,
    );

    await tester.ensureVisible(find.text('Continue'));
    await tester.tap(find.text('Continue'));
    await tester.pumpAndSettle();

    expect(find.text('Step 3 of 5'), findsOneWidget);
    expect(
      find.widgetWithText(TextFormField, 'Country'),
      findsOneWidget,
    );
  });

  testWidgets('the review step shows prefilled values and a Save label', (
    tester,
  ) async {
    await pumpEditPage(tester);

    for (var i = 0; i < 4; i++) {
      await tester.ensureVisible(find.text('Continue'));
      await tester.tap(find.text('Continue'));
      await tester.pumpAndSettle();
    }

    expect(find.text('Step 5 of 5'), findsOneWidget);
    expect(find.text('Grassroots cricket training.'), findsOneWidget);
    expect(find.text('team@warriors.in'), findsOneWidget);
    expect(find.text('411001'), findsOneWidget);
    expect(find.text('Save Changes'), findsOneWidget);
  });

  testWidgets('saving the review step submits both updates and navigates back',
      (tester) async {
    await pumpEditPage(tester);

    for (var i = 0; i < 4; i++) {
      await tester.ensureVisible(find.text('Continue'));
      await tester.tap(find.text('Continue'));
      await tester.pumpAndSettle();
    }

    await tester.ensureVisible(find.text('Save Changes'));
    await tester.tap(find.text('Save Changes'));
    await tester.pumpAndSettle();

    expect(coreUseCase.calls, 1);
    expect(contactUseCase.calls, 1);
    expect(coreUseCase.lastAcademyId, 'ac-1');
    expect(find.text('Academy Dashboard'), findsOneWidget);
    expect(find.byType(EditAcademyPage), findsNothing);
  });
}

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart' show Override;
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/app/app.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/application/create_academy_use_case_providers.dart';
import 'package:sports_gurukul/features/academy/create/application/usecases/create_academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_contact.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';
import 'package:sports_gurukul/features/academy/create/domain/repositories/create_academy_repository.dart';
import 'package:sports_gurukul/features/academy/create/presentation/create_academy_draft.dart';
import 'package:sports_gurukul/features/academy/create/presentation/pages/create_academy_page.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/create_academy_controller.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/dashboard/presentation/pages/dashboard_page.dart';
import 'package:sports_gurukul/features/dashboard/presentation/widgets/new_user_dashboard.dart';
import 'package:sports_gurukul/features/onboarding/application/onboarding_providers.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/current_user.dart';
import 'package:sports_gurukul/features/onboarding/infrastructure/mappers/current_user_mapper.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';

import '../helpers/auth_test_helper.dart';
import '../helpers/onboarding_test_helper.dart';

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
  ) async => const Result.failure(UnknownFailure());

  @override
  Future<Result<AcademyContact>> updateAcademyContact(
    String academyId,
    CreateAcademyParams params,
  ) async => const Result.failure(UnknownFailure());
}

class _FakeCreateAcademy extends CreateAcademy {
  _FakeCreateAcademy(this._result) : super(const _NoopRepository());

  final Result<Academy> _result;

  @override
  Future<Result<Academy>> call(CreateAcademyParams params) async => _result;
}

/// A create-academy use case that first advances the current-user holder to
/// the academy-admin state (as the backend does by granting the role) and only
/// then reports success.
class _GrantingCreateAcademy extends _FakeCreateAcademy {
  _GrantingCreateAcademy(this._holder, Result<Academy> result) : super(result);

  final _MutableCurrentUser _holder;

  @override
  Future<Result<Academy>> call(CreateAcademyParams params) async {
    _holder.value = academyAdminAfterCreate();
    return super.call(params);
  }
}

/// Holds a [CurrentUser] that can change between re-resolutions, letting the
/// test simulate the backend granting a role after an academy is created.
class _MutableCurrentUser {
  _MutableCurrentUser(this.value);

  CurrentUser value;
}

/// A brand-new user resolved from a profile carrying only the default role.
CurrentUser newUserBeforeCreate() => CurrentUserMapper.fromProfile(
  UserProfile(
    id: 'profile-1',
    userId: 'user-1',
    fullName: 'Test Player',
    email: 'player@example.com',
    createdAt: _createdAt,
    roles: const <String>['Athlete'],
  ),
);

/// The same account after the backend granted the `Academy Admin` role.
CurrentUser academyAdminAfterCreate() => CurrentUserMapper.fromProfile(
  UserProfile(
    id: 'profile-1',
    userId: 'user-1',
    fullName: 'Test Player',
    email: 'player@example.com',
    createdAt: _createdAt,
    roles: const <String>['Athlete', 'Academy Admin'],
  ),
);

final _createdAt = DateTime.utc(2026, 2, 3);

final _academy = Academy(
  id: 'ac-1',
  academyCode: 'SG-0001',
  name: 'Warriors Cricket Academy',
  email: 'team@warriors.in',
  phone: '9876543210',
  status: 'Active',
  verificationStatus: 'Pending',
  createdAt: DateTime.utc(2026, 2, 3),
);

/// A controller that starts directly on the review step with a full draft.
class _ReviewReadyController extends CreateAcademyController {
  @override
  CreateAcademyState build() => const CreateAcademyState(
    step: 4,
    draft: CreateAcademyDraft(
      name: 'Warriors Cricket Academy',
      description: 'Grassroots cricket training.',
      academyType: AcademyType.singleSport,
      sports: <String>['Cricket'],
      contactPerson: 'Aarav',
      email: 'team@warriors.in',
      phone: '9876543210',
      website: 'https://warriors.in',
      country: 'India',
      state: 'Maharashtra',
      city: 'Pune',
      addressLine: 'MG Road',
      postalCode: '411001',
    ),
  );
}

void main() {
  Future<void> pumpToWizard(
    WidgetTester tester, {
    List<Override> extraOverrides = const <Override>[],
    Override? currentUserOverride,
  }) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: <Override>[
          authControllerProvider.overrideWith(
            () => FakeAuthController(
              AuthAuthenticated(testAuthSession()),
            ),
          ),
          currentUserOverride ??
              currentUserProvider.overrideWith(
                (ref) async => testNewUserCurrentUser(),
              ),
          ...extraOverrides,
        ],
        child: const SportsGurukulApp(),
      ),
    );
    await tester.pump(const Duration(seconds: 2));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    await tester.ensureVisible(find.text('Create My Academy'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Create My Academy'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));
  }

  Future<void> tearDownApp(WidgetTester tester) async {
    await tester.pumpWidget(const SizedBox.shrink());
    await tester.pump();
  }

  testWidgets('renders the first step and gates navigation on validation', (
    tester,
  ) async {
    await pumpToWizard(tester);

    expect(find.byType(CreateAcademyPage), findsOneWidget);
    expect(find.text('Step 1 of 5'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, 'Academy name'), findsOneWidget);

    await tester.ensureVisible(find.text('Continue'));
    await tester.tap(find.text('Continue'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.text('Step 1 of 5'), findsOneWidget);
    expect(find.text('This field is required.'), findsWidgets);
    expect(find.text('Select at least one sport.'), findsOneWidget);

    await tearDownApp(tester);
  });

  testWidgets('advances to the contact step when basic info is valid', (
    tester,
  ) async {
    await pumpToWizard(tester);

    await tester.enterText(
      find.widgetWithText(TextFormField, 'Academy name'),
      'Warriors',
    );
    await tester.ensureVisible(find.text('Single-sport'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Single-sport'));
    await tester.pump();
    await tester.ensureVisible(find.text('Cricket'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Cricket'));
    await tester.pump();
    await tester.ensureVisible(find.text('Continue'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Continue'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.text('Step 2 of 5'), findsOneWidget);
    expect(
      find.widgetWithText(TextFormField, 'Contact person'),
      findsOneWidget,
    );

    await tearDownApp(tester);
  });

  testWidgets('shows the collected draft on the review step', (tester) async {
    await pumpToWizard(
      tester,
      extraOverrides: <Override>[
        createAcademyControllerProvider.overrideWith(
          _ReviewReadyController.new,
        ),
      ],
    );

    expect(find.text('Step 5 of 5'), findsOneWidget);
    expect(find.text('Warriors Cricket Academy'), findsOneWidget);
    expect(find.text('Grassroots cricket training.'), findsOneWidget);
    expect(find.text('team@warriors.in'), findsOneWidget);
    expect(find.text('411001'), findsOneWidget);

    await tearDownApp(tester);
  });

  testWidgets('the home button hands the user back to the dashboard', (
    tester,
  ) async {
    await pumpToWizard(tester);

    expect(find.byIcon(Icons.home_rounded), findsOneWidget);

    await tester.ensureVisible(find.byIcon(Icons.home_rounded));
    await tester.tap(find.byIcon(Icons.home_rounded));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.byType(CreateAcademyPage), findsNothing);

    await tearDownApp(tester);
  });

  testWidgets('submitting a valid review navigates to the dashboard', (
    tester,
  ) async {
    await pumpToWizard(
      tester,
      extraOverrides: <Override>[
        createAcademyControllerProvider.overrideWith(
          _ReviewReadyController.new,
        ),
        createAcademyUseCaseProvider.overrideWithValue(
          _FakeCreateAcademy(Result.success(_academy)),
        ),
      ],
    );

    await tester.ensureVisible(find.text('Create academy'));
    await tester.tap(find.text('Create academy'));
    await tester.pump();
    await tester.pump(const Duration(milliseconds: 400));
    await tester.pump(const Duration(milliseconds: 400));

    expect(find.byType(DashboardPage), findsOneWidget);
    expect(find.byType(CreateAcademyPage), findsNothing);

    await tearDownApp(tester);
  });

  testWidgets(
    'a created academy hands the refreshed Academy Admin user to the '
    'full dashboard',
    (tester) async {
      final holder = _MutableCurrentUser(newUserBeforeCreate());
      await pumpToWizard(
        tester,
        currentUserOverride: currentUserProvider.overrideWith(
          (ref) async => holder.value,
        ),
        extraOverrides: <Override>[
          createAcademyControllerProvider.overrideWith(
            _ReviewReadyController.new,
          ),
          createAcademyUseCaseProvider.overrideWithValue(
            _GrantingCreateAcademy(holder, Result.success(_academy)),
          ),
        ],
      );

      await tester.ensureVisible(find.text('Create academy'));
      await tester.tap(find.text('Create academy'));
      await tester.pump();
      await tester.pump(const Duration(milliseconds: 400));
      await tester.pump(const Duration(milliseconds: 400));
      await tester.pump(const Duration(milliseconds: 400));

      expect(find.byType(DashboardPage), findsOneWidget);
      expect(find.byType(NewUserDashboard), findsNothing);
      expect(find.text('Academy Admin'), findsOneWidget);

      await tearDownApp(tester);
    },
  );
}

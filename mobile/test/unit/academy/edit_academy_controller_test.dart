import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/application/create_academy_use_case_providers.dart';
import 'package:sports_gurukul/features/academy/create/application/my_academy_provider.dart';
import 'package:sports_gurukul/features/academy/create/application/usecases/update_academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_contact.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';
import 'package:sports_gurukul/features/academy/create/domain/repositories/create_academy_repository.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/edit_academy_controller.dart';

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

class _FakeUpdateAcademy extends UpdateAcademy {
  _FakeUpdateAcademy(this._result) : super(const _NoopRepository());

  final Result<Academy> _result;
  int calls = 0;
  String? lastAcademyId;
  CreateAcademyParams? lastParams;

  @override
  Future<Result<Academy>> call(
    String academyId,
    CreateAcademyParams params,
  ) async {
    calls++;
    lastAcademyId = academyId;
    lastParams = params;
    return _result;
  }
}

class _FakeUpdateAcademyContact extends UpdateAcademyContact {
  _FakeUpdateAcademyContact(this._result) : super(const _NoopRepository());

  final Result<AcademyContact> _result;
  int calls = 0;
  String? lastAcademyId;
  CreateAcademyParams? lastParams;

  @override
  Future<Result<AcademyContact>> call(
    String academyId,
    CreateAcademyParams params,
  ) async {
    calls++;
    lastAcademyId = academyId;
    lastParams = params;
    return _result;
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
  late ProviderContainer container;
  late _FakeUpdateAcademy coreUseCase;
  late _FakeUpdateAcademyContact contactUseCase;

  tearDown(() => container.dispose());

  ProviderContainer buildContainer({
    required Result<Academy> core,
    required Result<AcademyContact> contact,
  }) {
    coreUseCase = _FakeUpdateAcademy(core);
    contactUseCase = _FakeUpdateAcademyContact(contact);
    return container = ProviderContainer(
      overrides: [
        myAcademyProvider.overrideWithValue(AsyncData<Academy?>(_academy)),
        updateAcademyUseCaseProvider.overrideWithValue(coreUseCase),
        updateAcademyContactUseCaseProvider.overrideWithValue(contactUseCase),
      ],
    );
  }

  group('EditAcademyController', () {
    test('prefills the draft from the current academy', () {
      buildContainer(
        core: Result.success(_academy),
        contact: const Result.success(AcademyContact()),
      );

      final state = container.read(editAcademyControllerProvider);
      expect(state.step, 0);
      expect(state.status, EditAcademyStatus.idle);

      final draft = state.draft;
      expect(draft.name, 'Warriors Cricket Academy');
      expect(draft.description, 'Grassroots cricket training.');
      expect(draft.academyType, AcademyType.multiSport);
      expect(draft.sports, <String>['Cricket', 'Football']);
      expect(draft.contactPerson, 'Aarav Sharma');
      expect(draft.email, 'team@warriors.in');
      expect(draft.phone, '9876543210');
      expect(draft.website, 'https://warriors.in');
      expect(draft.country, 'India');
      expect(draft.state, 'Maharashtra');
      expect(draft.city, 'Pune');
      expect(draft.addressLine, 'MG Road');
      expect(draft.postalCode, '411001');
    });

    test('next, back and jumpTo navigate within bounds', () {
      buildContainer(
        core: Result.success(_academy),
        contact: const Result.success(AcademyContact()),
      );

      container.read(editAcademyControllerProvider.notifier).next();
      expect(container.read(editAcademyControllerProvider).step, 1);

      container.read(editAcademyControllerProvider.notifier).back();
      expect(container.read(editAcademyControllerProvider).step, 0);

      container
          .read(editAcademyControllerProvider.notifier)
          .jumpTo(EditAcademyState.stepCount - 1);
      expect(
        container.read(editAcademyControllerProvider).isLastStep,
        isTrue,
      );

      container
          .read(editAcademyControllerProvider.notifier)
          .jumpTo(EditAcademyState.stepCount);
      expect(container.read(editAcademyControllerProvider).step, 4);
    });

    test('update methods merge values into the prefilled draft', () {
      buildContainer(
        core: Result.success(_academy),
        contact: const Result.success(AcademyContact()),
      );

      container
          .read(editAcademyControllerProvider.notifier)
        ..updateBasic(name: 'Warriors Renamed')
        ..updateContact(website: 'https://warriors.in/new')
        ..updateAddress(city: 'Mumbai');

      final draft = container.read(editAcademyControllerProvider).draft;
      expect(draft.name, 'Warriors Renamed');
      expect(draft.website, 'https://warriors.in/new');
      expect(draft.city, 'Mumbai');
      expect(draft.sports, <String>['Cricket', 'Football']);
    });

    test('submit sends core and contact updates and resolves to success',
        () async {
      buildContainer(
        core: Result.success(_academy),
        contact: const Result.success(AcademyContact()),
      );

      container
          .read(editAcademyControllerProvider.notifier)
        ..updateBasic(name: '  Warriors Renamed  ', description: '   ')
        ..updateContact(
          email: '  team@warriors.in  ',
          phone: ' 9876543210 ',
          website: '  ',
        )
        ..updateAddress(
          country: 'India',
          stateName: 'Maharashtra',
          city: 'Pune',
          addressLine: 'MG Road',
          postalCode: '411001',
        );

      await container.read(editAcademyControllerProvider.notifier).submit();

      final state = container.read(editAcademyControllerProvider);
      expect(state.status, EditAcademyStatus.success);

      expect(coreUseCase.calls, 1);
      expect(contactUseCase.calls, 1);
      expect(coreUseCase.lastAcademyId, 'ac-1');
      expect(contactUseCase.lastAcademyId, 'ac-1');

      final params = coreUseCase.lastParams;
      expect(params?.name, 'Warriors Renamed');
      expect(params?.description, isNull);
      expect(params?.website, isNull);
      expect(params?.email, 'team@warriors.in');
      expect(params?.phone, '9876543210');
      expect(params?.primaryContactName, 'Aarav Sharma');
      expect(params?.address, 'MG Road');
      expect(params?.country, 'India');
      expect(params?.state, 'Maharashtra');
      expect(params?.city, 'Pune');
      expect(params?.postalCode, '411001');
      expect(params?.academyType, AcademyType.multiSport);
      expect(params?.sports, <String>['Cricket', 'Football']);
    });

    test('a failed core update surfaces the failure and skips the contact',
        () async {
      buildContainer(
        core: const Result.failure(UnknownFailure(message: 'boom')),
        contact: const Result.success(AcademyContact()),
      );

      await container.read(editAcademyControllerProvider.notifier).submit();

      final state = container.read(editAcademyControllerProvider);
      expect(state.status, EditAcademyStatus.idle);
      expect(state.failure?.message, 'boom');
      expect(contactUseCase.calls, 0);
    });

    test('a failed contact update surfaces the failure', () async {
      buildContainer(
        core: Result.success(_academy),
        contact: const Result.failure(UnknownFailure(message: 'nope')),
      );

      await container.read(editAcademyControllerProvider.notifier).submit();

      final state = container.read(editAcademyControllerProvider);
      expect(state.status, EditAcademyStatus.idle);
      expect(state.failure?.message, 'nope');
    });
  });
}

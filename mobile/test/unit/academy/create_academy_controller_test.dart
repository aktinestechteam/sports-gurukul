import 'dart:typed_data';

import 'package:cross_file/cross_file.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/application/create_academy_use_case_providers.dart';
import 'package:sports_gurukul/features/academy/create/application/usecases/create_academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_contact.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';
import 'package:sports_gurukul/features/academy/create/domain/repositories/create_academy_repository.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/create_academy_controller.dart';

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
  CreateAcademyParams? lastParams;

  @override
  Future<Result<Academy>> call(CreateAcademyParams params) async {
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
);

void main() {
  late ProviderContainer container;
  late _FakeCreateAcademy useCase;

  tearDown(() => container.dispose());

  ProviderContainer buildContainer(Result<Academy> result) {
    useCase = _FakeCreateAcademy(result);
    return container = ProviderContainer(
      overrides: [
        createAcademyUseCaseProvider.overrideWithValue(useCase),
      ],
    );
  }

  group('CreateAcademyController', () {
    test('starts idle on the first step', () {
      buildContainer(Result.success(_academy));

      final state = container.read(createAcademyControllerProvider);
      expect(state.step, 0);
      expect(state.status, CreateAcademyStatus.idle);
      expect(state.isLastStep, isFalse);
    });

    test('next, back and jumpTo navigate within bounds', () {
      buildContainer(Result.success(_academy));

      container.read(createAcademyControllerProvider.notifier).next();
      expect(container.read(createAcademyControllerProvider).step, 1);

      container.read(createAcademyControllerProvider.notifier).back();
      expect(container.read(createAcademyControllerProvider).step, 0);

      container.read(createAcademyControllerProvider.notifier).back();
      expect(container.read(createAcademyControllerProvider).step, 0);

      container
          .read(createAcademyControllerProvider.notifier)
          .jumpTo(CreateAcademyState.stepCount - 1);
      expect(
        container.read(createAcademyControllerProvider).isLastStep,
        isTrue,
      );

      container
          .read(createAcademyControllerProvider.notifier)
          .jumpTo(CreateAcademyState.stepCount);
      expect(container.read(createAcademyControllerProvider).step, 4);

      container.read(createAcademyControllerProvider.notifier).next();
      expect(container.read(createAcademyControllerProvider).step, 4);
    });

    test('update methods merge values into the draft', () {
      buildContainer(Result.success(_academy));

      container
          .read(createAcademyControllerProvider.notifier)
        ..updateBasic(
          name: 'Warriors',
          academyType: AcademyType.singleSport,
          sports: const <String>['Cricket'],
        )
        ..updateContact(
          contactPerson: 'Aarav',
          email: 'team@warriors.in',
          phone: '9876543210',
          website: 'https://warriors.in',
        )
        ..updateAddress(
          country: 'India',
          stateName: 'Maharashtra',
          city: 'Pune',
          addressLine: 'MG Road',
          postalCode: '411001',
        );

      final draft = container.read(createAcademyControllerProvider).draft;
      expect(draft.name, 'Warriors');
      expect(draft.academyType, AcademyType.singleSport);
      expect(draft.sports, <String>['Cricket']);
      expect(draft.contactPerson, 'Aarav');
      expect(draft.website, 'https://warriors.in');
      expect(draft.country, 'India');
      expect(draft.state, 'Maharashtra');
      expect(draft.postalCode, '411001');
    });

    test('updateBasic replaces the academy type and sports', () {
      buildContainer(Result.success(_academy));

      container
          .read(createAcademyControllerProvider.notifier)
        ..updateBasic(academyType: AcademyType.multiSport)
        ..updateBasic(
          academyType: AcademyType.singleSport,
          sports: const <String>['Tennis'],
        );

      final draft = container.read(createAcademyControllerProvider).draft;
      expect(draft.academyType, AcademyType.singleSport);
      expect(draft.sports, <String>['Tennis']);
    });

    test('setLogo and setCover update the branding files', () async {
      buildContainer(Result.success(_academy));

      final logo = XFile.fromData(
        Uint8List.fromList(<int>[1]),
        path: 'logo.png',
      );
      final cover = XFile.fromData(
        Uint8List.fromList(<int>[2]),
        path: 'cover.png',
      );

      container
          .read(createAcademyControllerProvider.notifier)
        ..setLogo(logo)
        ..setCover(cover);

      final draft = container.read(createAcademyControllerProvider).draft;
      expect(draft.logo, same(logo));
      expect(draft.cover, same(cover));
    });

    test('submit sends the full draft and resolves to success', () async {
      buildContainer(Result.success(_academy));

      final logo = XFile.fromData(
        Uint8List.fromList(<int>[1]),
        path: 'submit-logo.png',
      );
      final cover = XFile.fromData(
        Uint8List.fromList(<int>[2]),
        path: 'submit-cover.png',
      );

      container
          .read(createAcademyControllerProvider.notifier)
        ..updateBasic(
          name: '  Warriors  ',
          description: '   ',
          academyType: AcademyType.singleSport,
          sports: const <String>['Cricket', 'Football'],
        )
        ..updateContact(
          contactPerson: 'Aarav',
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
        )
        ..setLogo(logo)
        ..setCover(cover);

      await container.read(createAcademyControllerProvider.notifier).submit();

      final state = container.read(createAcademyControllerProvider);
      expect(state.status, CreateAcademyStatus.success);
      expect(state.academy?.id, 'ac-1');

      final params = useCase.lastParams;
      expect(params?.name, 'Warriors');
      expect(params?.description, isNull);
      expect(params?.website, isNull);
      expect(params?.email, 'team@warriors.in');
      expect(params?.phone, '9876543210');
      expect(params?.academyType, AcademyType.singleSport);
      expect(params?.sports, <String>['Cricket', 'Football']);
      expect(params?.primaryContactName, 'Aarav');
      expect(params?.address, 'MG Road');
      expect(params?.country, 'India');
      expect(params?.state, 'Maharashtra');
      expect(params?.city, 'Pune');
      expect(params?.postalCode, '411001');
      expect(params?.logo, same(logo));
      expect(params?.cover, same(cover));
    });

    test('submit surfaces failures as data', () async {
      buildContainer(
        const Result.failure(UnknownFailure(message: 'boom')),
      );
      final controller = container.read(
        createAcademyControllerProvider.notifier,
      );

      await controller.submit();

      final state = container.read(createAcademyControllerProvider);
      expect(state.status, CreateAcademyStatus.idle);
      expect(state.failure?.message, 'boom');
    });

    test('submit is a no-op while already submitting', () async {
      buildContainer(Result.success(_academy));
      final controller = container.read(
        createAcademyControllerProvider.notifier,
      );

      await controller.submit();
      await controller.submit();

      expect(useCase.lastParams, isNotNull);
      expect(container.read(createAcademyControllerProvider).status,
          CreateAcademyStatus.success);
    });
  });
}

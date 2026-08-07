import 'dart:typed_data';

import 'package:cross_file/cross_file.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_contact.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/datasources/create_academy_remote_datasource.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/error/create_academy_error_mapper.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_contact_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/create_academy_request_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/update_academy_request_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/update_contact_request_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/repositories/create_academy_repository_impl.dart';

class FakeCreateAcademyRemoteDataSource
    implements CreateAcademyRemoteDataSource {
  final Map<Type, Object> _responses = <Type, Object>{};
  final List<Object> _capturedRequests = <Object>[];
  int _calls = 0;
  int _uploadCalls = 0;
  ApiException? _uploadError;

  void respondWith<T extends Object>(Object value) => _responses[T] = value;

  void failWith<T>(ApiException error) => _responses[T] = error;

  // Deliberate: test helpers mirror the datasource method vocabulary.
  // ignore: use_setters_to_change_properties
  void failUploadWith(ApiException error) => _uploadError = error;

  List<Object> get capturedRequests =>
      List<Object>.unmodifiable(_capturedRequests);

  int get calls => _calls;

  int get uploadCalls => _uploadCalls;

  @override
  Future<AcademyDto> createAcademy(CreateAcademyRequestDto request) async {
    _calls++;
    _capturedRequests.add(request);
    return _next<AcademyDto>();
  }

  @override
  Future<AcademyDto> getAcademy(String academyId) async {
    _calls++;
    _capturedRequests.add(academyId);
    return _next<AcademyDto>();
  }

  @override
  Future<AcademyDto?> getMyAcademy() async {
    _calls++;
    return _nextOrNull<AcademyDto>();
  }

  @override
  Future<AcademyDto> updateAcademy(
    String academyId,
    UpdateAcademyRequestDto request,
  ) async {
    _calls++;
    _capturedRequests.add((academyId: academyId, request: request));
    return _next<AcademyDto>();
  }

  @override
  Future<AcademyContactDto> updateContact(
    String academyId,
    UpdateContactRequestDto request,
  ) async {
    _calls++;
    _capturedRequests.add((academyId: academyId, request: request));
    return _next<AcademyContactDto>();
  }

  @override
  Future<AcademyDto> uploadLogo({
    required String academyId,
    required String fileName,
    required String contentType,
    required List<int> fileBytes,
  }) async {
    _uploadCalls++;
    _capturedRequests.add((academyId: academyId, fileName: fileName));
    return _nextUpload<AcademyDto>();
  }

  @override
  Future<AcademyDto> uploadBanner({
    required String academyId,
    required String fileName,
    required String contentType,
    required List<int> fileBytes,
  }) async {
    _uploadCalls++;
    _capturedRequests.add((academyId: academyId, fileName: fileName));
    return _nextUpload<AcademyDto>();
  }

  T _next<T>() {
    final value = _responses[T];
    if (value is ApiException) {
      throw value;
    }
    return value as T;
  }

  T? _nextOrNull<T>() {
    final value = _responses[T];
    if (value is ApiException) {
      throw value;
    }
    if (value == null) {
      return null;
    }
    return value as T;
  }

  T _nextUpload<T>() {
    final error = _uploadError;
    if (error != null) {
      throw error;
    }
    return _next<T>();
  }
}

void main() {
  late FakeCreateAcademyRemoteDataSource remote;
  late CreateAcademyRepositoryImpl repository;

  const academyDto = AcademyDto(
    id: 'ac-1',
    academyCode: 'SG-0001',
    name: 'Warriors Cricket Academy',
    email: 'team@warriors.in',
    phone: '9876543210',
    status: 'Active',
    verificationStatus: 'Pending',
    createdAt: '2026-02-03T10:30:00.0000000Z',
  );

  setUp(() {
    remote = FakeCreateAcademyRemoteDataSource();
    repository = CreateAcademyRepositoryImpl(remote: remote);
  });

  group('CreateAcademyRepositoryImpl', () {
    test('createAcademy maps the response and sends a UTC date-only request',
        () async {
      remote.respondWith<AcademyDto>(academyDto);

      final result = await repository.createAcademy(
        CreateAcademyParams(
          name: 'Warriors Cricket Academy',
          legalName: 'Warriors Sports LLP',
          email: 'team@warriors.in',
          phone: '9876543210',
          description: 'Grassroots cricket training.',
          website: 'https://warriors.in',
          establishedDate: DateTime(2018, 4, 15, 23, 30),
        ),
      );

      expect(result, isA<Success<Academy>>());
      expect(result.requireValue().name, 'Warriors Cricket Academy');
      expect(remote.calls, 1);

      final request = remote.capturedRequests.single as CreateAcademyRequestDto;
      expect(request.name, 'Warriors Cricket Academy');
      expect(request.legalName, 'Warriors Sports LLP');
      expect(request.description, 'Grassroots cricket training.');
      expect(request.website, 'https://warriors.in');
      expect(request.establishedDate, '2018-04-15');
      expect(request.gstNumber, isNull);
    });

    test('createAcademy sends type, sports, contact and address fields',
        () async {
      remote.respondWith<AcademyDto>(academyDto);

      final result = await repository.createAcademy(
        const CreateAcademyParams(
          name: 'Warriors Cricket Academy',
          email: 'team@warriors.in',
          phone: '9876543210',
          academyType: AcademyType.singleSport,
          sports: <String>['Cricket', 'Football'],
          primaryContactName: 'Aarav Sharma',
          address: 'MG Road',
          country: 'India',
          state: 'Maharashtra',
          city: 'Pune',
          postalCode: '411001',
        ),
      );

      expect(result, isA<Success<Academy>>());

      final request = remote.capturedRequests.single as CreateAcademyRequestDto;
      expect(request.academyType, 'SingleSport');
      expect(request.sportNames, <String>['Cricket', 'Football']);
      expect(request.primaryContactName, 'Aarav Sharma');
      expect(request.address, 'MG Road');
      expect(request.country, 'India');
      expect(request.state, 'Maharashtra');
      expect(request.city, 'Pune');
      expect(request.postalCode, '411001');
    });

    test('createAcademy maps a multi-sport type to its wire value', () async {
      remote.respondWith<AcademyDto>(academyDto);

      await repository.createAcademy(
        const CreateAcademyParams(
          name: 'Warriors',
          email: 'team@warriors.in',
          phone: '9876543210',
          academyType: AcademyType.multiSport,
        ),
      );

      final request = remote.capturedRequests.single as CreateAcademyRequestDto;
      expect(request.academyType, 'MultiSport');
    });

    test('createAcademy uploads logo and cover and returns the updated academy',
        () async {
      final updatedDto = academyDto.copyWith(
        logoUrl: 'https://cdn.example.com/logo.png',
        bannerUrl: 'https://cdn.example.com/banner.png',
      );
      remote.respondWith<AcademyDto>(updatedDto);

      final logo = XFile.fromData(
        Uint8List.fromList(<int>[1, 2, 3]),
        path: 'sg-logo.png',
      );
      final cover = XFile.fromData(
        Uint8List.fromList(<int>[4, 5, 6]),
        path: 'sg-cover.png',
      );

      final result = await repository.createAcademy(
        CreateAcademyParams(
          name: 'Warriors',
          email: 'team@warriors.in',
          phone: '9876543210',
          logo: logo,
          cover: cover,
        ),
      );

      expect(result, isA<Success<Academy>>());
      expect(remote.calls, 1);
      expect(remote.uploadCalls, 2);
      expect(result.requireValue().logoUrl, 'https://cdn.example.com/logo.png');
      expect(
        result.requireValue().bannerUrl,
        'https://cdn.example.com/banner.png',
      );

      final uploadArgs = remote.capturedRequests.skip(1).toList();
      expect(uploadArgs[0], (academyId: 'ac-1', fileName: 'sg-logo.png'));
      expect(uploadArgs[1], (academyId: 'ac-1', fileName: 'sg-cover.png'));
    });

    test('createAcademy ignores a failed branding upload', () async {
      remote
        ..respondWith<AcademyDto>(academyDto)
        ..failUploadWith(
          const ApiException(
            statusCode: 413,
            code: CreateAcademyOperations.uploadLogo,
            message: 'too large',
          ),
        );

      final logo = XFile.fromData(
        Uint8List.fromList(<int>[1]),
        path: 'sg-logo.png',
      );

      final result = await repository.createAcademy(
        CreateAcademyParams(
          name: 'Warriors',
          email: 'team@warriors.in',
          phone: '9876543210',
          logo: logo,
        ),
      );

      expect(result, isA<Success<Academy>>());
      expect(result.requireValue().logoUrl, isNull);
      expect(remote.uploadCalls, 1);
    });

    test('createAcademy maps transport errors to failures', () async {
      remote.failWith<AcademyDto>(
        const ApiException(
          statusCode: 503,
          code: CreateAcademyOperations.createAcademy,
          message: 'down',
        ),
      );

      final result = await repository.createAcademy(
        const CreateAcademyParams(
          name: 'Warriors',
          email: 'team@warriors.in',
          phone: '9876543210',
        ),
      );

      expect(result, isA<FailureResult<Academy>>());
      final failure = result.failureOrNull;
      expect(failure, isA<ServerFailure>());
      expect(failure?.code, CreateAcademyErrorCodes.server);
    });

    test('createAcademy maps unexpected errors to unknown failures', () async {
      remote.respondWith<AcademyDto>(StateError('boom'));

      final result = await repository.createAcademy(
        const CreateAcademyParams(
          name: 'Warriors',
          email: 'team@warriors.in',
          phone: '9876543210',
        ),
      );

      expect(result, isA<FailureResult<Academy>>());
      expect(result.failureOrNull, isA<UnknownFailure>());
    });

    test('updateAcademy sends only populated core fields', () async {
      remote.respondWith<AcademyDto>(academyDto);

      final result = await repository.updateAcademy(
        'ac-1',
        CreateAcademyParams(
          name: 'Warriors Updated',
          legalName: 'Warriors Sports LLP',
          description: 'Updated blurb.',
          registrationNumber: 'REG-42',
          gstNumber: 'GST-7',
          website: 'https://warriors.in',
          email: 'team@warriors.in',
          phone: '9876543210',
          establishedDate: DateTime(2018, 4, 15, 23, 30),
        ),
      );

      expect(result, isA<Success<Academy>>());
      expect(result.requireValue().name, 'Warriors Cricket Academy');
      expect(remote.calls, 1);

      final updateCall = remote.capturedRequests.single as ({
        String academyId,
        UpdateAcademyRequestDto request,
      });
      expect(updateCall.academyId, 'ac-1');
      final request = updateCall.request;
      expect(request.name, 'Warriors Updated');
      expect(request.legalName, 'Warriors Sports LLP');
      expect(request.description, 'Updated blurb.');
      expect(request.registrationNumber, 'REG-42');
      expect(request.gstNumber, 'GST-7');
      expect(request.establishedDate, '2018-04-15');
      expect(request.email, 'team@warriors.in');
      expect(request.phone, '9876543210');
    });

    test('updateAcademy leaves unset core fields as null', () async {
      remote.respondWith<AcademyDto>(academyDto);

      final result = await repository.updateAcademy(
        'ac-1',
        const CreateAcademyParams(
          name: 'Warriors Updated',
          email: 'team@warriors.in',
          phone: '9876543210',
        ),
      );

      expect(result, isA<Success<Academy>>());

      final updateCall = remote.capturedRequests.single as ({
        String academyId,
        UpdateAcademyRequestDto request,
      });
      final request = updateCall.request;
      expect(request.legalName, isNull);
      expect(request.description, isNull);
      expect(request.website, isNull);
      expect(request.establishedDate, isNull);
    });

    test('updateAcademy uploads a new logo and returns the updated academy',
        () async {
      final updatedDto = academyDto.copyWith(
        logoUrl: 'https://cdn.example.com/logo.png',
      );
      remote.respondWith<AcademyDto>(updatedDto);

      final logo = XFile.fromData(
        Uint8List.fromList(<int>[1, 2, 3]),
        path: 'sg-logo.png',
      );

      final result = await repository.updateAcademy(
        'ac-1',
        CreateAcademyParams(
          name: 'Warriors Updated',
          email: 'team@warriors.in',
          phone: '9876543210',
          logo: logo,
        ),
      );

      expect(result, isA<Success<Academy>>());
      expect(result.requireValue().logoUrl, 'https://cdn.example.com/logo.png');
      expect(remote.uploadCalls, 1);

      final uploadArgs = remote.capturedRequests.skip(1).toList();
      expect(uploadArgs[0], (academyId: 'ac-1', fileName: 'sg-logo.png'));
    });

    test('updateAcademy maps transport errors to failures', () async {
      remote.failWith<AcademyDto>(
        const ApiException(
          statusCode: 503,
          code: CreateAcademyOperations.updateAcademy,
          message: 'down',
        ),
      );

      final result = await repository.updateAcademy(
        'ac-1',
        const CreateAcademyParams(
          name: 'Warriors',
          email: 'team@warriors.in',
          phone: '9876543210',
        ),
      );

      expect(result, isA<FailureResult<Academy>>());
      final failure = result.failureOrNull;
      expect(failure, isA<ServerFailure>());
      expect(failure?.code, CreateAcademyErrorCodes.server);
    });

    test('updateAcademyContact sends contact and address fields', () async {
      const contactDto = AcademyContactDto(
        primaryContactName: 'Aarav Sharma',
        address: 'MG Road',
        country: 'India',
        state: 'Maharashtra',
        city: 'Pune',
        postalCode: '411001',
      );
      remote.respondWith<AcademyContactDto>(contactDto);

      final result = await repository.updateAcademyContact(
        'ac-1',
        const CreateAcademyParams(
          name: 'Warriors',
          email: 'team@warriors.in',
          phone: '9876543210',
          primaryContactName: 'Aarav Sharma',
          address: 'MG Road',
          country: 'India',
          state: 'Maharashtra',
          city: 'Pune',
          postalCode: '411001',
        ),
      );

      expect(result, isA<Success<AcademyContact>>());
      expect(result.requireValue().primaryContactName, 'Aarav Sharma');
      expect(result.requireValue().city, 'Pune');
      expect(remote.calls, 1);

      final updateCall = remote.capturedRequests.single as ({
        String academyId,
        UpdateContactRequestDto request,
      });
      expect(updateCall.academyId, 'ac-1');
      final request = updateCall.request;
      expect(request.primaryContactName, 'Aarav Sharma');
      expect(request.primaryPhone, '9876543210');
      expect(request.primaryEmail, 'team@warriors.in');
      expect(request.address, 'MG Road');
      expect(request.country, 'India');
      expect(request.state, 'Maharashtra');
      expect(request.city, 'Pune');
      expect(request.postalCode, '411001');
    });

    test('updateAcademyContact maps transport errors to failures', () async {
      remote.failWith<AcademyContactDto>(
        const ApiException(
          statusCode: 503,
          code: CreateAcademyOperations.updateContact,
          message: 'down',
        ),
      );

      final result = await repository.updateAcademyContact(
        'ac-1',
        const CreateAcademyParams(
          name: 'Warriors',
          email: 'team@warriors.in',
          phone: '9876543210',
        ),
      );

      expect(result, isA<FailureResult<AcademyContact>>());
      expect(result.failureOrNull, isA<ServerFailure>());
    });

    test('getAcademy forwards the id and maps the response', () async {
      remote.respondWith<AcademyDto>(academyDto);

      final result = await repository.getAcademy('ac-1');

      expect(result, isA<Success<Academy>>());
      expect(result.requireValue().id, 'ac-1');
      expect(remote.capturedRequests.single, 'ac-1');
    });

    test('getAcademy maps failures', () async {
      remote.failWith<AcademyDto>(
        const ApiException(
          statusCode: 404,
          code: CreateAcademyOperations.getAcademy,
        ),
      );

      final result = await repository.getAcademy('ac-1');

      expect(result, isA<FailureResult<Academy>>());
      expect(result.failureOrNull, isA<UnknownFailure>());
    });

    test('getMyAcademy maps the owned academy response', () async {
      remote.respondWith<AcademyDto>(academyDto);

      final result = await repository.getMyAcademy();

      expect(result, isA<Success<Academy?>>());
      expect(result.requireValue()?.name, 'Warriors Cricket Academy');
      expect(remote.calls, 1);
    });

    test('getMyAcademy succeeds with null when the user owns no academy',
        () async {
      final result = await repository.getMyAcademy();

      expect(result, isA<Success<Academy?>>());
      expect(result.requireValue(), isNull);
    });

    test('getMyAcademy maps failures', () async {
      remote.failWith<AcademyDto>(
        const ApiException(
          statusCode: 503,
          code: CreateAcademyOperations.getMyAcademy,
          message: 'down',
        ),
      );

      final result = await repository.getMyAcademy();

      expect(result, isA<FailureResult<Academy?>>());
      expect(result.failureOrNull, isA<ServerFailure>());
    });
  });
}

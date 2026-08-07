import 'package:cross_file/cross_file.dart';
import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_contact.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';
import 'package:sports_gurukul/features/academy/create/domain/repositories/create_academy_repository.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/datasources/create_academy_remote_datasource.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/error/create_academy_error_mapper.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/mappers/academy_mappers.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/create_academy_request_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/update_academy_request_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/update_contact_request_dto.dart';

/// [CreateAcademyRepository] implementation backed by the remote datasource.
///
/// Transport and server failures are mapped to typed [BaseFailure]s at this
/// boundary; features only ever see `Result` values.
class CreateAcademyRepositoryImpl implements CreateAcademyRepository {
  CreateAcademyRepositoryImpl({
    required CreateAcademyRemoteDataSource remote,
  }) : _remote = remote;

  final CreateAcademyRemoteDataSource _remote;

  @override
  Future<Result<Academy>> createAcademy(CreateAcademyParams params) async {
    try {
      final request = CreateAcademyRequestDto(
        name: params.name,
        legalName: params.legalName,
        description: params.description,
        registrationNumber: params.registrationNumber,
        gstNumber: params.gstNumber,
        establishedDate: params.establishedDate
            ?.toUtc()
            .toIso8601String()
            .split('T')
            .first,
        website: params.website,
        email: params.email,
        phone: params.phone,
        academyType: params.academyType?.wireValue,
        sportNames: params.sports,
        primaryContactName: params.primaryContactName,
        address: params.address,
        country: params.country,
        state: params.state,
        city: params.city,
        postalCode: params.postalCode,
      );
      final dto = await _remote.createAcademy(request);
      var academy = AcademyMappers.toAcademy(dto);

      if (params.logo != null) {
        academy = await _uploadBranding(
          academy,
          () async => _remote.uploadLogo(
            academyId: dto.id,
            fileName: params.logo!.name,
            contentType: _contentTypeFor(params.logo!),
            fileBytes: await params.logo!.readAsBytes(),
          ),
        );
      }

      if (params.cover != null) {
        academy = await _uploadBranding(
          academy,
          () async => _remote.uploadBanner(
            academyId: dto.id,
            fileName: params.cover!.name,
            contentType: _contentTypeFor(params.cover!),
            fileBytes: await params.cover!.readAsBytes(),
          ),
        );
      }

      return Result.success(academy);
    } on ApiException catch (error) {
      return Result.failure(CreateAcademyErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  /// Uploads branding images after the academy exists. A failed branding
  /// upload does not fail the whole creation — the academy record was already
  /// persisted — so errors are swallowed and the created academy is returned.
  Future<Academy> _uploadBranding(
    Academy academy,
    Future<AcademyDto> Function() upload,
  ) async {
    try {
      final updated = await upload();
      return AcademyMappers.toAcademy(updated);
    } on ApiException {
      return academy;
    }
  }

  static String _contentTypeFor(XFile file) {
    final name = file.name.toLowerCase();
    if (name.endsWith('.png')) return 'image/png';
    if (name.endsWith('.webp')) return 'image/webp';
    return 'image/jpeg';
  }

  @override
  Future<Result<Academy>> getAcademy(String academyId) async {
    try {
      final dto = await _remote.getAcademy(academyId);
      return Result.success(AcademyMappers.toAcademy(dto));
    } on ApiException catch (error) {
      return Result.failure(CreateAcademyErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<Result<Academy?>> getMyAcademy() async {
    try {
      final dto = await _remote.getMyAcademy();
      return Result.success(
        dto == null ? null : AcademyMappers.toAcademy(dto),
      );
    } on ApiException catch (error) {
      return Result.failure(CreateAcademyErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<Result<Academy>> updateAcademy(
    String academyId,
    CreateAcademyParams params,
  ) async {
    try {
      final request = UpdateAcademyRequestDto(
        name: params.name,
        legalName: params.legalName,
        description: params.description,
        registrationNumber: params.registrationNumber,
        gstNumber: params.gstNumber,
        establishedDate: _dateOnly(params.establishedDate),
        website: params.website,
        email: params.email,
        phone: params.phone,
      );
      final dto = await _remote.updateAcademy(academyId, request);
      var academy = AcademyMappers.toAcademy(dto);

      if (params.logo != null) {
        academy = await _uploadBranding(
          academy,
          () async => _remote.uploadLogo(
            academyId: dto.id,
            fileName: params.logo!.name,
            contentType: _contentTypeFor(params.logo!),
            fileBytes: await params.logo!.readAsBytes(),
          ),
        );
      }

      if (params.cover != null) {
        academy = await _uploadBranding(
          academy,
          () async => _remote.uploadBanner(
            academyId: dto.id,
            fileName: params.cover!.name,
            contentType: _contentTypeFor(params.cover!),
            fileBytes: await params.cover!.readAsBytes(),
          ),
        );
      }

      return Result.success(academy);
    } on ApiException catch (error) {
      return Result.failure(CreateAcademyErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<Result<AcademyContact>> updateAcademyContact(
    String academyId,
    CreateAcademyParams params,
  ) async {
    try {
      final request = UpdateContactRequestDto(
        primaryContactName: params.primaryContactName,
        primaryPhone: params.phone,
        primaryEmail: params.email,
        address: params.address,
        country: params.country,
        state: params.state,
        city: params.city,
        postalCode: params.postalCode,
      );
      final dto = await _remote.updateContact(academyId, request);
      return Result.success(AcademyMappers.toContact(dto));
    } on ApiException catch (error) {
      return Result.failure(CreateAcademyErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  static String? _dateOnly(DateTime? date) {
    if (date == null) {
      return null;
    }
    return date.toUtc().toIso8601String().split('T').first;
  }

  static BaseFailure _unexpected(Object error) => UnknownFailure(
    message: 'Unexpected academy creation failure',
    cause: error,
  );
}

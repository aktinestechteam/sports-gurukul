import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_branch_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_contact_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_sport_dto.dart';

part 'academy_dto.freezed.dart';
part 'academy_dto.g.dart';

/// DTO matching the backend `AcademyDto` schema returned by
/// `POST /api/v1/academies` and the academy GET endpoints.
@freezed
abstract class AcademyDto with _$AcademyDto {
  const factory AcademyDto({
    required String id,
    required String academyCode,
    required String name,
    required String email,
    required String phone,
    required String status,
    required String verificationStatus,
    required String createdAt,
    String? legalName,
    String? description,
    String? registrationNumber,
    String? gstNumber,
    String? establishedDate,
    String? website,
    String? academyType,
    String? logoUrl,
    String? bannerUrl,
    String? updatedAt,
    AcademyContactDto? contact,
    List<AcademyBranchDto>? branches,
    List<AcademySportDto>? sports,
  }) = _AcademyDto;

  factory AcademyDto.fromJson(Map<String, dynamic> json) =>
      _$AcademyDtoFromJson(json);
}

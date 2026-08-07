import 'package:freezed_annotation/freezed_annotation.dart';

part 'update_academy_request_dto.freezed.dart';
part 'update_academy_request_dto.g.dart';

/// Request DTO matching the backend `UpdateAcademyCommand` body consumed by
/// `PUT /api/v1/academies/{academyId}`.
///
/// All fields are optional: the backend leaves any omitted field unchanged,
/// so clients only send the fields they want to replace.
@freezed
abstract class UpdateAcademyRequestDto with _$UpdateAcademyRequestDto {
  const factory UpdateAcademyRequestDto({
    String? name,
    String? legalName,
    String? description,
    String? registrationNumber,
    String? gstNumber,
    String? establishedDate,
    String? website,
    String? email,
    String? phone,
  }) = _UpdateAcademyRequestDto;

  factory UpdateAcademyRequestDto.fromJson(Map<String, dynamic> json) =>
      _$UpdateAcademyRequestDtoFromJson(json);
}

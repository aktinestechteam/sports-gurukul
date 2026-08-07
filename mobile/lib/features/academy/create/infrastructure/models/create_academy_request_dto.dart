import 'package:freezed_annotation/freezed_annotation.dart';

part 'create_academy_request_dto.freezed.dart';
part 'create_academy_request_dto.g.dart';

/// Request DTO matching the backend `CreateAcademyRequest` schema.
@freezed
abstract class CreateAcademyRequestDto with _$CreateAcademyRequestDto {
  const factory CreateAcademyRequestDto({
    required String name,
    required String email,
    required String phone,
    String? legalName,
    String? description,
    String? registrationNumber,
    String? gstNumber,
    String? establishedDate,
    String? website,
    String? academyType,
    @Default(<String>[]) List<String> sportNames,
    String? primaryContactName,
    String? address,
    String? country,
    String? state,
    String? city,
    String? postalCode,
  }) = _CreateAcademyRequestDto;

  factory CreateAcademyRequestDto.fromJson(Map<String, dynamic> json) =>
      _$CreateAcademyRequestDtoFromJson(json);
}

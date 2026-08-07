import 'package:freezed_annotation/freezed_annotation.dart';

part 'academy_contact_dto.freezed.dart';
part 'academy_contact_dto.g.dart';

/// DTO matching the backend `ContactDto` schema returned inside
/// `AcademyDto.Contact` and by `PUT /api/v1/academies/{id}/contact`.
@freezed
abstract class AcademyContactDto with _$AcademyContactDto {
  const factory AcademyContactDto({
    String? id,
    String? academyId,
    String? primaryContactName,
    String? primaryPhone,
    String? primaryEmail,
    String? secondaryContactName,
    String? secondaryPhone,
    String? secondaryEmail,
    String? address,
    String? country,
    String? state,
    String? city,
    String? postalCode,
    String? createdAt,
    String? updatedAt,
  }) = _AcademyContactDto;

  factory AcademyContactDto.fromJson(Map<String, dynamic> json) =>
      _$AcademyContactDtoFromJson(json);
}

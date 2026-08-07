import 'package:freezed_annotation/freezed_annotation.dart';

part 'update_contact_request_dto.freezed.dart';
part 'update_contact_request_dto.g.dart';

/// Request DTO matching the backend `UpdateContactCommand` body consumed by
/// `PUT /api/v1/academies/{academyId}/contact`.
///
/// All fields are optional: the backend leaves any omitted field unchanged,
/// so clients only send the fields they want to replace.
@freezed
abstract class UpdateContactRequestDto with _$UpdateContactRequestDto {
  const factory UpdateContactRequestDto({
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
  }) = _UpdateContactRequestDto;

  factory UpdateContactRequestDto.fromJson(Map<String, dynamic> json) =>
      _$UpdateContactRequestDtoFromJson(json);
}

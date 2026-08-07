import 'package:freezed_annotation/freezed_annotation.dart';

part 'contact_dto.freezed.dart';
part 'contact_dto.g.dart';

/// DTO matching the backend `ContactDto` schema.
@freezed
abstract class ContactDto with _$ContactDto {
  const factory ContactDto({
    required String id,
    String? primaryPhoneCountryCode,
    String? primaryPhoneNumber,
    @Default(false) bool primaryPhoneVerified,
    String? secondaryPhoneCountryCode,
    String? secondaryPhoneNumber,
    @Default(false) bool secondaryPhoneVerified,
    String? websiteUrl,
    String? facebookUrl,
    String? twitterUrl,
    String? instagramUrl,
    String? linkedInUrl,
    String? youTubeUrl,
  }) = _ContactDto;

  factory ContactDto.fromJson(Map<String, dynamic> json) =>
      _$ContactDtoFromJson(json);
}

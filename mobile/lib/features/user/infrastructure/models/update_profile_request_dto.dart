import 'package:freezed_annotation/freezed_annotation.dart';

import 'package:sports_gurukul/features/user/infrastructure/models/address_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_profile_dto.dart';

part 'update_profile_request_dto.freezed.dart';
part 'update_profile_request_dto.g.dart';

/// Request body for `PUT /api/v1/users/me`.
///
/// All fields are nullable so only supplied values are sent to the backend
/// (partial update). The backend ignores null fields.
@freezed
abstract class UpdateProfileRequestDto with _$UpdateProfileRequestDto {
  const factory UpdateProfileRequestDto({
    String? dateOfBirth,
    @JsonKey(unknownEnumValue: GenderDto.preferNotToSay)
    GenderDto? gender,
    String? bio,
    String? height,
    String? weight,
    String? preferredSport,
    String? experienceLevel,
    String? primaryPhoneCountryCode,
    String? primaryPhoneNumber,
    String? addressLine1,
    String? addressLine2,
    String? city,
    String? state,
    String? country,
    String? postalCode,
    @JsonKey(unknownEnumValue: AddressTypeDto.home)
    AddressTypeDto? addressType,
  }) = _UpdateProfileRequestDto;

  factory UpdateProfileRequestDto.fromJson(Map<String, dynamic> json) =>
      _$UpdateProfileRequestDtoFromJson(json);
}

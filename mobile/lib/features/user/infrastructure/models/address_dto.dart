import 'package:freezed_annotation/freezed_annotation.dart';

part 'address_dto.freezed.dart';
part 'address_dto.g.dart';

/// DTO matching the backend `AddressDto` schema.
@freezed
abstract class AddressDto with _$AddressDto {
  const factory AddressDto({
    required String id,
    @JsonKey(unknownEnumValue: AddressTypeDto.other)
    required AddressTypeDto addressType,
    required String line1,
    required String city,
    required String state,
    required String country,
    String? line2,
    String? postalCode,
    @Default(false) bool isPrimary,
    double? latitude,
    double? longitude,
  }) = _AddressDto;

  factory AddressDto.fromJson(Map<String, dynamic> json) =>
      _$AddressDtoFromJson(json);
}

/// Address type enum matching the backend `AddressType`.
@JsonEnum(valueField: 'value')
enum AddressTypeDto {
  @JsonValue(0)
  home,
  @JsonValue(1)
  work,
  @JsonValue(2)
  academy,
  @JsonValue(3)
  other,
}

import 'package:freezed_annotation/freezed_annotation.dart';

part 'verify_email_request_dto.freezed.dart';
part 'verify_email_request_dto.g.dart';

/// Request body for `POST /api/v1/auth/verify-email`.
@freezed
abstract class VerifyEmailRequestDto with _$VerifyEmailRequestDto {
  const factory VerifyEmailRequestDto({
    required String token,
  }) = _VerifyEmailRequestDto;

  factory VerifyEmailRequestDto.fromJson(Map<String, dynamic> json) =>
      _$VerifyEmailRequestDtoFromJson(json);
}

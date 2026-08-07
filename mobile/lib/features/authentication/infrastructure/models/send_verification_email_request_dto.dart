import 'package:freezed_annotation/freezed_annotation.dart';

part 'send_verification_email_request_dto.freezed.dart';
part 'send_verification_email_request_dto.g.dart';

/// Request body for `POST /api/v1/auth/send-verification-email`.
@freezed
abstract class SendVerificationEmailRequestDto
    with _$SendVerificationEmailRequestDto {
  const factory SendVerificationEmailRequestDto({
    required String email,
  }) = _SendVerificationEmailRequestDto;

  factory SendVerificationEmailRequestDto.fromJson(
    Map<String, dynamic> json,
  ) => _$SendVerificationEmailRequestDtoFromJson(json);
}

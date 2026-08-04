import 'package:freezed_annotation/freezed_annotation.dart';

part 'forgot_password_request_dto.freezed.dart';
part 'forgot_password_request_dto.g.dart';

/// Request body for `POST /api/v1/auth/forgot-password`.
@freezed
abstract class ForgotPasswordRequestDto with _$ForgotPasswordRequestDto {
  const factory ForgotPasswordRequestDto({
    required String email,
  }) = _ForgotPasswordRequestDto;

  factory ForgotPasswordRequestDto.fromJson(Map<String, dynamic> json) =>
      _$ForgotPasswordRequestDtoFromJson(json);
}

import 'package:freezed_annotation/freezed_annotation.dart';

part 'reset_password_request_dto.freezed.dart';
part 'reset_password_request_dto.g.dart';

/// Request body for `POST /api/v1/auth/reset-password`.
@freezed
abstract class ResetPasswordRequestDto with _$ResetPasswordRequestDto {
  const factory ResetPasswordRequestDto({
    required String token,
    required String newPassword,
    required String confirmNewPassword,
  }) = _ResetPasswordRequestDto;

  factory ResetPasswordRequestDto.fromJson(Map<String, dynamic> json) =>
      _$ResetPasswordRequestDtoFromJson(json);
}

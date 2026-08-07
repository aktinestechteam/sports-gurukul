// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'reset_password_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_ResetPasswordRequestDto _$ResetPasswordRequestDtoFromJson(
  Map<String, dynamic> json,
) => _ResetPasswordRequestDto(
  token: json['token'] as String,
  newPassword: json['newPassword'] as String,
  confirmNewPassword: json['confirmNewPassword'] as String,
);

Map<String, dynamic> _$ResetPasswordRequestDtoToJson(
  _ResetPasswordRequestDto instance,
) => <String, dynamic>{
  'token': instance.token,
  'newPassword': instance.newPassword,
  'confirmNewPassword': instance.confirmNewPassword,
};

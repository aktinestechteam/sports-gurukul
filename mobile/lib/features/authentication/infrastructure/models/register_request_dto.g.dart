// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'register_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_RegisterRequestDto _$RegisterRequestDtoFromJson(Map<String, dynamic> json) =>
    _RegisterRequestDto(
      fullName: json['fullName'] as String,
      email: json['email'] as String,
      password: json['password'] as String,
      confirmPassword: json['confirmPassword'] as String,
      phoneNumber: json['phoneNumber'] as String?,
    );

Map<String, dynamic> _$RegisterRequestDtoToJson(_RegisterRequestDto instance) =>
    <String, dynamic>{
      'fullName': instance.fullName,
      'email': instance.email,
      'password': instance.password,
      'confirmPassword': instance.confirmPassword,
      'phoneNumber': instance.phoneNumber,
    };

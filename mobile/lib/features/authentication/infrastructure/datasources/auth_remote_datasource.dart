import 'package:dio/dio.dart';

import 'package:sports_gurukul/core/constants/api_constants.dart';
import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/error/auth_error_mapper.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/api_response_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/auth_session_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/forgot_password_request_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/login_request_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/message_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/refresh_token_request_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/register_request_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/reset_password_request_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/send_verification_email_request_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/token_pair_dto.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/verify_email_request_dto.dart';

/// Remote auth operations against `api/v1/auth/*`.
///
/// Consumes only the endpoints defined by the backend auth controller
/// (`AuthController.cs`). Every call returns the decoded DTO or throws an
/// [ApiException] carrying the operation code and server detail; transport
/// details never leak to callers.
abstract interface class AuthRemoteDataSource {
  Future<AuthSessionDto> login({
    required String email,
    required String password,
  });

  Future<AuthSessionDto> register({
    required String fullName,
    required String email,
    required String password,
    required String confirmPassword,
    String? phoneNumber,
  });

  Future<TokenPairDto> refreshToken(String refreshToken);

  Future<void> logout();

  Future<MessageDto> forgotPassword(String email);

  Future<MessageDto> resetPassword({
    required String token,
    required String newPassword,
    required String confirmNewPassword,
  });

  Future<MessageDto> sendVerificationEmail(String email);

  Future<MessageDto> verifyEmail(String token);
}

/// [AuthRemoteDataSource] implementation backed by [Dio].
class DioAuthRemoteDataSource implements AuthRemoteDataSource {
  DioAuthRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  static const String _authPath = '${ApiConstants.apiBasePath}/auth';

  @override
  Future<AuthSessionDto> login({
    required String email,
    required String password,
  }) async {
    final response = await _guard(
      () => _dio.post<Map<String, dynamic>>(
        '$_authPath/login',
        data: LoginRequestDto(email: email, password: password).toJson(),
      ),
      operation: AuthOperations.login,
    );
    return _decodeData(response, AuthSessionDto.fromJson);
  }

  @override
  Future<AuthSessionDto> register({
    required String fullName,
    required String email,
    required String password,
    required String confirmPassword,
    String? phoneNumber,
  }) async {
    final response = await _guard(
      () => _dio.post<Map<String, dynamic>>(
        '$_authPath/register',
        data: RegisterRequestDto(
          fullName: fullName,
          email: email,
          password: password,
          confirmPassword: confirmPassword,
          phoneNumber: phoneNumber,
        ).toJson(),
      ),
      operation: AuthOperations.register,
    );
    return _decodeData(response, AuthSessionDto.fromJson);
  }

  @override
  Future<TokenPairDto> refreshToken(String refreshToken) async {
    final response = await _guard(
      () => _dio.post<Map<String, dynamic>>(
        '$_authPath/refresh-token',
        data: RefreshTokenRequestDto(refreshToken: refreshToken).toJson(),
      ),
      operation: AuthOperations.refresh,
    );
    return _decodeData(response, TokenPairDto.fromJson);
  }

  @override
  Future<void> logout() async {
    await _guard(
      () => _dio.post<dynamic>('$_authPath/logout'),
      operation: AuthOperations.logout,
    );
  }

  @override
  Future<MessageDto> forgotPassword(String email) async {
    final response = await _guard(
      () => _dio.post<Map<String, dynamic>>(
        '$_authPath/forgot-password',
        data: ForgotPasswordRequestDto(email: email).toJson(),
      ),
      operation: AuthOperations.forgotPassword,
    );
    return _decodeData(response, MessageDto.fromJson);
  }

  @override
  Future<MessageDto> resetPassword({
    required String token,
    required String newPassword,
    required String confirmNewPassword,
  }) async {
    final response = await _guard(
      () => _dio.post<Map<String, dynamic>>(
        '$_authPath/reset-password',
        data: ResetPasswordRequestDto(
          token: token,
          newPassword: newPassword,
          confirmNewPassword: confirmNewPassword,
        ).toJson(),
      ),
      operation: AuthOperations.resetPassword,
    );
    return _decodeData(response, MessageDto.fromJson);
  }

  @override
  Future<MessageDto> sendVerificationEmail(String email) async {
    final response = await _guard(
      () => _dio.post<Map<String, dynamic>>(
        '$_authPath/send-verification-email',
        data: SendVerificationEmailRequestDto(email: email).toJson(),
      ),
      operation: AuthOperations.sendVerificationEmail,
    );
    return _decodeData(response, MessageDto.fromJson);
  }

  @override
  Future<MessageDto> verifyEmail(String token) async {
    final response = await _guard(
      () => _dio.post<Map<String, dynamic>>(
        '$_authPath/verify-email',
        data: VerifyEmailRequestDto(token: token).toJson(),
      ),
      operation: AuthOperations.verifyEmail,
    );
    return _decodeData(response, MessageDto.fromJson);
  }

  Future<Response<T>> _guard<T>(
    Future<Response<T>> Function() call, {
    required String operation,
  }) async {
    try {
      return await call();
    } on DioException catch (error) {
      throw _toApiException(error, operation);
    }
  }

  ApiException _toApiException(DioException error, String operation) {
    final statusCode = error.response?.statusCode;
    final body = error.response?.data;
    final message =
        _extractServerMessage(body) ?? _messageForStatus(statusCode, operation);

    return ApiException(
      message: message,
      statusCode: statusCode,
      code: operation,
      cause: error,
    );
  }

  T _decodeData<T>(
    Response<dynamic> response,
    T Function(Map<String, dynamic>) fromJson,
  ) {
    final body = response.data;
    if (body is Map<String, dynamic>) {
      final envelope = ApiResponseDto<T>.fromJson(
        body,
        (json) => fromJson(json! as Map<String, dynamic>),
      );
      if (envelope.data != null) {
        return envelope.data!;
      }
    }
    throw const ApiException(
      message: 'Response payload missing "data" object',
    );
  }

  static String? _extractServerMessage(Object? body) {
    if (body is Map<String, dynamic>) {
      final detail = body['detail'];
      if (detail is String && detail.isNotEmpty) {
        return detail;
      }
      final title = body['title'];
      if (title is String && title.isNotEmpty) {
        return title;
      }
      final message = body['message'];
      if (message is String && message.isNotEmpty) {
        return message;
      }
    }
    return null;
  }

  static String _messageForStatus(int? statusCode, String operation) {
    if (statusCode == null) {
      return 'Network request failed';
    }
    return switch (statusCode) {
      >= 500 => 'Server error',
      429 => 'Too many requests. Please try again later.',
      401 => 'Authentication failed',
      400 => 'Invalid request',
      _ => 'Request failed',
    };
  }
}

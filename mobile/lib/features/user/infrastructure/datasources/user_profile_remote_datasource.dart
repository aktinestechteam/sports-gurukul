import 'package:dio/dio.dart';

import 'package:sports_gurukul/core/constants/api_constants.dart';
import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/features/user/infrastructure/error/user_profile_error_mapper.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/profile_photo_response_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/update_preferences_request_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/update_profile_request_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_preference_dto.dart';
import 'package:sports_gurukul/features/user/infrastructure/models/user_profile_dto.dart';

/// Remote user-profile operations against `api/v1/users/*`.
///
/// Consumes only the endpoints defined by the backend UserProfileController.
/// Every call returns the decoded DTO or throws an [ApiException] carrying
/// the operation code and server detail.
abstract interface class UserProfileRemoteDataSource {
  Future<UserProfileDto> getCurrentProfile();

  Future<UserProfileDto> updateProfile(UpdateProfileRequestDto request);

  Future<UserPreferenceDto> updatePreferences(
    UpdatePreferencesRequestDto request,
  );

  Future<ProfilePhotoResponseDto> uploadProfilePhoto({
    required String fileName,
    required String contentType,
    required List<int> fileBytes,
  });

  Future<ProfilePhotoResponseDto> getProfilePhoto();

  Future<void> deleteProfilePhoto();
}

/// [UserProfileRemoteDataSource] implementation backed by [Dio].
class DioUserProfileRemoteDataSource implements UserProfileRemoteDataSource {
  DioUserProfileRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  static const String _usersPath = '${ApiConstants.apiBasePath}/users';

  @override
  Future<UserProfileDto> getCurrentProfile() async {
    final response = await _guard(
      () => _dio.get<Map<String, dynamic>>('$_usersPath/me'),
      operation: UserProfileOperations.getCurrentProfile,
    );
    return _decodeData(response, UserProfileDto.fromJson);
  }

  @override
  Future<UserProfileDto> updateProfile(UpdateProfileRequestDto request) async {
    final response = await _guard(
      () => _dio.put<Map<String, dynamic>>(
        '$_usersPath/me',
        data: request.toJson(),
      ),
      operation: UserProfileOperations.updateProfile,
    );
    return _decodeData(response, UserProfileDto.fromJson);
  }

  @override
  Future<UserPreferenceDto> updatePreferences(
    UpdatePreferencesRequestDto request,
  ) async {
    final response = await _guard(
      () => _dio.put<Map<String, dynamic>>(
        '$_usersPath/preferences',
        data: request.toJson(),
      ),
      operation: UserProfileOperations.updatePreferences,
    );
    return _decodeData(response, UserPreferenceDto.fromJson);
  }

  @override
  Future<ProfilePhotoResponseDto> uploadProfilePhoto({
    required String fileName,
    required String contentType,
    required List<int> fileBytes,
  }) async {
    final formData = FormData.fromMap({
      'file': MultipartFile.fromBytes(
        fileBytes,
        filename: fileName,
        contentType: DioMediaType(
          contentType.split('/').first,
          contentType.split('/').last,
        ),
      ),
    });
    final response = await _guard(
      () => _dio.post<Map<String, dynamic>>(
        '$_usersPath/me/photo',
        data: formData,
      ),
      operation: UserProfileOperations.uploadPhoto,
    );
    return _decodeData(response, ProfilePhotoResponseDto.fromJson);
  }

  @override
  Future<ProfilePhotoResponseDto> getProfilePhoto() async {
    final response = await _guard(
      () => _dio.get<Map<String, dynamic>>('$_usersPath/me/photo'),
      operation: UserProfileOperations.getPhoto,
    );
    return _decodeData(response, ProfilePhotoResponseDto.fromJson);
  }

  @override
  Future<void> deleteProfilePhoto() async {
    await _guard(
      () => _dio.delete<dynamic>('$_usersPath/me/photo'),
      operation: UserProfileOperations.deletePhoto,
    );
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
      final data = body['data'];
      if (data is Map<String, dynamic>) {
        return fromJson(data);
      }
      if (data == null) {
        throw const ApiException(
          message: 'Response payload missing "data" object',
        );
      }
    }
    throw const ApiException(
      message: 'Unexpected response format',
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
      404 => 'Resource not found',
      400 => 'Invalid request',
      _ => 'Request failed ($operation)',
    };
  }
}

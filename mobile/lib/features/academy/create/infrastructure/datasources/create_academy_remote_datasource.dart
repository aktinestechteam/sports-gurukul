import 'package:dio/dio.dart';

import 'package:sports_gurukul/core/constants/api_constants.dart';
import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/error/create_academy_error_mapper.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_contact_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/create_academy_request_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/update_academy_request_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/update_contact_request_dto.dart';

/// Remote academy-creation operations against `api/v1/academies`.
///
/// Consumes only the endpoints defined by the backend AcademyController.
/// Every call returns the decoded DTO or throws an [ApiException] carrying
/// the operation code and server detail.
abstract interface class CreateAcademyRemoteDataSource {
  Future<AcademyDto> createAcademy(CreateAcademyRequestDto request);

  Future<AcademyDto> getAcademy(String academyId);

  /// Resolves the current user's owned academy, or null when they own none.
  Future<AcademyDto?> getMyAcademy();

  /// Updates an existing academy's core fields. The backend leaves any
  /// omitted field unchanged.
  Future<AcademyDto> updateAcademy(
    String academyId,
    UpdateAcademyRequestDto request,
  );

  /// Updates an existing academy's contact + address block. The backend
  /// leaves any omitted field unchanged.
  Future<AcademyContactDto> updateContact(
    String academyId,
    UpdateContactRequestDto request,
  );

  Future<AcademyDto> uploadLogo({
    required String academyId,
    required String fileName,
    required String contentType,
    required List<int> fileBytes,
  });

  Future<AcademyDto> uploadBanner({
    required String academyId,
    required String fileName,
    required String contentType,
    required List<int> fileBytes,
  });
}

/// [CreateAcademyRemoteDataSource] implementation backed by [Dio].
class DioCreateAcademyRemoteDataSource
    implements CreateAcademyRemoteDataSource {
  DioCreateAcademyRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  static const String _academiesPath = '${ApiConstants.apiBasePath}/academies';

  @override
  Future<AcademyDto> createAcademy(CreateAcademyRequestDto request) async {
    final response = await _guard(
      () => _dio.post<Map<String, dynamic>>(
        _academiesPath,
        data: request.toJson(),
      ),
      operation: CreateAcademyOperations.createAcademy,
    );
    return _decodeData(response, AcademyDto.fromJson);
  }

  @override
  Future<AcademyDto> getAcademy(String academyId) async {
    final response = await _guard(
      () => _dio.get<Map<String, dynamic>>('$_academiesPath/$academyId'),
      operation: CreateAcademyOperations.getAcademy,
    );
    return _decodeData(response, AcademyDto.fromJson);
  }

  @override
  Future<AcademyDto?> getMyAcademy() async {
    try {
      final response = await _dio.get<Map<String, dynamic>>(
        '$_academiesPath/my',
      );
      return _decodeData(response, AcademyDto.fromJson);
    } on DioException catch (error) {
      if (error.response?.statusCode == 404) {
        return null;
      }
      throw _toApiException(error, CreateAcademyOperations.getMyAcademy);
    }
  }

  @override
  Future<AcademyDto> updateAcademy(
    String academyId,
    UpdateAcademyRequestDto request,
  ) async {
    final response = await _guard(
      () => _dio.put<Map<String, dynamic>>(
        '$_academiesPath/$academyId',
        data: request.toJson(),
      ),
      operation: CreateAcademyOperations.updateAcademy,
    );
    return _decodeData(response, AcademyDto.fromJson);
  }

  @override
  Future<AcademyContactDto> updateContact(
    String academyId,
    UpdateContactRequestDto request,
  ) async {
    final response = await _guard(
      () => _dio.put<Map<String, dynamic>>(
        '$_academiesPath/$academyId/contact',
        data: request.toJson(),
      ),
      operation: CreateAcademyOperations.updateContact,
    );
    return _decodeData(response, AcademyContactDto.fromJson);
  }

  @override
  Future<AcademyDto> uploadLogo({
    required String academyId,
    required String fileName,
    required String contentType,
    required List<int> fileBytes,
  }) =>
      _uploadImage(
        academyId: academyId,
        endpoint: 'logo',
        fileName: fileName,
        contentType: contentType,
        fileBytes: fileBytes,
        operation: CreateAcademyOperations.uploadLogo,
      );

  @override
  Future<AcademyDto> uploadBanner({
    required String academyId,
    required String fileName,
    required String contentType,
    required List<int> fileBytes,
  }) =>
      _uploadImage(
        academyId: academyId,
        endpoint: 'banner',
        fileName: fileName,
        contentType: contentType,
        fileBytes: fileBytes,
        operation: CreateAcademyOperations.uploadBanner,
      );

  Future<AcademyDto> _uploadImage({
    required String academyId,
    required String endpoint,
    required String fileName,
    required String contentType,
    required List<int> fileBytes,
    required String operation,
  }) async {
    final contentTypeParts = contentType.split('/');
    final formData = FormData.fromMap({
      'file': MultipartFile.fromBytes(
        fileBytes,
        filename: fileName,
        contentType: DioMediaType(
          contentTypeParts.first,
          contentTypeParts.length > 1 ? contentTypeParts.last : 'octet-stream',
        ),
      ),
    });
    final response = await _guard(
      () => _dio.post<Map<String, dynamic>>(
        '$_academiesPath/$academyId/$endpoint',
        data: formData,
      ),
      operation: operation,
    );
    return _decodeData(response, AcademyDto.fromJson);
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

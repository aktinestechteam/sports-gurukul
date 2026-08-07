import 'package:freezed_annotation/freezed_annotation.dart';

part 'api_response_dto.freezed.dart';

/// The backend's envelope for every successful JSON response.
///
/// `{ success, message, data, errors }`. [T] is the typed payload; on
/// failure the payload is absent and [errors] carries human-readable reasons.
///
/// JSON is hand-written here because freezed's generated redirect cannot
/// target the generic `_$ApiResponseDtoFromJson<T>` function.
@freezed
abstract class ApiResponseDto<T> with _$ApiResponseDto<T> {
  const factory ApiResponseDto({
    @Default(false) bool success,
    @Default('') String message,
    T? data,
    @Default(<String>[]) List<String> errors,
  }) = _ApiResponseDto<T>;

  factory ApiResponseDto.fromJson(
    Map<String, dynamic> json,
    T Function(Object? json) fromJsonT,
  ) {
    final rawData = json['data'];
    return ApiResponseDto<T>(
      success: json['success'] as bool? ?? false,
      message: json['message'] as String? ?? '',
      data: rawData == null ? null : fromJsonT(rawData),
      errors: (json['errors'] as List<dynamic>?)?.cast<String>() ?? const [],
    );
  }
}

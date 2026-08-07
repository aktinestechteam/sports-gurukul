import 'package:freezed_annotation/freezed_annotation.dart';

part 'profile_photo_response_dto.freezed.dart';
part 'profile_photo_response_dto.g.dart';

/// DTO matching the backend `ProfilePhotoResponse` schema.
@freezed
abstract class ProfilePhotoResponseDto with _$ProfilePhotoResponseDto {
  const factory ProfilePhotoResponseDto({
    required String fileId,
    required String url,
    required String fileName,
    required int fileSize,
    required String contentType,
    required String uploadedAt,
  }) = _ProfilePhotoResponseDto;

  factory ProfilePhotoResponseDto.fromJson(Map<String, dynamic> json) =>
      _$ProfilePhotoResponseDtoFromJson(json);
}

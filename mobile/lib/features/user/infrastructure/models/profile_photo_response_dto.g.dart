// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'profile_photo_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_ProfilePhotoResponseDto _$ProfilePhotoResponseDtoFromJson(
  Map<String, dynamic> json,
) => _ProfilePhotoResponseDto(
  fileId: json['fileId'] as String,
  url: json['url'] as String,
  fileName: json['fileName'] as String,
  fileSize: (json['fileSize'] as num).toInt(),
  contentType: json['contentType'] as String,
  uploadedAt: json['uploadedAt'] as String,
);

Map<String, dynamic> _$ProfilePhotoResponseDtoToJson(
  _ProfilePhotoResponseDto instance,
) => <String, dynamic>{
  'fileId': instance.fileId,
  'url': instance.url,
  'fileName': instance.fileName,
  'fileSize': instance.fileSize,
  'contentType': instance.contentType,
  'uploadedAt': instance.uploadedAt,
};

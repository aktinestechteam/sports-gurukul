import 'package:flutter/foundation.dart';

/// Profile photo metadata returned by the upload / get endpoints.
@immutable
class ProfilePhoto {
  const ProfilePhoto({
    required this.fileId,
    required this.url,
    required this.fileName,
    required this.fileSize,
    required this.contentType,
    required this.uploadedAt,
  });

  final String fileId;
  final String url;
  final String fileName;
  final int fileSize;
  final String contentType;
  final DateTime uploadedAt;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ProfilePhoto &&
          runtimeType == other.runtimeType &&
          fileId == other.fileId &&
          url == other.url &&
          fileName == other.fileName &&
          fileSize == other.fileSize &&
          contentType == other.contentType;

  @override
  int get hashCode => Object.hash(fileId, url, fileName, fileSize, contentType);
}

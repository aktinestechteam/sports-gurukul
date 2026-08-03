import 'package:sports_gurukul/core/constants/validation_constants.dart';
import 'package:sports_gurukul/core/utils/file_utils.dart';

/// Lightweight image metadata and size helpers.
///
/// Used for picker and upload guardrails (extension/mime checks, size
/// limits). Pixel decoding and compression are owned by the image feature.
abstract final class ImageUtils {
  static const Map<String, String> _mimeTypes = {
    'jpg': 'image/jpeg',
    'jpeg': 'image/jpeg',
    'png': 'image/png',
    'webp': 'image/webp',
    'gif': 'image/gif',
    'heic': 'image/heic',
    'heif': 'image/heif',
    'bmp': 'image/bmp',
    'svg': 'image/svg+xml',
  };

  /// The MIME type for [fileName]'s extension, or `null` when the
  /// extension is not a known image type.
  static String? mimeTypeOf(String fileName) =>
      _mimeTypes[FileUtils.extensionOf(fileName)];

  /// Whether [fileName] has a supported image extension.
  static bool isSupportedImage(String fileName) =>
      _mimeTypes.containsKey(FileUtils.extensionOf(fileName));

  /// Whether [bytes] fits within [maxBytes] (defaults to the standard
  /// image upload limit).
  static bool isWithinSizeLimit(
    int bytes, {
    int maxBytes = ValidationConstants.maxImageUploadBytes,
  }) => bytes >= 0 && bytes <= maxBytes;
}

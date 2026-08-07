import 'dart:io';

import 'package:sports_gurukul/core/constants/regex_constants.dart';
import 'package:sports_gurukul/core/utils/formatter.dart';

/// File-system helpers.
///
/// All IO is asynchronous to avoid blocking the UI thread. Intended for
/// attachment downloads, exports and cache staging; business file handling
/// lives in the owning feature.
abstract final class FileUtils {
  /// Returns the lowercased extension of [path] (without the dot), or `''`
  /// when there is none.
  static String extensionOf(String path) {
    final name = fileNameOf(path);
    final dot = name.lastIndexOf('.');
    if (dot == -1 || dot == name.length - 1) {
      return '';
    }
    return name.substring(dot + 1).toLowerCase();
  }

  /// Returns the final path segment of [path].
  static String fileNameOf(String path) {
    final normalized = path.replaceAll(r'\', '/');
    final slash = normalized.lastIndexOf('/');
    return slash == -1 ? normalized : normalized.substring(slash + 1);
  }

  /// Replaces characters that are invalid in file names with `_`.
  static String sanitizeFileName(String name) =>
      name.trim().replaceAll(RegexConstants.invalidFileName, '_');

  /// Writes [bytes] to a new file inside a fresh temp directory and returns
  /// the file. The caller owns cleanup of the returned file.
  static Future<File> writeTempFile(String fileName, List<int> bytes) async {
    final directory = await Directory.systemTemp.createTemp('sports_gurukul_');
    final file = File(
      '${directory.path}${Platform.pathSeparator}${sanitizeFileName(fileName)}',
    );
    await file.writeAsBytes(bytes, flush: true);
    return file;
  }

  /// Formats [bytes] as a human-readable size, e.g. `1.5 MB`.
  static String sizeLabel(int bytes) => Formatter.bytes(bytes);
}

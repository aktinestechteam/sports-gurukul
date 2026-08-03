import 'dart:convert';

/// Failure-tolerant parsing helpers.
///
/// Every method returns `null` instead of throwing when the input is not
/// parseable, so callers never need try/catch for routine conversions.
abstract final class Parser {
  /// Parses [input] as an [int], or `null` when it is not numeric.
  static int? intOrNull(String? input) {
    final candidate = input?.trim();
    if (candidate == null || candidate.isEmpty) {
      return null;
    }
    return int.tryParse(candidate);
  }

  /// Parses [input] as a [double], or `null` when it is not numeric.
  static double? doubleOrNull(String? input) {
    final candidate = input?.trim();
    if (candidate == null || candidate.isEmpty) {
      return null;
    }
    return double.tryParse(candidate);
  }

  /// Parses [input] as a [bool].
  ///
  /// Accepts `true`/`false`, `1`/`0` and `yes`/`no` (case-insensitive).
  /// Returns `null` for anything else.
  static bool? boolOrNull(String? input) {
    final candidate = input?.trim().toLowerCase();
    return switch (candidate) {
      'true' || '1' || 'yes' || 'y' => true,
      'false' || '0' || 'no' || 'n' => false,
      _ => null,
    };
  }

  /// Parses [input] as a [DateTime], or `null` when it is not a valid
  /// ISO-8601 timestamp.
  static DateTime? dateTimeOrNull(String? input) {
    final candidate = input?.trim();
    if (candidate == null || candidate.isEmpty) {
      return null;
    }
    return DateTime.tryParse(candidate);
  }

  /// Decodes [input] as a JSON object, or `null` when it is not valid JSON
  /// or is not a JSON object.
  static Map<String, dynamic>? jsonMapOrNull(String? input) {
    final candidate = input?.trim();
    if (candidate == null || candidate.isEmpty) {
      return null;
    }
    try {
      final decoded = jsonDecode(candidate);
      return decoded is Map<String, dynamic> ? decoded : null;
    } on FormatException {
      return null;
    }
  }
}

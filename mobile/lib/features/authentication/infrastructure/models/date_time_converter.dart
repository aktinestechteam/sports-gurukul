import 'package:json_annotation/json_annotation.dart';

/// Parses backend timestamps into [DateTime]s.
///
/// ASP.NET Core serializes `DateTime` with seven fractional-second digits
/// (e.g. `2026-08-03T10:30:00.1234567Z`) which `DateTime.parse` rejects
/// (it accepts at most six). This converter normalizes the fraction to six
/// digits and treats zone-less timestamps as UTC, matching the backend's UTC
/// contract.
class FlexibleDateTimeConverter implements JsonConverter<DateTime, String> {
  const FlexibleDateTimeConverter();

  @override
  DateTime fromJson(String value) {
    final parsed = _tryParse(value);
    if (parsed == null) {
      throw FormatException('Invalid date-time value: $value');
    }
    return parsed;
  }

  @override
  String toJson(DateTime value) => value.toUtc().toIso8601String();

  /// Parses [input], tolerant of seven-digit fractions and missing zones.
  static DateTime? parse(String input) => _tryParse(input);

  /// Parses [input], tolerant of seven-digit fractions and missing zones.
  static DateTime? _tryParse(String input) {
    final hasZone = RegExp(r'(Z|z|[+-]\d{2}:?\d{2})$').hasMatch(input);
    final withZone = hasZone ? input : '${input}Z';
    final normalized = withZone.replaceFirstMapped(
      RegExp(r'\.(\d{7,})'),
      (match) => '.${match.group(1)!.substring(0, 6)}',
    );
    return DateTime.tryParse(normalized);
  }
}

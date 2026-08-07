import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/models/date_time_converter.dart';

void main() {
  const converter = FlexibleDateTimeConverter();

  group('FlexibleDateTimeConverter', () {
    test('parses seven-digit fractional seconds', () {
      final parsed = converter.fromJson('2026-08-03T10:30:00.1234567Z');
      expect(parsed, DateTime.utc(2026, 8, 3, 10, 30, 0, 0, 123456));
    });

    test('treats zone-less timestamps as UTC', () {
      final parsed = converter.fromJson('2026-08-03T10:30:00');
      expect(parsed, DateTime.utc(2026, 8, 3, 10, 30));
    });

    test('preserves an explicit timezone offset', () {
      final parsed = converter.fromJson('2026-08-03T10:30:00+05:30');
      expect(parsed, DateTime.parse('2026-08-03T10:30:00+05:30'));
    });

    test('static parse returns null for invalid input', () {
      expect(FlexibleDateTimeConverter.parse('not-a-date'), isNull);
      expect(FlexibleDateTimeConverter.parse(''), isNull);
    });

    test('toJson emits UTC ISO-8601', () {
      final value = DateTime.parse('2026-08-03T10:30:00.123Z');
      expect(converter.toJson(value), '2026-08-03T10:30:00.123Z');
    });
  });
}

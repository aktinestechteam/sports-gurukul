import 'package:flutter_test/flutter_test.dart';
import 'package:intl/date_symbol_data_local.dart';
import 'package:sports_gurukul/core/utils/date_utils.dart';

void main() {
  setUpAll(initializeDateFormatting);

  group('date boundaries', () {
    final day = DateTime(2026, 8, 3, 10, 30);

    test('startOfDay and endOfDay', () {
      expect(dateStartOfDay(day), DateTime(2026, 8, 3));
      expect(dateEndOfDay(day), DateTime(2026, 8, 3, 23, 59, 59, 999));
    });

    test('startOfWeek and endOfWeek start on Monday', () {
      expect(dateStartOfWeek(day), DateTime(2026, 8, 3));
      expect(dateEndOfWeek(day), DateTime(2026, 8, 9, 23, 59, 59, 999));
    });

    test('startOfWeek handles a mid-week date', () {
      final wednesday = DateTime(2026, 8, 5);
      expect(dateStartOfWeek(wednesday), DateTime(2026, 8, 3));
    });
  });

  group('date comparisons', () {
    test('isSameDay and isToday', () {
      expect(
        dateIsSameDay(DateTime(2026, 8, 3), DateTime(2026, 8, 3, 23)),
        isTrue,
      );
      expect(
        dateIsSameDay(DateTime(2026, 8, 3), DateTime(2026, 8, 4)),
        isFalse,
      );
      expect(dateIsToday(DateTime.now()), isTrue);
    });

    test('isSameMonth', () {
      expect(
        dateIsSameMonth(DateTime(2026, 8), DateTime(2026, 8, 31)),
        isTrue,
      );
      expect(
        dateIsSameMonth(DateTime(2026, 8), DateTime(2026, 9)),
        isFalse,
      );
    });

    test('isBetween is inclusive', () {
      expect(
        dateIsBetween(
          DateTime(2026, 8, 3),
          DateTime(2026, 8, 3),
          DateTime(2026, 8, 5),
        ),
        isTrue,
      );
      expect(
        dateIsBetween(
          DateTime(2026, 8, 6),
          DateTime(2026, 8, 3),
          DateTime(2026, 8, 5),
        ),
        isFalse,
      );
    });
  });

  group('ageYears', () {
    test('counts whole years against today', () {
      final now = DateTime.now();
      expect(ageYears(DateTime(now.year - 30, now.month, now.day - 1)), 30);
      expect(ageYears(DateTime(now.year - 30, now.month, now.day + 1)), 29);
    });
  });

  group('formatting', () {
    final value = DateTime(2026, 8, 3, 9, 30);

    test('formats dates and times', () {
      expect(formatDate(value), '03 Aug 2026');
      expect(formatTime(value), '09:30 AM');
      expect(formatDateTime(value), '03 Aug 2026, 09:30 AM');
    });

    test('honours custom patterns', () {
      expect(formatDate(value, pattern: 'dd/MM/yyyy'), '03/08/2026');
    });
  });
}

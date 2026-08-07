import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/extensions/datetime_extensions.dart';
import 'package:sports_gurukul/core/extensions/duration_extensions.dart';
import 'package:sports_gurukul/core/extensions/iterable_extensions.dart';
import 'package:sports_gurukul/core/extensions/list_extensions.dart';
import 'package:sports_gurukul/core/extensions/number_extensions.dart';
import 'package:sports_gurukul/core/extensions/string_extensions.dart';

void main() {
  group('StringX', () {
    test('blank detection', () {
      expect(''.isBlank, isTrue);
      expect('   '.isBlank, isTrue);
      expect('x'.isBlank, isFalse);
      expect('  '.nullIfBlank, isNull);
      expect('x'.nullIfBlank, 'x');
    });

    test('parsing', () {
      expect('42'.toIntOrNull(), 42);
      expect('4.2'.toIntOrNull(), isNull);
      expect('4.2'.toDoubleOrNull(), 4.2);
      expect(' 7 '.toIntOrNull(), 7);
    });

    test('capitalization', () {
      expect('hello'.capitalize(), 'Hello');
      expect(''.capitalize(), '');
      expect('hello world'.titleCase(), 'Hello World');
    });

    test('whitespace and truncation', () {
      expect('a b c'.removeAllWhitespace(), 'abc');
      expect('sports gurukul'.truncate(6), 'sports...');
      expect('short'.truncate(10), 'short');
    });
  });

  group('DateTimeX', () {
    final day = DateTime(2026, 8, 3, 10, 30);

    test('day boundaries', () {
      expect(day.startOfDay(), DateTime(2026, 8, 3));
      expect(day.endOfDay(), DateTime(2026, 8, 3, 23, 59, 59, 999));
    });

    test('week boundaries start on Monday', () {
      expect(day.startOfWeek(), DateTime(2026, 8, 3));
      expect(day.endOfWeek(), DateTime(2026, 8, 9, 23, 59, 59, 999));
    });

    test('same-day comparisons', () {
      expect(day.isSameDay(DateTime(2026, 8, 3, 23, 59)), isTrue);
      expect(day.isSameDay(DateTime(2026, 8, 4)), isFalse);
      expect(DateTime.now().isToday(), isTrue);
      expect(
        DateTime.now().subtract(const Duration(days: 1)).isYesterday(),
        isTrue,
      );
      expect(DateTime.now().add(const Duration(days: 1)).isTomorrow(), isTrue);
    });

    test('inclusive range check', () {
      expect(day.isBetween(DateTime(2026, 8, 3), DateTime(2026, 8, 5)), isTrue);
      expect(
        day.isBetween(DateTime(2026, 8, 4), DateTime(2026, 8, 5)),
        isFalse,
      );
    });
  });

  group('IterableX', () {
    test('lookups', () {
      expect([1, 2, 3].firstWhereOrNull((x) => x > 1), 2);
      expect([1, 2, 3].firstWhereOrNull((x) => x > 9), isNull);
      expect([1, 2, 3].lastWhereOrNull((x) => x < 3), 2);
      expect([1, 2, 3].elementAtOrNull(1), 2);
      expect([1, 2, 3].elementAtOrNull(9), isNull);
      expect([1, 2, 3].elementAtOrNull(-1), isNull);
    });

    test('counting and indexing', () {
      expect([1, 2, 3, 4].countWhere((x) => x.isEven), 2);
      expect(['a', 'b'].mapIndexed((i, e) => '$i$e'), ['0a', '1b']);
    });

    test('grouping and summing', () {
      final grouped = ['cat', 'dog', 'car'].groupBy((w) => w[0]);
      expect(grouped['c'], ['cat', 'car']);
      expect([1, 2, 3].sumBy((x) => x * 2), 12);
    });

    test('whereNotNull drops nulls', () {
      expect(<int?>[1, null, 2, null].whereNotNull(), [1, 2]);
    });
  });

  group('ListX', () {
    test('replaceOrAppend replaces the first match', () {
      final list = [1, 2, 3]..replaceOrAppend(9, test: (x) => x == 2);
      expect(list, [1, 9, 3]);
    });

    test('replaceOrAppend appends when nothing matches', () {
      final list = [1, 9, 3]..replaceOrAppend(4, test: (x) => x == 99);
      expect(list, [1, 9, 3, 4]);
    });

    test('membership checks', () {
      expect([1, 2, 3].containsWhere((x) => x == 2), isTrue);
      expect([1, 2, 3].containsWhere((x) => x == 9), isFalse);
      expect([1, 2, 3].indexWhereOrNull((x) => x == 2), 1);
      expect([1, 2, 3].indexWhereOrNull((x) => x == 9), isNull);
    });

    test('unique and sortedCopy do not mutate the receiver', () {
      expect([1, 1, 2, 3, 3].unique(), [1, 2, 3]);
      final source = [3, 1, 2];
      expect(source.sortedCopy(), [1, 2, 3]);
      expect(source, [3, 1, 2]);
    });
  });

  group('NumberX', () {
    test('range checks', () {
      expect(5.isBetween(1, 10), isTrue);
      expect(5.isBetween(5, 5), isTrue);
      expect(5.5.isBetween(1, 5), isFalse);
    });

    test('formatting', () {
      expect(1234.toFormatted(), '1,234');
      expect(1234.5.toFormatted(decimalDigits: 1), '1,234.5');
      expect(1500.toCompact(), contains('K'));
    });

    test('ordinals', () {
      expect(1.ordinal(), '1st');
      expect(2.ordinal(), '2nd');
      expect(3.ordinal(), '3rd');
      expect(11.ordinal(), '11th');
      expect(23.ordinal(), '23rd');
    });

    test('precision rounding', () {
      expect(2.345.roundToPrecision(2), 2.35);
      expect(2.3.roundToPrecision(2), 2.3);
    });
  });

  group('DurationX', () {
    test('labels', () {
      expect(const Duration(days: 2, hours: 3).toShortLabel(), '2d 3h');
      expect(
        const Duration(hours: 2, minutes: 3, seconds: 4).toClockLabel(),
        '02:03:04',
      );
    });

    test('clamping and comparison', () {
      expect(
        const Duration(
          seconds: 5,
        ).clampTo(const Duration(seconds: 1), const Duration(seconds: 10)),
        const Duration(seconds: 5),
      );
      expect(
        const Duration(
          seconds: 30,
        ).clampTo(const Duration(seconds: 1), const Duration(seconds: 10)),
        const Duration(seconds: 10),
      );
      expect(
        const Duration(seconds: 2).isShorterThan(const Duration(seconds: 3)),
        isTrue,
      );
      expect(
        const Duration(seconds: 4).isLongerThan(const Duration(seconds: 3)),
        isTrue,
      );
    });
  });
}

import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/utils/formatter.dart';
import 'package:sports_gurukul/core/utils/parser.dart';

void main() {
  group('Formatter', () {
    test('numbers are grouped and rounded', () {
      expect(Formatter.number(1234), '1,234');
      expect(Formatter.number(1234.56, decimalDigits: 1), '1,234.6');
    });

    test('currency uses the rupee symbol by default', () {
      expect(Formatter.currency(1000), '₹1,000');
      expect(Formatter.currency(9.99, symbol: r'$'), r'$9.99');
    });

    test('percent appends the sign', () {
      expect(Formatter.percent(12.5), '12.5%');
    });

    test('bytes formats units', () {
      expect(Formatter.bytes(0), '0 B');
      expect(Formatter.bytes(1024), '1 KB');
      expect(Formatter.bytes(1536), '1.5 KB');
      expect(Formatter.bytes(1024 * 1024), '1 MB');
    });

    test('phone masking', () {
      expect(Formatter.phoneMask('9876543210'), '+91 98765 43210');
      expect(Formatter.phoneMask('123'), '123');
    });

    test('email masking', () {
      expect(
        Formatter.maskEmail('rohit.sharma@gmail.com'),
        'r**********a@gmail.com',
      );
      expect(Formatter.maskEmail('a@b.com'), 'a@b.com');
    });

    test('duration formatting', () {
      expect(Formatter.duration(const Duration(days: 2, hours: 3)), '2d 3h');
      expect(Formatter.duration(const Duration(minutes: 45)), '45m');
      expect(Formatter.duration(const Duration(seconds: 8)), '8s');
      expect(
        Formatter.durationClock(
          const Duration(hours: 2, minutes: 3, seconds: 4),
        ),
        '02:03:04',
      );
    });
  });

  group('Parser', () {
    test('parses ints', () {
      expect(Parser.intOrNull('42'), 42);
      expect(Parser.intOrNull(' 7 '), 7);
      expect(Parser.intOrNull('4.2'), isNull);
      expect(Parser.intOrNull(''), isNull);
      expect(Parser.intOrNull(null), isNull);
    });

    test('parses doubles', () {
      expect(Parser.doubleOrNull('4.2'), 4.2);
      expect(Parser.doubleOrNull('abc'), isNull);
    });

    test('parses bools', () {
      expect(Parser.boolOrNull('true'), isTrue);
      expect(Parser.boolOrNull('1'), isTrue);
      expect(Parser.boolOrNull('YES'), isTrue);
      expect(Parser.boolOrNull('false'), isFalse);
      expect(Parser.boolOrNull('0'), isFalse);
      expect(Parser.boolOrNull('maybe'), isNull);
    });

    test('parses dates', () {
      final parsed = Parser.dateTimeOrNull('2026-08-03T09:30:00Z');
      expect(parsed?.year, 2026);
      expect(parsed?.month, 8);
      expect(Parser.dateTimeOrNull('not a date'), isNull);
    });

    test('parses JSON objects', () {
      expect(Parser.jsonMapOrNull('{"a":1}'), {'a': 1});
      expect(Parser.jsonMapOrNull('[1,2]'), isNull);
      expect(Parser.jsonMapOrNull('not json'), isNull);
      expect(Parser.jsonMapOrNull(''), isNull);
    });
  });
}

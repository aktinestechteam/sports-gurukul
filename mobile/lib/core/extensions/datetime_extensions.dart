import 'package:sports_gurukul/core/utils/date_utils.dart';

/// Convenience extensions on [DateTime].
extension DateTimeX on DateTime {
  /// Whether this date falls on the same calendar day as [other].
  bool isSameDay(DateTime other) => dateIsSameDay(this, other);

  /// Whether this date is today.
  bool isToday() => dateIsToday(this);

  /// Whether this date is the day before today.
  bool isYesterday() => dateIsSameDay(
    this,
    DateTime.now().subtract(const Duration(days: 1)),
  );

  /// Whether this date is the day after today.
  bool isTomorrow() => dateIsSameDay(
    this,
    DateTime.now().add(const Duration(days: 1)),
  );

  /// This date at 00:00:00.000.
  DateTime startOfDay() => dateStartOfDay(this);

  /// This date at 23:59:59.999.
  DateTime endOfDay() => dateEndOfDay(this);

  /// The first day (Monday) of this date's week at 00:00:00.000.
  DateTime startOfWeek() => dateStartOfWeek(this);

  /// The last day (Sunday) of this date's week at 23:59:59.999.
  DateTime endOfWeek() => dateEndOfWeek(this);

  /// Whether this instant falls within [start] and [end], inclusive.
  bool isBetween(DateTime start, DateTime end) =>
      dateIsBetween(this, start, end);
}

import 'package:intl/intl.dart';

/// Pure date helpers shared across the app.
///
/// Exposed as top-level functions rather than a class to avoid colliding
/// with Flutter's `DateUtils`. The `DateTimeX` extension delegates here so
/// all date logic lives in one place.
DateTime dateStartOfDay(DateTime value) =>
    DateTime(value.year, value.month, value.day);

/// The last instant of [value]'s day (23:59:59.999).
DateTime dateEndOfDay(DateTime value) =>
    DateTime(value.year, value.month, value.day, 23, 59, 59, 999);

/// The first instant of [value]'s week (Monday 00:00:00.000).
DateTime dateStartOfWeek(DateTime value, {int firstDay = DateTime.monday}) {
  final day = dateStartOfDay(value);
  final offset = (day.weekday - firstDay) % 7;
  return day.subtract(Duration(days: offset));
}

/// The last instant of [value]'s week (Sunday 23:59:59.999).
DateTime dateEndOfWeek(DateTime value, {int firstDay = DateTime.monday}) =>
    dateStartOfWeek(value, firstDay: firstDay).add(
      const Duration(
        days: 6,
        hours: 23,
        minutes: 59,
        seconds: 59,
        milliseconds: 999,
      ),
    );

/// Whether [a] and [b] fall on the same calendar day.
bool dateIsSameDay(DateTime a, DateTime b) =>
    a.year == b.year && a.month == b.month && a.day == b.day;

/// Whether [value] falls on the current calendar day.
bool dateIsToday(DateTime value) => dateIsSameDay(value, DateTime.now());

/// Whether [a] and [b] fall in the same calendar month.
bool dateIsSameMonth(DateTime a, DateTime b) =>
    a.year == b.year && a.month == b.month;

/// Whether [value] is within [start] and [end], inclusive.
bool dateIsBetween(DateTime value, DateTime start, DateTime end) =>
    !value.isBefore(start) && !value.isAfter(end);

/// Whole years completed since [birthDate].
int ageYears(DateTime birthDate) {
  final now = DateTime.now();
  var age = now.year - birthDate.year;
  final birthdayNotReached =
      now.month < birthDate.month ||
      (now.month == birthDate.month && now.day < birthDate.day);
  if (birthdayNotReached) {
    age--;
  }
  return age;
}

/// Formats [value] as a date, e.g. `03 Aug 2026`.
String formatDate(
  DateTime value, {
  String pattern = 'dd MMM yyyy',
  String locale = 'en',
}) => DateFormat(pattern, locale).format(value);

/// Formats [value] as a time, e.g. `09:30 AM`.
String formatTime(
  DateTime value, {
  String pattern = 'hh:mm a',
  String locale = 'en',
}) => DateFormat(pattern, locale).format(value);

/// Formats [value] as a date and time, e.g. `03 Aug 2026, 09:30 AM`.
String formatDateTime(
  DateTime value, {
  String pattern = 'dd MMM yyyy, hh:mm a',
  String locale = 'en',
}) => DateFormat(pattern, locale).format(value);

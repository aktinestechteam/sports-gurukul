import 'package:intl/intl.dart';

/// Formatting helpers for numbers, currency, bytes and text.
abstract final class Formatter {
  /// Formats [value] with grouped thousands and [decimalDigits] decimals.
  static String number(
    num value, {
    int decimalDigits = 0,
    String locale = 'en',
  }) {
    final format = NumberFormat.decimalPattern(locale)
      ..minimumFractionDigits = 0
      ..maximumFractionDigits = decimalDigits;
    return format.format(value);
  }

  /// Formats [value] as currency with [symbol].
  ///
  /// Trailing zero decimals are dropped, so `1000` becomes `₹1,000` while
  /// `9.99` keeps both decimals.
  static String currency(
    num value, {
    int decimalDigits = 2,
    String symbol = '₹',
    String locale = 'en',
  }) {
    final format = NumberFormat.currency(locale: locale, symbol: symbol)
      ..minimumFractionDigits = 0
      ..maximumFractionDigits = decimalDigits;
    return format.format(value);
  }

  /// Formats [value] compactly, e.g. `1.2K`.
  static String compactNumber(num value, {String locale = 'en'}) =>
      NumberFormat.compact(locale: locale).format(value);

  /// Formats [value] with a trailing percent sign.
  static String percent(
    num value, {
    int decimalDigits = 2,
    String locale = 'en',
  }) {
    final format = NumberFormat.decimalPattern(locale)
      ..minimumFractionDigits = 0
      ..maximumFractionDigits = decimalDigits;
    return '${format.format(value)}%';
  }

  /// Formats a byte count as a human-readable size, e.g. `1.5 MB`.
  static String bytes(int bytes, {String locale = 'en'}) {
    const units = ['B', 'KB', 'MB', 'GB', 'TB'];
    var value = bytes.toDouble();
    var unitIndex = 0;
    while (value >= 1024 && unitIndex < units.length - 1) {
      value /= 1024;
      unitIndex++;
    }
    final format = NumberFormat.decimalPattern(locale)
      ..minimumFractionDigits = 0
      ..maximumFractionDigits = unitIndex == 0 ? 0 : 1;
    return '${format.format(value)} ${units[unitIndex]}';
  }

  /// Formats a 10-digit [phone] as `+91 98765 43210`; returns it unchanged
  /// when it is not a 10-digit number.
  static String phoneMask(String phone, {String countryCode = '+91'}) {
    if (phone.length != 10 || int.tryParse(phone) == null) {
      return phone;
    }
    return '$countryCode ${phone.substring(0, 5)} ${phone.substring(5)}';
  }

  /// Masks [email] for display, e.g. `r****a@gmail.com`; returns it
  /// unchanged when it has no plausible local part.
  static String maskEmail(String email) {
    final at = email.indexOf('@');
    if (at <= 1) {
      return email;
    }
    final name = email.substring(0, at);
    final domain = email.substring(at);
    return '${name[0]}${'*' * (name.length - 2)}${name[name.length - 1]}'
        '$domain';
  }

  /// Formats [value] compactly, e.g. `2d 3h`, `45m`, `8s`.
  static String duration(Duration value) {
    if (value.inDays > 0) {
      final hours = value.inHours % 24;
      return hours == 0 ? '${value.inDays}d' : '${value.inDays}d ${hours}h';
    }
    if (value.inHours > 0) {
      final minutes = value.inMinutes % 60;
      return minutes == 0
          ? '${value.inHours}h'
          : '${value.inHours}h ${minutes}m';
    }
    if (value.inMinutes > 0) {
      final seconds = value.inSeconds % 60;
      return seconds == 0
          ? '${value.inMinutes}m'
          : '${value.inMinutes}m ${seconds}s';
    }
    return '${value.inSeconds}s';
  }

  /// Formats [value] as a clock string, e.g. `02:03:04`.
  static String durationClock(Duration value) {
    final hours = _twoDigits(value.inHours);
    final minutes = _twoDigits(value.inMinutes % 60);
    final seconds = _twoDigits(value.inSeconds % 60);
    return '$hours:$minutes:$seconds';
  }

  static String _twoDigits(int value) => value.toString().padLeft(2, '0');
}

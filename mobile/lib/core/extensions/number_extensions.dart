import 'dart:math' as math;

import 'package:sports_gurukul/core/utils/formatter.dart';

/// Convenience extensions on [num].
extension NumX on num {
  /// Whether this number is within [lower] and [upper], inclusive.
  bool isBetween(num lower, num upper) => this >= lower && this <= upper;

  /// Formats this number with [decimalDigits] decimals.
  String toFormatted({int decimalDigits = 0}) =>
      Formatter.number(this, decimalDigits: decimalDigits);

  /// Formats this number in a compact, human-readable form.
  String toCompact() => Formatter.compactNumber(this);
}

/// Convenience extensions on [int].
extension IntX on int {
  /// The ordinal representation of this number, e.g. `1st`, `2nd`, `3rd`.
  String ordinal() {
    final mod100 = this % 100;
    if (mod100 >= 11 && mod100 <= 13) {
      return '${this}th';
    }
    return switch (this % 10) {
      1 => '${this}st',
      2 => '${this}nd',
      3 => '${this}rd',
      _ => '${this}th',
    };
  }
}

/// Convenience extensions on [double].
extension DoubleX on double {
  /// Rounds this value to [decimals] decimal places.
  double roundToPrecision(int decimals) {
    final factor = math.pow(10, decimals).toDouble();
    return (this * factor).roundToDouble() / factor;
  }
}

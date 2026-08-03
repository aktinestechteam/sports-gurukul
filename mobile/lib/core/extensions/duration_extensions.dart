import 'package:sports_gurukul/core/utils/formatter.dart';

/// Convenience extensions on [Duration].
extension DurationX on Duration {
  /// Formats this duration as a clock string (`HH:mm:ss`).
  String toClockLabel() => Formatter.durationClock(this);

  /// Formats this duration as a compact human-readable label (`2d 3h`).
  String toShortLabel() => Formatter.duration(this);

  /// Clamps this duration to [lower] and [upper], inclusive.
  Duration clampTo(Duration lower, Duration upper) {
    if (this < lower) {
      return lower;
    }
    if (this > upper) {
      return upper;
    }
    return this;
  }

  /// Whether this duration is shorter than [other].
  bool isShorterThan(Duration other) => this < other;

  /// Whether this duration is longer than [other].
  bool isLongerThan(Duration other) => this > other;
}

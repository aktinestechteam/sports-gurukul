import 'package:flutter/animation.dart';

/// Animation durations and curves shared across the app.
///
/// UI code must use these tokens instead of inline durations and curves so
/// motion stays consistent and is tweakable from a single place.
abstract final class AnimationConstants {
  /// Snappy transitions for micro-interactions.
  static const Duration quick = Duration(milliseconds: 150);

  /// Standard duration for common transitions.
  static const Duration standard = Duration(milliseconds: 250);

  /// Slower duration for prominent or full-screen transitions.
  static const Duration slow = Duration(milliseconds: 400);

  /// Duration of page-level navigation transitions.
  static const Duration pageTransition = Duration(milliseconds: 300);

  /// Duration of opacity fades.
  static const Duration fade = Duration(milliseconds: 200);

  /// Default easing curve.
  static const Curve curveStandard = Curves.easeInOut;

  /// Easing for quick entrances.
  static const Curve curveFast = Curves.easeOut;

  /// Easing for decelerating entrances.
  static const Curve curveDecelerate = Curves.easeOutCubic;

  /// Emphasized curve recommended by Material 3.
  static const Curve curveEmphasized = Curves.easeInOutCubicEmphasized;
}

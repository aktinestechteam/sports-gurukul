import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/app_animation.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';

/// An animated gradient progress meter.
///
/// Fills from zero to [value] on mount and whenever the target changes, with
/// a rounded gradient track and a soft color-tinted glow on the fill head.
class GradientProgress extends StatelessWidget {
  const GradientProgress({
    required this.value,
    super.key,
    this.gradient = AppGradients.primary,
    this.height = 10,
    this.trackColor = AppColors.glassFill,
    this.duration = AppAnimation.card,
    this.shadows = const <BoxShadow>[],
    this.borderRadius = const BorderRadius.all(Radius.circular(AppRadius.pill)),
  });

  /// Fill amount, clamped to `0..1`.
  final double value;

  /// Gradient painted along the fill.
  final Gradient gradient;

  /// Track thickness in logical pixels.
  final double height;

  /// Unfilled track colour.
  final Color trackColor;

  /// How long the fill animation takes.
  final Duration duration;

  /// Layered soft shadows painted beneath the fill head.
  final List<BoxShadow> shadows;

  /// Corner radius of the track and fill.
  final BorderRadius borderRadius;

  @override
  Widget build(BuildContext context) {
    return TweenAnimationBuilder<double>(
      tween: Tween<double>(begin: 0, end: value.clamp(0, 1)),
      duration: duration,
      curve: Curves.easeOutCubic,
      builder: (context, progress, child) {
        return Container(
          height: height,
          decoration: BoxDecoration(
            color: trackColor,
            borderRadius: borderRadius,
          ),
          child: Align(
            alignment: Alignment.centerLeft,
            child: FractionallySizedBox(
              widthFactor: progress,
              child: Container(
                decoration: BoxDecoration(
                  borderRadius: borderRadius,
                  gradient: gradient,
                  boxShadow: shadows,
                ),
              ),
            ),
          ),
        );
      },
    );
  }
}

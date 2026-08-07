import 'dart:ui';

import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/app_shadow.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';

/// Premium smoked-glass surface (Apple Vision Pro / Windows 11 Mica).
///
/// A [BackdropFilter] blurs whatever sits behind the card ([blur]) while a
/// dark translucent [fill] (~20% opacity) keeps it dark and glassy rather than
/// opaque, so the background stays faintly visible through the smoke. A very
/// subtle white [highlight] hugs the top edge, the outer rim is a thin
/// semi-transparent white [borderGradient], and the card floats on the
/// softened [shadows]. Content stays opaque for readability.
class GlassCard extends StatelessWidget {
  const GlassCard({
    required this.child,
    super.key,
    this.padding = const EdgeInsets.all(AppSpacing.xl),
    this.borderRadius = const BorderRadius.all(
      Radius.circular(AppRadius.xlarge),
    ),
    this.fill = AppColors.glassFillDark, // ~20% dark smoked glass
    this.highlight = AppGradients.glassHighlight,
    this.borderGradient = AppGradients.glassBorder,
    this.borderColor,
    this.borderWidth = 1.2,
    this.blur = 28,
    this.shadows = AppShadow.glass,
  });

  final Widget child;
  final EdgeInsetsGeometry padding;
  final BorderRadius borderRadius;
  final Color fill;

  /// Very subtle white sheen near the top of the card; adds a faint glass rim
  /// without brightening the surface.
  final Gradient highlight;
  final Gradient borderGradient;
  final Color? borderColor;
  final double borderWidth;
  final double blur;
  final List<BoxShadow> shadows;

  @override
  Widget build(BuildContext context) {
    final innerRadius = _deflate(borderRadius, borderWidth);

    final gradient = borderColor != null
        ? LinearGradient(
            colors: [borderColor!, borderColor!],
          )
        : borderGradient;

    return DecoratedBox(
      decoration: BoxDecoration(
        borderRadius: borderRadius,
        gradient: gradient,
        boxShadow: shadows,
      ),
      child: Padding(
        padding: EdgeInsets.all(borderWidth),
        child: ClipRRect(
          borderRadius: innerRadius,
          child: BackdropFilter(
            filter: ImageFilter.blur(
              sigmaX: blur,
              sigmaY: blur,
            ),
            child: Stack(
              children: <Widget>[
                // Dark translucent smoked-glass fill with a thin white edge.
                Positioned.fill(
                  child: DecoratedBox(
                    decoration: BoxDecoration(
                      borderRadius: innerRadius,
                      color: fill,
                      border: Border.all(
                        color: Colors.white.withValues(alpha: 0.14),
                      ),
                    ),
                  ),
                ),
                // Very subtle white sheen hugging the top edge of the card.
                Positioned.fill(
                  child: DecoratedBox(
                    decoration: BoxDecoration(
                      borderRadius: innerRadius,
                      gradient: highlight,
                    ),
                  ),
                ),
                Padding(
                  padding: padding,
                  child: child,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  static BorderRadius _deflate(BorderRadius radius, double amount) {
    return BorderRadius.only(
      topLeft: _shrink(radius.topLeft, amount),
      topRight: _shrink(radius.topRight, amount),
      bottomLeft: _shrink(radius.bottomLeft, amount),
      bottomRight: _shrink(radius.bottomRight, amount),
    );
  }

  static Radius _shrink(Radius radius, double amount) {
    return Radius.elliptical(
      (radius.x - amount).clamp(0.0, double.infinity),
      (radius.y - amount).clamp(0.0, double.infinity),
    );
  }
}

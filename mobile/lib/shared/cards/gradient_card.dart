import 'dart:ui';

import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/shared/animations/spring_press.dart';

/// A filled gradient action surface with a glass icon chip.
///
/// Composes a [LinearGradient] fill, layered glow shadows and a springy press
/// ([SpringPress]) into a single tap target for dashboard quick actions.
class GradientCard extends StatelessWidget {
  const GradientCard({
    required this.title,
    required this.icon,
    required this.onPressed,
    super.key,
    this.subtitle,
    this.gradient = const LinearGradient(
      colors: <Color>[AppColors.primary400, AppColors.primary600],
    ),
    this.shadows = const <BoxShadow>[],
    this.trailing = const Icon(Icons.arrow_forward_rounded),
    this.borderRadius = const BorderRadius.all(
      Radius.circular(AppRadius.xlarge),
    ),
  });

  /// Primary label.
  final String title;

  /// Optional supporting label rendered under [title].
  final String? subtitle;

  /// Leading icon shown inside the frosted chip.
  final IconData icon;

  /// Gradient painted across the card body.
  final Gradient gradient;

  /// Layered soft shadows (typically a color-tinted glow).
  final List<BoxShadow> shadows;

  /// Trailing affordance (defaults to a forward chevron).
  final Widget trailing;

  /// Corner radius of the card body.
  final BorderRadius borderRadius;

  /// Tap callback.
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    const textColor = AppColors.surface;
    final titleStyle = Theme.of(context).textTheme.titleMedium?.copyWith(
      color: textColor,
      fontWeight: FontWeight.w700,
    );

    return SpringPress(
      onPressed: onPressed,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        decoration: BoxDecoration(
          borderRadius: borderRadius,
          gradient: gradient,
          boxShadow: shadows,
        ),
        padding: const EdgeInsets.all(AppSpacing.lg),
        child: Row(
          children: <Widget>[
            ClipOval(
              child: BackdropFilter(
                filter: ImageFilter.blur(sigmaX: 8, sigmaY: 8),
                child: Container(
                  width: 46,
                  height: 46,
                  decoration: const BoxDecoration(
                    shape: BoxShape.circle,
                    color: Color(0x29FFFFFF),
                  ),
                  child: Icon(icon, color: textColor, size: 22),
                ),
              ),
            ),
            const SizedBox(width: AppSpacing.md),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: <Widget>[
                  Text(
                    title,
                    style: titleStyle,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  if (subtitle != null) ...<Widget>[
                    const SizedBox(height: 2),
                    Text(
                      subtitle!,
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: textColor.withValues(alpha: 0.85),
                      ),
                    ),
                  ],
                ],
              ),
            ),
            const SizedBox(width: AppSpacing.sm),
            DefaultTextStyle(
              style: TextStyle(color: textColor.withValues(alpha: 0.9)),
              child: IconTheme(
                data: IconThemeData(color: textColor.withValues(alpha: 0.9)),
                child: trailing,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

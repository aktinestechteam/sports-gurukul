import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/shared/animations/spring_press.dart';

/// A gradient-filled primary button with a color-tinted glow.
///
/// Combines [SpringPress] feedback with a [LinearGradient] fill and layered
/// soft shadow for the "gradient touch" pattern. Shows a spinner while
/// [loading] is true and disables itself when [onPressed] is null.
class GradientButton extends StatelessWidget {
  const GradientButton({
    required this.label,
    super.key,
    this.onPressed,
    this.icon,
    this.loading = false,
    this.gradient = AppGradients.primary,
    this.shadows = const <BoxShadow>[],
    this.height = 56,
    this.borderRadius = const BorderRadius.all(
      Radius.circular(AppRadius.xlarge),
    ),
  });

  /// Button label.
  final String label;

  /// Tap callback; null renders a disabled button.
  final VoidCallback? onPressed;

  /// Optional leading icon.
  final IconData? icon;

  /// When true a spinner replaces [icon].
  final bool loading;

  /// Gradient painted across the button body.
  final Gradient gradient;

  /// Layered soft shadows (typically a color-tinted glow).
  final List<BoxShadow> shadows;

  /// Button body height.
  final double height;

  /// Corner radius of the button body.
  final BorderRadius borderRadius;

  bool get _enabled => !loading && onPressed != null;

  @override
  Widget build(BuildContext context) {
    const foreground = AppColors.surface;
    return SpringPress(
      enabled: _enabled,
      onPressed: onPressed,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        height: height,
        decoration: BoxDecoration(
          borderRadius: borderRadius,
          gradient: _enabled ? gradient : _disabledGradient,
          boxShadow: _enabled ? shadows : const <BoxShadow>[],
        ),
        child: Center(
          child: loading
              ? const SizedBox(
                  width: 22,
                  height: 22,
                  child: CircularProgressIndicator(
                    strokeWidth: 2.4,
                    color: foreground,
                  ),
                )
              : Row(
                  mainAxisSize: MainAxisSize.min,
                  children: <Widget>[
                    if (icon != null) ...<Widget>[
                      Icon(icon, color: foreground, size: 20),
                      const SizedBox(width: AppSpacing.sm),
                    ],
                    Text(
                      label,
                      style: Theme.of(context).textTheme.titleSmall?.copyWith(
                        color: foreground,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
        ),
      ),
    );
  }

  static const LinearGradient _disabledGradient = LinearGradient(
    colors: <Color>[AppColors.grey600, AppColors.grey700],
  );
}

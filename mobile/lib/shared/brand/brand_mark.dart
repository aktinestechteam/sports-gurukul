import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/app_animation.dart';
import 'package:sports_gurukul/app/theme/app_shadow.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/shared/animations/entrance.dart';

/// Floating brand mark shared by the authentication screens.
///
/// Renders the Sports Gurukul mark on a blue-purple gradient tile with the
/// primary glow. It is designed to straddle the top border of an auth card,
/// half above and half below, so pages must place it inside a
/// [Stack] with `clipBehavior: Clip.none`.
class BrandMark extends StatelessWidget {
  const BrandMark({super.key});

  /// Edge-to-edge layout size of the mark (before any entrance scale).
  static const double size = 88;

  @override
  Widget build(BuildContext context) {
    return Entrance(
      delay: const Duration(milliseconds: 60),
      duration: AppAnimation.entrance,
      child: Hero(
        tag: 'auth-brand-mark',
        child: Container(
          width: size,
          height: size,
          decoration: BoxDecoration(
            gradient: AppGradients.bluePurple,
            borderRadius: BorderRadius.circular(AppRadius.extraLarge),
            boxShadow: AppShadow.glowPrimary,
          ),
          child: const Icon(
            Icons.sports_soccer,
            color: AppColors.surface,
            size: 46,
          ),
        ),
      ),
    );
  }
}

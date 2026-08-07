import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';

/// Centralized gradient tokens for the Sports Gurukul design system.
///
/// Every gradient used by the application must be sourced from this class;
/// gradients never hardcode raw colors, they compose [AppColors] tokens.
abstract final class AppGradients {
  /// Brand primary: sports blue -> deep blue.
  static const LinearGradient primary = LinearGradient(
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    colors: <Color>[AppColors.primary400, AppColors.primary600],
  );

  /// Ocean: brand blue -> violet.
  static const LinearGradient ocean = LinearGradient(
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    colors: <Color>[AppColors.primary500, AppColors.violet500],
  );

  /// Blue -> purple brand gradient from the approved login mockup (logo,
  /// welcome text, primary button).
  static const LinearGradient bluePurple = LinearGradient(
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    colors: <Color>[AppColors.blue600, AppColors.violet600],
  );

  /// Blue -> purple brand gradient running left-to-right (primary button),
  /// matching the horizontal gradient measured on the approved mockup.
  static const LinearGradient bluePurpleHorizontal = LinearGradient(
    colors: <Color>[AppColors.blue600, AppColors.violet600],
  );

  /// Aurora: violet -> blue -> cyan (hero surfaces).
  static const LinearGradient aurora = LinearGradient(
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    colors: <Color>[
      AppColors.violet500,
      AppColors.primary500,
      AppColors.cyan400,
    ],
  );

  /// Sunset: orange -> pink (energy actions).
  static const LinearGradient sunset = LinearGradient(
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    colors: <Color>[AppColors.accent, AppColors.pink500],
  );

  /// Emerald: secondary -> success (recovery/fitness actions).
  static const LinearGradient emerald = LinearGradient(
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    colors: <Color>[AppColors.secondary, AppColors.success],
  );

  /// Full-screen vibrant dark base for the dashboard canvas.
  static const LinearGradient background = LinearGradient(
    begin: Alignment.topCenter,
    end: Alignment.bottomCenter,
    colors: <Color>[Color(0xFF161D45), AppColors.inkDeep],
  );

  /// Soft radial blob (violet) painted behind glass surfaces.
  static const RadialGradient blobViolet = RadialGradient(
    colors: <Color>[AppColors.violet700, Colors.transparent],
  );

  /// Soft radial blob (cyan) painted behind glass surfaces.
  static const RadialGradient blobCyan = RadialGradient(
    colors: <Color>[AppColors.cyan500, Colors.transparent],
  );

  /// Soft radial blob (pink) painted behind glass surfaces.
  static const RadialGradient blobPink = RadialGradient(
    colors: <Color>[AppColors.pink500, Colors.transparent],
  );

  /// Transparent-to-white glass border highlight.
  static const LinearGradient glassBorder = LinearGradient(
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    colors: <Color>[AppColors.glassBorderHi, AppColors.glassBorderLo],
  );

  /// Very subtle white sheen near the top of smoked-glass surfaces, fading
  /// out before the vertical middle.
  ///
  /// Kept faint (low opacity, top-anchored) so the card reads as dark smoked
  /// glass instead of a bright frosted panel; it sits above the translucent
  /// [AppColors.glassFillDark] and the blurred background stays visible.
  static const LinearGradient glassHighlight = LinearGradient(
    begin: Alignment.topCenter,
    end: Alignment.bottomCenter,
    colors: <Color>[AppColors.glassFillHi, Colors.transparent],
    stops: <double>[0, 0.45],
  );

  /// Dark readability overlay painted over full-bleed photography.
  ///
  /// Deepens toward the bottom (where the auth card sits) so foreground
  /// text keeps sufficient contrast while the photo stays visible (~45%
  /// black, per the approved login mockup).
  static const LinearGradient photoOverlay = LinearGradient(
    begin: Alignment.topCenter,
    end: Alignment.bottomCenter,
    colors: <Color>[Color(0x66000000), Color(0x99000000)],
  );
}

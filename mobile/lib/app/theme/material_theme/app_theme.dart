import 'package:flutter/material.dart';

import '../app_elevation.dart';
import '../colors/app_colors.dart';
import '../radius/app_radius.dart';
import '../typography/app_typography.dart';

/// Builds the Material 3 theme pair for the Sports Gurukul application.
///
/// Both light and dark themes derive from a single seed color
/// ([AppColors.primary500]) through `ColorScheme.fromSeed`. The dark theme
/// uses the Material 3 dark color scheme; colors are never inverted manually.
///
/// All visual values are sourced from the design tokens in [AppColors],
/// [AppRadius], [AppElevation] and [AppTypography]. No widget should define
/// its own colors, radii, elevations or spacing.
abstract final class AppTheme {
  static ThemeData get light => _build(Brightness.light);

  static ThemeData get dark => _build(Brightness.dark);

  static ThemeData _build(Brightness brightness) {
    final ColorScheme colorScheme = ColorScheme.fromSeed(
      seedColor: AppColors.primary500,
      brightness: brightness,
    );

    return ThemeData(
      useMaterial3: true,
      brightness: brightness,
      colorScheme: colorScheme,
      fontFamily: AppTypography.brandFontFamily,
      fontFamilyFallback: AppTypography.brandFontFamilyFallback,
      scaffoldBackgroundColor: brightness == Brightness.light
          ? AppColors.surface
          : AppColors.surfaceDark,
      appBarTheme: AppBarTheme(
        backgroundColor: colorScheme.surface,
        foregroundColor: colorScheme.onSurface,
        elevation: AppElevation.none,
        centerTitle: false,
      ),
      cardTheme: CardThemeData(
        elevation: AppElevation.card,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.medium),
        ),
      ),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          elevation: AppElevation.none,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppRadius.medium),
          ),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.medium),
        ),
      ),
      snackBarTheme: SnackBarThemeData(
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.medium),
        ),
      ),
    );
  }
}

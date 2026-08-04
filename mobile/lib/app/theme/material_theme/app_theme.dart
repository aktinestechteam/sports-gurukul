import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/app_elevation.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/typography/app_typography.dart';

/// Builds the Material 3 theme pair for the Sports Gurukul application.
abstract final class AppTheme {
  static ThemeData get light => _build(Brightness.light);

  static ThemeData get dark => _build(Brightness.dark);

  static ThemeData _build(Brightness brightness) {
    final isDark = brightness == Brightness.dark;

    final colorScheme = ColorScheme.fromSeed(
      seedColor: AppColors.primary500,
      brightness: brightness,
    );

    return ThemeData(
      useMaterial3: true,
      brightness: brightness,
      colorScheme: colorScheme,
      fontFamily: AppTypography.brandFontFamily,
      fontFamilyFallback: AppTypography.brandFontFamilyFallback,
      scaffoldBackgroundColor:
          isDark ? AppColors.surfaceDark : AppColors.surface,

      // ─── AppBar Theme ───────────────────────────────────────────────
      appBarTheme: AppBarTheme(
        backgroundColor: Colors.transparent, // Translucent for background glows
        foregroundColor: colorScheme.onSurface,
        elevation: AppElevation.none,
        centerTitle: false,
      ),

      // ─── Card Theme (Glassmorphism look) ───────────────────────────
      cardTheme: CardThemeData(
        elevation: AppElevation.card,
        color: isDark
            ? colorScheme.surfaceContainerHighest.withValues(alpha: 0.3)
            : colorScheme.surfaceContainerHighest.withValues(alpha: 0.6),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.xlarge),
          side: BorderSide(
            color: colorScheme.outlineVariant.withValues(alpha: 0.3),
            width: 1.5,
          ),
        ),
      ),

      // ─── Input Decoration Theme (Login Forms, Search Bars) ──────────
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: isDark
            ? colorScheme.surfaceContainer.withValues(alpha: 0.4)
            : colorScheme.surfaceContainer.withValues(alpha: 0.8),
        prefixIconColor: colorScheme.primary,
        hintStyle: TextStyle(
          color: colorScheme.onSurfaceVariant.withValues(alpha: 0.6),
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.xlarge),
          borderSide: BorderSide(
            color: colorScheme.outline.withValues(alpha: 0.2),
          ),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.xlarge),
          borderSide: BorderSide(
            color: colorScheme.outline.withValues(alpha: 0.2),
          ),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(AppRadius.xlarge),
          borderSide: BorderSide(
            color: colorScheme.primary,
            width: 2,
          ),
        ),
      ),

      // ─── Primary Button Theme ──────────────────────────────────────
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          elevation: AppElevation.none,
          backgroundColor: colorScheme.primary,
          foregroundColor: colorScheme.onPrimary,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(AppRadius.xlarge),
          ),
          padding: const EdgeInsets.symmetric(vertical: 16, horizontal: 24),
          textStyle: const TextStyle(
            fontWeight: FontWeight.bold,
            letterSpacing: 1.2,
          ),
        ),
      ),

      // ─── SnackBar, Dialog, BottomSheet Themes ──────────────────────
      snackBarTheme: SnackBarThemeData(
        behavior: SnackBarBehavior.floating,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.xlarge),
        ),
      ),
      dialogTheme: DialogThemeData(
        backgroundColor: isDark
            ? AppColors.surfaceDark.withValues(alpha: 0.9)
            : AppColors.surface.withValues(alpha: 0.9),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.xlarge),
        ),
      ),
      bottomSheetTheme: BottomSheetThemeData(
        backgroundColor: isDark
            ? AppColors.surfaceDark.withValues(alpha: 0.9)
            : AppColors.surface.withValues(alpha: 0.9),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(AppRadius.xlarge),
        ),
      ),
    );
  }
}

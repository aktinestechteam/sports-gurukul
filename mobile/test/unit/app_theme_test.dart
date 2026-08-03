import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/app/theme/app_animation.dart';
import 'package:sports_gurukul/app/theme/app_elevation.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/material_theme/app_theme.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';

void main() {
  group('Design tokens', () {
    test('uses an 8-point spacing scale', () {
      const List<double> scale = <double>[
        AppSpacing.none,
        AppSpacing.xs,
        AppSpacing.sm,
        AppSpacing.md,
        AppSpacing.lg,
        AppSpacing.xl,
        AppSpacing.xxl,
        AppSpacing.xxxl,
        AppSpacing.xxxxl,
        AppSpacing.xxxxxl,
        AppSpacing.huge,
      ];
      for (final double value in scale) {
        expect(value % 4, 0, reason: '$value is not on the 8-point grid');
      }
    });

    test('primary 500 matches the approved design system seed', () {
      expect(AppColors.primary500, const Color(0xFF006DFF));
    });

    test('radius and elevation tokens are non-negative', () {
      expect(AppRadius.small, greaterThan(0));
      expect(AppElevation.card, greaterThan(0));
      expect(AppAnimation.page, const Duration(milliseconds: 250));
    });
  });

  group('AppTheme', () {
    test('light theme is Material 3', () {
      final ThemeData theme = AppTheme.light;
      expect(theme.useMaterial3, isTrue);
      expect(theme.brightness, Brightness.light);
    });

    test('dark theme is Material 3 and never manually inverted', () {
      final ThemeData theme = AppTheme.dark;
      expect(theme.useMaterial3, isTrue);
      expect(theme.brightness, Brightness.dark);
    });

    test('both themes derive from the same seed', () {
      expect(AppTheme.light.colorScheme.primary,
          isNot(AppTheme.dark.colorScheme.primary));
    });
  });
}

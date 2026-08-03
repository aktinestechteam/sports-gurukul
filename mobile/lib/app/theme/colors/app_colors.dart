import 'package:flutter/material.dart';

/// Centralized color tokens for the Sports Gurukul design system.
///
/// Every color used by the application must be sourced from this class.
/// Never hardcode colors inside widgets.
///
/// The primary scale values are defined in the Approved mobile design system
/// (`docs/mobile/01-Design-System.md`, §3 Color System). Semantic and neutral
/// values follow the documented color philosophy and the Material 3 palette.
///
/// Design system reference: docs/mobile/01-Design-System.md
abstract final class AppColors {
  /// Sports Blue - Primary scale.
  static const Color primary50 = Color(0xFFE8F2FF);
  static const Color primary100 = Color(0xFFCCE4FF);
  static const Color primary200 = Color(0xFF99C9FF);
  static const Color primary300 = Color(0xFF66AEFF);
  static const Color primary400 = Color(0xFF3393FF);
  static const Color primary500 = Color(0xFF006DFF);
  static const Color primary600 = Color(0xFF0058CC);
  static const Color primary700 = Color(0xFF004399);
  static const Color primary800 = Color(0xFF003066);
  static const Color primary900 = Color(0xFF001C33);

  /// Emerald - Secondary color.
  static const Color secondary = Color(0xFF10B981);

  /// Orange - Accent color.
  static const Color accent = Color(0xFFFF8A00);

  /// Success.
  static const Color success = Color(0xFF16A34A);

  /// Warning.
  static const Color warning = Color(0xFFF59E0B);

  /// Danger.
  static const Color danger = Color(0xFFDC2626);

  /// Information.
  static const Color information = Color(0xFF3B82F6);

  /// Neutral grays.
  static const Color grey50 = Color(0xFFF9FAFB);
  static const Color grey100 = Color(0xFFF3F4F6);
  static const Color grey200 = Color(0xFFE5E7EB);
  static const Color grey300 = Color(0xFFD1D5DB);
  static const Color grey400 = Color(0xFF9CA3AF);
  static const Color grey500 = Color(0xFF6B7280);
  static const Color grey600 = Color(0xFF4B5563);
  static const Color grey700 = Color(0xFF374151);
  static const Color grey800 = Color(0xFF1F2937);
  static const Color grey900 = Color(0xFF111827);

  /// Neutral surface colors used on light backgrounds.
  static const Color surface = Color(0xFFFFFFFF);
  static const Color surfaceVariant = Color(0xFFF1F5F9);
  static const Color onSurface = Color(0xFF111827);

  /// Neutral surface colors used on dark backgrounds.
  static const Color surfaceDark = Color(0xFF0F172A);
  static const Color surfaceDarkVariant = Color(0xFF1E293B);
  static const Color onSurfaceDark = Color(0xFFF8FAFC);
}

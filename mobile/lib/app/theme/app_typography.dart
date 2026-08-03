import 'package:flutter/material.dart';

/// Centralized typography tokens for the Sports Gurukul design system.
///
/// The brand font family is Inter, with Roboto as the platform fallback.
/// The Inter font asset will be bundled when brand assets land (P-sprint
/// after brand asset delivery); until then the platform default is used.
///
/// Source: docs/mobile/01-Design-System.md (§4 Typography).
abstract final class AppTypography {
  static const String brandFontFamily = 'Inter';
  static const List<String> brandFontFamilyFallback = <String>['Roboto'];

  // Display.
  static const double displayLarge = 48;
  static const double displayMedium = 40;
  static const double displaySmall = 36;

  // Headings.
  static const double headingXl = 32;
  static const double headingLg = 28;
  static const double headingMd = 24;
  static const double headingSm = 20;

  // Body. The design system mandates a minimum body size of 16sp.
  static const double bodyLarge = 18;
  static const double bodyMedium = 16;
  static const double bodySmall = 14;
  static const double caption = 12;

  // Weights.
  static const FontWeight regular = FontWeight.w400;
  static const FontWeight medium = FontWeight.w500;
  static const FontWeight semiBold = FontWeight.w600;
  static const FontWeight bold = FontWeight.w700;
}

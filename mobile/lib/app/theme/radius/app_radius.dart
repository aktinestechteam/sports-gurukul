/// Centralized border radius tokens for the Sports Gurukul design system.
///
/// Never hardcode radius values inside widgets; always reference [AppRadius].
abstract final class AppRadius {
  /// Small radius (cards, buttons).
  static const double small = 8;

  /// Medium radius (inputs, dialogs).
  static const double medium = 12;

  /// Compact controls (buttons, text fields).
  static const double control = 16;

  /// Text fields and action buttons (approved login mockup).
  static const double input = 18;

  /// Large radius (cards, sheets).
  static const double large = 20;

  /// Extra large radius (cards, modals, inputs, glass surfaces).
  static const double xlarge = 24;

  /// Extra large radius (illustrations, hero surfaces).
  static const double extraLarge = 28;

  /// Pill radius (chips, avatars).
  static const double pill = 999;
}

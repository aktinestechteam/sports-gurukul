/// Centralized motion/duration tokens for the Sports Gurukul design system.
///
/// Source: docs/mobile/01-Design-System.md (§6 Motion).
abstract final class AppAnimation {
  /// Page transitions.
  static const Duration page = Duration(milliseconds: 250);

  /// Cards and surfaces.
  static const Duration card = Duration(milliseconds: 200);

  /// Hero animations.
  static const Duration hero = Duration(milliseconds: 350);

  /// Button press feedback.
  static const Duration buttonPress = Duration(milliseconds: 100);

  /// Bottom sheets.
  static const Duration bottomSheet = Duration(milliseconds: 300);
}

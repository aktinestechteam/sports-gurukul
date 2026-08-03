/// Shell route contract for role-based navigation.
///
/// The persistent bottom navigation shell (Dashboard, Training, Attendance,
/// Messages, Profile) and role-specific shells are delivered in later
/// sprints. This file locks the shape in for Sprint 0.
///
/// Reference: docs/mobile/09-Implementation/07-Navigation.md
abstract final class ShellRoutes {
  static const List<String> _tabs = <String>[];

  /// Reserved for the future persistent shell tab set.
  static List<String> get tabs => List<String>.unmodifiable(_tabs);
}

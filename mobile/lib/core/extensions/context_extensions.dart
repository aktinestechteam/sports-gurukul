import 'package:flutter/material.dart';

/// Convenience extensions on [BuildContext].
extension BuildContextX on BuildContext {
  /// The ambient [ThemeData].
  ThemeData get theme => Theme.of(this);

  /// The ambient [TextTheme].
  TextTheme get textTheme => Theme.of(this).textTheme;

  /// The ambient [ColorScheme].
  ColorScheme get colorScheme => Theme.of(this).colorScheme;

  /// The ambient [MediaQueryData].
  MediaQueryData get mediaQuery => MediaQuery.of(this);

  /// The logical size of the enclosing screen.
  Size get screenSize => MediaQuery.sizeOf(this);

  /// The logical width of the enclosing screen.
  double get screenWidth => MediaQuery.sizeOf(this).width;

  /// The logical height of the enclosing screen.
  double get screenHeight => MediaQuery.sizeOf(this).height;

  /// Whether the active theme is dark.
  bool get isDarkMode => Theme.of(this).brightness == Brightness.dark;

  /// The active locale, when one has been resolved.
  Locale? get locale => Localizations.maybeLocaleOf(this);

  /// Shows a [SnackBar] with [message].
  void showSnackBar(String message, {bool clearPrevious = false}) {
    final messenger = ScaffoldMessenger.of(this);
    if (clearPrevious) {
      messenger.clearSnackBars();
    }
    messenger.showSnackBar(SnackBar(content: Text(message)));
  }

  /// Pops the current route off the navigator.
  void pop([Object? result]) => Navigator.of(this).pop(result);
}

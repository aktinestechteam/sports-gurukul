import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

/// Global [ThemeMode] state.
///
/// Defaults to the system setting. The setting is persisted by the
/// settings feature in a later sprint; for now it is application session
/// scoped only.
final themeModeProvider = NotifierProvider<ThemeModeNotifier, ThemeMode>(
  ThemeModeNotifier.new,
);

class ThemeModeNotifier extends Notifier<ThemeMode> {
  @override
  ThemeMode build() => ThemeMode.system;

  /// Updates the active theme mode.
  void setMode(ThemeMode mode) => state = mode;
}

import 'package:flutter/foundation.dart';

/// Platform classification helpers.
///
/// Uses `defaultTargetPlatform` (and [kIsWeb]) so these getters are safe on
/// every target including web, and can be overridden in tests via
/// `debugDefaultTargetPlatformOverride`.
abstract final class PlatformUtils {
  /// Whether the app runs on Android.
  static bool get isAndroid => defaultTargetPlatform == TargetPlatform.android;

  /// Whether the app runs on iOS.
  static bool get isIOS => defaultTargetPlatform == TargetPlatform.iOS;

  /// Whether the app runs in a browser.
  static bool get isWeb => kIsWeb;

  /// Whether the app runs on Linux.
  static bool get isLinux => defaultTargetPlatform == TargetPlatform.linux;

  /// Whether the app runs on macOS.
  static bool get isMacOS => defaultTargetPlatform == TargetPlatform.macOS;

  /// Whether the app runs on Windows.
  static bool get isWindows => defaultTargetPlatform == TargetPlatform.windows;

  /// Whether the app runs on Fuchsia.
  static bool get isFuchsia => defaultTargetPlatform == TargetPlatform.fuchsia;

  /// Whether the app runs on a phone or tablet form factor.
  static bool get isMobile => isAndroid || isIOS;

  /// Whether the app runs on a desktop form factor.
  static bool get isDesktop => isLinux || isMacOS || isWindows;

  /// Human-readable name of the current platform.
  static String get osName {
    if (isWeb) {
      return 'web';
    }
    return switch (defaultTargetPlatform) {
      TargetPlatform.android => 'android',
      TargetPlatform.iOS => 'ios',
      TargetPlatform.linux => 'linux',
      TargetPlatform.macOS => 'macos',
      TargetPlatform.windows => 'windows',
      TargetPlatform.fuchsia => 'fuchsia',
    };
  }
}

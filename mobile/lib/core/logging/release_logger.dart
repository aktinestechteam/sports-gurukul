import 'package:logger/logger.dart' as pkg;

import 'package:sports_gurukul/core/logging/log_level.dart';
import 'package:sports_gurukul/core/logging/logger.dart';

/// Production-safe [Logger] used in release builds.
///
/// Filters out verbose, debug and info messages so sensitive or noisy data
/// never reaches production logs. Only warnings, errors and fatal messages
/// are emitted, through a minimal printer with no colors.
class ReleaseLogger extends Logger {
  ReleaseLogger({pkg.Logger? logger})
    : _logger =
          logger ??
          pkg.Logger(
            printer: pkg.SimplePrinter(colors: false),
            level: pkg.Level.warning,
          );

  /// Levels below this are suppressed in release builds.
  static const LogLevel _minimumLevel = LogLevel.warning;

  final pkg.Logger _logger;

  @override
  bool isEnabled(LogLevel level) => level.isAtLeast(_minimumLevel);

  @override
  void log(
    LogLevel level,
    Object? message, {
    Object? error,
    StackTrace? stackTrace,
  }) {
    if (!isEnabled(level)) {
      return;
    }
    switch (level) {
      case LogLevel.warning:
        _logger.w(message);
      case LogLevel.error:
        _logger.e(message, error: error, stackTrace: stackTrace);
      case LogLevel.fatal:
        _logger.e(message, error: error, stackTrace: stackTrace);
      case LogLevel.trace:
      case LogLevel.debug:
      case LogLevel.info:
        break;
    }
  }
}

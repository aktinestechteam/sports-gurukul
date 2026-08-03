import 'package:logger/logger.dart' as pkg;

import 'package:sports_gurukul/core/logging/log_level.dart';
import 'package:sports_gurukul/core/logging/logger.dart';

/// Verbose [Logger] used in debug builds.
///
/// Wraps `package:logger` with a readable, color-free pretty printer and
/// enables every level so no diagnostics are lost during development.
class DebugLogger extends Logger {
  DebugLogger({pkg.Logger? logger})
    : _logger =
          logger ??
          pkg.Logger(
            printer: pkg.PrettyPrinter(methodCount: 0, colors: false),
            level: pkg.Level.debug,
          );

  final pkg.Logger _logger;

  @override
  bool isEnabled(LogLevel level) => true;

  @override
  void log(
    LogLevel level,
    Object? message, {
    Object? error,
    StackTrace? stackTrace,
  }) {
    switch (level) {
      case LogLevel.trace:
        _logger.t(message);
      case LogLevel.debug:
        _logger.d(message);
      case LogLevel.info:
        _logger.i(message);
      case LogLevel.warning:
        _logger.w(message);
      case LogLevel.error:
        _logger.e(message, error: error, stackTrace: stackTrace);
      case LogLevel.fatal:
        _logger.e(message, error: error, stackTrace: stackTrace);
    }
  }
}

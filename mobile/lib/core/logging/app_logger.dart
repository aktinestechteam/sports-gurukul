import 'package:sports_gurukul/core/config/app_config.dart';
import 'package:sports_gurukul/core/logging/debug_logger.dart';
import 'package:sports_gurukul/core/logging/logger.dart';
import 'package:sports_gurukul/core/logging/release_logger.dart';

/// Application-wide logging facade.
///
/// Every log statement in the app goes through this facade; `print()` is
/// banned by linting. It delegates to a [Logger] implementation selected
/// automatically for the current build (verbose in debug, filtered in
/// release) and swappable at runtime via [configure] (e.g. for tests or an
/// injected remote sink).
///
/// Sensitive data (tokens, passwords, PII) must never be logged.
abstract final class AppLogger {
  static Logger _logger = AppConfig.isDebugMode
      ? DebugLogger()
      : ReleaseLogger();

  /// Replaces the active [Logger] implementation.
  // ignore: use_setters_to_change_properties
  static void configure(Logger logger) {
    _logger = logger;
  }

  /// Logs a trace message.
  static void t(Object? message) => _logger.trace(message);

  /// Logs a debug message.
  static void d(Object? message) => _logger.debug(message);

  /// Logs an info message.
  static void i(Object? message) => _logger.info(message);

  /// Logs a warning.
  static void w(Object? message) => _logger.warning(message);

  /// Logs an error with an optional error object and stack trace.
  static void e(Object? message, [Object? error, StackTrace? stackTrace]) =>
      _logger.error(message, error, stackTrace);

  /// Logs a fatal error with an optional error object and stack trace.
  static void f(Object? message, [Object? error, StackTrace? stackTrace]) =>
      _logger.fatal(message, error, stackTrace);
}

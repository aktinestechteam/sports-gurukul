import 'package:logger/logger.dart';

/// Application-wide logging facade.
///
/// Wraps `package:logger` so call sites never depend on the concrete
/// implementation and configuration lives in one place. `print()` is banned
/// by linting; every log statement goes through this facade.
///
/// Debug-level messages are intended for development only. Sensitive data
/// (tokens, passwords, PII) must never be logged (see Coding Standards).
abstract final class AppLogger {
  static final Logger _logger = Logger(
    printer: PrettyPrinter(
      methodCount: 0,
      colors: false,
    ),
    level: Level.debug,
  );

  /// Logs a trace message.
  static void t(Object? message) => _logger.t(message);

  /// Logs a debug message.
  static void d(Object? message) => _logger.d(message);

  /// Logs an info message.
  static void i(Object? message) => _logger.i(message);

  /// Logs a warning.
  static void w(Object? message) => _logger.w(message);

  /// Logs an error with an optional error object and stack trace.
  static void e(Object? message, [Object? error, StackTrace? stackTrace]) =>
      _logger.e(message, error: error, stackTrace: stackTrace);
}

import 'package:sports_gurukul/core/logging/log_level.dart';

/// Contract for application logging sinks.
///
/// Core and feature code should depend on this interface (or the
/// `AppLogger` facade) and never call `print()`. Implementations decide
/// which levels are enabled and how messages are formatted and emitted.
abstract class Logger {
  const Logger();

  /// Whether messages at [level] should be emitted.
  ///
  /// Implementations may filter based on the compiled build (e.g. release
  /// builds disable verbose levels).
  bool isEnabled(LogLevel level);

  /// Emits [message] at [level], optionally with an [error] and
  /// [stackTrace].
  void log(
    LogLevel level,
    Object? message, {
    Object? error,
    StackTrace? stackTrace,
  });

  /// Logs at [LogLevel.trace].
  void trace(Object? message) => log(LogLevel.trace, message);

  /// Logs at [LogLevel.debug].
  void debug(Object? message) => log(LogLevel.debug, message);

  /// Logs at [LogLevel.info].
  void info(Object? message) => log(LogLevel.info, message);

  /// Logs at [LogLevel.warning].
  void warning(Object? message) => log(LogLevel.warning, message);

  /// Logs at [LogLevel.error] with an optional error and stack trace.
  void error(
    Object? message, [
    Object? error,
    StackTrace? stackTrace,
  ]) => log(LogLevel.error, message, error: error, stackTrace: stackTrace);

  /// Logs at [LogLevel.fatal] with an optional error and stack trace.
  void fatal(
    Object? message, [
    Object? error,
    StackTrace? stackTrace,
  ]) => log(LogLevel.fatal, message, error: error, stackTrace: stackTrace);
}

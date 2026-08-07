/// Severity levels used by the logging module.
enum LogLevel {
  /// Extremely verbose, diagnostic-only messages.
  trace(0),

  /// Detailed messages useful during development.
  debug(1),

  /// Informational messages describing normal operation.
  info(2),

  /// Suspicious or recoverable conditions.
  warning(3),

  /// Errors that should be investigated.
  error(4),

  /// Errors that may be fatal to the running task.
  fatal(5);

  const LogLevel(this.priority);

  /// Numeric severity; higher means more important.
  final int priority;

  /// Whether this level is at least as severe as [threshold].
  bool isAtLeast(LogLevel threshold) => priority >= threshold.priority;
}

/// Well-known durations used across the application.
abstract final class DurationConstants {
  /// Default debounce window for rapid user input.
  static const Duration debounceDefault = Duration(milliseconds: 300);

  /// Debounce window for search-as-you-type inputs.
  static const Duration debounceSearch = Duration(milliseconds: 400);

  /// Network connect/send timeout.
  static const Duration networkConnectTimeout = Duration(seconds: 10);
  static const Duration networkSendTimeout = Duration(seconds: 10);

  /// Network receive/request timeout.
  static const Duration networkReceiveTimeout = Duration(seconds: 15);
  static const Duration networkRequestTimeout = Duration(seconds: 15);

  /// Default time-to-live for cached data.
  static const Duration cacheTtl = Duration(days: 1);

  /// Display duration for snack bars and toasts.
  static const Duration snackBarDuration = Duration(seconds: 2);
  static const Duration toastDuration = Duration(seconds: 2);

  /// Time the splash screen stays visible before routing.
  static const Duration splashDisplayDuration = Duration(seconds: 2);

  /// Base delay before the first retry of a transient failure.
  static const Duration retryBaseDelay = Duration(milliseconds: 500);
}

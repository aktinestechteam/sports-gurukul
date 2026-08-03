/// Network-wide defaults for the Sports Gurukul API client.
///
/// Timeouts and retry budgets live here so they can be tuned in one place
/// and overridden per environment when the environment configuration layer
/// lands.
abstract final class NetworkConfig {
  /// Default connection timeout.
  static const Duration connectTimeout = Duration(seconds: 15);

  /// Default receive timeout.
  static const Duration receiveTimeout = Duration(seconds: 30);

  /// Default send timeout.
  static const Duration sendTimeout = Duration(seconds: 30);

  /// Maximum automatic retries for idempotent requests.
  static const int maxRetries = 2;

  /// Header that carries the per-request correlation id.
  static const String requestIdHeader = 'X-Request-Id';
}

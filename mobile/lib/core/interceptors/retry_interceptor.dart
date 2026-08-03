import 'package:dio/dio.dart';

import 'package:sports_gurukul/core/network/network_config.dart';

/// Retries idempotent requests on transient failures.
///
/// Retries timeouts, connection errors and 429/5xx responses up to
/// [maxRetries] times. The retry attempt is tracked per-request in
/// `RequestOptions.extra` so the chain never loops.
class RetryInterceptor extends Interceptor {
  RetryInterceptor(this._dio, {this.maxRetries = NetworkConfig.maxRetries});

  final Dio _dio;
  final int maxRetries;

  static const String _attemptKey = 'retry_attempt';

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    final options = err.requestOptions;
    final attempt = options.extra[_attemptKey] as int? ?? 0;
    if (attempt >= maxRetries || !_isRetriable(err)) {
      handler.next(err);
      return;
    }
    options.extra[_attemptKey] = attempt + 1;
    try {
      final response = await _dio.fetch<dynamic>(options);
      handler.resolve(response);
    } on DioException catch (e) {
      handler.next(e);
    }
  }

  static bool _isRetriable(DioException err) {
    switch (err.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.sendTimeout:
      case DioExceptionType.receiveTimeout:
      case DioExceptionType.transformTimeout:
      case DioExceptionType.connectionError:
        return true;
      case DioExceptionType.badResponse:
        final status = err.response?.statusCode ?? 0;
        return status == 429 || status >= 500;
      case DioExceptionType.badCertificate:
      case DioExceptionType.cancel:
      case DioExceptionType.unknown:
        return false;
    }
  }
}

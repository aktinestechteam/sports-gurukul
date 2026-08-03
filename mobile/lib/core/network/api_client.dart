import 'package:dio/dio.dart';

import 'package:sports_gurukul/core/interceptors/auth_interceptor.dart';
import 'package:sports_gurukul/core/interceptors/logging_interceptor.dart';
import 'package:sports_gurukul/core/interceptors/request_id_interceptor.dart';
import 'package:sports_gurukul/core/interceptors/retry_interceptor.dart';
import 'package:sports_gurukul/core/network/network_config.dart';

/// Constructs the shared [Dio] instance for the application.
///
/// Wiring-only in P003: the client is configured with base options,
/// timeouts and the interceptor chain (request-id, auth, logging, retry).
/// No endpoint, repository or DTO logic exists yet.
abstract final class ApiClient {
  /// Builds a configured [Dio] instance.
  ///
  /// The base URL is resolved from the active environment when the
  /// environment configuration layer lands; it defaults to an empty string
  /// until then.
  static Dio create({String baseUrl = ''}) {
    final dio = Dio(
      BaseOptions(
        baseUrl: baseUrl,
        connectTimeout: NetworkConfig.connectTimeout,
        receiveTimeout: NetworkConfig.receiveTimeout,
        sendTimeout: NetworkConfig.sendTimeout,
        headers: const <String, Object>{'Accept': 'application/json'},
      ),
    );
    dio.interceptors.addAll(<Interceptor>[
      const RequestIdInterceptor(),
      const AuthInterceptor(),
      const LoggingInterceptor(),
      RetryInterceptor(dio),
    ]);
    return dio;
  }
}

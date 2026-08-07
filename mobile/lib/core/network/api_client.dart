import 'package:dio/dio.dart';

import 'package:sports_gurukul/core/authentication/token_store.dart';
import 'package:sports_gurukul/core/interceptors/auth_interceptor.dart';
import 'package:sports_gurukul/core/interceptors/logging_interceptor.dart';
import 'package:sports_gurukul/core/interceptors/request_id_interceptor.dart';
import 'package:sports_gurukul/core/interceptors/retry_interceptor.dart';
import 'package:sports_gurukul/core/network/network_config.dart';
import 'package:sports_gurukul/core/storage/secure_storage.dart';

/// Constructs the shared [Dio] instance for the application.
///
/// The client is configured with base options, timeouts and the interceptor
/// chain (request-id, auth, logging, retry). The [AuthInterceptor] can be
/// supplied by the composition root (wired to the app-wide [TokenStore] and
/// session-expiry notifications); otherwise a default one backed by secure
/// storage is created.
abstract final class ApiClient {
  /// Builds a configured [Dio] instance.
  static Dio create({
    String baseUrl = '',
    AuthInterceptor? authInterceptor,
  }) {
    // Dio appends relative paths to the base URL verbatim, so it must end
    // with a slash or `http://host:5297api/v1/auth/login` is produced.
    final normalizedBaseUrl = baseUrl.isEmpty
        ? ''
        : (baseUrl.endsWith('/') ? baseUrl : '$baseUrl/');
    final dio = Dio(
      BaseOptions(
        baseUrl: normalizedBaseUrl,
        connectTimeout: NetworkConfig.connectTimeout,
        receiveTimeout: NetworkConfig.receiveTimeout,
        sendTimeout: NetworkConfig.sendTimeout,
        headers: const <String, Object>{'Accept': 'application/json'},
      ),
    );
    dio.interceptors.addAll(<Interceptor>[
      const RequestIdInterceptor(),
      authInterceptor ??
          AuthInterceptor(
            tokenStore: TokenStore(storage: SecureStorage()),
            baseUrl: normalizedBaseUrl,
          ),
      const LoggingInterceptor(),
      RetryInterceptor(dio),
    ]);
    AuthInterceptor? wiredAuth;
    for (final interceptor in dio.interceptors) {
      if (interceptor case AuthInterceptor()) {
        wiredAuth = interceptor;
        break;
      }
    }
    wiredAuth?.attach(dio);
    return dio;
  }
}

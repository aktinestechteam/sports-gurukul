import 'package:dio/dio.dart';

import 'package:sports_gurukul/core/authentication/token_store.dart';
import 'package:sports_gurukul/core/constants/api_constants.dart';
import 'package:sports_gurukul/core/logging/app_logger.dart';

/// Attaches the access token to outgoing requests and refreshes it on 401.
///
/// Reading the token is asynchronous, so [onRequest] completes the request
/// only after the header has been attached (or a read failure has been
/// logged). A 401 response on an authorized endpoint triggers a single
/// refresh-token call with rotation; on success the original request is
/// retried with the new token, on failure the session is expired.
class AuthInterceptor extends Interceptor {
  AuthInterceptor({
    required TokenStore tokenStore,
    required String baseUrl,
    this.onSessionExpired,
  }) : _tokenStore = tokenStore,
       _baseUrl = baseUrl;

  final TokenStore _tokenStore;
  final String _baseUrl;

  /// Invoked when the refresh token is missing or rejected.
  final void Function()? onSessionExpired;

  /// The owning [Dio] instance, used to retry the original request.
  ///
  /// Wired by `ApiClient` after construction so the interceptor can re-issue
  /// the failed request through the same transport chain.
  Dio? _dio;

  /// Attaches the owning [Dio] instance.
  // ignore: use_setters_to_change_properties - wired once by the client
  void attach(Dio dio) => _dio = dio;

  static const String _refreshPathSegment = '/auth/refresh-token';
  static const String _retryKey = 'auth_retried';

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    try {
      final token = await _tokenStore.readAccessToken();
      if (token != null &&
          token.isNotEmpty &&
          !options.path.contains(_refreshPathSegment)) {
        options.headers[ApiConstants.authorizationHeader] =
            '${ApiConstants.bearerPrefix}$token';
      }
    } on Object catch (error) {
      AppLogger.e('Failed to read access token', error);
    }
    handler.next(options);
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    final request = err.requestOptions;
    final isUnauthorized =
        err.response?.statusCode == ApiConstants.statusUnauthorized;
    final alreadyRetried = request.extra[_retryKey] == true;
    final isRefreshCall = request.path.contains(_refreshPathSegment);

    if (!isUnauthorized || alreadyRetried || isRefreshCall) {
      handler.next(err);
      return;
    }

    final dio = _dio;
    if (dio == null) {
      handler.next(err);
      return;
    }

    String? refreshToken;
    try {
      refreshToken = await _tokenStore.readRefreshToken();
    } on Object catch (error) {
      AppLogger.e('Failed to read refresh token', error);
      handler.next(err);
      return;
    }

    if (refreshToken == null || refreshToken.isEmpty) {
      _expireSession();
      handler.next(err);
      return;
    }

    try {
      final response = await dio.post<Map<String, dynamic>>(
        '${ApiConstants.apiBasePath}$_refreshPathSegment',
        data: <String, Object>{'refreshToken': refreshToken},
      );
      final payload = response.data?['data'];
      if (payload is! Map<String, dynamic>) {
        throw StateError('Refresh response missing "data" object');
      }
      final accessToken = payload['accessToken'] as String?;
      final rotatedRefreshToken = payload['refreshToken'] as String?;
      if (accessToken == null || rotatedRefreshToken == null) {
        throw StateError('Refresh response missing token pair');
      }

      await _tokenStore.writeTokens(
        accessToken: accessToken,
        refreshToken: rotatedRefreshToken,
      );

      request.headers[ApiConstants.authorizationHeader] =
          '${ApiConstants.bearerPrefix}$accessToken';
      request.extra[_retryKey] = true;
      final retried = await dio.fetch<dynamic>(request);
      handler.resolve(retried);
    } on Object catch (error) {
      AppLogger.e('Token refresh failed', error);
      await _tokenStore.clear();
      _expireSession();
      handler.next(err);
    }
  }

  void _expireSession() {
    AppLogger.w('Session expired: unable to refresh access token');
    onSessionExpired?.call();
  }

  /// The base URL the interceptor was configured with, for diagnostics.
  String get baseUrl => _baseUrl;
}

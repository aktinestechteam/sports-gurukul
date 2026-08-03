import 'package:dio/dio.dart';

/// Attaches the access token to outgoing requests.
///
/// Scaffolded in P003. Reads the token from secure storage and handles
/// refresh on 401 when the authentication feature lands in P004.
class AuthInterceptor extends Interceptor {
  const AuthInterceptor();

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    handler.next(options);
  }
}

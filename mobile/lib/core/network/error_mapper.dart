import 'package:dio/dio.dart';

/// Stable error classifications produced by [mapNetworkError].
enum NetworkErrorKind {
  cancelled,
  timeout,
  connectivity,
  server,
  unauthorized,
  notFound,
  client,
  unknown,
}

/// Maps a [DioException] into a stable [NetworkErrorKind].
///
/// Consumed by the error-mapping layer in P004 to translate transport
/// failures into domain `Failure` values without leaking Dio types.
NetworkErrorKind mapNetworkError(DioException error) {
  switch (error.type) {
    case DioExceptionType.connectionTimeout:
    case DioExceptionType.sendTimeout:
    case DioExceptionType.receiveTimeout:
    case DioExceptionType.transformTimeout:
      return NetworkErrorKind.timeout;
    case DioExceptionType.connectionError:
      return NetworkErrorKind.connectivity;
    case DioExceptionType.badCertificate:
    case DioExceptionType.unknown:
      return NetworkErrorKind.unknown;
    case DioExceptionType.cancel:
      return NetworkErrorKind.cancelled;
    case DioExceptionType.badResponse:
      final status = error.response?.statusCode ?? 0;
      if (status == 401) {
        return NetworkErrorKind.unauthorized;
      }
      if (status == 404) {
        return NetworkErrorKind.notFound;
      }
      if (status >= 500) {
        return NetworkErrorKind.server;
      }
      return NetworkErrorKind.client;
  }
}

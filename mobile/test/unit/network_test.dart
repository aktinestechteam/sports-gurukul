import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/interceptors/auth_interceptor.dart';
import 'package:sports_gurukul/core/interceptors/logging_interceptor.dart';
import 'package:sports_gurukul/core/interceptors/request_id_interceptor.dart';
import 'package:sports_gurukul/core/interceptors/retry_interceptor.dart';
import 'package:sports_gurukul/core/network/api_client.dart';
import 'package:sports_gurukul/core/network/error_mapper.dart';
import 'package:sports_gurukul/core/network/network_config.dart';

void main() {
  group('mapNetworkError', () {
    DioException exceptionOf(DioExceptionType type, {int? statusCode}) {
      final options = RequestOptions(path: '/test');
      return DioException(
        requestOptions: options,
        type: type,
        response: statusCode == null
            ? null
            : Response<dynamic>(
                requestOptions: options,
                statusCode: statusCode,
              ),
      );
    }

    test('maps 401 to unauthorized', () {
      final kind = mapNetworkError(
        exceptionOf(
          DioExceptionType.badResponse,
          statusCode: 401,
        ),
      );
      expect(kind, NetworkErrorKind.unauthorized);
    });

    test('maps 404 to notFound', () {
      final kind = mapNetworkError(
        exceptionOf(
          DioExceptionType.badResponse,
          statusCode: 404,
        ),
      );
      expect(kind, NetworkErrorKind.notFound);
    });

    test('maps 500 to server', () {
      final kind = mapNetworkError(
        exceptionOf(
          DioExceptionType.badResponse,
          statusCode: 500,
        ),
      );
      expect(kind, NetworkErrorKind.server);
    });

    test('maps 400 to client', () {
      final kind = mapNetworkError(
        exceptionOf(
          DioExceptionType.badResponse,
          statusCode: 400,
        ),
      );
      expect(kind, NetworkErrorKind.client);
    });

    test('maps timeouts to timeout', () {
      final kind = mapNetworkError(
        exceptionOf(DioExceptionType.connectionTimeout),
      );
      expect(kind, NetworkErrorKind.timeout);
    });

    test('maps connectionError to connectivity', () {
      final kind = mapNetworkError(
        exceptionOf(DioExceptionType.connectionError),
      );
      expect(kind, NetworkErrorKind.connectivity);
    });
  });

  group('ApiClient', () {
    test('configures base url, timeouts and the full interceptor chain', () {
      final dio = ApiClient.create(baseUrl: 'https://api.example.com');

      expect(dio.options.baseUrl, 'https://api.example.com/');
      expect(dio.options.connectTimeout, NetworkConfig.connectTimeout);
      expect(dio.options.receiveTimeout, NetworkConfig.receiveTimeout);
      expect(dio.options.sendTimeout, NetworkConfig.sendTimeout);
      expect(dio.interceptors.whereType<RequestIdInterceptor>(), isNotEmpty);
      expect(dio.interceptors.whereType<AuthInterceptor>(), isNotEmpty);
      expect(dio.interceptors.whereType<LoggingInterceptor>(), isNotEmpty);
      expect(dio.interceptors.whereType<RetryInterceptor>(), isNotEmpty);
    });

    test('request-id interceptor attaches a correlation header', () {
      final dio = ApiClient.create();
      final interceptor = dio.interceptors
          .whereType<RequestIdInterceptor>()
          .first;
      final options = RequestOptions(path: '/ping');

      interceptor.onRequest(options, RequestInterceptorHandler());

      expect(options.headers, contains(NetworkConfig.requestIdHeader));
      expect(
        options.headers[NetworkConfig.requestIdHeader],
        isA<String>(),
      );
    });

    test('base url without a trailing slash gets one appended', () {
      final dio = ApiClient.create(baseUrl: 'http://localhost:5297');

      expect(
        dio.options.baseUrl,
        'http://localhost:5297/',
        reason: 'Dio concatenates baseUrl + relative path verbatim; without '
            'the slash it would request http://localhost:5297api/v1/...',
      );
    });

    test('base url with a trailing slash is left unchanged', () {
      final dio = ApiClient.create(baseUrl: 'http://localhost:5297/');

      expect(dio.options.baseUrl, 'http://localhost:5297/');
    });
  });
}

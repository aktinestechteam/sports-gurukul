import 'dart:math';

import 'package:dio/dio.dart';

import 'package:sports_gurukul/core/network/network_config.dart';

/// Attaches a correlation id to every request.
///
/// The id propagates to the backend for log correlation and is reused
/// across automatic retries. Generated with `dart:math` for now; replaced
/// by `package:uuid` when the utilities layer lands.
class RequestIdInterceptor extends Interceptor {
  const RequestIdInterceptor();

  static final _random = Random.secure();

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    options.headers.putIfAbsent(
      NetworkConfig.requestIdHeader,
      _generateId,
    );
    handler.next(options);
  }

  static String _generateId() {
    final timestamp = DateTime.now().microsecondsSinceEpoch;
    final random = _random.nextInt(1 << 31);
    return '$timestamp-$random';
  }
}

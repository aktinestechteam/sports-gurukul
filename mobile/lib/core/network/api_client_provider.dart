import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/authentication/auth_providers.dart';
import 'package:sports_gurukul/core/constants/api_constants.dart';
import 'package:sports_gurukul/core/interceptors/auth_interceptor.dart';
import 'package:sports_gurukul/core/network/api_client.dart';

/// Provides the application-wide [Dio] client.
///
/// The auth interceptor is wired to the shared token store and to the
/// session-expiry channel, so a failed refresh anywhere in the app logs the
/// user out through the auth controller rather than throwing per-screen.
final apiClientProvider = Provider<Dio>((ref) {
  final tokenStore = ref.watch(tokenStoreProvider);
  final sessionEvents = ref.watch(sessionEventsProvider);
  final authInterceptor = AuthInterceptor(
    tokenStore: tokenStore,
    baseUrl: ApiConstants.baseUrl,
    onSessionExpired: sessionEvents.expireSession,
  );
  return ApiClient.create(
    baseUrl: ApiConstants.baseUrl,
    authInterceptor: authInterceptor,
  );
});

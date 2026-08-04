import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/network/api_client_provider.dart';
import 'package:sports_gurukul/core/storage/storage_providers.dart';
import 'package:sports_gurukul/features/authentication/domain/repositories/auth_repository.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/datasources/auth_remote_datasource.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/repositories/auth_repository_impl.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/session/auth_session_store.dart';

/// Provides the auth remote datasource backed by the shared Dio client.
final authRemoteDataSourceProvider = Provider<AuthRemoteDataSource>(
  (ref) => DioAuthRemoteDataSource(dio: ref.watch(apiClientProvider)),
);

/// Provides the local session cache used for auto-login.
final authSessionStoreProvider = Provider<AuthSessionStore>(
  (ref) => AuthSessionStore(storage: ref.watch(secureStorageProvider)),
);

/// Provides the auth repository.
final authRepositoryProvider = Provider<AuthRepository>(
  (ref) => AuthRepositoryImpl(
    remote: ref.watch(authRemoteDataSourceProvider),
  ),
);

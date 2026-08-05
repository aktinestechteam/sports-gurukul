import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/network/api_client_provider.dart';
import 'package:sports_gurukul/features/user/domain/repositories/user_profile_repository.dart';
import 'package:sports_gurukul/features/user/infrastructure/datasources/user_profile_remote_datasource.dart';
import 'package:sports_gurukul/features/user/infrastructure/repositories/user_profile_repository_impl.dart';

/// Provides the user-profile remote datasource backed by the shared Dio client.
final userProfileRemoteDataSourceProvider =
    Provider<UserProfileRemoteDataSource>(
  (ref) => DioUserProfileRemoteDataSource(dio: ref.watch(apiClientProvider)),
);

/// Provides the user-profile repository.
final userProfileRepositoryProvider = Provider<UserProfileRepository>(
  (ref) => UserProfileRepositoryImpl(
    remote: ref.watch(userProfileRemoteDataSourceProvider),
  ),
);

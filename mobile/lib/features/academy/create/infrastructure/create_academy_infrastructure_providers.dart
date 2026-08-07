import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/network/api_client_provider.dart';
import 'package:sports_gurukul/features/academy/create/domain/repositories/create_academy_repository.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/datasources/create_academy_remote_datasource.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/repositories/create_academy_repository_impl.dart';

/// Provides the academy-creation remote datasource backed by the shared Dio
/// client.
final createAcademyRemoteDataSourceProvider =
    Provider<CreateAcademyRemoteDataSource>(
      (ref) => DioCreateAcademyRemoteDataSource(
        dio: ref.watch(apiClientProvider),
      ),
    );

/// Provides the academy-creation repository.
final createAcademyRepositoryProvider = Provider<CreateAcademyRepository>(
  (ref) => CreateAcademyRepositoryImpl(
    remote: ref.watch(createAcademyRemoteDataSourceProvider),
  ),
);

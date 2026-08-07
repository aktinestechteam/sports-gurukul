import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/features/academy/create/application/usecases/create_academy.dart';
import 'package:sports_gurukul/features/academy/create/application/usecases/update_academy.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/create_academy_infrastructure_providers.dart';

/// Provides the academy-creation use case.
final createAcademyUseCaseProvider = Provider<CreateAcademy>(
  (ref) => CreateAcademy(ref.watch(createAcademyRepositoryProvider)),
);

/// Provides the academy core-field update use case.
final updateAcademyUseCaseProvider = Provider<UpdateAcademy>(
  (ref) => UpdateAcademy(ref.watch(createAcademyRepositoryProvider)),
);

/// Provides the academy contact + address update use case.
final updateAcademyContactUseCaseProvider = Provider<UpdateAcademyContact>(
  (ref) => UpdateAcademyContact(ref.watch(createAcademyRepositoryProvider)),
);

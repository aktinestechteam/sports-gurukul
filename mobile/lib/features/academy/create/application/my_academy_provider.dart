import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/create_academy_infrastructure_providers.dart';
import 'package:sports_gurukul/features/onboarding/application/onboarding_providers.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_state.dart';

/// Resolves the current user's academy (their most recently created one) so
/// academy admins can brand their dashboard with the academy logo and name.
///
/// Only fetches for academy admins: any other session resolves to null
/// without a network round-trip. Failures (e.g. no academy yet) also resolve
/// to null so the dashboard degrades to the user-initials header.
final myAcademyProvider = FutureProvider<Academy?>((ref) async {
  final session = ref.watch(applicationSessionProvider);
  if (session == null || session.userState != UserState.academyAdmin) {
    return null;
  }
  final repository = ref.watch(createAcademyRepositoryProvider);
  return (await repository.getMyAcademy()).valueOrNull;
});

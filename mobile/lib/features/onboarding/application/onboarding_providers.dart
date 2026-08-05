import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/onboarding/application/current_user_resolution_exception.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/application_session.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/current_user.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/permission.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_state.dart';
import 'package:sports_gurukul/features/onboarding/infrastructure/mappers/current_user_mapper.dart';
import 'package:sports_gurukul/features/user/application/user_profile_use_case_providers.dart';
import 'package:sports_gurukul/features/user/infrastructure/error/user_profile_error_mapper.dart';

/// Fetches and caches the current user right after authentication.
///
/// Reuses the shared `GetCurrentProfile` use case (backed by
/// `GET /api/v1/users/me`); returns `null` while signed out and throws a
/// [CurrentUserResolutionException] carrying the underlying failure so the
/// onboarding UI can render a retry state.
///
/// A missing profile is not a failure: registration does not create a
/// `UserProfile`, so `GET /api/v1/users/me` answers 404 for a brand-new
/// account. That account is resolved from the auth session as a brand-new
/// user (default identity, no roles from the profile) so it reaches the
/// onboarding gate instead of an error screen.
final currentUserProvider = FutureProvider<CurrentUser?>((ref) async {
  final authState = ref.watch(authControllerProvider);
  if (authState is! AuthAuthenticated) return null;
  final result = await ref.watch(getCurrentProfileProvider).call();
  return switch (result) {
    Success(value: final profile) => CurrentUserMapper.fromProfile(profile),
    FailureResult(:final failure)
        when failure.code == UserProfileErrorCodes.notFound =>
      _currentUserWithoutProfile(authState),
    FailureResult(:final failure) => throw CurrentUserResolutionException(
      failure,
    ),
  };
});

/// Builds the [CurrentUser] for an account that has no profile yet, using the
/// identity carried by the auth session.
CurrentUser _currentUserWithoutProfile(AuthAuthenticated authState) {
  final user = authState.session.user;
  return CurrentUser(
    id: user.id,
    fullName: user.fullName,
    email: user.email,
    roles: user.roles
        .map(UserRole.fromName)
        .whereType<UserRole>()
        .toList(growable: false),
  );
}

/// Precedence used to pick the primary role (highest privilege first).
const List<UserRole> _rolePrecedence = <UserRole>[
  UserRole.superAdmin,
  UserRole.admin,
  UserRole.academy,
  UserRole.coach,
  UserRole.athlete,
  UserRole.parent,
  UserRole.scout,
  UserRole.sponsor,
  UserRole.aiAdministrator,
];

/// The highest-privilege role assigned to the current user.
final userRoleProvider = Provider<UserRole?>((ref) {
  final currentUser = ref.watch(currentUserProvider).value;
  if (currentUser == null) return null;
  for (final role in _rolePrecedence) {
    if (currentUser.roles.contains(role)) return role;
  }
  return currentUser.roles.isEmpty ? null : currentUser.roles.first;
});

/// Permissions derived from the resolved primary role.
final permissionProvider = Provider<Set<Permission>>(
  (ref) => permissionsForRole(ref.watch(userRoleProvider)),
);

/// The fully resolved application session, or `null` while it is being
/// assembled (loading / error / signed out).
final applicationSessionProvider = Provider<ApplicationSession?>((ref) {
  final authState = ref.watch(authControllerProvider);
  if (authState is! AuthAuthenticated) return null;
  final currentUser = ref.watch(currentUserProvider).value;
  if (currentUser == null) return null;
  final role = ref.watch(userRoleProvider);
  return ApplicationSession(
    authSession: authState.session,
    currentUser: currentUser,
    primaryRole: role,
    permissions: permissionsForRole(role),
    userState: resolveUserState(
      roles: currentUser.roles,
      hasAcademyAssociation: currentUser.hasAcademyAssociation,
      isMembershipPending: currentUser.hasPendingMembership,
    ),
  );
});

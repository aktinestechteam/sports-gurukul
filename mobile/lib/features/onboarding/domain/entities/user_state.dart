import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';

/// Onboarding classification of a user, driving post-login navigation and the
/// profile role badge.
///
/// Authentication identifies WHO the user is; business roles identify WHAT the
/// user can do. A new registration creates only a platform user and must never
/// surface a business role badge (Athlete, Coach, ...) until the backend
/// explicitly assigns one.
enum UserState {
  /// The session has not been restored / resolved yet (splash screen).
  unknown,

  /// Signed out (login screens).
  unauthenticated,

  /// Brand-new account with no academy association and no business role; must
  /// pick an onboarding path (welcome screen / limited dashboard).
  newUser,

  /// A membership/join request is awaiting academy approval.
  pendingApproval,

  /// Established academy member without a distinct business role.
  academyMember,

  /// Academy administrator (Academy Admin role).
  academyAdmin,

  /// Coach.
  coach,

  /// Athlete assigned by an academy.
  athlete,

  /// Platform/system administrator.
  systemAdmin,
}

/// Resolves the [UserState] from the signals exposed by the application.
///
/// The backend's current-user API does not expose a dedicated
/// academy-association or first-login field, so membership is derived from the
/// available profile signals: assigned roles, an academy-type address and (once
/// a join flow exists) a pending membership request.
///
/// Registration assigns every account the default `Athlete` role
/// ([UserRole.defaultRegistrationRoleName]); that role alone never marks a
/// member - the account stays a [UserState.newUser] until it gains an academy
/// association or a business role, so a brand-new user is never shown an
/// Athlete badge.
///
/// [isAuthStateKnown] and [isAuthenticated] let the same resolver describe the
/// whole lifecycle; callers that only classify authenticated users may rely on
/// the defaults.
UserState resolveUserState({
  required List<UserRole> roles,
  required bool hasAcademyAssociation,
  bool isMembershipPending = false,
  bool isAuthStateKnown = true,
  bool isAuthenticated = true,
}) {
  if (!isAuthStateKnown) return UserState.unknown;
  if (!isAuthenticated) return UserState.unauthenticated;
  if (roles.any((role) => role.isPlatformAdministrator)) {
    return UserState.systemAdmin;
  }
  if (isMembershipPending) {
    return UserState.pendingApproval;
  }
  final hasBusinessRole = roles.any(
    (role) => !role.isDefaultRegistrationRole,
  );
  if (!hasAcademyAssociation && !hasBusinessRole) {
    return UserState.newUser;
  }
  return _stateForRoles(roles);
}

/// Maps the assigned roles to the business-role [UserState].
UserState _stateForRoles(List<UserRole> roles) {
  if (roles.contains(UserRole.academy)) return UserState.academyAdmin;
  if (roles.contains(UserRole.coach)) return UserState.coach;
  if (roles.contains(UserRole.athlete)) return UserState.athlete;
  return UserState.academyMember;
}

import 'package:flutter/foundation.dart';

import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/current_user.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/permission.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_state.dart';

/// A user's academy relationship, when one is available.
///
/// The current-user API does not expose this yet, so an application session
/// carries a null association until a dedicated endpoint exists.
@immutable
class AcademyAssociation {
  const AcademyAssociation({
    this.academyId,
    this.academyName,
    this.roleInAcademy,
  });

  final String? academyId;
  final String? academyName;
  final String? roleInAcademy;

  @override
  bool operator ==(Object other) =>
      other is AcademyAssociation &&
      other.academyId == academyId &&
      other.academyName == academyName &&
      other.roleInAcademy == roleInAcademy;

  @override
  int get hashCode => Object.hash(academyId, academyName, roleInAcademy);
}

/// The resolved application-level session assembled right after login.
///
/// Aggregates the raw [AuthSession] (tokens), the resolved [CurrentUser], the
/// primary [UserRole], the role-derived [Permission] set, and the classified
/// [UserState] that drives post-login navigation.
@immutable
class ApplicationSession {
  const ApplicationSession({
    required this.authSession,
    required this.currentUser,
    required this.userState,
    this.primaryRole,
    this.permissions = const <Permission>{},
    this.academyAssociation,
  });

  /// The underlying authentication session (identity + token pair).
  final AuthSession authSession;

  /// The resolved current user.
  final CurrentUser currentUser;

  /// The highest-privilege role assigned to the user, when any is known.
  final UserRole? primaryRole;

  /// Permissions derived from [primaryRole].
  final Set<Permission> permissions;

  /// The onboarding classification that drives navigation.
  final UserState userState;

  /// The user's academy relationship, when the backend provides one.
  final AcademyAssociation? academyAssociation;

  /// Whether the user must pass through the welcome/onboarding screen.
  bool get isNewUser => userState == UserState.newUser;

  /// Whether this is the account's first sign-in (derived heuristic: the
  /// backend does not expose a first-login flag on the current-user API).
  bool get isFirstLogin => currentUser.profileCompletionPercentage == 0;

  /// Whether [permission] is granted to the user.
  bool hasPermission(Permission permission) => permissions.contains(permission);

  @override
  bool operator ==(Object other) =>
      other is ApplicationSession &&
      other.authSession == authSession &&
      other.currentUser == currentUser &&
      other.primaryRole == primaryRole &&
      other.permissions.length == permissions.length &&
      other.permissions.containsAll(permissions) &&
      other.userState == userState &&
      other.academyAssociation == academyAssociation;

  @override
  int get hashCode => Object.hash(
    authSession,
    currentUser,
    primaryRole,
    Object.hashAll(permissions),
    userState,
    academyAssociation,
  );
}

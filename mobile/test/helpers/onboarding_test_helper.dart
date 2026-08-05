import 'package:sports_gurukul/features/onboarding/domain/entities/application_session.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/current_user.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/permission.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_state.dart';

import 'auth_test_helper.dart';

/// A brand-new account carrying only the default registration role.
CurrentUser testNewUserCurrentUser() => const CurrentUser(
  id: 'user-1',
  fullName: 'Test Player',
  email: 'player@example.com',
  roles: <UserRole>[UserRole.athlete],
);

/// An established academy member with a completed profile and a role.
CurrentUser testMemberCurrentUser() => const CurrentUser(
  id: 'user-2',
  fullName: 'Ravi Verma',
  email: 'ravi@example.com',
  roles: <UserRole>[UserRole.coach],
  profileCompletionPercentage: 45,
  hasAcademyAssociation: true,
);

/// An academy administrator assigned by an academy.
CurrentUser testAcademyAdminCurrentUser() => const CurrentUser(
  id: 'user-4',
  fullName: 'Aisha Khan',
  email: 'aisha@example.com',
  roles: <UserRole>[UserRole.academy],
  profileCompletionPercentage: 60,
  hasAcademyAssociation: true,
);

/// An athlete whose role was assigned by an academy.
CurrentUser testAthleteMemberCurrentUser() => const CurrentUser(
  id: 'user-5',
  fullName: 'Kiran Patel',
  email: 'kiran@example.com',
  roles: <UserRole>[UserRole.athlete],
  profileCompletionPercentage: 35,
  hasAcademyAssociation: true,
);

/// A user whose join-academy request is awaiting approval.
CurrentUser testPendingApprovalCurrentUser() => const CurrentUser(
  id: 'user-6',
  fullName: 'Maya Rao',
  email: 'maya@example.com',
  roles: <UserRole>[UserRole.athlete],
  hasPendingMembership: true,
);

/// A platform administrator.
CurrentUser testAdminCurrentUser() => const CurrentUser(
  id: 'user-3',
  fullName: 'Platform Admin',
  email: 'admin@example.com',
  roles: <UserRole>[UserRole.admin],
  profileCompletionPercentage: 100,
);

/// A resolved application session for a brand-new user.
ApplicationSession testNewUserSession() => ApplicationSession(
  authSession: testAuthSession(),
  currentUser: testNewUserCurrentUser(),
  primaryRole: UserRole.athlete,
  permissions: permissionsForRole(UserRole.athlete),
  userState: resolveUserState(
    roles: const <UserRole>[UserRole.athlete],
    hasAcademyAssociation: false,
  ),
);

/// A resolved application session for an established academy member.
ApplicationSession testMemberSession() => ApplicationSession(
  authSession: testAuthSession(email: 'ravi@example.com'),
  currentUser: testMemberCurrentUser(),
  primaryRole: UserRole.coach,
  permissions: permissionsForRole(UserRole.coach),
  userState: resolveUserState(
    roles: const <UserRole>[UserRole.coach],
    hasAcademyAssociation: true,
  ),
);

/// A resolved application session for an academy administrator.
ApplicationSession testAcademyAdminSession() => ApplicationSession(
  authSession: testAuthSession(email: 'aisha@example.com'),
  currentUser: testAcademyAdminCurrentUser(),
  primaryRole: UserRole.academy,
  permissions: permissionsForRole(UserRole.academy),
  userState: resolveUserState(
    roles: const <UserRole>[UserRole.academy],
    hasAcademyAssociation: true,
  ),
);

/// A resolved application session for an academy-assigned athlete.
ApplicationSession testAthleteMemberSession() => ApplicationSession(
  authSession: testAuthSession(email: 'kiran@example.com'),
  currentUser: testAthleteMemberCurrentUser(),
  primaryRole: UserRole.athlete,
  permissions: permissionsForRole(UserRole.athlete),
  userState: resolveUserState(
    roles: const <UserRole>[UserRole.athlete],
    hasAcademyAssociation: true,
  ),
);

/// A resolved application session for a pending membership request.
ApplicationSession testPendingApprovalSession() => ApplicationSession(
  authSession: testAuthSession(email: 'maya@example.com'),
  currentUser: testPendingApprovalCurrentUser(),
  primaryRole: UserRole.athlete,
  permissions: permissionsForRole(UserRole.athlete),
  userState: resolveUserState(
    roles: const <UserRole>[UserRole.athlete],
    hasAcademyAssociation: false,
    isMembershipPending: true,
  ),
);

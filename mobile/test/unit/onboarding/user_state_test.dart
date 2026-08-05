import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_state.dart';

void main() {
  group('resolveUserState', () {
    test('an unknown auth state resolves to unknown', () {
      expect(
        resolveUserState(
          roles: const <UserRole>[],
          hasAcademyAssociation: false,
          isAuthStateKnown: false,
        ),
        UserState.unknown,
      );
    });

    test('a signed-out user resolves to unauthenticated', () {
      expect(
        resolveUserState(
          roles: const <UserRole>[],
          hasAcademyAssociation: false,
          isAuthenticated: false,
        ),
        UserState.unauthenticated,
      );
    });

    test('admin roles classify as system admins', () {
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.admin],
          hasAcademyAssociation: false,
        ),
        UserState.systemAdmin,
      );
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.superAdmin, UserRole.athlete],
          hasAcademyAssociation: true,
        ),
        UserState.systemAdmin,
      );
    });

    test('the academy role classifies as an academy admin', () {
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.academy],
          hasAcademyAssociation: false,
        ),
        UserState.academyAdmin,
      );
    });

    test('the coach role classifies as a coach', () {
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.coach],
          hasAcademyAssociation: false,
        ),
        UserState.coach,
      );
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.coach],
          hasAcademyAssociation: true,
        ),
        UserState.coach,
      );
    });

    test('a default athlete role alone never marks a member', () {
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.athlete],
          hasAcademyAssociation: false,
        ),
        UserState.newUser,
      );
      expect(
        resolveUserState(
          roles: const <UserRole>[],
          hasAcademyAssociation: false,
        ),
        UserState.newUser,
      );
    });

    test('an athlete with an academy association is an athlete', () {
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.athlete],
          hasAcademyAssociation: true,
        ),
        UserState.athlete,
      );
    });

    test('a pending membership resolves to pending approval', () {
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.athlete],
          hasAcademyAssociation: false,
          isMembershipPending: true,
        ),
        UserState.pendingApproval,
      );
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.coach],
          hasAcademyAssociation: true,
          isMembershipPending: true,
        ),
        UserState.pendingApproval,
      );
    });

    test('other business roles classify as academy members', () {
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.parent],
          hasAcademyAssociation: false,
        ),
        UserState.academyMember,
      );
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.scout],
          hasAcademyAssociation: true,
        ),
        UserState.academyMember,
      );
    });

    test('profile completion alone never marks a member', () {
      expect(
        resolveUserState(
          roles: const <UserRole>[UserRole.athlete],
          hasAcademyAssociation: false,
        ),
        UserState.newUser,
      );
    });
  });
}

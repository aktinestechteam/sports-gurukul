import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/permission.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';

void main() {
  group('permissionsForRole', () {
    test('every role holds the base permissions', () {
      for (final role in UserRole.values) {
        expect(
          permissionsForRole(role).containsAll(basePermissions),
          isTrue,
          reason: 'base permissions missing for $role',
        );
      }
      expect(permissionsForRole(null), basePermissions);
    });

    test('administrators receive full management permissions', () {
      final permissions = permissionsForRole(UserRole.admin);
      expect(permissions, contains(Permission.createAcademy));
      expect(permissions, contains(Permission.joinAcademy));
      expect(permissions, contains(Permission.manageAcademy));
      expect(permissions, contains(Permission.managePayments));
      expect(permissions, contains(Permission.viewAnalytics));
    });

    test('athletes get bookings and joining but no management', () {
      final permissions = permissionsForRole(UserRole.athlete);
      expect(permissions, contains(Permission.manageBookings));
      expect(permissions, contains(Permission.joinAcademy));
      expect(permissions, isNot(contains(Permission.createAcademy)));
      expect(permissions, isNot(contains(Permission.manageAcademy)));
      expect(permissions, isNot(contains(Permission.manageSessions)));
    });

    test('coaches schedule sessions but do not manage the academy', () {
      final permissions = permissionsForRole(UserRole.coach);
      expect(permissions, contains(Permission.manageSessions));
      expect(permissions, isNot(contains(Permission.manageAcademy)));
      expect(permissions, isNot(contains(Permission.managePayments)));
    });

    test('unknown role falls back to the base permission set', () {
      expect(permissionsForRole(null), basePermissions);
    });
  });
}

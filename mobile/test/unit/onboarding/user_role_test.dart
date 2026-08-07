import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';

void main() {
  group('UserRole.fromName', () {
    test('parses every backend role name', () {
      expect(UserRole.fromName('SuperAdmin'), UserRole.superAdmin);
      expect(UserRole.fromName('Admin'), UserRole.admin);
      expect(UserRole.fromName('Academy'), UserRole.academy);
      expect(UserRole.fromName('Academy Admin'), UserRole.academy);
      expect(UserRole.fromName('Coach'), UserRole.coach);
      expect(UserRole.fromName('Athlete'), UserRole.athlete);
      expect(UserRole.fromName('Parent'), UserRole.parent);
      expect(UserRole.fromName('Scout'), UserRole.scout);
      expect(UserRole.fromName('Sponsor'), UserRole.sponsor);
      expect(UserRole.fromName('AIAdministrator'), UserRole.aiAdministrator);
    });

    test('normalizes casing and separators', () {
      expect(UserRole.fromName('superadmin'), UserRole.superAdmin);
      expect(UserRole.fromName('Super-Admin'), UserRole.superAdmin);
      expect(UserRole.fromName('SUPER_ADMIN'), UserRole.superAdmin);
      expect(UserRole.fromName('ai-admin'), UserRole.aiAdministrator);
      expect(UserRole.fromName(' Academy Admin '), UserRole.academy);
      expect(UserRole.fromName('AcademyAdmin'), UserRole.academy);
    });

    test('returns null for unknown roles', () {
      expect(UserRole.fromName('Guest'), isNull);
      expect(UserRole.fromName(''), isNull);
    });
  });

  group('UserRole flags', () {
    test('only super admin and admin are platform administrators', () {
      expect(UserRole.superAdmin.isPlatformAdministrator, isTrue);
      expect(UserRole.admin.isPlatformAdministrator, isTrue);
      expect(UserRole.academy.isPlatformAdministrator, isFalse);
      expect(UserRole.athlete.isPlatformAdministrator, isFalse);
    });

    test('athlete is the default registration role', () {
      expect(UserRole.defaultRegistrationRoleName, 'Athlete');
      expect(UserRole.athlete.isDefaultRegistrationRole, isTrue);
      expect(UserRole.coach.isDefaultRegistrationRole, isFalse);
    });
  });
}

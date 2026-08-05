import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/permission.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';

import '../../helpers/onboarding_test_helper.dart';

void main() {
  group('ApplicationSession', () {
    test('isNewUser and isFirstLogin reflect a brand-new account', () {
      final session = testNewUserSession();
      expect(session.isNewUser, isTrue);
      expect(session.isFirstLogin, isTrue);
      expect(session.currentUser.id, 'user-1');
      expect(session.primaryRole, UserRole.athlete);
    });

    test('members and admins are not new users', () {
      expect(testMemberSession().isNewUser, isFalse);
      expect(testMemberSession().isFirstLogin, isFalse);
    });

    test('hasPermission answers from the derived permission set', () {
      final session = testNewUserSession();
      expect(session.hasPermission(Permission.manageBookings), isTrue);
      expect(session.hasPermission(Permission.manageAcademy), isFalse);
      expect(session.hasPermission(Permission.viewDashboard), isTrue);
    });

    test('academy association stays null until the backend exposes one', () {
      expect(testNewUserSession().academyAssociation, isNull);
    });
  });
}

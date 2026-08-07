import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/current_user.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';

void main() {
  group('CurrentUser', () {
    test('hasOnlyDefaultRole is true for a lone athlete role', () {
      const user = CurrentUser(
        id: 'u1',
        fullName: 'Aarav',
        email: 'aarav@example.com',
        roles: <UserRole>[UserRole.athlete],
      );
      expect(user.hasOnlyDefaultRole, isTrue);
    });

    test('hasOnlyDefaultRole is false for other or multiple roles', () {
      const coach = CurrentUser(
        id: 'u2',
        fullName: 'Coach',
        email: 'coach@example.com',
        roles: <UserRole>[UserRole.coach],
      );
      expect(coach.hasOnlyDefaultRole, isFalse);

      const mixed = CurrentUser(
        id: 'u3',
        fullName: 'Mixed',
        email: 'mixed@example.com',
        roles: <UserRole>[UserRole.coach, UserRole.athlete],
      );
      expect(mixed.hasOnlyDefaultRole, isFalse);
    });

    test('equality is based on the identity fields', () {
      const a = CurrentUser(
        id: 'u1',
        fullName: 'Aarav',
        email: 'aarav@example.com',
        roles: <UserRole>[UserRole.athlete],
      );
      const b = CurrentUser(
        id: 'u1',
        fullName: 'Aarav',
        email: 'aarav@example.com',
        roles: <UserRole>[UserRole.coach],
      );
      const c = CurrentUser(
        id: 'u9',
        fullName: 'Aarav',
        email: 'aarav@example.com',
        roles: <UserRole>[UserRole.athlete],
      );
      expect(a, b);
      expect(a == c, isFalse);
    });

    test('defaults keep the completion and association flags neutral', () {
      const user = CurrentUser(
        id: 'u1',
        fullName: 'Aarav',
        email: 'aarav@example.com',
        roles: <UserRole>[UserRole.athlete],
      );
      expect(user.profileCompletionPercentage, 0);
      expect(user.hasAcademyAssociation, isFalse);
      expect(user.isEmailVerified, isFalse);
      expect(user.profileImageUrl, isNull);
    });
  });
}

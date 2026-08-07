import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';
import 'package:sports_gurukul/features/onboarding/infrastructure/mappers/current_user_mapper.dart';
import 'package:sports_gurukul/features/user/domain/entities/address.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';

void main() {
  group('CurrentUserMapper', () {
    test('fromProfile maps identity, roles and profile signals', () {
      final profile = UserProfile(
        id: 'profile-1',
        userId: 'user-1',
        fullName: 'Aarav Sharma',
        email: 'aarav@example.com',
        createdAt: DateTime(2025, 5, 5),
        isEmailVerified: true,
        profileCompletionPercentage: 85,
        roles: const <String>['Coach', 'Athlete'],
      );

      final user = CurrentUserMapper.fromProfile(profile);
      expect(user.id, 'user-1');
      expect(user.fullName, 'Aarav Sharma');
      expect(user.email, 'aarav@example.com');
      expect(user.roles, <UserRole>[UserRole.coach, UserRole.athlete]);
      expect(user.isEmailVerified, isTrue);
      expect(user.profileCompletionPercentage, 85);
      expect(user.hasAcademyAssociation, isFalse);
    });

    test('fromProfile detects an academy-type address', () {
      final profile = UserProfile(
        id: 'p',
        userId: 'u',
        fullName: 'X',
        email: 'x@example.com',
        createdAt: DateTime(2025, 5, 5),
        roles: const <String>['Athlete'],
        addresses: const <Address>[
          Address(
            id: 'addr-1',
            addressType: AddressType.academy,
            line1: 'Sports Complex',
            city: 'Pune',
            state: 'MH',
            country: 'IN',
          ),
        ],
      );
      expect(
        CurrentUserMapper.fromProfile(profile).hasAcademyAssociation,
        isTrue,
      );
    });

    test('unknown role strings are dropped', () {
      final profile = UserProfile(
        id: 'p',
        userId: 'u',
        fullName: 'X',
        email: 'x@example.com',
        createdAt: DateTime(2025, 5, 5),
        roles: const <String>['Guest', 'Athlete'],
      );
      expect(
        CurrentUserMapper.fromProfile(profile).roles,
        <UserRole>[UserRole.athlete],
      );
    });
  });
}

import 'package:sports_gurukul/features/onboarding/domain/entities/current_user.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';
import 'package:sports_gurukul/features/user/domain/entities/address.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';

/// Converts the shared [UserProfile] entity into the onboarding [CurrentUser].
///
/// Mapping happens only at the feature boundary; the onboarding feature never
/// depends on the user feature's DTO layer directly.
abstract final class CurrentUserMapper {
  static CurrentUser fromProfile(UserProfile profile) => CurrentUser(
    id: profile.userId,
    fullName: profile.fullName,
    email: profile.email,
    roles: profile.roles
        .map(UserRole.fromName)
        .whereType<UserRole>()
        .toList(growable: false),
    profileImageUrl: profile.profileImageUrl,
    isEmailVerified: profile.isEmailVerified,
    profileCompletionPercentage: profile.profileCompletionPercentage,
    hasAcademyAssociation: profile.addresses.any(
      (address) => address.addressType == AddressType.academy,
    ),
  );
}

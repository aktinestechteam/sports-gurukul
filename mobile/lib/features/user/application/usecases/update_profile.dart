import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/domain/repositories/user_profile_repository.dart';

/// Updates the profile of the currently authenticated user.
///
/// Only non-null parameters are sent; the backend applies partial updates.
class UpdateProfile {
  const UpdateProfile(this._repository);

  final UserProfileRepository _repository;

  Future<Result<UserProfile>> call({
    DateTime? dateOfBirth,
    String? gender,
    String? bio,
    String? height,
    String? weight,
    String? preferredSport,
    String? experienceLevel,
    String? primaryPhoneCountryCode,
    String? primaryPhoneNumber,
    String? addressLine1,
    String? addressLine2,
    String? city,
    String? state,
    String? country,
    String? postalCode,
    String? addressType,
  }) => _repository.updateProfile(
    dateOfBirth: dateOfBirth,
    gender: gender,
    bio: bio,
    height: height,
    weight: weight,
    preferredSport: preferredSport,
    experienceLevel: experienceLevel,
    primaryPhoneCountryCode: primaryPhoneCountryCode,
    primaryPhoneNumber: primaryPhoneNumber,
    addressLine1: addressLine1,
    addressLine2: addressLine2,
    city: city,
    state: state,
    country: country,
    postalCode: postalCode,
    addressType: addressType,
  );
}

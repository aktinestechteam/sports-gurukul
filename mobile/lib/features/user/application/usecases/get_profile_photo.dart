import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/user/domain/entities/profile_photo.dart';
import 'package:sports_gurukul/features/user/domain/repositories/user_profile_repository.dart';

/// Gets the profile photo metadata for the currently authenticated user.
class GetProfilePhoto {
  const GetProfilePhoto(this._repository);

  final UserProfileRepository _repository;

  Future<Result<ProfilePhoto>> call() => _repository.getProfilePhoto();
}

import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/domain/repositories/user_profile_repository.dart';

/// Fetches the full profile of the currently authenticated user.
class GetCurrentProfile {
  const GetCurrentProfile(this._repository);

  final UserProfileRepository _repository;

  Future<Result<UserProfile>> call() => _repository.getCurrentProfile();
}

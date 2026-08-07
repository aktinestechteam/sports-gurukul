import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/features/user/domain/repositories/user_profile_repository.dart';

/// Deletes the profile photo of the currently authenticated user.
class DeleteProfilePhoto {
  const DeleteProfilePhoto(this._repository);

  final UserProfileRepository _repository;

  Future<OperationResult> call() => _repository.deleteProfilePhoto();
}

import 'dart:io';

import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/user/domain/entities/profile_photo.dart';
import 'package:sports_gurukul/features/user/domain/repositories/user_profile_repository.dart';

/// Uploads a new profile photo for the currently authenticated user.
class UploadProfilePhoto {
  const UploadProfilePhoto(this._repository);

  final UserProfileRepository _repository;

  Future<Result<ProfilePhoto>> call(File imageFile) =>
      _repository.uploadProfilePhoto(imageFile);
}

import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/features/authentication/domain/repositories/auth_repository.dart';

/// Resets a user's password using the token from the reset email.
class ResetPassword {
  const ResetPassword(this._repository);

  final AuthRepository _repository;

  Future<OperationResult> call({
    required String token,
    required String newPassword,
    required String confirmNewPassword,
  }) => _repository.resetPassword(
    token: token,
    newPassword: newPassword,
    confirmNewPassword: confirmNewPassword,
  );
}

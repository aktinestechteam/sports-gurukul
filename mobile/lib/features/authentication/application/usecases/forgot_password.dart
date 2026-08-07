import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/features/authentication/domain/repositories/auth_repository.dart';

/// Requests a password reset email for an account.
class ForgotPassword {
  const ForgotPassword(this._repository);

  final AuthRepository _repository;

  Future<OperationResult> call(String email) =>
      _repository.forgotPassword(email);
}

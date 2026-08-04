import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/features/authentication/domain/repositories/auth_repository.dart';

/// Confirms an email address using the verification token from the email.
class VerifyEmail {
  const VerifyEmail(this._repository);

  final AuthRepository _repository;

  Future<OperationResult> call(String token) => _repository.verifyEmail(token);
}

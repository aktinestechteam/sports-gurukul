import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/features/authentication/domain/repositories/auth_repository.dart';

/// Logs the current user out by revoking server-side refresh tokens.
class LogoutUser {
  const LogoutUser(this._repository);

  final AuthRepository _repository;

  Future<OperationResult> call() => _repository.logout();
}

import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/domain/repositories/auth_repository.dart';

/// Authenticates a user with email and password.
class LoginUser {
  const LoginUser(this._repository);

  final AuthRepository _repository;

  Future<Result<AuthSession>> call({
    required String email,
    required String password,
  }) => _repository.login(email: email, password: password);
}

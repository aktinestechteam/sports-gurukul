import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/domain/repositories/auth_repository.dart';

/// Registers a new user account.
class RegisterUser {
  const RegisterUser(this._repository);

  final AuthRepository _repository;

  Future<Result<AuthSession>> call({
    required String fullName,
    required String email,
    required String password,
    required String confirmPassword,
    String? phoneNumber,
  }) => _repository.register(
    fullName: fullName,
    email: email,
    password: password,
    confirmPassword: confirmPassword,
    phoneNumber: phoneNumber,
  );
}

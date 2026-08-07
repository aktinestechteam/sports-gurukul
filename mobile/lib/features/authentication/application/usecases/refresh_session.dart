import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/token_pair.dart';
import 'package:sports_gurukul/features/authentication/domain/repositories/auth_repository.dart';

/// Rotates a refresh token into a fresh token pair.
class RefreshSession {
  const RefreshSession(this._repository);

  final AuthRepository _repository;

  Future<Result<TokenPair>> call(String refreshToken) =>
      _repository.refreshToken(refreshToken);
}

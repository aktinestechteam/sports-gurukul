import 'package:sports_gurukul/core/exceptions/app_exception.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/token_pair.dart';
import 'package:sports_gurukul/features/authentication/domain/repositories/auth_repository.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/datasources/auth_remote_datasource.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/error/auth_error_mapper.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/mappers/auth_mappers.dart';

/// [AuthRepository] implementation backed by the remote auth datasource.
///
/// Transport and server failures are mapped to typed [BaseFailure]s at this
/// boundary; features only ever see `Result`/`OperationResult` values.
class AuthRepositoryImpl implements AuthRepository {
  AuthRepositoryImpl({required AuthRemoteDataSource remote}) : _remote = remote;

  final AuthRemoteDataSource _remote;

  @override
  Future<Result<AuthSession>> login({
    required String email,
    required String password,
  }) async {
    try {
      final dto = await _remote.login(email: email, password: password);
      return Result.success(AuthMappers.toSession(dto));
    } on ApiException catch (error) {
      return Result.failure(AuthErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<Result<AuthSession>> register({
    required String fullName,
    required String email,
    required String password,
    required String confirmPassword,
    String? phoneNumber,
  }) async {
    try {
      final dto = await _remote.register(
        fullName: fullName,
        email: email,
        password: password,
        confirmPassword: confirmPassword,
        phoneNumber: phoneNumber,
      );
      return Result.success(AuthMappers.toSession(dto));
    } on ApiException catch (error) {
      return Result.failure(AuthErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<Result<TokenPair>> refreshToken(String refreshToken) async {
    try {
      final dto = await _remote.refreshToken(refreshToken);
      return Result.success(AuthMappers.toTokenPair(dto));
    } on ApiException catch (error) {
      return Result.failure(AuthErrorMapper.map(error));
    } on Object catch (error) {
      return Result.failure(_unexpected(error));
    }
  }

  @override
  Future<OperationResult> logout() async {
    try {
      await _remote.logout();
      return const OperationResult.success();
    } on ApiException catch (error) {
      return OperationResult.failure(AuthErrorMapper.map(error));
    } on Object catch (error) {
      return OperationResult.failure(_unexpected(error));
    }
  }

  @override
  Future<OperationResult> forgotPassword(String email) async {
    try {
      await _remote.forgotPassword(email);
      return const OperationResult.success();
    } on ApiException catch (error) {
      return OperationResult.failure(AuthErrorMapper.map(error));
    } on Object catch (error) {
      return OperationResult.failure(_unexpected(error));
    }
  }

  @override
  Future<OperationResult> resetPassword({
    required String token,
    required String newPassword,
    required String confirmNewPassword,
  }) async {
    try {
      await _remote.resetPassword(
        token: token,
        newPassword: newPassword,
        confirmNewPassword: confirmNewPassword,
      );
      return const OperationResult.success();
    } on ApiException catch (error) {
      return OperationResult.failure(AuthErrorMapper.map(error));
    } on Object catch (error) {
      return OperationResult.failure(_unexpected(error));
    }
  }

  @override
  Future<OperationResult> sendVerificationEmail(String email) async {
    try {
      await _remote.sendVerificationEmail(email);
      return const OperationResult.success();
    } on ApiException catch (error) {
      return OperationResult.failure(AuthErrorMapper.map(error));
    } on Object catch (error) {
      return OperationResult.failure(_unexpected(error));
    }
  }

  @override
  Future<OperationResult> verifyEmail(String token) async {
    try {
      await _remote.verifyEmail(token);
      return const OperationResult.success();
    } on ApiException catch (error) {
      return OperationResult.failure(AuthErrorMapper.map(error));
    } on Object catch (error) {
      return OperationResult.failure(_unexpected(error));
    }
  }

  static BaseFailure _unexpected(Object error) => UnknownFailure(
    message: 'Unexpected authentication failure',
    cause: error,
  );
}

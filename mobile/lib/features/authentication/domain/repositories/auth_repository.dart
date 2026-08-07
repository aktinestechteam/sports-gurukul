import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/token_pair.dart';

/// Contract for authentication operations backed by the backend auth API.
///
/// Implementations in infrastructure talk to the network and local session
/// storage; domain and application layers depend only on this abstraction.
/// Every remote operation returns a [Result]/[OperationResult] so failures
/// flow to the UI as data, never as raw exceptions.
abstract interface class AuthRepository {
  /// Authenticates [email]/[password] and returns the issued session.
  Future<Result<AuthSession>> login({
    required String email,
    required String password,
  });

  /// Registers a new user account and returns the issued session.
  Future<Result<AuthSession>> register({
    required String fullName,
    required String email,
    required String password,
    required String confirmPassword,
    String? phoneNumber,
  });

  /// Rotates [refreshToken] into a fresh token pair.
  Future<Result<TokenPair>> refreshToken(String refreshToken);

  /// Revokes the current user's refresh tokens server-side.
  ///
  /// Requires a valid access token; failures are non-fatal because the local
  /// session is cleared regardless.
  Future<OperationResult> logout();

  /// Requests a password reset email for [email].
  Future<OperationResult> forgotPassword(String email);

  /// Resets the password using the reset [token] from the email.
  Future<OperationResult> resetPassword({
    required String token,
    required String newPassword,
    required String confirmNewPassword,
  });

  /// Requests a verification email for [email].
  Future<OperationResult> sendVerificationEmail(String email);

  /// Confirms an email address using the verification [token] from the email.
  Future<OperationResult> verifyEmail(String token);
}

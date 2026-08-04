import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/authentication/auth_providers.dart';
import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/authentication/application/auth_use_case_providers.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/infrastructure/auth_infrastructure_providers.dart';

/// The authentication lifecycle state.
sealed class AuthState {
  const AuthState();
}

/// Session state has not been resolved yet (splash in progress).
final class AuthUnknown extends AuthState {
  const AuthUnknown();
}

/// The user is signed out.
final class AuthUnauthenticated extends AuthState {
  const AuthUnauthenticated();
}

/// The user holds a live [AuthSession].
final class AuthAuthenticated extends AuthState {
  const AuthAuthenticated(this.session);

  final AuthSession session;
}

/// Owns the authentication lifecycle and exposes it to the UI.
///
/// Restores a cached session on startup (auto-login), signs in/out, and
/// listens for session expiry signalled by the networking layer.
class AuthController extends Notifier<AuthState> {
  StreamSubscription<void>? _sessionExpirySubscription;

  @override
  AuthState build() {
    final events = ref.watch(sessionEventsProvider);
    _sessionExpirySubscription ??= events.onSessionExpired.listen(
      (_) => forceLogout(),
    );
    ref.onDispose(() {
      unawaited(_sessionExpirySubscription?.cancel());
      _sessionExpirySubscription = null;
    });
    return const AuthUnknown();
  }

  /// Restores the cached session (auto-login) or signs the user out.
  ///
  /// An unexpired access token restores the session directly; an expired one
  /// triggers a single refresh attempt before falling back to login.
  Future<void> restoreSession() async {
    final store = ref.read(authSessionStoreProvider);
    final session = await store.read();
    if (session == null) {
      state = const AuthUnauthenticated();
      return;
    }
    if (!session.hasExpiredAccessToken) {
      state = AuthAuthenticated(session);
      return;
    }

    final result = await ref.read(refreshSessionProvider)(session.refreshToken);
    if (result.isSuccess) {
      final updated = session.withTokenPair(result.requireValue());
      await store.write(updated);
      state = AuthAuthenticated(updated);
      return;
    }
    await _clearLocalSession();
    state = const AuthUnauthenticated();
  }

  /// Signs in with [email]/[password] and caches the resulting session.
  Future<Result<AuthSession>> login({
    required String email,
    required String password,
  }) async {
    final result = await ref
        .read(loginUserProvider)
        .call(email: email, password: password);
    if (result is Success<AuthSession>) {
      await ref.read(authSessionStoreProvider).write(result.value);
      state = AuthAuthenticated(result.value);
    }
    return result;
  }

  /// Registers a new account and caches the resulting session.
  Future<Result<AuthSession>> register({
    required String fullName,
    required String email,
    required String password,
    required String confirmPassword,
    String? phoneNumber,
  }) async {
    final result = await ref
        .read(registerUserProvider)
        .call(
          fullName: fullName,
          email: email,
          password: password,
          confirmPassword: confirmPassword,
          phoneNumber: phoneNumber,
        );
    if (result is Success<AuthSession>) {
      await ref.read(authSessionStoreProvider).write(result.value);
      state = AuthAuthenticated(result.value);
    }
    return result;
  }

  /// Signs out locally and best-effort revokes the session server-side.
  Future<OperationResult> logout() async {
    final result = await ref.read(logoutUserProvider).call();
    await _clearLocalSession();
    state = const AuthUnauthenticated();
    return result;
  }

  /// Requests a password reset email for [email].
  Future<OperationResult> forgotPassword(String email) =>
      ref.read(forgotPasswordProvider).call(email);

  /// Resets the password using the token from the reset email.
  Future<OperationResult> resetPassword({
    required String token,
    required String newPassword,
    required String confirmNewPassword,
  }) => ref
      .read(resetPasswordProvider)
      .call(
        token: token,
        newPassword: newPassword,
        confirmNewPassword: confirmNewPassword,
      );

  /// Ends the session without a server round trip (e.g. token expiry).
  Future<void> forceLogout() async {
    await _clearLocalSession();
    state = const AuthUnauthenticated();
  }

  Future<void> _clearLocalSession() async {
    await ref.read(authSessionStoreProvider).clear();
    await ref.read(tokenStoreProvider).clear();
  }
}

/// Provides the authentication controller.
final authControllerProvider = NotifierProvider<AuthController, AuthState>(
  AuthController.new,
);

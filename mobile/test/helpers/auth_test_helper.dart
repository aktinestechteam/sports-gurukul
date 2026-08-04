import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sports_gurukul/app/app.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_user.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';

/// Builds a [AuthSession] for tests.
AuthSession testAuthSession({String email = 'player@example.com'}) =>
    AuthSession(
      user: AuthUser(
        id: 'user-1',
        email: email,
        fullName: 'Test Player',
        roles: const <String>['Player'],
      ),
      accessToken: 'access-token',
      refreshToken: 'refresh-token',
      accessTokenExpiresAt: DateTime.utc(2099),
    );

/// An [AuthController] that reports [initialState] without touching storage.
///
/// Used by bootstrap/golden tests to bypass secure storage and drive the
/// router straight to a given state.
class FakeAuthController extends AuthController {
  FakeAuthController(this.initialState);

  final AuthState initialState;

  @override
  AuthState build() => initialState;

  @override
  Future<void> restoreSession() async {}
}

/// Wraps the real app with [authControllerProvider] pinned to [state].
///
/// Pass `null` to run the app with real providers (splash-only assertions).
Widget buildTestApp({AuthState? state}) => ProviderScope(
  overrides: state == null
      ? const []
      : [
          authControllerProvider.overrideWith(
            () => FakeAuthController(state),
          ),
        ],
  child: const SportsGurukulApp(),
);

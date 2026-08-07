import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sports_gurukul/app/app.dart';
import 'package:sports_gurukul/features/academy/create/application/my_academy_provider.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_session.dart';
import 'package:sports_gurukul/features/authentication/domain/entities/auth_user.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/onboarding/application/onboarding_providers.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/current_user.dart';

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
/// Pass `null` for [state] to run the app with real providers (splash-only
/// assertions). Pass [currentUser] to make onboarding resolution
/// deterministic instead of hitting the real profile endpoint.
Widget buildTestApp({AuthState? state, CurrentUser? currentUser}) {
  final overrides = [
    if (state != null)
      authControllerProvider.overrideWith(() => FakeAuthController(state)),
    if (currentUser != null) ...[
      currentUserProvider.overrideWith((ref) async => currentUser),
      // Keep the academy-dashboard branding fetch offline in tests; tests that
      // assert branding override myAcademyProvider themselves.
      myAcademyProvider.overrideWith((ref) async => null),
    ],
  ];
  return ProviderScope(overrides: overrides, child: const SportsGurukulApp());
}

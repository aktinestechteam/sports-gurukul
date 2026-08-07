import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/onboarding/application/current_user_resolution_exception.dart';
import 'package:sports_gurukul/features/onboarding/application/onboarding_providers.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/application_session.dart';

/// The onboarding lifecycle state.
sealed class OnboardingState {
  const OnboardingState();
}

/// Nothing to resolve yet (signed out).
final class OnboardingIdle extends OnboardingState {
  const OnboardingIdle();
}

/// The current user is being resolved from the backend.
final class OnboardingLoading extends OnboardingState {
  const OnboardingLoading();
}

/// Current-user resolution failed; the UI can retry.
final class OnboardingError extends OnboardingState {
  const OnboardingError(this.failure);

  final BaseFailure failure;
}

/// The application session is ready and drives post-login navigation.
final class OnboardingResolved extends OnboardingState {
  const OnboardingResolved(this.session);

  final ApplicationSession session;
}

/// The user chose an onboarding path; the dashboard is now reachable.
final class OnboardingCompleted extends OnboardingState {
  const OnboardingCompleted(this.session);

  final ApplicationSession session;
}

/// Resolves the application session for the authenticated user.
///
/// Watches the auth lifecycle; once signed in it fetches the current user,
/// classifies the user state, and exposes the assembled [ApplicationSession]
/// to the router guard and the welcome screen. [completeOnboarding] records
/// that a new user picked a path so the guard stops routing them back to the
/// welcome screen.
class OnboardingController extends Notifier<OnboardingState> {
  bool _completed = false;

  @override
  OnboardingState build() {
    final authState = ref.watch(authControllerProvider);
    if (authState is! AuthAuthenticated) return const OnboardingIdle();

    final currentUserAsync = ref.watch(currentUserProvider);
    final session = ref.watch(applicationSessionProvider);

    // Riverpod 3.x surfaces a failed future as a loading value that carries
    // the error, so the error must be read from `AsyncValue.error` rather
    // than relying on `AsyncError` alone.
    final resolutionError = currentUserAsync.error;
    final resolved = switch (currentUserAsync) {
      AsyncData(:final value) when value == null => const OnboardingIdle(),
      AsyncData() when session != null => OnboardingResolved(session),
      _ when resolutionError != null => OnboardingError(
        _toFailure(resolutionError),
      ),
      _ => const OnboardingLoading(),
    };

    if (_completed && resolved is OnboardingResolved) {
      return OnboardingCompleted(resolved.session);
    }
    return resolved;
  }

  /// Re-runs current-user resolution (used by the error retry action).
  void refresh() => ref.invalidate(currentUserProvider);

  /// Marks the onboarding flow as done so the dashboard stays reachable.
  void completeOnboarding() {
    _completed = true;
    final current = state;
    if (current is OnboardingResolved) {
      state = OnboardingCompleted(current.session);
    }
  }

  static BaseFailure _toFailure(Object error) {
    if (error is CurrentUserResolutionException) return error.failure;
    if (error is BaseFailure) return error;
    return UnknownFailure(message: '$error', cause: error);
  }
}

/// Provides the onboarding controller.
final onboardingControllerProvider =
    NotifierProvider<OnboardingController, OnboardingState>(
      OnboardingController.new,
    );

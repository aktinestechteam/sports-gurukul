import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/onboarding/application/current_user_resolution_exception.dart';
import 'package:sports_gurukul/features/onboarding/application/onboarding_providers.dart';
import 'package:sports_gurukul/features/onboarding/presentation/providers/onboarding_controller.dart';

import '../../helpers/auth_test_helper.dart';
import '../../helpers/onboarding_test_helper.dart';

void main() {
  late ProviderContainer container;

  tearDown(() => container.dispose());

  ProviderContainer buildContainer({required bool isNewUser}) {
    final currentUser = isNewUser
        ? testNewUserCurrentUser()
        : testMemberCurrentUser();
    return ProviderContainer(
      overrides: [
        authControllerProvider.overrideWith(
          () => FakeAuthController(AuthAuthenticated(testAuthSession())),
        ),
        currentUserProvider.overrideWith((ref) async => currentUser),
      ],
    );
  }

  test('resolves a brand-new user into OnboardingResolved', () async {
    container = buildContainer(isNewUser: true);
    await container.read(currentUserProvider.future);

    final state = container.read(onboardingControllerProvider);
    expect(state, isA<OnboardingResolved>());
    final session = (state as OnboardingResolved).session;
    expect(session.isNewUser, isTrue);
    expect(session.currentUser.id, 'user-1');
    expect(session.primaryRole, isNotNull);
  });

  test('resolves an academy member into OnboardingResolved', () async {
    container = buildContainer(isNewUser: false);
    await container.read(currentUserProvider.future);

    final state = container.read(onboardingControllerProvider);
    expect(state, isA<OnboardingResolved>());
    expect((state as OnboardingResolved).session.isNewUser, isFalse);
  });

  test('completeOnboarding completes the lifecycle', () async {
    container = buildContainer(isNewUser: true);
    await container.read(currentUserProvider.future);

    container.read(onboardingControllerProvider.notifier).completeOnboarding();

    expect(
      container.read(onboardingControllerProvider),
      isA<OnboardingCompleted>(),
    );
  });

  test('stays idle while signed out', () {
    container = ProviderContainer(
      overrides: [
        authControllerProvider.overrideWith(
          () => FakeAuthController(const AuthUnauthenticated()),
        ),
      ],
    );
    expect(container.read(onboardingControllerProvider), isA<OnboardingIdle>());
  });

  test('surfaces a typed failure when current-user resolution fails', () async {
    container = ProviderContainer(
      overrides: [
        authControllerProvider.overrideWith(
          () => FakeAuthController(AuthAuthenticated(testAuthSession())),
        ),
        currentUserProvider.overrideWith(
          (ref) async => throw const CurrentUserResolutionException(
            NetworkFailure(),
          ),
        ),
      ],
    );

    // Reading the controller kicks off current-user resolution; it is still
    // loading until the overridden provider completes with the failure.
    final initialState = container.read(onboardingControllerProvider);
    expect(initialState, isA<OnboardingLoading>());

    await Future<void>.delayed(Duration.zero);
    await Future<void>.delayed(Duration.zero);

    final resolved = container.read(currentUserProvider);
    expect(resolved.error, isA<CurrentUserResolutionException>());

    final state = container.read(onboardingControllerProvider);
    expect(state, isA<OnboardingError>());
    expect((state as OnboardingError).failure, isA<NetworkFailure>());
  });
}

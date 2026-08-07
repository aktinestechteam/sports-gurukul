import 'dart:io';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/operation_result.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/authentication/presentation/providers/auth_controller.dart';
import 'package:sports_gurukul/features/onboarding/application/current_user_resolution_exception.dart';
import 'package:sports_gurukul/features/onboarding/application/onboarding_providers.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';
import 'package:sports_gurukul/features/onboarding/domain/entities/user_state.dart';
import 'package:sports_gurukul/features/onboarding/presentation/providers/onboarding_controller.dart';
import 'package:sports_gurukul/features/user/application/usecases/get_current_profile.dart';
import 'package:sports_gurukul/features/user/application/user_profile_use_case_providers.dart';
import 'package:sports_gurukul/features/user/domain/entities/profile_photo.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_preference.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/domain/repositories/user_profile_repository.dart';
import 'package:sports_gurukul/features/user/infrastructure/error/user_profile_error_mapper.dart';

import '../../helpers/auth_test_helper.dart';

/// A [UserProfileRepository] that answers [result] for the current-profile
/// call and nothing else.
class _StubUserProfileRepository implements UserProfileRepository {
  _StubUserProfileRepository(this.result);

  final Result<UserProfile> result;

  @override
  Future<Result<UserProfile>> getCurrentProfile() async => result;

  @override
  Future<Result<UserProfile>> updateProfile({
    DateTime? dateOfBirth,
    String? gender,
    String? bio,
    String? height,
    String? weight,
    String? preferredSport,
    String? experienceLevel,
    String? primaryPhoneCountryCode,
    String? primaryPhoneNumber,
    String? addressLine1,
    String? addressLine2,
    String? city,
    String? state,
    String? country,
    String? postalCode,
    String? addressType,
  }) => throw UnimplementedError();

  @override
  Future<Result<UserPreference>> updatePreferences({
    String? language,
    String? theme,
    String? timeZone,
    bool? emailNotifications,
    bool? pushNotifications,
    bool? smsNotifications,
    bool? marketingEmails,
    bool? profileVisibility,
    bool? showOnlineStatus,
  }) => throw UnimplementedError();

  @override
  Future<Result<ProfilePhoto>> uploadProfilePhoto(File imageFile) =>
      throw UnimplementedError();

  @override
  Future<Result<ProfilePhoto>> getProfilePhoto() => throw UnimplementedError();

  @override
  Future<OperationResult> deleteProfilePhoto() => throw UnimplementedError();
}

void main() {
  late ProviderContainer container;

  tearDown(() => container.dispose());

  ProviderContainer buildContainer(Result<UserProfile> profileResult) =>
      ProviderContainer(
        overrides: [
          authControllerProvider.overrideWith(
            () => FakeAuthController(AuthAuthenticated(testAuthSession())),
          ),
          getCurrentProfileProvider.overrideWith(
            (ref) =>
                GetCurrentProfile(_StubUserProfileRepository(profileResult)),
          ),
        ],
      );

  test(
    'a missing profile resolves a brand-new user from the auth session',
    () async {
      container = buildContainer(
        const Result.failure(
          NetworkFailure(
            message: 'Profile not found. Please create your profile first.',
            code: UserProfileErrorCodes.notFound,
          ),
        ),
      );

      final currentUser = await container.read(currentUserProvider.future);

      expect(currentUser, isNotNull);
      expect(currentUser!.id, 'user-1');
      expect(currentUser.fullName, 'Test Player');
      expect(currentUser.email, 'player@example.com');
      expect(currentUser.roles, isEmpty);
      expect(currentUser.hasAcademyAssociation, isFalse);

      expect(
        resolveUserState(
          roles: currentUser.roles,
          hasAcademyAssociation: currentUser.hasAcademyAssociation,
        ),
        UserState.newUser,
      );

      final onboarding = container.read(onboardingControllerProvider);
      expect(onboarding, isA<OnboardingResolved>());
      expect((onboarding as OnboardingResolved).session.isNewUser, isTrue);
    },
  );

  test(
    'a profile-less user carrying a fresh Academy Admin role resolves as an '
    'academy admin',
    () async {
      container = buildContainer(
        Result.success(
          UserProfile(
            id: 'user-1',
            userId: 'user-1',
            fullName: 'Test Player',
            email: 'player@example.com',
            createdAt: DateTime.utc(2026, 2, 3),
            roles: const <String>['Athlete', 'Academy Admin'],
            hasProfile: false,
          ),
        ),
      );

      final currentUser = await container.read(currentUserProvider.future);

      expect(currentUser, isNotNull);
      expect(currentUser!.roles, contains(UserRole.academy));
      expect(
        resolveUserState(
          roles: currentUser.roles,
          hasAcademyAssociation: currentUser.hasAcademyAssociation,
        ),
        UserState.academyAdmin,
      );

      final onboarding = container.read(onboardingControllerProvider);
      expect(onboarding, isA<OnboardingResolved>());
      final session = (onboarding as OnboardingResolved).session;
      expect(session.userState, UserState.academyAdmin);
      expect(session.isNewUser, isFalse);
    },
  );

  test(
    'a profile-less brand-new user with only the default role stays a new user',
    () async {
      container = buildContainer(
        Result.success(
          UserProfile(
            id: 'user-1',
            userId: 'user-1',
            fullName: 'Test Player',
            email: 'player@example.com',
            createdAt: DateTime.utc(2026, 2, 3),
            roles: const <String>['Athlete'],
            hasProfile: false,
          ),
        ),
      );

      final currentUser = await container.read(currentUserProvider.future);

      expect(currentUser, isNotNull);
      expect(
        resolveUserState(
          roles: currentUser!.roles,
          hasAcademyAssociation: currentUser.hasAcademyAssociation,
        ),
        UserState.newUser,
      );

      final onboarding = container.read(onboardingControllerProvider);
      expect(onboarding, isA<OnboardingResolved>());
      expect((onboarding as OnboardingResolved).session.isNewUser, isTrue);
    },
  );

  test('other resolution failures still surface as an error', () async {
    container = buildContainer(
      const Result.failure(
        NetworkFailure(
          message: 'Connection refused',
          code: UserProfileErrorCodes.network,
        ),
      ),
    );

    // Riverpod 3.x keeps a failed future in the loading state and carries the
    // error on `AsyncValue.error`, so the error must be read from a fresh
    // snapshot once the provider has settled rather than awaited as a thrown
    // future. The first read starts the provider; the awaited yields let the
    // failed future settle, and the later reads observe the settled value.
    // ignore: cascade_invocations
    container.read(currentUserProvider);
    await Future<void>.delayed(Duration.zero);
    await Future<void>.delayed(Duration.zero);
    await Future<void>.delayed(Duration.zero);

    final resolved = container.read(currentUserProvider);
    expect(resolved.error, isA<CurrentUserResolutionException>());

    final onboarding = container.read(onboardingControllerProvider);
    expect(onboarding, isA<OnboardingError>());
    expect((onboarding as OnboardingError).failure, isA<NetworkFailure>());
  });
}

import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/app_animation.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/app/theme/typography/app_typography.dart';
import 'package:sports_gurukul/features/user/domain/entities/address.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/presentation/providers/profile_controller.dart';
import 'package:sports_gurukul/features/user/presentation/widgets/profile_photo_picker.dart';
import 'package:sports_gurukul/features/user/presentation/widgets/profile_scaffold.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/animations/entrance.dart';
import 'package:sports_gurukul/shared/animations/spring_press.dart';
import 'package:sports_gurukul/shared/buttons/gradient_button.dart';
import 'package:sports_gurukul/shared/cards/glass_card.dart';

/// Displays the current user's profile with pull-to-refresh, profile photo,
/// key details, preferences and edit actions.
///
/// Pushed from the dashboard so the system back gesture/button returns to it;
/// edit screens are pushed on top so they can pop back to this page.
class MyProfilePage extends ConsumerStatefulWidget {
  const MyProfilePage({super.key});

  @override
  ConsumerState<MyProfilePage> createState() => _MyProfilePageState();
}

class _MyProfilePageState extends ConsumerState<MyProfilePage> {
  @override
  void initState() {
    super.initState();
    unawaited(Future.microtask(() async {
      if (mounted) {
        await ref.read(profileControllerProvider.notifier).loadProfile();
      }
    }));
  }

  Future<void> _onRefresh() =>
      ref.read(profileControllerProvider.notifier).refreshProfile();

  void _onEditProfile() => context.push(RoutePaths.editProfile);

  void _onEditPreferences() => context.push(RoutePaths.editPreferences);

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(profileControllerProvider);

    return ProfileScaffold(
      title: l10n.profileMyProfileTitle,
      child: switch (state) {
        ProfileInitial() || ProfileLoading() => const _LoadingView(),
        ProfileError(:final message, :final canCreate) => _ErrorView(
          message: message,
          onCreatePressed: canCreate ? _onEditProfile : null,
          onRetry: _onRefresh,
        ),
        ProfileLoaded(:final profile) => RefreshIndicator(
          onRefresh: _onRefresh,
          color: AppColors.primary400,
          backgroundColor: AppColors.surfaceDark,
          child: _ProfileContent(
            profile: profile,
            onEditPressed: _onEditProfile,
            onPreferencesPressed: _onEditPreferences,
          ),
        ),
      },
    );
  }
}

/// The loaded profile content: photo header and detail sections.
class _ProfileContent extends StatelessWidget {
  const _ProfileContent({
    required this.profile,
    required this.onEditPressed,
    required this.onPreferencesPressed,
  });

  final UserProfile profile;
  final VoidCallback onEditPressed;
  final VoidCallback onPreferencesPressed;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);

    return ListView(
      physics: const AlwaysScrollableScrollPhysics(),
      padding: const EdgeInsets.fromLTRB(
        AppSpacing.xl,
        AppSpacing.sm,
        AppSpacing.xl,
        AppSpacing.xxxl,
      ),
      children: <Widget>[
        Entrance(
          duration: AppAnimation.entrance,
          offset: const Offset(0, 0.06),
          child: ProfilePhotoPicker(profile: profile),
        ),
        const SizedBox(height: AppSpacing.lg),
        Entrance(
          delay: const Duration(milliseconds: 80),
          duration: AppAnimation.entrance,
          offset: const Offset(0, 0.06),
          child: GlassCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: <Widget>[
                _InfoRow(
                  icon: Icons.person_outline,
                  label: l10n.profileFullName,
                  value: profile.fullName,
                ),
                _InfoRow(
                  icon: Icons.mail_outline,
                  label: l10n.profileEmail,
                  value: profile.email,
                ),
                if (profile.phoneNumber != null)
                  _InfoRow(
                    icon: Icons.phone_outlined,
                    label: l10n.profilePhone,
                    value: profile.phoneNumber!,
                  ),
                if (profile.preferredSport != null)
                  _InfoRow(
                    icon: Icons.sports_basketball_outlined,
                    label: l10n.profilePreferredSport,
                    value: profile.preferredSport!,
                  ),
                if (profile.experienceLevel != null)
                  _InfoRow(
                    icon: Icons.trending_up,
                    label: l10n.profileExperienceLevel,
                    value: profile.experienceLevel!,
                  ),
                if (profile.height != null)
                  _InfoRow(
                    icon: Icons.height,
                    label: l10n.profileHeight,
                    value: profile.height!,
                  ),
                if (profile.weight != null)
                  _InfoRow(
                    icon: Icons.monitor_weight_outlined,
                    label: l10n.profileWeight,
                    value: profile.weight!,
                  ),
                const SizedBox(height: AppSpacing.lg),
                GradientButton(
                  label: l10n.profileEditProfile,
                  icon: Icons.edit_outlined,
                  gradient: AppGradients.bluePurpleHorizontal,
                  onPressed: onEditPressed,
                  borderRadius: BorderRadius.circular(AppRadius.input),
                ),
              ],
            ),
          ),
        ),
        const SizedBox(height: AppSpacing.lg),
        Entrance(
          delay: const Duration(milliseconds: 160),
          duration: AppAnimation.entrance,
          offset: const Offset(0, 0.06),
          child: GlassCard(
            child: SpringPress(
              onPressed: onPreferencesPressed,
              child: Row(
                children: <Widget>[
                  Container(
                    width: 38,
                    height: 38,
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: AppColors.primary500.withValues(alpha: 0.18),
                    ),
                    child: const Icon(
                      Icons.tune_rounded,
                      color: AppColors.primary400,
                      size: 20,
                    ),
                  ),
                  const SizedBox(width: AppSpacing.md),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: <Widget>[
                        Text(
                          l10n.profilePreferencesTitle,
                          style: const TextStyle(
                            fontSize: AppTypography.bodyLarge,
                            fontWeight: AppTypography.semiBold,
                            color: AppColors.surface,
                          ),
                        ),
                        const SizedBox(height: 2),
                        Text(
                          l10n.profilePreferencesSubtitle,
                          style: const TextStyle(
                            fontSize: AppTypography.bodySmall,
                            color: AppColors.grey300,
                            height: 1.3,
                          ),
                        ),
                      ],
                    ),
                  ),
                  const Icon(
                    Icons.chevron_right_rounded,
                    color: AppColors.grey400,
                  ),
                ],
              ),
            ),
          ),
        ),
        if (profile.bio != null && profile.bio!.isNotEmpty) ...<Widget>[
          const SizedBox(height: AppSpacing.lg),
          Entrance(
            delay: const Duration(milliseconds: 200),
            duration: AppAnimation.entrance,
            offset: const Offset(0, 0.06),
            child: GlassCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: <Widget>[
                  Text(
                    l10n.profileAboutMe,
                    style: const TextStyle(
                      fontSize: AppTypography.headingSm,
                      fontWeight: AppTypography.semiBold,
                      color: AppColors.surface,
                    ),
                  ),
                  const SizedBox(height: AppSpacing.sm),
                  Text(
                    profile.bio!,
                    style: const TextStyle(
                      fontSize: AppTypography.bodyMedium,
                      color: AppColors.grey300,
                      height: 1.5,
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
        if (profile.addresses.isNotEmpty) ...<Widget>[
          const SizedBox(height: AppSpacing.lg),
          Entrance(
            delay: const Duration(milliseconds: 240),
            duration: AppAnimation.entrance,
            offset: const Offset(0, 0.06),
            child: GlassCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: <Widget>[
                  Text(
                    l10n.profileAddresses,
                    style: const TextStyle(
                      fontSize: AppTypography.headingSm,
                      fontWeight: AppTypography.semiBold,
                      color: AppColors.surface,
                    ),
                  ),
                  const SizedBox(height: AppSpacing.sm),
                  for (final address in profile.addresses) ...<Widget>[
                    _AddressTile(
                      address: address,
                      isLast: address == profile.addresses.last,
                    ),
                  ],
                ],
              ),
            ),
          ),
        ],
      ],
    );
  }
}

/// A single labelled info row (icon + label + value).
class _InfoRow extends StatelessWidget {
  const _InfoRow({
    required this.icon,
    required this.label,
    required this.value,
  });

  final IconData icon;
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.sm),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          Icon(icon, size: 20, color: AppColors.primary400),
          const SizedBox(width: AppSpacing.md),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                Text(
                  label,
                  style: const TextStyle(
                    fontSize: AppTypography.bodySmall,
                    color: AppColors.grey300,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  value,
                  style: const TextStyle(
                    fontSize: AppTypography.bodyLarge,
                    fontWeight: AppTypography.medium,
                    color: AppColors.surface,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

/// A single address entry with a separator.
class _AddressTile extends StatelessWidget {
  const _AddressTile({required this.address, required this.isLast});

  final Address address;
  final bool isLast;

  @override
  Widget build(BuildContext context) {
    final lines = <String>[
      address.line1,
      ?address.line2,
      '${address.city}, ${address.state} ${address.postalCode ?? ''}'.trim(),
      address.country,
    ];
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.sm),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          Row(
            children: <Widget>[
              const Icon(
                Icons.location_on_outlined,
                size: 20,
                color: AppColors.primary400,
              ),
              const SizedBox(width: AppSpacing.sm),
              Expanded(
                child: Text(
                  lines.join('\n'),
                  style: const TextStyle(
                    fontSize: AppTypography.bodyMedium,
                    color: AppColors.surface,
                    height: 1.4,
                  ),
                ),
              ),
            ],
          ),
          if (!isLast)
            const Padding(
              padding: EdgeInsets.only(top: AppSpacing.sm),
              child: Divider(height: 1, color: AppColors.whiteBorder),
            ),
        ],
      ),
    );
  }
}

/// Dark-friendly centered loading indicator.
class _LoadingView extends StatelessWidget {
  const _LoadingView();

  @override
  Widget build(BuildContext context) {
    return const Center(
      child: CircularProgressIndicator(color: AppColors.primary400),
    );
  }
}

/// Dark-friendly error state with optional create-profile action and retry.
class _ErrorView extends StatelessWidget {
  const _ErrorView({
    required this.message,
    required this.onCreatePressed,
    required this.onRetry,
  });

  final String message;
  final VoidCallback? onCreatePressed;
  final VoidCallback onRetry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(AppSpacing.xl),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: <Widget>[
            const Icon(
              Icons.error_outline,
              size: AppSpacing.huge,
              color: AppColors.danger,
            ),
            const SizedBox(height: AppSpacing.lg),
            Text(
              l10n.profileErrorTitle,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: AppTypography.headingSm,
                fontWeight: AppTypography.semiBold,
                color: AppColors.surface,
              ),
            ),
            const SizedBox(height: AppSpacing.sm),
            Text(
              message,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: AppTypography.bodyMedium,
                color: AppColors.grey300,
                height: 1.4,
              ),
            ),
            if (onCreatePressed != null) ...<Widget>[
              const SizedBox(height: AppSpacing.xxl),
              GradientButton(
                label: l10n.profileCreateProfile,
                gradient: AppGradients.bluePurpleHorizontal,
                onPressed: onCreatePressed,
                borderRadius: BorderRadius.circular(AppRadius.input),
              ),
            ],
            const SizedBox(height: AppSpacing.lg),
            GradientButton(
              label: l10n.profileRetry,
              onPressed: onRetry,
              borderRadius: BorderRadius.circular(AppRadius.input),
            ),
          ],
        ),
      ),
    );
  }
}

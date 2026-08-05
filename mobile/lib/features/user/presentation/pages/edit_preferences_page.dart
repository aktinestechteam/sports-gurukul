import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/theme/app_animation.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/app/theme/typography/app_typography.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_preference.dart';
import 'package:sports_gurukul/features/user/presentation/providers/profile_controller.dart';
import 'package:sports_gurukul/features/user/presentation/widgets/profile_dropdown_field.dart';
import 'package:sports_gurukul/features/user/presentation/widgets/profile_messages.dart';
import 'package:sports_gurukul/features/user/presentation/widgets/profile_scaffold.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/animations/entrance.dart';
import 'package:sports_gurukul/shared/buttons/gradient_button.dart';
import 'package:sports_gurukul/shared/cards/glass_card.dart';

/// Form for editing the current user's notification and appearance
/// preferences via `PUT /api/v1/users/preferences`.
///
/// Rendered on the shared dark [ProfileScaffold]; on success the screen pops
/// back and confirms via snackbar.
class EditPreferencesPage extends ConsumerStatefulWidget {
  const EditPreferencesPage({super.key});

  @override
  ConsumerState<EditPreferencesPage> createState() =>
      _EditPreferencesPageState();
}

class _EditPreferencesPageState extends ConsumerState<EditPreferencesPage> {
  String _language = 'en';
  String _theme = 'system';
  bool _emailNotifications = true;
  bool _pushNotifications = true;
  bool _smsNotifications = false;
  bool _marketingEmails = false;
  bool _profileVisibility = true;
  bool _showOnlineStatus = true;
  bool _saving = false;

  UserPreference? get _preferences {
    final state = ref.read(profileControllerProvider);
    if (state is ProfileLoaded && state.profile.preferences != null) {
      return state.profile.preferences;
    }
    return null;
  }

  @override
  void initState() {
    super.initState();
    unawaited(Future.microtask(() {
      if (!mounted) {
        return;
      }
      final preferences = _preferences;
      if (preferences == null) {
        return;
      }
      setState(() {
        _language = preferences.language;
        _theme = preferences.theme.name;
        _emailNotifications = preferences.emailNotifications;
        _pushNotifications = preferences.pushNotifications;
        _smsNotifications = preferences.smsNotifications;
        _marketingEmails = preferences.marketingEmails;
        _profileVisibility = preferences.profileVisibility;
        _showOnlineStatus = preferences.showOnlineStatus;
      });
    }));
  }

  Future<void> _save() async {
    setState(() => _saving = true);
    final result = await ref
        .read(profileControllerProvider.notifier)
        .updatePreferences(
          language: _language,
          theme: _theme,
          emailNotifications: _emailNotifications,
          pushNotifications: _pushNotifications,
          smsNotifications: _smsNotifications,
          marketingEmails: _marketingEmails,
          profileVisibility: _profileVisibility,
          showOnlineStatus: _showOnlineStatus,
        );
    if (!mounted) {
      return;
    }
    setState(() => _saving = false);
    result.when(
      onSuccess: () {
        final l10n = AppLocalizations.of(context);
        final messenger = ScaffoldMessenger.of(context);
        context.pop();
        messenger.showSnackBar(
          SnackBar(content: Text(l10n.profilePreferencesSaved)),
        );
      },
      onFailure: (failure) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(ProfileMessages.failure(context, failure))),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    return ProfileScaffold(
      title: l10n.profilePreferencesTitle,
      subtitle: l10n.profilePreferencesSubtitle,
      child: ListView(
        keyboardDismissBehavior: ScrollViewKeyboardDismissBehavior.onDrag,
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
            child: GlassCard(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: <Widget>[
                  _SectionHeader(
                    icon: Icons.palette_outlined,
                    title: l10n.profilePreferencesAppearance,
                  ),
                  const SizedBox(height: AppSpacing.lg),
                  ProfileDropdownField<String>(
                    label: l10n.profilePreferencesTheme,
                    icon: Icons.brightness_6_outlined,
                    value: _theme,
                    enabled: !_saving,
                    items: <String>['light', 'dark', 'system'].map((value) {
                      return DropdownMenuItem<String>(
                        value: value,
                        child: Text(_themeLabel(context, value)),
                      );
                    }).toList(),
                    onChanged: (value) =>
                        setState(() => _theme = value ?? 'system'),
                  ),
                ],
              ),
            ),
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
                  _SectionHeader(
                    icon: Icons.notifications_none_rounded,
                    title: l10n.profilePreferencesNotifications,
                  ),
                  const SizedBox(height: AppSpacing.sm),
                  _SwitchTile(
                    title: l10n.profilePreferenceEmailNotifications,
                    icon: Icons.mail_outline,
                    value: _emailNotifications,
                    enabled: !_saving,
                    onChanged: (value) =>
                        setState(() => _emailNotifications = value),
                  ),
                  const Divider(height: 1, color: AppColors.whiteBorder),
                  _SwitchTile(
                    title: l10n.profilePreferencePushNotifications,
                    icon: Icons.notifications_active_outlined,
                    value: _pushNotifications,
                    enabled: !_saving,
                    onChanged: (value) =>
                        setState(() => _pushNotifications = value),
                  ),
                  const Divider(height: 1, color: AppColors.whiteBorder),
                  _SwitchTile(
                    title: l10n.profilePreferenceSmsNotifications,
                    icon: Icons.sms_outlined,
                    value: _smsNotifications,
                    enabled: !_saving,
                    onChanged: (value) =>
                        setState(() => _smsNotifications = value),
                  ),
                  const Divider(height: 1, color: AppColors.whiteBorder),
                  _SwitchTile(
                    title: l10n.profilePreferenceMarketingEmails,
                    icon: Icons.campaign_outlined,
                    value: _marketingEmails,
                    enabled: !_saving,
                    onChanged: (value) =>
                        setState(() => _marketingEmails = value),
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
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: <Widget>[
                  _SectionHeader(
                    icon: Icons.lock_outline,
                    title: l10n.profilePreferencesPrivacy,
                  ),
                  const SizedBox(height: AppSpacing.sm),
                  _SwitchTile(
                    title: l10n.profilePreferenceProfileVisibility,
                    icon: Icons.visibility_outlined,
                    value: _profileVisibility,
                    enabled: !_saving,
                    onChanged: (value) =>
                        setState(() => _profileVisibility = value),
                  ),
                  const Divider(height: 1, color: AppColors.whiteBorder),
                  _SwitchTile(
                    title: l10n.profilePreferenceShowOnlineStatus,
                    icon: Icons.people_outline,
                    value: _showOnlineStatus,
                    enabled: !_saving,
                    onChanged: (value) =>
                        setState(() => _showOnlineStatus = value),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: AppSpacing.xxxl),
          Entrance(
            delay: const Duration(milliseconds: 240),
            duration: AppAnimation.entrance,
            offset: const Offset(0, 0.06),
            child: GradientButton(
              label: l10n.profileSave,
              icon: Icons.check_rounded,
              gradient: AppGradients.bluePurpleHorizontal,
              onPressed: _saving ? null : _save,
              loading: _saving,
              borderRadius: BorderRadius.circular(AppRadius.input),
            ),
          ),
        ],
      ),
    );
  }

  String _themeLabel(BuildContext context, String value) {
    final l10n = AppLocalizations.of(context);
    return switch (value) {
      'light' => l10n.profileThemeLight,
      'dark' => l10n.profileThemeDark,
      _ => l10n.profileThemeSystem,
    };
  }
}

/// A labelled switch row used inside the preference cards.
class _SwitchTile extends StatelessWidget {
  const _SwitchTile({
    required this.title,
    required this.icon,
    required this.value,
    required this.enabled,
    required this.onChanged,
  });

  final String title;
  final IconData icon;
  final bool value;
  final bool enabled;
  final ValueChanged<bool> onChanged;

  @override
  Widget build(BuildContext context) {
    return SwitchListTile(
      contentPadding: EdgeInsets.zero,
      secondary: Icon(icon, color: AppColors.primary400, size: 20),
      title: Text(
        title,
        style: const TextStyle(
          fontSize: AppTypography.bodyMedium,
          fontWeight: AppTypography.medium,
          color: AppColors.surface,
        ),
      ),
      value: value,
      activeTrackColor: AppColors.primary500,
      onChanged: enabled ? onChanged : null,
    );
  }
}

/// Section heading with a tinted icon, used inside preference glass cards.
class _SectionHeader extends StatelessWidget {
  const _SectionHeader({required this.icon, required this.title});

  final IconData icon;
  final String title;

  @override
  Widget build(BuildContext context) {
    return Row(
      children: <Widget>[
        Container(
          width: 38,
          height: 38,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: AppColors.primary500.withValues(alpha: 0.18),
          ),
          child: Icon(icon, color: AppColors.primary400, size: 20),
        ),
        const SizedBox(width: AppSpacing.md),
        Expanded(
          child: Text(
            title,
            style: const TextStyle(
              fontSize: AppTypography.headingSm,
              fontWeight: AppTypography.semiBold,
              color: AppColors.surface,
            ),
          ),
        ),
      ],
    );
  }
}

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/theme/app_animation.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/colors/app_gradients.dart';
import 'package:sports_gurukul/app/theme/radius/app_radius.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/app/theme/typography/app_typography.dart';
import 'package:sports_gurukul/core/validators/phone_validator.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_profile.dart';
import 'package:sports_gurukul/features/user/presentation/providers/profile_controller.dart';
import 'package:sports_gurukul/features/user/presentation/widgets/profile_dropdown_field.dart';
import 'package:sports_gurukul/features/user/presentation/widgets/profile_messages.dart';
import 'package:sports_gurukul/features/user/presentation/widgets/profile_scaffold.dart';
import 'package:sports_gurukul/features/user/presentation/widgets/profile_text_field.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/animations/entrance.dart';
import 'package:sports_gurukul/shared/buttons/gradient_button.dart';
import 'package:sports_gurukul/shared/cards/glass_card.dart';

/// Form for creating or editing the current user's profile.
///
/// Rendered on the shared dark [ProfileScaffold] with glass cards and the
/// authentication-style input fields. Fields are pre-populated from the loaded
/// profile, validated before submit, and only non-empty changes are sent. On
/// success the screen pops back to the profile page and confirms via snackbar.
class EditProfilePage extends ConsumerStatefulWidget {
  const EditProfilePage({super.key});

  @override
  ConsumerState<EditProfilePage> createState() => _EditProfilePageState();
}

class _EditProfilePageState extends ConsumerState<EditProfilePage> {
  final _formKey = GlobalKey<FormState>();

  late final TextEditingController _bioController;
  late final TextEditingController _heightController;
  late final TextEditingController _weightController;
  late final TextEditingController _preferredSportController;
  late final TextEditingController _experienceLevelController;
  late final TextEditingController _primaryPhoneCountryCodeController;
  late final TextEditingController _primaryPhoneNumberController;
  late final TextEditingController _addressLine1Controller;
  late final TextEditingController _addressLine2Controller;
  late final TextEditingController _cityController;
  late final TextEditingController _stateController;
  late final TextEditingController _countryController;
  late final TextEditingController _postalCodeController;
  late final TextEditingController _dateOfBirthController;

  String _gender = 'preferNotToSay';
  String _addressType = 'home';
  bool _saving = false;

  UserProfile? get _profile =>
      (ref.read(profileControllerProvider) is ProfileLoaded)
          ? (ref.read(profileControllerProvider) as ProfileLoaded).profile
          : null;

  @override
  void initState() {
    super.initState();
    final profile = _profile;
    _bioController = TextEditingController(text: profile?.bio);
    _heightController = TextEditingController(text: profile?.height);
    _weightController = TextEditingController(text: profile?.weight);
    _preferredSportController =
        TextEditingController(text: profile?.preferredSport);
    _experienceLevelController =
        TextEditingController(text: profile?.experienceLevel);
    _primaryPhoneCountryCodeController = TextEditingController(
      text: profile?.contactInformation?.primaryPhoneCountryCode,
    );
    _primaryPhoneNumberController = TextEditingController(
      text: profile?.contactInformation?.primaryPhoneNumber,
    );
    final address = profile?.addresses.isNotEmpty == true
        ? profile!.addresses.first
        : null;
    _addressLine1Controller = TextEditingController(text: address?.line1);
    _addressLine2Controller = TextEditingController(text: address?.line2);
    _cityController = TextEditingController(text: address?.city);
    _stateController = TextEditingController(text: address?.state);
    _countryController = TextEditingController(text: address?.country);
    _postalCodeController = TextEditingController(text: address?.postalCode);
    _dateOfBirthController = TextEditingController(
      text: profile?.dateOfBirth?.toIso8601String().split('T').first,
    );
    if (profile != null) {
      _gender = profile.gender.name;
      _addressType = address?.addressType.name ?? 'home';
    }
  }

  @override
  void dispose() {
    _bioController.dispose();
    _heightController.dispose();
    _weightController.dispose();
    _preferredSportController.dispose();
    _experienceLevelController.dispose();
    _primaryPhoneCountryCodeController.dispose();
    _primaryPhoneNumberController.dispose();
    _addressLine1Controller.dispose();
    _addressLine2Controller.dispose();
    _cityController.dispose();
    _stateController.dispose();
    _countryController.dispose();
    _postalCodeController.dispose();
    _dateOfBirthController.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    FocusScope.of(context).unfocus();
    if (!_formKey.currentState!.validate()) {
      return;
    }
    setState(() => _saving = true);
    final result = await ref
        .read(profileControllerProvider.notifier)
        .updateProfile(
          bio: _bioController.text.trim().isEmpty
              ? null
              : _bioController.text.trim(),
          height: _heightController.text.trim().isEmpty
              ? null
              : _heightController.text.trim(),
          weight: _weightController.text.trim().isEmpty
              ? null
              : _weightController.text.trim(),
          preferredSport: _preferredSportController.text.trim().isEmpty
              ? null
              : _preferredSportController.text.trim(),
          experienceLevel: _experienceLevelController.text.trim().isEmpty
              ? null
              : _experienceLevelController.text.trim(),
          primaryPhoneCountryCode:
              _primaryPhoneCountryCodeController.text.trim().isEmpty
              ? null
              : _primaryPhoneCountryCodeController.text.trim(),
          primaryPhoneNumber: _primaryPhoneNumberController.text.trim().isEmpty
              ? null
              : _primaryPhoneNumberController.text.trim(),
          addressLine1: _addressLine1Controller.text.trim().isEmpty
              ? null
              : _addressLine1Controller.text.trim(),
          addressLine2: _addressLine2Controller.text.trim().isEmpty
              ? null
              : _addressLine2Controller.text.trim(),
          city: _cityController.text.trim().isEmpty
              ? null
              : _cityController.text.trim(),
          region: _stateController.text.trim().isEmpty
              ? null
              : _stateController.text.trim(),
          country: _countryController.text.trim().isEmpty
              ? null
              : _countryController.text.trim(),
          postalCode: _postalCodeController.text.trim().isEmpty
              ? null
              : _postalCodeController.text.trim(),
          gender: _gender,
          addressType: _addressType,
          dateOfBirth: DateTime.tryParse(_dateOfBirthController.text.trim()),
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
          SnackBar(content: Text(l10n.profileUpdateSuccess)),
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
    final isCreating = _profile == null;
    return ProfileScaffold(
      title: isCreating ? l10n.profileCreateTitle : l10n.profileEditTitle,
      subtitle: isCreating
          ? l10n.profileCreateSubtitle
          : l10n.profileEditSubtitle,
      child: Form(
        key: _formKey,
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
                      icon: Icons.person_outline,
                      title: l10n.profileEditBasicInfo,
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    ProfileDropdownField<String>(
                      label: l10n.profileGender,
                      icon: Icons.wc_outlined,
                      value: _gender,
                      enabled: !_saving,
                      items: _genderOptions(context),
                      onChanged: (value) =>
                          setState(() => _gender = value ?? 'preferNotToSay'),
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    ProfileTextField(
                      controller: _bioController,
                      label: l10n.profileBio,
                      icon: Icons.notes_outlined,
                      maxLines: 4,
                      textInputAction: TextInputAction.newline,
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: <Widget>[
                        Expanded(
                          child: ProfileTextField(
                            controller: _heightController,
                            label: l10n.profileHeight,
                            icon: Icons.height,
                            keyboardType: const TextInputType.numberWithOptions(
                              decimal: true,
                            ),
                            validator: (value) => ProfileMessages.validation(
                              context,
                              _validateNumber(value),
                            ),
                          ),
                        ),
                        const SizedBox(width: AppSpacing.md),
                        Expanded(
                          child: ProfileTextField(
                            controller: _weightController,
                            label: l10n.profileWeight,
                            icon: Icons.monitor_weight_outlined,
                            keyboardType: const TextInputType.numberWithOptions(
                              decimal: true,
                            ),
                            validator: (value) => ProfileMessages.validation(
                              context,
                              _validateNumber(value),
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    ProfileTextField(
                      controller: _preferredSportController,
                      label: l10n.profilePreferredSport,
                      icon: Icons.sports_basketball_outlined,
                      textInputAction: TextInputAction.next,
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    ProfileTextField(
                      controller: _experienceLevelController,
                      label: l10n.profileExperienceLevel,
                      icon: Icons.trending_up,
                      textInputAction: TextInputAction.next,
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    ProfileTextField(
                      controller: _dateOfBirthController,
                      label: l10n.profileDateOfBirth,
                      icon: Icons.cake_outlined,
                      keyboardType: TextInputType.datetime,
                      textInputAction: TextInputAction.next,
                      validator: (value) => ProfileMessages.validation(
                        context,
                        _validateDateOfBirth(value),
                      ),
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
                      icon: Icons.contact_phone_outlined,
                      title: l10n.profileEditContactInfo,
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: <Widget>[
                        SizedBox(
                          width: 132,
                          child: ProfileTextField(
                            controller: _primaryPhoneCountryCodeController,
                            label: l10n.profileCountryCode,
                            prefixText: '+',
                            keyboardType: TextInputType.phone,
                            textInputAction: TextInputAction.next,
                          ),
                        ),
                        const SizedBox(width: AppSpacing.md),
                        Expanded(
                          child: ProfileTextField(
                            controller: _primaryPhoneNumberController,
                            label: l10n.profilePhone,
                            icon: Icons.phone_outlined,
                            keyboardType: TextInputType.phone,
                            textInputAction: TextInputAction.done,
                            validator: (value) => ProfileMessages.validation(
                              context,
                              const PhoneValidator().validate(value),
                            ),
                          ),
                        ),
                      ],
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
                      icon: Icons.location_on_outlined,
                      title: l10n.profileEditAddress,
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    ProfileDropdownField<String>(
                      label: l10n.profileAddressType,
                      icon: Icons.home_outlined,
                      value: _addressType,
                      enabled: !_saving,
                      items: _addressTypeOptions(context),
                      onChanged: (value) =>
                          setState(() => _addressType = value ?? 'home'),
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    ProfileTextField(
                      controller: _addressLine1Controller,
                      label: l10n.profileAddressLine1,
                      icon: Icons.place_outlined,
                      textInputAction: TextInputAction.next,
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    ProfileTextField(
                      controller: _addressLine2Controller,
                      label: l10n.profileAddressLine2,
                      textInputAction: TextInputAction.next,
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: <Widget>[
                        Expanded(
                          child: ProfileTextField(
                            controller: _cityController,
                            label: l10n.profileCity,
                            textInputAction: TextInputAction.next,
                          ),
                        ),
                        const SizedBox(width: AppSpacing.md),
                        Expanded(
                          child: ProfileTextField(
                            controller: _stateController,
                            label: l10n.profileState,
                            textInputAction: TextInputAction.next,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: AppSpacing.lg),
                    Row(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: <Widget>[
                        Expanded(
                          child: ProfileTextField(
                            controller: _countryController,
                            label: l10n.profileCountry,
                            textInputAction: TextInputAction.next,
                          ),
                        ),
                        const SizedBox(width: AppSpacing.md),
                        Expanded(
                          child: ProfileTextField(
                            controller: _postalCodeController,
                            label: l10n.profilePostalCode,
                            keyboardType: TextInputType.streetAddress,
                            textInputAction: TextInputAction.done,
                            validator: (value) => ProfileMessages.validation(
                              context,
                              _validatePostalCode(value),
                            ),
                          ),
                        ),
                      ],
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
      ),
    );
  }

  ValidationError? _validateNumber(String? value) {
    if (value == null || value.trim().isEmpty) {
      return null;
    }
    return num.tryParse(value.trim()) == null
        ? const ValidationError('validation.number.invalid')
        : null;
  }

  ValidationError? _validateDateOfBirth(String? value) {
    if (value == null || value.trim().isEmpty) {
      return null;
    }
    final parsed = DateTime.tryParse(value.trim());
    if (parsed == null) {
      return const ValidationError('validation.date.invalid');
    }
    if (parsed.isAfter(DateTime.now())) {
      return const ValidationError('validation.date.future');
    }
    return null;
  }

  ValidationError? _validatePostalCode(String? value) {
    if (value == null || value.trim().isEmpty) {
      return null;
    }
    final valid = RegExp(r'^[A-Za-z0-9][A-Za-z0-9 -]{2,9}$');
    return valid.hasMatch(value.trim())
        ? null
        : const ValidationError('validation.postalCode.invalid');
  }

  List<DropdownMenuItem<String>> _genderOptions(BuildContext context) {
    return <String>[
      'male',
      'female',
      'nonBinary',
      'preferNotToSay',
    ].map((value) {
      return DropdownMenuItem<String>(
        value: value,
        child: Text(_genderLabel(context, value)),
      );
    }).toList();
  }

  List<DropdownMenuItem<String>> _addressTypeOptions(BuildContext context) {
    return <String>[
      'home',
      'work',
      'academy',
      'other',
    ].map((value) {
      return DropdownMenuItem<String>(
        value: value,
        child: Text(_addressTypeLabel(context, value)),
      );
    }).toList();
  }

  String _genderLabel(BuildContext context, String value) {
    final l10n = AppLocalizations.of(context);
    return switch (value) {
      'male' => l10n.profileGenderMale,
      'female' => l10n.profileGenderFemale,
      'nonBinary' => l10n.profileGenderNonBinary,
      _ => l10n.profileGenderPreferNotToSay,
    };
  }

  String _addressTypeLabel(BuildContext context, String value) {
    final l10n = AppLocalizations.of(context);
    return switch (value) {
      'home' => l10n.profileAddressTypeHome,
      'work' => l10n.profileAddressTypeWork,
      'academy' => l10n.profileAddressTypeAcademy,
      _ => l10n.profileAddressTypeOther,
    };
  }
}

/// Section heading with a tinted icon, used inside profile glass cards.
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

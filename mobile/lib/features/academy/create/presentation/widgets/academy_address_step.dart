import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/core/validators/required_validator.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/core/validators/validator.dart';
import 'package:sports_gurukul/features/academy/create/presentation/academy_create_messages.dart';
import 'package:sports_gurukul/features/academy/create/presentation/create_academy_draft.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/academy_wizard_controller.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_text_field.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Step 3: academy address.
///
/// Persisted on the backend contact record via `PUT /api/v1/academies/{id}/
/// contact` (create flow carries the same fields on `CreateAcademyRequest`).
class AcademyAddressStep extends StatelessWidget {
  const AcademyAddressStep({
    required this.draft,
    required this.controller,
    super.key,
  });

  final CreateAcademyDraft draft;
  final AcademyWizardController controller;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: <Widget>[
        AcademyTextField(
          label: l10n.academyCountryLabel,
          icon: Icons.public_rounded,
          initialValue: draft.country,
          textInputAction: TextInputAction.next,
          onChanged: (value) => controller.updateAddress(country: value),
          validator: (value) => _validation(
            context,
            const CompositeValidator<String>([
              RequiredValidator<String>(),
            ]).validate(value),
          ),
        ),
        const SizedBox(height: AppSpacing.lg),
        AcademyTextField(
          label: l10n.academyStateLabel,
          icon: Icons.map_outlined,
          initialValue: draft.state,
          textInputAction: TextInputAction.next,
          onChanged: (value) => controller.updateAddress(stateName: value),
          validator: (value) => _validation(
            context,
            const CompositeValidator<String>([
              RequiredValidator<String>(),
            ]).validate(value),
          ),
        ),
        const SizedBox(height: AppSpacing.lg),
        AcademyTextField(
          label: l10n.academyCityLabel,
          icon: Icons.location_city_rounded,
          initialValue: draft.city,
          textInputAction: TextInputAction.next,
          onChanged: (value) => controller.updateAddress(city: value),
          validator: (value) => _validation(
            context,
            const CompositeValidator<String>([
              RequiredValidator<String>(),
            ]).validate(value),
          ),
        ),
        const SizedBox(height: AppSpacing.lg),
        AcademyTextField(
          label: l10n.academyAddressLineLabel,
          icon: Icons.home_work_outlined,
          initialValue: draft.addressLine,
          maxLines: 2,
          textInputAction: TextInputAction.next,
          onChanged: (value) => controller.updateAddress(addressLine: value),
          validator: (value) => _validation(
            context,
            const CompositeValidator<String>([
              RequiredValidator<String>(),
            ]).validate(value),
          ),
        ),
        const SizedBox(height: AppSpacing.lg),
        AcademyTextField(
          label: l10n.academyPostalCodeLabel,
          icon: Icons.mail_outline_rounded,
          initialValue: draft.postalCode,
          keyboardType: TextInputType.text,
          onChanged: (value) => controller.updateAddress(postalCode: value),
          validator: (value) => _validateOptionalPostalCode(context, value),
        ),
      ],
    );
  }

  /// Localized error for [value], or null when empty or a valid postal code.
  String? _validateOptionalPostalCode(BuildContext context, String? value) {
    final candidate = value?.trim();
    if (candidate == null || candidate.isEmpty) {
      return null;
    }
    final valid = RegExp(r'^[A-Za-z0-9][A-Za-z0-9 -]{2,9}$');
    if (!valid.hasMatch(candidate)) {
      return AcademyCreateMessages.validation(
        context,
        const ValidationError('validation.postalCode.invalid'),
      );
    }
    return null;
  }
}

/// Resolves the localized error text, returning null when valid.
String? _validation(BuildContext context, ValidationError? error) =>
    AcademyCreateMessages.validation(context, error);

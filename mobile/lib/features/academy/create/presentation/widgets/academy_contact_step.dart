import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/core/constants/regex_constants.dart';
import 'package:sports_gurukul/core/validators/email_validator.dart';
import 'package:sports_gurukul/core/validators/phone_validator.dart';
import 'package:sports_gurukul/core/validators/required_validator.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/core/validators/validator.dart';
import 'package:sports_gurukul/features/academy/create/presentation/academy_create_messages.dart';
import 'package:sports_gurukul/features/academy/create/presentation/create_academy_draft.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/academy_wizard_controller.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_text_field.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Step 2: contact person, email, mobile and optional website.
class AcademyContactStep extends StatelessWidget {
  const AcademyContactStep({
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
          label: l10n.academyContactPersonLabel,
          icon: Icons.person_outline_rounded,
          initialValue: draft.contactPerson,
          textInputAction: TextInputAction.next,
          onChanged: (value) => controller.updateContact(contactPerson: value),
          validator: (value) => _validation(
            context,
            const CompositeValidator<String>([
              RequiredValidator<String>(),
            ]).validate(value),
          ),
        ),
        const SizedBox(height: AppSpacing.lg),
        AcademyTextField(
          label: l10n.academyEmailLabel,
          icon: Icons.mail_outline_rounded,
          initialValue: draft.email,
          keyboardType: TextInputType.emailAddress,
          textInputAction: TextInputAction.next,
          onChanged: (value) => controller.updateContact(email: value),
          validator: (value) => _validation(
            context,
            const CompositeValidator<String>([
              RequiredValidator<String>(),
              EmailValidator(),
            ]).validate(value),
          ),
        ),
        const SizedBox(height: AppSpacing.lg),
        AcademyTextField(
          label: l10n.academyPhoneLabel,
          icon: Icons.phone_outlined,
          initialValue: draft.phone,
          keyboardType: TextInputType.phone,
          textInputAction: TextInputAction.next,
          onChanged: (value) => controller.updateContact(phone: value),
          validator: (value) => _validation(
            context,
            const CompositeValidator<String>([
              RequiredValidator<String>(),
              PhoneValidator(),
            ]).validate(value),
          ),
        ),
        const SizedBox(height: AppSpacing.lg),
        AcademyTextField(
          label: l10n.academyWebsiteLabel,
          icon: Icons.link_rounded,
          initialValue: draft.website,
          keyboardType: TextInputType.url,
          onChanged: (value) => controller.updateContact(website: value),
          validator: (value) => _validateOptionalUrl(context, value),
        ),
      ],
    );
  }

  /// Localized error for [value], or null when empty or a valid URL.
  String? _validateOptionalUrl(BuildContext context, String? value) {
    final candidate = value?.trim();
    if (candidate == null || candidate.isEmpty) {
      return null;
    }
    if (!RegexConstants.url.hasMatch(candidate)) {
      return AcademyCreateMessages.validation(
        context,
        const ValidationError('validation.url.invalid'),
      );
    }
    return null;
  }
}

/// Resolves the localized error text, returning null when valid.
String? _validation(BuildContext context, ValidationError? error) =>
    AcademyCreateMessages.validation(context, error);

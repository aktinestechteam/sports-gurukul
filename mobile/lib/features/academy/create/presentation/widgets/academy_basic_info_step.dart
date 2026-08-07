import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/core/validators/required_validator.dart';
import 'package:sports_gurukul/core/validators/validator.dart';
import 'package:sports_gurukul/features/academy/create/presentation/academy_create_messages.dart';
import 'package:sports_gurukul/features/academy/create/presentation/create_academy_draft.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/academy_wizard_controller.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_sports_selector.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_text_field.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_type_selector.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Step 1: academy name, description, type and sports.
class AcademyBasicInfoStep extends StatelessWidget {
  const AcademyBasicInfoStep({
    required this.draft,
    required this.controller,
    this.readOnly = false,
    super.key,
  });

  final CreateAcademyDraft draft;
  final AcademyWizardController controller;

  /// When true the academy type and sports are shown read-only (the backend
  /// has no endpoint to change them after creation); the name and description
  /// stay editable.
  final bool readOnly;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: <Widget>[
        AcademyTextField(
          label: l10n.academyNameLabel,
          icon: Icons.school_rounded,
          initialValue: draft.name,
          textInputAction: TextInputAction.next,
          onChanged: (value) => controller.updateBasic(name: value),
          validator: (value) => AcademyCreateMessages.validation(
            context,
            const CompositeValidator<String>([
              RequiredValidator<String>(),
            ]).validate(value),
          ),
        ),
        const SizedBox(height: AppSpacing.lg),
        AcademyTextField(
          label: l10n.academyDescriptionLabel,
          icon: Icons.notes_rounded,
          initialValue: draft.description,
          maxLines: 3,
          onChanged: (value) => controller.updateBasic(description: value),
        ),
        const SizedBox(height: AppSpacing.xl),
        AcademyTypeSelector(
          draft: draft,
          controller: controller,
          readOnly: readOnly,
        ),
        const SizedBox(height: AppSpacing.xl),
        AcademySportsSelector(
          draft: draft,
          controller: controller,
          readOnly: readOnly,
        ),
        if (readOnly)
          Padding(
            padding: const EdgeInsets.only(top: AppSpacing.lg),
            child: Text(
              l10n.academyTypeSportsLocked,
              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                color: AppColors.grey300,
              ),
            ),
          ),
      ],
    );
  }
}

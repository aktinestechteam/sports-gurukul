import 'package:cross_file/cross_file.dart';
import 'package:flutter/material.dart';

import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/core/validators/validation_error.dart';
import 'package:sports_gurukul/features/academy/create/presentation/academy_create_messages.dart';
import 'package:sports_gurukul/features/academy/create/presentation/create_academy_draft.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/academy_wizard_controller.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_image_picker.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';

/// Step 4: academy logo (required) and cover image (optional).
///
/// Both images are collected client-side; the logo is uploaded to the academy
/// right after creation/update and then shown on the academy-admin dashboard
/// header. When editing an existing academy, [existingLogoUrl] and
/// [existingBannerUrl] preview the currently stored images.
class AcademyBrandingStep extends StatelessWidget {
  const AcademyBrandingStep({
    required this.draft,
    required this.controller,
    this.existingLogoUrl,
    this.existingBannerUrl,
    super.key,
  });

  final CreateAcademyDraft draft;
  final AcademyWizardController controller;
  final String? existingLogoUrl;
  final String? existingBannerUrl;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: <Widget>[
        FormField<XFile>(
          initialValue: draft.logo,
          validator: (value) => AcademyCreateMessages.validation(
            context,
            value == null && existingLogoUrl == null
                ? const ValidationError('academy.validation.logo')
                : null,
          ),
          builder: (field) {
            return Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                AcademyImagePicker(
                  label: l10n.academyLogoLabel,
                  helpText: l10n.academyLogoHint,
                  file: field.value,
                  fallbackUrl: existingLogoUrl,
                  square: true,
                  onPicked: (file) {
                    field.didChange(file);
                    controller.setLogo(file);
                  },
                  onRemoved: () {
                    field.didChange(null);
                    controller.setLogo(null);
                  },
                ),
                if (field.hasError)
                  Padding(
                    padding: const EdgeInsets.only(
                      top: AppSpacing.xs,
                      left: AppSpacing.lg,
                    ),
                    child: Text(
                      field.errorText ?? '',
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: AppColors.danger,
                      ),
                    ),
                  ),
              ],
            );
          },
        ),
        const SizedBox(height: AppSpacing.xl),
        AcademyImagePicker(
          label: l10n.academyCoverLabel,
          helpText: l10n.academyCoverHint,
          file: draft.cover,
          fallbackUrl: existingBannerUrl,
          onPicked: controller.setCover,
          onRemoved: () => controller.setCover(null),
        ),
      ],
    );
  }
}

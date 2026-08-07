import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/features/academy/create/application/my_academy_provider.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/presentation/academy_create_messages.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/edit_academy_controller.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_address_step.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_basic_info_step.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_bottom_bar.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_branding_step.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_contact_step.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_home_button.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_review_step.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_step_indicator.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/layouts/aurora_background.dart';

/// The five-step edit-academy wizard.
///
/// Reuses the create-academy steps, prefilled from the admin's academy. The
/// basic-information step keeps the academy type and sports read-only (the
/// backend exposes no endpoint to change them); everything else saves via
/// `PUT /api/v1/academies/{id}` and `PUT /api/v1/academies/{id}/contact`.
class EditAcademyPage extends ConsumerStatefulWidget {
  const EditAcademyPage({super.key});

  @override
  ConsumerState<EditAcademyPage> createState() => _EditAcademyPageState();
}

class _EditAcademyPageState extends ConsumerState<EditAcademyPage> {
  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final academy = ref.watch(myAcademyProvider).value;
    final state = ref.watch(editAcademyControllerProvider);

    ref.listen<EditAcademyState>(editAcademyControllerProvider, (
      previous,
      next,
    ) {
      if (next.status == EditAcademyStatus.success &&
          previous?.status != EditAcademyStatus.success) {
        _onSaved();
      } else if (next.failure != null && previous?.failure != next.failure) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              AcademyCreateMessages.failure(context, next.failure!),
            ),
          ),
        );
      }
    });

    return Scaffold(
      backgroundColor: Colors.transparent,
      body: AuroraBackground(
        child: SafeArea(
          bottom: false,
          child: Padding(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.xl,
              AppSpacing.md,
              AppSpacing.xl,
              AppSpacing.md,
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: <Widget>[
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: <Widget>[
                          Text(
                            l10n.academyEditTitle,
                            style: Theme.of(context).textTheme.headlineSmall
                                ?.copyWith(
                                  color: AppColors.surface,
                                  fontWeight: FontWeight.w800,
                                ),
                          ),
                          const SizedBox(height: AppSpacing.xs),
                          Text(
                            l10n.academyEditSubtitle,
                            style: Theme.of(context).textTheme.bodyMedium
                                ?.copyWith(
                                  color: AppColors.grey300,
                                ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(width: AppSpacing.md),
                    AcademyHomeButton(
                      tooltip: l10n.academyBackToDashboard,
                      onPressed: () => context.go(RoutePaths.academyDashboard),
                    ),
                  ],
                ),
                const SizedBox(height: AppSpacing.lg),
                AcademyStepIndicator(
                  step: state.step,
                  totalSteps: EditAcademyState.stepCount,
                ),
                const SizedBox(height: AppSpacing.lg),
                Expanded(
                  child: academy == null
                      ? const Center(
                          child: CircularProgressIndicator(
                            color: AppColors.surface,
                          ),
                        )
                      : SingleChildScrollView(
                          physics: const BouncingScrollPhysics(),
                          child: Form(
                            key: _formKey,
                            child: _stepContent(state, academy),
                          ),
                        ),
                ),
                if (academy != null) ...<Widget>[
                  const SizedBox(height: AppSpacing.lg),
                  AcademyBottomBar(
                    submitting:
                        state.status == EditAcademyStatus.submitting,
                    isLastStep: state.isLastStep,
                    submitLabel: l10n.academyEditSaveButton,
                    onBack: state.step > 0 ? _onBack : null,
                    onNext: state.isLastStep ? _onSubmit : _onNext,
                  ),
                ],
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _stepContent(EditAcademyState state, Academy academy) {
    final draft = state.draft;
    final controller = ref.read(editAcademyControllerProvider.notifier);
    return switch (state.step) {
      0 => AcademyBasicInfoStep(
        draft: draft,
        controller: controller,
        readOnly: true,
      ),
      1 => AcademyContactStep(draft: draft, controller: controller),
      2 => AcademyAddressStep(draft: draft, controller: controller),
      3 => AcademyBrandingStep(
        draft: draft,
        controller: controller,
        existingLogoUrl: academy.logoUrl,
        existingBannerUrl: academy.bannerUrl,
      ),
      _ => AcademyReviewStep(draft: draft, controller: controller),
    };
  }

  void _onBack() {
    ref.read(editAcademyControllerProvider.notifier).back();
  }

  void _onNext() {
    if (_formKey.currentState?.validate() ?? false) {
      ref.read(editAcademyControllerProvider.notifier).next();
    }
  }

  void _onSubmit() {
    if (_formKey.currentState?.validate() ?? false) {
      unawaited(ref.read(editAcademyControllerProvider.notifier).submit());
    }
  }

  /// Refreshes the academy (so the header shows the saved logo/name) and
  /// hands back to the academy dashboard.
  void _onSaved() {
    ref.invalidate(myAcademyProvider);
    context.go(RoutePaths.academyDashboard);
  }
}

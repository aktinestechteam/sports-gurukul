import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:sports_gurukul/app/router/route_paths.dart';
import 'package:sports_gurukul/app/theme/colors/app_colors.dart';
import 'package:sports_gurukul/app/theme/spacing/app_spacing.dart';
import 'package:sports_gurukul/features/academy/create/application/my_academy_provider.dart';
import 'package:sports_gurukul/features/academy/create/presentation/academy_create_messages.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/create_academy_controller.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_address_step.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_basic_info_step.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_bottom_bar.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_branding_step.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_contact_step.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_home_button.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_review_step.dart';
import 'package:sports_gurukul/features/academy/create/presentation/widgets/academy_step_indicator.dart';
import 'package:sports_gurukul/features/onboarding/presentation/providers/onboarding_controller.dart';
import 'package:sports_gurukul/l10n/generated/app_localizations.dart';
import 'package:sports_gurukul/shared/layouts/aurora_background.dart';

/// The five-step create-academy wizard.
///
/// Collects basic information, contact details, address, branding and a final
/// review before submitting to `POST /api/v1/academies`. On success the
/// onboarding session is re-resolved and the user is handed to the academy
/// dashboard.
class CreateAcademyPage extends ConsumerStatefulWidget {
  const CreateAcademyPage({super.key});

  @override
  ConsumerState<CreateAcademyPage> createState() => _CreateAcademyPageState();
}

class _CreateAcademyPageState extends ConsumerState<CreateAcademyPage> {
  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context);
    final state = ref.watch(createAcademyControllerProvider);

    ref.listen<CreateAcademyState>(createAcademyControllerProvider, (
      previous,
      next,
    ) {
      if (next.status == CreateAcademyStatus.success &&
          previous?.status != CreateAcademyStatus.success) {
        _onCreated();
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
                            l10n.academyCreateTitle,
                            style: Theme.of(context).textTheme.headlineSmall
                                ?.copyWith(
                                  color: AppColors.surface,
                                  fontWeight: FontWeight.w800,
                                ),
                          ),
                          const SizedBox(height: AppSpacing.xs),
                          Text(
                            l10n.academyCreateSubtitle,
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
                      onPressed: () => context.go(RoutePaths.dashboard),
                    ),
                  ],
                ),
                const SizedBox(height: AppSpacing.lg),
                AcademyStepIndicator(
                  step: state.step,
                  totalSteps: CreateAcademyState.stepCount,
                ),
                const SizedBox(height: AppSpacing.lg),
                Expanded(
                  child: SingleChildScrollView(
                    physics: const BouncingScrollPhysics(),
                    child: Form(
                      key: _formKey,
                      child: _stepContent(state),
                    ),
                  ),
                ),
                const SizedBox(height: AppSpacing.lg),
                AcademyBottomBar(
                  submitting: state.status == CreateAcademyStatus.submitting,
                  isLastStep: state.isLastStep,
                  onBack: state.step > 0 ? _onBack : null,
                  onNext: state.isLastStep ? _onSubmit : _onNext,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _stepContent(CreateAcademyState state) {
    final draft = state.draft;
    final controller = ref.read(createAcademyControllerProvider.notifier);
    return switch (state.step) {
      0 => AcademyBasicInfoStep(draft: draft, controller: controller),
      1 => AcademyContactStep(draft: draft, controller: controller),
      2 => AcademyAddressStep(draft: draft, controller: controller),
      3 => AcademyBrandingStep(draft: draft, controller: controller),
      _ => AcademyReviewStep(draft: draft, controller: controller),
    };
  }

  void _onBack() {
    ref.read(createAcademyControllerProvider.notifier).back();
  }

  void _onNext() {
    if (_formKey.currentState?.validate() ?? false) {
      ref.read(createAcademyControllerProvider.notifier).next();
    }
  }

  void _onSubmit() {
    if (_formKey.currentState?.validate() ?? false) {
      unawaited(ref.read(createAcademyControllerProvider.notifier).submit());
    }
  }

  /// Completes onboarding, re-resolves the current user (so a freshly assigned
  /// academy role is picked up) and hands off to the academy dashboard.
  void _onCreated() {
    ref.read(onboardingControllerProvider.notifier)
      ..completeOnboarding()
      ..refresh();
    ref.invalidate(myAcademyProvider);
    context.go(RoutePaths.academyDashboard);
  }
}

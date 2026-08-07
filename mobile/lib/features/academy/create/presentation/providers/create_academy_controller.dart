import 'package:cross_file/cross_file.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/application/create_academy_use_case_providers.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';
import 'package:sports_gurukul/features/academy/create/presentation/create_academy_draft.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/academy_wizard_controller.dart';

/// The submission lifecycle of the create-academy wizard.
enum CreateAcademyStatus {
  /// The wizard is collecting input; no request is in flight.
  idle,

  /// A create-academy request is being sent.
  submitting,

  /// The academy was created successfully.
  success,
}

/// The wizard state: current step, collected [CreateAcademyDraft] and the
/// submission outcome.
class CreateAcademyState {
  const CreateAcademyState({
    this.step = 0,
    this.draft = const CreateAcademyDraft(),
    this.status = CreateAcademyStatus.idle,
    this.failure,
    this.academy,
  });

  /// Total number of wizard steps (0-indexed 0..4).
  static const int stepCount = 5;

  /// The active wizard step index.
  final int step;

  /// Values collected so far.
  final CreateAcademyDraft draft;

  /// Submission lifecycle.
  final CreateAcademyStatus status;

  /// The failure that aborted the last submit, when any.
  final BaseFailure? failure;

  /// The created academy after a successful submit.
  final Academy? academy;

  bool get isLastStep => step == stepCount - 1;

  CreateAcademyState copyWith({
    int? step,
    CreateAcademyDraft? draft,
    CreateAcademyStatus? status,
    BaseFailure? failure,
    Academy? academy,
  }) => CreateAcademyState(
    step: step ?? this.step,
    draft: draft ?? this.draft,
    status: status ?? this.status,
    failure: failure ?? this.failure,
    academy: academy ?? this.academy,
  );
}

/// Owns the create-academy wizard: step navigation, the collected draft and
/// the submission against `POST /api/v1/academies`.
class CreateAcademyController extends Notifier<CreateAcademyState>
    implements AcademyWizardController {
  @override
  CreateAcademyState build() => const CreateAcademyState();

  /// Moves to the next step.
  @override
  void next() {
    if (!state.isLastStep && state.status == CreateAcademyStatus.idle) {
      state = state.copyWith(step: state.step + 1);
    }
  }

  /// Moves to the previous step.
  @override
  void back() {
    if (state.step > 0 && state.status == CreateAcademyStatus.idle) {
      state = state.copyWith(step: state.step - 1);
    }
  }

  /// Jumps straight to [step] (used by review-screen edit actions).
  @override
  void jumpTo(int step) {
    if (step < 0 || step >= CreateAcademyState.stepCount) {
      return;
    }
    state = state.copyWith(step: step);
  }

  /// Replaces the basic-information fields.
  @override
  void updateBasic({
    String? name,
    String? description,
    AcademyType? academyType,
    List<String>? sports,
  }) {
    state = state.copyWith(
      draft: state.draft.copyWith(
        name: name ?? state.draft.name,
        description: description ?? state.draft.description,
        academyType: academyType ?? state.draft.academyType,
        sports: sports ?? state.draft.sports,
      ),
    );
  }

  /// Replaces the contact-information fields.
  @override
  void updateContact({
    String? contactPerson,
    String? email,
    String? phone,
    String? website,
  }) {
    state = state.copyWith(
      draft: state.draft.copyWith(
        contactPerson: contactPerson ?? state.draft.contactPerson,
        email: email ?? state.draft.email,
        phone: phone ?? state.draft.phone,
        website: website ?? state.draft.website,
      ),
    );
  }

  /// Replaces the address fields.
  @override
  void updateAddress({
    String? country,
    String? stateName,
    String? city,
    String? addressLine,
    String? postalCode,
  }) {
    state = state.copyWith(
      draft: state.draft.copyWith(
        country: country ?? state.draft.country,
        state: stateName ?? state.draft.state,
        city: city ?? state.draft.city,
        addressLine: addressLine ?? state.draft.addressLine,
        postalCode: postalCode ?? state.draft.postalCode,
      ),
    );
  }

  /// Replaces the academy logo.
  @override
  void setLogo(XFile? file) {
    state = state.copyWith(draft: state.draft.copyWith(logo: file));
  }

  /// Replaces the cover image.
  @override
  void setCover(XFile? file) {
    state = state.copyWith(draft: state.draft.copyWith(cover: file));
  }

  /// Submits the wizard against `POST /api/v1/academies`, then uploads any
  /// logo/cover images chosen in the wizard. Branding uploads are best-effort:
  /// the academy record itself is created first, so a failed image upload does
  /// not fail the whole submission.
  Future<void> submit() async {
    if (state.status == CreateAcademyStatus.submitting) {
      return;
    }
    final draft = state.draft;
    state = state.copyWith(status: CreateAcademyStatus.submitting);

    final params = CreateAcademyParams(
      name: draft.name.trim(),
      description: _emptyToNull(draft.description),
      academyType: draft.academyType,
      sports: draft.sports,
      primaryContactName: _emptyToNull(draft.contactPerson),
      email: draft.email.trim(),
      phone: draft.phone.trim(),
      website: _emptyToNull(draft.website),
      address: _emptyToNull(draft.addressLine),
      country: _emptyToNull(draft.country),
      state: _emptyToNull(draft.state),
      city: _emptyToNull(draft.city),
      postalCode: _emptyToNull(draft.postalCode),
      logo: draft.logo,
      cover: draft.cover,
    );
    final result = await ref.read(createAcademyUseCaseProvider).call(params);
    state = switch (result) {
      Success(value: final academy) => state.copyWith(
        status: CreateAcademyStatus.success,
        academy: academy,
      ),
      FailureResult(:final failure) => state.copyWith(
        status: CreateAcademyStatus.idle,
        failure: failure,
      ),
    };
  }

  static String? _emptyToNull(String value) {
    final trimmed = value.trim();
    return trimmed.isEmpty ? null : trimmed;
  }
}

/// Provides the create-academy wizard controller.
final createAcademyControllerProvider =
    NotifierProvider<CreateAcademyController, CreateAcademyState>(
      CreateAcademyController.new,
    );

import 'package:cross_file/cross_file.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:sports_gurukul/core/failures/base_failure.dart';
import 'package:sports_gurukul/core/result/result.dart';
import 'package:sports_gurukul/features/academy/create/application/create_academy_use_case_providers.dart';
import 'package:sports_gurukul/features/academy/create/application/my_academy_provider.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_contact.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/create_academy_params.dart';
import 'package:sports_gurukul/features/academy/create/presentation/create_academy_draft.dart';
import 'package:sports_gurukul/features/academy/create/presentation/providers/academy_wizard_controller.dart';

/// The submission lifecycle of the edit-academy wizard.
enum EditAcademyStatus {
  /// The wizard is collecting input; no request is in flight.
  idle,

  /// An update request is being sent.
  submitting,

  /// The academy was updated successfully.
  success,
}

/// The edit-wizard state: current step, the prefilled [CreateAcademyDraft]
/// and the submission outcome.
class EditAcademyState {
  const EditAcademyState({
    this.step = 0,
    this.draft = const CreateAcademyDraft(),
    this.status = EditAcademyStatus.idle,
    this.failure,
  });

  /// Total number of wizard steps (0-indexed 0..4).
  static const int stepCount = 5;

  /// The active wizard step index.
  final int step;

  /// Values collected so far (prefilled from the current academy).
  final CreateAcademyDraft draft;

  /// Submission lifecycle.
  final EditAcademyStatus status;

  /// The failure that aborted the last submit, when any.
  final BaseFailure? failure;

  bool get isLastStep => step == stepCount - 1;

  EditAcademyState copyWith({
    int? step,
    CreateAcademyDraft? draft,
    EditAcademyStatus? status,
    BaseFailure? failure,
  }) => EditAcademyState(
    step: step ?? this.step,
    draft: draft ?? this.draft,
    status: status ?? this.status,
    failure: failure ?? this.failure,
  );
}

/// Owns the edit-academy wizard: the prefilled draft, step navigation and the
/// submission against `PUT /api/v1/academies/{id}` plus
/// `PUT /api/v1/academies/{id}/contact`.
class EditAcademyController extends Notifier<EditAcademyState>
    implements AcademyWizardController {
  @override
  EditAcademyState build() {
    final academy = ref.watch(myAcademyProvider).value;
    return EditAcademyState(draft: _draftFrom(academy));
  }

  /// Prefills the wizard draft from the admin's academy so each step shows
  /// the current values before any edits.
  static CreateAcademyDraft _draftFrom(Academy? academy) {
    if (academy == null) {
      return const CreateAcademyDraft();
    }
    return CreateAcademyDraft(
      name: academy.name,
      description: academy.description ?? '',
      academyType: _parseAcademyType(academy.academyType),
      sports: academy.sports,
      contactPerson: academy.primaryContactName ?? '',
      email: academy.email,
      phone: academy.phone,
      website: academy.website ?? '',
      country: academy.country ?? '',
      state: academy.state ?? '',
      city: academy.city ?? '',
      addressLine: academy.address ?? '',
      postalCode: academy.postalCode ?? '',
    );
  }

  static AcademyType? _parseAcademyType(String? value) {
    if (value == null) {
      return null;
    }
    for (final type in AcademyType.values) {
      if (type.wireValue == value) {
        return type;
      }
    }
    return null;
  }

  @override
  void next() {
    if (!state.isLastStep && state.status == EditAcademyStatus.idle) {
      state = state.copyWith(step: state.step + 1);
    }
  }

  @override
  void back() {
    if (state.step > 0 && state.status == EditAcademyStatus.idle) {
      state = state.copyWith(step: state.step - 1);
    }
  }

  @override
  void jumpTo(int step) {
    if (step < 0 || step >= EditAcademyState.stepCount) {
      return;
    }
    state = state.copyWith(step: step);
  }

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

  @override
  void setLogo(XFile? file) {
    state = state.copyWith(draft: state.draft.copyWith(logo: file));
  }

  @override
  void setCover(XFile? file) {
    state = state.copyWith(draft: state.draft.copyWith(cover: file));
  }

  /// Saves the wizard against the current academy: core fields (name,
  /// description, email, phone, website) plus any branding images first, then
  /// the contact + address block. Academy type and sports are shown read-only
  /// and never sent, as the backend exposes no endpoint to change them.
  Future<void> submit() async {
    if (state.status == EditAcademyStatus.submitting) {
      return;
    }
    final academy = ref.read(myAcademyProvider).value;
    if (academy == null) {
      return;
    }

    final draft = state.draft;
    state = state.copyWith(status: EditAcademyStatus.submitting);

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

    final core = await ref.read(updateAcademyUseCaseProvider).call(
      academy.id,
      params,
    );
    if (core case FailureResult(:final failure)) {
      state = state.copyWith(status: EditAcademyStatus.idle, failure: failure);
      return;
    }

    final contact = await ref.read(updateAcademyContactUseCaseProvider).call(
      academy.id,
      params,
    );
    state = switch (contact) {
      Success<AcademyContact>(value: _) => state.copyWith(
        status: EditAcademyStatus.success,
      ),
      FailureResult<AcademyContact>(:final failure) => state.copyWith(
        status: EditAcademyStatus.idle,
        failure: failure,
      ),
    };
  }

  static String? _emptyToNull(String value) {
    final trimmed = value.trim();
    return trimmed.isEmpty ? null : trimmed;
  }
}

/// Provides the edit-academy wizard controller.
final editAcademyControllerProvider =
    NotifierProvider<EditAcademyController, EditAcademyState>(
      EditAcademyController.new,
    );

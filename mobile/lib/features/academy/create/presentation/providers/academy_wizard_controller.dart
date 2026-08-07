import 'package:cross_file/cross_file.dart';

import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';

/// Common wizard mutations shared by the create- and edit-academy flows.
///
/// Step widgets bind to this abstraction so the same steps render in both
/// flows while each controller owns its own draft and submission lifecycle.
abstract interface class AcademyWizardController {
  /// Moves to the next step.
  void next();

  /// Moves to the previous step.
  void back();

  /// Jumps straight to [step] (used by review-screen edit actions).
  void jumpTo(int step);

  /// Replaces the basic-information fields.
  void updateBasic({
    String? name,
    String? description,
    AcademyType? academyType,
    List<String>? sports,
  });

  /// Replaces the contact-information fields.
  void updateContact({
    String? contactPerson,
    String? email,
    String? phone,
    String? website,
  });

  /// Replaces the address fields.
  void updateAddress({
    String? country,
    String? stateName,
    String? city,
    String? addressLine,
    String? postalCode,
  });

  /// Replaces the academy logo.
  void setLogo(XFile? file);

  /// Replaces the cover image.
  void setCover(XFile? file);
}

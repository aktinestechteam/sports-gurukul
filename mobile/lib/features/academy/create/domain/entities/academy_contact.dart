import 'package:flutter/foundation.dart';

/// The contact + address block of an academy, returned by
/// `PUT /api/v1/academies/{academyId}/contact`.
@immutable
class AcademyContact {
  const AcademyContact({
    this.primaryContactName,
    this.address,
    this.country,
    this.state,
    this.city,
    this.postalCode,
  });

  /// Primary contact person's name.
  final String? primaryContactName;

  /// Street address of the academy.
  final String? address;

  /// Country of the academy.
  final String? country;

  /// State or province of the academy.
  final String? state;

  /// City of the academy.
  final String? city;

  /// Postal code of the academy.
  final String? postalCode;
}

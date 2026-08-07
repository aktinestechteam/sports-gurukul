import 'package:cross_file/cross_file.dart';
import 'package:flutter/foundation.dart';

import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';

/// Fields submitted to `POST /api/v1/academies`.
///
/// Mirrors the backend `CreateAcademyRequest` contract. Academy type, sports,
/// contact/address and branding images are persisted server-side.
@immutable
class CreateAcademyParams {
  const CreateAcademyParams({
    required this.name,
    required this.email,
    required this.phone,
    this.legalName,
    this.description,
    this.registrationNumber,
    this.gstNumber,
    this.establishedDate,
    this.website,
    this.academyType,
    this.sports = const <String>[],
    this.primaryContactName,
    this.address,
    this.country,
    this.state,
    this.city,
    this.postalCode,
    this.logo,
    this.cover,
  });

  /// Academy name (required by the backend).
  final String name;

  /// Legal registered name.
  final String? legalName;

  /// Brief description of the academy.
  final String? description;

  /// Registration or incorporation number.
  final String? registrationNumber;

  /// GST identification number.
  final String? gstNumber;

  /// Date the academy was established.
  final DateTime? establishedDate;

  /// Academy website URL.
  final String? website;

  /// Primary contact email (required by the backend).
  final String email;

  /// Primary contact phone (required by the backend).
  final String phone;

  /// Whether the academy offers a single sport or several.
  final AcademyType? academyType;

  /// Names of the sports offered. The first entry is the primary sport.
  final List<String> sports;

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

  /// Optional logo image uploaded after the academy is created.
  final XFile? logo;

  /// Optional cover/banner image uploaded after the academy is created.
  final XFile? cover;
}

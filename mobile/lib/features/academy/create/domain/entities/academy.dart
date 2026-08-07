import 'package:flutter/foundation.dart';

/// A created academy as returned by `POST /api/v1/academies`.
///
/// Mirrors the backend `AcademyDto` response shape. This entity never carries
/// transport-level concerns (HTTP metadata, tokens).
@immutable
class Academy {
  const Academy({
    required this.id,
    required this.academyCode,
    required this.name,
    required this.email,
    required this.phone,
    required this.status,
    required this.verificationStatus,
    required this.createdAt,
    this.legalName,
    this.description,
    this.website,
    this.establishedDate,
    this.academyType,
    this.sports = const <String>[],
    this.primaryContactName,
    this.address,
    this.country,
    this.state,
    this.city,
    this.postalCode,
    this.logoUrl,
    this.bannerUrl,
  });

  /// Backend academy id.
  final String id;

  /// Human-friendly unique academy code (e.g. `MSA-0001`).
  final String academyCode;

  /// The academy name.
  final String name;

  /// Legal registered name, when provided.
  final String? legalName;

  /// Brief description of the academy.
  final String? description;

  /// Academy website URL.
  final String? website;

  /// Primary contact email.
  final String email;

  /// Primary contact phone.
  final String phone;

  /// Current lifecycle status (e.g. `Active`).
  final String status;

  /// Verification state (e.g. `Pending`).
  final String verificationStatus;

  /// Display logo URL, when the backend provides one.
  final String? logoUrl;

  /// Display banner URL, when the backend provides one.
  final String? bannerUrl;

  /// Date the academy was established, when provided.
  final DateTime? establishedDate;

  /// How the academy structures its programs (`SingleSport`/`MultiSport`),
  /// when the backend reports it.
  final String? academyType;

  /// Names of the sports offered, when the backend reports them.
  final List<String> sports;

  /// Primary contact person's name, when the backend reports it.
  final String? primaryContactName;

  /// Street address of the academy, when the backend reports it.
  final String? address;

  /// Country of the academy, when the backend reports it.
  final String? country;

  /// State or province of the academy, when the backend reports it.
  final String? state;

  /// City of the academy, when the backend reports it.
  final String? city;

  /// Postal code of the academy, when the backend reports it.
  final String? postalCode;

  /// When the academy record was created.
  final DateTime createdAt;

  @override
  bool operator ==(Object other) =>
      other is Academy &&
      other.id == id &&
      other.academyCode == academyCode &&
      other.name == name;

  @override
  int get hashCode => Object.hash(id, academyCode, name);
}

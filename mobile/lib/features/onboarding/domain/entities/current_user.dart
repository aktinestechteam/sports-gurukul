import 'package:flutter/foundation.dart';

import 'package:sports_gurukul/features/onboarding/domain/entities/user_role.dart';

/// The authenticated user's identity as resolved right after login.
///
/// A thin, onboarding-oriented projection of the backend `UserProfile` from
/// `GET /api/v1/users/me`, carrying only the identity and role signals needed
/// for state resolution. Never carries transport-level concerns.
@immutable
class CurrentUser {
  const CurrentUser({
    required this.id,
    required this.fullName,
    required this.email,
    required this.roles,
    this.profileImageUrl,
    this.isEmailVerified = false,
    this.profileCompletionPercentage = 0,
    this.hasAcademyAssociation = false,
    this.hasPendingMembership = false,
  });

  /// Backend user id.
  final String id;

  /// The user's display name.
  final String fullName;

  /// The user's email address.
  final String email;

  /// Roles assigned to the user, parsed from the profile's role strings.
  final List<UserRole> roles;

  /// Display photo URL, when the profile provides one.
  final String? profileImageUrl;

  /// Whether the backend confirmed the email address.
  final bool isEmailVerified;

  /// 0..100 profile completion reported by the backend.
  final int profileCompletionPercentage;

  /// Whether the profile carries an academy-type association (an academy-type
  /// address). The backend does not expose a dedicated association field.
  final bool hasAcademyAssociation;

  /// Whether a join-academy membership request is awaiting approval.
  final bool hasPendingMembership;

  /// Whether the account holds only the default registration role.
  bool get hasOnlyDefaultRole =>
      roles.length == 1 && roles.first.isDefaultRegistrationRole;

  @override
  bool operator ==(Object other) =>
      other is CurrentUser &&
      other.id == id &&
      other.email == email &&
      other.fullName == fullName;

  @override
  int get hashCode => Object.hash(id, email, fullName);

  @override
  String toString() =>
      'CurrentUser('
      'id: $id, fullName: $fullName, email: $email)';
}

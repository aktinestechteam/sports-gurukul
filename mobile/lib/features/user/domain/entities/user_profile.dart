import 'package:flutter/foundation.dart';

import 'package:sports_gurukul/features/user/domain/entities/address.dart';
import 'package:sports_gurukul/features/user/domain/entities/contact_information.dart';
import 'package:sports_gurukul/features/user/domain/entities/user_preference.dart';

/// User gender options matching the backend `Gender` enum.
enum Gender { male, female, nonBinary, preferNotToSay }

/// User account status matching the backend `UserStatus` enum.
enum UserStatus { active, inactive, suspended, locked }

/// The full user profile entity returned by `GET /api/v1/users/me`.
///
/// Contains identity information, physical attributes, addresses, contact
/// details, preferences, and role assignments. This entity never carries
/// transport-level concerns (tokens, HTTP metadata).
@immutable
class UserProfile {
  const UserProfile({
    required this.id,
    required this.userId,
    required this.fullName,
    required this.email,
    required this.createdAt,
    this.phoneNumber,
    this.dateOfBirth,
    this.gender = Gender.preferNotToSay,
    this.bio,
    this.profileImageUrl,
    this.coverImageUrl,
    this.height,
    this.weight,
    this.preferredSport,
    this.experienceLevel,
    this.status = UserStatus.active,
    this.isEmailVerified = false,
    this.updatedAt,
    this.profileCompletionPercentage = 0,
    this.addresses = const [],
    this.contactInformation,
    this.preferences,
    this.roles = const [],
  });

  final String id;
  final String userId;
  final String fullName;
  final String email;
  final String? phoneNumber;
  final DateTime? dateOfBirth;
  final Gender gender;
  final String? bio;
  final String? profileImageUrl;
  final String? coverImageUrl;
  final String? height;
  final String? weight;
  final String? preferredSport;
  final String? experienceLevel;
  final UserStatus status;
  final bool isEmailVerified;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final int profileCompletionPercentage;
  final List<Address> addresses;
  final ContactInformation? contactInformation;
  final UserPreference? preferences;
  final List<String> roles;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is UserProfile &&
          runtimeType == other.runtimeType &&
          id == other.id &&
          userId == other.userId &&
          fullName == other.fullName &&
          email == other.email;

  @override
  int get hashCode => Object.hash(id, userId, fullName, email);
}

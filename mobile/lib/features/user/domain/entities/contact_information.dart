import 'package:flutter/foundation.dart';

/// Contact details associated with a user profile.
@immutable
class ContactInformation {
  const ContactInformation({
    required this.id,
    this.primaryPhoneCountryCode,
    this.primaryPhoneNumber,
    this.primaryPhoneVerified = false,
    this.secondaryPhoneCountryCode,
    this.secondaryPhoneNumber,
    this.secondaryPhoneVerified = false,
    this.websiteUrl,
    this.facebookUrl,
    this.twitterUrl,
    this.instagramUrl,
    this.linkedInUrl,
    this.youTubeUrl,
  });

  final String id;
  final String? primaryPhoneCountryCode;
  final String? primaryPhoneNumber;
  final bool primaryPhoneVerified;
  final String? secondaryPhoneCountryCode;
  final String? secondaryPhoneNumber;
  final bool secondaryPhoneVerified;
  final String? websiteUrl;
  final String? facebookUrl;
  final String? twitterUrl;
  final String? instagramUrl;
  final String? linkedInUrl;
  final String? youTubeUrl;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ContactInformation &&
          runtimeType == other.runtimeType &&
          id == other.id &&
          primaryPhoneCountryCode == other.primaryPhoneCountryCode &&
          primaryPhoneNumber == other.primaryPhoneNumber &&
          primaryPhoneVerified == other.primaryPhoneVerified;

  @override
  int get hashCode => Object.hash(
    id,
    primaryPhoneCountryCode,
    primaryPhoneNumber,
    primaryPhoneVerified,
  );
}

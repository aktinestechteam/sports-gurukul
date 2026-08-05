import 'package:flutter/foundation.dart';

/// Types of address (Home, Work, Academy, Other).
enum AddressType { home, work, academy, other }

/// An address associated with a user profile.
@immutable
class Address {
  const Address({
    required this.id,
    required this.addressType,
    required this.line1,
    required this.city,
    required this.state,
    required this.country,
    this.line2,
    this.postalCode,
    this.isPrimary = false,
    this.latitude,
    this.longitude,
  });

  final String id;
  final AddressType addressType;
  final String line1;
  final String? line2;
  final String city;
  final String state;
  final String country;
  final String? postalCode;
  final bool isPrimary;
  final double? latitude;
  final double? longitude;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is Address &&
          runtimeType == other.runtimeType &&
          id == other.id &&
          addressType == other.addressType &&
          line1 == other.line1 &&
          line2 == other.line2 &&
          city == other.city &&
          state == other.state &&
          country == other.country &&
          postalCode == other.postalCode &&
          isPrimary == other.isPrimary;

  @override
  int get hashCode => Object.hash(
    id,
    addressType,
    line1,
    line2,
    city,
    state,
    country,
    postalCode,
    isPrimary,
  );
}

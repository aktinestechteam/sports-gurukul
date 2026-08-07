import 'package:cross_file/cross_file.dart';
import 'package:flutter/foundation.dart';

import 'package:sports_gurukul/features/academy/create/domain/entities/academy_type.dart';

/// In-progress values captured by the create-academy wizard.
///
/// Owned by the wizard controller so values survive step navigation.
@immutable
class CreateAcademyDraft {
  const CreateAcademyDraft({
    this.name = '',
    this.description = '',
    this.academyType,
    this.sports = const <String>[],
    this.contactPerson = '',
    this.email = '',
    this.phone = '',
    this.website = '',
    this.country = '',
    this.state = '',
    this.city = '',
    this.addressLine = '',
    this.postalCode = '',
    this.logo,
    this.cover,
  });

  final String name;
  final String description;
  final AcademyType? academyType;
  final List<String> sports;
  final String contactPerson;
  final String email;
  final String phone;
  final String website;
  final String country;
  final String state;
  final String city;
  final String addressLine;
  final String postalCode;
  final XFile? logo;
  final XFile? cover;

  CreateAcademyDraft copyWith({
    String? name,
    String? description,
    AcademyType? academyType,
    List<String>? sports,
    String? contactPerson,
    String? email,
    String? phone,
    String? website,
    String? country,
    String? state,
    String? city,
    String? addressLine,
    String? postalCode,
    XFile? logo,
    XFile? cover,
  }) => CreateAcademyDraft(
    name: name ?? this.name,
    description: description ?? this.description,
    academyType: academyType ?? this.academyType,
    sports: sports ?? this.sports,
    contactPerson: contactPerson ?? this.contactPerson,
    email: email ?? this.email,
    phone: phone ?? this.phone,
    website: website ?? this.website,
    country: country ?? this.country,
    state: state ?? this.state,
    city: city ?? this.city,
    addressLine: addressLine ?? this.addressLine,
    postalCode: postalCode ?? this.postalCode,
    logo: logo ?? this.logo,
    cover: cover ?? this.cover,
  );
}

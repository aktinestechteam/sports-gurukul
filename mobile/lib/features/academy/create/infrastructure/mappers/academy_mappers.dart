import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy_contact.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_contact_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_dto.dart';

/// Converts infrastructure DTOs into domain entities.
///
/// Mapping happens only at the repository boundary: DTOs never leave
/// infrastructure and domain entities never travel to the wire.
abstract final class AcademyMappers {
  static Academy toAcademy(AcademyDto dto) {
    final branch = dto.branches == null || dto.branches!.isEmpty
        ? null
        : dto.branches!.first;
    final contact = dto.contact;
    return Academy(
      id: dto.id,
      academyCode: dto.academyCode,
      name: dto.name,
      legalName: dto.legalName,
      description: dto.description,
      website: dto.website,
      email: dto.email,
      phone: dto.phone,
      status: dto.status,
      verificationStatus: dto.verificationStatus,
      academyType: dto.academyType,
      sports: dto.sports?.map((sport) => sport.name ?? '').toList() ??
          const <String>[],
      primaryContactName: contact?.primaryContactName,
      address: contact?.address ?? branch?.address,
      country: contact?.country ?? branch?.country,
      state: contact?.state ?? branch?.state,
      city: contact?.city ?? branch?.city,
      postalCode: contact?.postalCode ?? branch?.postalCode,
      logoUrl: dto.logoUrl,
      bannerUrl: dto.bannerUrl,
      establishedDate: dto.establishedDate != null
          ? DateTime.tryParse(dto.establishedDate!)
          : null,
      createdAt: DateTime.parse(dto.createdAt),
    );
  }

  /// Maps a contact DTO (from `PUT /api/v1/academies/{id}/contact`) onto the
  /// domain [AcademyContact].
  static AcademyContact toContact(AcademyContactDto dto) => AcademyContact(
    primaryContactName: dto.primaryContactName,
    address: dto.address,
    country: dto.country,
    state: dto.state,
    city: dto.city,
    postalCode: dto.postalCode,
  );
}

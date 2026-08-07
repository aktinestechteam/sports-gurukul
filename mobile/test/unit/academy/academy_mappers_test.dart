import 'package:flutter_test/flutter_test.dart';
import 'package:sports_gurukul/features/academy/create/domain/entities/academy.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/mappers/academy_mappers.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_branch_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_contact_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_dto.dart';
import 'package:sports_gurukul/features/academy/create/infrastructure/models/academy_sport_dto.dart';

void main() {
  const academyDto = AcademyDto(
    id: 'ac-1',
    academyCode: 'SG-0001',
    name: 'Warriors Cricket Academy',
    email: 'team@warriors.in',
    phone: '9876543210',
    status: 'Active',
    verificationStatus: 'Pending',
    createdAt: '2026-02-03T10:30:00.0000000Z',
    legalName: 'Warriors Sports LLP',
    description: 'Grassroots cricket training.',
    website: 'https://warriors.in',
    establishedDate: '2018-04-15',
    logoUrl: 'https://cdn.example.com/w.png',
    bannerUrl: 'https://cdn.example.com/wb.png',
    academyType: 'MultiSport',
    sports: <AcademySportDto>[
      AcademySportDto(id: 'sp-1', name: 'Cricket'),
      AcademySportDto(id: 'sp-2', name: 'Football'),
    ],
    contact: AcademyContactDto(
      primaryContactName: 'Aarav Sharma',
      primaryPhone: '9876543210',
      primaryEmail: 'team@warriors.in',
      address: 'Marine Drive',
      country: 'India',
      state: 'Maharashtra',
      city: 'Mumbai',
      postalCode: '400001',
    ),
    branches: <AcademyBranchDto>[
      AcademyBranchDto(
        id: 'br-1',
        branchName: 'Main Branch',
        address: 'MG Road',
        country: 'India',
        state: 'Maharashtra',
        city: 'Pune',
        postalCode: '411001',
      ),
    ],
  );

  group('AcademyMappers', () {
    test('maps a full AcademyDto onto the Academy entity', () {
      final academy = AcademyMappers.toAcademy(academyDto);

      expect(academy, isA<Academy>());
      expect(academy.id, 'ac-1');
      expect(academy.academyCode, 'SG-0001');
      expect(academy.name, 'Warriors Cricket Academy');
      expect(academy.legalName, 'Warriors Sports LLP');
      expect(academy.description, 'Grassroots cricket training.');
      expect(academy.website, 'https://warriors.in');
      expect(academy.email, 'team@warriors.in');
      expect(academy.phone, '9876543210');
      expect(academy.status, 'Active');
      expect(academy.verificationStatus, 'Pending');
      expect(academy.logoUrl, 'https://cdn.example.com/w.png');
      expect(academy.bannerUrl, 'https://cdn.example.com/wb.png');
      expect(academy.establishedDate, DateTime(2018, 4, 15));
      expect(academy.createdAt, DateTime.utc(2026, 2, 3, 10, 30));
      expect(academy.academyType, 'MultiSport');
      expect(academy.sports, <String>['Cricket', 'Football']);
      expect(academy.primaryContactName, 'Aarav Sharma');
      expect(academy.address, 'Marine Drive');
      expect(academy.country, 'India');
      expect(academy.state, 'Maharashtra');
      expect(academy.city, 'Mumbai');
      expect(academy.postalCode, '400001');
    });

    test('reads contact/address fields from the contact when no branches exist', () {
      const contactOnly = AcademyDto(
        id: 'ac-3',
        academyCode: 'SG-0003',
        name: 'Contact Only Academy',
        email: 'c@o.in',
        phone: '9876543210',
        status: 'Active',
        verificationStatus: 'Pending',
        createdAt: '2026-01-01T00:00:00Z',
        contact: AcademyContactDto(
          primaryContactName: 'Priya Nair',
          address: 'Green Road',
          country: 'India',
          state: 'Kerala',
          city: 'Kochi',
          postalCode: '682001',
        ),
      );

      final academy = AcademyMappers.toAcademy(contactOnly);

      expect(academy.primaryContactName, 'Priya Nair');
      expect(academy.address, 'Green Road');
      expect(academy.country, 'India');
      expect(academy.state, 'Kerala');
      expect(academy.city, 'Kochi');
      expect(academy.postalCode, '682001');
    });

    test('keeps optional fields null when absent', () {
      const sparse = AcademyDto(
        id: 'ac-2',
        academyCode: 'SG-0002',
        name: 'Sparse Academy',
        email: 'a@b.in',
        phone: '9876543210',
        status: 'Active',
        verificationStatus: 'Pending',
        createdAt: '2026-01-01T00:00:00Z',
      );

      final academy = AcademyMappers.toAcademy(sparse);

      expect(academy.legalName, isNull);
      expect(academy.description, isNull);
      expect(academy.website, isNull);
      expect(academy.establishedDate, isNull);
      expect(academy.logoUrl, isNull);
      expect(academy.bannerUrl, isNull);
      expect(academy.academyType, isNull);
      expect(academy.sports, isEmpty);
      expect(academy.primaryContactName, isNull);
      expect(academy.address, isNull);
      expect(academy.country, isNull);
      expect(academy.state, isNull);
      expect(academy.city, isNull);
      expect(academy.postalCode, isNull);
    });
  });
}

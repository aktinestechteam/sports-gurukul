// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'academy_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_AcademyDto _$AcademyDtoFromJson(Map<String, dynamic> json) => _AcademyDto(
  id: json['id'] as String,
  academyCode: json['academyCode'] as String,
  name: json['name'] as String,
  email: json['email'] as String,
  phone: json['phone'] as String,
  status: json['status'] as String,
  verificationStatus: json['verificationStatus'] as String,
  createdAt: json['createdAt'] as String,
  legalName: json['legalName'] as String?,
  description: json['description'] as String?,
  registrationNumber: json['registrationNumber'] as String?,
  gstNumber: json['gstNumber'] as String?,
  establishedDate: json['establishedDate'] as String?,
  website: json['website'] as String?,
  academyType: json['academyType'] as String?,
  logoUrl: json['logoUrl'] as String?,
  bannerUrl: json['bannerUrl'] as String?,
  updatedAt: json['updatedAt'] as String?,
  contact: json['contact'] == null
      ? null
      : AcademyContactDto.fromJson(json['contact'] as Map<String, dynamic>),
  branches: (json['branches'] as List<dynamic>?)
      ?.map((e) => AcademyBranchDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  sports: (json['sports'] as List<dynamic>?)
      ?.map((e) => AcademySportDto.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$AcademyDtoToJson(_AcademyDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'academyCode': instance.academyCode,
      'name': instance.name,
      'email': instance.email,
      'phone': instance.phone,
      'status': instance.status,
      'verificationStatus': instance.verificationStatus,
      'createdAt': instance.createdAt,
      'legalName': instance.legalName,
      'description': instance.description,
      'registrationNumber': instance.registrationNumber,
      'gstNumber': instance.gstNumber,
      'establishedDate': instance.establishedDate,
      'website': instance.website,
      'academyType': instance.academyType,
      'logoUrl': instance.logoUrl,
      'bannerUrl': instance.bannerUrl,
      'updatedAt': instance.updatedAt,
      'contact': instance.contact,
      'branches': instance.branches,
      'sports': instance.sports,
    };

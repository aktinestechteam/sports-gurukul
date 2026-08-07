// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'academy_branch_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_AcademyBranchDto _$AcademyBranchDtoFromJson(Map<String, dynamic> json) =>
    _AcademyBranchDto(
      id: json['id'] as String?,
      academyId: json['academyId'] as String?,
      branchName: json['branchName'] as String?,
      address: json['address'] as String?,
      country: json['country'] as String?,
      state: json['state'] as String?,
      city: json['city'] as String?,
      district: json['district'] as String?,
      postalCode: json['postalCode'] as String?,
      createdAt: json['createdAt'] as String?,
      updatedAt: json['updatedAt'] as String?,
    );

Map<String, dynamic> _$AcademyBranchDtoToJson(_AcademyBranchDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'academyId': instance.academyId,
      'branchName': instance.branchName,
      'address': instance.address,
      'country': instance.country,
      'state': instance.state,
      'city': instance.city,
      'district': instance.district,
      'postalCode': instance.postalCode,
      'createdAt': instance.createdAt,
      'updatedAt': instance.updatedAt,
    };

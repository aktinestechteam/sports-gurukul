// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'academy_contact_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_AcademyContactDto _$AcademyContactDtoFromJson(Map<String, dynamic> json) =>
    _AcademyContactDto(
      id: json['id'] as String?,
      academyId: json['academyId'] as String?,
      primaryContactName: json['primaryContactName'] as String?,
      primaryPhone: json['primaryPhone'] as String?,
      primaryEmail: json['primaryEmail'] as String?,
      secondaryContactName: json['secondaryContactName'] as String?,
      secondaryPhone: json['secondaryPhone'] as String?,
      secondaryEmail: json['secondaryEmail'] as String?,
      address: json['address'] as String?,
      country: json['country'] as String?,
      state: json['state'] as String?,
      city: json['city'] as String?,
      postalCode: json['postalCode'] as String?,
      createdAt: json['createdAt'] as String?,
      updatedAt: json['updatedAt'] as String?,
    );

Map<String, dynamic> _$AcademyContactDtoToJson(_AcademyContactDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'academyId': instance.academyId,
      'primaryContactName': instance.primaryContactName,
      'primaryPhone': instance.primaryPhone,
      'primaryEmail': instance.primaryEmail,
      'secondaryContactName': instance.secondaryContactName,
      'secondaryPhone': instance.secondaryPhone,
      'secondaryEmail': instance.secondaryEmail,
      'address': instance.address,
      'country': instance.country,
      'state': instance.state,
      'city': instance.city,
      'postalCode': instance.postalCode,
      'createdAt': instance.createdAt,
      'updatedAt': instance.updatedAt,
    };

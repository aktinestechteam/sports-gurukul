// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'academy_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$AcademyDto {

 String get id; String get academyCode; String get name; String get email; String get phone; String get status; String get verificationStatus; String get createdAt; String? get legalName; String? get description; String? get registrationNumber; String? get gstNumber; String? get establishedDate; String? get website; String? get academyType; String? get logoUrl; String? get bannerUrl; String? get updatedAt; AcademyContactDto? get contact; List<AcademyBranchDto>? get branches; List<AcademySportDto>? get sports;
/// Create a copy of AcademyDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AcademyDtoCopyWith<AcademyDto> get copyWith => _$AcademyDtoCopyWithImpl<AcademyDto>(this as AcademyDto, _$identity);

  /// Serializes this AcademyDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AcademyDto&&(identical(other.id, id) || other.id == id)&&(identical(other.academyCode, academyCode) || other.academyCode == academyCode)&&(identical(other.name, name) || other.name == name)&&(identical(other.email, email) || other.email == email)&&(identical(other.phone, phone) || other.phone == phone)&&(identical(other.status, status) || other.status == status)&&(identical(other.verificationStatus, verificationStatus) || other.verificationStatus == verificationStatus)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt)&&(identical(other.legalName, legalName) || other.legalName == legalName)&&(identical(other.description, description) || other.description == description)&&(identical(other.registrationNumber, registrationNumber) || other.registrationNumber == registrationNumber)&&(identical(other.gstNumber, gstNumber) || other.gstNumber == gstNumber)&&(identical(other.establishedDate, establishedDate) || other.establishedDate == establishedDate)&&(identical(other.website, website) || other.website == website)&&(identical(other.academyType, academyType) || other.academyType == academyType)&&(identical(other.logoUrl, logoUrl) || other.logoUrl == logoUrl)&&(identical(other.bannerUrl, bannerUrl) || other.bannerUrl == bannerUrl)&&(identical(other.updatedAt, updatedAt) || other.updatedAt == updatedAt)&&(identical(other.contact, contact) || other.contact == contact)&&const DeepCollectionEquality().equals(other.branches, branches)&&const DeepCollectionEquality().equals(other.sports, sports));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hashAll([runtimeType,id,academyCode,name,email,phone,status,verificationStatus,createdAt,legalName,description,registrationNumber,gstNumber,establishedDate,website,academyType,logoUrl,bannerUrl,updatedAt,contact,const DeepCollectionEquality().hash(branches),const DeepCollectionEquality().hash(sports)]);

@override
String toString() {
  return 'AcademyDto(id: $id, academyCode: $academyCode, name: $name, email: $email, phone: $phone, status: $status, verificationStatus: $verificationStatus, createdAt: $createdAt, legalName: $legalName, description: $description, registrationNumber: $registrationNumber, gstNumber: $gstNumber, establishedDate: $establishedDate, website: $website, academyType: $academyType, logoUrl: $logoUrl, bannerUrl: $bannerUrl, updatedAt: $updatedAt, contact: $contact, branches: $branches, sports: $sports)';
}


}

/// @nodoc
abstract mixin class $AcademyDtoCopyWith<$Res>  {
  factory $AcademyDtoCopyWith(AcademyDto value, $Res Function(AcademyDto) _then) = _$AcademyDtoCopyWithImpl;
@useResult
$Res call({
 String id, String academyCode, String name, String email, String phone, String status, String verificationStatus, String createdAt, String? legalName, String? description, String? registrationNumber, String? gstNumber, String? establishedDate, String? website, String? academyType, String? logoUrl, String? bannerUrl, String? updatedAt, AcademyContactDto? contact, List<AcademyBranchDto>? branches, List<AcademySportDto>? sports
});


$AcademyContactDtoCopyWith<$Res>? get contact;

}
/// @nodoc
class _$AcademyDtoCopyWithImpl<$Res>
    implements $AcademyDtoCopyWith<$Res> {
  _$AcademyDtoCopyWithImpl(this._self, this._then);

  final AcademyDto _self;
  final $Res Function(AcademyDto) _then;

/// Create a copy of AcademyDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? academyCode = null,Object? name = null,Object? email = null,Object? phone = null,Object? status = null,Object? verificationStatus = null,Object? createdAt = null,Object? legalName = freezed,Object? description = freezed,Object? registrationNumber = freezed,Object? gstNumber = freezed,Object? establishedDate = freezed,Object? website = freezed,Object? academyType = freezed,Object? logoUrl = freezed,Object? bannerUrl = freezed,Object? updatedAt = freezed,Object? contact = freezed,Object? branches = freezed,Object? sports = freezed,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,academyCode: null == academyCode ? _self.academyCode : academyCode // ignore: cast_nullable_to_non_nullable
as String,name: null == name ? _self.name : name // ignore: cast_nullable_to_non_nullable
as String,email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,phone: null == phone ? _self.phone : phone // ignore: cast_nullable_to_non_nullable
as String,status: null == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as String,verificationStatus: null == verificationStatus ? _self.verificationStatus : verificationStatus // ignore: cast_nullable_to_non_nullable
as String,createdAt: null == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as String,legalName: freezed == legalName ? _self.legalName : legalName // ignore: cast_nullable_to_non_nullable
as String?,description: freezed == description ? _self.description : description // ignore: cast_nullable_to_non_nullable
as String?,registrationNumber: freezed == registrationNumber ? _self.registrationNumber : registrationNumber // ignore: cast_nullable_to_non_nullable
as String?,gstNumber: freezed == gstNumber ? _self.gstNumber : gstNumber // ignore: cast_nullable_to_non_nullable
as String?,establishedDate: freezed == establishedDate ? _self.establishedDate : establishedDate // ignore: cast_nullable_to_non_nullable
as String?,website: freezed == website ? _self.website : website // ignore: cast_nullable_to_non_nullable
as String?,academyType: freezed == academyType ? _self.academyType : academyType // ignore: cast_nullable_to_non_nullable
as String?,logoUrl: freezed == logoUrl ? _self.logoUrl : logoUrl // ignore: cast_nullable_to_non_nullable
as String?,bannerUrl: freezed == bannerUrl ? _self.bannerUrl : bannerUrl // ignore: cast_nullable_to_non_nullable
as String?,updatedAt: freezed == updatedAt ? _self.updatedAt : updatedAt // ignore: cast_nullable_to_non_nullable
as String?,contact: freezed == contact ? _self.contact : contact // ignore: cast_nullable_to_non_nullable
as AcademyContactDto?,branches: freezed == branches ? _self.branches : branches // ignore: cast_nullable_to_non_nullable
as List<AcademyBranchDto>?,sports: freezed == sports ? _self.sports : sports // ignore: cast_nullable_to_non_nullable
as List<AcademySportDto>?,
  ));
}
/// Create a copy of AcademyDto
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$AcademyContactDtoCopyWith<$Res>? get contact {
    if (_self.contact == null) {
    return null;
  }

  return $AcademyContactDtoCopyWith<$Res>(_self.contact!, (value) {
    return _then(_self.copyWith(contact: value));
  });
}
}


/// Adds pattern-matching-related methods to [AcademyDto].
extension AcademyDtoPatterns on AcademyDto {
/// A variant of `map` that fallback to returning `orElse`.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case _:
///     return orElse();
/// }
/// ```

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AcademyDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AcademyDto() when $default != null:
return $default(_that);case _:
  return orElse();

}
}
/// A `switch`-like method, using callbacks.
///
/// Callbacks receives the raw object, upcasted.
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case final Subclass2 value:
///     return ...;
/// }
/// ```

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AcademyDto value)  $default,){
final _that = this;
switch (_that) {
case _AcademyDto():
return $default(_that);case _:
  throw StateError('Unexpected subclass');

}
}
/// A variant of `map` that fallback to returning `null`.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case _:
///     return null;
/// }
/// ```

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AcademyDto value)?  $default,){
final _that = this;
switch (_that) {
case _AcademyDto() when $default != null:
return $default(_that);case _:
  return null;

}
}
/// A variant of `when` that fallback to an `orElse` callback.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case _:
///     return orElse();
/// }
/// ```

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String academyCode,  String name,  String email,  String phone,  String status,  String verificationStatus,  String createdAt,  String? legalName,  String? description,  String? registrationNumber,  String? gstNumber,  String? establishedDate,  String? website,  String? academyType,  String? logoUrl,  String? bannerUrl,  String? updatedAt,  AcademyContactDto? contact,  List<AcademyBranchDto>? branches,  List<AcademySportDto>? sports)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AcademyDto() when $default != null:
return $default(_that.id,_that.academyCode,_that.name,_that.email,_that.phone,_that.status,_that.verificationStatus,_that.createdAt,_that.legalName,_that.description,_that.registrationNumber,_that.gstNumber,_that.establishedDate,_that.website,_that.academyType,_that.logoUrl,_that.bannerUrl,_that.updatedAt,_that.contact,_that.branches,_that.sports);case _:
  return orElse();

}
}
/// A `switch`-like method, using callbacks.
///
/// As opposed to `map`, this offers destructuring.
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case Subclass2(:final field2):
///     return ...;
/// }
/// ```

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String academyCode,  String name,  String email,  String phone,  String status,  String verificationStatus,  String createdAt,  String? legalName,  String? description,  String? registrationNumber,  String? gstNumber,  String? establishedDate,  String? website,  String? academyType,  String? logoUrl,  String? bannerUrl,  String? updatedAt,  AcademyContactDto? contact,  List<AcademyBranchDto>? branches,  List<AcademySportDto>? sports)  $default,) {final _that = this;
switch (_that) {
case _AcademyDto():
return $default(_that.id,_that.academyCode,_that.name,_that.email,_that.phone,_that.status,_that.verificationStatus,_that.createdAt,_that.legalName,_that.description,_that.registrationNumber,_that.gstNumber,_that.establishedDate,_that.website,_that.academyType,_that.logoUrl,_that.bannerUrl,_that.updatedAt,_that.contact,_that.branches,_that.sports);case _:
  throw StateError('Unexpected subclass');

}
}
/// A variant of `when` that fallback to returning `null`
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case _:
///     return null;
/// }
/// ```

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String academyCode,  String name,  String email,  String phone,  String status,  String verificationStatus,  String createdAt,  String? legalName,  String? description,  String? registrationNumber,  String? gstNumber,  String? establishedDate,  String? website,  String? academyType,  String? logoUrl,  String? bannerUrl,  String? updatedAt,  AcademyContactDto? contact,  List<AcademyBranchDto>? branches,  List<AcademySportDto>? sports)?  $default,) {final _that = this;
switch (_that) {
case _AcademyDto() when $default != null:
return $default(_that.id,_that.academyCode,_that.name,_that.email,_that.phone,_that.status,_that.verificationStatus,_that.createdAt,_that.legalName,_that.description,_that.registrationNumber,_that.gstNumber,_that.establishedDate,_that.website,_that.academyType,_that.logoUrl,_that.bannerUrl,_that.updatedAt,_that.contact,_that.branches,_that.sports);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _AcademyDto implements AcademyDto {
  const _AcademyDto({required this.id, required this.academyCode, required this.name, required this.email, required this.phone, required this.status, required this.verificationStatus, required this.createdAt, this.legalName, this.description, this.registrationNumber, this.gstNumber, this.establishedDate, this.website, this.academyType, this.logoUrl, this.bannerUrl, this.updatedAt, this.contact, final  List<AcademyBranchDto>? branches, final  List<AcademySportDto>? sports}): _branches = branches,_sports = sports;
  factory _AcademyDto.fromJson(Map<String, dynamic> json) => _$AcademyDtoFromJson(json);

@override final  String id;
@override final  String academyCode;
@override final  String name;
@override final  String email;
@override final  String phone;
@override final  String status;
@override final  String verificationStatus;
@override final  String createdAt;
@override final  String? legalName;
@override final  String? description;
@override final  String? registrationNumber;
@override final  String? gstNumber;
@override final  String? establishedDate;
@override final  String? website;
@override final  String? academyType;
@override final  String? logoUrl;
@override final  String? bannerUrl;
@override final  String? updatedAt;
@override final  AcademyContactDto? contact;
 final  List<AcademyBranchDto>? _branches;
@override List<AcademyBranchDto>? get branches {
  final value = _branches;
  if (value == null) return null;
  if (_branches is EqualUnmodifiableListView) return _branches;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(value);
}

 final  List<AcademySportDto>? _sports;
@override List<AcademySportDto>? get sports {
  final value = _sports;
  if (value == null) return null;
  if (_sports is EqualUnmodifiableListView) return _sports;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(value);
}


/// Create a copy of AcademyDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AcademyDtoCopyWith<_AcademyDto> get copyWith => __$AcademyDtoCopyWithImpl<_AcademyDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$AcademyDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _AcademyDto&&(identical(other.id, id) || other.id == id)&&(identical(other.academyCode, academyCode) || other.academyCode == academyCode)&&(identical(other.name, name) || other.name == name)&&(identical(other.email, email) || other.email == email)&&(identical(other.phone, phone) || other.phone == phone)&&(identical(other.status, status) || other.status == status)&&(identical(other.verificationStatus, verificationStatus) || other.verificationStatus == verificationStatus)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt)&&(identical(other.legalName, legalName) || other.legalName == legalName)&&(identical(other.description, description) || other.description == description)&&(identical(other.registrationNumber, registrationNumber) || other.registrationNumber == registrationNumber)&&(identical(other.gstNumber, gstNumber) || other.gstNumber == gstNumber)&&(identical(other.establishedDate, establishedDate) || other.establishedDate == establishedDate)&&(identical(other.website, website) || other.website == website)&&(identical(other.academyType, academyType) || other.academyType == academyType)&&(identical(other.logoUrl, logoUrl) || other.logoUrl == logoUrl)&&(identical(other.bannerUrl, bannerUrl) || other.bannerUrl == bannerUrl)&&(identical(other.updatedAt, updatedAt) || other.updatedAt == updatedAt)&&(identical(other.contact, contact) || other.contact == contact)&&const DeepCollectionEquality().equals(other._branches, _branches)&&const DeepCollectionEquality().equals(other._sports, _sports));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hashAll([runtimeType,id,academyCode,name,email,phone,status,verificationStatus,createdAt,legalName,description,registrationNumber,gstNumber,establishedDate,website,academyType,logoUrl,bannerUrl,updatedAt,contact,const DeepCollectionEquality().hash(_branches),const DeepCollectionEquality().hash(_sports)]);

@override
String toString() {
  return 'AcademyDto(id: $id, academyCode: $academyCode, name: $name, email: $email, phone: $phone, status: $status, verificationStatus: $verificationStatus, createdAt: $createdAt, legalName: $legalName, description: $description, registrationNumber: $registrationNumber, gstNumber: $gstNumber, establishedDate: $establishedDate, website: $website, academyType: $academyType, logoUrl: $logoUrl, bannerUrl: $bannerUrl, updatedAt: $updatedAt, contact: $contact, branches: $branches, sports: $sports)';
}


}

/// @nodoc
abstract mixin class _$AcademyDtoCopyWith<$Res> implements $AcademyDtoCopyWith<$Res> {
  factory _$AcademyDtoCopyWith(_AcademyDto value, $Res Function(_AcademyDto) _then) = __$AcademyDtoCopyWithImpl;
@override @useResult
$Res call({
 String id, String academyCode, String name, String email, String phone, String status, String verificationStatus, String createdAt, String? legalName, String? description, String? registrationNumber, String? gstNumber, String? establishedDate, String? website, String? academyType, String? logoUrl, String? bannerUrl, String? updatedAt, AcademyContactDto? contact, List<AcademyBranchDto>? branches, List<AcademySportDto>? sports
});


@override $AcademyContactDtoCopyWith<$Res>? get contact;

}
/// @nodoc
class __$AcademyDtoCopyWithImpl<$Res>
    implements _$AcademyDtoCopyWith<$Res> {
  __$AcademyDtoCopyWithImpl(this._self, this._then);

  final _AcademyDto _self;
  final $Res Function(_AcademyDto) _then;

/// Create a copy of AcademyDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? academyCode = null,Object? name = null,Object? email = null,Object? phone = null,Object? status = null,Object? verificationStatus = null,Object? createdAt = null,Object? legalName = freezed,Object? description = freezed,Object? registrationNumber = freezed,Object? gstNumber = freezed,Object? establishedDate = freezed,Object? website = freezed,Object? academyType = freezed,Object? logoUrl = freezed,Object? bannerUrl = freezed,Object? updatedAt = freezed,Object? contact = freezed,Object? branches = freezed,Object? sports = freezed,}) {
  return _then(_AcademyDto(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,academyCode: null == academyCode ? _self.academyCode : academyCode // ignore: cast_nullable_to_non_nullable
as String,name: null == name ? _self.name : name // ignore: cast_nullable_to_non_nullable
as String,email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,phone: null == phone ? _self.phone : phone // ignore: cast_nullable_to_non_nullable
as String,status: null == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as String,verificationStatus: null == verificationStatus ? _self.verificationStatus : verificationStatus // ignore: cast_nullable_to_non_nullable
as String,createdAt: null == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as String,legalName: freezed == legalName ? _self.legalName : legalName // ignore: cast_nullable_to_non_nullable
as String?,description: freezed == description ? _self.description : description // ignore: cast_nullable_to_non_nullable
as String?,registrationNumber: freezed == registrationNumber ? _self.registrationNumber : registrationNumber // ignore: cast_nullable_to_non_nullable
as String?,gstNumber: freezed == gstNumber ? _self.gstNumber : gstNumber // ignore: cast_nullable_to_non_nullable
as String?,establishedDate: freezed == establishedDate ? _self.establishedDate : establishedDate // ignore: cast_nullable_to_non_nullable
as String?,website: freezed == website ? _self.website : website // ignore: cast_nullable_to_non_nullable
as String?,academyType: freezed == academyType ? _self.academyType : academyType // ignore: cast_nullable_to_non_nullable
as String?,logoUrl: freezed == logoUrl ? _self.logoUrl : logoUrl // ignore: cast_nullable_to_non_nullable
as String?,bannerUrl: freezed == bannerUrl ? _self.bannerUrl : bannerUrl // ignore: cast_nullable_to_non_nullable
as String?,updatedAt: freezed == updatedAt ? _self.updatedAt : updatedAt // ignore: cast_nullable_to_non_nullable
as String?,contact: freezed == contact ? _self.contact : contact // ignore: cast_nullable_to_non_nullable
as AcademyContactDto?,branches: freezed == branches ? _self._branches : branches // ignore: cast_nullable_to_non_nullable
as List<AcademyBranchDto>?,sports: freezed == sports ? _self._sports : sports // ignore: cast_nullable_to_non_nullable
as List<AcademySportDto>?,
  ));
}

/// Create a copy of AcademyDto
/// with the given fields replaced by the non-null parameter values.
@override
@pragma('vm:prefer-inline')
$AcademyContactDtoCopyWith<$Res>? get contact {
    if (_self.contact == null) {
    return null;
  }

  return $AcademyContactDtoCopyWith<$Res>(_self.contact!, (value) {
    return _then(_self.copyWith(contact: value));
  });
}
}

// dart format on

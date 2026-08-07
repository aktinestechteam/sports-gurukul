// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'create_academy_request_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$CreateAcademyRequestDto {

 String get name; String get email; String get phone; String? get legalName; String? get description; String? get registrationNumber; String? get gstNumber; String? get establishedDate; String? get website; String? get academyType; List<String> get sportNames; String? get primaryContactName; String? get address; String? get country; String? get state; String? get city; String? get postalCode;
/// Create a copy of CreateAcademyRequestDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$CreateAcademyRequestDtoCopyWith<CreateAcademyRequestDto> get copyWith => _$CreateAcademyRequestDtoCopyWithImpl<CreateAcademyRequestDto>(this as CreateAcademyRequestDto, _$identity);

  /// Serializes this CreateAcademyRequestDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is CreateAcademyRequestDto&&(identical(other.name, name) || other.name == name)&&(identical(other.email, email) || other.email == email)&&(identical(other.phone, phone) || other.phone == phone)&&(identical(other.legalName, legalName) || other.legalName == legalName)&&(identical(other.description, description) || other.description == description)&&(identical(other.registrationNumber, registrationNumber) || other.registrationNumber == registrationNumber)&&(identical(other.gstNumber, gstNumber) || other.gstNumber == gstNumber)&&(identical(other.establishedDate, establishedDate) || other.establishedDate == establishedDate)&&(identical(other.website, website) || other.website == website)&&(identical(other.academyType, academyType) || other.academyType == academyType)&&const DeepCollectionEquality().equals(other.sportNames, sportNames)&&(identical(other.primaryContactName, primaryContactName) || other.primaryContactName == primaryContactName)&&(identical(other.address, address) || other.address == address)&&(identical(other.country, country) || other.country == country)&&(identical(other.state, state) || other.state == state)&&(identical(other.city, city) || other.city == city)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,name,email,phone,legalName,description,registrationNumber,gstNumber,establishedDate,website,academyType,const DeepCollectionEquality().hash(sportNames),primaryContactName,address,country,state,city,postalCode);

@override
String toString() {
  return 'CreateAcademyRequestDto(name: $name, email: $email, phone: $phone, legalName: $legalName, description: $description, registrationNumber: $registrationNumber, gstNumber: $gstNumber, establishedDate: $establishedDate, website: $website, academyType: $academyType, sportNames: $sportNames, primaryContactName: $primaryContactName, address: $address, country: $country, state: $state, city: $city, postalCode: $postalCode)';
}


}

/// @nodoc
abstract mixin class $CreateAcademyRequestDtoCopyWith<$Res>  {
  factory $CreateAcademyRequestDtoCopyWith(CreateAcademyRequestDto value, $Res Function(CreateAcademyRequestDto) _then) = _$CreateAcademyRequestDtoCopyWithImpl;
@useResult
$Res call({
 String name, String email, String phone, String? legalName, String? description, String? registrationNumber, String? gstNumber, String? establishedDate, String? website, String? academyType, List<String> sportNames, String? primaryContactName, String? address, String? country, String? state, String? city, String? postalCode
});




}
/// @nodoc
class _$CreateAcademyRequestDtoCopyWithImpl<$Res>
    implements $CreateAcademyRequestDtoCopyWith<$Res> {
  _$CreateAcademyRequestDtoCopyWithImpl(this._self, this._then);

  final CreateAcademyRequestDto _self;
  final $Res Function(CreateAcademyRequestDto) _then;

/// Create a copy of CreateAcademyRequestDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? name = null,Object? email = null,Object? phone = null,Object? legalName = freezed,Object? description = freezed,Object? registrationNumber = freezed,Object? gstNumber = freezed,Object? establishedDate = freezed,Object? website = freezed,Object? academyType = freezed,Object? sportNames = null,Object? primaryContactName = freezed,Object? address = freezed,Object? country = freezed,Object? state = freezed,Object? city = freezed,Object? postalCode = freezed,}) {
  return _then(_self.copyWith(
name: null == name ? _self.name : name // ignore: cast_nullable_to_non_nullable
as String,email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,phone: null == phone ? _self.phone : phone // ignore: cast_nullable_to_non_nullable
as String,legalName: freezed == legalName ? _self.legalName : legalName // ignore: cast_nullable_to_non_nullable
as String?,description: freezed == description ? _self.description : description // ignore: cast_nullable_to_non_nullable
as String?,registrationNumber: freezed == registrationNumber ? _self.registrationNumber : registrationNumber // ignore: cast_nullable_to_non_nullable
as String?,gstNumber: freezed == gstNumber ? _self.gstNumber : gstNumber // ignore: cast_nullable_to_non_nullable
as String?,establishedDate: freezed == establishedDate ? _self.establishedDate : establishedDate // ignore: cast_nullable_to_non_nullable
as String?,website: freezed == website ? _self.website : website // ignore: cast_nullable_to_non_nullable
as String?,academyType: freezed == academyType ? _self.academyType : academyType // ignore: cast_nullable_to_non_nullable
as String?,sportNames: null == sportNames ? _self.sportNames : sportNames // ignore: cast_nullable_to_non_nullable
as List<String>,primaryContactName: freezed == primaryContactName ? _self.primaryContactName : primaryContactName // ignore: cast_nullable_to_non_nullable
as String?,address: freezed == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String?,country: freezed == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String?,state: freezed == state ? _self.state : state // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String?,postalCode: freezed == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [CreateAcademyRequestDto].
extension CreateAcademyRequestDtoPatterns on CreateAcademyRequestDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _CreateAcademyRequestDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _CreateAcademyRequestDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _CreateAcademyRequestDto value)  $default,){
final _that = this;
switch (_that) {
case _CreateAcademyRequestDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _CreateAcademyRequestDto value)?  $default,){
final _that = this;
switch (_that) {
case _CreateAcademyRequestDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String name,  String email,  String phone,  String? legalName,  String? description,  String? registrationNumber,  String? gstNumber,  String? establishedDate,  String? website,  String? academyType,  List<String> sportNames,  String? primaryContactName,  String? address,  String? country,  String? state,  String? city,  String? postalCode)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _CreateAcademyRequestDto() when $default != null:
return $default(_that.name,_that.email,_that.phone,_that.legalName,_that.description,_that.registrationNumber,_that.gstNumber,_that.establishedDate,_that.website,_that.academyType,_that.sportNames,_that.primaryContactName,_that.address,_that.country,_that.state,_that.city,_that.postalCode);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String name,  String email,  String phone,  String? legalName,  String? description,  String? registrationNumber,  String? gstNumber,  String? establishedDate,  String? website,  String? academyType,  List<String> sportNames,  String? primaryContactName,  String? address,  String? country,  String? state,  String? city,  String? postalCode)  $default,) {final _that = this;
switch (_that) {
case _CreateAcademyRequestDto():
return $default(_that.name,_that.email,_that.phone,_that.legalName,_that.description,_that.registrationNumber,_that.gstNumber,_that.establishedDate,_that.website,_that.academyType,_that.sportNames,_that.primaryContactName,_that.address,_that.country,_that.state,_that.city,_that.postalCode);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String name,  String email,  String phone,  String? legalName,  String? description,  String? registrationNumber,  String? gstNumber,  String? establishedDate,  String? website,  String? academyType,  List<String> sportNames,  String? primaryContactName,  String? address,  String? country,  String? state,  String? city,  String? postalCode)?  $default,) {final _that = this;
switch (_that) {
case _CreateAcademyRequestDto() when $default != null:
return $default(_that.name,_that.email,_that.phone,_that.legalName,_that.description,_that.registrationNumber,_that.gstNumber,_that.establishedDate,_that.website,_that.academyType,_that.sportNames,_that.primaryContactName,_that.address,_that.country,_that.state,_that.city,_that.postalCode);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _CreateAcademyRequestDto implements CreateAcademyRequestDto {
  const _CreateAcademyRequestDto({required this.name, required this.email, required this.phone, this.legalName, this.description, this.registrationNumber, this.gstNumber, this.establishedDate, this.website, this.academyType, final  List<String> sportNames = const <String>[], this.primaryContactName, this.address, this.country, this.state, this.city, this.postalCode}): _sportNames = sportNames;
  factory _CreateAcademyRequestDto.fromJson(Map<String, dynamic> json) => _$CreateAcademyRequestDtoFromJson(json);

@override final  String name;
@override final  String email;
@override final  String phone;
@override final  String? legalName;
@override final  String? description;
@override final  String? registrationNumber;
@override final  String? gstNumber;
@override final  String? establishedDate;
@override final  String? website;
@override final  String? academyType;
 final  List<String> _sportNames;
@override@JsonKey() List<String> get sportNames {
  if (_sportNames is EqualUnmodifiableListView) return _sportNames;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_sportNames);
}

@override final  String? primaryContactName;
@override final  String? address;
@override final  String? country;
@override final  String? state;
@override final  String? city;
@override final  String? postalCode;

/// Create a copy of CreateAcademyRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$CreateAcademyRequestDtoCopyWith<_CreateAcademyRequestDto> get copyWith => __$CreateAcademyRequestDtoCopyWithImpl<_CreateAcademyRequestDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$CreateAcademyRequestDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _CreateAcademyRequestDto&&(identical(other.name, name) || other.name == name)&&(identical(other.email, email) || other.email == email)&&(identical(other.phone, phone) || other.phone == phone)&&(identical(other.legalName, legalName) || other.legalName == legalName)&&(identical(other.description, description) || other.description == description)&&(identical(other.registrationNumber, registrationNumber) || other.registrationNumber == registrationNumber)&&(identical(other.gstNumber, gstNumber) || other.gstNumber == gstNumber)&&(identical(other.establishedDate, establishedDate) || other.establishedDate == establishedDate)&&(identical(other.website, website) || other.website == website)&&(identical(other.academyType, academyType) || other.academyType == academyType)&&const DeepCollectionEquality().equals(other._sportNames, _sportNames)&&(identical(other.primaryContactName, primaryContactName) || other.primaryContactName == primaryContactName)&&(identical(other.address, address) || other.address == address)&&(identical(other.country, country) || other.country == country)&&(identical(other.state, state) || other.state == state)&&(identical(other.city, city) || other.city == city)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,name,email,phone,legalName,description,registrationNumber,gstNumber,establishedDate,website,academyType,const DeepCollectionEquality().hash(_sportNames),primaryContactName,address,country,state,city,postalCode);

@override
String toString() {
  return 'CreateAcademyRequestDto(name: $name, email: $email, phone: $phone, legalName: $legalName, description: $description, registrationNumber: $registrationNumber, gstNumber: $gstNumber, establishedDate: $establishedDate, website: $website, academyType: $academyType, sportNames: $sportNames, primaryContactName: $primaryContactName, address: $address, country: $country, state: $state, city: $city, postalCode: $postalCode)';
}


}

/// @nodoc
abstract mixin class _$CreateAcademyRequestDtoCopyWith<$Res> implements $CreateAcademyRequestDtoCopyWith<$Res> {
  factory _$CreateAcademyRequestDtoCopyWith(_CreateAcademyRequestDto value, $Res Function(_CreateAcademyRequestDto) _then) = __$CreateAcademyRequestDtoCopyWithImpl;
@override @useResult
$Res call({
 String name, String email, String phone, String? legalName, String? description, String? registrationNumber, String? gstNumber, String? establishedDate, String? website, String? academyType, List<String> sportNames, String? primaryContactName, String? address, String? country, String? state, String? city, String? postalCode
});




}
/// @nodoc
class __$CreateAcademyRequestDtoCopyWithImpl<$Res>
    implements _$CreateAcademyRequestDtoCopyWith<$Res> {
  __$CreateAcademyRequestDtoCopyWithImpl(this._self, this._then);

  final _CreateAcademyRequestDto _self;
  final $Res Function(_CreateAcademyRequestDto) _then;

/// Create a copy of CreateAcademyRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? name = null,Object? email = null,Object? phone = null,Object? legalName = freezed,Object? description = freezed,Object? registrationNumber = freezed,Object? gstNumber = freezed,Object? establishedDate = freezed,Object? website = freezed,Object? academyType = freezed,Object? sportNames = null,Object? primaryContactName = freezed,Object? address = freezed,Object? country = freezed,Object? state = freezed,Object? city = freezed,Object? postalCode = freezed,}) {
  return _then(_CreateAcademyRequestDto(
name: null == name ? _self.name : name // ignore: cast_nullable_to_non_nullable
as String,email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,phone: null == phone ? _self.phone : phone // ignore: cast_nullable_to_non_nullable
as String,legalName: freezed == legalName ? _self.legalName : legalName // ignore: cast_nullable_to_non_nullable
as String?,description: freezed == description ? _self.description : description // ignore: cast_nullable_to_non_nullable
as String?,registrationNumber: freezed == registrationNumber ? _self.registrationNumber : registrationNumber // ignore: cast_nullable_to_non_nullable
as String?,gstNumber: freezed == gstNumber ? _self.gstNumber : gstNumber // ignore: cast_nullable_to_non_nullable
as String?,establishedDate: freezed == establishedDate ? _self.establishedDate : establishedDate // ignore: cast_nullable_to_non_nullable
as String?,website: freezed == website ? _self.website : website // ignore: cast_nullable_to_non_nullable
as String?,academyType: freezed == academyType ? _self.academyType : academyType // ignore: cast_nullable_to_non_nullable
as String?,sportNames: null == sportNames ? _self._sportNames : sportNames // ignore: cast_nullable_to_non_nullable
as List<String>,primaryContactName: freezed == primaryContactName ? _self.primaryContactName : primaryContactName // ignore: cast_nullable_to_non_nullable
as String?,address: freezed == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String?,country: freezed == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String?,state: freezed == state ? _self.state : state // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String?,postalCode: freezed == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}

// dart format on

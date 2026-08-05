// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'update_profile_request_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$UpdateProfileRequestDto {

 String? get dateOfBirth;@JsonKey(unknownEnumValue: GenderDto.preferNotToSay) GenderDto? get gender; String? get bio; String? get height; String? get weight; String? get preferredSport; String? get experienceLevel; String? get primaryPhoneCountryCode; String? get primaryPhoneNumber; String? get addressLine1; String? get addressLine2; String? get city; String? get state; String? get country; String? get postalCode;@JsonKey(unknownEnumValue: AddressTypeDto.home) AddressTypeDto? get addressType;
/// Create a copy of UpdateProfileRequestDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$UpdateProfileRequestDtoCopyWith<UpdateProfileRequestDto> get copyWith => _$UpdateProfileRequestDtoCopyWithImpl<UpdateProfileRequestDto>(this as UpdateProfileRequestDto, _$identity);

  /// Serializes this UpdateProfileRequestDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is UpdateProfileRequestDto&&(identical(other.dateOfBirth, dateOfBirth) || other.dateOfBirth == dateOfBirth)&&(identical(other.gender, gender) || other.gender == gender)&&(identical(other.bio, bio) || other.bio == bio)&&(identical(other.height, height) || other.height == height)&&(identical(other.weight, weight) || other.weight == weight)&&(identical(other.preferredSport, preferredSport) || other.preferredSport == preferredSport)&&(identical(other.experienceLevel, experienceLevel) || other.experienceLevel == experienceLevel)&&(identical(other.primaryPhoneCountryCode, primaryPhoneCountryCode) || other.primaryPhoneCountryCode == primaryPhoneCountryCode)&&(identical(other.primaryPhoneNumber, primaryPhoneNumber) || other.primaryPhoneNumber == primaryPhoneNumber)&&(identical(other.addressLine1, addressLine1) || other.addressLine1 == addressLine1)&&(identical(other.addressLine2, addressLine2) || other.addressLine2 == addressLine2)&&(identical(other.city, city) || other.city == city)&&(identical(other.state, state) || other.state == state)&&(identical(other.country, country) || other.country == country)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode)&&(identical(other.addressType, addressType) || other.addressType == addressType));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,dateOfBirth,gender,bio,height,weight,preferredSport,experienceLevel,primaryPhoneCountryCode,primaryPhoneNumber,addressLine1,addressLine2,city,state,country,postalCode,addressType);

@override
String toString() {
  return 'UpdateProfileRequestDto(dateOfBirth: $dateOfBirth, gender: $gender, bio: $bio, height: $height, weight: $weight, preferredSport: $preferredSport, experienceLevel: $experienceLevel, primaryPhoneCountryCode: $primaryPhoneCountryCode, primaryPhoneNumber: $primaryPhoneNumber, addressLine1: $addressLine1, addressLine2: $addressLine2, city: $city, state: $state, country: $country, postalCode: $postalCode, addressType: $addressType)';
}


}

/// @nodoc
abstract mixin class $UpdateProfileRequestDtoCopyWith<$Res>  {
  factory $UpdateProfileRequestDtoCopyWith(UpdateProfileRequestDto value, $Res Function(UpdateProfileRequestDto) _then) = _$UpdateProfileRequestDtoCopyWithImpl;
@useResult
$Res call({
 String? dateOfBirth,@JsonKey(unknownEnumValue: GenderDto.preferNotToSay) GenderDto? gender, String? bio, String? height, String? weight, String? preferredSport, String? experienceLevel, String? primaryPhoneCountryCode, String? primaryPhoneNumber, String? addressLine1, String? addressLine2, String? city, String? state, String? country, String? postalCode,@JsonKey(unknownEnumValue: AddressTypeDto.home) AddressTypeDto? addressType
});




}
/// @nodoc
class _$UpdateProfileRequestDtoCopyWithImpl<$Res>
    implements $UpdateProfileRequestDtoCopyWith<$Res> {
  _$UpdateProfileRequestDtoCopyWithImpl(this._self, this._then);

  final UpdateProfileRequestDto _self;
  final $Res Function(UpdateProfileRequestDto) _then;

/// Create a copy of UpdateProfileRequestDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? dateOfBirth = freezed,Object? gender = freezed,Object? bio = freezed,Object? height = freezed,Object? weight = freezed,Object? preferredSport = freezed,Object? experienceLevel = freezed,Object? primaryPhoneCountryCode = freezed,Object? primaryPhoneNumber = freezed,Object? addressLine1 = freezed,Object? addressLine2 = freezed,Object? city = freezed,Object? state = freezed,Object? country = freezed,Object? postalCode = freezed,Object? addressType = freezed,}) {
  return _then(_self.copyWith(
dateOfBirth: freezed == dateOfBirth ? _self.dateOfBirth : dateOfBirth // ignore: cast_nullable_to_non_nullable
as String?,gender: freezed == gender ? _self.gender : gender // ignore: cast_nullable_to_non_nullable
as GenderDto?,bio: freezed == bio ? _self.bio : bio // ignore: cast_nullable_to_non_nullable
as String?,height: freezed == height ? _self.height : height // ignore: cast_nullable_to_non_nullable
as String?,weight: freezed == weight ? _self.weight : weight // ignore: cast_nullable_to_non_nullable
as String?,preferredSport: freezed == preferredSport ? _self.preferredSport : preferredSport // ignore: cast_nullable_to_non_nullable
as String?,experienceLevel: freezed == experienceLevel ? _self.experienceLevel : experienceLevel // ignore: cast_nullable_to_non_nullable
as String?,primaryPhoneCountryCode: freezed == primaryPhoneCountryCode ? _self.primaryPhoneCountryCode : primaryPhoneCountryCode // ignore: cast_nullable_to_non_nullable
as String?,primaryPhoneNumber: freezed == primaryPhoneNumber ? _self.primaryPhoneNumber : primaryPhoneNumber // ignore: cast_nullable_to_non_nullable
as String?,addressLine1: freezed == addressLine1 ? _self.addressLine1 : addressLine1 // ignore: cast_nullable_to_non_nullable
as String?,addressLine2: freezed == addressLine2 ? _self.addressLine2 : addressLine2 // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String?,state: freezed == state ? _self.state : state // ignore: cast_nullable_to_non_nullable
as String?,country: freezed == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String?,postalCode: freezed == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String?,addressType: freezed == addressType ? _self.addressType : addressType // ignore: cast_nullable_to_non_nullable
as AddressTypeDto?,
  ));
}

}


/// Adds pattern-matching-related methods to [UpdateProfileRequestDto].
extension UpdateProfileRequestDtoPatterns on UpdateProfileRequestDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _UpdateProfileRequestDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _UpdateProfileRequestDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _UpdateProfileRequestDto value)  $default,){
final _that = this;
switch (_that) {
case _UpdateProfileRequestDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _UpdateProfileRequestDto value)?  $default,){
final _that = this;
switch (_that) {
case _UpdateProfileRequestDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? dateOfBirth, @JsonKey(unknownEnumValue: GenderDto.preferNotToSay)  GenderDto? gender,  String? bio,  String? height,  String? weight,  String? preferredSport,  String? experienceLevel,  String? primaryPhoneCountryCode,  String? primaryPhoneNumber,  String? addressLine1,  String? addressLine2,  String? city,  String? state,  String? country,  String? postalCode, @JsonKey(unknownEnumValue: AddressTypeDto.home)  AddressTypeDto? addressType)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _UpdateProfileRequestDto() when $default != null:
return $default(_that.dateOfBirth,_that.gender,_that.bio,_that.height,_that.weight,_that.preferredSport,_that.experienceLevel,_that.primaryPhoneCountryCode,_that.primaryPhoneNumber,_that.addressLine1,_that.addressLine2,_that.city,_that.state,_that.country,_that.postalCode,_that.addressType);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? dateOfBirth, @JsonKey(unknownEnumValue: GenderDto.preferNotToSay)  GenderDto? gender,  String? bio,  String? height,  String? weight,  String? preferredSport,  String? experienceLevel,  String? primaryPhoneCountryCode,  String? primaryPhoneNumber,  String? addressLine1,  String? addressLine2,  String? city,  String? state,  String? country,  String? postalCode, @JsonKey(unknownEnumValue: AddressTypeDto.home)  AddressTypeDto? addressType)  $default,) {final _that = this;
switch (_that) {
case _UpdateProfileRequestDto():
return $default(_that.dateOfBirth,_that.gender,_that.bio,_that.height,_that.weight,_that.preferredSport,_that.experienceLevel,_that.primaryPhoneCountryCode,_that.primaryPhoneNumber,_that.addressLine1,_that.addressLine2,_that.city,_that.state,_that.country,_that.postalCode,_that.addressType);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? dateOfBirth, @JsonKey(unknownEnumValue: GenderDto.preferNotToSay)  GenderDto? gender,  String? bio,  String? height,  String? weight,  String? preferredSport,  String? experienceLevel,  String? primaryPhoneCountryCode,  String? primaryPhoneNumber,  String? addressLine1,  String? addressLine2,  String? city,  String? state,  String? country,  String? postalCode, @JsonKey(unknownEnumValue: AddressTypeDto.home)  AddressTypeDto? addressType)?  $default,) {final _that = this;
switch (_that) {
case _UpdateProfileRequestDto() when $default != null:
return $default(_that.dateOfBirth,_that.gender,_that.bio,_that.height,_that.weight,_that.preferredSport,_that.experienceLevel,_that.primaryPhoneCountryCode,_that.primaryPhoneNumber,_that.addressLine1,_that.addressLine2,_that.city,_that.state,_that.country,_that.postalCode,_that.addressType);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _UpdateProfileRequestDto implements UpdateProfileRequestDto {
  const _UpdateProfileRequestDto({this.dateOfBirth, @JsonKey(unknownEnumValue: GenderDto.preferNotToSay) this.gender, this.bio, this.height, this.weight, this.preferredSport, this.experienceLevel, this.primaryPhoneCountryCode, this.primaryPhoneNumber, this.addressLine1, this.addressLine2, this.city, this.state, this.country, this.postalCode, @JsonKey(unknownEnumValue: AddressTypeDto.home) this.addressType});
  factory _UpdateProfileRequestDto.fromJson(Map<String, dynamic> json) => _$UpdateProfileRequestDtoFromJson(json);

@override final  String? dateOfBirth;
@override@JsonKey(unknownEnumValue: GenderDto.preferNotToSay) final  GenderDto? gender;
@override final  String? bio;
@override final  String? height;
@override final  String? weight;
@override final  String? preferredSport;
@override final  String? experienceLevel;
@override final  String? primaryPhoneCountryCode;
@override final  String? primaryPhoneNumber;
@override final  String? addressLine1;
@override final  String? addressLine2;
@override final  String? city;
@override final  String? state;
@override final  String? country;
@override final  String? postalCode;
@override@JsonKey(unknownEnumValue: AddressTypeDto.home) final  AddressTypeDto? addressType;

/// Create a copy of UpdateProfileRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$UpdateProfileRequestDtoCopyWith<_UpdateProfileRequestDto> get copyWith => __$UpdateProfileRequestDtoCopyWithImpl<_UpdateProfileRequestDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$UpdateProfileRequestDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _UpdateProfileRequestDto&&(identical(other.dateOfBirth, dateOfBirth) || other.dateOfBirth == dateOfBirth)&&(identical(other.gender, gender) || other.gender == gender)&&(identical(other.bio, bio) || other.bio == bio)&&(identical(other.height, height) || other.height == height)&&(identical(other.weight, weight) || other.weight == weight)&&(identical(other.preferredSport, preferredSport) || other.preferredSport == preferredSport)&&(identical(other.experienceLevel, experienceLevel) || other.experienceLevel == experienceLevel)&&(identical(other.primaryPhoneCountryCode, primaryPhoneCountryCode) || other.primaryPhoneCountryCode == primaryPhoneCountryCode)&&(identical(other.primaryPhoneNumber, primaryPhoneNumber) || other.primaryPhoneNumber == primaryPhoneNumber)&&(identical(other.addressLine1, addressLine1) || other.addressLine1 == addressLine1)&&(identical(other.addressLine2, addressLine2) || other.addressLine2 == addressLine2)&&(identical(other.city, city) || other.city == city)&&(identical(other.state, state) || other.state == state)&&(identical(other.country, country) || other.country == country)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode)&&(identical(other.addressType, addressType) || other.addressType == addressType));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,dateOfBirth,gender,bio,height,weight,preferredSport,experienceLevel,primaryPhoneCountryCode,primaryPhoneNumber,addressLine1,addressLine2,city,state,country,postalCode,addressType);

@override
String toString() {
  return 'UpdateProfileRequestDto(dateOfBirth: $dateOfBirth, gender: $gender, bio: $bio, height: $height, weight: $weight, preferredSport: $preferredSport, experienceLevel: $experienceLevel, primaryPhoneCountryCode: $primaryPhoneCountryCode, primaryPhoneNumber: $primaryPhoneNumber, addressLine1: $addressLine1, addressLine2: $addressLine2, city: $city, state: $state, country: $country, postalCode: $postalCode, addressType: $addressType)';
}


}

/// @nodoc
abstract mixin class _$UpdateProfileRequestDtoCopyWith<$Res> implements $UpdateProfileRequestDtoCopyWith<$Res> {
  factory _$UpdateProfileRequestDtoCopyWith(_UpdateProfileRequestDto value, $Res Function(_UpdateProfileRequestDto) _then) = __$UpdateProfileRequestDtoCopyWithImpl;
@override @useResult
$Res call({
 String? dateOfBirth,@JsonKey(unknownEnumValue: GenderDto.preferNotToSay) GenderDto? gender, String? bio, String? height, String? weight, String? preferredSport, String? experienceLevel, String? primaryPhoneCountryCode, String? primaryPhoneNumber, String? addressLine1, String? addressLine2, String? city, String? state, String? country, String? postalCode,@JsonKey(unknownEnumValue: AddressTypeDto.home) AddressTypeDto? addressType
});




}
/// @nodoc
class __$UpdateProfileRequestDtoCopyWithImpl<$Res>
    implements _$UpdateProfileRequestDtoCopyWith<$Res> {
  __$UpdateProfileRequestDtoCopyWithImpl(this._self, this._then);

  final _UpdateProfileRequestDto _self;
  final $Res Function(_UpdateProfileRequestDto) _then;

/// Create a copy of UpdateProfileRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? dateOfBirth = freezed,Object? gender = freezed,Object? bio = freezed,Object? height = freezed,Object? weight = freezed,Object? preferredSport = freezed,Object? experienceLevel = freezed,Object? primaryPhoneCountryCode = freezed,Object? primaryPhoneNumber = freezed,Object? addressLine1 = freezed,Object? addressLine2 = freezed,Object? city = freezed,Object? state = freezed,Object? country = freezed,Object? postalCode = freezed,Object? addressType = freezed,}) {
  return _then(_UpdateProfileRequestDto(
dateOfBirth: freezed == dateOfBirth ? _self.dateOfBirth : dateOfBirth // ignore: cast_nullable_to_non_nullable
as String?,gender: freezed == gender ? _self.gender : gender // ignore: cast_nullable_to_non_nullable
as GenderDto?,bio: freezed == bio ? _self.bio : bio // ignore: cast_nullable_to_non_nullable
as String?,height: freezed == height ? _self.height : height // ignore: cast_nullable_to_non_nullable
as String?,weight: freezed == weight ? _self.weight : weight // ignore: cast_nullable_to_non_nullable
as String?,preferredSport: freezed == preferredSport ? _self.preferredSport : preferredSport // ignore: cast_nullable_to_non_nullable
as String?,experienceLevel: freezed == experienceLevel ? _self.experienceLevel : experienceLevel // ignore: cast_nullable_to_non_nullable
as String?,primaryPhoneCountryCode: freezed == primaryPhoneCountryCode ? _self.primaryPhoneCountryCode : primaryPhoneCountryCode // ignore: cast_nullable_to_non_nullable
as String?,primaryPhoneNumber: freezed == primaryPhoneNumber ? _self.primaryPhoneNumber : primaryPhoneNumber // ignore: cast_nullable_to_non_nullable
as String?,addressLine1: freezed == addressLine1 ? _self.addressLine1 : addressLine1 // ignore: cast_nullable_to_non_nullable
as String?,addressLine2: freezed == addressLine2 ? _self.addressLine2 : addressLine2 // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String?,state: freezed == state ? _self.state : state // ignore: cast_nullable_to_non_nullable
as String?,country: freezed == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String?,postalCode: freezed == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String?,addressType: freezed == addressType ? _self.addressType : addressType // ignore: cast_nullable_to_non_nullable
as AddressTypeDto?,
  ));
}


}

// dart format on

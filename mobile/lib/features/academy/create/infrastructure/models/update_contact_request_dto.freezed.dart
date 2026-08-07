// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'update_contact_request_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$UpdateContactRequestDto {

 String? get primaryContactName; String? get primaryPhone; String? get primaryEmail; String? get secondaryContactName; String? get secondaryPhone; String? get secondaryEmail; String? get address; String? get country; String? get state; String? get city; String? get postalCode;
/// Create a copy of UpdateContactRequestDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$UpdateContactRequestDtoCopyWith<UpdateContactRequestDto> get copyWith => _$UpdateContactRequestDtoCopyWithImpl<UpdateContactRequestDto>(this as UpdateContactRequestDto, _$identity);

  /// Serializes this UpdateContactRequestDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is UpdateContactRequestDto&&(identical(other.primaryContactName, primaryContactName) || other.primaryContactName == primaryContactName)&&(identical(other.primaryPhone, primaryPhone) || other.primaryPhone == primaryPhone)&&(identical(other.primaryEmail, primaryEmail) || other.primaryEmail == primaryEmail)&&(identical(other.secondaryContactName, secondaryContactName) || other.secondaryContactName == secondaryContactName)&&(identical(other.secondaryPhone, secondaryPhone) || other.secondaryPhone == secondaryPhone)&&(identical(other.secondaryEmail, secondaryEmail) || other.secondaryEmail == secondaryEmail)&&(identical(other.address, address) || other.address == address)&&(identical(other.country, country) || other.country == country)&&(identical(other.state, state) || other.state == state)&&(identical(other.city, city) || other.city == city)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,primaryContactName,primaryPhone,primaryEmail,secondaryContactName,secondaryPhone,secondaryEmail,address,country,state,city,postalCode);

@override
String toString() {
  return 'UpdateContactRequestDto(primaryContactName: $primaryContactName, primaryPhone: $primaryPhone, primaryEmail: $primaryEmail, secondaryContactName: $secondaryContactName, secondaryPhone: $secondaryPhone, secondaryEmail: $secondaryEmail, address: $address, country: $country, state: $state, city: $city, postalCode: $postalCode)';
}


}

/// @nodoc
abstract mixin class $UpdateContactRequestDtoCopyWith<$Res>  {
  factory $UpdateContactRequestDtoCopyWith(UpdateContactRequestDto value, $Res Function(UpdateContactRequestDto) _then) = _$UpdateContactRequestDtoCopyWithImpl;
@useResult
$Res call({
 String? primaryContactName, String? primaryPhone, String? primaryEmail, String? secondaryContactName, String? secondaryPhone, String? secondaryEmail, String? address, String? country, String? state, String? city, String? postalCode
});




}
/// @nodoc
class _$UpdateContactRequestDtoCopyWithImpl<$Res>
    implements $UpdateContactRequestDtoCopyWith<$Res> {
  _$UpdateContactRequestDtoCopyWithImpl(this._self, this._then);

  final UpdateContactRequestDto _self;
  final $Res Function(UpdateContactRequestDto) _then;

/// Create a copy of UpdateContactRequestDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? primaryContactName = freezed,Object? primaryPhone = freezed,Object? primaryEmail = freezed,Object? secondaryContactName = freezed,Object? secondaryPhone = freezed,Object? secondaryEmail = freezed,Object? address = freezed,Object? country = freezed,Object? state = freezed,Object? city = freezed,Object? postalCode = freezed,}) {
  return _then(_self.copyWith(
primaryContactName: freezed == primaryContactName ? _self.primaryContactName : primaryContactName // ignore: cast_nullable_to_non_nullable
as String?,primaryPhone: freezed == primaryPhone ? _self.primaryPhone : primaryPhone // ignore: cast_nullable_to_non_nullable
as String?,primaryEmail: freezed == primaryEmail ? _self.primaryEmail : primaryEmail // ignore: cast_nullable_to_non_nullable
as String?,secondaryContactName: freezed == secondaryContactName ? _self.secondaryContactName : secondaryContactName // ignore: cast_nullable_to_non_nullable
as String?,secondaryPhone: freezed == secondaryPhone ? _self.secondaryPhone : secondaryPhone // ignore: cast_nullable_to_non_nullable
as String?,secondaryEmail: freezed == secondaryEmail ? _self.secondaryEmail : secondaryEmail // ignore: cast_nullable_to_non_nullable
as String?,address: freezed == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String?,country: freezed == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String?,state: freezed == state ? _self.state : state // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String?,postalCode: freezed == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [UpdateContactRequestDto].
extension UpdateContactRequestDtoPatterns on UpdateContactRequestDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _UpdateContactRequestDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _UpdateContactRequestDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _UpdateContactRequestDto value)  $default,){
final _that = this;
switch (_that) {
case _UpdateContactRequestDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _UpdateContactRequestDto value)?  $default,){
final _that = this;
switch (_that) {
case _UpdateContactRequestDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? primaryContactName,  String? primaryPhone,  String? primaryEmail,  String? secondaryContactName,  String? secondaryPhone,  String? secondaryEmail,  String? address,  String? country,  String? state,  String? city,  String? postalCode)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _UpdateContactRequestDto() when $default != null:
return $default(_that.primaryContactName,_that.primaryPhone,_that.primaryEmail,_that.secondaryContactName,_that.secondaryPhone,_that.secondaryEmail,_that.address,_that.country,_that.state,_that.city,_that.postalCode);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? primaryContactName,  String? primaryPhone,  String? primaryEmail,  String? secondaryContactName,  String? secondaryPhone,  String? secondaryEmail,  String? address,  String? country,  String? state,  String? city,  String? postalCode)  $default,) {final _that = this;
switch (_that) {
case _UpdateContactRequestDto():
return $default(_that.primaryContactName,_that.primaryPhone,_that.primaryEmail,_that.secondaryContactName,_that.secondaryPhone,_that.secondaryEmail,_that.address,_that.country,_that.state,_that.city,_that.postalCode);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? primaryContactName,  String? primaryPhone,  String? primaryEmail,  String? secondaryContactName,  String? secondaryPhone,  String? secondaryEmail,  String? address,  String? country,  String? state,  String? city,  String? postalCode)?  $default,) {final _that = this;
switch (_that) {
case _UpdateContactRequestDto() when $default != null:
return $default(_that.primaryContactName,_that.primaryPhone,_that.primaryEmail,_that.secondaryContactName,_that.secondaryPhone,_that.secondaryEmail,_that.address,_that.country,_that.state,_that.city,_that.postalCode);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _UpdateContactRequestDto implements UpdateContactRequestDto {
  const _UpdateContactRequestDto({this.primaryContactName, this.primaryPhone, this.primaryEmail, this.secondaryContactName, this.secondaryPhone, this.secondaryEmail, this.address, this.country, this.state, this.city, this.postalCode});
  factory _UpdateContactRequestDto.fromJson(Map<String, dynamic> json) => _$UpdateContactRequestDtoFromJson(json);

@override final  String? primaryContactName;
@override final  String? primaryPhone;
@override final  String? primaryEmail;
@override final  String? secondaryContactName;
@override final  String? secondaryPhone;
@override final  String? secondaryEmail;
@override final  String? address;
@override final  String? country;
@override final  String? state;
@override final  String? city;
@override final  String? postalCode;

/// Create a copy of UpdateContactRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$UpdateContactRequestDtoCopyWith<_UpdateContactRequestDto> get copyWith => __$UpdateContactRequestDtoCopyWithImpl<_UpdateContactRequestDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$UpdateContactRequestDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _UpdateContactRequestDto&&(identical(other.primaryContactName, primaryContactName) || other.primaryContactName == primaryContactName)&&(identical(other.primaryPhone, primaryPhone) || other.primaryPhone == primaryPhone)&&(identical(other.primaryEmail, primaryEmail) || other.primaryEmail == primaryEmail)&&(identical(other.secondaryContactName, secondaryContactName) || other.secondaryContactName == secondaryContactName)&&(identical(other.secondaryPhone, secondaryPhone) || other.secondaryPhone == secondaryPhone)&&(identical(other.secondaryEmail, secondaryEmail) || other.secondaryEmail == secondaryEmail)&&(identical(other.address, address) || other.address == address)&&(identical(other.country, country) || other.country == country)&&(identical(other.state, state) || other.state == state)&&(identical(other.city, city) || other.city == city)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,primaryContactName,primaryPhone,primaryEmail,secondaryContactName,secondaryPhone,secondaryEmail,address,country,state,city,postalCode);

@override
String toString() {
  return 'UpdateContactRequestDto(primaryContactName: $primaryContactName, primaryPhone: $primaryPhone, primaryEmail: $primaryEmail, secondaryContactName: $secondaryContactName, secondaryPhone: $secondaryPhone, secondaryEmail: $secondaryEmail, address: $address, country: $country, state: $state, city: $city, postalCode: $postalCode)';
}


}

/// @nodoc
abstract mixin class _$UpdateContactRequestDtoCopyWith<$Res> implements $UpdateContactRequestDtoCopyWith<$Res> {
  factory _$UpdateContactRequestDtoCopyWith(_UpdateContactRequestDto value, $Res Function(_UpdateContactRequestDto) _then) = __$UpdateContactRequestDtoCopyWithImpl;
@override @useResult
$Res call({
 String? primaryContactName, String? primaryPhone, String? primaryEmail, String? secondaryContactName, String? secondaryPhone, String? secondaryEmail, String? address, String? country, String? state, String? city, String? postalCode
});




}
/// @nodoc
class __$UpdateContactRequestDtoCopyWithImpl<$Res>
    implements _$UpdateContactRequestDtoCopyWith<$Res> {
  __$UpdateContactRequestDtoCopyWithImpl(this._self, this._then);

  final _UpdateContactRequestDto _self;
  final $Res Function(_UpdateContactRequestDto) _then;

/// Create a copy of UpdateContactRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? primaryContactName = freezed,Object? primaryPhone = freezed,Object? primaryEmail = freezed,Object? secondaryContactName = freezed,Object? secondaryPhone = freezed,Object? secondaryEmail = freezed,Object? address = freezed,Object? country = freezed,Object? state = freezed,Object? city = freezed,Object? postalCode = freezed,}) {
  return _then(_UpdateContactRequestDto(
primaryContactName: freezed == primaryContactName ? _self.primaryContactName : primaryContactName // ignore: cast_nullable_to_non_nullable
as String?,primaryPhone: freezed == primaryPhone ? _self.primaryPhone : primaryPhone // ignore: cast_nullable_to_non_nullable
as String?,primaryEmail: freezed == primaryEmail ? _self.primaryEmail : primaryEmail // ignore: cast_nullable_to_non_nullable
as String?,secondaryContactName: freezed == secondaryContactName ? _self.secondaryContactName : secondaryContactName // ignore: cast_nullable_to_non_nullable
as String?,secondaryPhone: freezed == secondaryPhone ? _self.secondaryPhone : secondaryPhone // ignore: cast_nullable_to_non_nullable
as String?,secondaryEmail: freezed == secondaryEmail ? _self.secondaryEmail : secondaryEmail // ignore: cast_nullable_to_non_nullable
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

// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'update_academy_request_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$UpdateAcademyRequestDto {

 String? get name; String? get legalName; String? get description; String? get registrationNumber; String? get gstNumber; String? get establishedDate; String? get website; String? get email; String? get phone;
/// Create a copy of UpdateAcademyRequestDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$UpdateAcademyRequestDtoCopyWith<UpdateAcademyRequestDto> get copyWith => _$UpdateAcademyRequestDtoCopyWithImpl<UpdateAcademyRequestDto>(this as UpdateAcademyRequestDto, _$identity);

  /// Serializes this UpdateAcademyRequestDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is UpdateAcademyRequestDto&&(identical(other.name, name) || other.name == name)&&(identical(other.legalName, legalName) || other.legalName == legalName)&&(identical(other.description, description) || other.description == description)&&(identical(other.registrationNumber, registrationNumber) || other.registrationNumber == registrationNumber)&&(identical(other.gstNumber, gstNumber) || other.gstNumber == gstNumber)&&(identical(other.establishedDate, establishedDate) || other.establishedDate == establishedDate)&&(identical(other.website, website) || other.website == website)&&(identical(other.email, email) || other.email == email)&&(identical(other.phone, phone) || other.phone == phone));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,name,legalName,description,registrationNumber,gstNumber,establishedDate,website,email,phone);

@override
String toString() {
  return 'UpdateAcademyRequestDto(name: $name, legalName: $legalName, description: $description, registrationNumber: $registrationNumber, gstNumber: $gstNumber, establishedDate: $establishedDate, website: $website, email: $email, phone: $phone)';
}


}

/// @nodoc
abstract mixin class $UpdateAcademyRequestDtoCopyWith<$Res>  {
  factory $UpdateAcademyRequestDtoCopyWith(UpdateAcademyRequestDto value, $Res Function(UpdateAcademyRequestDto) _then) = _$UpdateAcademyRequestDtoCopyWithImpl;
@useResult
$Res call({
 String? name, String? legalName, String? description, String? registrationNumber, String? gstNumber, String? establishedDate, String? website, String? email, String? phone
});




}
/// @nodoc
class _$UpdateAcademyRequestDtoCopyWithImpl<$Res>
    implements $UpdateAcademyRequestDtoCopyWith<$Res> {
  _$UpdateAcademyRequestDtoCopyWithImpl(this._self, this._then);

  final UpdateAcademyRequestDto _self;
  final $Res Function(UpdateAcademyRequestDto) _then;

/// Create a copy of UpdateAcademyRequestDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? name = freezed,Object? legalName = freezed,Object? description = freezed,Object? registrationNumber = freezed,Object? gstNumber = freezed,Object? establishedDate = freezed,Object? website = freezed,Object? email = freezed,Object? phone = freezed,}) {
  return _then(_self.copyWith(
name: freezed == name ? _self.name : name // ignore: cast_nullable_to_non_nullable
as String?,legalName: freezed == legalName ? _self.legalName : legalName // ignore: cast_nullable_to_non_nullable
as String?,description: freezed == description ? _self.description : description // ignore: cast_nullable_to_non_nullable
as String?,registrationNumber: freezed == registrationNumber ? _self.registrationNumber : registrationNumber // ignore: cast_nullable_to_non_nullable
as String?,gstNumber: freezed == gstNumber ? _self.gstNumber : gstNumber // ignore: cast_nullable_to_non_nullable
as String?,establishedDate: freezed == establishedDate ? _self.establishedDate : establishedDate // ignore: cast_nullable_to_non_nullable
as String?,website: freezed == website ? _self.website : website // ignore: cast_nullable_to_non_nullable
as String?,email: freezed == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String?,phone: freezed == phone ? _self.phone : phone // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [UpdateAcademyRequestDto].
extension UpdateAcademyRequestDtoPatterns on UpdateAcademyRequestDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _UpdateAcademyRequestDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _UpdateAcademyRequestDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _UpdateAcademyRequestDto value)  $default,){
final _that = this;
switch (_that) {
case _UpdateAcademyRequestDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _UpdateAcademyRequestDto value)?  $default,){
final _that = this;
switch (_that) {
case _UpdateAcademyRequestDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? name,  String? legalName,  String? description,  String? registrationNumber,  String? gstNumber,  String? establishedDate,  String? website,  String? email,  String? phone)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _UpdateAcademyRequestDto() when $default != null:
return $default(_that.name,_that.legalName,_that.description,_that.registrationNumber,_that.gstNumber,_that.establishedDate,_that.website,_that.email,_that.phone);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? name,  String? legalName,  String? description,  String? registrationNumber,  String? gstNumber,  String? establishedDate,  String? website,  String? email,  String? phone)  $default,) {final _that = this;
switch (_that) {
case _UpdateAcademyRequestDto():
return $default(_that.name,_that.legalName,_that.description,_that.registrationNumber,_that.gstNumber,_that.establishedDate,_that.website,_that.email,_that.phone);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? name,  String? legalName,  String? description,  String? registrationNumber,  String? gstNumber,  String? establishedDate,  String? website,  String? email,  String? phone)?  $default,) {final _that = this;
switch (_that) {
case _UpdateAcademyRequestDto() when $default != null:
return $default(_that.name,_that.legalName,_that.description,_that.registrationNumber,_that.gstNumber,_that.establishedDate,_that.website,_that.email,_that.phone);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _UpdateAcademyRequestDto implements UpdateAcademyRequestDto {
  const _UpdateAcademyRequestDto({this.name, this.legalName, this.description, this.registrationNumber, this.gstNumber, this.establishedDate, this.website, this.email, this.phone});
  factory _UpdateAcademyRequestDto.fromJson(Map<String, dynamic> json) => _$UpdateAcademyRequestDtoFromJson(json);

@override final  String? name;
@override final  String? legalName;
@override final  String? description;
@override final  String? registrationNumber;
@override final  String? gstNumber;
@override final  String? establishedDate;
@override final  String? website;
@override final  String? email;
@override final  String? phone;

/// Create a copy of UpdateAcademyRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$UpdateAcademyRequestDtoCopyWith<_UpdateAcademyRequestDto> get copyWith => __$UpdateAcademyRequestDtoCopyWithImpl<_UpdateAcademyRequestDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$UpdateAcademyRequestDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _UpdateAcademyRequestDto&&(identical(other.name, name) || other.name == name)&&(identical(other.legalName, legalName) || other.legalName == legalName)&&(identical(other.description, description) || other.description == description)&&(identical(other.registrationNumber, registrationNumber) || other.registrationNumber == registrationNumber)&&(identical(other.gstNumber, gstNumber) || other.gstNumber == gstNumber)&&(identical(other.establishedDate, establishedDate) || other.establishedDate == establishedDate)&&(identical(other.website, website) || other.website == website)&&(identical(other.email, email) || other.email == email)&&(identical(other.phone, phone) || other.phone == phone));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,name,legalName,description,registrationNumber,gstNumber,establishedDate,website,email,phone);

@override
String toString() {
  return 'UpdateAcademyRequestDto(name: $name, legalName: $legalName, description: $description, registrationNumber: $registrationNumber, gstNumber: $gstNumber, establishedDate: $establishedDate, website: $website, email: $email, phone: $phone)';
}


}

/// @nodoc
abstract mixin class _$UpdateAcademyRequestDtoCopyWith<$Res> implements $UpdateAcademyRequestDtoCopyWith<$Res> {
  factory _$UpdateAcademyRequestDtoCopyWith(_UpdateAcademyRequestDto value, $Res Function(_UpdateAcademyRequestDto) _then) = __$UpdateAcademyRequestDtoCopyWithImpl;
@override @useResult
$Res call({
 String? name, String? legalName, String? description, String? registrationNumber, String? gstNumber, String? establishedDate, String? website, String? email, String? phone
});




}
/// @nodoc
class __$UpdateAcademyRequestDtoCopyWithImpl<$Res>
    implements _$UpdateAcademyRequestDtoCopyWith<$Res> {
  __$UpdateAcademyRequestDtoCopyWithImpl(this._self, this._then);

  final _UpdateAcademyRequestDto _self;
  final $Res Function(_UpdateAcademyRequestDto) _then;

/// Create a copy of UpdateAcademyRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? name = freezed,Object? legalName = freezed,Object? description = freezed,Object? registrationNumber = freezed,Object? gstNumber = freezed,Object? establishedDate = freezed,Object? website = freezed,Object? email = freezed,Object? phone = freezed,}) {
  return _then(_UpdateAcademyRequestDto(
name: freezed == name ? _self.name : name // ignore: cast_nullable_to_non_nullable
as String?,legalName: freezed == legalName ? _self.legalName : legalName // ignore: cast_nullable_to_non_nullable
as String?,description: freezed == description ? _self.description : description // ignore: cast_nullable_to_non_nullable
as String?,registrationNumber: freezed == registrationNumber ? _self.registrationNumber : registrationNumber // ignore: cast_nullable_to_non_nullable
as String?,gstNumber: freezed == gstNumber ? _self.gstNumber : gstNumber // ignore: cast_nullable_to_non_nullable
as String?,establishedDate: freezed == establishedDate ? _self.establishedDate : establishedDate // ignore: cast_nullable_to_non_nullable
as String?,website: freezed == website ? _self.website : website // ignore: cast_nullable_to_non_nullable
as String?,email: freezed == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String?,phone: freezed == phone ? _self.phone : phone // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}

// dart format on

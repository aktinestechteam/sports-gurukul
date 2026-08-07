// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'academy_branch_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$AcademyBranchDto {

 String? get id; String? get academyId; String? get branchName; String? get address; String? get country; String? get state; String? get city; String? get district; String? get postalCode; String? get createdAt; String? get updatedAt;
/// Create a copy of AcademyBranchDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AcademyBranchDtoCopyWith<AcademyBranchDto> get copyWith => _$AcademyBranchDtoCopyWithImpl<AcademyBranchDto>(this as AcademyBranchDto, _$identity);

  /// Serializes this AcademyBranchDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AcademyBranchDto&&(identical(other.id, id) || other.id == id)&&(identical(other.academyId, academyId) || other.academyId == academyId)&&(identical(other.branchName, branchName) || other.branchName == branchName)&&(identical(other.address, address) || other.address == address)&&(identical(other.country, country) || other.country == country)&&(identical(other.state, state) || other.state == state)&&(identical(other.city, city) || other.city == city)&&(identical(other.district, district) || other.district == district)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt)&&(identical(other.updatedAt, updatedAt) || other.updatedAt == updatedAt));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,academyId,branchName,address,country,state,city,district,postalCode,createdAt,updatedAt);

@override
String toString() {
  return 'AcademyBranchDto(id: $id, academyId: $academyId, branchName: $branchName, address: $address, country: $country, state: $state, city: $city, district: $district, postalCode: $postalCode, createdAt: $createdAt, updatedAt: $updatedAt)';
}


}

/// @nodoc
abstract mixin class $AcademyBranchDtoCopyWith<$Res>  {
  factory $AcademyBranchDtoCopyWith(AcademyBranchDto value, $Res Function(AcademyBranchDto) _then) = _$AcademyBranchDtoCopyWithImpl;
@useResult
$Res call({
 String? id, String? academyId, String? branchName, String? address, String? country, String? state, String? city, String? district, String? postalCode, String? createdAt, String? updatedAt
});




}
/// @nodoc
class _$AcademyBranchDtoCopyWithImpl<$Res>
    implements $AcademyBranchDtoCopyWith<$Res> {
  _$AcademyBranchDtoCopyWithImpl(this._self, this._then);

  final AcademyBranchDto _self;
  final $Res Function(AcademyBranchDto) _then;

/// Create a copy of AcademyBranchDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = freezed,Object? academyId = freezed,Object? branchName = freezed,Object? address = freezed,Object? country = freezed,Object? state = freezed,Object? city = freezed,Object? district = freezed,Object? postalCode = freezed,Object? createdAt = freezed,Object? updatedAt = freezed,}) {
  return _then(_self.copyWith(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,academyId: freezed == academyId ? _self.academyId : academyId // ignore: cast_nullable_to_non_nullable
as String?,branchName: freezed == branchName ? _self.branchName : branchName // ignore: cast_nullable_to_non_nullable
as String?,address: freezed == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String?,country: freezed == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String?,state: freezed == state ? _self.state : state // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String?,district: freezed == district ? _self.district : district // ignore: cast_nullable_to_non_nullable
as String?,postalCode: freezed == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String?,createdAt: freezed == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as String?,updatedAt: freezed == updatedAt ? _self.updatedAt : updatedAt // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [AcademyBranchDto].
extension AcademyBranchDtoPatterns on AcademyBranchDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AcademyBranchDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AcademyBranchDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AcademyBranchDto value)  $default,){
final _that = this;
switch (_that) {
case _AcademyBranchDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AcademyBranchDto value)?  $default,){
final _that = this;
switch (_that) {
case _AcademyBranchDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? id,  String? academyId,  String? branchName,  String? address,  String? country,  String? state,  String? city,  String? district,  String? postalCode,  String? createdAt,  String? updatedAt)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AcademyBranchDto() when $default != null:
return $default(_that.id,_that.academyId,_that.branchName,_that.address,_that.country,_that.state,_that.city,_that.district,_that.postalCode,_that.createdAt,_that.updatedAt);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? id,  String? academyId,  String? branchName,  String? address,  String? country,  String? state,  String? city,  String? district,  String? postalCode,  String? createdAt,  String? updatedAt)  $default,) {final _that = this;
switch (_that) {
case _AcademyBranchDto():
return $default(_that.id,_that.academyId,_that.branchName,_that.address,_that.country,_that.state,_that.city,_that.district,_that.postalCode,_that.createdAt,_that.updatedAt);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? id,  String? academyId,  String? branchName,  String? address,  String? country,  String? state,  String? city,  String? district,  String? postalCode,  String? createdAt,  String? updatedAt)?  $default,) {final _that = this;
switch (_that) {
case _AcademyBranchDto() when $default != null:
return $default(_that.id,_that.academyId,_that.branchName,_that.address,_that.country,_that.state,_that.city,_that.district,_that.postalCode,_that.createdAt,_that.updatedAt);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _AcademyBranchDto implements AcademyBranchDto {
  const _AcademyBranchDto({this.id, this.academyId, this.branchName, this.address, this.country, this.state, this.city, this.district, this.postalCode, this.createdAt, this.updatedAt});
  factory _AcademyBranchDto.fromJson(Map<String, dynamic> json) => _$AcademyBranchDtoFromJson(json);

@override final  String? id;
@override final  String? academyId;
@override final  String? branchName;
@override final  String? address;
@override final  String? country;
@override final  String? state;
@override final  String? city;
@override final  String? district;
@override final  String? postalCode;
@override final  String? createdAt;
@override final  String? updatedAt;

/// Create a copy of AcademyBranchDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AcademyBranchDtoCopyWith<_AcademyBranchDto> get copyWith => __$AcademyBranchDtoCopyWithImpl<_AcademyBranchDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$AcademyBranchDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _AcademyBranchDto&&(identical(other.id, id) || other.id == id)&&(identical(other.academyId, academyId) || other.academyId == academyId)&&(identical(other.branchName, branchName) || other.branchName == branchName)&&(identical(other.address, address) || other.address == address)&&(identical(other.country, country) || other.country == country)&&(identical(other.state, state) || other.state == state)&&(identical(other.city, city) || other.city == city)&&(identical(other.district, district) || other.district == district)&&(identical(other.postalCode, postalCode) || other.postalCode == postalCode)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt)&&(identical(other.updatedAt, updatedAt) || other.updatedAt == updatedAt));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,academyId,branchName,address,country,state,city,district,postalCode,createdAt,updatedAt);

@override
String toString() {
  return 'AcademyBranchDto(id: $id, academyId: $academyId, branchName: $branchName, address: $address, country: $country, state: $state, city: $city, district: $district, postalCode: $postalCode, createdAt: $createdAt, updatedAt: $updatedAt)';
}


}

/// @nodoc
abstract mixin class _$AcademyBranchDtoCopyWith<$Res> implements $AcademyBranchDtoCopyWith<$Res> {
  factory _$AcademyBranchDtoCopyWith(_AcademyBranchDto value, $Res Function(_AcademyBranchDto) _then) = __$AcademyBranchDtoCopyWithImpl;
@override @useResult
$Res call({
 String? id, String? academyId, String? branchName, String? address, String? country, String? state, String? city, String? district, String? postalCode, String? createdAt, String? updatedAt
});




}
/// @nodoc
class __$AcademyBranchDtoCopyWithImpl<$Res>
    implements _$AcademyBranchDtoCopyWith<$Res> {
  __$AcademyBranchDtoCopyWithImpl(this._self, this._then);

  final _AcademyBranchDto _self;
  final $Res Function(_AcademyBranchDto) _then;

/// Create a copy of AcademyBranchDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = freezed,Object? academyId = freezed,Object? branchName = freezed,Object? address = freezed,Object? country = freezed,Object? state = freezed,Object? city = freezed,Object? district = freezed,Object? postalCode = freezed,Object? createdAt = freezed,Object? updatedAt = freezed,}) {
  return _then(_AcademyBranchDto(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,academyId: freezed == academyId ? _self.academyId : academyId // ignore: cast_nullable_to_non_nullable
as String?,branchName: freezed == branchName ? _self.branchName : branchName // ignore: cast_nullable_to_non_nullable
as String?,address: freezed == address ? _self.address : address // ignore: cast_nullable_to_non_nullable
as String?,country: freezed == country ? _self.country : country // ignore: cast_nullable_to_non_nullable
as String?,state: freezed == state ? _self.state : state // ignore: cast_nullable_to_non_nullable
as String?,city: freezed == city ? _self.city : city // ignore: cast_nullable_to_non_nullable
as String?,district: freezed == district ? _self.district : district // ignore: cast_nullable_to_non_nullable
as String?,postalCode: freezed == postalCode ? _self.postalCode : postalCode // ignore: cast_nullable_to_non_nullable
as String?,createdAt: freezed == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as String?,updatedAt: freezed == updatedAt ? _self.updatedAt : updatedAt // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}

// dart format on

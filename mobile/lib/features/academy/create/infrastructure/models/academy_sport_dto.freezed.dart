// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'academy_sport_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$AcademySportDto {

 String? get id; String? get sportId; String? get name; String? get code; bool get isPrimarySport;
/// Create a copy of AcademySportDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AcademySportDtoCopyWith<AcademySportDto> get copyWith => _$AcademySportDtoCopyWithImpl<AcademySportDto>(this as AcademySportDto, _$identity);

  /// Serializes this AcademySportDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AcademySportDto&&(identical(other.id, id) || other.id == id)&&(identical(other.sportId, sportId) || other.sportId == sportId)&&(identical(other.name, name) || other.name == name)&&(identical(other.code, code) || other.code == code)&&(identical(other.isPrimarySport, isPrimarySport) || other.isPrimarySport == isPrimarySport));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,sportId,name,code,isPrimarySport);

@override
String toString() {
  return 'AcademySportDto(id: $id, sportId: $sportId, name: $name, code: $code, isPrimarySport: $isPrimarySport)';
}


}

/// @nodoc
abstract mixin class $AcademySportDtoCopyWith<$Res>  {
  factory $AcademySportDtoCopyWith(AcademySportDto value, $Res Function(AcademySportDto) _then) = _$AcademySportDtoCopyWithImpl;
@useResult
$Res call({
 String? id, String? sportId, String? name, String? code, bool isPrimarySport
});




}
/// @nodoc
class _$AcademySportDtoCopyWithImpl<$Res>
    implements $AcademySportDtoCopyWith<$Res> {
  _$AcademySportDtoCopyWithImpl(this._self, this._then);

  final AcademySportDto _self;
  final $Res Function(AcademySportDto) _then;

/// Create a copy of AcademySportDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = freezed,Object? sportId = freezed,Object? name = freezed,Object? code = freezed,Object? isPrimarySport = null,}) {
  return _then(_self.copyWith(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,sportId: freezed == sportId ? _self.sportId : sportId // ignore: cast_nullable_to_non_nullable
as String?,name: freezed == name ? _self.name : name // ignore: cast_nullable_to_non_nullable
as String?,code: freezed == code ? _self.code : code // ignore: cast_nullable_to_non_nullable
as String?,isPrimarySport: null == isPrimarySport ? _self.isPrimarySport : isPrimarySport // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [AcademySportDto].
extension AcademySportDtoPatterns on AcademySportDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AcademySportDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AcademySportDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AcademySportDto value)  $default,){
final _that = this;
switch (_that) {
case _AcademySportDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AcademySportDto value)?  $default,){
final _that = this;
switch (_that) {
case _AcademySportDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? id,  String? sportId,  String? name,  String? code,  bool isPrimarySport)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AcademySportDto() when $default != null:
return $default(_that.id,_that.sportId,_that.name,_that.code,_that.isPrimarySport);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? id,  String? sportId,  String? name,  String? code,  bool isPrimarySport)  $default,) {final _that = this;
switch (_that) {
case _AcademySportDto():
return $default(_that.id,_that.sportId,_that.name,_that.code,_that.isPrimarySport);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? id,  String? sportId,  String? name,  String? code,  bool isPrimarySport)?  $default,) {final _that = this;
switch (_that) {
case _AcademySportDto() when $default != null:
return $default(_that.id,_that.sportId,_that.name,_that.code,_that.isPrimarySport);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _AcademySportDto implements AcademySportDto {
  const _AcademySportDto({this.id, this.sportId, this.name, this.code, this.isPrimarySport = false});
  factory _AcademySportDto.fromJson(Map<String, dynamic> json) => _$AcademySportDtoFromJson(json);

@override final  String? id;
@override final  String? sportId;
@override final  String? name;
@override final  String? code;
@override@JsonKey() final  bool isPrimarySport;

/// Create a copy of AcademySportDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AcademySportDtoCopyWith<_AcademySportDto> get copyWith => __$AcademySportDtoCopyWithImpl<_AcademySportDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$AcademySportDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _AcademySportDto&&(identical(other.id, id) || other.id == id)&&(identical(other.sportId, sportId) || other.sportId == sportId)&&(identical(other.name, name) || other.name == name)&&(identical(other.code, code) || other.code == code)&&(identical(other.isPrimarySport, isPrimarySport) || other.isPrimarySport == isPrimarySport));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,sportId,name,code,isPrimarySport);

@override
String toString() {
  return 'AcademySportDto(id: $id, sportId: $sportId, name: $name, code: $code, isPrimarySport: $isPrimarySport)';
}


}

/// @nodoc
abstract mixin class _$AcademySportDtoCopyWith<$Res> implements $AcademySportDtoCopyWith<$Res> {
  factory _$AcademySportDtoCopyWith(_AcademySportDto value, $Res Function(_AcademySportDto) _then) = __$AcademySportDtoCopyWithImpl;
@override @useResult
$Res call({
 String? id, String? sportId, String? name, String? code, bool isPrimarySport
});




}
/// @nodoc
class __$AcademySportDtoCopyWithImpl<$Res>
    implements _$AcademySportDtoCopyWith<$Res> {
  __$AcademySportDtoCopyWithImpl(this._self, this._then);

  final _AcademySportDto _self;
  final $Res Function(_AcademySportDto) _then;

/// Create a copy of AcademySportDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = freezed,Object? sportId = freezed,Object? name = freezed,Object? code = freezed,Object? isPrimarySport = null,}) {
  return _then(_AcademySportDto(
id: freezed == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String?,sportId: freezed == sportId ? _self.sportId : sportId // ignore: cast_nullable_to_non_nullable
as String?,name: freezed == name ? _self.name : name // ignore: cast_nullable_to_non_nullable
as String?,code: freezed == code ? _self.code : code // ignore: cast_nullable_to_non_nullable
as String?,isPrimarySport: null == isPrimarySport ? _self.isPrimarySport : isPrimarySport // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on

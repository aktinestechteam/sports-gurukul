// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'forgot_password_request_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$ForgotPasswordRequestDto {

 String get email;
/// Create a copy of ForgotPasswordRequestDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$ForgotPasswordRequestDtoCopyWith<ForgotPasswordRequestDto> get copyWith => _$ForgotPasswordRequestDtoCopyWithImpl<ForgotPasswordRequestDto>(this as ForgotPasswordRequestDto, _$identity);

  /// Serializes this ForgotPasswordRequestDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is ForgotPasswordRequestDto&&(identical(other.email, email) || other.email == email));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,email);

@override
String toString() {
  return 'ForgotPasswordRequestDto(email: $email)';
}


}

/// @nodoc
abstract mixin class $ForgotPasswordRequestDtoCopyWith<$Res>  {
  factory $ForgotPasswordRequestDtoCopyWith(ForgotPasswordRequestDto value, $Res Function(ForgotPasswordRequestDto) _then) = _$ForgotPasswordRequestDtoCopyWithImpl;
@useResult
$Res call({
 String email
});




}
/// @nodoc
class _$ForgotPasswordRequestDtoCopyWithImpl<$Res>
    implements $ForgotPasswordRequestDtoCopyWith<$Res> {
  _$ForgotPasswordRequestDtoCopyWithImpl(this._self, this._then);

  final ForgotPasswordRequestDto _self;
  final $Res Function(ForgotPasswordRequestDto) _then;

/// Create a copy of ForgotPasswordRequestDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? email = null,}) {
  return _then(_self.copyWith(
email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,
  ));
}

}


/// Adds pattern-matching-related methods to [ForgotPasswordRequestDto].
extension ForgotPasswordRequestDtoPatterns on ForgotPasswordRequestDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _ForgotPasswordRequestDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _ForgotPasswordRequestDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _ForgotPasswordRequestDto value)  $default,){
final _that = this;
switch (_that) {
case _ForgotPasswordRequestDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _ForgotPasswordRequestDto value)?  $default,){
final _that = this;
switch (_that) {
case _ForgotPasswordRequestDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String email)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _ForgotPasswordRequestDto() when $default != null:
return $default(_that.email);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String email)  $default,) {final _that = this;
switch (_that) {
case _ForgotPasswordRequestDto():
return $default(_that.email);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String email)?  $default,) {final _that = this;
switch (_that) {
case _ForgotPasswordRequestDto() when $default != null:
return $default(_that.email);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _ForgotPasswordRequestDto implements ForgotPasswordRequestDto {
  const _ForgotPasswordRequestDto({required this.email});
  factory _ForgotPasswordRequestDto.fromJson(Map<String, dynamic> json) => _$ForgotPasswordRequestDtoFromJson(json);

@override final  String email;

/// Create a copy of ForgotPasswordRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ForgotPasswordRequestDtoCopyWith<_ForgotPasswordRequestDto> get copyWith => __$ForgotPasswordRequestDtoCopyWithImpl<_ForgotPasswordRequestDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$ForgotPasswordRequestDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ForgotPasswordRequestDto&&(identical(other.email, email) || other.email == email));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,email);

@override
String toString() {
  return 'ForgotPasswordRequestDto(email: $email)';
}


}

/// @nodoc
abstract mixin class _$ForgotPasswordRequestDtoCopyWith<$Res> implements $ForgotPasswordRequestDtoCopyWith<$Res> {
  factory _$ForgotPasswordRequestDtoCopyWith(_ForgotPasswordRequestDto value, $Res Function(_ForgotPasswordRequestDto) _then) = __$ForgotPasswordRequestDtoCopyWithImpl;
@override @useResult
$Res call({
 String email
});




}
/// @nodoc
class __$ForgotPasswordRequestDtoCopyWithImpl<$Res>
    implements _$ForgotPasswordRequestDtoCopyWith<$Res> {
  __$ForgotPasswordRequestDtoCopyWithImpl(this._self, this._then);

  final _ForgotPasswordRequestDto _self;
  final $Res Function(_ForgotPasswordRequestDto) _then;

/// Create a copy of ForgotPasswordRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? email = null,}) {
  return _then(_ForgotPasswordRequestDto(
email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,
  ));
}


}

// dart format on

// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'send_verification_email_request_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$SendVerificationEmailRequestDto {

 String get email;
/// Create a copy of SendVerificationEmailRequestDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$SendVerificationEmailRequestDtoCopyWith<SendVerificationEmailRequestDto> get copyWith => _$SendVerificationEmailRequestDtoCopyWithImpl<SendVerificationEmailRequestDto>(this as SendVerificationEmailRequestDto, _$identity);

  /// Serializes this SendVerificationEmailRequestDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is SendVerificationEmailRequestDto&&(identical(other.email, email) || other.email == email));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,email);

@override
String toString() {
  return 'SendVerificationEmailRequestDto(email: $email)';
}


}

/// @nodoc
abstract mixin class $SendVerificationEmailRequestDtoCopyWith<$Res>  {
  factory $SendVerificationEmailRequestDtoCopyWith(SendVerificationEmailRequestDto value, $Res Function(SendVerificationEmailRequestDto) _then) = _$SendVerificationEmailRequestDtoCopyWithImpl;
@useResult
$Res call({
 String email
});




}
/// @nodoc
class _$SendVerificationEmailRequestDtoCopyWithImpl<$Res>
    implements $SendVerificationEmailRequestDtoCopyWith<$Res> {
  _$SendVerificationEmailRequestDtoCopyWithImpl(this._self, this._then);

  final SendVerificationEmailRequestDto _self;
  final $Res Function(SendVerificationEmailRequestDto) _then;

/// Create a copy of SendVerificationEmailRequestDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? email = null,}) {
  return _then(_self.copyWith(
email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,
  ));
}

}


/// Adds pattern-matching-related methods to [SendVerificationEmailRequestDto].
extension SendVerificationEmailRequestDtoPatterns on SendVerificationEmailRequestDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _SendVerificationEmailRequestDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _SendVerificationEmailRequestDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _SendVerificationEmailRequestDto value)  $default,){
final _that = this;
switch (_that) {
case _SendVerificationEmailRequestDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _SendVerificationEmailRequestDto value)?  $default,){
final _that = this;
switch (_that) {
case _SendVerificationEmailRequestDto() when $default != null:
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
case _SendVerificationEmailRequestDto() when $default != null:
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
case _SendVerificationEmailRequestDto():
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
case _SendVerificationEmailRequestDto() when $default != null:
return $default(_that.email);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _SendVerificationEmailRequestDto implements SendVerificationEmailRequestDto {
  const _SendVerificationEmailRequestDto({required this.email});
  factory _SendVerificationEmailRequestDto.fromJson(Map<String, dynamic> json) => _$SendVerificationEmailRequestDtoFromJson(json);

@override final  String email;

/// Create a copy of SendVerificationEmailRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$SendVerificationEmailRequestDtoCopyWith<_SendVerificationEmailRequestDto> get copyWith => __$SendVerificationEmailRequestDtoCopyWithImpl<_SendVerificationEmailRequestDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$SendVerificationEmailRequestDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _SendVerificationEmailRequestDto&&(identical(other.email, email) || other.email == email));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,email);

@override
String toString() {
  return 'SendVerificationEmailRequestDto(email: $email)';
}


}

/// @nodoc
abstract mixin class _$SendVerificationEmailRequestDtoCopyWith<$Res> implements $SendVerificationEmailRequestDtoCopyWith<$Res> {
  factory _$SendVerificationEmailRequestDtoCopyWith(_SendVerificationEmailRequestDto value, $Res Function(_SendVerificationEmailRequestDto) _then) = __$SendVerificationEmailRequestDtoCopyWithImpl;
@override @useResult
$Res call({
 String email
});




}
/// @nodoc
class __$SendVerificationEmailRequestDtoCopyWithImpl<$Res>
    implements _$SendVerificationEmailRequestDtoCopyWith<$Res> {
  __$SendVerificationEmailRequestDtoCopyWithImpl(this._self, this._then);

  final _SendVerificationEmailRequestDto _self;
  final $Res Function(_SendVerificationEmailRequestDto) _then;

/// Create a copy of SendVerificationEmailRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? email = null,}) {
  return _then(_SendVerificationEmailRequestDto(
email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,
  ));
}


}

// dart format on

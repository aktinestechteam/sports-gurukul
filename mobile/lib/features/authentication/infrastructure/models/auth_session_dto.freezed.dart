// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'auth_session_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$AuthSessionDto {

 String get userId; String get email; String get fullName; String get accessToken; String get refreshToken;@FlexibleDateTimeConverter() DateTime get accessTokenExpiresAt; List<String> get roles;
/// Create a copy of AuthSessionDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$AuthSessionDtoCopyWith<AuthSessionDto> get copyWith => _$AuthSessionDtoCopyWithImpl<AuthSessionDto>(this as AuthSessionDto, _$identity);

  /// Serializes this AuthSessionDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AuthSessionDto&&(identical(other.userId, userId) || other.userId == userId)&&(identical(other.email, email) || other.email == email)&&(identical(other.fullName, fullName) || other.fullName == fullName)&&(identical(other.accessToken, accessToken) || other.accessToken == accessToken)&&(identical(other.refreshToken, refreshToken) || other.refreshToken == refreshToken)&&(identical(other.accessTokenExpiresAt, accessTokenExpiresAt) || other.accessTokenExpiresAt == accessTokenExpiresAt)&&const DeepCollectionEquality().equals(other.roles, roles));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,userId,email,fullName,accessToken,refreshToken,accessTokenExpiresAt,const DeepCollectionEquality().hash(roles));

@override
String toString() {
  return 'AuthSessionDto(userId: $userId, email: $email, fullName: $fullName, accessToken: $accessToken, refreshToken: $refreshToken, accessTokenExpiresAt: $accessTokenExpiresAt, roles: $roles)';
}


}

/// @nodoc
abstract mixin class $AuthSessionDtoCopyWith<$Res>  {
  factory $AuthSessionDtoCopyWith(AuthSessionDto value, $Res Function(AuthSessionDto) _then) = _$AuthSessionDtoCopyWithImpl;
@useResult
$Res call({
 String userId, String email, String fullName, String accessToken, String refreshToken,@FlexibleDateTimeConverter() DateTime accessTokenExpiresAt, List<String> roles
});




}
/// @nodoc
class _$AuthSessionDtoCopyWithImpl<$Res>
    implements $AuthSessionDtoCopyWith<$Res> {
  _$AuthSessionDtoCopyWithImpl(this._self, this._then);

  final AuthSessionDto _self;
  final $Res Function(AuthSessionDto) _then;

/// Create a copy of AuthSessionDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? userId = null,Object? email = null,Object? fullName = null,Object? accessToken = null,Object? refreshToken = null,Object? accessTokenExpiresAt = null,Object? roles = null,}) {
  return _then(_self.copyWith(
userId: null == userId ? _self.userId : userId // ignore: cast_nullable_to_non_nullable
as String,email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,fullName: null == fullName ? _self.fullName : fullName // ignore: cast_nullable_to_non_nullable
as String,accessToken: null == accessToken ? _self.accessToken : accessToken // ignore: cast_nullable_to_non_nullable
as String,refreshToken: null == refreshToken ? _self.refreshToken : refreshToken // ignore: cast_nullable_to_non_nullable
as String,accessTokenExpiresAt: null == accessTokenExpiresAt ? _self.accessTokenExpiresAt : accessTokenExpiresAt // ignore: cast_nullable_to_non_nullable
as DateTime,roles: null == roles ? _self.roles : roles // ignore: cast_nullable_to_non_nullable
as List<String>,
  ));
}

}


/// Adds pattern-matching-related methods to [AuthSessionDto].
extension AuthSessionDtoPatterns on AuthSessionDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _AuthSessionDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _AuthSessionDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _AuthSessionDto value)  $default,){
final _that = this;
switch (_that) {
case _AuthSessionDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _AuthSessionDto value)?  $default,){
final _that = this;
switch (_that) {
case _AuthSessionDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String userId,  String email,  String fullName,  String accessToken,  String refreshToken, @FlexibleDateTimeConverter()  DateTime accessTokenExpiresAt,  List<String> roles)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _AuthSessionDto() when $default != null:
return $default(_that.userId,_that.email,_that.fullName,_that.accessToken,_that.refreshToken,_that.accessTokenExpiresAt,_that.roles);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String userId,  String email,  String fullName,  String accessToken,  String refreshToken, @FlexibleDateTimeConverter()  DateTime accessTokenExpiresAt,  List<String> roles)  $default,) {final _that = this;
switch (_that) {
case _AuthSessionDto():
return $default(_that.userId,_that.email,_that.fullName,_that.accessToken,_that.refreshToken,_that.accessTokenExpiresAt,_that.roles);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String userId,  String email,  String fullName,  String accessToken,  String refreshToken, @FlexibleDateTimeConverter()  DateTime accessTokenExpiresAt,  List<String> roles)?  $default,) {final _that = this;
switch (_that) {
case _AuthSessionDto() when $default != null:
return $default(_that.userId,_that.email,_that.fullName,_that.accessToken,_that.refreshToken,_that.accessTokenExpiresAt,_that.roles);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _AuthSessionDto implements AuthSessionDto {
  const _AuthSessionDto({required this.userId, required this.email, required this.fullName, required this.accessToken, required this.refreshToken, @FlexibleDateTimeConverter() required this.accessTokenExpiresAt, final  List<String> roles = const <String>[]}): _roles = roles;
  factory _AuthSessionDto.fromJson(Map<String, dynamic> json) => _$AuthSessionDtoFromJson(json);

@override final  String userId;
@override final  String email;
@override final  String fullName;
@override final  String accessToken;
@override final  String refreshToken;
@override@FlexibleDateTimeConverter() final  DateTime accessTokenExpiresAt;
 final  List<String> _roles;
@override@JsonKey() List<String> get roles {
  if (_roles is EqualUnmodifiableListView) return _roles;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableListView(_roles);
}


/// Create a copy of AuthSessionDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$AuthSessionDtoCopyWith<_AuthSessionDto> get copyWith => __$AuthSessionDtoCopyWithImpl<_AuthSessionDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$AuthSessionDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _AuthSessionDto&&(identical(other.userId, userId) || other.userId == userId)&&(identical(other.email, email) || other.email == email)&&(identical(other.fullName, fullName) || other.fullName == fullName)&&(identical(other.accessToken, accessToken) || other.accessToken == accessToken)&&(identical(other.refreshToken, refreshToken) || other.refreshToken == refreshToken)&&(identical(other.accessTokenExpiresAt, accessTokenExpiresAt) || other.accessTokenExpiresAt == accessTokenExpiresAt)&&const DeepCollectionEquality().equals(other._roles, _roles));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,userId,email,fullName,accessToken,refreshToken,accessTokenExpiresAt,const DeepCollectionEquality().hash(_roles));

@override
String toString() {
  return 'AuthSessionDto(userId: $userId, email: $email, fullName: $fullName, accessToken: $accessToken, refreshToken: $refreshToken, accessTokenExpiresAt: $accessTokenExpiresAt, roles: $roles)';
}


}

/// @nodoc
abstract mixin class _$AuthSessionDtoCopyWith<$Res> implements $AuthSessionDtoCopyWith<$Res> {
  factory _$AuthSessionDtoCopyWith(_AuthSessionDto value, $Res Function(_AuthSessionDto) _then) = __$AuthSessionDtoCopyWithImpl;
@override @useResult
$Res call({
 String userId, String email, String fullName, String accessToken, String refreshToken,@FlexibleDateTimeConverter() DateTime accessTokenExpiresAt, List<String> roles
});




}
/// @nodoc
class __$AuthSessionDtoCopyWithImpl<$Res>
    implements _$AuthSessionDtoCopyWith<$Res> {
  __$AuthSessionDtoCopyWithImpl(this._self, this._then);

  final _AuthSessionDto _self;
  final $Res Function(_AuthSessionDto) _then;

/// Create a copy of AuthSessionDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? userId = null,Object? email = null,Object? fullName = null,Object? accessToken = null,Object? refreshToken = null,Object? accessTokenExpiresAt = null,Object? roles = null,}) {
  return _then(_AuthSessionDto(
userId: null == userId ? _self.userId : userId // ignore: cast_nullable_to_non_nullable
as String,email: null == email ? _self.email : email // ignore: cast_nullable_to_non_nullable
as String,fullName: null == fullName ? _self.fullName : fullName // ignore: cast_nullable_to_non_nullable
as String,accessToken: null == accessToken ? _self.accessToken : accessToken // ignore: cast_nullable_to_non_nullable
as String,refreshToken: null == refreshToken ? _self.refreshToken : refreshToken // ignore: cast_nullable_to_non_nullable
as String,accessTokenExpiresAt: null == accessTokenExpiresAt ? _self.accessTokenExpiresAt : accessTokenExpiresAt // ignore: cast_nullable_to_non_nullable
as DateTime,roles: null == roles ? _self._roles : roles // ignore: cast_nullable_to_non_nullable
as List<String>,
  ));
}


}

// dart format on

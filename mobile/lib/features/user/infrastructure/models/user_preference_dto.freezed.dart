// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'user_preference_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$UserPreferenceDto {

 String get id; String get language;@JsonKey(unknownEnumValue: ThemeDto.system) ThemeDto get theme; String get timeZone; bool get emailNotifications; bool get pushNotifications; bool get smsNotifications; bool get marketingEmails; bool get profileVisibility; bool get showOnlineStatus;
/// Create a copy of UserPreferenceDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$UserPreferenceDtoCopyWith<UserPreferenceDto> get copyWith => _$UserPreferenceDtoCopyWithImpl<UserPreferenceDto>(this as UserPreferenceDto, _$identity);

  /// Serializes this UserPreferenceDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is UserPreferenceDto&&(identical(other.id, id) || other.id == id)&&(identical(other.language, language) || other.language == language)&&(identical(other.theme, theme) || other.theme == theme)&&(identical(other.timeZone, timeZone) || other.timeZone == timeZone)&&(identical(other.emailNotifications, emailNotifications) || other.emailNotifications == emailNotifications)&&(identical(other.pushNotifications, pushNotifications) || other.pushNotifications == pushNotifications)&&(identical(other.smsNotifications, smsNotifications) || other.smsNotifications == smsNotifications)&&(identical(other.marketingEmails, marketingEmails) || other.marketingEmails == marketingEmails)&&(identical(other.profileVisibility, profileVisibility) || other.profileVisibility == profileVisibility)&&(identical(other.showOnlineStatus, showOnlineStatus) || other.showOnlineStatus == showOnlineStatus));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,language,theme,timeZone,emailNotifications,pushNotifications,smsNotifications,marketingEmails,profileVisibility,showOnlineStatus);

@override
String toString() {
  return 'UserPreferenceDto(id: $id, language: $language, theme: $theme, timeZone: $timeZone, emailNotifications: $emailNotifications, pushNotifications: $pushNotifications, smsNotifications: $smsNotifications, marketingEmails: $marketingEmails, profileVisibility: $profileVisibility, showOnlineStatus: $showOnlineStatus)';
}


}

/// @nodoc
abstract mixin class $UserPreferenceDtoCopyWith<$Res>  {
  factory $UserPreferenceDtoCopyWith(UserPreferenceDto value, $Res Function(UserPreferenceDto) _then) = _$UserPreferenceDtoCopyWithImpl;
@useResult
$Res call({
 String id, String language,@JsonKey(unknownEnumValue: ThemeDto.system) ThemeDto theme, String timeZone, bool emailNotifications, bool pushNotifications, bool smsNotifications, bool marketingEmails, bool profileVisibility, bool showOnlineStatus
});




}
/// @nodoc
class _$UserPreferenceDtoCopyWithImpl<$Res>
    implements $UserPreferenceDtoCopyWith<$Res> {
  _$UserPreferenceDtoCopyWithImpl(this._self, this._then);

  final UserPreferenceDto _self;
  final $Res Function(UserPreferenceDto) _then;

/// Create a copy of UserPreferenceDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? language = null,Object? theme = null,Object? timeZone = null,Object? emailNotifications = null,Object? pushNotifications = null,Object? smsNotifications = null,Object? marketingEmails = null,Object? profileVisibility = null,Object? showOnlineStatus = null,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,language: null == language ? _self.language : language // ignore: cast_nullable_to_non_nullable
as String,theme: null == theme ? _self.theme : theme // ignore: cast_nullable_to_non_nullable
as ThemeDto,timeZone: null == timeZone ? _self.timeZone : timeZone // ignore: cast_nullable_to_non_nullable
as String,emailNotifications: null == emailNotifications ? _self.emailNotifications : emailNotifications // ignore: cast_nullable_to_non_nullable
as bool,pushNotifications: null == pushNotifications ? _self.pushNotifications : pushNotifications // ignore: cast_nullable_to_non_nullable
as bool,smsNotifications: null == smsNotifications ? _self.smsNotifications : smsNotifications // ignore: cast_nullable_to_non_nullable
as bool,marketingEmails: null == marketingEmails ? _self.marketingEmails : marketingEmails // ignore: cast_nullable_to_non_nullable
as bool,profileVisibility: null == profileVisibility ? _self.profileVisibility : profileVisibility // ignore: cast_nullable_to_non_nullable
as bool,showOnlineStatus: null == showOnlineStatus ? _self.showOnlineStatus : showOnlineStatus // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}

}


/// Adds pattern-matching-related methods to [UserPreferenceDto].
extension UserPreferenceDtoPatterns on UserPreferenceDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _UserPreferenceDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _UserPreferenceDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _UserPreferenceDto value)  $default,){
final _that = this;
switch (_that) {
case _UserPreferenceDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _UserPreferenceDto value)?  $default,){
final _that = this;
switch (_that) {
case _UserPreferenceDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String language, @JsonKey(unknownEnumValue: ThemeDto.system)  ThemeDto theme,  String timeZone,  bool emailNotifications,  bool pushNotifications,  bool smsNotifications,  bool marketingEmails,  bool profileVisibility,  bool showOnlineStatus)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _UserPreferenceDto() when $default != null:
return $default(_that.id,_that.language,_that.theme,_that.timeZone,_that.emailNotifications,_that.pushNotifications,_that.smsNotifications,_that.marketingEmails,_that.profileVisibility,_that.showOnlineStatus);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String language, @JsonKey(unknownEnumValue: ThemeDto.system)  ThemeDto theme,  String timeZone,  bool emailNotifications,  bool pushNotifications,  bool smsNotifications,  bool marketingEmails,  bool profileVisibility,  bool showOnlineStatus)  $default,) {final _that = this;
switch (_that) {
case _UserPreferenceDto():
return $default(_that.id,_that.language,_that.theme,_that.timeZone,_that.emailNotifications,_that.pushNotifications,_that.smsNotifications,_that.marketingEmails,_that.profileVisibility,_that.showOnlineStatus);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String language, @JsonKey(unknownEnumValue: ThemeDto.system)  ThemeDto theme,  String timeZone,  bool emailNotifications,  bool pushNotifications,  bool smsNotifications,  bool marketingEmails,  bool profileVisibility,  bool showOnlineStatus)?  $default,) {final _that = this;
switch (_that) {
case _UserPreferenceDto() when $default != null:
return $default(_that.id,_that.language,_that.theme,_that.timeZone,_that.emailNotifications,_that.pushNotifications,_that.smsNotifications,_that.marketingEmails,_that.profileVisibility,_that.showOnlineStatus);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _UserPreferenceDto implements UserPreferenceDto {
  const _UserPreferenceDto({required this.id, this.language = 'en', @JsonKey(unknownEnumValue: ThemeDto.system) this.theme = ThemeDto.system, this.timeZone = 'UTC', this.emailNotifications = true, this.pushNotifications = true, this.smsNotifications = false, this.marketingEmails = false, this.profileVisibility = true, this.showOnlineStatus = true});
  factory _UserPreferenceDto.fromJson(Map<String, dynamic> json) => _$UserPreferenceDtoFromJson(json);

@override final  String id;
@override@JsonKey() final  String language;
@override@JsonKey(unknownEnumValue: ThemeDto.system) final  ThemeDto theme;
@override@JsonKey() final  String timeZone;
@override@JsonKey() final  bool emailNotifications;
@override@JsonKey() final  bool pushNotifications;
@override@JsonKey() final  bool smsNotifications;
@override@JsonKey() final  bool marketingEmails;
@override@JsonKey() final  bool profileVisibility;
@override@JsonKey() final  bool showOnlineStatus;

/// Create a copy of UserPreferenceDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$UserPreferenceDtoCopyWith<_UserPreferenceDto> get copyWith => __$UserPreferenceDtoCopyWithImpl<_UserPreferenceDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$UserPreferenceDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _UserPreferenceDto&&(identical(other.id, id) || other.id == id)&&(identical(other.language, language) || other.language == language)&&(identical(other.theme, theme) || other.theme == theme)&&(identical(other.timeZone, timeZone) || other.timeZone == timeZone)&&(identical(other.emailNotifications, emailNotifications) || other.emailNotifications == emailNotifications)&&(identical(other.pushNotifications, pushNotifications) || other.pushNotifications == pushNotifications)&&(identical(other.smsNotifications, smsNotifications) || other.smsNotifications == smsNotifications)&&(identical(other.marketingEmails, marketingEmails) || other.marketingEmails == marketingEmails)&&(identical(other.profileVisibility, profileVisibility) || other.profileVisibility == profileVisibility)&&(identical(other.showOnlineStatus, showOnlineStatus) || other.showOnlineStatus == showOnlineStatus));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,language,theme,timeZone,emailNotifications,pushNotifications,smsNotifications,marketingEmails,profileVisibility,showOnlineStatus);

@override
String toString() {
  return 'UserPreferenceDto(id: $id, language: $language, theme: $theme, timeZone: $timeZone, emailNotifications: $emailNotifications, pushNotifications: $pushNotifications, smsNotifications: $smsNotifications, marketingEmails: $marketingEmails, profileVisibility: $profileVisibility, showOnlineStatus: $showOnlineStatus)';
}


}

/// @nodoc
abstract mixin class _$UserPreferenceDtoCopyWith<$Res> implements $UserPreferenceDtoCopyWith<$Res> {
  factory _$UserPreferenceDtoCopyWith(_UserPreferenceDto value, $Res Function(_UserPreferenceDto) _then) = __$UserPreferenceDtoCopyWithImpl;
@override @useResult
$Res call({
 String id, String language,@JsonKey(unknownEnumValue: ThemeDto.system) ThemeDto theme, String timeZone, bool emailNotifications, bool pushNotifications, bool smsNotifications, bool marketingEmails, bool profileVisibility, bool showOnlineStatus
});




}
/// @nodoc
class __$UserPreferenceDtoCopyWithImpl<$Res>
    implements _$UserPreferenceDtoCopyWith<$Res> {
  __$UserPreferenceDtoCopyWithImpl(this._self, this._then);

  final _UserPreferenceDto _self;
  final $Res Function(_UserPreferenceDto) _then;

/// Create a copy of UserPreferenceDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? language = null,Object? theme = null,Object? timeZone = null,Object? emailNotifications = null,Object? pushNotifications = null,Object? smsNotifications = null,Object? marketingEmails = null,Object? profileVisibility = null,Object? showOnlineStatus = null,}) {
  return _then(_UserPreferenceDto(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,language: null == language ? _self.language : language // ignore: cast_nullable_to_non_nullable
as String,theme: null == theme ? _self.theme : theme // ignore: cast_nullable_to_non_nullable
as ThemeDto,timeZone: null == timeZone ? _self.timeZone : timeZone // ignore: cast_nullable_to_non_nullable
as String,emailNotifications: null == emailNotifications ? _self.emailNotifications : emailNotifications // ignore: cast_nullable_to_non_nullable
as bool,pushNotifications: null == pushNotifications ? _self.pushNotifications : pushNotifications // ignore: cast_nullable_to_non_nullable
as bool,smsNotifications: null == smsNotifications ? _self.smsNotifications : smsNotifications // ignore: cast_nullable_to_non_nullable
as bool,marketingEmails: null == marketingEmails ? _self.marketingEmails : marketingEmails // ignore: cast_nullable_to_non_nullable
as bool,profileVisibility: null == profileVisibility ? _self.profileVisibility : profileVisibility // ignore: cast_nullable_to_non_nullable
as bool,showOnlineStatus: null == showOnlineStatus ? _self.showOnlineStatus : showOnlineStatus // ignore: cast_nullable_to_non_nullable
as bool,
  ));
}


}

// dart format on

// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'update_preferences_request_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$UpdatePreferencesRequestDto {

 String? get language;@JsonKey(unknownEnumValue: ThemeDto.system) ThemeDto? get theme; String? get timeZone; bool? get emailNotifications; bool? get pushNotifications; bool? get smsNotifications; bool? get marketingEmails; bool? get profileVisibility; bool? get showOnlineStatus;
/// Create a copy of UpdatePreferencesRequestDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$UpdatePreferencesRequestDtoCopyWith<UpdatePreferencesRequestDto> get copyWith => _$UpdatePreferencesRequestDtoCopyWithImpl<UpdatePreferencesRequestDto>(this as UpdatePreferencesRequestDto, _$identity);

  /// Serializes this UpdatePreferencesRequestDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is UpdatePreferencesRequestDto&&(identical(other.language, language) || other.language == language)&&(identical(other.theme, theme) || other.theme == theme)&&(identical(other.timeZone, timeZone) || other.timeZone == timeZone)&&(identical(other.emailNotifications, emailNotifications) || other.emailNotifications == emailNotifications)&&(identical(other.pushNotifications, pushNotifications) || other.pushNotifications == pushNotifications)&&(identical(other.smsNotifications, smsNotifications) || other.smsNotifications == smsNotifications)&&(identical(other.marketingEmails, marketingEmails) || other.marketingEmails == marketingEmails)&&(identical(other.profileVisibility, profileVisibility) || other.profileVisibility == profileVisibility)&&(identical(other.showOnlineStatus, showOnlineStatus) || other.showOnlineStatus == showOnlineStatus));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,language,theme,timeZone,emailNotifications,pushNotifications,smsNotifications,marketingEmails,profileVisibility,showOnlineStatus);

@override
String toString() {
  return 'UpdatePreferencesRequestDto(language: $language, theme: $theme, timeZone: $timeZone, emailNotifications: $emailNotifications, pushNotifications: $pushNotifications, smsNotifications: $smsNotifications, marketingEmails: $marketingEmails, profileVisibility: $profileVisibility, showOnlineStatus: $showOnlineStatus)';
}


}

/// @nodoc
abstract mixin class $UpdatePreferencesRequestDtoCopyWith<$Res>  {
  factory $UpdatePreferencesRequestDtoCopyWith(UpdatePreferencesRequestDto value, $Res Function(UpdatePreferencesRequestDto) _then) = _$UpdatePreferencesRequestDtoCopyWithImpl;
@useResult
$Res call({
 String? language,@JsonKey(unknownEnumValue: ThemeDto.system) ThemeDto? theme, String? timeZone, bool? emailNotifications, bool? pushNotifications, bool? smsNotifications, bool? marketingEmails, bool? profileVisibility, bool? showOnlineStatus
});




}
/// @nodoc
class _$UpdatePreferencesRequestDtoCopyWithImpl<$Res>
    implements $UpdatePreferencesRequestDtoCopyWith<$Res> {
  _$UpdatePreferencesRequestDtoCopyWithImpl(this._self, this._then);

  final UpdatePreferencesRequestDto _self;
  final $Res Function(UpdatePreferencesRequestDto) _then;

/// Create a copy of UpdatePreferencesRequestDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? language = freezed,Object? theme = freezed,Object? timeZone = freezed,Object? emailNotifications = freezed,Object? pushNotifications = freezed,Object? smsNotifications = freezed,Object? marketingEmails = freezed,Object? profileVisibility = freezed,Object? showOnlineStatus = freezed,}) {
  return _then(_self.copyWith(
language: freezed == language ? _self.language : language // ignore: cast_nullable_to_non_nullable
as String?,theme: freezed == theme ? _self.theme : theme // ignore: cast_nullable_to_non_nullable
as ThemeDto?,timeZone: freezed == timeZone ? _self.timeZone : timeZone // ignore: cast_nullable_to_non_nullable
as String?,emailNotifications: freezed == emailNotifications ? _self.emailNotifications : emailNotifications // ignore: cast_nullable_to_non_nullable
as bool?,pushNotifications: freezed == pushNotifications ? _self.pushNotifications : pushNotifications // ignore: cast_nullable_to_non_nullable
as bool?,smsNotifications: freezed == smsNotifications ? _self.smsNotifications : smsNotifications // ignore: cast_nullable_to_non_nullable
as bool?,marketingEmails: freezed == marketingEmails ? _self.marketingEmails : marketingEmails // ignore: cast_nullable_to_non_nullable
as bool?,profileVisibility: freezed == profileVisibility ? _self.profileVisibility : profileVisibility // ignore: cast_nullable_to_non_nullable
as bool?,showOnlineStatus: freezed == showOnlineStatus ? _self.showOnlineStatus : showOnlineStatus // ignore: cast_nullable_to_non_nullable
as bool?,
  ));
}

}


/// Adds pattern-matching-related methods to [UpdatePreferencesRequestDto].
extension UpdatePreferencesRequestDtoPatterns on UpdatePreferencesRequestDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _UpdatePreferencesRequestDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _UpdatePreferencesRequestDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _UpdatePreferencesRequestDto value)  $default,){
final _that = this;
switch (_that) {
case _UpdatePreferencesRequestDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _UpdatePreferencesRequestDto value)?  $default,){
final _that = this;
switch (_that) {
case _UpdatePreferencesRequestDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String? language, @JsonKey(unknownEnumValue: ThemeDto.system)  ThemeDto? theme,  String? timeZone,  bool? emailNotifications,  bool? pushNotifications,  bool? smsNotifications,  bool? marketingEmails,  bool? profileVisibility,  bool? showOnlineStatus)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _UpdatePreferencesRequestDto() when $default != null:
return $default(_that.language,_that.theme,_that.timeZone,_that.emailNotifications,_that.pushNotifications,_that.smsNotifications,_that.marketingEmails,_that.profileVisibility,_that.showOnlineStatus);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String? language, @JsonKey(unknownEnumValue: ThemeDto.system)  ThemeDto? theme,  String? timeZone,  bool? emailNotifications,  bool? pushNotifications,  bool? smsNotifications,  bool? marketingEmails,  bool? profileVisibility,  bool? showOnlineStatus)  $default,) {final _that = this;
switch (_that) {
case _UpdatePreferencesRequestDto():
return $default(_that.language,_that.theme,_that.timeZone,_that.emailNotifications,_that.pushNotifications,_that.smsNotifications,_that.marketingEmails,_that.profileVisibility,_that.showOnlineStatus);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String? language, @JsonKey(unknownEnumValue: ThemeDto.system)  ThemeDto? theme,  String? timeZone,  bool? emailNotifications,  bool? pushNotifications,  bool? smsNotifications,  bool? marketingEmails,  bool? profileVisibility,  bool? showOnlineStatus)?  $default,) {final _that = this;
switch (_that) {
case _UpdatePreferencesRequestDto() when $default != null:
return $default(_that.language,_that.theme,_that.timeZone,_that.emailNotifications,_that.pushNotifications,_that.smsNotifications,_that.marketingEmails,_that.profileVisibility,_that.showOnlineStatus);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _UpdatePreferencesRequestDto implements UpdatePreferencesRequestDto {
  const _UpdatePreferencesRequestDto({this.language, @JsonKey(unknownEnumValue: ThemeDto.system) this.theme, this.timeZone, this.emailNotifications, this.pushNotifications, this.smsNotifications, this.marketingEmails, this.profileVisibility, this.showOnlineStatus});
  factory _UpdatePreferencesRequestDto.fromJson(Map<String, dynamic> json) => _$UpdatePreferencesRequestDtoFromJson(json);

@override final  String? language;
@override@JsonKey(unknownEnumValue: ThemeDto.system) final  ThemeDto? theme;
@override final  String? timeZone;
@override final  bool? emailNotifications;
@override final  bool? pushNotifications;
@override final  bool? smsNotifications;
@override final  bool? marketingEmails;
@override final  bool? profileVisibility;
@override final  bool? showOnlineStatus;

/// Create a copy of UpdatePreferencesRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$UpdatePreferencesRequestDtoCopyWith<_UpdatePreferencesRequestDto> get copyWith => __$UpdatePreferencesRequestDtoCopyWithImpl<_UpdatePreferencesRequestDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$UpdatePreferencesRequestDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _UpdatePreferencesRequestDto&&(identical(other.language, language) || other.language == language)&&(identical(other.theme, theme) || other.theme == theme)&&(identical(other.timeZone, timeZone) || other.timeZone == timeZone)&&(identical(other.emailNotifications, emailNotifications) || other.emailNotifications == emailNotifications)&&(identical(other.pushNotifications, pushNotifications) || other.pushNotifications == pushNotifications)&&(identical(other.smsNotifications, smsNotifications) || other.smsNotifications == smsNotifications)&&(identical(other.marketingEmails, marketingEmails) || other.marketingEmails == marketingEmails)&&(identical(other.profileVisibility, profileVisibility) || other.profileVisibility == profileVisibility)&&(identical(other.showOnlineStatus, showOnlineStatus) || other.showOnlineStatus == showOnlineStatus));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,language,theme,timeZone,emailNotifications,pushNotifications,smsNotifications,marketingEmails,profileVisibility,showOnlineStatus);

@override
String toString() {
  return 'UpdatePreferencesRequestDto(language: $language, theme: $theme, timeZone: $timeZone, emailNotifications: $emailNotifications, pushNotifications: $pushNotifications, smsNotifications: $smsNotifications, marketingEmails: $marketingEmails, profileVisibility: $profileVisibility, showOnlineStatus: $showOnlineStatus)';
}


}

/// @nodoc
abstract mixin class _$UpdatePreferencesRequestDtoCopyWith<$Res> implements $UpdatePreferencesRequestDtoCopyWith<$Res> {
  factory _$UpdatePreferencesRequestDtoCopyWith(_UpdatePreferencesRequestDto value, $Res Function(_UpdatePreferencesRequestDto) _then) = __$UpdatePreferencesRequestDtoCopyWithImpl;
@override @useResult
$Res call({
 String? language,@JsonKey(unknownEnumValue: ThemeDto.system) ThemeDto? theme, String? timeZone, bool? emailNotifications, bool? pushNotifications, bool? smsNotifications, bool? marketingEmails, bool? profileVisibility, bool? showOnlineStatus
});




}
/// @nodoc
class __$UpdatePreferencesRequestDtoCopyWithImpl<$Res>
    implements _$UpdatePreferencesRequestDtoCopyWith<$Res> {
  __$UpdatePreferencesRequestDtoCopyWithImpl(this._self, this._then);

  final _UpdatePreferencesRequestDto _self;
  final $Res Function(_UpdatePreferencesRequestDto) _then;

/// Create a copy of UpdatePreferencesRequestDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? language = freezed,Object? theme = freezed,Object? timeZone = freezed,Object? emailNotifications = freezed,Object? pushNotifications = freezed,Object? smsNotifications = freezed,Object? marketingEmails = freezed,Object? profileVisibility = freezed,Object? showOnlineStatus = freezed,}) {
  return _then(_UpdatePreferencesRequestDto(
language: freezed == language ? _self.language : language // ignore: cast_nullable_to_non_nullable
as String?,theme: freezed == theme ? _self.theme : theme // ignore: cast_nullable_to_non_nullable
as ThemeDto?,timeZone: freezed == timeZone ? _self.timeZone : timeZone // ignore: cast_nullable_to_non_nullable
as String?,emailNotifications: freezed == emailNotifications ? _self.emailNotifications : emailNotifications // ignore: cast_nullable_to_non_nullable
as bool?,pushNotifications: freezed == pushNotifications ? _self.pushNotifications : pushNotifications // ignore: cast_nullable_to_non_nullable
as bool?,smsNotifications: freezed == smsNotifications ? _self.smsNotifications : smsNotifications // ignore: cast_nullable_to_non_nullable
as bool?,marketingEmails: freezed == marketingEmails ? _self.marketingEmails : marketingEmails // ignore: cast_nullable_to_non_nullable
as bool?,profileVisibility: freezed == profileVisibility ? _self.profileVisibility : profileVisibility // ignore: cast_nullable_to_non_nullable
as bool?,showOnlineStatus: freezed == showOnlineStatus ? _self.showOnlineStatus : showOnlineStatus // ignore: cast_nullable_to_non_nullable
as bool?,
  ));
}


}

// dart format on

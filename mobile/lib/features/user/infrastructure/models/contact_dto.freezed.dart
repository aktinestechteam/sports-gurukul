// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'contact_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$ContactDto {

 String get id; String? get primaryPhoneCountryCode; String? get primaryPhoneNumber; bool get primaryPhoneVerified; String? get secondaryPhoneCountryCode; String? get secondaryPhoneNumber; bool get secondaryPhoneVerified; String? get websiteUrl; String? get facebookUrl; String? get twitterUrl; String? get instagramUrl; String? get linkedInUrl; String? get youTubeUrl;
/// Create a copy of ContactDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$ContactDtoCopyWith<ContactDto> get copyWith => _$ContactDtoCopyWithImpl<ContactDto>(this as ContactDto, _$identity);

  /// Serializes this ContactDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is ContactDto&&(identical(other.id, id) || other.id == id)&&(identical(other.primaryPhoneCountryCode, primaryPhoneCountryCode) || other.primaryPhoneCountryCode == primaryPhoneCountryCode)&&(identical(other.primaryPhoneNumber, primaryPhoneNumber) || other.primaryPhoneNumber == primaryPhoneNumber)&&(identical(other.primaryPhoneVerified, primaryPhoneVerified) || other.primaryPhoneVerified == primaryPhoneVerified)&&(identical(other.secondaryPhoneCountryCode, secondaryPhoneCountryCode) || other.secondaryPhoneCountryCode == secondaryPhoneCountryCode)&&(identical(other.secondaryPhoneNumber, secondaryPhoneNumber) || other.secondaryPhoneNumber == secondaryPhoneNumber)&&(identical(other.secondaryPhoneVerified, secondaryPhoneVerified) || other.secondaryPhoneVerified == secondaryPhoneVerified)&&(identical(other.websiteUrl, websiteUrl) || other.websiteUrl == websiteUrl)&&(identical(other.facebookUrl, facebookUrl) || other.facebookUrl == facebookUrl)&&(identical(other.twitterUrl, twitterUrl) || other.twitterUrl == twitterUrl)&&(identical(other.instagramUrl, instagramUrl) || other.instagramUrl == instagramUrl)&&(identical(other.linkedInUrl, linkedInUrl) || other.linkedInUrl == linkedInUrl)&&(identical(other.youTubeUrl, youTubeUrl) || other.youTubeUrl == youTubeUrl));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,primaryPhoneCountryCode,primaryPhoneNumber,primaryPhoneVerified,secondaryPhoneCountryCode,secondaryPhoneNumber,secondaryPhoneVerified,websiteUrl,facebookUrl,twitterUrl,instagramUrl,linkedInUrl,youTubeUrl);

@override
String toString() {
  return 'ContactDto(id: $id, primaryPhoneCountryCode: $primaryPhoneCountryCode, primaryPhoneNumber: $primaryPhoneNumber, primaryPhoneVerified: $primaryPhoneVerified, secondaryPhoneCountryCode: $secondaryPhoneCountryCode, secondaryPhoneNumber: $secondaryPhoneNumber, secondaryPhoneVerified: $secondaryPhoneVerified, websiteUrl: $websiteUrl, facebookUrl: $facebookUrl, twitterUrl: $twitterUrl, instagramUrl: $instagramUrl, linkedInUrl: $linkedInUrl, youTubeUrl: $youTubeUrl)';
}


}

/// @nodoc
abstract mixin class $ContactDtoCopyWith<$Res>  {
  factory $ContactDtoCopyWith(ContactDto value, $Res Function(ContactDto) _then) = _$ContactDtoCopyWithImpl;
@useResult
$Res call({
 String id, String? primaryPhoneCountryCode, String? primaryPhoneNumber, bool primaryPhoneVerified, String? secondaryPhoneCountryCode, String? secondaryPhoneNumber, bool secondaryPhoneVerified, String? websiteUrl, String? facebookUrl, String? twitterUrl, String? instagramUrl, String? linkedInUrl, String? youTubeUrl
});




}
/// @nodoc
class _$ContactDtoCopyWithImpl<$Res>
    implements $ContactDtoCopyWith<$Res> {
  _$ContactDtoCopyWithImpl(this._self, this._then);

  final ContactDto _self;
  final $Res Function(ContactDto) _then;

/// Create a copy of ContactDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? primaryPhoneCountryCode = freezed,Object? primaryPhoneNumber = freezed,Object? primaryPhoneVerified = null,Object? secondaryPhoneCountryCode = freezed,Object? secondaryPhoneNumber = freezed,Object? secondaryPhoneVerified = null,Object? websiteUrl = freezed,Object? facebookUrl = freezed,Object? twitterUrl = freezed,Object? instagramUrl = freezed,Object? linkedInUrl = freezed,Object? youTubeUrl = freezed,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,primaryPhoneCountryCode: freezed == primaryPhoneCountryCode ? _self.primaryPhoneCountryCode : primaryPhoneCountryCode // ignore: cast_nullable_to_non_nullable
as String?,primaryPhoneNumber: freezed == primaryPhoneNumber ? _self.primaryPhoneNumber : primaryPhoneNumber // ignore: cast_nullable_to_non_nullable
as String?,primaryPhoneVerified: null == primaryPhoneVerified ? _self.primaryPhoneVerified : primaryPhoneVerified // ignore: cast_nullable_to_non_nullable
as bool,secondaryPhoneCountryCode: freezed == secondaryPhoneCountryCode ? _self.secondaryPhoneCountryCode : secondaryPhoneCountryCode // ignore: cast_nullable_to_non_nullable
as String?,secondaryPhoneNumber: freezed == secondaryPhoneNumber ? _self.secondaryPhoneNumber : secondaryPhoneNumber // ignore: cast_nullable_to_non_nullable
as String?,secondaryPhoneVerified: null == secondaryPhoneVerified ? _self.secondaryPhoneVerified : secondaryPhoneVerified // ignore: cast_nullable_to_non_nullable
as bool,websiteUrl: freezed == websiteUrl ? _self.websiteUrl : websiteUrl // ignore: cast_nullable_to_non_nullable
as String?,facebookUrl: freezed == facebookUrl ? _self.facebookUrl : facebookUrl // ignore: cast_nullable_to_non_nullable
as String?,twitterUrl: freezed == twitterUrl ? _self.twitterUrl : twitterUrl // ignore: cast_nullable_to_non_nullable
as String?,instagramUrl: freezed == instagramUrl ? _self.instagramUrl : instagramUrl // ignore: cast_nullable_to_non_nullable
as String?,linkedInUrl: freezed == linkedInUrl ? _self.linkedInUrl : linkedInUrl // ignore: cast_nullable_to_non_nullable
as String?,youTubeUrl: freezed == youTubeUrl ? _self.youTubeUrl : youTubeUrl // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}

}


/// Adds pattern-matching-related methods to [ContactDto].
extension ContactDtoPatterns on ContactDto {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _ContactDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _ContactDto() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _ContactDto value)  $default,){
final _that = this;
switch (_that) {
case _ContactDto():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _ContactDto value)?  $default,){
final _that = this;
switch (_that) {
case _ContactDto() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String? primaryPhoneCountryCode,  String? primaryPhoneNumber,  bool primaryPhoneVerified,  String? secondaryPhoneCountryCode,  String? secondaryPhoneNumber,  bool secondaryPhoneVerified,  String? websiteUrl,  String? facebookUrl,  String? twitterUrl,  String? instagramUrl,  String? linkedInUrl,  String? youTubeUrl)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _ContactDto() when $default != null:
return $default(_that.id,_that.primaryPhoneCountryCode,_that.primaryPhoneNumber,_that.primaryPhoneVerified,_that.secondaryPhoneCountryCode,_that.secondaryPhoneNumber,_that.secondaryPhoneVerified,_that.websiteUrl,_that.facebookUrl,_that.twitterUrl,_that.instagramUrl,_that.linkedInUrl,_that.youTubeUrl);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String? primaryPhoneCountryCode,  String? primaryPhoneNumber,  bool primaryPhoneVerified,  String? secondaryPhoneCountryCode,  String? secondaryPhoneNumber,  bool secondaryPhoneVerified,  String? websiteUrl,  String? facebookUrl,  String? twitterUrl,  String? instagramUrl,  String? linkedInUrl,  String? youTubeUrl)  $default,) {final _that = this;
switch (_that) {
case _ContactDto():
return $default(_that.id,_that.primaryPhoneCountryCode,_that.primaryPhoneNumber,_that.primaryPhoneVerified,_that.secondaryPhoneCountryCode,_that.secondaryPhoneNumber,_that.secondaryPhoneVerified,_that.websiteUrl,_that.facebookUrl,_that.twitterUrl,_that.instagramUrl,_that.linkedInUrl,_that.youTubeUrl);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String? primaryPhoneCountryCode,  String? primaryPhoneNumber,  bool primaryPhoneVerified,  String? secondaryPhoneCountryCode,  String? secondaryPhoneNumber,  bool secondaryPhoneVerified,  String? websiteUrl,  String? facebookUrl,  String? twitterUrl,  String? instagramUrl,  String? linkedInUrl,  String? youTubeUrl)?  $default,) {final _that = this;
switch (_that) {
case _ContactDto() when $default != null:
return $default(_that.id,_that.primaryPhoneCountryCode,_that.primaryPhoneNumber,_that.primaryPhoneVerified,_that.secondaryPhoneCountryCode,_that.secondaryPhoneNumber,_that.secondaryPhoneVerified,_that.websiteUrl,_that.facebookUrl,_that.twitterUrl,_that.instagramUrl,_that.linkedInUrl,_that.youTubeUrl);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _ContactDto implements ContactDto {
  const _ContactDto({required this.id, this.primaryPhoneCountryCode, this.primaryPhoneNumber, this.primaryPhoneVerified = false, this.secondaryPhoneCountryCode, this.secondaryPhoneNumber, this.secondaryPhoneVerified = false, this.websiteUrl, this.facebookUrl, this.twitterUrl, this.instagramUrl, this.linkedInUrl, this.youTubeUrl});
  factory _ContactDto.fromJson(Map<String, dynamic> json) => _$ContactDtoFromJson(json);

@override final  String id;
@override final  String? primaryPhoneCountryCode;
@override final  String? primaryPhoneNumber;
@override@JsonKey() final  bool primaryPhoneVerified;
@override final  String? secondaryPhoneCountryCode;
@override final  String? secondaryPhoneNumber;
@override@JsonKey() final  bool secondaryPhoneVerified;
@override final  String? websiteUrl;
@override final  String? facebookUrl;
@override final  String? twitterUrl;
@override final  String? instagramUrl;
@override final  String? linkedInUrl;
@override final  String? youTubeUrl;

/// Create a copy of ContactDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ContactDtoCopyWith<_ContactDto> get copyWith => __$ContactDtoCopyWithImpl<_ContactDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$ContactDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ContactDto&&(identical(other.id, id) || other.id == id)&&(identical(other.primaryPhoneCountryCode, primaryPhoneCountryCode) || other.primaryPhoneCountryCode == primaryPhoneCountryCode)&&(identical(other.primaryPhoneNumber, primaryPhoneNumber) || other.primaryPhoneNumber == primaryPhoneNumber)&&(identical(other.primaryPhoneVerified, primaryPhoneVerified) || other.primaryPhoneVerified == primaryPhoneVerified)&&(identical(other.secondaryPhoneCountryCode, secondaryPhoneCountryCode) || other.secondaryPhoneCountryCode == secondaryPhoneCountryCode)&&(identical(other.secondaryPhoneNumber, secondaryPhoneNumber) || other.secondaryPhoneNumber == secondaryPhoneNumber)&&(identical(other.secondaryPhoneVerified, secondaryPhoneVerified) || other.secondaryPhoneVerified == secondaryPhoneVerified)&&(identical(other.websiteUrl, websiteUrl) || other.websiteUrl == websiteUrl)&&(identical(other.facebookUrl, facebookUrl) || other.facebookUrl == facebookUrl)&&(identical(other.twitterUrl, twitterUrl) || other.twitterUrl == twitterUrl)&&(identical(other.instagramUrl, instagramUrl) || other.instagramUrl == instagramUrl)&&(identical(other.linkedInUrl, linkedInUrl) || other.linkedInUrl == linkedInUrl)&&(identical(other.youTubeUrl, youTubeUrl) || other.youTubeUrl == youTubeUrl));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,primaryPhoneCountryCode,primaryPhoneNumber,primaryPhoneVerified,secondaryPhoneCountryCode,secondaryPhoneNumber,secondaryPhoneVerified,websiteUrl,facebookUrl,twitterUrl,instagramUrl,linkedInUrl,youTubeUrl);

@override
String toString() {
  return 'ContactDto(id: $id, primaryPhoneCountryCode: $primaryPhoneCountryCode, primaryPhoneNumber: $primaryPhoneNumber, primaryPhoneVerified: $primaryPhoneVerified, secondaryPhoneCountryCode: $secondaryPhoneCountryCode, secondaryPhoneNumber: $secondaryPhoneNumber, secondaryPhoneVerified: $secondaryPhoneVerified, websiteUrl: $websiteUrl, facebookUrl: $facebookUrl, twitterUrl: $twitterUrl, instagramUrl: $instagramUrl, linkedInUrl: $linkedInUrl, youTubeUrl: $youTubeUrl)';
}


}

/// @nodoc
abstract mixin class _$ContactDtoCopyWith<$Res> implements $ContactDtoCopyWith<$Res> {
  factory _$ContactDtoCopyWith(_ContactDto value, $Res Function(_ContactDto) _then) = __$ContactDtoCopyWithImpl;
@override @useResult
$Res call({
 String id, String? primaryPhoneCountryCode, String? primaryPhoneNumber, bool primaryPhoneVerified, String? secondaryPhoneCountryCode, String? secondaryPhoneNumber, bool secondaryPhoneVerified, String? websiteUrl, String? facebookUrl, String? twitterUrl, String? instagramUrl, String? linkedInUrl, String? youTubeUrl
});




}
/// @nodoc
class __$ContactDtoCopyWithImpl<$Res>
    implements _$ContactDtoCopyWith<$Res> {
  __$ContactDtoCopyWithImpl(this._self, this._then);

  final _ContactDto _self;
  final $Res Function(_ContactDto) _then;

/// Create a copy of ContactDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? primaryPhoneCountryCode = freezed,Object? primaryPhoneNumber = freezed,Object? primaryPhoneVerified = null,Object? secondaryPhoneCountryCode = freezed,Object? secondaryPhoneNumber = freezed,Object? secondaryPhoneVerified = null,Object? websiteUrl = freezed,Object? facebookUrl = freezed,Object? twitterUrl = freezed,Object? instagramUrl = freezed,Object? linkedInUrl = freezed,Object? youTubeUrl = freezed,}) {
  return _then(_ContactDto(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,primaryPhoneCountryCode: freezed == primaryPhoneCountryCode ? _self.primaryPhoneCountryCode : primaryPhoneCountryCode // ignore: cast_nullable_to_non_nullable
as String?,primaryPhoneNumber: freezed == primaryPhoneNumber ? _self.primaryPhoneNumber : primaryPhoneNumber // ignore: cast_nullable_to_non_nullable
as String?,primaryPhoneVerified: null == primaryPhoneVerified ? _self.primaryPhoneVerified : primaryPhoneVerified // ignore: cast_nullable_to_non_nullable
as bool,secondaryPhoneCountryCode: freezed == secondaryPhoneCountryCode ? _self.secondaryPhoneCountryCode : secondaryPhoneCountryCode // ignore: cast_nullable_to_non_nullable
as String?,secondaryPhoneNumber: freezed == secondaryPhoneNumber ? _self.secondaryPhoneNumber : secondaryPhoneNumber // ignore: cast_nullable_to_non_nullable
as String?,secondaryPhoneVerified: null == secondaryPhoneVerified ? _self.secondaryPhoneVerified : secondaryPhoneVerified // ignore: cast_nullable_to_non_nullable
as bool,websiteUrl: freezed == websiteUrl ? _self.websiteUrl : websiteUrl // ignore: cast_nullable_to_non_nullable
as String?,facebookUrl: freezed == facebookUrl ? _self.facebookUrl : facebookUrl // ignore: cast_nullable_to_non_nullable
as String?,twitterUrl: freezed == twitterUrl ? _self.twitterUrl : twitterUrl // ignore: cast_nullable_to_non_nullable
as String?,instagramUrl: freezed == instagramUrl ? _self.instagramUrl : instagramUrl // ignore: cast_nullable_to_non_nullable
as String?,linkedInUrl: freezed == linkedInUrl ? _self.linkedInUrl : linkedInUrl // ignore: cast_nullable_to_non_nullable
as String?,youTubeUrl: freezed == youTubeUrl ? _self.youTubeUrl : youTubeUrl // ignore: cast_nullable_to_non_nullable
as String?,
  ));
}


}

// dart format on

/// Transport-level API constants.
///
/// These constants describe *how* requests are made, never endpoint shapes.
/// Endpoint paths are owned by the networking layer (a later sprint) and are
/// derived from the backend OpenAPI specification; they are not duplicated
/// here. Backend routes live under the `api/v1` prefix.
abstract final class ApiConstants {
  /// Base URL, injectable at build time through `--dart-define=API_BASE_URL`.
  static const String baseUrl = String.fromEnvironment('API_BASE_URL');

  /// Version segment of the backend route prefix.
  static const String apiVersion = 'v1';

  /// Backend route prefix shared by every controller.
  static const String apiBasePath = 'api/v1';

  /// Content-Type header value for JSON payloads.
  static const String contentTypeJson = 'application/json';

  /// Accept header value for JSON responses.
  static const String acceptJson = 'application/json';

  /// Name of the authorization header.
  static const String authorizationHeader = 'Authorization';

  /// Scheme prefix used for bearer tokens.
  static const String bearerPrefix = 'Bearer ';

  /// Name of the correlation/request identifier header.
  static const String requestIdHeader = 'X-Request-Id';

  /// Successful response status codes.
  static const int statusOk = 200;
  static const int statusCreated = 201;
  static const int statusNoContent = 204;

  /// Client-error status codes.
  static const int statusBadRequest = 400;
  static const int statusUnauthorized = 401;
  static const int statusForbidden = 403;
  static const int statusNotFound = 404;
  static const int statusConflict = 409;
  static const int statusUnprocessableEntity = 422;

  /// Server-error status codes.
  static const int statusInternalServerError = 500;
  static const int statusServiceUnavailable = 503;
}

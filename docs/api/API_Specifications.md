# API Specifications

Version: 1.0

## Overview
All APIs follow REST principles with JSON payloads and JWT authentication.

## Standards
- Base URL: /api/v1
- Content-Type: application/json
- HTTPS only
- JWT Bearer authentication
- Standard error responses

## Common Response Format
Success:
{
  "success": true,
  "data": {}
}

Error:
{
  "success": false,
  "code": "AUTH-001",
  "message": "Invalid credentials"
}

## Authentication APIs
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout

## Athlete APIs
GET /api/v1/athletes
POST /api/v1/athletes
PUT /api/v1/athletes/{id}

## Coach APIs
GET /api/v1/coaches
POST /api/v1/coaches
PUT /api/v1/coaches/{id}

## Booking APIs
POST /api/v1/bookings
PUT /api/v1/bookings/{id}/cancel

## Payment APIs
POST /api/v1/payments/create
POST /api/v1/payments/webhook

## Versioning
- URI versioning
- Backward compatibility

## Security
- JWT
- Rate limiting
- Input validation
- Idempotency for payment APIs

## Future
- OpenAPI 3.1
- GraphQL gateway

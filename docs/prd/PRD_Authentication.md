# PRD - Authentication Module

Version: 1.0

## 1. Purpose
Provide secure authentication and authorization for all Sports Gurukul users.

## 2. Supported Roles
- Athlete
- Coach
- Academy
- Parent
- Scout
- Sponsor
- Admin

## 3. Registration Methods
- Mobile + OTP
- Email + Password
- Google
- Apple (Future)
- Microsoft (Future)

## 4. Functional Requirements

### FR-AUTH-001 User Registration
- Validate input
- Check duplicate mobile/email
- Send OTP
- Verify OTP
- Create account
- Assign default role
- Create audit log

### FR-AUTH-002 Login
- Email/password
- Mobile/OTP
- JWT access token
- Refresh token

### FR-AUTH-003 Password Reset
- Forgot password
- OTP verification
- Password policy

### FR-AUTH-004 Session Management
- Active sessions
- Logout current
- Logout all devices

### FR-AUTH-005 MFA
- Optional OTP
- Future authenticator support

## 5. Validation Rules
- Password >=8 chars
- Upper/lowercase
- Number
- Special character
- OTP expires in 5 minutes

## 6. Database Tables
- Users
- Roles
- UserRoles
- OTP
- Sessions
- RefreshTokens
- AuditLogs

## 7. APIs
POST /api/auth/register
POST /api/auth/login
POST /api/auth/verify-otp
POST /api/auth/forgot-password
POST /api/auth/reset-password
POST /api/auth/logout
POST /api/auth/refresh

## 8. Security
- JWT
- HTTPS
- Password hashing
- Rate limiting
- Account lock after repeated failures

## 9. Acceptance Criteria
- Successful registration
- Duplicate prevention
- Secure login
- Refresh token rotation
- Audit logging

## 10. Future
- Passkeys
- Biometric login

---
title: Forgot Password Screen Specification
module: Authentication
screen: Forgot Password
platform: Flutter
backend: Identity Platform
version: 1.0
status: Draft
owner: Product Team
---

# 🔑 Forgot Password

> Enables users to securely recover access to their Sports Gurukul account using mobile number or email verification while protecting against account enumeration and unauthorized access.

---

# Table of Contents

1. Purpose
2. Business Goals
3. Password Recovery Flow
4. User Stories
5. Screen Flow
6. UI Specifications
7. Flutter Widget Tree
8. Backend API Integration
9. Password Policy
10. Validation Rules
11. State Management
12. Error Handling
13. Security
14. Accessibility
15. Analytics
16. Acceptance Criteria

---

# 1. Purpose

The password recovery process must be simple, secure, and complete within two minutes.

Supported recovery methods:

- Mobile Number + OTP
- Email + OTP

Future:

- Passkey Recovery
- Biometric Recovery
- Trusted Device Recovery

---

# 2. Business Goals

- Reduce password reset failures
- Minimize support requests
- Prevent unauthorized account recovery
- Complete reset in less than 2 minutes

---

# 3. Password Recovery Flow

```text
Forgot Password

↓

Enter Email / Mobile

↓

Validate Account

↓

Generate OTP

↓

OTP Verification

↓

Enter New Password

↓

Confirm Password

↓

Password Updated

↓

Login
```

---

# 4. User Stories

### Athlete

As an athlete,

I forgot my password,

I want to reset it quickly,

so I can continue my training.

---

### Parent

As a parent,

I want a secure recovery process,

so nobody else can access my account.

---

### Coach

As a coach,

I want to recover my account without contacting support.

---

# 5. Screen Layout

```
┌────────────────────────────┐

← Back

Forgot Password

Recover your account

────────────────────────────

Email or Mobile

[________________]

────────────────────────────

[ Continue ]

────────────────────────────

Remember Password?

Login

└────────────────────────────┘
```

---

# 6. Password Reset Screen

```
┌────────────────────────────┐

Create New Password

────────────────────────────

New Password

[______________]

Strength Meter

Weak

Medium

Strong

────────────────────────────

Confirm Password

[______________]

────────────────────────────

✔ Minimum 8 characters

✔ Uppercase

✔ Lowercase

✔ Number

✔ Special Character

────────────────────────────

[ Update Password ]

└────────────────────────────┘
```

---

# 7. Flutter Widget Tree

```text
Scaffold

SafeArea

Column

AppBar

Instruction

EmailOrMobileField

ContinueButton

PasswordField

ConfirmPasswordField

PasswordStrengthIndicator

UpdateButton
```

---

# 8. Backend API Integration

## Request Password Reset

```
POST /api/v1/auth/forgot-password
```

Request

```json
{
  "username": "athlete@example.com"
}
```

---

## Verify OTP

```
POST /api/v1/auth/verify-reset-otp
```

---

## Reset Password

```
POST /api/v1/auth/reset-password
```

Request

```json
{
  "requestId": "12345",
  "otp": "458921",
  "newPassword": "StrongPassword@123"
}
```

---

# 9. Password Policy

Minimum Length

```
8 Characters
```

Maximum Length

```
64 Characters
```

Must Include

- Uppercase Letter
- Lowercase Letter
- Number
- Special Character

Must Not

- Match previous passwords
- Contain username
- Contain mobile number

---

# 10. Validation Rules

Email

- RFC compliant

Mobile

- Valid country code
- Valid number

Password

- Strong policy validation

Confirm Password

- Must match

---

# 11. State Management

Providers

```
ForgotPasswordProvider

OTPProvider

PasswordPolicyProvider

AuthenticationProvider
```

---

State Flow

```
Idle

↓

Requesting OTP

↓

OTP Sent

↓

Verifying OTP

↓

Updating Password

↓

Success
```

---

# 12. Error Handling

| Status | UI Message                |
| ------ | ------------------------- |
| 400    | Invalid request           |
| 401    | OTP expired               |
| 403    | Password policy violation |
| 404    | Account not found\*       |
| 409    | Password already used     |
| 429    | Too many requests         |
| 500    | Something went wrong      |

\*For security, the UI should display a generic message such as "If an account exists, you'll receive reset instructions."

---

# 13. Security

- Never reveal whether an account exists.
- OTP expires after 5 minutes.
- Passwords never stored locally.
- Secure Storage only.
- HTTPS required.
- Certificate Pinning.
- Rate limiting.
- Audit logging.
- Force logout from all devices after password reset (recommended).

---

# 14. Accessibility

Supports

- Screen Reader
- Dynamic Font
- High Contrast
- VoiceOver
- TalkBack
- Keyboard Navigation

---

# 15. Analytics Events

```text
forgot_password_opened

forgot_password_requested

reset_otp_sent

reset_otp_verified

password_strength_updated

password_reset_success

password_reset_failed
```

---

# 16. Performance Goals

OTP Request

<300 ms

Password Reset

<500 ms

Navigation

<100 ms

---

# 17. Acceptance Criteria

- Supports email and mobile recovery
- Enforces password policy
- Uses OTP verification
- Prevents account enumeration
- Accessible and responsive
- Fully integrated with Identity Platform
- Logs security events
- Supports session invalidation after reset

---

# Related Backend APIs

| API                                | Purpose                                  |
| ---------------------------------- | ---------------------------------------- |
| POST /api/v1/auth/forgot-password  | Request password reset                   |
| POST /api/v1/auth/verify-reset-otp | Verify reset OTP                         |
| POST /api/v1/auth/reset-password   | Update password                          |
| POST /api/v1/auth/logout-all       | Invalidate active sessions (recommended) |

---

# Related Documents

- 01-Splash.md
- 02-Welcome.md
- 03-Login.md
- 04-OTP-Verification.md
- 06-Biometric-Authentication.md

---

**End of Document**

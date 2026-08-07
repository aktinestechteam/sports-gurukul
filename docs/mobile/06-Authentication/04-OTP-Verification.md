---
title: OTP Verification Screen
module: Authentication
screen: OTP Verification
platform: Flutter
backend: Identity Platform
version: 1.0
status: Draft
owner: Product Team
---

# 🔑 OTP Verification

> The OTP Verification screen validates user identity during login, registration, password reset, device verification, and sensitive account operations.

---

# Table of Contents

1. Purpose
2. Business Goals
3. Supported Scenarios
4. User Stories
5. UI Layout
6. Authentication Flow
7. Flutter Widget Tree
8. Backend API Integration
9. OTP Validation Rules
10. Auto Read OTP
11. Resend OTP
12. State Management
13. Error Handling
14. Security
15. Accessibility
16. Analytics
17. Acceptance Criteria

---

# 1. Purpose

OTP verification ensures the person using the application owns the registered mobile number or email.

The screen should minimize user effort while maintaining enterprise-grade security.

---

# 2. Business Goals

- Verify users within 30 seconds
- Reduce OTP failures
- Support automatic OTP detection
- Minimize customer support calls
- Prevent unauthorized access

---

# 3. Supported Scenarios

✔ Login

✔ Registration

✔ Forgot Password

✔ Device Verification

✔ Change Mobile Number

✔ Change Email

✔ High Risk Login

✔ Two Factor Authentication

---

# 4. User Stories

### Athlete

As an athlete,

I want the OTP to be detected automatically,

so I don't need to type it.

---

### Parent

As a parent,

I want to receive OTP quickly,

so I can securely access my child's account.

---

### Coach

As a coach,

I want a reliable verification process,

so I can continue managing my athletes.

---

# 5. Screen Layout

```
┌──────────────────────────────┐

← Back

Verify OTP

We've sent a verification code to

+91 98XXXXXX45

──────────────────────────────

○ ○ ○ ○ ○ ○

(6 Digit OTP)

──────────────────────────────

⏱ 00:59

Didn't receive it?

Resend OTP

──────────────────────────────

[ Verify ]

──────────────────────────────

Change Mobile Number

Need Help?

└──────────────────────────────┘
```

---

# 6. Authentication Flow

```
User

↓

Enter Mobile

↓

Receive OTP

↓

Enter / Auto Detect OTP

↓

POST /api/v1/auth/verify-otp

↓

OTP Valid?

├── No

│

Error

│

Retry

│

└── Yes

↓

Generate JWT

↓

Load Profile

↓

Dashboard
```

---

# 7. Flutter Widget Tree

```
Scaffold

SafeArea

Column

AppBar

Instruction Text

OTPInputWidget

CountdownTimer

ResendButton

PrimaryButton

HelpLink
```

---

# 8. Backend API Integration

## Verify OTP

```
POST /api/v1/auth/verify-otp
```

Request

```json
{
  "mobileNumber": "9876543210",
  "otp": "458921",
  "deviceId": "DEVICE_UUID"
}
```

---

Success Response

```json
{
  "accessToken": "...",
  "refreshToken": "...",
  "expiresIn": 3600,
  "user": {
    "id": "123",
    "role": "Athlete"
  }
}
```

---

## Resend OTP

```
POST /api/v1/auth/resend-otp
```

---

## Validate OTP Status

```
GET /api/v1/auth/otp-status/{requestId}
```

---

# 9. OTP Validation Rules

Length

```
6 Digits
```

Expiry

```
5 Minutes
```

Maximum Attempts

```
5
```

Lock Duration

```
15 Minutes
```

---

# 10. Auto Read OTP

Android

- SMS Retriever API
- Automatic detection
- No SMS permission required

iOS

- AutoFill from Messages

Manual entry remains available.

---

# 11. Resend OTP

Resend available after

```
60 Seconds
```

Maximum resend attempts

```
5
```

Display countdown timer.

Disable button until timer expires.

---

# 12. State Management

Riverpod Providers

```
OtpProvider

AuthenticationProvider

TimerProvider

ConnectivityProvider
```

State Flow

```
Idle

↓

Sending OTP

↓

Waiting

↓

Verifying

↓

Verified

↓

Error
```

---

# 13. Loading States

Sending OTP

Verifying OTP

Generating Session

Loading Dashboard

---

# 14. Error Handling

| Status | UI Action                |
| ------ | ------------------------ |
| 400    | Invalid OTP              |
| 401    | OTP Expired              |
| 403    | Maximum Attempts Reached |
| 404    | OTP Request Not Found    |
| 409    | OTP Already Used         |
| 429    | Too Many Requests        |
| 500    | Retry                    |

---

# 15. Security

OTP never stored locally.

Never log OTP.

Mask mobile number.

Use HTTPS only.

Certificate pinning enabled.

Prevent screenshots on Android/iOS where supported.

Detect rooted/jailbroken devices.

---

# 16. Accessibility

Supports

- Screen Reader
- Dynamic Font
- VoiceOver
- TalkBack
- High Contrast
- Keyboard Navigation

---

# 17. Analytics Events

```
otp_screen_opened

otp_received

otp_auto_detected

otp_verified

otp_failed

otp_resend_clicked

otp_timeout

otp_max_attempts
```

---

# 18. Performance Goals

OTP Verification

< 300 ms

Auto Read

< 5 sec

Navigation

< 100 ms

---

# 19. Acceptance Criteria

- Supports automatic OTP detection
- Supports manual entry
- Countdown timer works correctly
- Resend button enabled after timer
- Handles expired OTP
- Handles invalid OTP
- Generates authenticated session
- Fully accessible
- Secure implementation
- Backend integrated

---

# Related Backend APIs

| API                                     | Purpose          |
| --------------------------------------- | ---------------- |
| POST /api/v1/auth/verify-otp            | Verify OTP       |
| POST /api/v1/auth/resend-otp            | Resend OTP       |
| GET /api/v1/auth/otp-status/{requestId} | Check OTP status |

---

# Related Documents

- 01-Splash.md
- 02-Welcome.md
- 03-Login.md
- 05-Forgot-Password.md
- 06-Biometric-Authentication.md

---

**End of Document**

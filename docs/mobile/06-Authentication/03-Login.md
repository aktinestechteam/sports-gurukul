---
title: Login Screen Specification
module: Authentication
screen: Login
platform: Flutter
backend: Identity Platform
version: 1.0
status: Draft
owner: Product Team
---

# 🔐 Login Screen

> The Login screen provides a secure, fast, and intuitive authentication experience for Athletes, Parents, and Coaches while integrating seamlessly with the Sports Gurukul Identity Platform.

---

# Table of Contents

1. Purpose
2. Business Goals
3. User Stories
4. Screen Layout
5. Login Methods
6. Authentication Flow
7. Flutter Widget Tree
8. Backend API Integration
9. Validation Rules
10. State Management
11. Error Handling
12. Security
13. Accessibility
14. Analytics
15. Acceptance Criteria

---

# 1. Purpose

Allow registered users to securely authenticate using the Sports Gurukul Identity Platform.

Supported Users

- Athlete
- Parent
- Coach

Future

- Academy Admin
- Tournament Organizer

---

# 2. Business Goals

- Login within 30 seconds
- Reduce authentication failures
- Support biometric login
- Support multiple devices
- Enable seamless session restoration

---

# 3. User Stories

### Athlete

As an athlete,

I want to login quickly,

so I can view today's training.

---

### Parent

As a parent,

I want secure login,

so I can monitor my child's progress.

---

### Coach

As a coach,

I want immediate access,

so I can manage today's sessions.

---

# 4. Screen Layout

```
┌──────────────────────────────┐

         Sports Gurukul

 Welcome 👋

 Continue your sports journey

──────────────────────────────

📱 Mobile Number

[______________]

OR

📧 Email Address

[______________]

🔒 Password

[______________]

👁 Show Password

☐ Remember this device

Forgot Password?

──────────────────────────────

[ Login ]

──────────────────────────────

🔒 Login with Biometrics

──────────────────────────────

Don't have an account?

Create Account

──────────────────────────────

Version 1.0

└──────────────────────────────┘
```

---

# 5. Supported Login Methods

### Mobile + Password

Preferred

---

### Email + Password

Supported

---

### Biometric

Fingerprint

Face ID

Future

Passkeys

---

### Future

Google Login

Apple Login

Microsoft Login

---

# 6. Authentication Flow

```text
User

↓

Enter Credentials

↓

Client Validation

↓

POST /api/v1/auth/login

↓

Identity Platform

↓

JWT

↓

Refresh Token

↓

Secure Storage

↓

Load User Profile

↓

Load Permissions

↓

Dashboard
```

---

# 7. Flutter Widget Tree

```text
Scaffold

SafeArea

SingleChildScrollView

Column

Logo

Title

Subtitle

LoginForm

EmailOrPhoneField

PasswordField

RememberDeviceCheckbox

ForgotPasswordLink

PrimaryButton

BiometricButton

RegisterLink

VersionLabel
```

---

# 8. Backend API Integration

## Login

```
POST /api/v1/auth/login
```

Request

```json
{
  "username": "athlete@example.com",
  "password": "********",
  "deviceId": "DEVICE_UUID",
  "deviceName": "Samsung S24",
  "platform": "Android"
}
```

Success Response

```json
{
  "accessToken": "...",
  "refreshToken": "...",
  "expiresIn": 3600,
  "user": {
    "id": "...",
    "role": "Athlete"
  }
}
```

---

## Load Profile

```
GET /api/v1/profile
```

---

## Refresh Token

```
POST /api/v1/auth/refresh-token
```

---

# 9. Validation Rules

### Mobile

- Required
- 10 digits
- Country code supported

### Email

- RFC compliant format

### Password

- Required
- Minimum 8 characters

Disable Login button until form is valid.

---

# 10. State Management

Riverpod Providers

```text
AuthenticationProvider

LoginControllerProvider

SecureStorageProvider

ConnectivityProvider

ProfileProvider
```

Flow

```text
Login Screen

↓

Login Controller

↓

Authentication Repository

↓

API Client

↓

Identity Platform
```

---

# 11. Loading States

- Authenticating...
- Loading Profile...
- Restoring Session...

Disable all input controls while login is in progress.

---

# 12. Error Handling

| Status | UI Message                              |
| ------ | --------------------------------------- |
| 400    | Please check your input.                |
| 401    | Invalid username or password.           |
| 403    | Your account is not authorized.         |
| 423    | Account temporarily locked.             |
| 429    | Too many attempts. Try again later.     |
| 500    | Something went wrong. Please try again. |

---

# 13. Security

- JWT stored in encrypted secure storage
- Refresh Token never exposed to UI
- Password field protected
- Certificate Pinning
- Root/Jailbreak detection
- Session timeout supported
- Device registration
- No sensitive logging

---

# 14. Accessibility

- Screen reader labels
- Keyboard navigation
- High contrast mode
- Dynamic font scaling
- Minimum touch target 48dp

---

# 15. Analytics Events

```text
login_screen_opened

login_attempt

login_success

login_failure

forgot_password_clicked

biometric_login_clicked

register_clicked
```

---

# Acceptance Criteria

- Login completes in <2 seconds under normal conditions
- Supports email and mobile authentication
- Integrates with JWT & Refresh Token flow
- Supports biometric authentication
- Handles offline and server error scenarios gracefully
- Fully accessible (WCAG 2.2 AA)
- Responsive across phones and tablets
- Compatible with Android and iOS

---

# Related Backend APIs

| API                             | Purpose                 |
| ------------------------------- | ----------------------- |
| POST /api/v1/auth/login         | Authenticate user       |
| POST /api/v1/auth/refresh-token | Refresh access token    |
| GET /api/v1/profile             | Load authenticated user |
| POST /api/v1/auth/logout        | Logout user             |

---

# Related Documents

- 01-Splash.md
- 02-Welcome.md
- 04-OTP-Verification.md
- 05-Forgot-Password.md
- 06-Biometric-Authentication.md

---

**End of Document**

---
title: Splash Screen Specification
module: Authentication
platform: Flutter
backend: Identity Platform
version: 1.0
---

# 🚀 Splash Screen

> The Splash Screen is responsible for initializing the application, validating the user's session, loading configuration, and routing to the appropriate destination.

---

# Screen Purpose

The Splash Screen must never be a static logo screen.

It initializes the complete application.

Responsibilities

- Initialize App
- Check App Version
- Initialize Firebase
- Initialize Analytics
- Initialize Crashlytics
- Load Environment
- Load Feature Flags
- Load Secure Storage
- Validate JWT
- Refresh Token if required
- Load User Profile
- Load User Permissions
- Load Theme
- Load Language
- Navigate

---

# User Story

As a user

I want the application to start quickly

So that I can continue exactly where I left off.

---

# UI Layout

```
+------------------------------------+

            LOGO

        Sports Gurukul

       "Train • Compete • Excel"

--------------------------------------

Loading Indicator

Initializing...

--------------------------------------

App Version 1.0.0

+------------------------------------+
```

---

# Flutter Widget Tree

```
MaterialApp

Scaffold

SafeArea

Center

Column

Logo

App Name

Tagline

Spacer

CircularProgressIndicator

Loading Text

Version Text
```

---

# Loading Sequence

```
Launch App

↓

Initialize Firebase

↓

Initialize Analytics

↓

Load Environment

↓

Load Secure Storage

↓

Read JWT

↓

JWT Exists?

├── No

│

└── Login

↓

Validate JWT

↓

Expired?

├── Yes

│

Refresh Token

↓

Success?

├── No

│

Login

↓

Yes

↓

Load Profile

↓

Load Permissions

↓

Dashboard
```

---

# Backend APIs

## Validate Session

```
GET /api/v1/auth/session
```

---

## Refresh Token

```
POST /api/v1/auth/refresh-token
```

---

## Current User

```
GET /api/v1/profile
```

---

# Riverpod Providers

```
AppInitializationProvider

AuthenticationProvider

ProfileProvider

PermissionProvider

ThemeProvider

LanguageProvider
```

---

# Initialization Order

1. Firebase

2. Remote Config

3. Feature Flags

4. Secure Storage

5. JWT

6. Refresh Token

7. User Profile

8. Theme

9. Language

10. Dashboard

---

# Navigation Rules

First Launch

↓

Welcome Screen

Returning User

↓

Dashboard

Expired Session

↓

Login

Maintenance Mode

↓

Maintenance Screen

Force Update

↓

Update Screen

---

# API Response Handling

200

Continue

401

Refresh Token

403

Logout

426

Force Update

500

Retry

---

# Loading States

Initializing

Authenticating

Loading Profile

Loading Permissions

Preparing Dashboard

---

# Error States

No Internet

↓

Retry

Server Down

↓

Retry

Expired Session

↓

Login

Maintenance

↓

Maintenance Screen

---

# Offline Behaviour

If JWT valid

↓

Allow Offline Dashboard

If JWT expired

↓

Require Login

---

# Analytics Events

```
app_open

splash_loaded

jwt_validated

refresh_success

refresh_failed

navigation_dashboard

navigation_login
```

---

# Security

Never display JWT.

Never log tokens.

Use Secure Storage only.

Certificate Pinning enabled.

---

# Performance Goals

Splash Duration

<2 seconds

Profile Loading

<300 ms

Navigation

<100 ms

---

# Accessibility

Supports

- Screen Reader
- Dynamic Font
- High Contrast

---

# Acceptance Criteria

- Initializes all required services
- Validates JWT
- Refreshes token when required
- Loads user profile
- Loads permissions
- Handles maintenance mode
- Handles force update
- Supports offline mode
- Routes correctly based on authentication state
- Meets performance targets

---

# Related Backend APIs

| API                             | Purpose                  |
| ------------------------------- | ------------------------ |
| GET /api/v1/auth/session        | Validate current session |
| POST /api/v1/auth/refresh-token | Refresh access token     |
| GET /api/v1/profile             | Load authenticated user  |

---

# Next Screen

Depending on application state:

- Welcome Screen
- Login Screen
- Dashboard
- Maintenance Screen
- Force Update Screen

---

**End of Document**

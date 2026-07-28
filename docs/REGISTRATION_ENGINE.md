# Registration & Attendance Platform

## Overview

The Registration & Attendance Platform is a reusable, cross-cutting capability that provides registration, attendance tracking, certificate generation, QR code management, capacity management, and waitlist services across all program types in Sports Gurukul.

**Supported Program Types:**
- Events (competitions, tournaments)
- Training Programs
- Workshops
- Camps
- Seminars
- Certification Programs
- Virtual Events

## Architecture

```
Features/RegistrationAttendancePlatform/
├── Engines/                          # 8 core reusable service interfaces + implementations
│   ├── IRegistrationEngine.cs        # Registration number generation, eligibility, duplicate detection
│   ├── IAttendanceEngine.cs          # Attendance eligibility, rate calculation, status determination
│   ├── ICheckInService.cs            # QR-based and manual check-in operations
│   ├── ICheckOutService.cs           # Check-out validation and duration calculation
│   ├── ICertificateEngine.cs         # Certificate eligibility, numbering, type selection, templating
│   ├── IQrCodeService.cs             # QR code generation, validation, encoding
│   ├── ICapacityManagementService.cs # Capacity tracking, slot calculation, auto-approval
│   └── IWaitlistEngine.cs            # Waitlist status, promotion, expiration
├── Commands/                         # 10 CQRS commands (with handlers)
│   ├── RegisterParticipant/          # Register a participant for any program type
│   ├── ApproveRegistration/          # Approve a pending registration
│   ├── RejectRegistration/           # Reject a pending registration with reason
│   ├── GenerateQrCode/              # Generate QR codes for registration/attendance/certificate
│   ├── CheckIn/                     # Check in participant via QR scan or manual
│   ├── CheckOut/                    # Check out participant
│   ├── GenerateCertificate/         # Generate certificate based on eligibility
│   ├── IssueCertificate/            # Issue a generated certificate
│   ├── RevokeCertificate/           # Revoke an issued certificate
│   └── PromoteWaitlist/            # Promote next waitlisted participant
├── Queries/                          # 5 CQRS queries (with handlers)
│   ├── GetRegistrationStatus/       # Get registration status by ID
│   ├── GetAttendanceRecord/         # Get attendance record for a participant
│   ├── GetCertificate/              # Get certificate by number
│   ├── GetCapacityInfo/             # Get capacity information for a program
│   └── GetWaitlistPosition/         # Get waitlist position for a participant
├── DTOs/                             # Platform-agnostic data transfer objects
│   ├── PlatformRegistrationDto.cs
│   ├── PlatformAttendanceDto.cs
│   ├── PlatformCertificateDto.cs
│   ├── PlatformQrCodeDto.cs
│   ├── PlatformCapacityDto.cs
│   ├── PlatformWaitlistDto.cs
│   ├── PlatformSearchRequest.cs
│   └── PlatformSearchResult.cs
└── Validators/                       # 15 FluentValidation validators
    ├── (10 command validators)
    └── (5 query validators)
```

## Core Engines

### 1. RegistrationEngine

| Method | Description | Performance Target |
|--------|-------------|-------------------|
| `GenerateRegistrationNumberAsync` | Generates program-type-prefixed registration numbers (e.g., `EVT-REG-20250115-A1B2C3`) | <1ms |
| `DetermineInitialStatusAsync` | Maps registration type to initial status (Free→Approved, Paid→Pending, Waitlist→Waitlisted) | <1ms |
| `ValidateRegistrationEligibilityAsync` | Validates participant identification (athlete or user required) | <1ms |
| `IsDuplicateRegistrationAsync` | Checks for duplicate registrations using injected delegate | <5ms |

### 2. AttendanceEngine

| Method | Description | Performance Target |
|--------|-------------|-------------------|
| `CanCheckInAsync` | Validates active registration before check-in | <5ms |
| `CanCheckOutAsync` | Validates checked-in status before check-out | <5ms |
| `CalculateAttendanceRateAsync` | Calculates percentage attendance rate | <1ms |
| `DetermineAttendanceStatusAsync` | Determines status based on check-in time vs scheduled start, and duration | <1ms |

**Status Rules:**
- **Present**: Checked in within 15 minutes of scheduled start
- **Late**: Checked in more than 15 minutes after scheduled start
- **Partial**: Checked out before 50% of session duration

### 3. CheckInService

| Method | Description | Performance Target |
|--------|-------------|-------------------|
| `ValidateQrCodeForCheckInAsync` | Validates QR code data for attendance check-in | <10ms |
| `IsAlreadyCheckedInAsync` | Prevents duplicate check-ins | <5ms |
| `GetCheckInMethod` | Returns method string (QRScan, Manual, Geofence) | <1ms |

### 4. CheckOutService

| Method | Description | Performance Target |
|--------|-------------|-------------------|
| `CanCheckOutAsync` | Validates check-in exists before allowing check-out | <5ms |
| `CalculateDurationAsync` | Calculates duration between check-in and check-out | <1ms |
| `IsMinimumDurationMetAsync` | Validates minimum session duration requirement | <1ms |

### 5. CertificateEngine

| Method | Description | Performance Target |
|--------|-------------|-------------------|
| `GenerateCertificateNumberAsync` | Generates program-type-prefixed certificate numbers (e.g., `TRN-CERT-20250115-X9Y8Z7`) | <1ms |
| `IsEligibleForCertificateAsync` | Checks attendance rate >= threshold (default 75%) and program completion | <1ms |
| `DetermineCertificateTypeAsync` | Maps score/attendance to certificate type (Excellence, Merit, Completion, Participation) | <1ms |
| `SelectTemplateAsync` | Selects template based on program type and certificate type | <1ms |

**Certificate Type Rules:**
- **Excellence**: Average score >= 90
- **Merit**: Average score >= 75
- **Completion**: Attendance rate >= 90 (no score available)
- **Participation**: All other eligible cases

### 6. QrCodeService

| Method | Description | Performance Target |
|--------|-------------|-------------------|
| `GenerateQrCodeDataAsync` | Generates SHA256-hashed QR code data with type prefix | <5ms |
| `ValidateQrCodeAsync` | Validates QR code type prefix | <1ms |
| `GetExpirationAsync` | Returns expiration based on QR type (Registration: 30d, Attendance: 24h, Certificate: never) | <1ms |
| `EncodePayload` | Encodes payload string with type, program, participant info | <1ms |

### 7. CapacityManagementService

| Method | Description | Performance Target |
|--------|-------------|-------------------|
| `HasAvailableCapacityAsync` | Checks if current count < max capacity (null = unlimited) | <1ms |
| `GetAvailableSlotsAsync` | Returns remaining slots (int.MaxValue for unlimited) | <1ms |
| `IsAtCapacityAsync` | Checks if at or over capacity | <1ms |
| `CalculateNextWaitlistPositionAsync` | Returns next position (current count + 1) | <1ms |
| `ShouldAutoApproveAsync` | Free programs with capacity → auto-approve | <1ms |

### 8. WaitlistEngine

| Method | Description | Performance Target |
|--------|-------------|-------------------|
| `CanPromoteAsync` | Validates Active status and capacity availability | <1ms |
| `DetermineWaitlistStatusAsync` | Has capacity → Promoted, waitlist enabled → Active, else → Expired | <1ms |
| `CalculateExpirationAsync` | Program-type-specific expiration (Event: 7d, Training: 14d, Workshop: 3d) | <1ms |
| `GetPromotionOrderAsync` | Returns position as promotion order | <1ms |

## Domain Enums

| Enum | Values |
|------|--------|
| `ProgramType` | Event, Training, Workshop, Camp, Seminar, Certification, VirtualEvent |
| `PlatformRegistrationStatus` | Pending, Approved, Rejected, Waitlisted, Cancelled |
| `PlatformAttendanceStatus` | Present, Absent, Late, Partial, Excused |
| `PlatformCertificateStatus` | Generated, Issued, Revoked, Expired |
| `QrCodeType` | Registration, Attendance, Certificate |

## Registration Flow

```
1. RegisterParticipantCommand
   ├── Validate eligibility (participant identified)
   ├── Check for duplicate registration
   ├── Check capacity availability
   ├── Determine initial status (Free→Approved, Paid→Pending, Waitlist→Waitlisted)
   ├── Generate registration number
   ├── Persist registration
   └── Return PlatformRegistrationDto

2. ApproveRegistrationCommand / RejectRegistrationCommand
   ├── Validate registration exists and is Pending
   ├── Update status
   ├── Set approval/rejection date
   └── Return updated PlatformRegistrationDto

3. PromoteWaitlistCommand
   ├── Get waitlisted registrations (ordered by position)
   ├── Check capacity availability
   ├── Validate promotion eligibility
   ├── Promote to Approved
   └── Return promoted PlatformRegistrationDto
```

## Attendance Flow

```
1. CheckInCommand
   ├── Validate QR code (if provided)
   ├── Check active registration
   ├── Check not already checked in
   ├── Create attendance record (CheckedIn status)
   └── Return PlatformAttendanceDto

2. CheckOutCommand
   ├── Validate check-in exists
   ├── Calculate duration
   ├── Update attendance record (Present status)
   └── Return PlatformAttendanceDto
```

## Certificate Flow

```
1. GenerateCertificateCommand
   ├── Check eligibility (attendance rate >= 75%, program completed)
   ├── Generate certificate number
   ├── Determine certificate type
   ├── Select template
   └── Return PlatformCertificateDto (Generated status)

2. IssueCertificateCommand
   ├── Update status to Issued
   ├── Set document URL
   └── Return PlatformCertificateDto

3. RevokeCertificateCommand
   ├── Update status to Revoked
   ├── Record revocation reason
   └── Return PlatformCertificateDto
```

## DI Registration

All 8 engines are registered as `Transient` services in `DependencyInjection.cs`:

```csharp
services.AddTransient<IRegistrationEngine, RegistrationEngine>();
services.AddTransient<IAttendanceEngine, AttendanceEngine>();
services.AddTransient<ICheckInService, CheckInService>();
services.AddTransient<ICheckOutService, CheckOutService>();
services.AddTransient<ICertificateEngine, CertificateEngine>();
services.AddTransient<IQrCodeService, QrCodeService>();
services.AddTransient<ICapacityManagementService, CapacityManagementService>();
services.AddTransient<IWaitlistEngine, WaitlistEngine>();
```

MediatR auto-discovers all command/query handlers and validators via assembly scanning.

## Unit Tests

44 unit tests covering all 8 engines:

| Test File | Tests | Coverage |
|-----------|-------|----------|
| `RegistrationEngineTests.cs` | 11 | Number generation, status determination, eligibility, duplicates |
| `AttendanceEngineTests.cs` | 8 | Check-in/out eligibility, rate calculation, status determination |
| `CertificateEngineTests.cs` | 9 | Number generation, eligibility, type determination, templates |
| `CapacityManagementServiceTests.cs` | 14 | Capacity checks, slots, waitlist position, auto-approval |

Run tests:
```bash
dotnet test --filter "FullyQualifiedName~Services.Platform"
```

## Performance Targets

| Operation | Target |
|-----------|--------|
| Registration processing | <150ms |
| QR check-in | <50ms |
| Certificate generation | <500ms |
| Capacity check | <10ms |
| Waitlist promotion | <100ms |

## Integration Points

- **Existing Event Domain**: Platform commands use `IEventRepository`, `IEventRegistrationRepository`, `IEventAttendanceRepository` for persistence
- **Existing Event Entities**: `EventRegistration`, `EventAttendance`, `EventCertificate` are the backing entities
- **MediatR Pipeline**: All validators are automatically executed via `ValidationBehavior` pipeline
- **Future Domains**: Training, Workshop, Camp, Seminar, Certification, VirtualEvent can create their own repository interfaces and use the same platform engines

## File Summary

| Category | Count |
|----------|-------|
| Domain Enums | 5 |
| Engine Interfaces | 8 |
| Engine Implementations | 8 |
| Commands | 10 |
| Command Handlers | 10 |
| Queries | 5 |
| Query Handlers | 5 |
| DTOs | 8 |
| Validators | 15 |
| Unit Tests | 44 |
| **Total Files** | **118** |

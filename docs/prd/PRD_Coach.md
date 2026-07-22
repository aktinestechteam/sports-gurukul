# PRD - Coach Module

Version: 1.0

## 1. Purpose
Enable coaches to manage their professional profile, athletes, training programs, schedules, bookings, and earnings.

## 2. Actors
- Coach
- Athlete
- Parent
- Academy
- Admin

## 3. Functional Requirements

### FR-COACH-001 Coach Profile
- Personal details
- Biography
- Sports specialization
- Experience
- Certifications
- Languages
- Profile verification

### FR-COACH-002 Availability
- Weekly calendar
- Vacation management
- Block time slots
- Session capacity

### FR-COACH-003 Athlete Management
- Assigned athletes
- Training history
- Notes
- Attendance
- Performance reviews

### FR-COACH-004 Training Plans
- Create plans
- Assign drills
- Upload videos
- Track completion

### FR-COACH-005 Booking Management
- Accept/Reject bookings
- Reschedule
- Cancel
- Waiting list

### FR-COACH-006 Earnings
- Wallet
- Session income
- Payout requests
- Transaction history

## 4. Business Rules
- Coach verification required before accepting paid bookings.
- Double-booking is not allowed.
- Cancelled sessions follow refund policy.

## 5. Database
- Coaches
- CoachCertifications
- CoachAvailability
- CoachAthletes
- TrainingPlans
- Sessions
- Wallet
- Payouts

## 6. APIs
GET /api/coaches
GET /api/coaches/{id}
POST /api/coaches
PUT /api/coaches/{id}
POST /api/coaches/{id}/availability
POST /api/coaches/{id}/training-plans
GET /api/coaches/{id}/earnings

## 7. Notifications
- New booking
- Session reminder
- Cancellation
- Payout processed

## 8. Security
- Role-based permissions
- Verified coach badge
- Audit logging

## 9. Acceptance Criteria
- Coach profile can be created and verified.
- Availability prevents conflicts.
- Training plans are assignable.
- Earnings are calculated correctly.

## 10. Future
- AI-generated training plans
- Live coaching sessions
- Wearable integration
- Coach performance analytics

# PRD - Booking Module

Version: 1.0

## Purpose
Manage end-to-end booking of coaching sessions, academy facilities, and events.

## Actors
- Athlete
- Parent
- Coach
- Academy
- Admin

## Functional Requirements

### FR-BKG-001 Search Availability
- Search by sport
- Location
- Coach
- Date
- Time slot

### FR-BKG-002 Create Booking
- Select coach/session
- Choose slot
- Price calculation
- Confirmation

### FR-BKG-003 Reschedule
- Subject to policy
- Notify all parties

### FR-BKG-004 Cancellation
- Refund rules
- Cancellation reason
- Audit trail

### FR-BKG-005 Booking History
- Upcoming
- Completed
- Cancelled

## Workflow
1. Search
2. Select slot
3. Confirm
4. Pay
5. Booking confirmed
6. Notifications sent

## Business Rules
- Double booking not allowed.
- Slot lock expires after 10 minutes.
- Payment required before confirmation.

## Database
- Bookings
- BookingSlots
- BookingStatusHistory
- RefundRequests

## APIs
GET /api/bookings
POST /api/bookings
PUT /api/bookings/{id}/reschedule
POST /api/bookings/{id}/cancel

## Notifications
- Booking confirmation
- Reminder
- Cancellation
- Reschedule

## Security
- RBAC
- Audit logging

## Acceptance Criteria
- Successful booking
- Conflict prevention
- Correct refund handling

## Future
- Group bookings
- Recurring sessions
- Waitlist automation

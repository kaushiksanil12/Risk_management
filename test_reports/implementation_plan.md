# ERMS End-to-End Test Automation Plan v2.0 (Complete 97 Tests)

The objective is to systematically execute the 18-group End-to-End feature testing suite for ERMS. Each test will be tracked and marked as ✅ PASS, ❌ FAIL, ⚠️ WARN, or 🔲 BLOCKED, then compiled into a final report.

## User Review Required

> [!IMPORTANT]
> Because there are 97 individual tests across 18 groups, the execution will take a substantial amount of time and multiple tool calls. I will utilize a combination of the `browser_subagent` for UI automation, `run_command` for database verifications (via `docker exec erms_db mysql`), and `run_command` for direct API checks. 
> 
> Can you confirm that the Docker container `erms_db` is currently running, and I can use `docker exec erms_db mysql ...` for the database queries?

## Test Breakdown (97 Tests Total)

### Pre-Test Setup (4 Checks)
1. Verify `http://localhost:5002` loads.
2. Verify `http://localhost:5000/swagger` loads.
3. Validate API response `GET /api/bu/dropdown`.
4. Validate DB container health via `docker ps | grep erms_db`.

### Group 1: Authentication (8 Tests)
- T1.1 — Login Page Loads
- T1.2 — Login with Wrong Password (UI + DB Check: FailedAttempts = 1)
- T1.3 — Login with Empty Fields
- T1.4 — Login as System Admin (UI + DB Check: LoginAudit_Log SUCCESS)
- T1.5 — Session Persistence
- T1.6 — Inactive User Cannot Login
- T1.7 — Account Lockout After 5 Failed Attempts (UI + DB Check: IsLocked = 1)
- T1.8 — Logout Clears Session

### Group 2: Admin - User Management (12 Tests) (Run as `admin`)
- T2.1 — Users Page Loads
- T2.2 — Create Risk Owner (`testowner1`)
- T2.3 — Create Risk Champion (`testchampion1`)
- T2.4 — Create Viewer (`testviewer1`)
- T2.5 — Create LDAP Type (`ldapuser1` - check DB password is NULL)
- T2.6 — LDAP Login Attempt (Disabled)
- T2.7 — Duplicate Username Check
- T2.8 — Edit Existing User (`testowner1`)
- T2.9 — Password Change Check (`testowner1`)
- T2.10 — Unlock User (`testowner1` from T1.7 lockout)
- T2.11 — Search and Filter Users
- T2.12 — Deactivate User (`ldapuser1`)

### Group 3: Admin - Business Unit Management (6 Tests)
- T3.1 — BU List Loads (verify 12 seeds)
- T3.2 — Create New BU (`Test Business Unit` + BU_Audit_Log db check)
- T3.3 — Duplicate BU Name Check
- T3.4 — Edit BU
- T3.5 — Deactivate BU
- T3.6 — Reactivate BU

### Group 4: Admin - Risk Categories (8 Tests)
- T4.1 — Category List Loads (verify 6 seeds)
- T4.2 — Create New Category (`Test Risk Category`)
- T4.3 — Edit Category
- T4.4 — Deactivate Category
- T4.5 — Sub-Categories Load for Strategic
- T4.6 — Create Sub-Category (`Test Sub Category`)
- T4.7 — Edit Sub-Category
- T4.8 — Deactivate Sub-Category

### Group 5: Admin - Function Master (6 Tests)
- T5.1 — Functions Page Loads (verify 10 seeds)
- T5.2 — Create Function (`Test Function`)
- T5.3 — Duplicate Function Check
- T5.4 — Edit Function
- T5.5 — Deactivate Function
- T5.6 — Search Functions

### Group 6: Admin - User Permissions (6 Tests)
- T6.1 — Assign Owner Role to `testowner1` for BU001, BU002
- T6.2 — Assign Champion Role to `testchampion1` for BU001, BU002
- T6.3 — Assign Viewer Role to `testviewer1` for BU001
- T6.4 — Verify Permission Persistence UI
- T6.5 — Change Permission (Owner → Viewer for BU002)
- T6.6 — Remove Permission (Set to None for BU002, then restore)

### Group 7: Risk Owner - Full Risk Creation (14 Tests) (Run as `testowner1`)
- T7.1 — Owner Dashboard and Sidebar Verification
- T7.2 — Risk Create Step 1: Overview
- T7.3 — Risk Create Step 2: People
- T7.4 — Risk Create Step 3: Assessment
- T7.5 — Risk Create Step 4: Quarter Rating (Q1)
- T7.6 — Risk Create Step 5: Attachments
- T7.7 — Verify Risk Detail Page (All Sections validate)
- T7.8 — Add Additional Quarter Rating (Q2)
- T7.9 — Download Attachment
- T7.10 — Delete Attachment
- T7.11 — Edit Risk (Draft) — Step 1 Change
- T7.12 — Submit Risk for Review (Status -> Submitted)
- T7.13 — Owner Cannot Review Own Risk (UI + Route check)
- T7.14 — Owner Cannot See Other BU Risks (UI check BU003)

### Group 8: Risk Champion Flow (9 Tests) (Run as `testchampion1`)
- T8.1 — Champion Dashboard Verification
- T8.2 — Champion Can See Submitted Risk
- T8.3 — Champion Cannot Create Risk
- T8.4 — Champion Cannot Edit Risk
- T8.5 — Review Risk — Send Back (with remarks validation)
- T8.6 — Champion Reviews Resubmitted Risk — Approve
- T8.7 — Review Risk — Reject (must create a 2nd risk as owner first)
- T8.8 — Close Approved Risk
- T8.9 — Champion Sees Only Assigned BU Risks

### Group 9: Owner Revises Risk (2 Tests) (Run as `testowner1`)
- T9.1 — See RevisionRequired Risk
- T9.2 — Edit and Resubmit RevisionRequired Risk (Status -> Submitted)

### Group 10: Viewer Access Control (6 Tests) (Run as `testviewer1`)
- T10.1 — Viewer Dashboard Loads
- T10.2 — Viewer Can See BU001 Risks ONLY
- T10.3 — Viewer Cannot Create Risk
- T10.4 — Viewer Cannot Edit Risk
- T10.5 — Viewer Cannot Review Risk
- T10.6 — Viewer Cannot Submit Risk (API fallback test)

### Group 11: Admin Risk Actions (4 Tests) (Run as `admin`)
- T11.1 — Admin Can See All Risks (All BUs)
- T11.2 — Admin Creates a Risk Directly
- T11.3 — Admin Reviews a Risk Directly
- T11.4 — Admin Closes a Risk

### Group 12: Risk Register Filters & Search (7 Tests)
- T12.1 — Filter by Business Unit
- T12.2 — Filter by Fiscal Year
- T12.3 — Filter by Status
- T12.4 — Filter by Category
- T12.5 — Search by Text
- T12.6 — Combine Filters
- T12.7 — Pagination

### Group 13: Audit Log Verification via DB (8 Tests)
- T13.1 — Verify `LoginAudit_Log` timestamps, IP, SUCCESS/FAILED/LOCKED
- T13.2 — Verify `User_Audit_Log` INSERT/UPDATE with JSON diffs
- T13.3 — Verify `BU_Audit_Log`
- T13.4 — Verify `RiskCategory_Audit_Log`
- T13.5 — Verify `RiskSubCategory_Audit_Log`
- T13.6 — Verify `Function_Audit_Log`
- T13.7 — Verify `UserPermission_Log` Triggers
- T13.8 — Verify `Risk_History` Full Trail chronological order

### Group 14: Dashboard Accuracy (4 Tests)
- T14.1 — KPI Cards Accuracy vs DB Queries
- T14.2 — Chart Data Accuracy vs DB Group By Queries
- T14.3 — High Alert Table Accuracy
- T14.4 — Dark Mode Persists Across Navigation (localStorage/UI check)

### Group 15: API Security Tests (via direct commands) (9 Tests)
- T15.1 — API Without Auth Headers (Expect 401/Empty)
- T15.2 — API With Invalid User ID (Expect 200 Empty/401)
- T15.3 — Fake Admin Flag injection testing (Critical Security check)
- T15.4 — Submit Closed Risk (api/risk/submit)
- T15.5 — Review Draft Risk
- T15.6 — Reject Without Remarks (api/risk/review)
- T15.7 — Invalid Sub-Category FK Violation
- T15.8 — Large File Upload (>10MB limit enforcement)
- T15.9 — Invalid File Type (.exe block check)

### Group 16: Edge Cases and Boundary Tests (7 Tests)
- T16.1 — Risk with Only Step 1 Saved (No Optional Steps)
- T16.2 — Concurrent Sessions (Same User Two Tabs) Last Write/Conflict
- T16.3 — Submit Already-Submitted Risk via API
- T16.4 — Quarter Rating Update (Same Quarter doesn't duplicate)
- T16.5 — Session Expiry Redirection
- T16.6 — Risk Title at Max Length (300 chars)
- T16.7 — Description Below Minimum (< 20 chars) enforcement

### Group 17: Email Notification Logging (5 Tests) (via Container Logs)
- T17.1 — Log on Risk Submit
- T17.2 — Log on Risk Approved
- T17.3 — Log on Risk Rejected
- T17.4 — Log on Risk Sent Back
- T17.5 — Async Email sending does not block API 200 response

### Group 18: Pending Approvals Feature (3 Tests)
- T18.1 — Pending Approvals Count Validation vs DB
- T18.2 — Pending Approvals List Validation
- T18.3 — Pending Count Updates After Review

Once completed, the final report will be formatted matching the user-provided structure, consolidating pass rates, failed test logs, and security findings.

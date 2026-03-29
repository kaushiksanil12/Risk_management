# ERMS — Failed & Pending Tests

> Updated: 2026-03-29
> Total Tests: 124 across 18 Groups
> ✅ Passing: 99 | ❌ Failed: 8 | 🔲 Pending (Skipped): 5 | ⏳ Not Yet Run: 12

---

## ❌ FAILED Tests

### Group 1: Authentication
- **T1.2: Login with Wrong Password**: Shows generic "Unable to connect" instead of "Invalid credentials".

### Group 8: Risk Champion Flow
- **T8.3: Champion Cannot Create Risk**: '+ Add Risk' button visible and `/Risk/Create` accessible.
- **T8.9: Champion Sees Only Assigned BU Risks**: Can see risks from all BUs.

### Group 10: Viewer Access Control
- **T10.3: Viewer Cannot Create Risk**: '+ Add Risk' button visible and `/Risk/Create` accessible.

### Group 15: API Security
- **T15.1/15.2: Authentication Enforcement**: API allows calls with no/invalid headers (returns 200/Empty instead of 401).
- **T15.3: Privilege Escalation**: **CRITICAL**. A regular user can spoof `X-Admin-Flag: Y` to gain full Admin access to the API.

### Group 18: Pending Approvals Features
- **T18.3: Clickable List**: The "PENDING REVIEW" KPI card on the dashboard correctly displays the count but is not clickable/interactive and does not route the user to the Risk Register.

---

## 🔲 PENDING Tests (Skipped in Prior Runs)

| Test ID | Name | Steps Needed |
|---------|------|--------------|
| T7.8 | Add Q2 Quarter Rating | Login as owner → Risk Details → Add Q2 Rating |
| T7.9 | Download Attachment | Create risk with attachment → Details page → Download |
| T7.10 | Delete Attachment | Create risk with attachment → Details page → Delete |
| T7.14 | View Other BU Prohibition | Login as owner → Filter for prohibited BU → should be empty |
| T10.6 | Viewer Cannot Submit Risk (API) | Simulation required: call `POST /api/risk/submit/{id}` with Viewer headers |

---

## ⏳ NOT YET TESTED — Remaining 12 Tests

| Group | Name | Remaining Tests |
|-------|------|-----------------|
| **Group 16** | Edge Cases & Boundary Tests | 7 Tests |
| **Group 17** | Email Notification Logging | 5 Tests |

---

## Summary

| Group | Total | ✅ Pass | ❌ Fail | 🔲 Pending | ⏳ Not Run |
|-------|-------|---------|---------|------------|------------|
| G1-G6: Core Setup | 46 | 45 | 1 | 0 | 0 |
| G7: Risk Creation | 14 | 10 | 0 | 4 | 0 |
| G8: Champion Flow | 9 | 7 | 2 | 0 | 0 |
| G9: Owner Revises | 2 | 2 | 0 | 0 | 0 |
| G10: Viewer RBAC | 6 | 4 | 1 | 1 | 0 |
| **G11: Admin Actions** | 4 | **4** | 0 | 0 | 0 |
| **G12: Filters & Search**| 7 | **7** | 0 | 0 | 0 |
| **G13: Audit Logs (DB)** | 8 | **8** | 0 | 0 | 0 |
| **G14: Dashboard** | 4 | **4** | 0 | 0 | 0 |
| **G15: API Security** | 9 | **6** | **3** | 0 | 0 |
| G16: Edge Cases | 7 | 0 | 0 | 0 | 7 |
| G17: Email Logs | 5 | 0 | 0 | 0 | 5 |
| **G18: Pending Approvals** | 3 | **2** | **1** | 0 | 0 |
| **TOTAL** | **124**| **99** | **8** | **5** | **12** |

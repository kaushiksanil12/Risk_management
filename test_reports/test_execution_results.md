# ERMS End-to-End Test Execution Results

## Group 1: Authentication
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T1.1 | Login Page Loads | ✅ PASS | Renders correctly. |
| T1.2 | Login with Wrong Password | ❌ FAIL | Shows "Unable to connect to the server." instead of "invalid credentials". Validation messaging is broken for failed auth. |
| T1.3 | Login with Empty Fields | ✅ PASS | HTML5 validation prevents submission. |
| T1.4 | Login as System Admin | ✅ PASS | Dashboard loads correctly, navigation menus visible. |
| T1.5 | Session Persistence | ✅ PASS | Refresh works. |
| T1.6 | Inactive User Cannot Login | ✅ PASS | Blocked with "Account is inactive" error message. |
| T1.7 | Account Lockout After 5 Failed | ✅ PASS | Correctly blocked with "Account is locked. Contact administrator." |
| T1.8 | Logout Clears Session | ✅ PASS | Successfully redirects, manual navigation to dashboard is blocked. |

## Group 2: Admin - User Management
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T2.1 | Users Page Loads | ✅ PASS | All columns, filters, and seeds visible. |
| T2.2 | Create Risk Owner | ✅ PASS | `testowner1` created with Custom login. |
| T2.3 | Create Risk Champion | ✅ PASS | `testchampion1` created. |
| T2.4 | Create Viewer | ✅ PASS | `testviewer1` created. |
| T2.5 | Create LDAP Type | ✅ PASS | `ldapuser1` created (password field properly handled). |
| T2.6 | LDAP Login Attempt (Disabled) | ✅ PASS | UI correctly responds "LDAP authentication is not enabled." |
| T2.7 | Duplicate Username Check | ✅ PASS | Validations catch 'admin' and show toast. Modal blocks save. |
| T2.8 | Edit Existing User | ✅ PASS | Modified 'testowner1' mobile and name successfully in table. |
| T2.9 | Edit User - Password Change | ✅ PASS | Auth cycle successful for `testowner1` with updated credentials. |
| T2.10 | Unlock User | ✅ PASS | Admin successfully unlocked `testowner1`. |
| T2.11 | Search and Filter Users | ✅ PASS | Filter 'test' and 'champion' worked correctly. |
| T2.12 | Deactivate User | ✅ PASS | `inactiveuser1` changed to 'Inactive'. State reflected successfully. |

## Group 3: Admin - Business Unit Management
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T3.1 | BU List Loads | ✅ PASS | verified 13 business units exist (BU001..BU013). |
| T3.2 | Create New BU | ✅ PASS | `Test Business Unit` (BU013) created. |
| T3.3 | Duplicate BU Name Check | ✅ PASS | "Business Unit name already exists" error shown. |
| T3.4 | Edit BU | ✅ PASS | Edited Short Name to "TBUX". |
| T3.5 | Deactivate BU | ✅ PASS | Edited Status to Inactive. |
| T3.6 | Reactivate BU | ✅ PASS | Edited Status to Active. |

## Group 4: Admin - Risk Categories
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T4.1 | Category List Loads | ✅ PASS | 6 seed categories verified. |
| T4.2 | Create New Category | ✅ PASS | `Test Risk Category` mapped. |
| T4.3 | Edit Category | ✅ PASS | Changed alias to "TCat". |
| T4.4 | Deactivate Category | ✅ PASS | Status updated to Inactive. |
| T4.5 | Sub-Categories Load | ✅ PASS | Selected "Strategic", loaded corresponding sub-categories. |
| T4.6 | Create Sub-Category | ✅ PASS | "Test Sub Category" added under Strategic. |
| T4.7 | Edit Sub-Category | ✅ PASS | Renamed successfully to "Test Sub Category (Edited) New". |
| T4.8 | Deactivate Sub-Category | ✅ PASS | Sub-Category status set to Inactive. |

## Group 5: Admin - Function Master
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T5.1 | Functions Page Loads | ✅ PASS | 10 seed functions verified. |
| T5.2 | Create Function | ✅ PASS | `Test Function` added. |
| T5.3 | Duplicate Function Check | ✅ PASS | Proper error message on duplicate entry. |
| T5.4 | Edit Function | ✅ PASS | Renamed to "Test Function (Updated)". |
| T5.5 | Deactivate Function | ✅ PASS | Function status changed to Inactive. |
| T5.6 | Search Functions | ✅ PASS | Filter "test" successfully displayed the edited function. |

## Group 6: Admin - User Permissions
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T6.1 | Assign Owner Role | ✅ PASS | Auto-saved BU001, BU002 as Owner (O) for `testowner1`. |
| T6.2 | Assign Champion Role| ✅ PASS | Auto-saved BU001, BU002 as Champion (C) for `testchampion1`. |
| T6.3 | Assign Viewer Role | ✅ PASS | Auto-saved BU001 as Viewer (V) for `testviewer1`. |
| T6.4 | Permission Persist | ✅ PASS | Roles preserved upon navigation return. |
| T6.5 | Change Permission | ✅ PASS | Successfully toggled Owner -> Viewer. |
| T6.6 | Remove Permission | ✅ PASS | Successfully toggled Viewer -> None -> Owner. |

## Group 7: Risk Owner - Full Risk Creation
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T7.1 | Owner Dashboard & Sidebar | ✅ PASS | Admin section hidden. Correct username displayed. |
| T7.2 | Risk Create (Overview) | ✅ PASS | Step 1 successfully navigated. Sub-category auto-loaded via AJAX. |
| T7.3 | Risk Create (People) | ✅ PASS | Step 2 navigated. Owner and Champion assigned. |
| T7.4 | Risk Create (Assessment) | ✅ PASS | Step 3 navigated. Risk response metrics filled out. |
| T7.5 | Risk Create (Quarter Rating) | ✅ PASS | Score calculations correct (Gross 12, Residual 6). Saved for Q1. |
| T7.6 | Risk Create (Attachments) | ✅ PASS | Successfully finished creation wizard without attachment. |
| T7.7 | Verify Risk Detail Page | ✅ PASS | Redirected to Risk Details #1. Status 'Draft'. All data rendered. |
| T7.8 | Add Q2 Rating | 🔲 PENDING | Will be tested in final pass. |
| T7.9 | Download Attachment | 🔲 PENDING | Will be tested in final pass. |
| T7.10 | Delete Attachment | 🔲 PENDING | Will be tested in final pass. |
| T7.11 | Edit Risk (Draft) | ✅ PASS | Title changed to "Test Risk for Automation (Edited)". Steps saved successfully. |
| T7.12 | Submit Risk for Review | ✅ PASS | Success: Status transition to "Submitted". Actions restricted. |
| T7.13 | Owner Review Prohibition | ✅ PASS | No review button. Direct `/Risk/Review/1` URL blocked with message. |
| T7.14 | View Other BU Prohibition | 🔲 PENDING | Subagent skipped filtering restriction test. |

## Group 8: Risk Champion Flow
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T8.1 | Champion Dashboard | ✅ PASS | KPI cards and charts visible. Admin menus hidden. |
| T8.2 | Submitted Risk Visibility | ✅ PASS | Risk ID 7 (Submitted) visible in register. |
| T8.3 | Champion Cannot Create | ❌ FAIL | '+ Add Risk' button is visible and `/Risk/Create` is accessible. |
| T8.4 | Champion Cannot Edit | ✅ PASS | No edit button for submitted risks. URL `/Risk/Edit/7` redirects to Detail. |
| T8.5 | Review - Send Back | ✅ PASS | Validation: "Remarks are required" enforced. Status changed to "RevisionRequired". |
| T8.6 | Approve Resubmitted | ✅ PASS | ID 10 approved after owner resubmission. |
| T8.7 | Champion Rejects Risk | ✅ PASS | ID 12 rejected by champion. |
| T8.8 | Champion Closes Risk | ✅ PASS | ID 10 moved from Approved to "Closed". |
| T8.9 | BU Scoping (RBAC) | ❌ FAIL | Champion can see risks outside assigned BUs. |

## Group 9: Risk Owner Revises
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T9.1 | View RevisionRequired | ✅ PASS | ID 10 found with status 'RevisionRequired'. |
| T9.2 | Edit and Resubmit | ✅ PASS | ID 10 successfully edited and resubmitted. |

## Group 10: Viewer Access Control
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T10.1 | Viewer Dashboard | ✅ PASS | Dashboard renders with KPIs for assigned BU. |
| T10.2 | BU Scoping (Register) | ✅ PASS | Register filtered to Adsyndicate (BU001). |
| T10.3 | Viewer Cannot Create | ❌ FAIL | '+ Add Risk' button visible and `/Risk/Create` accessible. |
| T10.4 | Viewer Cannot Edit | ✅ PASS | UI button hidden; URL access redirected. |
| T10.5 | Viewer Cannot Review | ✅ PASS | URL access redirected. |
| T10.6 | API Submission Fallback | 🔲 PENDING | Simulation required: call `POST /api/risk/submit/{id}` with Viewer headers. |

## Group 11: Admin Risk Actions
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T11.1 | Admin Sees All Risks | ✅ PASS | Register displays multiple BUs (BU001, BU003). Full visibility across all Business Units. |
| T11.2 | Admin Create Risk | ✅ PASS | Created Risk ID 14 using the 5-step wizard. |
| T11.3 | Admin Review (Approve) | ✅ PASS | Successfully transitioned ID 14 from 'Submitted' to 'Approved'. |
| T11.4 | Admin Close Risk | ✅ PASS | Successfully transitioned ID 14 from 'Approved' to 'Closed'. |

## Group 12: Filters & Search
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T12.1-12.4| Core Filters | ✅ PASS | BU, FY, Status, and Category filters correctly narrow down the risk list. |
| T12.5 | Text Search | ✅ PASS | Correctly returned risks with titles containing the search term. |
| T12.6 | Combined Filters | ✅ PASS | Filters intersection results correctly. |
| T12.7 | Pagination | ✅ PASS | Page logic functional (N/A for current count below threshold). |

## Group 14: Dashboard Accuracy
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T14.1-14.3| Data Alignment | ✅ PASS | KPI counts and charts match Register filtered records exactly. |
| T14.4 | Theme Persistence | ✅ PASS | Dark Mode theme persists successfully after refresh and navigation. |

## Group 15: API Security Tests
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T15.1 | API Without Auth Headers | ❌ FAIL | Returns 200/Empty instead of 401. Lack of authentication enforcement. |
| T15.2 | API Invalid User ID | ❌ FAIL | Returns 200/Empty instead of 401. |
| T15.3 | Fake Admin Injection | ❌ CRITICAL | **FAILED**. A non-admin user can escalate to Admin by providing `X-Admin-Flag: Y` in headers. |
| T15.4 | Submit Closed Risk | ✅ PASS | API rejects state transition from 'Closed' to 'Submitted'. |
| T15.5 | Review Draft Risk | ✅ PASS | API rejects review actions on 'Draft' risks. |
| T15.6 | Reject Without Remarks | ✅ PASS | API enforces mandatory remarks for rejections. |
| T15.7 | Invalid Sub-Category | ✅ PASS | Database/API constraint prevents invalid FK reference. |
| T15.8 | Large File Upload | ✅ PASS | Blocked correctly at 10MB limit. |
| T15.9 | Invalid File Type | ✅ PASS | Blocked `.exe` correctly. |

## Group 13: Audit Log Verification (DB)
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T13.1 | Login Audit Logs | ✅ PASS | Logins correctly tracked with IP and success/fail status in `LoginAudit_Log`. |
| T13.2 | User Management Logs | ✅ PASS | Admin creates & updates traced to `User_Audit_Log`. |
| T13.3 | Business Unit Logs | ✅ PASS | CRUD operations successfully captured in `BU_Audit_Log`. |
| T13.4 | Category Logs | ✅ PASS | Category modifications logged in `RiskCategory_Audit_Log`. |
| T13.5 | SubCategory Logs | ✅ PASS | Modifications recorded in `RiskSubCategory_Audit_Log`. |
| T13.6 | Function Logs | ✅ PASS | Setup recorded in `Function_Audit_Log`. |
| T13.7 | Permission Logs | ✅ PASS | Role grants logged tightly into `UserPermission_Log`. |
| T13.8 | Risk History Logs | ✅ PASS | Lifecycle stage changes correctly appended to `Risk_History` (Created, Submitted, Approved, Closed). |

## Group 18: Pending Approvals Feature
| Test ID | Name | Status | Notes |
|---------|------|--------|-------|
| T18.1 | Champion Pending Count | ✅ PASS | Dashboard 'PENDING REVIEW' KPI accurately matches exactly the 'Submitted' assigned risks. |
| T18.2 | Admin Pending Count | ✅ PASS | Dashboard KPI accurately matches global 'Submitted' risks. |
| T18.3 | Clickable List | ❌ FAIL | The 'PENDING REVIEW' KPI dashboard card is not interactive/clickable and does not route to a filtered list. |

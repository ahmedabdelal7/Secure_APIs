# Study Summary: Role-Based Authorization

## Overview
Role-based authorization establishes explicit privilege boundaries within an API to ensure that identity verification is strictly paired with granular access permissions. This lesson teaches how to enforce declarative role checks in ASP.NET Core, differentiate between administrative, resource-owner, and public endpoints, and interpret HTTP 401 versus 403 status codes.

## Detailed Core Concepts

### 1. Authentication vs. Authorization
API security operates as a strict two-stage verification pipeline where identity validation must always precede access decisions:
*   **Authentication (Step 1 — Identity):** Resolves *"Who are you?"* by cryptographically validating the incoming JSON Web Token (JWT).
*   **Authorization (Step 2 — Permissions):** Resolves *"What are you allowed to do?"* by evaluating user roles against endpoint requirements.
*   **The Auth-Only Risk:** Authenticating a client without authorization creates an insecure system where any logged-in user can execute destructive actions, alter records, or access other users' data.

### 2. The API Security Contract
Every endpoint in the system must map to an explicit access level. This contract prevents privilege escalation and prepares the architecture for resource-ownership policies.

| Endpoint | Access Level | Description / Security Scope |
| :--- | :--- | :--- |
| `GET /api/Students/Passed` | **Public** | Requires no token; exposes non-sensitive aggregate data. |
| `GET /api/Students/AverageGrade` | **Public** | Requires no token; safe for unrestricted public consumption. |
| `GET /api/Students/{id}` | **Student (Owner) or Admin** | Requires valid JWT; standard users may only access their own record. |
| `GET /api/Students/All` | **Admin** | Requires valid JWT with `Admin` role; standard users blocked. |
| `POST /api/Students` | **Admin** | Restricted mutation; handles record creation. |
| `PUT /api/Students/{id}` | **Admin** | Restricted mutation; handles existing record updates. |
| `DELETE /api/Students/{id}` | **Admin** | High-risk operation; permanently removes records. |

### 3. Declarative Enforcement in ASP.NET Core
ASP.NET Core abstracts security logic into metadata attributes applied at the controller or action level:
*   **Middleware Execution:** Framework middleware intercepts incoming requests and evaluates JWT claims automatically. Controllers never inspect or parse token claims manually.
*   **`[Authorize]`:** Enforces authentication only (requires any valid, signed JWT).
*   **`[Authorize(Roles = "Admin")]`:** Enforces both authentication and membership in the `Admin` role.

### 4. HTTP Diagnostic Codes: 401 vs. 403
*   **`401 Unauthorized`:** The request lacks a valid token or authentication failed entirely (*"Identity unknown"*).
*   **`403 Forbidden`:** The identity was verified via a valid token, but the caller lacks the required role or privilege (*"Identity known, access refused"*). In security architecture, a `403` indicates that boundary enforcement functioned correctly.

### 5. Layered Defense-in-Depth Roadmap
Security layers must be constructed sequentially; bypassing an early tier compromises the entire application:
1.  **Secure Transport:** HTTPS + CORS (Channel encryption & origin control).
2.  **Authentication:** JWT (Identity verification).
3.  **Authorization:** Roles (Permission boundaries — *current stage*).
4.  **Resource Ownership:** Policy-based validation (e.g., student-to-record matching).
5.  **Production Hardening:** Rate limiting, audit logging, and input sanitization.

## Quick-Reference Cheat Sheet

### Core Concepts & Diagnostics
| Term / Code | Evaluation Stage | Core Question / Meaning | Technical Cause |
| :--- | :--- | :--- | :--- |
| **Authentication** | Step 1 | *"Who are you?"* | Validates signature and expiration of the JWT. |
| **Authorization** | Step 2 | *"What can you do?"* | Compares user claims/roles against endpoint rules. |
| **`401 Unauthorized`** | Authentication | *"Identity unknown."* | Token is missing, expired, or malformed. |
| **`403 Forbidden`** | Authorization | *"Permission denied."* | Valid token, but role check failed. (Expected security response). |

### ASP.NET Core Declarative Attributes
| Attribute Syntax | Authentication Check | Role Check | Failure Code |
| :--- | :--- | :--- | :--- |
| `[AllowAnonymous]` | Bypassed | None | N/A (Permits all traffic) |
| `[Authorize]` | Required (Valid JWT) | None | `401 Unauthorized` |
| `[Authorize(Roles = "Admin")]` | Required (Valid JWT) | Required (`Admin`) | `401` (No token) / `403` (Not Admin) |
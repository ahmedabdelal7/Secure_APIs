# Security Maturity Levels

## Overview
API security develops through progressive maturity levels, with each level addressing a different class of threats and security gaps. For the Student API, the path is **Open → Foundation (HTTPS + CORS) → Authenticated → Authorized → Production**, showing why security must be implemented in layers rather than relying on JWT alone.

## 1. Security Maturity: The Progression

| Level | Stage | Main Question | What It Provides |
|---|---|---|---|
| **0** | Open API | No security controls? | Anyone can call any endpoint. |
| **1** | Foundation | Is communication protected and browser access controlled? | HTTPS + CORS |
| **2** | Authenticated | Who is calling the API? | Identity verification with JWT |
| **3** | Authorized | What can this user do? | Role-based access control |
| **4** | Production | Is access safe under real-world conditions? | Ownership, session controls, abuse protection, and auditing |

## 2. Core Concepts by Level

### Level 0 — Open API
- No HTTPS enforcement, CORS policy, authentication, or authorization.
- Anyone can call endpoints, including destructive operations.
- Example: `DELETE /api/Students/5` works without login.
- **Risk:** The API is easy to build but easy to attack.

### Level 1 — Foundation: HTTPS + CORS
- **HTTPS (TLS):** Encrypts data in transit and helps protect credentials and tokens from interception and man-in-the-middle attacks when correctly configured.
- **CORS:** Controls which browser origins may make cross-origin requests and which methods/headers are allowed.
- **Important distinction:** HTTPS protects communication; CORS is a browser-enforced access policy. Neither identifies the user or replaces authentication.

### Level 2 — Authenticated API: JWT
Adds:
- Registration and password hashing.
- Login and JWT issuance.
- JWT authentication middleware.

**Result:** Anonymous requests to protected endpoints are rejected, commonly with `401 Unauthorized` when no valid credentials are supplied.

**Remaining gap:** Authentication proves identity, not permission. A logged-in student must not automatically gain administrator capabilities.

### Level 3 — Authorized API: Roles
Adds role-based authorization, such as `Student` and `Admin`, using `[Authorize]` and role restrictions such as `[Authorize(Roles = "Admin")]`.

**Result:** Students are restricted from admin-only operations; administrators can perform permitted administrative actions. A student attempting a forbidden operation typically receives `403 Forbidden`.

**Remaining gap:** Roles alone do not guarantee data ownership. A student might still try to access another student's record.

### Level 4 — Production-Ready API
Adds controls for real-world risks:

- **Ownership / policy-based authorization:** Users can access only resources they are entitled to access.
- **Refresh tokens, logout, and rotation:** Support controlled sessions and safer long-lived authentication.
- **Rate limiting:** Reduces brute-force attempts and abuse, especially on login and token-refresh endpoints.
- **Logging and auditing:** Help detect, investigate, and trace security-relevant activity.

**Result:** Reduces horizontal privilege escalation, improves session control, limits abuse, and provides accountability.

## 3. Quick-Reference Cheat Sheet

| Security Concept | Core Question / Purpose |
|---|---|
| **HTTPS** | Is data encrypted in transit? |
| **CORS** | Which browser origins may call the API? |
| **Authentication** | Who are you? |
| **JWT** | How does the API receive and validate identity claims? |
| **Authorization** | What are you allowed to do? |
| **Roles** | Which operations can a category of user perform? |
| **Ownership** | Are you allowed to access this specific resource? |
| **Rate Limiting** | Can repeated requests or abuse be controlled? |
| **Logging / Auditing** | Can security events be detected and traced? |

### Key Distinctions

- **401 Unauthorized:** Authentication is missing or invalid.
- **403 Forbidden:** The caller is authenticated but lacks permission.
- **Authentication ≠ Authorization:** Knowing who the user is does not determine what they may do.
- **Roles ≠ Ownership:** A role may allow an action generally, while ownership checks whether the user may act on a particular record.

**Maturity Path:**  
Open → HTTPS + CORS → JWT Authentication → Role Authorization → Ownership + Session Controls + Rate Limiting + Auditing

**Key Takeaway:** Build security in layers: protect communication, establish identity, restrict actions, then protect individual resources and monitor real-world abuse.
